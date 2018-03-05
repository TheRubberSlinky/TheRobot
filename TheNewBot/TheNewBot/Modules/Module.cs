using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using System.IO;
using System.Threading;
using System.Configuration;
using TheNewBot.Classes;
using System.Net;

namespace TheNewBot.Modules
{
    // MUST BE PUBLIC
    public class Module : ModuleBase
    {
        // !say hello -> hello
        [Command("say"), Summary("Echos a message.")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            await ReplyAsync($"/tts {echo}");
        }
        

        }

    }
