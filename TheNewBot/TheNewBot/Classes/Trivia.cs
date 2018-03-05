using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace TheNewBot.Classes
{
    public class Result
    {
        public string category { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }

        private string _question;
        public string question
        {
            get
            {
                return _question;
            }

            set
            {
                string s = System.Net.WebUtility.HtmlDecode(value);
                _question = s;
            }
        }

        private string _correct_answer;
        public string correct_answer
        {
            get
            {
                return _correct_answer;
            }
            set
            {
                string s = System.Net.WebUtility.HtmlDecode(value);
                _correct_answer = s;
            }
        }

        public List<string> incorrect_answers { get; set; }
    }

    public class Trivia
    {
        public int response_code { get; set; }
        public List<Result> results { get; set; }
    }
}

