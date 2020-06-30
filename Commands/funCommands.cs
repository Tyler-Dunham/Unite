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

            int randomChoice = rnd.Next(0, sexualities.Length + 1);

            await ctx.Channel.SendMessageAsync($"{mention} is {sexualities[randomChoice]} {flushedEmoji}.");
        }

        
    }
}
