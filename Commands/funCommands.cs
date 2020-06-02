using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Tutorial.Commands
{
    class funCommands : BaseCommandModule
    {
        [Command("dick")]
        public async Task Dick(CommandContext ctx)
        {
            var author = ctx.Message.Author.Id;

            if (author == 238471482009714688)
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
    }


}
