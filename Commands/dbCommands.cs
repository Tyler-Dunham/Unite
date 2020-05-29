using Discord_Bot_Tutorial.Context;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Discord_Bot_Tutorial.Commands
{
    class dbCommands : BaseCommandModule
    {
        [Command("migrate")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task MigrateLite(CommandContext ctx)
        {
            try
            {

                Console.WriteLine("***migrating...***");

                await using SqliteContext lite = new SqliteContext();

                if (lite.Database.GetPendingMigrationsAsync().Result.Any())
                {
                    await lite.Database.MigrateAsync();
                }

                await ctx.Channel.SendMessageAsync($"Migration complete.");
            }

            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync($"Error: {e}");
            }
        }
    }
}
