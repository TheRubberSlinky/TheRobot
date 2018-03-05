using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheNewBot.Classes
{
    class TicTacAI
    {
        private int difficultyp;
        Random rnd = new Random();
        public void SetDifficulty(int difficulty)
        {
            difficultyp = difficulty * 1;
        }
        public string[,] Place(string[,] BattleArena, bool XorO)
        {
            string choice = XorO ? "X" : "O";

            int[,] posPos = new int[3, 3];
            
            for(int x = 0; x < 3; x++)
            {
                for(int y = 0; y < 3; y++)
                {
                    posPos[x, y] = (BattleArena[x, y] == choice) ? 1 : (BattleArena[x,y] == " " ? 0: -5);
                }
            }


            //first see if any can make me win... if not, check if i can cockblock my opponent. if neither, random spot
            //lets add a difficulty slider, simple value, which will determine percentage
            //check first row for 2 out of 3, then second row, then colums, then diagonals

            bool Complete = false;

            for(int x = 0; x < 3; x++)
            {
                if (posPos[x, 0] + posPos[x, 1] + posPos[x, 2] > 1)
                    if (chanceIt())
                    { if (posPos[x, 0] == 0) { posPos[x, 0] = 1; Complete = true; break; } if (posPos[x, 1] == 0) { posPos[x, 1] = 1; Complete = true; break; } if (posPos[x, 2] == 0) { posPos[x, 2] = 1; Complete = true; break; } }
                if (posPos[0, x] + posPos[1, x] + posPos[2, x] > 1)
                    if (chanceIt())
                    { if (posPos[0, x] == 0) { posPos[0, x] = 1; Complete = true; break; } if (posPos[1, x] == 0) { posPos[1, x] = 1; Complete = true; break; } if (posPos[2, x] == 0) { posPos[2, x] = 1; Complete = true; break; } }
            }
            if (posPos[0, 0] + posPos[1, 1] + posPos[2, 2] > 1)
                if (chanceIt())
                { if (posPos[0, 0] == 0) { posPos[0, 0] = 1; Complete = true; } if (posPos[1, 1] == 0) { posPos[1, 1] = 1; Complete = true; } if (posPos[2, 2] == 0) { posPos[2, 2] = 1; Complete = true; } }
            if (posPos[0, 2] + posPos[1, 1] + posPos[2, 0] > 1)
                if (chanceIt())
                { if (posPos[0, 2] == 0) { posPos[0, 2] = 1; Complete = true; } if (posPos[1, 1] == 0) { posPos[1, 1] = 1; Complete = true; } if (posPos[2, 0] == 0) { posPos[2, 0] = 1; Complete = true; } }
                        

            if(!Complete)
            {
                //alright, no winning moves, time to cockblock
                for (int x = 0; x < 3; x++)
                {
                    if (posPos[x, 0] + posPos[x, 1] + posPos[x, 2] < -9)
                        if (chanceIt())
                        {
                            if (posPos[x, 0] == 0) { posPos[x, 0] = 1; Complete = true; break; }
                            if (posPos[x, 1] == 0) { posPos[x, 1] = 1; Complete = true; break; }
                            if (posPos[x, 2] == 0) { posPos[x, 2] = 1; Complete = true; break; }
                        }
                    if (posPos[0, x] + posPos[1, x] + posPos[2, x] < -9)
                        if (chanceIt())
                        {
                            if (posPos[0, x] == 0) { posPos[0, x] = 1; Complete = true; break; }
                            if (posPos[1, x] == 0) { posPos[1, x] = 1; Complete = true; break; }
                            if (posPos[2, x] == 0) { posPos[2, x] = 1; Complete = true; break; }
                        }
                }
                if (posPos[0, 0] + posPos[1, 1] + posPos[2, 2] < -9)
                    if (chanceIt())
                    {
                        if (posPos[0, 0] == 0) { posPos[0, 0] = 1; Complete = true; }
                        if (posPos[1, 1] == 0) { posPos[1, 1] = 1; Complete = true; }
                        if (posPos[2, 2] == 0) { posPos[2, 2] = 1; Complete = true; }
                    }
                if (posPos[0, 2] + posPos[1, 1] + posPos[2, 0] < -9)
                    if (chanceIt())
                    {
                        if (posPos[0, 2] == 0) { posPos[0, 2] = 1; Complete = true; }
                        if (posPos[1, 1] == 0) { posPos[1, 1] = 1; Complete = true; }
                        if (posPos[2, 0] == 0) { posPos[2, 0] = 1; Complete = true; }
                    }

            }

            if(!Complete)
            {
                //still nothing? random move it is
                posPos = RandomSpot(posPos);
            }

            string[,] Arena = new string[3,3];
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    Arena[x, y] = ((posPos[x, y] == 1) ? choice : (posPos[x, y] == 0 ? " " : (XorO ? "O": "X")));
                }
            }
            return Arena;
        }

        private int[,] RandomSpot(int[,] posPos)
        {
            bool found = false;
            while (!found)
            {
                int x = rnd.Next(3), y = rnd.Next(3);
                if (posPos[x, y] == 0)
                {
                    posPos[x, y] = 1;
                    found = true;
                }
            }
            return posPos;
        }

        private bool chanceIt()
        {
            //take random, divide by random and subtract by random then add random. if result < difficulty, chance
            if(rnd.Next(0, difficultyp +10) < difficultyp)
                return true;
            return false;
        }
    }
}
