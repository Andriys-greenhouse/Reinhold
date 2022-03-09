using CoreLab;
using MethodTestSite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Reinhold
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage, INotifyPropertyChanged
    {
        public Data DataOfApplicationConnector { get; set; }
        Random rnd = new Random();
        Regex nameSearchRx = new Regex(@"[A-Z][a-z]+");
        Regex phoneNumberRx = new Regex(@"\+\d{12}|\d{9}");
        Regex dateRx = new Regex(@"\d{4}\/\d{2}\/\d{2}");
        string dateForamt = "yyyy/MM/dd";
        WordSoccer wscrGame;
        Stream wordList = FileSystem.OpenAppPackageFileAsync("wordList.txt").Result;

        public string MicOrSendButtonIcon
        {
            get
            {
                try
                {
                    return MessageBox.IsFocused || MessageBox.Text.Length > 0 ? DataOfApplicationConnector.ArrowIcon : DataOfApplicationConnector.MicIcon;
                }
                catch (Exception)
                {
                    return DataOfApplicationConnector.MicIcon;
                }
            }
        }
        public ChatPage()
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
            MicOrSendButton.BindingContext = this;

            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
            wscrGame = WordSoccer.GetWordSoccerGame(wordList);
        }

        private async void MicOrSendButton_Clicked(object sender, EventArgs e)
        {
            Methods.StopReading();
            if (MessageBox.IsFocused || (MessageBox != null && MessageBox.Text.Length > 0))
            {
                DataOfApplicationConnector.Messages.Messages.Add(new Message(MessageBox.Text, true));
                MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
                MicOrSendButton.Source = MicOrSendButtonIcon;
                //core action
                string output = "newly created output";
                AnalysisResult coreResult;
                if (MessageBox.Text[0] != '|') { coreResult = DataOfApplicationConnector.Core.Process(MessageBox.Text); }
                else { coreResult = new AnalysisResult() { Context = "command", Intent = MessageBox.Text.Substring(1, MessageBox.Text.Length - 1), PastContext = "command" }; }

                /*
                //according to context - commands that need more turns
                if (coreResult.PastContext == "RPS" && coreResult.Context != "default" && coreResult.Intent != "approval")
                {
                    RockPaperScissors input;
                    bool setted = false;
                    if (MessageBox.Text.ToLower().Contains("rock"))
                    {
                        input = RockPaperScissors.Rock;
                        setted = true;
                    }
                    else if (MessageBox.Text.ToLower().Contains("paper"))
                    {
                        input = RockPaperScissors.Paper;
                        setted = true;
                    }
                    else if (MessageBox.Text.ToLower().Contains("scissors"))
                    {
                        input = RockPaperScissors.Scissors;
                        setted = true;
                    }
                    if (!setted)
                    {
                        output = "Hmm, I can't get which word from rock, paper or scissors is your choice... let's leave it for now.";
                    }
                }
                */

                output = ActionOnInput(coreResult).Result;

                DataOfApplicationConnector.Messages.Messages.Add(new Message(output, false));
                MessageBox.Text = "";
                MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
                MicOrSendButton.Source = MicOrSendButtonIcon;
                MessageListView.ScrollTo(DataOfApplicationConnector.Messages.Messages[DataOfApplicationConnector.Messages.Messages.Count - 1], ScrollToPosition.End, false);
            }
            else if (!MessageBox.IsFocused && MessageBox.Text.Length > 0)
            {
                DataOfApplicationConnector.Messages.Messages.Add(new Message("Sorry, I can't yet process voice input...", false));
                MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
                MicOrSendButton.Source = MicOrSendButtonIcon;
                MessageListView.ScrollTo(DataOfApplicationConnector.Messages.Messages[DataOfApplicationConnector.Messages.Messages.Count - 1], ScrollToPosition.End, false);
            }
        }

        private void MessageBox_Focus(object sender, FocusEventArgs e)
        {
            MicOrSendButton.Source = MicOrSendButtonIcon;
            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
        }

        private void ChatPage_Appearing(object sender, EventArgs e)
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
            MicOrSendButton.BindingContext = this;
            MicOrSendButton.Source = MicOrSendButtonIcon;
            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
        }

        private async Task<string> ActionOnInput(AnalysisResult coreResult)
        {
            string output = "newly created output";
            string defaultString = "Hm, I am not shure that I understand, could you say that in different words please?", search;
            bool found;
            int num;
            List<string> toRemove;
            StringBuilder sb = new StringBuilder();
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
                    MatchCollection recievers = phoneNumberRx.Matches(MessageBox.Text);
                    if (recievers.Count > 0)
                    {
                        number = recievers[0].Value;
                    }
                    else
                    {
                        recievers = nameSearchRx.Matches(MessageBox.Text);
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
                    MatchCollection dates = dateRx.Matches(MessageBox.Text);
                    DateTime date = new DateTime();
                    if (MessageBox.Text.Contains("tomorrow"))
                    {
                        date = DateTime.Today.AddDays(1);
                    }
                    else if (MessageBox.Text.Contains("today"))
                    {
                        date = DateTime.Today;
                    }
                    else if (dates.Count > 0)
                    {
                        date = DateTime.ParseExact(dates[0].Value, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        //output = //použít callendar
                        output = $"Oh, I see, you want me to display events for {date.ToString("yyyy/MM/dd")} but unfortunately I am not yet able to do that.";
                    }
                    else { output = "Hmm, I can't find a date to search in the callendar (format yyyy/MM/dd)."; }
                    break;
                case "calendarAdd":
                    output = "Oh, I see, you want me to add to your callendar an event but unfortunately I am not yet able to do this, sorry.";
                    break;
                case "wordSoccer":
                    if (coreResult.PastContext != "wordSoccer")
                    {
                        output = "Before we start playing word soccer, I would like to point out, that I can't recognize, weather what you inputs are going to be meaningfull words or just set of characters but I will beleave you, that you won't play unfair...\nOk, go ahead you start.";
                        wscrGame.NewGame(wordList);
                    }
                    else
                    {
                        output = wscrGame.MakeMove(MessageBox.Text);
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
                        if (MessageBox.Text.ToLower().Contains("rock"))
                        {
                            move = RockPaperScissors.Rock;
                            setted = true;
                        }
                        else if (MessageBox.Text.ToLower().Contains("paper"))
                        {
                            move = RockPaperScissors.Paper;
                            setted = true;
                        }
                        else if (MessageBox.Text.ToLower().Contains("scissors"))
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
                    foreach (string word in Regex.Replace(MessageBox.Text, @"[^0-9a-zA-Z\ ]+", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
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
                    search = MessageBox.Text;
                    toRemove = new List<string> { "quote", "Quote" };
                    foreach (string word in toRemove)
                    {
                        search = search.Replace(word, "");
                    }
                    if (search.Length > 2)
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
                    search = MessageBox.Text;
                    toRemove = new List<string> { };
                    foreach (string word in toRemove)
                    {
                        search = search.Replace(word, "");
                    }
                    if (search.Length > 2)
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
                    break;
                case "news":
                    if (coreResult.PastContext == "news")
                    {
                        output = await WebModule.News(MessageBox.Text);
                    }
                    else
                    {
                        output = "Input keyword to search news for.";
                    }
                    break;
                case "TTS":
                    output = "Ok, I am speaking now!";
                    await Methods.ReadOutLoud(MessageBox.Text);
                    break;
                case "rndQuestion":
                    output = (new string[] { "Hello :-)", "How are you?", "Hi, what are you doing?", "Was your day good sofar?", "Is there a place where instead of rain there are birds falling from the sky?" })[rnd.Next(0, 5)];
                    break;

                default:
                    output = defaultString;
                    break;
            }
            return output;
        }
    }
}