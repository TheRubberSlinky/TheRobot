using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TheNewBot.Classes
{
    class XmlReader
    {

        public static List<string> ReadXml(string TextFile, string Root, string Username = "")
        {
            List<string> strings = new List<string>();
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;
            FileStream fs = new FileStream(TextFile, FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            xmlnode = xmldoc.GetElementsByTagName(Root);
            if(Username != "")
            for (int i = 0; i <= xmlnode.Count - 1; i++)
            {
                string str = null;
                    if (xmlnode[i].ChildNodes.Item(0).InnerText.StartsWith(Username))
                    {
                        for (var j = 0; j < xmlnode[i].ChildNodes.Item(0).ChildNodes.Count; j++)
                        {
                            str += (xmlnode[i].ChildNodes.Item(0).ChildNodes.Item(j).InnerText + "|");
                        }
                        strings.Add(str);
                    }
            }
            return strings;
        }


        public static bool AppendToTextFile(string TextFile, string Item)
        {
            try
            {
                File.AppendAllLines(TextFile, new string[] { Item });
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        public static bool WriteToTextFile(string TextFile, List<string> values)
        {
            try
            {
                File.WriteAllLines(TextFile, values.ToArray());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<string> ReadTextFile(string TextFile, string Username = "")
        {
            try
            {
                List<string> strings = new List<string>();
                string[] readText = File.ReadAllLines(TextFile, Encoding.UTF8);
                foreach (string s in readText)
                {
                    if (Username == "")
                    {
                        strings.Add(s);
                    }
                    else
                    {
                        if (s.StartsWith(Username))
                        {
                            strings.Add(s);
                            return strings;
                        }
                    }
                }
                return strings;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void WriteToTextFile(string file, string[] value)
        {
            File.WriteAllLines(file, value);
        }
    }
}
