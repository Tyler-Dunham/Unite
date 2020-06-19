using Discord_Bot_Tutorial.Context;
using Discord_Bot_Tutorial.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord_Bot_Tutorial.Commands
{
    class Matchmaking : BaseCommandModule
    {
        public static bool game = false;

        static List<Queue> dpsTeamList = new List<Queue>();
        static List<Queue> tankTeamList = new List<Queue>();
        static List<Queue> supportTeamList = new List<Queue>();

        static Queue[] dpsTeam { get { return dpsTeamList.OrderByDescending(i => i.queueSr).ToArray(); } }
        static Queue[] tankTeam { get { return tankTeamList.OrderByDescending(i => i.queueSr).ToArray(); } }
        static Queue[] supportTeam { get { return supportTeamList.OrderByDescending(i => i.queueSr).ToArray(); } }

        static List<Queue> dpspair1 { get { Queue[] team = dpsTeam; return new List<Queue> { team[0], team[3] }; } }
        static List<Queue> dpspair2 { get { Queue[] team = dpsTeam; return new List<Queue> { team[1], team[2] }; } }
        static List<Queue> tankpair1 { get { Queue[] team = tankTeam; return new List<Queue> { team[0], team[3] }; } }
        static List<Queue> tankpair2 { get { Queue[] team = tankTeam; return new List<Queue> { team[1], team[2] }; } }
        static List<Queue> supportpair1 { get { Queue[] team = supportTeam; return new List<Queue> { team[0], team[3] }; } }
        static List<Queue> supportpair2 { get { Queue[] team = supportTeam; return new List<Queue> { team[1], team[2] }; } }
        static List<Queue> Team1 { get {return tankpair1.Concat(dpspair2).Concat(supportpair2).ToList(); } }
        static List<Queue> Team2 { get { return tankpair2.Concat(dpspair1).Concat(supportpair1).ToList(); } }

        [Command("mm")]
        [Description("Generates fairly balanced teams.")]
        public async Task MM(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                var queue = lite.playerQueue;
                int queueCount = 12;

                if (queue.Count() < 12)
                {
                    await ctx.Channel.SendMessageAsync($"Not enough players queued. Need {12 - queue.Count()} players.");
                    return;
                }

                while (queueCount != 0)
                {
                    //Adding player to role team
                    foreach (var player in queue)
                    {
                        if (player.role == "dps")
                        {
                            dpsTeamList.Add(player);
                            queueCount--;
                        }
                        else if (player.role == "tank")
                        {
                            tankTeamList.Add(player);
                            queueCount--;
                        }
                        else //player.role == support
                        {
                            supportTeamList.Add(player);
                            queueCount--;
                        }
                    }
                }

                //For each role team, find the highest and lowest value, pair them, then the remaining two are the last

                string[] maps = {"Blizzard World", "Busan", "Dorado", "Eichenwalde", "Hanamura", 
                        "Havana", "Hollywood", "Ilios", "Junkertown", "Kings Row", "Lijiang Tower", 
                    "Nepal", "Numbani", "Oasis", "Rialto", "Route 66", "Temple of Anubis", 
                    "Volskaya Industries", "Watchpoint: Gibraltar"};

                var Team1Average = Math.Round(Team1.Average(i => i.queueSr));
                var Team2Average = Math.Round(Team2.Average(i => i.queueSr));

                Random rnd = new Random();
                int i = rnd.Next(0, maps.Length + 1);

                string map = maps[i];

                string Team1Message = "```" + $"Team 1: ";
                string Team2Message = "Team 2: ";

                foreach (var player in Team1)
                {
                    Team1Message = Team1Message + "\n" + $"{player.userName,-25} : {player.role,10} : {player.queueSr}";
                    player.Team = 1;
                    await lite.SaveChangesAsync();
                }

                foreach (var player in Team2)
                {
                    Team2Message = Team2Message + "\n" + $"{player.userName,-25} : {player.role,10} : {player.queueSr}";
                    player.Team = 2;
                    await lite.SaveChangesAsync();
                }

                await ctx.Channel.SendMessageAsync(Team1Message + "\n" + "\n" + Team2Message
                    + "\n" + "\n" + $"Team 1 Average : {Team1Average, 10}"
                    + "\n" + $"Team 2 Average: { Team2Average, 10}"
                    + "\n" + "\n" + $"{map}" +
                    "```");

                game = true;
            }
            return;
        }

        [Command("win")]
        public async Task Win(CommandContext ctx, int winner)
        {
            if (game == false)
            {
                await ctx.Channel.SendMessageAsync("A game is not in progress.");
                return;
            }
            if (winner != 0 && winner != 1 && winner != 2)
            {
                await ctx.Channel.SendMessageAsync("Not a valid winner.");
                return;
            }

            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                var queue = lite.playerQueue;

                //Draw
                if (winner == 0)
                {
                    await ctx.Channel.SendMessageAsync("The game was a tie.");
                    return;
                }
                //Team 1 wins
                else if (winner == 1)
                {
                    foreach (var profile in allProfiles)
                    {
                        foreach (var player in Team1)
                        {
                            if (profile.userID == player.userID)
                            {
                                if (profile.role == "dps")
                                {
                                    profile.dps = profile.dps + 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank + 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support + 50;
                                    await lite.SaveChangesAsync();
                                }
                            }
                        }

                        foreach (var player in Team2)
                        {
                            if (profile.userID == player.userID)
                            {
                                if (profile.role == "dps")
                                {
                                    profile.dps = profile.dps - 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank - 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support - 50;
                                    await lite.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }

                //Team 2 wins
                else
                {
                    foreach (var profile in allProfiles)
                    {
                        foreach (var player in Team2)
                        {
                            if (profile.userID == player.userID)
                            {
                                if (profile.role == "dps")
                                {
                                    profile.dps = profile.dps + 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank + 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support + 50;
                                    await lite.SaveChangesAsync();
                                }
                            }
                        }

                        foreach (var player in Team1)
                        {
                            if (profile.userID == player.userID)
                            {
                                if (profile.role == "dps")
                                {
                                    profile.dps = profile.dps - 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank - 50;
                                    await lite.SaveChangesAsync();
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support - 50;
                                    await lite.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }

                //Clear teams
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
                foreach (var player in dpsTeamList)
                {
                    dpsTeamList.Remove(player);
                }
                foreach (var player in tankTeamList)
                {
                    tankTeamList.Remove(player);
                }
                foreach (var player in supportTeamList)
                {
                    supportTeamList.Remove(player);
                }
                foreach (var player in Team1)
                {
                    player.Team = 0;
                }
                foreach (var player in Team2)
                {
                    player.Team = 0;
                }

                game = false;

                //Send Confirmation message
                await ctx.Channel.SendMessageAsync("Sr's have been updated.");
            }
        }
        [Command("mtd")]
        [RequireRoles(RoleCheckMode.Any, "Mover")]
        [Description("Moves all players in Team 1 and Team 2 back to draft channel.")]
        public async Task MTD(CommandContext ctx)
        {
            var team1Channel = ctx.Guild.Channels.Values.First(x => x.Name == "Team 1");
            var team2Channel = ctx.Guild.Channels.Values.First(x => x.Name == "Team 2");
            var draftChannel = ctx.Guild.Channels.Values.First(x => x.Name == "Draft Channel");

            var team1Users = team1Channel.Users;
            var team2Users = team2Channel.Users;

            var moveUsersTasks = new List<Task>();
            var allUsers = team1Users.Union(team2Users);
            moveUsersTasks.AddRange(allUsers.Select(member => draftChannel.PlaceMemberAsync(member)));
            await Task.WhenAll(moveUsersTasks);
        }

        [Command("mtt")]
        [Description("Move players to their respective team.")]
        public async Task MTT(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                if (game == false)
                {
                    await ctx.Channel.SendMessageAsync("A game is not in progress, no teams are assigned.");
                    return;
                }

                var allProfiles = lite.Profiles;
                var Queue = lite.playerQueue;
                var team1Channel = ctx.Guild.Channels.Values.First(x => x.Name == "Team 1");
                var team2Channel = ctx.Guild.Channels.Values.First(x => x.Name == "Team 2");
                var draftChannel = ctx.Guild.Channels.Values.First(x => x.Name == "Draft Channel");
                var draftChannelUsers = draftChannel.Users;

                foreach (var user in draftChannelUsers)
                {
                    foreach (var player in Queue)
                    {
                        if (user.Id == player.userID && player.Team == 1)
                        {
                            await team1Channel.PlaceMemberAsync(user);
                        }
                        else if (user.Id == player.userID && player.Team == 2)
                        {
                            await team2Channel.PlaceMemberAsync(user);
                        }
                    }
                }

                await ctx.Channel.SendMessageAsync("Players have been moved.");
            }
        }
    }
}
