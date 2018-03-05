using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TheNewBot.Examples
{
    // MUST BE PUBLIC
    // No prefix commands just a '!'
    public class NoPrefixCommands : ModuleBase
    {
        // usage: !userinfo skeivy --> Skeivy#2201
        [Command("userinfo"), Summary("Returns info about the current user, or the user parameter, if one passed.")]
        // Aliass that can be used instead of userinfo
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IUser user = null)
        {
            // userinfo = user unless it is null, otherwise user = CurrentUser
            var userInfo = user ?? Context.Client.CurrentUser;
            // reply to the channel the message originated from
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }



    }

    // Commands with group have the relevant prefix '![group] [command]' e.g: '!admin assignrole'
    [Group("Math")]
    public class MathStuff : ModuleBase
    {
        // !test square 20 -> 400
        [Command("square"), Summary("Squares a number.")]
        public async Task Square([Summary("The number to square")] int num)
        {
            // reply to the channel the message originated from
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }
    }

    // Commands that require a specified Permissions i.e: Require​Context​Attribute, Require​Owner​Attribute,
    // Require​Bot​Permission​Attribute, Require​User​Permission​Attribute
    public class Permission : ModuleBase
    {
        
        [Command("kicktheuser"), Summary("Kick the specified user.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task KickTheUser([Summary("The user to kick")] string userParam)
        {
            await ReplyAsync($"{userParam} has been kicked. But not really.");
        }

        [Command("moveuser"), Summary("Move user somewhere else")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        public async Task MoveUser([Summary("The user to move")] string userParam, [Summary("The place to move to")] string channelToMoveTo)
        {
            await ReplyAsync($"{userParam} has been moved to {channelToMoveTo}");
        }

        [Command("deafenuser"), Summary("deafen user")]
        [RequireUserPermission(ChannelPermission.DeafenMembers)]
        public async Task DeafenUser([Summary("The user to deafen")] string userParam)
        {
            await ReplyAsync($"{userParam} has been deafened");
        }
    }


}
