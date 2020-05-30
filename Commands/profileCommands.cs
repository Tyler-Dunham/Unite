using Discord_Bot_Tutorial.Context;
using Discord_Bot_Tutorial.Models;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Tutorial.Commands
{
    public class profileCommands : BaseCommandModule
    {
        [Command("profile")]
        [Description("Add all SRs .profile <dpssr> <tanksr> <supportsr>")]
        public async Task ProfileCreation(CommandContext ctx, int dps, int tank, int support)
        {

            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                ulong author = ctx.Message.Author.Id;

                foreach (var _profile in allProfiles)
                {
                    if (_profile.userID == author)
                    {
                        _profile.dps = dps;
                        _profile.tank = tank;
                        _profile.support = support;
                        _profile.userName = ctx.Message.Author.Username;
                        _profile.queue = false;

                        await lite.SaveChangesAsync();

                        await ctx.Channel.SendMessageAsync("Your profile has been updated.");
                        return;
                    }
                }

                var profile = new Profile();
                profile.userID = ctx.Message.Author.Id;
                profile.userName = ctx.Message.Author.Username;
                profile.dps = dps;
                profile.tank = tank;
                profile.support = support;
                profile.queue = false;

                lite.Profiles.Add(profile);
                await lite.SaveChangesAsync();

                await ctx.Channel.SendMessageAsync("Added your profile.");
            }
        }

        [Command("dps")]
        [Description("Shows dps sr attached to your profile.")]
        public async Task Dps(CommandContext ctx, int dps)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                ulong author = ctx.Message.Author.Id;

                foreach (var _profile in allProfiles)
                {
                    if (_profile.userID == author)
                    {
                        _profile.dps = dps;
                        _profile.userName = ctx.Message.Author.Username; 
                        _profile.queue = false;

                        await lite.SaveChangesAsync();

                        await ctx.Channel.SendMessageAsync("DPS sr updated.");
                        return;
                    }
                }

                var profile = new Profile();
                profile.userID = ctx.Message.Author.Id;
                profile.userName = ctx.Message.Author.Username;
                profile.dps = dps;
                profile.queue = false;

                lite.Profiles.Add(profile);
                await lite.SaveChangesAsync();

                await ctx.Channel.SendMessageAsync("Added your profile with only a DPS value.");
            }
        }

        [Command("tank")]
        [Description("Shows tank sr attached to your profile.")]
        public async Task Tank(CommandContext ctx, int tank)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                ulong author = ctx.Message.Author.Id;

                foreach (var _profile in allProfiles)
                {
                    if (_profile.userID == author)
                    {
                        _profile.tank = tank;
                        _profile.userName = ctx.Message.Author.Username;
                        _profile.queue = false;

                        await lite.SaveChangesAsync();

                        await ctx.Channel.SendMessageAsync("Tank sr updated.");
                        return;
                    }
                }

                var profile = new Profile();
                profile.userID = ctx.Message.Author.Id;
                profile.userName = ctx.Message.Author.Username;
                profile.tank = tank;
                profile.queue = false;

                lite.Profiles.Add(profile);
                await lite.SaveChangesAsync();

                await ctx.Channel.SendMessageAsync("Added your profile with only a tank value.");
            }
        }

        [Command("support")]
        [Description("Shows support sr attached to your profile.")]
        public async Task Support(CommandContext ctx, int support)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                ulong author = ctx.Message.Author.Id;

                foreach (var _profile in allProfiles)
                {
                    if (_profile.userID == author)
                    {
                        _profile.support = support;
                        _profile.userName = ctx.Message.Author.Username;
                        _profile.queue = false;

                        await lite.SaveChangesAsync();

                        await ctx.Channel.SendMessageAsync("Support sr updated.");
                        return;
                    }
                }

                var profile = new Profile();
                profile.userID = ctx.Message.Author.Id;
                profile.userName = ctx.Message.Author.Username;
                profile.support = support;
                profile.queue = false;

                lite.Profiles.Add(profile);
                await lite.SaveChangesAsync();

                await ctx.Channel.SendMessageAsync("Added your profile with only a support value.");
            }
        }

        [Command("sr")]
        [Description("Shows all sr's attached to your profile.")]
        public async Task Sr(CommandContext ctx)
        {
            using (SqliteContext lite = new SqliteContext())
            {
                var allProfiles = lite.Profiles;
                ulong author = ctx.Message.Author.Id;
                string mention = ctx.User.Mention;

                foreach (var profile in allProfiles)
                {
                    if (profile.userID == author)
                    {
                        await ctx.Channel.SendMessageAsync($"{mention}" + "\n" + $"DPS: {profile.dps}" + "\n" + $"Tank: {profile.tank}" + "\n" + $"Support: {profile.support}");
                        return;
                    }
                }

                await ctx.Channel.SendMessageAsync("No profile found.");
            }
        }
    }
}
