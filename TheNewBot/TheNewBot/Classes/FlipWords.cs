using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewBot.Classes
{
    class FlipWords
    {
        public static string FlipEm(string response)
        {
                string final = "";
                var list = new Lists.LazyMansLists().flipwordsDict();
                //var response = "Test";
                for (int i = 0; i < response.Length; i++)
                {
                    var item = response.Substring(i, 1);
                    if (list.Keys.Contains(item))
                        final += list[item];
                    else if(list.Values.Contains(item))
                    final += list.FirstOrDefault(x => x.Value == item).Key;
            }

                final = Reverse(final);
                return final;
        }
        public static string Reverse(string text)
        {
            if (text == null) return null;
            
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }
    }
}
