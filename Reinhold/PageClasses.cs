using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace Reinhold
{
    public enum ColorWord { Red, Green, Blue }
    public enum Relation { Sibling, Parent, Grandparent, Aunt, Cousin, BrawderFamily, Friend, SuperiorCoworker, LevelCoworker, InferiorCoworker }
    public enum LevelOfLiking { Hatered, Dislike, Neutral, Pozitive, IsDear }

    public class Data
    {
        public ColorWord ColorScheme { get; set; }
        int searchDept;
        public int SearchDept
        {
            get { return searchDept; }
            set
            {
                if(value > 10 || value < 1) { throw new ArgumentException("Search dept must be between 1 and 10!"); }
            }
        }
        public bool DisplayNotifications { get; set; }

        public Person User { get; set; }
        public ObservableCollection<Acquaintance> Acquaintances { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Story> Stories { get; set; }

        [JsonIgnore]
        public MessageManager Messages { get; set; }

        public string ChatIcon { get { return $"{ColorScheme.ToString().ToLower()}Chat.png"; } }
        public string UserIcon { get { return $"{ColorScheme.ToString().ToLower()}User.png"; } }
        public string SettingsIcon { get { return $"{ColorScheme.ToString().ToLower()}Control.png"; } }
        public string ArrowIcon { get { return $"{ColorScheme.ToString().ToLower()}Arrow.png"; } }
        public string BookIcon { get { return $"{ColorScheme.ToString().ToLower()}Book.png"; } }
        public string DeleteIcon { get { return $"{ColorScheme.ToString().ToLower()}Delete.png"; } }
        public string FriendIcon { get { return $"{ColorScheme.ToString().ToLower()}Friend.png"; } }
        public string StoryIcon { get { return $"{ColorScheme.ToString().ToLower()}Labyrinth.png"; } }
        public string MenuIcon { get { return $"{ColorScheme.ToString().ToLower()}Menu.png"; } }
        public string MicIcon { get { return $"{ColorScheme.ToString().ToLower()}Mic.png"; } }


        public Color ColorSchemeInColor { get; set; }
    }

    public class Person
    {
        string id = "";
        public string ID
        {
            get { return id; }
            set
            {
                if(id == "") { id = DateTime.Now.Ticks.ToString(); }
                else { throw new ArgumentException("ID can be set only once!"); }
            }
        }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool DateIsAcurate { get; set; }
        public Place PlaceOfResidence { get; set; }
        public string Appearance { get; set; }
        public bool IsMan { get; set; }
        public ObservableCollection<string> Hobbys { get; set; }

        string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set
            {
                if (new Regex(@"^\+\d{12}$|^\d{9}$").IsMatch(value) || value == "")
                {
                    phoneNumber = value;
                }
                else { throw new ArgumentException("Invallid phone number!"); }
            }
        }
        string email;
        public string Email
        {
            get { return email; }
            set
            {
                Regex rx = new Regex(@"^(?<address>.*)\@(?<provider>.*)\.(?<domain>.{2,3})$");
                if (rx.IsMatch(value) || value == "")
                {
                    email = rx.Match(value).Groups[1].Value;
                }
                else { throw new ArgumentException("Invallid email!"); }
            }
        }
    }

    public class Acquaintance : Person
    {
        public Relation Relation { get; set; }
        public LevelOfLiking LevelOfRelarion { get; set; }
        int levelOfAcquaintance;
        public int LevelOfAcquaintance 
        {
            get { return levelOfAcquaintance; }
            set
            {
                if(value > 100 || value < 1) { throw new ArgumentException("Level of acquaintance must be between 1 and 100!"); }
            }
        }
    }

    public class Story
    {
        public DateTime Date { get; set; }
        public List<Acquaintance> People { get; set; }
        bool TextFetched = false;
        public string TextID;
        [JsonIgnore]
        string text;
        public string Text
        {
            get
            {
                if (!TextFetched)
                {
                    TextFetched = true;
                    text = FetchText().Result;
                }
                return text;
            }

            set
            {
                text = value;
                SaveText();
            }
        }

        async Task<string> FetchText()
        {
            try
            {
                return await SecureStorage.GetAsync(TextID);
            }
            catch (Exception e)
            {
                throw new Exception("Story SeSt Error!");
            }
        }

        async void SaveText()
        {
            try
            {
                await SecureStorage.SetAsync(TextID, text);
            }
            catch (Exception e)
            {
                throw new Exception("Story SeSt Error!");
            }
        }
    }

    public class Book
    {
        public string AuthorsCompleteName { get; set; }
        public string Title { get; set; }
        public LevelOfLiking Score { get; set; }
        bool QuoteFetched = false;
        public string QuotesID;
        string favouriteQuote;
        public string FavouriteQuote
        {
            get
            {
                if (!QuoteFetched)
                {
                    QuoteFetched = true;
                    favouriteQuote = FetchQuote().Result;
                }
                return favouriteQuote;
            }

            set
            {
                favouriteQuote = value;
                SaveQuote();
            }
        }

        async Task<string> FetchQuote()
        {
            try
            {
                return await SecureStorage.GetAsync(QuotesID);
            }
            catch (Exception e)
            {
                throw new Exception("Book SeSt Error!");
            }
        }

        async void SaveQuote()
        {
            try
            {
                await SecureStorage.SetAsync(QuotesID, favouriteQuote);
            }
            catch (Exception e)
            {
                throw new Exception("Book SeSt Error!");
            }
        }
    }

    public class Place
    {
        public string InWords { get; set; }
        public string GPS { get; set; }
        public float AproxymateDeviationOfGPSInMeters { get; set; }
        public Place(string aInWords, string aGPS = "", float aAproxymateDeviationInMeters = 1000)
        {
            InWords = aInWords;
            GPS = aGPS;
            AproxymateDeviationOfGPSInMeters = aAproxymateDeviationInMeters;
        }
    }

    public class Message
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool SendByUser { get; set; }

        public Message(string aText, bool aSendByUser)
        {
            Text = aText;
            Date = DateTime.Now;
            SendByUser = aSendByUser;
        }
    }

    public class DayOfMessages
    {
        public DateTime Date { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public DayOfMessages(IEnumerable<Message> aMessages)
        {
            bool first = true;
            foreach (Message item in aMessages)
            {
                if (first)
                {
                    first = false;
                    Date = item.Date;
                }

                if(Date.Year != item.Date.Year || Date.Month != item.Date.Month || Date.Day != item.Date.Day) 
                { 
                    throw new ArgumentException("Not all messages are from the same day!"); 
                }

                Messages.Add(item);
            }
        }
    }

    public class MessageManager
    {
        public ObservableCollection<DayOfMessages> IndividualDays { get; set; }
        public void Add(Message aMessage)
        {
            DateTime Date = IndividualDays.Count == 0 ? new DateTime() : IndividualDays[IndividualDays.Count - 1].Date;
            if (Date.Year != aMessage.Date.Year || Date.Month != aMessage.Date.Month || Date.Day != aMessage.Date.Day)
            {
                IndividualDays.Add(new DayOfMessages(new Message[] { aMessage }));
            }
            else { IndividualDays[IndividualDays.Count - 1].Messages.Add(aMessage); }
        }
    }
}
