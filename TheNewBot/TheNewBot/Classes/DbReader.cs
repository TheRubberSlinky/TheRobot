using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewBot.Classes
{
    class DbReader
    {
        //temporarily going to just use Text files.
        public string ConnectionString = "";
        public DbReader(string server, string db, string type, string user = "", string pass = "")
        {
            ConnectionString = "";
        }

        public List<string> GetScheduledTasks()
        {
            if(ConnectionString == "")
            {
                string CurrentPlace = AppDomain.CurrentDomain.BaseDirectory;
                //get the text file
                List<string> items = new List<string>();
                List<string> returnItems = new List<string>();
                var currentTime = DateTime.Now.ToString("yyyy/MM/dd hh:mm");
                var tasks = XmlReader.ReadTextFile(CurrentPlace + @"Lists\ScheduledStuff.txt");
                foreach (var item in tasks)
                {
                    if (item.StartsWith(currentTime))
                    {
                        items.Add(item);
                    }
                    else
                    {
                        returnItems.Add(item);
                    }
                }
                XmlReader.WriteToTextFile(@"C:\Dev\Discord\TheNewBot\TheNewBot\Lists\ScheduledStuff.txt", returnItems);
                return items;
            }
            else
            {
                return null;
            }

        }
    }

}
