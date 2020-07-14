using Discord_Bot_Tutorial.Context;
using Discord_Bot_Tutorial.Models;
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
using System.Threading;

namespace Discord_Bot_Tutorial.Commands
{
    class usefulCommands : BaseCommandModule
    {
        [Command("tempban")]
        [RequireRoles(RoleCheckMode.Any, "Moderator", "Admin")]
        [Description("Temp ban a user. This gives them a role for a period of time that doesn't allow them to type in any channels.")]
        public async Task SoftBan(CommandContext ctx, DiscordMember user, TimeSpan banLength)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var straightJacket = ctx.Guild.Roles.Values.First(x => x.Name == "Straight Jacket");
                var bans = lite.Bans;
                ulong userID = user.Id;

                foreach (var member in bans)
                {
                    if (member.userID == userID)
                    {
                        member.unbanTime += banLength;
                        await ctx.Channel.SendMessageAsync($"User already temp banned. Added {banLength} to their ban time.");
                        await lite.SaveChangesAsync();
                        return;
                    }
                }

                var userBan = new Ban();
                userBan.userID = user.Id;
                userBan.userName = user.Username;
                userBan.banTime = DateTime.Now;
                userBan.unbanTime = userBan.banTime + banLength;
                bans.Add(userBan);
                await lite.SaveChangesAsync();

                await user.GrantRoleAsync(straightJacket);
                await ctx.Channel.SendMessageAsync($"{user.Username} has been temp banned until {userBan.unbanTime.ToString("MM/dd/yyy HH:mm")}");

                await Task.Delay(DateTime.Compare(userBan.banTime, userBan.unbanTime));

                await user.RevokeRoleAsync(straightJacket);
                
                foreach (var member in bans)
                {
                    if (member.userID == userID)
                    {
                        bans.Remove(member);
                    }
                }

                await ctx.Channel.SendMessageAsync($"{user.Mention} has been unbanned.");
            }
        }

        [Command("ban")]
        [RequireRoles(RoleCheckMode.Any, "Moderator", "Admin")]
        [Description("Ban a user.")]
        public async Task Ban(CommandContext ctx, DiscordMember user)
        {
            var server = ctx.Guild.Name;

            await user.BanAsync();

            await ctx.Channel.SendMessageAsync($"{user.Username} has been banned from {server}");
        }
    }
}
