using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodTestSite
{
    class Program
    {
        static void Main(string[] args)
        {
            double max = 10.9;
            bool whole = false;
            double min = -10;

            double highest = RandomModule.RandomNumber(min, max, whole);
            double lowest = RandomModule.RandomNumber(min, max, whole);
            double current;

            for (int i = 0; i < 100; i++)
            {
                current = RandomModule.RandomNumber(min, max, whole);
                if (highest < current) { highest = current; }
                if (lowest > current) { lowest = current; }
                //Console.WriteLine(current);
            }

            Console.WriteLine($"Max: {highest}\nMin: {lowest}");
            Console.WriteLine("\n\n\n");

            TicTacToe tct = new TicTacToe(4, 5, 3, Console.WriteLine);

            while (tct.EvaluateGame() == WinningParty.None)
            {
                try
                {
                    tct.MakeMove(Console.ReadLine());
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
