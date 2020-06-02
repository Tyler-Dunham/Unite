using Discord_Bot_Tutorial.Context;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.EventHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Discord_Bot_Tutorial.Models;
using System.Threading.Channels;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Linq.Expressions;

namespace Discord_Bot_Tutorial.Commands
{
    class PUGCommands : BaseCommandModule
    {
        int dps = 0;
        int tank = 0;
        int support = 0;

        [Command("q")]
        [Aliases("queue")]
        [Description("Join the queue, in order to change roles, leave the queue first.")]
        public async Task Q(CommandContext ctx, string role = "view")
        {
            using (SqliteContext lite = new SqliteContext())
            {
                string[] responses = { "dps", "tank", "support" };

                var author = ctx.Message.Author.Id;
                var queue = lite.playerQueue;
                var allProfiles = lite.Profiles;


                if (role == "view")
                {
                    if (queue.Count() == 0)
                    {
                        await ctx.Channel.SendMessageAsync("There is no one in the queue.");
                        return;
                    }

                    string message = "List of players queued" + "```";
                    foreach (var player in queue)
                    {
                        message = message + "\n" + $"{player.userName,-25} : {player.role,10}";
                    }

                    await ctx.Channel.SendMessageAsync(message + "```");
                    return;

                }


                //Handles valid role responses
                //Checks if already in queue, if so, leaves and rejoins to avoid dupes
                //Keeps track of amount of each role
                //Tells you x/4 for each role after a valid role has been entered
                else if (responses.Contains(role.ToLower()))
                {
                    //Goes through each profile in allProfiles to see if it can find you 
                    //If it finds you, it add you to queue
                    foreach (var profile in allProfiles)
                    {
                        //Finds match in profiles
                        if (profile.userID == author)
                        {
                            //Checks if already in queue
                            if (profile.queue == true)
                            {
                                foreach (var player in queue)
                                {
                                    //Finds a match
                                    if (player.userID == author)
                                    {
                                        queue.Remove(player);
                                        queue.Add(player);

                                        await lite.SaveChangesAsync();

                                        await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} Added to queue. Role: {role}.");
                                    }
                                }
                            }

                            //If not in queue, add you to the queue.
                            profile.queue = true;
                            profile.role = role;

                            var counter = new Queue();

                            //If role == dps
                            if (role == "dps")
                            {
                                if (dps == 4)
                                {
                                    await ctx.Channel.SendMessageAsync("DPS slots are filled.");
                                    return;
                                }

                                profile.queueSr = profile.dps;
                                dps++;

                                if (dps == 4)
                                {
                                    await ctx.Channel.SendMessageAsync("4/4 DPS slots are filled.");
                                }
                            }

                            //If role == tank
                            else if (role == "tank")
                            {
                                if (tank == 4)
                                {
                                    await ctx.Channel.SendMessageAsync("Tank slots are filled.");
                                    return;
                                }

                                profile.queueSr = profile.tank;
                                tank++;

                                if (tank == 4)
                                {
                                    await ctx.Channel.SendMessageAsync("4/4 tank slots are filled.");
                                }
                            }

                            //If role == support
                            else
                            {
                                if (support == 4)
                                {
                                    await ctx.Channel.SendMessageAsync("Support slots are filled.");
                                    return;
                                }

                                profile.queueSr = profile.support;
                                support++;

                                if (support == 4)
                                {
                                    await ctx.Channel.SendMessageAsync("4/4 support slots are filled.");
                                }
                            }

                            var newPlayer = new Queue();
                            newPlayer.role = role;
                            newPlayer.userID = author;
                            newPlayer.userName = ctx.User.Username;
                            newPlayer.queueSr = profile.queueSr;

                            queue.Add(newPlayer);

                            await lite.SaveChangesAsync();

                            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} Added to queue. Role: {role}.");
                        }
                    }
                }

                else
                {
                    await ctx.Channel.SendMessageAsync("Not a valid role.");
                }
            }
        }

        [Command("leave")]
        [Aliases("l")]
        [Description("Leave the queue.")]
        public async Task Leave(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var author = ctx.Message.Author.Id;
                var allProfiles = lite.Profiles;
                var queue = lite.playerQueue;

                foreach (var player in queue)
                {
                    if (player.userID == author)
                    {
                        foreach (var profile in allProfiles)
                        {
                            if (profile.userID == author)
                            {
                                if (player.role == "dps")
                                {
                                    dps--;
                                }
                                if (player.role == "tank")
                                {
                                    tank--;
                                }
                                if (player.role == "support")
                                {
                                    support--;
                                }

                                profile.role = null;
                                profile.queue = false;
                                profile.queueSr = 0;
                                queue.Remove(player);

                                await lite.SaveChangesAsync();

                                await ctx.Channel.SendMessageAsync("You have left the queue");

                                return;
                            }
                        }
                    }
                }

                await ctx.Channel.SendMessageAsync($"{ ctx.User.Mention} you are not in queue");
            }
        }

        [Command("clear")]
        [Description("Clear the queue.")]
        public async Task Clear(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                var queue = lite.playerQueue;

                foreach (var profile in allProfiles)
                {
                    profile.role = null;
                    profile.queue = false;
                    profile.queueSr = 0;

                    await lite.SaveChangesAsync();
                }

                foreach (var player in queue)
                {
                    queue.Remove(player);

                    await lite.SaveChangesAsync();
                }

                dps = 0;
                tank = 0;
                support = 0;

                await ctx.Channel.SendMessageAsync("Queue has been cleared.");
            }
        }

        [Command("map")]
        [Description("Picks a map.")]
        public async Task PickMap(CommandContext ctx)
        {
            string[] maps = {"Blizzard World", "Busan", "Dorado", "Eichenwalde", "Hanamura",
                        "Havana", "Hollywood", "Ilios", "Junkertown", "Kings Row", "Lijiang Tower",
                    "Nepal", "Numbani", "Oasis", "Rialto", "Route 66", "Temple of Anubis",
                    "Volskaya Industries", "Watchpoint: Gibraltar"};


            Random rnd = new Random();
            int i = rnd.Next(0, maps.Length + 1);

            string map = maps[i];

            await ctx.Channel.SendMessageAsync($"Map: {map}");
        }
    }
}
