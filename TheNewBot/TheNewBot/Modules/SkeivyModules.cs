using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TheNewBot.Classes;
using Discord.Addons.Interactive;
using System.Net.Http;
using Newtonsoft.Json;

namespace TheNewBot.Modules
{
    [Group("skv")]
    public class SkeivyModules : InteractiveBase
    {
        private static readonly HttpClient client = new HttpClient();

        [Command("trivia", RunMode = RunMode.Async), Summary("Prompts the bot to ask trivia questions"),]
        [Alias("quiz")]
        public async Task doTriva(string difficulty = "easy") //added parameter to choose a difficulty
        {
            string correctAnswerNumber = "";
            string actualAnswer = "";
            string category = "";
            string choices = "";

            switch (difficulty.ToLower()) //this will filter the difficulty so it doesnt break lower down
            {
                case "easy":
                    difficulty = "easy";
                    break;
                case "medium":
                    difficulty = "medium";
                    break;
                case "hard":
                    difficulty = "hard";
                    break;
                default:
                    difficulty = "easy";
                    break;
            }

            RequestOptions opts = new RequestOptions();
            opts.Timeout = 300;



            using (Context.Channel.EnterTypingState())
            {
                await ReplyAsync($"Choose a topic:\n" +
                                                   $"1 : Games\n" +
                                                   $"2 : film\n" +
                                                   $"3: music\n" +
                                                   $"4: television");
            }
            

            bool game = true; //create a while loop to view the response, see if they actually replied in time, and if it begins
                              //with the character provided
            while (game)
            {
                var response = await NextMessageAsync();

                if (response != null)
                {
                    if (response.Content.StartsWith("#"))
                    {
                        await Task.Run(async () =>
                    {

                        switch (response.Content.Substring(1,1))
                        {
                            case "1":
                                category = "15";
                                break;
                            case "2":
                                category = "11";
                                break;
                            case "3":
                                category = "12";
                                break;
                            case "4":
                                category = "14";
                                break;
                            default:
                                await ReplyAsync("incorrect choice");
                                return;
                        }

                        var json = await client.GetStringAsync($"https://opentdb.com/api.php?amount=1&category={category}&difficulty="+difficulty+"&type=multiple"); //turned the difficulty dynamic

                        Trivia theTrivia = JsonConvert.DeserializeObject<Trivia>(json);


                        using (Context.Channel.EnterTypingState())
                        {
                            await ReplyAsync(theTrivia.results[0].question);
                        }

                        int i = 1;
                        int place = new Random(DateTime.Now.Millisecond).Next(1, 5);

                        correctAnswerNumber = place.ToString();
                        actualAnswer = theTrivia.results[0].correct_answer;
                        foreach (string s in theTrivia.results[0].incorrect_answers)
                        {
                            if (place == i)
                            {
                                choices += $"{i}:  " + actualAnswer + "\n";
                                ++i;
                                choices += $"{i}:  " + s + "\n";
                                ++i;
                            }
                            else
                            {
                                choices += $"{i}:  " + s + "\n";
                                ++i;
                            }

                        }
                    });

                        using (Context.Channel.EnterTypingState())
                        {
                            await ReplyAsync(choices);
                        }
                        bool reply = true; //same idea as the above while loop, to see when the reply is what we want andn ot just a normal reply
                        while (reply)
                        {
                            response = await NextMessageAsync();
                    if (response != null)
                        if (response.Content.StartsWith("#"))
                            if (response.Content.Substring(1,1) == correctAnswerNumber)
                            {
                                using (Context.Channel.EnterTypingState())
                                {
                                    await ReplyAsync("That is correct!");
                                    reply = false;
                                }
                            }
                            else
                            {
                                using (Context.Channel.EnterTypingState())
                                {
                                    await ReplyAsync($"The is incorrect, the answer is:  {actualAnswer}.\n Better luck next time.");
                                    reply = false;
                                }
                            }
                        }
                    }
                }
                else
                { //if they didnt reply in time, just end it
                    game = false;
                }
            }
        }




        [Command("next", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            await ReplyAsync("What is 2+2?");
            var response = await NextMessageAsync();
            if (response != null)
                await ReplyAsync($"You replied: {response.Content}");
            else
                await ReplyAsync("You did not reply before the timeout");
        }

        [Command("kill", RunMode = RunMode.Async), Summary("Kills the bot"),]
        [Alias("quit")]
        public async Task Kill()
        {
            Environment.Exit(0);
        }
    }
}
