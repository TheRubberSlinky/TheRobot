using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewBot.Classes
{
    class HangMan
    {
        //need a counter for number of guesses, need the array of the answer
        private int NumberOfGuesses = 10, counter = 0;
        string CurrentPlace = AppDomain.CurrentDomain.BaseDirectory;
        private char[] theAnswer, GuessedSoFar;
        List<string> guesses = new List<string>();
        public void StartNew()
        {
                List<string> rd = new List<string>();
            string text = CurrentPlace + @"Lists\HangManOptions.txt";
                rd = XmlReader.ReadTextFile(text);
                theAnswer = (rd[new Random().Next(0, rd.Count - 1)].ToLower().ToCharArray());
            //for testing
            //theAnswer = "aaaaaa bbbbbccaa".ToCharArray();
            GuessedSoFar = new char[theAnswer.Count()];
            for(var itm = 0; itm < GuessedSoFar.Count(); itm++)
            {
                if(theAnswer[itm] == ' ')
                    GuessedSoFar[itm] = ' ';
                else
                    GuessedSoFar[itm] = '╡';
            }
        }
        public int insertGuess(char guess)
        {
            if (!guesses.Contains(guess.ToString()))
            {
                guesses.Add(guess.ToString());
                if (theAnswer.Contains(guess))
                {
                    //correct, add it, check if whole word is done
                    for (int i = 0; i < theAnswer.Count(); i++)
                    {
                        if (theAnswer[i] == guess)
                            GuessedSoFar[i] = guess;
                    }
                    if (GuessedSoFar.Contains('╡'))
                        return 1;
                    return 2;
                }
                else
                {
                    //incorrect, does not exist in the point, add to counter and check if maxed out
                    counter++;
                    if (counter >= NumberOfGuesses)
                        return -1;
                    return 0;
                }
            }
            return 5;
        }

        public string ShowBoard()
        {
            string[] board = new string[12];
           board[0] =@"_______";
           board[1] =@"|/    |";
           board[2] =(counter > 0 ? counter >=  3 ? @"|    (_)": counter == 2 ? @"|    (_ " : @"|    (  " : @"|     ° ");
           board[3] =(counter > 3 ? counter >= 6 ? @"|    /|\": counter == 5 ? @"|    /|": @"|     |" : @"|");
           board[4] =(counter > 6 ? @"|     | ": @"|");
           board[5] =(counter > 7 ? counter > 8 ? @"|    / \" : @"|    / " : @"|");
           board[6] =@"|      ";
           board[7] =@"|________ ";
           board[9] = string.Join("", GuessedSoFar).ToUpper().Replace(" ", "   ").Replace("╡", "_ ");
           board[10] = string.Format("guessed letters: {0}", string.Join(", ", guesses));
            board[11] = $"only {NumberOfGuesses - counter} turns left!";

            string boards = ".```" + Environment.NewLine + string.Join(Environment.NewLine, board) + "```";

            return boards;
        }

    }
}
