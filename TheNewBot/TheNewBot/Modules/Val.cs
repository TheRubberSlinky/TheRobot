using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheNewBot.Classes;

namespace TheNewBot.Modules
{
   
    public class quartzstuff : ModuleBase
    {

    }

    public class IVal : InteractiveBase
    {
        public CommandService Commands { get; }
        [Command("HangMan", RunMode = RunMode.Async)]
        public async Task hangMan()
        {
            //pseudocode this shit and lets work with that
             HangMan hng = new HangMan();
             hng.StartNew();
             bool game = true;
             TimeSpan ts = new TimeSpan(0, 0, 300);
             await ReplyAsync(hng.ShowBoard());
             while(game)
            {
                var guess = await NextMessageAsync(true, true, ts);
                if (guess != null)// && guess.Content.StartsWith("guess! ") && guess.Content.Substring(6).Length == 1)
                    if (guess.Content.ToLower().StartsWith("guess! "))
                        if(guess.Content.ToLower().Substring(7).Length == 1)
                {
                   int result = hng.insertGuess(guess.Content.ToLower().Substring(7).ToCharArray()[0]);
                   switch (result)
                   {
                        case -1: //you lose
                            await ReplyAsync("You Lose!");
                            game = false;
                        break;
                        case 0: //incorrect
                            await ReplyAsync("sorry, that is incorrect");
                        break;
                        case 1: // correct
                            await ReplyAsync("yes! that is part of it! keep going");
                        break;
                        case 2: // you win
                            await ReplyAsync("**congratulations!! you guessed correctly!**");
                            game = false;
                        break;
                        case 5:
                                    await ReplyAsync("you have already guessed this");
                                    break;
                   }
                   await ReplyAsync(hng.ShowBoard());
                   if(result == 2)
                   game = false;
                }
             }
            await ReplyAsync("GG");
        }

        [Command("BattleShip")]
        public async Task BattleShip()
        {
            //on hold until can figure out messaging
        }

        [Command("jino")]
        public async Task Start()
        {

        }

