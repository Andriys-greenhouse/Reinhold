using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodTestSite
{
    class WordSoccer
    {
        public static string[][] Vocabrulary;
        static Random rnd = new Random();
        public List<string> UsedWords;

        public WordSoccer()
        {
            //load Vocabrulary from Jsons
            //and remove all prular forms
        }

        /*
        public string MakeMove(char latter)
        {
            int pickedIndex;
            do
            {
                pickedIndex = rnd.Next(Vocabrulary.Length);
            } while (UsedWords.Contains(Vocabrulary[pickedIndex]));
        }
        */

    }
}
