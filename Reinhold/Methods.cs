using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Reinhold
{
    class Methods
    {
        public string CallNumber(string Number)
        {
            try
            {
                PhoneDialer.Open(Number);
            }
            catch (Exception ex)
            {
                return "I am sorry, something went wrong. I am unable to call this number.";
            }
            return $"Calling {Number}.";
        }

        public async Task<string> FindContact(string Contact)
        {
            List<Contact> contacts = new List<Contact>();
            StringBuilder sb = new StringBuilder("");
            foreach (Contact cont in Contacts.GetAllAsync().Result)
            {
                if (cont.DisplayName.Contains(Contact)) 
                { 
                    sb.Append($"{cont.DisplayName} - {cont.Phones[0]}"); 
                }
            }

            return sb.ToString();
        }

        public string BatteryLevel()
        {
            return $"Battery charge is {Battery.ChargeLevel * 100}%";
        }
        public string Time()
        {
            return $"It is {DateTime.Now.ToString("HH:MM:ss")}";
        }
        public string Date()
        {
            return $"Today's date is {DateTime.Now.ToString("yyyy/MM/dd")}";
        }

        //from https://docs.microsoft.com/cs-cz/xamarin/essentials/text-to-speech
        static CancellationTokenSource cts;
        public static async Task ReadOutLoud(string Message)
        {
            cts = new CancellationTokenSource();
            await TextToSpeech.SpeakAsync(Message, cancelToken: cts.Token);
        }
        public static void StopReading()
        {
            if (cts?.IsCancellationRequested ?? true) { return; }

            cts.Cancel();
        }

        static Random rnd = new Random();
        public static async Task<string> RandomBookQuote(string searchTerm, ObservableCollection<Book> availableBooks) //if local search isn't wanted pass as argument empty ObservableCollection
        {
            foreach (Book bk in availableBooks)
            {
                if (bk.Title.Contains(searchTerm) || bk.AuthorsCompleteName.Contains(searchTerm)) { return $"(from local data)\n{bk.FavouriteQuote}"; }
            }

            Regex rx = new Regex(@"&ldquo;(?<Quote>.*)&rdquo;");
            MatchCollection mc;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync("https://www.goodreads.com/quotes/search?utf8=%E2%9C%93&commit=Search&q=" + searchTerm.Replace(" ", "+")).Result;
                mc = rx.Matches(await response.Content.ReadAsStringAsync());
            }
            if (mc.Count == 0) { return "Sorry I can't find any quotes for this book..."; }
            List<string> toPickFrom = new List<string>();
            for (int i = 0; i < (mc.Count < 5 ? mc.Count : 5); i++) //pick from 5 at most
            {
                toPickFrom.Add(mc[i].Groups["Quote"].Value.Replace("<br />", "\n"));
            }
            return $"(from goodreads.com)\n{toPickFrom[rnd.Next(0, toPickFrom.Count)]}";
        }
        public static string RandomStoryWithPerson(string SearchedName, ObservableCollection<Story> availableStories)
        {
            List<Story> toPickFrom = new List<Story>();
            foreach (Story story in availableStories)
            {
                foreach (Acquaintance person in story.People)
                {
                    if (person.FullName.Contains(SearchedName)) { toPickFrom.Add(story); }
                }
            }
            Story picked = toPickFrom[rnd.Next(0, toPickFrom.Count)];
            return $"({picked.Date})\n{picked.Text}";
        }

        public static string AboutReinhold
        {
            get { return "My name is Reinhold and I have been programmed for you to have a fun... or maybe just for my creator to make a project for Maturita exam... anyway I am now at your disposal, if you don't find my pre-programmed answers entertaining, what about some book quote searching or playing a game of Word soccer?"; }
        }

        public static string[] RandomQuestions
        {
            get
            {
                return new string[]
                {
                    "Hi, how are you?",
                    "How long can a turtle swim under water without breathing air?",
                    "How was your day sofar?",
                    $"Do you like {(new string[] { "red", "green", "blue", "orange", "purple", "yellow", "brown" })[rnd.Next(0,7)]} color?",
                    $"Got any new {(new string[] { "friends", "books", "stories"})[rnd.Next(0,3)]}, which you could tell me about?",
                    $"What about a game of {(new string[] { "Word soccer", "Tic tac toe", "Rock paper scissors"})[rnd.Next(0,3)]}?"
                };
            }
        }
    }
}
