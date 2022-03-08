using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace MethodTestSite
{
    public class WordSoccer
    {
        static WordSoccer singleInstance;
        public static WordSoccer SingleInstance
        {
            get
            {
                if(singleInstance == null)
                {
                    singleInstance = new WordSoccer();
                }
                return singleInstance;
            }
        }

        Dictionary<char, List<string>> Vocabrulary { get; set; }
        static Random rnd = new Random();
        List<string> UsedWords;
        static string alphabet { get { return "abcdefghijklmnopqrstuvwxyz"; } }
        string lastWord;
        bool gameRuning;
        public bool GameRuning { get { return gameRuning; } }

        private WordSoccer()
        {
            NewGame();
        }

        public void NewGame()
        {
            gameRuning = true;
            UsedWords = new List<string>();
            Vocabrulary = new Dictionary<char, List<string>>();

            string[] lines;
            char last = ' ';

            //somehow get insides of a wordList.txt
            using (StreamReader sr = new StreamReader("wordList.txt"))
            {
                lines = sr.ReadToEnd().Replace('\r', ' ').Split('\n');
            }

            for (int i = 0; i < lines.Length; i++)
            {
                if (last != lines[i].ToLower()[0] && !(lines[i] == lines[i == 0 ? 0 : i - 1] + "s" || lines[i] + "s" == lines[i == 0 ? 0 : i - 1]))
                {
                    last = lines[i].ToLower()[0];
                    Vocabrulary.Add(last, new List<string>());
                }
                Vocabrulary[last].Add(lines[i]);
            }
        }

        public string MakeMove(string word = "")//leave "" for starting move
        {
            if (!gameRuning) { throw new ArgumentException("This game has already ended."); }
            if (word == null) { throw new ArgumentException("Word can't be null."); }

            char letter;
            if(word.Length < 1) { letter = alphabet[rnd.Next(0, alphabet.Length)]; }
            else 
            {
                if (lastWord != null && word[0] != lastWord[lastWord.Length - 1]) { throw new ArgumentException("Hey, word must start by the letter, by which the prewious ends."); }
                if (UsedWords.Contains(word)) { throw new ArgumentException("This word was already used in this game."); }
                if (!IsValidWord(word)) { throw new ArgumentException("Word must be composed of only alphabet characters and occasional dashes."); }
                letter = word[word.Length - 1]; 
            }

            if(Vocabrulary[letter].Count == 0) //game over - computer lost
            {
                gameRuning = false;
                return null; 
            } 

            int pickedIndex;
            do
            {
                pickedIndex = rnd.Next(0, Vocabrulary[letter].Count);
            } while (UsedWords.Contains(Vocabrulary[letter][pickedIndex]));

            string picked = Vocabrulary[letter][pickedIndex];

            if (word != "") { UsedWords.Add(word); }
            Vocabrulary[letter].Remove(picked);
            UsedWords.Add(picked);
            lastWord = picked.Trim();
            return picked;
        }

        bool IsValidWord(string word)
        {
            Regex rx = new Regex(@"^([a-z]|[A-Z]|-)*$");
            return rx.IsMatch(word);
        }
    }
}
