using Discord_Bot_Tutorial.Context;
using Discord_Bot_Tutorial.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord_Bot_Tutorial.Commands
{
    class Matchmaking : BaseCommandModule
    {
        [Command("mm")]
        public async Task MM(CommandContext ctx)
        {
            List<Queue> dpsTeam = new List<Queue>();
            List<Queue> tankTeam = new List<Queue>();
            List<Queue> supportTeam = new List<Queue>();

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
                        dpsTeam.Add(player);
                    }
                    else if (player.role == "tank")
                    {
                        tankTeam.Add(player);
                    }
                    else //player.role == support
                    {
                        supportTeam.Add(player);
                    }
                }

                //For each role team, find the highest and lowest value, pair them, then the remaining two are the last
                dpsTeam.OrderByDescending(i => i.queueSr);
                tankTeam.OrderByDescending(i => i.queueSr);
                supportTeam.OrderByDescending(i => i.queueSr);

                List<Queue> dpspair1 = new List<Queue>{ dpsTeam[0], dpsTeam[3] };
                List<Queue> dpspair2 = new List<Queue> { dpsTeam[1], dpsTeam[2] };

                List<Queue> tankpair1 = new List<Queue> { tankTeam[0], tankTeam[3] };
                List<Queue> tankpair2 = new List<Queue> { tankTeam[1], tankTeam[2] };

                List<Queue> supportpair1 = new List<Queue> { supportTeam[0], supportTeam[3] };
                List<Queue> supportpair2 = new List<Queue>{ supportTeam[1], supportTeam[2] };

                List<Queue> Team1 = dpspair1.Concat(tankpair1).Concat(supportpair1).ToList();
                List<Queue> Team2 = dpspair2.Concat(tankpair2).Concat(supportpair2).ToList();

                var Team1Average = Team1.Average(i => i.queueSr);
                var Team2Average = Team2.Average(i => i.queueSr);

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
                    + "\n" + $"Team 1 Average : {Team1Average, 10}"
                    + "\n" + $"Team 2 Average: { Team2Average, 10}" +
                    "```");
            }

        }
    }
}
