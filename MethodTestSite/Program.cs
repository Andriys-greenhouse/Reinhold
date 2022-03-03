using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MethodTestSite
{
    class Program
    {
        static void Main(string[] args)
        {
            //random number
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
            
            //Tic tac toe
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
            Console.WriteLine("\n\n\n");

            //Google web search
            string keyWord = "Andrew";
            Console.WriteLine("Searching for: {0}\n\n", keyWord);
            List<MyWebSearchResult> res = WebModule.GoogleWebSearchResultConvert(WebModule.WebSearch(keyWord).Result);
            for (int i = 0; i < res.Count; i++)
            {
                Console.WriteLine(res[i].OutputText(i+1) + "\n");
            }
            Console.WriteLine("\n\n\n");

            
            //format words
            Dictionary<char, List<string>> data = new Dictionary<char, List<string>>();
            string[] lines;
            char last = ' ';
            using (StreamReader sr = new StreamReader("wordList.txt"))
            {
                lines = sr.ReadToEnd().Replace('\r',' ').Split('\n');
            }
            foreach(string line in lines)
            {
                if (last != line.ToLower()[0])
                {
                    last = line.ToLower()[0];
                    data.Add(last, new List<string>());
                }
                data[last].Add(line);
            }
            foreach (List<string> list in data)
            {
                last = list[0].ToLower()[0];
                StringBuilder sb = new StringBuilder();
                foreach (var item in list)
                {
                    if (list.IndexOf(item) != 0) { sb.Append('\n'); }
                    sb.Append(item);
                }
                using (StreamWriter sw = new StreamWriter($"{last}.txt"))
                {
                    sw.Write(sb.ToString());
                }

                Console.WriteLine($"{last} written");
            }

            //Word soccer
            WordSoccer wscrGame = WordSoccer.SingleInstance;
            string input;
            Console.WriteLine("computer: {0}", "Before we start, I would like to point out, that I can't recognize, weather what you input are real english words or just set of characters but I will beleave you, that you won't play unfair...");
            do
            {
                input = Console.ReadLine();
                Console.WriteLine("computer: {0}", wscrGame.MakeMove(input));
            } while (wscrGame.GameRuning);
            Console.WriteLine("I don't know any other word...\nYOU WIN");

            //Website search
            Console.WriteLine(WebModule.WebsiteSearch("https://www.ssps.cz/", "suplovani", 1).Result);

            //News
            Console.WriteLine(WebModule.News("Czech").Result);

            Console.ReadLine();
        }
    }
}
