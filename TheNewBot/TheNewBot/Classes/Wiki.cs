using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TheNewBot.Classes
{
    class Wiki
    {
        public static string WikiStuff(string toSearch)
        {
            WebClient client = new WebClient();

            List<string> results = new List<string>();

            //toSearch = HttpUtility.HtmlDecode(toSearch);

            using (Stream stream = client.OpenRead("http://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&explaintext=1&titles=" + toSearch))
            using (StreamReader reader = new StreamReader(stream))
            {
                JsonSerializer ser = new JsonSerializer();
                Res result = ser.Deserialize<Res>(new JsonTextReader(reader));

                foreach (Page page in result.query.pages.Values)
                    results.Add(page.extract);
            }

            string first = results[0] != null ? results[0]:"";
            try
            {
                if (first.Length > 2000)
                {
                    //instead of removing all, remove one at a time until its small enough
                    while (first.Length > 2000)
                    {
                        if(first.LastIndexOf("==") > -1)
                        {
                            int lastlast = first.LastIndexOf("==");
                            first = first.Substring(0, first.LastIndexOf("==", lastlast - 1));
                        }
                        else
                        {
                            var news = first.Replace(".", "|");
                            var ans = news.Split('|');
                            int counter = ans.Length - 1;
                            while(true)
                            {
                                if (string.Join(".", ans).Length > 2000)
                                {
                                    ans[counter] = "";
                                    counter--;
                                }
                                else
                                    break;
                            }
                            first = string.Join(".", ans);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                first = ex.Message;
            }


            return first;
        }
    }
    public class Res
    {
        public Query query { get; set; }
    }

    public class Query
    {
        public Dictionary<string, Page> pages { get; set; }
    }

    public class Page
    {
        public string extract { get; set; }
    }
}
