using Discord_Bot_Tutorial.Context;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Tutorial.Commands
{
    class Matchmaking : BaseCommandModule
    {
        [Command("mm")]
        public async Task MM(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                var queue = lite.playerQueue;

                if (queue.Count() < 12)
                {
                    await ctx.Channel.SendMessageAsync($"Not enough players queued. Need {12 - queue.Count()} players.");
                }


            }
        }


    }
}
