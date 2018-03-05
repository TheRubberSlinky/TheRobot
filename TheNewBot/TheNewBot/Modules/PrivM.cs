using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewBot.Modules
{
    public class EnsureFromUserCriterion : ICriterion<SocketMessage>
    {
        private readonly ulong _id;
        public EnsureFromUserCriterion(ulong id)
            => _id = id;

        public Task<bool> JudgeAsync(SocketCommandContext sourceContext, SocketMessage parameter)
        {
            bool ok = _id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }

    public class PrivM : InteractiveBase
    {
        [Command("msgTest")]
        public async Task test()
        {
            IDMChannel dMChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dMChannel.SendMessageAsync("hi");
            //var message = Context.User.SendMessageAsync("well hi! type something");

            //var test = dMChannel.GetMessageAsync();
            var response = await NextMessageAsync(new EnsureFromUserCriterion(dMChannel.Id), TimeSpan.FromSeconds(5));
            // var test = await dMChannel.GetMessageAsync(dMChannel.Id);

           string test = response.Content;

        }
    }
}
