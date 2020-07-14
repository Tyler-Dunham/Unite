using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Discord_Bot_Tutorial.Commands
{
    class funCommands : BaseCommandModule
    {
        [Command("dick")]
        public async Task Dick(CommandContext ctx)
        {
            var author = ctx.Message.Author.Id;
            ulong panda = 238471482009714688;

            if (author == panda)
            {
                await ctx.Channel.SendMessageAsync($"No.");
                return;
            }

            var mention = ctx.User.Mention;

            Random rnd = new Random();

            double dickSize = rnd.Next(0, 12) + Math.Round(rnd.NextDouble(), 2);

            var flushedEmoji = DiscordEmoji.FromName(ctx.Client, ":flushed:");

            await ctx.Channel.SendMessageAsync($"{mention} has a {dickSize} inch schlong {flushedEmoji}");
        }

        [Command("gay")]
        public async Task Gay(CommandContext ctx)
        {
            var flushedEmoji = DiscordEmoji.FromName(ctx.Client, ":flushed:");
            var mention = ctx.User.Mention;
            ulong panda = 238471482009714688;
            var author = ctx.Message.Author.Id;

            if (author == panda)
            {
                await ctx.Channel.SendMessageAsync($"{mention} is bi {flushedEmoji}.");
                return;
            }

            string[] sexualities = { "straight", "gay", "bi", "trans", "pansexual", "very ugly", "10/10 hottie", "asexual" };

            Random rnd = new Random();

            int randomChoice = rnd.Next(sexualities.Length + 1);

            await ctx.Channel.SendMessageAsync($"{mention} is {sexualities[randomChoice]} {flushedEmoji}.");
        }

        [Command("time")]
        [Description("Shows current time.")]
        public async Task ShowCurrentTime(CommandContext ctx)
        {
            string time = DateTime.Now.ToString("HH:mm");
            await ctx.Channel.SendMessageAsync($"{time} EST");
        }

        [Command("myroles")]
        [Description("Show your current roles.")]
        public async Task MyRoles(CommandContext ctx)
        {
            var myRoles = ctx.Member.Roles.ToList();
            string rolesMessage = " | ";
            
            foreach (var x in myRoles)
            {
                rolesMessage += x.Name + " | ";
            }

            await ctx.Channel.SendMessageAsync(rolesMessage);
        }

        [Command("puggers")]
        [Description("Tell you how many people are online that have the 'Puggers' role.")]
        public async Task OnlinePuggers(CommandContext ctx)
        {
            var puggersRole = ctx.Guild.Roles.Values.First(x => x.Name == "Puggers");
            var allMembers = ctx.Guild.Members.ToList();
            ulong pugBotId = 714654129275797585;
            int onlinePuggers = 0;
            int idlePuggers = 0;
            int dndPuggers = 0;
            
            foreach (var x in allMembers)
            {
                if (x.Value.Presence != null)
                {
                    if (x.Value.Presence.Status == UserStatus.Online && x.Value.Roles.Contains(puggersRole) && x.Value.Id != pugBotId)
                    {
                        onlinePuggers++;
                    }
                    else if (x.Value.Presence.Status == UserStatus.Idle && x.Value.Roles.Contains(puggersRole))
                    {
                        idlePuggers++;
                    }
                    else if (x.Value.Presence.Status == UserStatus.DoNotDisturb && x.Value.Roles.Contains(puggersRole))
                    {
                        dndPuggers++;
                    }
                }
            }

            await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" +
                                               $"{"Online Puggers", -14} : {onlinePuggers.ToString(), 2}" + "\n" + 
                                               $"{"Idle Puggers", -14} : {idlePuggers.ToString(), 2}" + "\n" +
                                               $"{"DND Puggers", -14} : {dndPuggers.ToString(), 2}" + "```");
        }
        
        [Command("fizzbuzz")]
        [Description("Fizzbuzz because why not.")]
        public async Task FizzBuzz(CommandContext ctx, int i)
        {
            if (i % 15 == 0)
            {
                await ctx.Channel.SendMessageAsync("FizzBuzz");
            }
            else if (i % 3 == 0)
            {
                await ctx.Channel.SendMessageAsync("Fizz");
            }
            else if (i % 5 == 0)
            {
                await ctx.Channel.SendMessageAsync("Buzz");
            }
            else
            {
                await ctx.Channel.SendMessageAsync("No fizz or buzz PepeHands.");
            }
        }

        
    }
}
