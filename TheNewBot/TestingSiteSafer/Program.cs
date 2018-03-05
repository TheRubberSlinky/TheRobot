using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSiteSafer
{
    class Program
    {
        static void Main(string[] args)
        {
            string original = "";

            Console.WriteLine("Original = " + original);
            string res = TheNewBot.Classes.mmmPepper.PepperMe("12345678901234567890", 5);
            Console.WriteLine("Peppered = " + res);

            Console.WriteLine("Cumin = " + TheNewBot.Classes.mmmPepper.Cumin(res, 5));
            Console.ReadLine();
        }
    }
}
