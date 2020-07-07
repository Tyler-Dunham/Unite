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
using System.Text;
using System.Threading.Tasks;
using Discord_Bot_Tutorial.Models;
using System.Threading.Channels;
using System.Threading;
using System.Linq.Expressions;
using DSharpPlus.Net.Models;

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

        [Command("status")]
        [Description("Queue Status")]
        public async Task Status(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var author = ctx.Message.Author.Id;
                var allProfiles = lite.Profiles;
                var mention = ctx.User.Mention;

                foreach (var profile in allProfiles)
                {
                    if (profile.userID == author)
                    {
                        if (profile.queue == true)
                        {
                            await ctx.Channel.SendMessageAsync($"{mention} You are in queue for {profile.role}.");
                            return;
                        }
                    }
                }

                await ctx.Channel.SendMessageAsync($"{mention} You are not in queue.");
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
            int i = rnd.Next(0, maps.Length);

            string map = maps[i];

            await ctx.Channel.SendMessageAsync($"{map}");
        }

        [Command("coin")]
        [Description("Flips a coin")]
        public async Task FlipCoin(CommandContext ctx)
        {
            string[] coin = { "Heads", "Tails" };

            Random rnd = new Random();
            int flip = rnd.Next(0, coin.Length);

            await ctx.Channel.SendMessageAsync($"{coin[flip]}");
        }

        [Command("schedule")]
        [RequireRoles(RoleCheckMode.Any, "Scheduler")]
        [Description("Schedule a pug")]
        public async Task Schedule(CommandContext ctx, TimeSpan pugTimeSpan)
        {
            var thumbsUpEmoji = DiscordEmoji.FromName(ctx.Client, ":+1:");
            var puggers = ctx.Guild.Roles.Values.First(x => x.Name == "Puggers");
            string pugTime = Convert.ToString(DateTime.Now.Add(pugTimeSpan).ToString("HH:mm"));

            var pugScheduleMessage = await ctx.Channel.SendMessageAsync($"{puggers.Mention} Pugs scheduled for {pugTime} EST");
            await pugScheduleMessage.CreateReactionAsync(thumbsUpEmoji);
        }

        [Command("captains")]
        [Description("Pick 2 random captains from draft channel.")]
        public async Task Captains(CommandContext ctx)
        {
            var draftChannel = ctx.Guild.Channels.Values.First(x => x.Name == "Draft Channel");
            var draftChannelUsers = draftChannel.Users.ToList();

            Random rnd = new Random();

            int i = rnd.Next(draftChannelUsers.Count);
            int x = rnd.Next(draftChannelUsers.Count);

            while (x == i)
            {
                x = rnd.Next(draftChannelUsers.Count);
            }

            var captainOne = draftChannelUsers[i];
            var captainTwo = draftChannelUsers[x];

            await ctx.Channel.SendMessageAsync($"Your captains are: {captainOne.Mention} and {captainTwo.Mention}.");
        }

        [Command("how")]
        [Description("Explains how some commands work.")]
        public async Task How(CommandContext ctx, string command)
        {
            string[] commands = { "profile", "sr", "mm", "how", "captains", "win", "q", "mtt", "mtd" };

            if (!commands.Contains(command.ToLower()))
            {
                await ctx.Channel.SendMessageAsync("That command is not valid or not supported yet.");
                return;
            }

            if (command.ToLower() == "profile")
            {
                string howProfileWorks = "This command is made by searching through the database of profiles using your discord ID to see if you are already there. " + 
                                         "If not, it will make a new profile with the values you provided. " +
                                         "If it finds you, it will take the values you put and update them. " + 
                                         "The dps, tank, and support commands work the same way except they search for the specified value rather than " +
                                         "just an entire profile.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howProfileWorks + "```");
                return;
            }
            
            else if (command.ToLower() == "sr")
            {
                string howSrWorks = "This command is made by searching for your profile in the profile database. " +
                                    "If you are found, it will display the sr values you have attached to that profile. " + 
                                    "If you are not found, it will tell you there are no values and you need to add values to see them.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howSrWorks + "```");
                return;
            } 
            
            else if (command.ToLower() == "mm")
            {
                string howMmWorks = "This command is made by taking the full queue and putting each player into a list one by one. " +
                    "There are many lists. One for dps, one for tank, one for support. " +
                    "Eventually you get funneled into role pairs. This works by constantly making new pairs until all teams are full. " +
                    "Once the teams are full, pairs are made by taking the highest and lowest sr and pairing them and taking the middle 2 and pairing them. " +
                    "Then teams are made by having a bias of which role can carry harder. " + 
                    "Tanks tend to have more carry power than the other roles at higher ranks, so team 1 has the higher tank pair but the lower dps and support pair. " + 
                    "Team 2 will have the lower tank pair, but the higher dps and support pair. This makes balanced teams and fair role balancing.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howMmWorks + "```");
                return;
            }  
            
            else if (command.ToLower() == "how")
            {
                string howHowWorks = "This command is quite simple and there is not much error handling to do. " +
                                     "It searches for the command you entered and if it is supported, it will give you a code block " + 
                                     "that you see now **the dark box** that sends a message. This message is what I wrote to describe how I made the command.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howHowWorks + "```");
                return;
            }  
            
            else if (command.ToLower() == "captains")
            {
                string howCaptainsWorks = "This command is made by searching through the draft channel and picking 2 random people. It is quite simple. " +
                                          "Deeper though, I find the channel 'Draft Channel' with a method of the library I'm using to do this. " +
                                          "That method involves lambdas as well and looks like this: " + "\n" + "\n" + 
                                          "var draftChannel = ctx.Guild.Channels.Values.First(x => x.Name == 'Draft Channel');" + "\n" + "\n" +
                                          "Once I find the channel, I get the users of the channel using another method. That method looks like this: " + "\n" + "\n" + 
                                          "var draftChannelUsers = draftChannel.Users.ToList(). " + "\n" + "\n" + 
                                          "It is that easy. I convert it to a list so I am able to pick out random indexes. I can also use an array, but I chose a list.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howCaptainsWorks + "```");
                return;
            }  
            
            else if (command.ToLower() == "win")
            {
                string howWinWorks = "This command is one of the longer ones. It is not complicated, it just has to look for a lot of things and reset a lot of things. " + 
                                     "You enter a winner and it searches for each player on the winning team and grants them sr towards the role they played as. " + 
                                     "It also does this for the losing team, removing sr for the role they played as. " + 
                                     "Once this is done, it then clears the queue and clears the teams. " +
                                     "It then sets the game boolean (true or false) to false, to signal a game is not in progress.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howWinWorks + "```");
                return;
            }  
            
            else if (command.ToLower() == "q")
            {
                string howQueueWorks = "This is one of the more complicated commands that I believe still has a bug in there. " + 
                                       "This command works by taking the role you queue for. It then sets that as your 'Queue role' so we can have sr consquences " + 
                                       "later. It then checks if you are in queue. If you are, it doesn't do anything. If you aren't it will add you to the queue. " + 
                                       "It checks to see if you are in queue by searching through the queue database. " + 
                                       "The bug that remains is if you are in queue but then queue for a different role. Until I decide I want to fix that, you " + 
                                       "will have to leave the queue then rejoin it with the role you want.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howQueueWorks + "```");
                return;
            }  
            
            else if (command.ToLower() == "mtt")
            {
                string howMttWorks = "This command is quite simple. It takes the same code snippet I show in the '.how captains' command and does the same for " + 
                                     "the team 1 and team 2 channels. It then checks for each person in the draft channel if they are on a team. If so, it " + 
                                     "identifies the team by searching through the database that shows your team and places you in the correct channel. " +
                                     "If you are not on a team, it will not move you from the draft channel.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howMttWorks + "```");
                return;
            }  
            
            else if (command.ToLower() == "mtd")
            {
                string howMtdWorks = "This command works very similarly to the MTT command. " +
                                     "It goes through everyone in the team 1 and 2 channels and move them to the draft channel. " +
                                     "If you are not in either of those channels, you will not be moved.";

                await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" + howMtdWorks + "```");
                return;
            }
        }

        [Command("howmany")]
        [Description("Tells you how many more you need for pugs.")]
        public async Task HowMany(CommandContext ctx)
        {
            var draftChannel = ctx.Guild.Channels.Values.First(x => x.Name == "Draft Channel");
            var draftChannelUsersCount = draftChannel.Users.Count();

            if (draftChannelUsersCount >= 12)
            {
                await ctx.Channel.SendMessageAsync("Why are you doing this, you have enough.");
                return;
            }

            await ctx.Channel.SendMessageAsync($"You need {12 - draftChannelUsersCount} more people.");
        }


    }
}
