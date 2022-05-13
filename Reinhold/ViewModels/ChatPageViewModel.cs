using CoreLab;
using MethodTestSite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Reinhold.ViewModels
{
    class ChatPageViewModel : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static Data DataOfApplicationConnector { get; set; } = (App.Current as App).DataOfApplication;
        static Random rnd = new Random();
        static Regex nameSearchRx = new Regex(@"[A-Z][a-z]+");
        static Regex phoneNumberRx = new Regex(@"\+\d{12}|\d{9}");
        static Regex dateRx = new Regex(@"\d{4}\/\d{2}\/\d{2}");
        string dateForamt = "yyyy/MM/dd";
        public WordSoccer wscrGame = WordSoccer.GetWordSoccerGame(wordList);
        static Stream wordList = FileSystem.OpenAppPackageFileAsync("wordList.txt").Result;

        public bool MsgBxFocused { get; set; } = false;
        string msgBxText;
        public string MsgBxText
        {
            get { return msgBxText; }
            set
            {
                msgBxText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MsgBxText)));
            }
        }

        public Command ButtonClick { get; set; }
        public Command UpdateIcon { get; set; }

        public string MicOrSendButtonIcon
        {
            get
            {
                try
                {
                    return MsgBxFocused || MsgBxText.Length > 0 ? DataOfApplicationConnector.ArrowIcon : DataOfApplicationConnector.MicIcon;
                }
                catch (Exception)
                {
                    return DataOfApplicationConnector.MicIcon;
                }
            }
        }

        public ChatPageViewModel()
        {
            ButtonClick = new Command(OnButtonClick);
            UpdateIcon = new Command(OnUpdateIcon);
            MsgBxText = "";
        }

        public async void OnUpdateIcon()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MicOrSendButtonIcon)));
        }

        public async void OnButtonClick()
        {
            Methods.StopReading();
            if (MsgBxText != null && MsgBxText.Length > 0)
            {
                DataOfApplicationConnector.Messages.Add(new Message(MsgBxText, true));

                //core action
                string output = "newly created output";
                AnalysisResult coreResult;
                if (MsgBxText[0] != '|') { coreResult = DataOfApplicationConnector.Core.Process(MsgBxText); }
                else { coreResult = new AnalysisResult() { Context = "command", Intent = MsgBxText.Substring(1, MsgBxText.Length - 1), PastContext = "command" }; }

                output = ActionOnInput(coreResult, MsgBxText).Result;

                DataOfApplicationConnector.Messages.Add(new Message(output, false));
            }
            else //if (!MessageBox.IsFocused && (MessageBox.Text.Length == 0 || == null))
            {
                DataOfApplicationConnector.Messages.Add(new Message("Sorry, I can't yet process voice input...", false));
            }
        }

        private async Task<string> ActionOnInput(AnalysisResult coreResult, string message)
        {
            string output = "newly created output";
            string defaultString = "Hm, I am not shure that I understand, could you say that in different words please?", search;
            bool found;
            int num;
            List<string> toRemove;
            StringBuilder sb = new StringBuilder();
            try
            {
                switch (coreResult.Intent)
                {
                    case "indefinite":
                        output = defaultString;
                        break; //I can't understand
                    case "about":
                        output = "I am Reinhold. I was made as maturity exam project. If I can entertain you anyhow it will be my pleashure.";
                        break;
                    case "preferences":
                        output = "As a part of programm I don't currently have any preferences or opinions.";
                        break; //of Reinhold (what do you like -> I don't have an opinion on that) + esteg bib
                    case "greet":
                        output = "Hello";
                        break;
                    case "mood":
                        output = "Still the same I want to serve my purpose";
                        break; //how are you
                               // ? case "weather": break;
                    case "residence":
                        output = "I live in your phone currently";
                        break; //where do you live
                    case "sugestSearch":
                        output = "Should I search for that on google?";
                        break;
                    case "interjection":
                        output = (new string[] { "uh", "oh", "wow", "really?", "hmm" })[rnd.Next(0, 5)];
                        break; //uh, oh, wow, really?, hm and similar
                    case "approval":
                        output = (new string[] { "ok", "allright", "yes" })[rnd.Next(0, 3)];
                        break; //OK, allright, yes
                    case "call":
                        string number = "";
                        MatchCollection recievers = phoneNumberRx.Matches(message);
                        if (recievers.Count > 0)
                        {
                            number = recievers[0].Value;
                        }
                        else
                        {
                            recievers = nameSearchRx.Matches(message);
                            List<string> searchName = new List<string>();
                            foreach (Match mch in recievers)
                            {
                                if (mch.Index > 1)
                                {
                                    searchName.Add(mch.Value);
                                }
                            }
                            Contact bestMatch = null;
                            int highestCount = 0;
                            int count;
                            foreach (Contact cnt in await Contacts.GetAllAsync())
                            {
                                count = 0;
                                foreach (string name in searchName)
                                {
                                    if (cnt.DisplayName.Contains(name)) { count++; }
                                }
                                if (count > highestCount)
                                {
                                    highestCount = count;
                                    bestMatch = cnt;
                                }
                            }
                            if (bestMatch != null) { number = bestMatch.Phones[0].PhoneNumber; }
                        }
                        if (number != "")
                        {
                            PhoneDialer.Open(number);
                        }
                        break;
                    case "calendar":
                        MatchCollection dates = dateRx.Matches(message);
                        DateTime date = new DateTime();
                        if (message.Contains("tomorrow"))
                        {
                            date = DateTime.Today.AddDays(1);
                        }
                        else if (message.Contains("today"))
                        {
                            date = DateTime.Today;
                        }
                        else if (dates.Count > 0)
                        {
                            date = DateTime.ParseExact(dates[0].Value, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                            //output = //použít callendar
                            output = $"Oh, I see, you want me to display events for {date.ToString("yyyy/MM/dd")} but unfortunately I am not yet able to do that.";
                        }
                        if (date != new DateTime())
                        {
                            output = $"Oh, I see, you want me to display events for {date.ToString("yyyy/MM/dd")} but unfortunately I am not yet able to do that.";
                        }
                        else { output = "Hmm, I can't find a date to search in the callendar (format yyyy/MM/dd)."; }
                        break;
                    case "calendarAdd":
                        output = "Oh, I see, you want me to add to your callendar an event but unfortunately I am not yet able to do this, sorry.";
                        break;
                    case "wordSoccer":
                        if (coreResult.PastContext != "wordSoccer" || message == "new game" || message == "again")
                        {
                            output = "Before we start playing word soccer, I would like to point out, that I can't recognize, weather what you inputs are going to be meaningfull words or just set of characters but I will beleave you, that you won't play unfair...\nOk, go ahead you start.";
                            wscrGame.NewGame();
                        }
                        else
                        {
                            output = wscrGame.MakeMove(message);
                            if (output == null) { output = "Hmm, I can't think of any other... you win."; }
                        }
                        break;
                    case "ticTacToe":
                        output = "Unfortunately I am not yet able to play tic tac toe... maybe in some future version";
                        break;
                    case "RPS":
                        if (coreResult.PastContext != "RPS")
                        {
                            output = "Ok, input your word. Don't worry I won't cheat, I'll pick my one at random.";
                        }
                        else
                        {
                            RockPaperScissors move = RockPaperScissors.Rock;
                            bool setted = false;
                            if (message.ToLower().Contains("rock"))
                            {
                                move = RockPaperScissors.Rock;
                                setted = true;
                            }
                            else if (message.ToLower().Contains("paper"))
                            {
                                move = RockPaperScissors.Paper;
                                setted = true;
                            }
                            else if (message.ToLower().Contains("scissors"))
                            {
                                move = RockPaperScissors.Scissors;
                                setted = true;
                            }
                            if (!setted)
                            {
                                output = "Hm, I can't get weather your word is rock, paper or scissors...";
                            }
                            else
                            {
                                RockPaperScissors my = RPSModule.RPSGenerate();
                                GameResult res = RPSModule.RPSEvaluator(move, my);
                                if (res == GameResult.Win)
                                {
                                    output = $"You: {move}    Me: {my}\nI won, yay!";
                                }
                                else if (res == GameResult.Lose)
                                {
                                    output = $"You: {move}    Me: {my}\nHmm, clever, you won.";
                                }
                                else if (res == GameResult.Draw)
                                {
                                    output = $"You: {move}    Me: {my}\nIt is a draw...";
                                }
                            }
                        }
                        break;
                    case "guessToTen":
                        num = -1;
                        found = false;
                        foreach (string word in Regex.Replace(message, @"[^0-9a-zA-Z\ ]+", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (int.TryParse(word, out num))
                            {
                                found = true;
                            }
                        }
                        if (found)
                        {
                            output = RandomModule.GuessToTen(num) == GameResult.Win ? "You got it right!" : "Nope not this time.";
                        }
                        else
                        {
                            if (coreResult.PastContext == "guessToTen")
                            {
                                output = "Hmm, I haven't found any valid input for game of guess to ten.";
                            }
                            else
                            {
                                output = "Ok, please input your number.";
                            }
                        }
                        break;
                    case "rndNum":
                        output = $"Ok, here is a random number: {RandomModule.RandomNumber(1, 10, false)}";
                        break;
                    case "quote":
                        search = message;
                        toRemove = new List<string> { "quote", "Quote" };
                        foreach (string word in toRemove)
                        {
                            search = search.Replace(word, "");
                        }
                        if (search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 0)
                        {
                            output = await Methods.RandomBookQuote(search, DataOfApplicationConnector.Books);
                        }
                        else
                        {
                            if (coreResult.PastContext == "quote")
                            {
                                output = "Hmm, I haven't found any valid input to search quote for.";
                            }
                            else
                            {
                                output = "Ok, please input book.";
                            }
                        }
                        break;
                    case "rndStory":
                        output = $"Here you have a random story of yours:\n{DataOfApplicationConnector.Stories[rnd.Next(0, DataOfApplicationConnector.Stories.Count)].Text}";
                        break;
                    case "battery":
                        output = $"{Battery.ChargeLevel * 100}% of battery left";
                        break;
                    case "time":
                        output = $"The time is {DateTime.Now.ToString("HH:mm ss")}";
                        break;
                    case "search":
                        search = message;
                        toRemove = new List<string> { "search" };
                        foreach (string word in toRemove)
                        {
                            search = search.Replace(word, "");
                        }
                        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                        {
                            if (search.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 0)
                            {
                                sb.Clear();
                                sb.Append($"Searching for: {search}\n\n");
                                List<MyWebSearchResult> res = WebModule.GoogleWebSearchResultConvert(await WebModule.WebSearch(search));
                                for (int i = 0; i < res.Count; i++)
                                {
                                    sb.Append(res[i].OutputText(i + 1));
                                    sb.Append("\n");
                                }
                                output = sb.ToString();
                            }
                            else
                            {
                                if (coreResult.PastContext == "search")
                                {
                                    output = "Hmm, I haven't found any valid input to search for.";
                                }
                                else
                                {
                                    output = "Ok, please input keyword to be searched.";
                                }
                            }
                        }
                        else
                        {
                            output = "I can't connect to the internet to perform a search :-(";
                        }
                        break;
                    case "news":
                        if (coreResult.PastContext == "news")
                        {
                            output = await WebModule.News(message);
                        }
                        else
                        {
                            output = "Input keyword to search news for.";
                        }
                        break;
                    case "TTS":
                        output = "Ok, I am speaking now!";
                        await Methods.ReadOutLoud(message);
                        break;
                    case "rndQuestion":
                        output = (new string[] { "Hello :-)", "How are you?", "Hi, what are you doing?", "Was your day good sofar?", "Is there a place where instead of rain there are birds falling from the sky?" })[rnd.Next(0, 5)];
                        break;

                    default:
                        output = defaultString;
                        break;
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }

            return output;
        }
    }
}
