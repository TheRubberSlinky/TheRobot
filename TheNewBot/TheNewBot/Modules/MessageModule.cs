using System;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheNewBot.Classes;
using System.Threading;
using System.Timers;

namespace TheNewBot.Modules
{
    public class MessageModule : ModuleBase
    {
        public bool Run;
        System.Timers.Timer aTimer = new System.Timers.Timer();
        [Command("BeginTimer"), Summary("this will be used to start the endless cycle of checking what to do when")]
        public async Task BeginTimer()
        {
            aTimer.Elapsed += new ElapsedEventHandler(checkforwork);
            aTimer.Interval = 60000;
            aTimer.Enabled = true;
        }

        [Command("KillTimer"), Summary("Kills the timer")]
        public async Task Kill()
        {
            aTimer.Enabled = false;
        }

        [Command("Schedule"), Summary(@"Schedule for bot to do something for you | Use it by simply going !schedule ""what you want it to schedule"" x days x hours x minutes")]
        public async Task ScheduleTask(string values, int tim1 = -1, int tim2= -1, int tim3= -1)
        {
            int minutes = 0, hours = 0, days = 0;
            if (tim3 == -1)
                if (tim2 == -1)
                    if (tim1 == -1)
                        return;
                    else
                        minutes = tim1;
                else
                {
                    hours = tim1;
                    minutes = tim2;
                }
            else
            {
                days = tim1;
                hours = tim2;
                minutes = tim3;
            }
            string user = Context.User.Username;
            string task = "none";
            string final = $"{DateTime.Now.AddDays(days).AddHours(hours).AddMinutes(minutes).ToString("yyyy/MM/dd hh:mm")}‰{task}‰{user}‰{values}";

            var result = XmlReader.AppendToTextFile(@"C:\Dev\Discord\TheNewBot\TheNewBot\Lists\ScheduledStuff.txt", final);
            if (result)
                await ReplyAsync($"alright **{Context.User.Username}**, ''{values}'' is scheduled for {days} days, {hours} hours and {minutes} minutes");
        }

        public async void checkforwork(object source, ElapsedEventArgs e)
        {
            var items = new DbReader("", "", "").GetScheduledTasks().ConvertToScheduleItem();
            var users = await Context.Guild.GetUsersAsync();
            for (int i = 0; i < items.Count; i++)
            {
                foreach (var itm in users)
                {
                    if (itm.Username == items[i].user)
                    {
                        IUser user = itm;
                        switch (items[i].task.ToLower())
                        {

                        }
                        IDMChannel dMChannel = await user.GetOrCreateDMChannelAsync();
                        await user.SendMessageAsync(items[i].variables);
                    }
                }
            }
        }
    }
    public class ScheduleItem : ModuleBase
    {
        public DateTime date { get; set; }
        public string user { get; set; }
        public string task { get; set; }
        public string variables { get; set; }
    }
}
