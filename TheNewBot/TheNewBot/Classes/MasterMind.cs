using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewBot.Classes
{
    class MasterMind
    {
        //6 colours, 4 peg options, simple game, guess the combination that the computer made, 10 turns
        //done it before, no longer have code, time to revamp

        //basically we will just use 4 strings in an array to guess the correct combo, this class will create the original combo
        //and will be used as the checker, brining in the original, and the guessed combo

        public string[] colourOptions = new string[] {":heart:", ":yellow_heart:", ":black_heart:", ":blue_heart:", ":green_heart:", ":purple_heart:" };
        public string[] colourDecisions = new string[] {"r", "y", "b", "bl", "g", "p" };

        /// <summary>
        /// Creates a new random combination of peg colours as what you must guess
        /// </summary>
        /// <returns></returns>
        public string[] CreateNew()
        {
            Random rnd = new Random();
            string[] options = new string[] { colourOptions[rnd.Next(0, colourOptions.Count())], colourOptions[rnd.Next(0, colourOptions.Count())] , colourOptions[rnd.Next(0, 5)] , colourOptions[rnd.Next(0, 5)] };
            return options;
        }
        /// <summary>
        /// will look if the 2 string arrays match, if they do, win, if not, it will say how many are correct in the correct spot, and how many
        /// are correct but in the wrong spot
        /// </summary>
        /// <param name="original">the set which is must equal to</param>
        /// <param name="guessed">the set the user has guessed</param>
        /// <returns></returns>
        public string[] Check(string[] original, string[] guessed)
        {
            //foreach original, if guessed at same position equals same, +1 to correct correct and add number to checked,
            //if guessed contains original and not same position and that position is not in number, +1 to correct wrong.
            int CorrectCorrect = 0, CorrectWrong = 0;
            List<int> checkedPositionsInGuessed = new List<int>();
            for (var i = 0; i < original.Length; i++)
            {
                //first checked for correcCorrect
                if (original[i] == guessed[i])
                {
                    CorrectCorrect++;
                    checkedPositionsInGuessed.Add(i);
                }
            }
            for(int i = 0; i < 4; i++)
            {
                int newNumber = guessed.GetPos(checkedPositionsInGuessed, original[i]);
                if (newNumber != -1)
                {
                    //oh good, add newNumber to list
                    checkedPositionsInGuessed.Add(newNumber);
                    CorrectWrong++;
                }
            }
            

            //tally up
            if(CorrectCorrect == 4)
                return new string[] {"Victory"};
                return new string[] {(CorrectCorrect == 0 ? "": CorrectCorrect + " pegs in the correct place"), (CorrectWrong == 0 ? "":CorrectWrong + " pegs correct but in the wrong place")};
        }
    }

    static class Extentions
    {
        public static int GetPos(this string[] values, List<int> AlreadyChosen, string lookingFor)
        {
            var position = Enumerable.Range(0, values.Count())
             .Where(i => values[i] == lookingFor)
             .ToList();
            //var position = Array.FindIndex(values, x => x.Equals(lookingFor));
            foreach(var itm in position)
            {
                if (itm != -1 && !(AlreadyChosen.Contains(itm)))
                {
                    //if (values == lookingFor && !(AlreadyChosen.Contains(values)))
                    return itm;
                }
            }
            return -1;
        }
    }

}
