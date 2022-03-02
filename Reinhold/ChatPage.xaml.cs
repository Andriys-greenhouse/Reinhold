using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CoreLab;
using MethodTestSite;
using Xamarin.Essentials;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage, INotifyPropertyChanged
    {
        public Data DataOfApplicationConnector { get; set; }
        Random rnd = new Random();
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
        }

        private void MicOrSendButton_Clicked(object sender, EventArgs e)
        {
            if (MessageBox.IsFocused || (MessageBox != null && MessageBox.Text.Length > 0))
            {
                DataOfApplicationConnector.Messages.Messages.Add(new Message(MessageBox.Text, true));
                MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
                MicOrSendButton.Source = MicOrSendButtonIcon;
                //core action
                /*
                string output = "newly created output";
                AnalysisResult coreResult;
                if (MessageBox.Text[0] != '|') { coreResult = DataOfApplicationConnector.Core.Process(MessageBox.Text); }
                else { coreResult = new AnalysisResult() { Context = "command", Intent = MessageBox.Text.Substring(1, MessageBox.Text.Length - 1), PastContext = "command" }; }
                switch (coreResult.Intent)
                {
                    case "indefinite":
                        output = "Hm, I am not shure that I understand, could you say that in different words please?";
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
                    case "weather": break;
                    case "residence":
                        output = "I live in your phone currently";
                        break; //where do you live
                    case "sugestSearch":
                        output = "Should I search for that on google?";
                        break;
                    case "interjection":
                        output = (new string[] { "uh", "oh", "wow", "really?", "hmm" })[rnd.Next(0,5)];
                        break; //uh, oh, wow, really?, hm and similar
                    case "approval":
                        output = (new string[] { "ok", "allright", "yes" })[rnd.Next(0, 3)];
                        break; //OK, allright, yes
                    case "call": break;
                    case "calendar": break;
                    case "calendarAdd": break;
                    case "wordSoccer": break;
                    case "ticTacToe": break;
                    case "RPS": break;
                    case "guessToTen": break;
                    case "rndNum":
                        output = $"Ok, here is a random number: {RandomModule.RandomNumber(1,10,false)}";
                        break;
                    case "quote": break;
                    case "rndStory":
                        output = $"Here you have a random story of yours:\n{DataOfApplicationConnector.Stories[rnd.Next(0, DataOfApplicationConnector.Stories.Count)].Text}";
                        break;
                    case "battery":
                        output = $"{Battery.ChargeLevel * 100}% of battery left";
                        break;
                    case "time":
                        output = $"The time is {DateTime.Now.ToString("HH:mm ss")}";
                        break;
                    case "search": break;
                    case "news": break;
                    case "TTS":
                        TextToSpeech.SpeakAsync(MessageBox.Text);
                        break;
                    case "rndQuestion":
                        output = (new string[] { "uh", "oh", "wow", "really?", "hmm" })[rnd.Next(0, 5)];
                        break;

                    default:
                        output = "Hm, I am not shure that I understand, could you say that in different words please?";
                        break;
                }

                DataOfApplicationConnector.Messages.Messages.Add(new Message(output, false));*/
                MessageBox.Text = "";
                MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
                MicOrSendButton.Source = MicOrSendButtonIcon;
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
    }
}