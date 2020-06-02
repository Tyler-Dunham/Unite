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
        bool game = false;

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
        static List<Queue> Team1 { get {return dpspair1.Concat(tankpair1).Concat(supportpair1).ToList(); } }
        static List<Queue> Team2 { get { return dpspair2.Concat(tankpair2).Concat(supportpair2).ToList(); } }

        [Command("mm")]
        [Description("Generates fairly balanced teams.")]
        public async Task MM(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                var queue = lite.playerQueue;

                if (queue.Count() < 12)
                {
                    await ctx.Channel.SendMessageAsync($"Not enough players queued. Need {12 - queue.Count()} players.");
                    return;
                }

                //Adding player to role team
                foreach (var player in queue)
                {
                    if (player.role == "dps")
                    {
                        dpsTeamList.Add(player);
                    }
                    else if (player.role == "tank")
                    {
                        tankTeamList.Add(player);
                    }
                    else //player.role == support
                    {
                        supportTeamList.Add(player);
                    }
                }

                //For each role team, find the highest and lowest value, pair them, then the remaining two are the last


                var Team1Average = Math.Round(Team1.Average(i => i.queueSr));
                var Team2Average = Math.Round(Team2.Average(i => i.queueSr));

                string[] maps = {"Blizzard World", "Busan", "Dorado", "Eichenwalde", "Hanamura", 
                        "Havana", "Hollywood", "Ilios", "Junkertown", "Kings Row", "Lijiang Tower", 
                    "Nepal", "Numbani", "Oasis", "Rialto", "Route 66", "Temple of Anubis", 
                    "Volskaya Industries", "Watchpoint: Gibraltar"};


                Random rnd = new Random();
                int i = rnd.Next(0, maps.Length + 1);

                string map = maps[i];

                string Team1Message = "```" + $"Team 1: ";
                string Team2Message = "Team 2: ";

                foreach (var player in Team1)
                {
                    Team1Message = Team1Message + "\n" + $"{player.userName,-25} : {player.role,10} : {player.queueSr}";
                }

                foreach (var player in Team2)
                {
                    Team2Message = Team2Message + "\n" + $"{player.userName,-25} : {player.role,10} : {player.queueSr}";
                }

                await ctx.Channel.SendMessageAsync(Team1Message + "\n" + "\n" + Team2Message
                    + "\n" + "\n" + $"Team 1 Average : {Team1Average, 10}"
                    + "\n" + $"Team 2 Average: { Team2Average, 10}"
                    + "\n" + "\n" + $"{map}" +
                    "```");

                game = true;
            }
        }

        [Command("win")]
        public async Task Win(CommandContext ctx, int winner)
        {
            if (game == false)
            {
                await ctx.Channel.SendMessageAsync("A game is not in progress.");
                return;
            }
            if (winner != 0 || winner != 1 || winner != 2)
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
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank + 50;
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support + 50;
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
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank - 50;
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support - 50;
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
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank + 50;
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support + 50;
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
                                }
                                else if (profile.role == "tank")
                                {
                                    profile.tank = profile.tank - 50;
                                }
                                else if (profile.role == "support")
                                {
                                    profile.support = profile.support - 50;
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

                //Send Confirmation message
                await ctx.Channel.SendMessageAsync("Sr's have been updated.");
            }
        }
    }
}
