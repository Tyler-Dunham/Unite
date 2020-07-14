using Discord_Bot_Tutorial.Commands;
using Discord_Bot_Tutorial.Context;
using Discord_Bot_Tutorial.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Tutorial
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextConfiguration Commands { get; private set; }
        
        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;
            Client.Heartbeated += CheckBanDatabaseOnHeartbeat;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {configJson.Prefix},
                EnableDms = false,
                EnableMentionPrefix = true,
                IgnoreExtraArguments = true,
            };

            var Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<profileCommands>();
            Commands.RegisterCommands<PUGCommands>();
            Commands.RegisterCommands<dbCommands>();
            Commands.RegisterCommands<Matchmaking>();
            Commands.RegisterCommands<funCommands>();
            Commands.RegisterCommands<usefulCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private async Task CheckBanDatabaseOnHeartbeat(HeartbeatEventArgs e)
        {
            if (Client.Guilds.Count == 0)
            {
                return;
            }

            using (SqliteContext lite = new SqliteContext())
            {
                var bans = lite.Bans;
                DateTime currentTime = DateTime.Now;
                var guild = Client.Guilds.Values.FirstOrDefault(x => x.Name == "Bot Testing");
                var members = guild.Members.Values;
                var roles = guild.Roles.Values;
                var straightJacket = roles.First(x => x.Name == "Straight Jacket");
                List<ulong> userIDs = new List<ulong>();

                foreach (var user in bans)
                {
                    if (user.unbanTime <= currentTime)
                    {
                        userIDs.Add(user.userID);
                        lite.Remove(user);
                        await lite.SaveChangesAsync();
                    }
                }

                foreach (var member in members)
                {
                    foreach (var id in userIDs)
                    {
                        if (member.Id == id)
                        {
                            await member.RevokeRoleAsync(straightJacket);
                        }
                    }
                }
            }
        }


    }
}
