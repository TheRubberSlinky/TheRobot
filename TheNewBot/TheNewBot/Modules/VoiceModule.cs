using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Audio;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace TheNewBot.Modules
{
    class VoiceModule : ModuleBase
    {
        private readonly AudioService _service;
        public VoiceModule(AudioService service)
        {
            _service = service;
        }
        //[Command("join", RunMode = RunMode.Async)]
        //public async Task JoinCmd()
        //{
        //    await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        //}
        //
        //[Command("leave", RunMode = RunMode.Async)]
        //public async Task LeaveCmd()
        //{
        //    await _service.LeaveAudio(Context.Guild);
        //}
        //
        //[Command("play", RunMode = RunMode.Async)]
        //public async Task PlayCmd([Remainder] string song)
        //{
        //    await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        //}
    }
}