        [Command("Master", RunMode = RunMode.Async), Summary("lets play MasterMind!")]
        public async Task Master([Remainder, Summary("if you want to change the length")] string val = "")
        {
            try
            {
                if (val != "")
                {
                    int changeTo = int.Parse(val);
                    //this means we want to change it, first check the privelage of the person, if they have what is needed, change it
                    if (changeTo < 0)
                    {
                        await ReplyAsync("Value too low");
                    }
                    else
                    {
                        if (true) //use this if for when you want only certain roles to be able to do it
                        {
                            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            configuration.AppSettings.Settings["MastermindTimer"].Value = changeTo.ToString();
                            configuration.Save();

                            ConfigurationManager.RefreshSection("appSettings");
                            await ReplyAsync("Mastermind length has been extended to " + changeTo);
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                try
                {
                    int amount = int.Parse(ConfigurationManager.AppSettings["MastermindTimer"]);
                    var Master = new Classes.MasterMind();
                    await ReplyAsync("." + Environment.NewLine + "I want to play a game, pick 4 of the colours between " + string.Join(", ", Master.colourOptions) + Environment.NewLine + " You Can reply with " + string.Join(", ", Master.colourDecisions) + Environment.NewLine + "to answer, type down '$' followed by the 4 colours you pick by their letters, Example: '$bl,g,b,r'" + Environment.NewLine + "You have " + amount + " seconds between answers");
                    var ans = Master.CreateNew();
                    TimeSpan ts = new TimeSpan(0, 0, amount);
                    //await ReplyAsync("Created, for testing the answer is " + string.Join(", ", ans));
                    List<string[]> lst = new List<string[]>();
                    for (int i = 0; i < 10; i++)
                    {
                        var response = await NextMessageAsync(true, true, ts);
                        if (response != null)
                        {
                            if (response.Content.ToLower() == "i give up")
                            {
                                await ReplyAsync("haha! pansy");
                                break;
                            }

                            if (response.Content.StartsWith("$"))
                            {
                                var responses = response.Content.Substring(1, response.Content.Length - 1).ToLower().Replace(" ", "").Split(',');
                                if (responses.Count() == 4)
                                {
                                    List<string> colours = new List<string>();
                                    foreach (var itm in responses)
                                    {
                                        int tst = Array.FindIndex(Master.colourDecisions, x => x.Equals(itm));
                                        if (tst == -1)
                                        {
                                            await ReplyAsync(itm + " is not one of the colours");
                                            i--;
                                            break;
                                        }
                                        else
                                        {
                                            colours.Add(Master.colourOptions[tst]);
                                        }
                                    }
                                    lst.Add(colours.ToArray());
                                    await ReplyAsync(string.Join(" ", colours));
                                    var rep = Master.Check(ans, colours.ToArray());
                                    await ReplyAsync((rep[0] == "" && rep[1] == "" ? "all pegs are wrong" : string.Join(Environment.NewLine, rep)));
                                    if (rep[0] == "Victory")
                                        break;
                                }
                                else
                                {
                                    await ReplyAsync($"{response.Content.Substring(1, response.Content.Length - 1)} is invalid somehow, try again");
                                    i--;
                                }
                            }
                        }
                        else
                        {
                            await ReplyAsync("you took too long to decide! Game Over");
                            break;
                        }
                        if (i == 9)
                        {
                            await ReplyAsync("you have guessed incorrectly 10 times, you lose" + $"{string.Join(", ", ans)} was correct");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ReplyAsync(ex.Message);
                    await ReplyAsync("you broke it! you bastard! now you have to start again");
                }
            }
        }

        [Command("TicTacToe", RunMode = RunMode.Async), Summary("battle friends to not the death!")]
        public async Task TicTac([Remainder, Summary("Your opponent")] IUser user = null)
        {
            if (user == null)
            {
                TicTacAI ai = new TicTacAI();

                await ReplyAsync("you challenge me? fine, pick your difficulty (anything)");

                var reply = await NextMessageAsync(true, true, new TimeSpan(0, 0, 10));
                if (reply != null)
                {
                    int dif;
                    if (int.TryParse(reply.Content, out dif))
                    {
                        ai.SetDifficulty(dif);
                        bool game = true, player = true;
                        TimeSpan battle = new TimeSpan(0, 0, 999999);
                        string[,] battleArena = new string[3, 3];
                        string filler = " ";
                        battleArena[0, 0] = filler;
                        battleArena[0, 1] = filler;
                        battleArena[0, 2] = filler;
                        battleArena[1, 0] = filler;
                        battleArena[1, 1] = filler;
                        battleArena[1, 2] = filler;
                        battleArena[2, 0] = filler;
                        battleArena[2, 1] = filler;
                        battleArena[2, 2] = filler;
                        var selectedUser = Context.User;
                        await ReplyAsync(TicTacToeLayout(battleArena, Context.User, game, true, null,player));
                        while (game)
                        {
                            if (player)
                            {
                                var attack = await NextMessageAsync(true, true, battle);
                                if (attack != null)
                                {
                                    if (attack.Content.StartsWith("b!"))
                                    {
                                        try
                                        {
                                            //Should have the attacks like a 1, so type it like b! a 1
                                            string battlepos = attack.Content.Substring(3, attack.Content.Length - 3);
                                            var coords1 = battlepos.Substring(0, 1);
                                            var coords2 = battlepos.Substring(1, 1);
                                            int posX = -1, posY = -1;
                                            int.TryParse(coords2, out posY);
                                            posY--;
                                            posX = (coords1.ToLower() == "a" ? 0 : coords1.ToLower() == "b" ? 1 : 2);
                                            //got both positions, and player holds which point to put where
                                            //place it! or at least try to
                                            if (battleArena[posX, posY] == filler)//might be null check
                                            {
                                                battleArena[posX, posY] = (player ? "X" : "O");
                                                if (WinConditionMet(battleArena, player))
                                                {
                                                    await ReplyAsync(TicTacToeLayout(battleArena, Context.User, game, true, null, player));
                                                    await ReplyAsync(player ? "You win!" : "you lose!");
                                                    game = false;
                                                    break;
                                                }
                                                else if (battleArena[0, 0] != filler && battleArena[0, 1] != filler && battleArena[0, 2] != filler &&
                                                        battleArena[1, 0] != filler && battleArena[1, 1] != filler && battleArena[1, 2] != filler &&
                                                        battleArena[2, 0] != filler && battleArena[2, 1] != filler && battleArena[2, 2] != filler)
                                                {
                                                    await ReplyAsync($"it is a tie! both are equally matched!");
                                                    game = false;
                                                }
                                                //true of player will always be X
                                                //at the end of the turn, swap player
                                                player = !player;
                                                if (game)
                                                    await ReplyAsync(TicTacToeLayout(battleArena, Context.User, game, true, null, player));
                                            }
                                            else
                                            {
                                                await ReplyAsync($"{selectedUser.Mention}, that is not a valid move. try again");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            await ReplyAsync(ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    await ReplyAsync($"{selectedUser} took too long! I am the winner!");
                                    game = false;
                                }
                            }
                            else
                            {
                                //pc turn
                                battleArena = ai.Place(battleArena, false);
                                if (WinConditionMet(battleArena, player))
                                {
                                    await ReplyAsync(TicTacToeLayout(battleArena, selectedUser, game, true));
                                    await ReplyAsync(player ? "You win!" : "you lose!");
                                    game = false;
                                    break;
                                }
                                else if (battleArena[0, 0] != filler && battleArena[0, 1] != filler && battleArena[0, 2] != filler &&
                                        battleArena[1, 0] != filler && battleArena[1, 1] != filler && battleArena[1, 2] != filler &&
                                        battleArena[2, 0] != filler && battleArena[2, 1] != filler && battleArena[2, 2] != filler)
                                {
                                    await ReplyAsync($"it is a tie! both are equally matched!");
                                    game = false;
                                }
                                player = !player;
                                if (game)
                                    await ReplyAsync(TicTacToeLayout(battleArena, selectedUser, game, true, null, true));
                            }
                        }
                    }
                    else
                    {
                        await ReplyAsync($"don't be stupid {Context.User.Mention}");
                    }
                }
            }
            else
            {
                TimeSpan ts = new TimeSpan(0, 0, 300);

                var userInfo = user ?? Context.Client.CurrentUser;
                // reply to the channel the message originated from
                await ReplyAsync($"{Context.User.Mention} has challenged {userInfo.Mention} to a duel! do you Accept? (Y/N)");

                bool no = true;
                while (no)
                {
                    var response = await NextMessageAsync(false, true, ts);
                    if (response != null)
                    {
                        if (response.Author == user)
                        {
                            if (response.Content.ToLower() == "y")
                            {
                                await ReplyAsync("Accepted! let the battle begin!");
                                await ReplyAsync($"{Context.User.Mention} will go first!, they are X!");
                                no = false;

                                #region Actual Game of Tic Tac Toe
                                bool game = true, player = true;
                                TimeSpan battle = new TimeSpan(0, 0, 999999);
                                string[,] battleArena = new string[3, 3];
                                string filler = "[ ]";
                                battleArena[0, 0] = filler;
                                battleArena[0, 1] = filler;
                                battleArena[0, 2] = filler;
                                battleArena[1, 0] = filler;
                                battleArena[1, 1] = filler;
                                battleArena[1, 2] = filler;
                                battleArena[2, 0] = filler;
                                battleArena[2, 1] = filler;
                                battleArena[2, 2] = filler;
                                var selectedUser = player ? Context.User : userInfo;
                                while (game)
                                {
                                    await ReplyAsync(TicTacToeLayout(battleArena, selectedUser, game, false, player ? Context.User : userInfo, player));

                                    var attack = await NextMessageAsync(false, true, battle);
                                    if (attack.Author == selectedUser)
                                        if (attack != null)
                                        {
                                            if (attack.Content.StartsWith("b!"))
                                            {
                                                try
                                                {
                                                    //Should have the attacks like a 1, so type it like b! a 1
                                                    string battlepos = attack.Content.Substring(3, attack.Content.Length - 3);
                                                    var coords1 = battlepos.Substring(0, 1);
                                                    var coords2 = battlepos.Substring(1, 1);
                                                    int posX = -1, posY = -1;
                                                    int.TryParse(coords2, out posY);
                                                    posY--;
                                                    posX = (coords1.ToLower() == "a" ? 0 : coords1.ToLower() == "b" ? 1 : 2);
                                                    //got both positions, and player holds which point to put where
                                                    //place it! or at least try to
                                                    if (battleArena[posX, posY] == filler)//might be null check
                                                    {
                                                        battleArena[posX, posY] = (player ? "X" : "O");
                                                        if (WinConditionMet(battleArena, player))
                                                        {
                                                            game = false;
                                                            await ReplyAsync(TicTacToeLayout(battleArena, selectedUser, game, false, player ? Context.User : userInfo, player));
                                                            await ReplyAsync($"{selectedUser.Mention} is the winner!! {(player ? userInfo.Mention : Context.User.Mention)} has been defeated!!");
                                                            break;
                                                        }
                                                        else if (battleArena[0, 0] != filler && battleArena[0, 1] != filler && battleArena[0, 2] != filler &&
                                                                battleArena[1, 0] != filler && battleArena[1, 1] != filler && battleArena[1, 2] != filler &&
                                                                battleArena[2, 0] != filler && battleArena[2, 1] != filler && battleArena[2, 2] != filler)
                                                        {
                                                            await ReplyAsync($"it is a tie! both are equally matched!");
                                                            game = false;
                                                        }
                                                        //true of player will always be X
                                                        //at the end of the turn, swap player
                                                        player = !player;
                                                        selectedUser = player ? Context.User : userInfo;
                                                    }
                                                    else
                                                    {
                                                        await ReplyAsync($"{selectedUser.Mention}, that is not a valid move. try again");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    await ReplyAsync(ex.Message);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            await ReplyAsync($"{selectedUser} took too long! {(player ? Context.User : userInfo)} is the winner!");
                                            game = false;
                                        }

                                }
                                #endregion

                            }
                            else if ((response.Content.ToLower() == "n"))
                            {
                                await ReplyAsync($"oooooo rejected! {Context.User.Mention} is clearly not worth it to {userInfo.Mention}");
                                no = false;
                            }
                        }
                    }
                    else
                    {
                        await ReplyAsync($"took too long! {userInfo.Mention} has given up before the battle has even begun!");
                        no = false;
                    }
                }

            }

        }

        public string TicTacToeLayout(string[,] battleArena, IUser user, bool game, bool vsAI, IUser opponent = null, bool turn = true)
        {
            try
            {
                string brd = string.Format("```{0}{1}The board stands! {8} | {9}{1}{7}{1}{2}{1}{6}{1}{3}{1}{6}{1}{4}{1}{10}{1} {5}```",
                                                "TicTacToe!", Environment.NewLine,
                                                string.Format("A||{0}||{1}||{2}||", battleArena[0, 0], battleArena[0, 1], battleArena[0, 2]),
                                                string.Format("B||{0}||{1}||{2}||", battleArena[1, 0], battleArena[1, 1], battleArena[1, 2]),
                                                string.Format("C||{0}||{1}||{2}||", battleArena[2, 0], battleArena[2, 1], battleArena[2, 2]),
                                                game ? "it is " + (vsAI ? (turn ? "your" : "my") : user.Username) + " turn" : (vsAI ? (turn ? "you win!" : "I win!") : user.Username + " wins!"),
                                                           " -----------",
                                                           " __1__2__3__",
                                                           user.Username,
                                                           vsAI ? "PC" : opponent.Username,
                                                           " -----------"
                                                );

                return brd;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private bool WinConditionMet(string[,] battleArena, bool player)
        {
            //first check player for X or y, then see if they win or not
            string correct = player ? "X" : "O";
            if ((battleArena[0, 0] == correct && ((battleArena[0, 1] == correct && battleArena[0, 2] == correct) || (battleArena[1, 0] == correct && battleArena[2, 0] == correct) || (battleArena[1, 1] == correct && battleArena[2, 2] == correct)))
                ||
                (battleArena[2, 2] == correct && ((battleArena[2, 1] == correct && battleArena[2, 0] == correct) || (battleArena[1, 2] == correct && battleArena[0, 2] == correct) || (battleArena[1, 1] == correct && battleArena[0, 0] == correct)))
                ||
                (battleArena[1, 1] == correct && ((battleArena[0, 1] == correct && battleArena[2, 1] == correct) || (battleArena[1, 0] == correct && battleArena[1, 2] == correct) || (battleArena[0, 2] == correct && battleArena[2,0]== correct))
                ))
                return true;
            return false;
        }

    }


    public class Val : ModuleBase
    {
        [Command("Encrypt")]
        public async Task Encrypt(int Key, [Remainder] string message)
        {
            int key = Key;
            string test = message;
            await ReplyAsync($"```{test.PepperMe(key)}```");
            IDMChannel dMChannel = await Context.User.GetOrCreateDMChannelAsync();
            await Context.User.SendMessageAsync("to get people to read your message, give them this number: "+key.ToString());
        }

        [Command("Decrypt")]
        public async Task Decrypt(int key, [Remainder] string message)
        {
            string test = message;
            await ReplyAsync(test.Cumin(key));
        }

        [Command("numbers")]
        public async Task numbers(int type, [Remainder]string values)
        {
            List<string> options = new List<string>() {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
            List<string> CellStyle = new List<string>() { "2", "22", "222", "3", "33", "333", "4", "44", "444", "5", "55", "555", "6", "66", "666", "7", "77", "777", "7777", "8", "88", "888", "9", "99", "999", "9999" };
            var items = values.ToLower().ToCharArray();
            string final = "";
            if(type == 0)
                foreach (char item in items)
                {
                    if (item == ' ')
                        final += " - ";
                    else
                    {
                        final += CellStyle[options.IndexOf(item.ToString())];
                        final += " ";
                    }
                }
            else
            foreach(char item in items)
            {
                if (item == ' ')
                    final += " - ";
                else
                {
                    final += options.IndexOf(item.ToString()) + 1;
                        final += " ";
                }
            }
            await ReplyAsync(final);
        }
        
        Random rnd = new Random();

        [Command("Time")]
        public async Task Time()
        {
            List<string[]> numbers = new List<string[]>();
            numbers.Add(new string[] { " _ ", "| |", "|_|"}); //0
            numbers.Add(new string[] { "   ", "  |", "  |"}); //1
            numbers.Add(new string[] { " _ ", " _|", "|_ "}); //2
            numbers.Add(new string[] { " _ ", " _|", " _|"}); //3
            numbers.Add(new string[] { "   ", "|_|", "  |"}); //4
            numbers.Add(new string[] { " _ ", "|_ ", " _|"}); //5
            numbers.Add(new string[] { " _ ", "|_ ", "|_|"}); //6
            numbers.Add(new string[] { " _ ", "  |", "  |"}); //7
            numbers.Add(new string[] { " _ ", "|_|", "|_|"}); //8
            numbers.Add(new string[] { " _ ", "|_|", " _|"}); //9
            

            var time = DateTime.Now.ToString("HHmmss");

            var parts =  time.ToCharArray();
            int[] options = new int[6];
            for (int i = 0; i < parts.Count(); i++)
            {
                options[i] = int.Parse(parts[i].ToString());
            }
           var ans= string.Format("The time is:{0}```{1}{4} {7}{10} {13}{16}{0}{2}{5}.{8}{11}.{14}{17}{0}{3}{6}.{9}{12}.{15}{18}```",
                Environment.NewLine, 
                numbers[options[0]][0], numbers[options[0]][1], numbers[options[0]][2],
                numbers[options[1]][0], numbers[options[1]][1], numbers[options[1]][2],
                numbers[options[2]][0], numbers[options[2]][1], numbers[options[2]][2],
                numbers[options[3]][0], numbers[options[3]][1], numbers[options[3]][2],
                numbers[options[4]][0], numbers[options[4]][1], numbers[options[4]][2],
                numbers[options[5]][0], numbers[options[5]][1], numbers[options[5]][2]

                );
            

            await ReplyAsync(ans);
            
        }


        [Command("halp")]
        public async Task help(string command = "")
        {
            //var test = Commands.Commands;
            //var tets = "";
            
        }

        [Command("choose")]
        public async Task choose([Remainder, Summary("choices")]string choices)
        {
            if(choices.Contains("|"))
            {
                var options = choices.Split('|');
                await ReplyAsync($"**{Context.User.Username}, I pick {options[rnd.Next(0, options.Count() - 1)]}**");
            }
            else
            {
                await ReplyAsync("I dont see the options");
            }
        }

        [Command("Wiki")]
        public async Task Wiki([Remainder, Summary("words to search")]string whattoSearch)
        {
            var ans = Classes.Wiki.WikiStuff(whattoSearch);
            await ReplyAsync(ans != "" ? string.Format("```{0}```", ans): "no results were found or results too large for " + whattoSearch);
        }

        [Command("urban")]
        public async Task urban([Remainder, Summary("words to search")]string whattoSearch)
        {
            try
            {
                string url = "http://www.urbandictionary.com/define.php?term=" + whattoSearch;
                WebRequest webRequest = WebRequest.Create(url);
                WebResponse response = webRequest.GetResponse();
                Stream str = response.GetResponseStream();
                StreamReader reader = new StreamReader(str);
                string source = reader.ReadToEnd();
                string result = source.Substring(source.IndexOf("meaning"));
                result = Regex.Replace(result, "<.*?>", string.Empty);
                result = HttpUtility.HtmlDecode(result);
                result = result.Replace("\n", "");
                result = result.Substring(9);
                var by = result.IndexOf("by ");
                var byp = false;
                if(by != -1)
                while(!byp)
                {
                    if (result.ToCharArray()[by - 1] != ' ')
                    {
                        byp = true;
                    }
                    else
                    {
                        by = result.IndexOf("by", by + 1);
                    }
                }
                if(by != -1)
                result = result.Substring(0, by);
                string quotes = "";
                if (result.IndexOf('"') > -1)
                {
                    int value = result.LastIndexOf('"');
                    quotes = result.Substring(result.IndexOf('"'), (value - result.IndexOf('"')));
                    result = result.Replace(quotes, Environment.NewLine + quotes);
                }
                result = result.Replace("#", Environment.NewLine);
                if(result.Length > 1900)
                {
                    string newreso = result.Replace(Environment.NewLine, "|");
                    var newres = newreso.Split('|');
                    int cnt = newres.Count() - 1;
                    while(true)
                    {
                        if (string.Join(Environment.NewLine, newres).Length > 1900)
                        {
                            newres[cnt] = "";
                            cnt--;
                        }
                        else
                            break;
                    }
                    result = string.Join(Environment.NewLine, newres);
                }

                await ReplyAsync(string.Format(":notebook_with_decorative_cover: Urban Dictionary {1}{0}{1} ```{2}```", whattoSearch.ToUpper(), Environment.NewLine, result));
            }
            catch (Exception ex)
            {
                string hi = ex.Message;

            }
        }

        [Command("test")]
        public async Task tester(int number)
        {
            //   TicTacAI ai = new TicTacAI();
            //   ai.SetDifficulty(number);
            //
            //   List<bool> values = new List<bool>();
            //       for(int i = 0; i < 500; i++)
            //       {
            //           values.Add(ai.());
            //       }
            //   var amount = Enumerable.Range(0, values.Count())
            //.Where(i => values[i] == false)
            //.ToList();
            //   await ReplyAsync($"{amount.Count} failed, and {500 - amount.Count} succeeded out of 500 attempts");

        }

        public async Task HelpLevel()
        {

        }

        [Command("Flip"), Summary("flip the words you give")]
        public async Task FlipEm([Remainder, Summary("words to flip")] string words)
        {
            List<string> tables = new List<string>()
                {
                    @"(╯°□°）╯︵ ┻━┻",
                    @"(┛◉Д◉)┛彡┻━┻",
                    @"(ﾉ≧∇≦)ﾉ ﾐ ┸━┸",
                    @"(ノಠ益ಠ)ノ彡┻━┻",
                    @"(╯ರ ~ರ）╯︵ ┻━┻",
                    @"(┛ಸ_ಸ)┛彡┻━┻",
                    @"(ﾉ´･ω･)ﾉ ﾐ ┸━┸",
                    @"(ノಥ, _｣ಥ)ノ彡┻━┻",
                    @"(┛✧Д✧))┛彡┻━┻",
                    @"┻━┻ ︵ヽ(`Д´)ﾉ︵﻿ ┻━┻",
                    @"┻━┻ ︵﻿ ¯\(ツ) /¯ ︵ ┻━┻",
                    @"(ノTДT)ノ ┫:･’.::･┻┻:･’.::･",
                    @"(ノ｀⌒´)ノ ┫：・’.：：・┻┻：・’.：：・",
                    @"(ﾉ *｀▽´*)ﾉ ⌒┫ ┻ ┣ ┳",
                    @"┻━┻ミ＼(≧ﾛ≦＼)",
                    @"┻━┻︵└(՞▃՞ └)",
                    @"┻━┻︵└(´▃｀└)",
                    @"─=≡Σ((((╯°□°）╯︵ ┻━┻",
                    @"(ノ｀´)ノ ~┻━┻",
                    @"(-_ - )ﾉ⌒┫ ┻ ┣",
                    @"(ノ￣皿￣）ノ ⌒=== ┫",
                    @"ノ｀⌒´)ノ ┫：・’.：：・┻┻",
                    @"༼ﾉຈل͜ຈ༽ﾉ︵┻━┻",
                    @"ヽ༼ຈل͜ຈ༽ﾉ⌒┫ ┻ ┣",
                    @"ﾐ┻┻(ﾉ >｡<)ﾉ",
                    @" .::･┻┻☆()ﾟOﾟ)",
                    @"Take that!(ﾉ｀A”)ﾉ ⌒┫ ┻ ┣ ┳☆(x x)",
                    @"(ノ｀m´)ノ ~┻━┻ (/o＼)",
                    @"⌒┫ ┻ ┣ ⌒┻☆)ﾟ⊿ﾟ)ﾉWTF!",
                    @"(ﾉ≧∇≦)ﾉ ﾐ ┸┸)`νﾟ)･;’.",
                    @"ミ(ノ￣^￣)ノ≡≡≡≡≡━┳━☆()￣□￣)/"
                };
            List<string> fixing = new List<string>()
            {
                @"┣ﾍ(^▽^ﾍ)Ξ(ﾟ▽ﾟ*)ﾉ┳━┳ There we go~♪",
                @"┬──┬ ノ( ゜-゜ノ)",
                @"┬──┬﻿ ¯\_(ツ)",
                @"(ヘ･_･)ヘ┳━┳",
                @"ヘ(´° □°)ヘ┳━┳",
                @"┣ﾍ(≧∇≦ﾍ)… (≧∇≦)/┳━┳",
            };
            //being lazy and curious
            if (words == "table")
            {
                await ReplyAsync($"```{tables[rnd.Next(0, tables.Count() - 1)]}```");
            }
            else if (tables.Contains(words))
            {
                await ReplyAsync($"```{fixing[rnd.Next(0, fixing.Count() - 1)]}```");
            }
            else
                try
                {
                    await ReplyAsync(FlipWords.FlipEm(words));
                }
                catch
                {
                    await ReplyAsync($"I refuse to flip your shit {Context.User.Username}");
                }

        }

        [Command("CatFacts"), Summary("cat facts for cat people")]
        public async Task GetCatFact()
        {
            //im doing this the lazy way because we havent decided on our databasing yet
            var lst = new Lists.LazyMansLists();
            await ReplyAsync(lst.catfacts[rnd.Next(0, lst.catfacts.Count - 1)]);
        }
        [Command("DogFacts"), Summary("dog facts for dog people")]
        public async Task GetDogFact()
        {
            //im doing this the lazy way because we havent decided on our databasing yet
            var lst = new Lists.LazyMansLists().DogList();
            await ReplyAsync(lst[rnd.Next(0, lst.Count - 1)]);
        }
        [Command("AnimalFacts"), Summary("animal facts for animal people")]
        public async Task GetAnimalFact()
        {
            //im doing this the lazy way because we havent decided on our databasing yet
            var lst = new Lists.LazyMansLists().AnimalList();
            await ReplyAsync(lst[rnd.Next(0, lst.Count - 1)]);
        }


        [Command("8ball"), Summary("The magic 8 ball of wisdom!")]
        public async Task ball8([Remainder, Summary("the question to ask the magical 8 ball")] string question)
        {
            var responses = new Lists.LazyMansLists().ball8Answers;
            await ReplyAsync(string.Format("**{0}** |   :8ball: |   {1}", Context.User.Username, responses[rnd.Next(0, responses.Count - 1)]));
        }

        [Command("slots"), Summary("the obviously totally not obvious rip off")]
        public async Task playSlots([Remainder, Summary("amount paying")] int number)
        {
            List<string> choices = new List<string>()
                {
                    ":banana:",":cherries:",":pear:",":melon:",":grapes:",":tangerine:",":watermelon:",":gem:",":flag_lv:",":seven:",":bell:"
                };
            //cant find a way to edit messages, if you can find it, please let me know
            //:banana: *3-> 1               0
            //:cherries: *2-> 1             1
            //:cherries: *3-> 3
            //:pear: *2-> 3             2
            //:pear: *3-> 10
            //:melon: *2-> 3            3
            //:melon: *3-> 10           
            //:grapes: *2-> 3           4
            //:grapes: *3-> 10
            //:tangerine: *2-> 3            5
            //:tangerine: *3-> 10
            //:watermelon: *2-> 3           6
            //:watermelon: *3-> 10
            //:gem: *2-> 20         7
            //:gem: *3->Jackpot
            //:flag_lv: *2-> 10         8
            //:flag_lv: *3-> 30
            //:seven: *2-> 20           9
            //:seven: *3-> 75
            //:bell: *3-> 75            10

            //11 options total
            //okay, so there is 20 possibilities that can return success, so i need a lot of ifs
            int num1 = rnd.Next(0, 11), num2 = rnd.Next(0, 11), num3 = rnd.Next(0, 11);
            string madness = "";
            if (num1 == 0 && num2 == 0 && num3 == 0)
                madness = succeed(1, number);
            else if (num1 == 1 && num2 == 1 && num3 == 1)
                madness = succeed(3, number);
            else if ((num1 == 1 && num2 == 1) || (num1 == 1 && num3 == 1) || (num2 == 1 && num3 == 1))
                madness = succeed(1, number);
            else if (num1 == 2 && num2 == 2 && num3 == 2)
                madness = succeed(10, number);
            else if ((num1 == 2 && num2 == 2) || (num1 == 2 && num3 == 2) || (num2 == 2 && num3 == 2))
                madness = succeed(3, number);
            else if (num1 == 3 && num2 == 3 && num3 == 3)
                madness = succeed(10, number);
            else if ((num1 == 3 && num2 == 3) || (num1 == 3 && num3 == 3) || (num2 == 3 && num3 == 3))
                madness = succeed(3, number);
            else if (num1 == 4 && num2 == 4 && num3 == 4)
                madness = succeed(10, number);
            else if ((num1 == 4 && num2 == 4) || (num1 == 4 && num3 == 4) || (num2 == 4 && num3 == 4))
                madness = succeed(3, number);
            else if (num1 == 5 && num2 == 5 && num3 == 5)
                madness = succeed(10, number);
            else if ((num1 == 5 && num2 == 5) || (num1 == 5 && num3 == 5) || (num2 == 5 && num3 == 5))
                madness = succeed(3, number);
            else if (num1 == 6 && num2 == 6 && num3 == 6)
                madness = succeed(1000, number);
            else if ((num1 == 6 && num2 == 6) || (num1 == 6 && num3 == 6) || (num2 == 6 && num3 == 6))
                madness = succeed(20, number);
            else if (num1 == 7 && num2 == 7 && num3 == 7)
                madness = succeed(30, number);
            else if ((num1 == 7 && num2 == 7) || (num1 == 7 && num3 == 7) || (num2 == 7 && num3 == 7))
                madness = succeed(10, number);
            else if (num1 == 8 && num2 == 8 && num3 == 8)
                madness = succeed(75, number);
            else if ((num1 == 8 && num2 == 8) || (num1 == 8 && num3 == 8) || (num2 == 8 && num3 == 8))
                madness = succeed(20, number);
            else if (num1 == 9 && num2 == 9 && num3 == 9)
                madness = succeed(75, number);
            else if ((num1 == 9 && num2 == 9) || (num1 == 9 && num3 == 9) || (num2 == 9 && num3 == 9))
                madness = succeed(20, number);
            else if (num1 == 10 && num2 == 10 && num3 == 10)
                madness = succeed(75, number);
            bool won = true;
            if (madness == "")
            {
                madness = string.Format("**{0} has spent {1} and lost it all**", Context.User.Username, number);
                won = false;
            }

            try
            {
                string result = string.Format(".{0}|{4}{11}{5}{11}{6}|{0}|{1}{11}{2}{11}{3}|<{0}|{7}{11}{8}{11}{9}|{0}{0}[//////////////]{0}|{11}{10}{11}|{0} {12}",
                    Environment.NewLine, choices[num1], choices[num2], choices[num3], choices[(num1 == 0 ? 10 : num1 - 1)], choices[(num2 == 0 ? 10 : num2 - 1)], choices[(num3 == 0 ? 10 : num3 - 1)], choices[(num1 == 10 ? 0 : num1 + 1)], choices[(num2 == 10 ? 0 : num2 + 1)], choices[(num3 == 10 ? 0 : num3 + 1)], (won ? "VICTORY!" : "LOSS"), "\t", madness);

                await ReplyAsync(result);
            }
            catch
            {
                await ReplyAsync("something went wrong");
            }
            //await m.ModifyAsync();
        }

        private string succeed(int multiply, int number)
        {
            return (string.Format("**{0} has spend {1} and won {2}!**", Context.User.Username, number, number * multiply));
        }

        [Command("ListEmotes")]
        public async Task ListEmAll()
        {
            var allOfThem = Context.Guild.Emotes;
            string response = "";
            foreach(var item in allOfThem)
            {
                response += item;
            }
            await ReplyAsync(response == "" ? "there are no server emotes":response);
        }

        // usage: !userinfo skeivy --> Skeivy#2201
        [Command("userinfo"), Summary("Returns info about the current user, or the user parameter, if one passed.")]
        // Aliass that can be used instead of userinfo
        [Alias("user", "whois")]
        public async Task UserInfo([Summary("The (optional) user to get info for")] IUser user = null)
        {
            // userinfo = user unless it is null, otherwise user = CurrentUser
            var userInfo = user ?? Context.Client.CurrentUser;
            // reply to the channel the message originated from
            await ReplyAsync($"{userInfo.Mention}#{userInfo.Discriminator}");
        }
        [Command("cookie"), Summary("Cookie monster would be proud")]
        public async Task cookie([Summary("who to give cookie to")] IUser user)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            if(userInfo != Context.User)
                await ReplyAsync(string.Format("**:cookie:  |   {0} has given {1} a cookie!**", Context.User.Username, userInfo.Mention));
                
            else
            {
                await ReplyAsync("You cannot send cookies to yourself");
            }
        }
    }

    public class Profile : ModuleBase
    {

        [Command("ResetProfiles"), Summary("Clear all the progress and start over")]
        public async Task reset()
        {
            var users = await Context.Guild.GetUsersAsync();

            string str = "";
            foreach (var itm in users)
            {
                if (!itm.IsBot)
                {
                    string stri = itm.Username + "|" + itm.Discriminator + "|0" + "|0" + "|0" + "|NoQuoteAdded";
                    str += stri + Environment.NewLine;
                }
            }

            using (StreamWriter bw = new StreamWriter(File.Create(@"C:\Dev\Discord\TheNewBot\TheNewBot\Lists\UserData.txt")))
            {
                bw.Write(str);
                bw.Close();
            }

            await ReplyAsync(str);
        }

        [Command("test"), Summary("Test the user specified with random math questions")]
        public async Task Question(/*[Remainder, Summary("what to ask")] string question*/)
        {
            var rd = XmlReader.ReadXml(@"C:\Dev\Discord\TheNewBot\TheNewBot\Lists\TestXmlFile.xml", "people", Context.User.Username);
            var result = rd[0].Split('|');
            var value1 = "".PadRight(30, '.');
            string str = string.Format("**Profile**{0}```{1}{0}{2}|{0}{3}|{0}{4}|{0}{5}```",
                                        Environment.NewLine,
                                       "--------------------------------------".PadRight(70, '-'),
                         string.Format("|___Name:{0}____________Level: {1}", result[0], result[2]).PadRight(69, ' '),
                         string.Format("|___Quote:{0}", result[3]).PadRight(69, ' '),
                         string.Format("|___Mood:{0}", result[4]).PadRight(69, ' '),
                                       "--------------------------------------".PadRight(70, '-')
                                       );
            await ReplyAsync(str);
        }
        [Command("Profile"), Summary("view your own profile and update it")]
        public async Task rofile(string Updater = "", [Remainder]string value = "")
        {

            List<string> rd = new List<string>();
            string text = @"C:\Dev\Discord\TheNewBot\TheNewBot\Lists\UserData.txt";
            if (Updater == "")
            {
                rd = XmlReader.ReadTextFile(text, Context.User.Username);
                await ReplyAsync(rd[0]);
            }
            else
            {
                if (value == "")
                {
                    await ReplyAsync("value was invalid for selected set");
                    return;
                }
                rd = XmlReader.ReadTextFile(text);
                string test = Update(Updater, Context.User.Username, rd, value, text);
                await ReplyAsync(test);
            }
        }
        public string Update(string updater, string User, List<string> value, string newVal, string textFileLocation)
        {
            int rowChange = 0;
            int pos = 0;
            for (int i = 0; i < value.Count; i++)
            {
                string changeable = value[i];
                if (value[i].StartsWith(User))
                {
                    rowChange = i;
                    switch (updater.ToLower())
                    {
                        case "xp":
                            pos = 2;
                            break;
                        case "level":
                            pos = 3;
                            break;
                        case "credits":
                            pos = 4;
                            break;
                        case "quote":
                            pos = 5;
                            break;
                    }
                    var set = value[i].Split('|');
                    if (pos != 5)
                    {
                        int.Parse(newVal);
                    }
                    set[pos] = newVal;
                    changeable = string.Join("|", set);
                    value[i] = changeable;
                    break;
                }
            }
            ////new Classes.XmlReader().WriteToTextFile(textFileLocation, value.ToArray());
            return value[rowChange];
        }
    }
}
