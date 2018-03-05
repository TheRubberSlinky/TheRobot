using System;
using System.Collections.Generic;
using Discord;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheNewBot.Modules;

namespace TheNewBot.Classes
{
    static class StringToObjectLists
    {
        public static List<ScheduleItem> ConvertToScheduleItem(this List<string> values)
        {
            List<ScheduleItem> items = new List<ScheduleItem>();
            foreach(var item in values)
            {
                var parts = item.Split('‰');
                ScheduleItem itm = new ScheduleItem()
                {
                    date = DateTime.Parse(parts[0]),
                    user = parts[2],
                    task = parts[1],
                    variables = parts[3]
                };
                items.Add(itm);
            }
            return items;
        }
    }
}
