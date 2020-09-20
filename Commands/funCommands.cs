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

namespace Discord_Bot_Tutorial.Commands
{
    class funCommands : BaseCommandModule
    {
        [Command("time")]
        [Description("Shows current time.")]
        public async Task ShowCurrentTime(CommandContext ctx)
        {
            string time = DateTime.Now.ToString("HH:mm");
            await ctx.Channel.SendMessageAsync($"{time} EST");
        }

        [Command("myroles")]
        [Description("Show your current roles.")]
        public async Task MyRoles(CommandContext ctx)
        {
            var myRoles = ctx.Member.Roles.ToList();
            string rolesMessage = " | ";
            
            foreach (var x in myRoles)
            {
                rolesMessage += x.Name + " | ";
            }

            await ctx.Channel.SendMessageAsync(rolesMessage);
        }

        [Command("puggers")]
        [Description("Tell you how many people are online that have the 'Puggers' role.")]
        public async Task OnlinePuggers(CommandContext ctx)
        {
            var puggersRole = ctx.Guild.Roles.Values.First(x => x.Name == "Puggers");
            var allMembers = ctx.Guild.Members.ToList();
            ulong pugBotId = 714654129275797585;
            int onlinePuggers = 0;
            int idlePuggers = 0;
            int dndPuggers = 0;
            
            foreach (var x in allMembers)
            {
                if (x.Value.Presence != null)
                {
                    if (x.Value.Presence.Status == UserStatus.Online && x.Value.Roles.Contains(puggersRole) && x.Value.Id != pugBotId)
                    {
                        onlinePuggers++;
                    }
                    else if (x.Value.Presence.Status == UserStatus.Idle && x.Value.Roles.Contains(puggersRole))
                    {
                        idlePuggers++;
                    }
                    else if (x.Value.Presence.Status == UserStatus.DoNotDisturb && x.Value.Roles.Contains(puggersRole))
                    {
                        dndPuggers++;
                    }
                }
            }

            await ctx.Channel.SendMessageAsync("```" + "\n" + "\n" +
                                               $"{"Online Puggers", -14} : {onlinePuggers.ToString(), 2}" + "\n" + 
                                               $"{"Idle Puggers", -14} : {idlePuggers.ToString(), 2}" + "\n" +
                                               $"{"DND Puggers", -14} : {dndPuggers.ToString(), 2}" + "```");
        }
        
        [Command("fizzbuzz")]
        [Description("Fizzbuzz because why not.")]
        public async Task FizzBuzz(CommandContext ctx, int i)
        {
            if (i % 15 == 0)
            {
                await ctx.Channel.SendMessageAsync("FizzBuzz");
            }
            else if (i % 3 == 0)
            {
                await ctx.Channel.SendMessageAsync("Fizz");
            }
            else if (i % 5 == 0)
            {
                await ctx.Channel.SendMessageAsync("Buzz");
            }
            else
            {
                await ctx.Channel.SendMessageAsync("No fizz or buzz PepeHands.");
            }
        }

        
    }
}
