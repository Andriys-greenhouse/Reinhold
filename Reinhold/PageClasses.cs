using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Reinhold
{
    public enum ColorWord { Red, Green, Blue }
    public enum Relation { Sibling, Parent, Grandparent, Aunt, Cousin, BrawderFamily, Friend, SuperiorCoworker, LevelCoworker, InferiorCoworker }
    public enum LevelOfLiking { Hatered, Dislike, Neutral, Pozitive, IsDear }

    public class Data
    {
        public DateTime FirstStart { get; set; }
        public DateTime LastStart { get; set; }
        public bool ResetOrdered { get; set; }

        public static int nextID { get; set; }

        public ColorWord ColorScheme
        {
            get;
            set;
        }
        [JsonIgnore]
        int searchDept;
        public int SearchDept
        {
            get { return searchDept; }
            set
            {
                if (value > 10 || value < 0) { throw new ArgumentException("Search dept must be between 0 and 10!"); }
                searchDept = value;
            }
        }
        public bool DisplayNotifications { get; set; }

        public Person User { get; set; }
        public ObservableCollection<Acquaintance> Acquaintances { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        public ObservableCollection<Story> Stories { get; set; }

        public MessageManager Messages { get; set; }
        [JsonIgnore]
        public string Version { get { return "Version: 0.1"; } }
        [JsonIgnore]
        public string CoreVersion { get { return "Core vrsion: 0.0"; } }

        [JsonIgnore]
        public string ChatIcon { get { return $"{ColorScheme.ToString().ToLower()}Chat.png"; } }
        [JsonIgnore]
        public string UserIcon { get { return $"{ColorScheme.ToString().ToLower()}User.png"; } }
        [JsonIgnore]
        public string SettingsIcon { get { return $"{ColorScheme.ToString().ToLower()}Control.png"; } }
        [JsonIgnore]
        public string ArrowIcon { get { return $"{ColorScheme.ToString().ToLower()}Arrow.png"; } }
        [JsonIgnore]
        public string BookIcon { get { return $"{ColorScheme.ToString().ToLower()}Book.png"; } }
        [JsonIgnore]
        public string DeleteIcon { get { return $"{ColorScheme.ToString().ToLower()}Delete.png"; } }
        [JsonIgnore]
        public string FriendIcon { get { return $"{ColorScheme.ToString().ToLower()}Friend.png"; } }
        [JsonIgnore]
        public string StoryIcon { get { return $"{ColorScheme.ToString().ToLower()}Labyrinth.png"; } }
        [JsonIgnore]
        public string MenuIcon { get { return $"{ColorScheme.ToString().ToLower()}Menu.png"; } }
        [JsonIgnore]
        public string MicIcon { get { return $"{ColorScheme.ToString().ToLower()}Mic.png"; } }

        [JsonIgnore]
        public Color ColorSchemeInColor
        {
            get
            {
                switch (ColorScheme)
                {
                    case ColorWord.Red:
                        return Color.FromHex("#fd0808");
                    //return Color.FromRgb(99, 3, 3);
                    case ColorWord.Green:
                        return Color.FromHex("#4c9f3d");
                    //return Color.FromRgb(29, 62, 23);
                    case ColorWord.Blue:
                        return Color.FromHex("#008ae6");
                    default:
                        return Color.FromRgb(0, 0, 0);
                }
            }
        }

        public Data()
        {
            SetDefaultValues();
        }

        public void SetDefaultValues()
        {
            ColorScheme = ColorWord.Green;
            SearchDept = 3;
            DisplayNotifications = true;
            Books = new ObservableCollection<Book>();
            Stories = new ObservableCollection<Story>();
            Acquaintances = new ObservableCollection<Acquaintance>();
            User = new Person();
            User.SetDefaultValues();
            Messages = new MessageManager();
            nextID = 0;
        }

        public void SetTestValues()
        {
            SetDefaultValues();
            Acquaintance friend = new Acquaintance();
            friend.SetDefaultValues();
            friend.FullName = "Reginald Roald Hamundson";
            friend.FirstName = "Roald";
            friend.FullName = "Hamundson";
            friend.BirthDate = new DateTime(2002, 3, 16);
            friend.DateIsAcurate = true;
            friend.PlaceOfResidence = new Place("České Budějovice");
            friend.Appearance = "Tall and haves gray hare.";
            friend.Hobbys.Add(new Hobby("Sailing"));

            Books.Add(new Book()
            {
                Title = "Farenheit 451",
                AuthorsCompleteName = "Raymond Douglas Bradbury"
            });

            for (int i = 0; i < 21; i++)
            {
                Stories.Add(new Story()
                {
                    Place = new Place("Paris"),
                    Text = "It all happend yesterday...",
                    Date = DateTime.Now,
                    People = new ObservableCollection<Acquaintance>() { friend }
                });
            }

            Acquaintances = new ObservableCollection<Acquaintance>() { friend };

            User = friend;
            User.FullName = "Gregorg Janík";
            User.FirstName = "Gregorg";
            User.LastName = "Janík";

            Messages = new MessageManager();
            Messages.Add(new Message("Hello", true));
            Messages.Add(new Message("Hello, how can I halp you?", false));
        }

        public void CopyTo(ref Data CopyInto)
        {
            CopyInto.FirstStart = FirstStart;
            CopyInto.LastStart = LastStart;
            CopyInto.ResetOrdered = ResetOrdered;
            CopyInto.ColorScheme = ColorScheme;
            CopyInto.SearchDept = SearchDept;
            CopyInto.DisplayNotifications = DisplayNotifications;
            CopyInto.User = User;
            CopyInto.Acquaintances = Acquaintances;
            CopyInto.Books = Books;
            CopyInto.Stories = Stories;
            CopyInto.Messages = Messages;
        }
    }

    public class Person
    {
        [JsonIgnore]
        string id = "";
        public string ID
        {
            get { return id; }
            set
            {
                if (id == "") { id = DateTime.Now.Ticks.ToString(); }
                else { throw new ArgumentException("ID can be set only once!"); }
            }
        }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool DateIsAcurate { get; set; }
        public Place LastLocation { get; set; }
        public Place PlaceOfResidence { get; set; }
        public string Appearance { get; set; }
        public bool IsMan { get; set; }
        public ObservableCollection<Hobby> Hobbys { get; set; }

        [JsonIgnore]
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
        [JsonIgnore]
        string email;
        public string Email
        {
            get { return email; }
            set
            {
                Regex rx = new Regex(@"^(?<address>.*)\@(?<provider>.*)\.(?<domain>.{2,3})$");
                if (rx.IsMatch(value) || value == "")
                {
                    //email = rx.Match(value).Groups[1].Value;
                    email = rx.Match(value).Value;
                }
                else { throw new ArgumentException("Invallid email!"); }
            }
        }
        public Person()
        {
            SetDefaultValues();
        }
        public void SetDefaultValues()
        {
            FullName = "";
            LastName = "";
            FirstName = "";
            BirthDate = new DateTime();
            DateIsAcurate = false;
            PlaceOfResidence = new Place("");
            LastLocation = new Place("");
            Appearance = "";
            IsMan = true;
            Hobbys = new ObservableCollection<Hobby>();
            PhoneNumber = "";
            Email = "";
        }
    }

    public class Acquaintance : Person
    {
        public Relation Relation { get; set; }
        public LevelOfLiking LevelOfRelarion { get; set; }
        [JsonIgnore]
        int levelOfAcquaintance;
        public int LevelOfAcquaintance
        {
            get { return levelOfAcquaintance; }
            set
            {
                if (value > 100 || value < 0) { throw new ArgumentException("Level of acquaintance must be between 0 and 100!"); }
                levelOfAcquaintance = value;
            }
        }
        [JsonIgnore]
        public string BinImagePointer { get { return (App.Current as App).DataOfApplication.DeleteIcon; } }
        [JsonIgnore]
        public string RepresentingName { get { return FullName == null ? (LastName == null ? FirstName : LastName) : FullName; } }
        public Acquaintance() : base()
        {
            Relation = Relation.BrawderFamily;
            LevelOfRelarion = LevelOfLiking.Neutral;
            LevelOfAcquaintance = 50;
        }
    }

    public class Story
    {
        public Place Place { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<Acquaintance> People { get; set; }
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
                if (value != null) { SaveText(); }
            }
        }
        [JsonIgnore]
        public string RepresentingText { get { return Text.Replace('\n', ' ').Substring(0, Text.Length > 17 ? 17 : Text.Length) + (Text.Length > 17 ? "..." : ""); } }
        [JsonIgnore]
        public string BinImagePointer { get { return (App.Current as App).DataOfApplication.DeleteIcon; } }

        public Story()
        {
            TextID = (Data.nextID++).ToString();
            Place = new Place("");
            Date = DateTime.Now;
            People = new ObservableCollection<Acquaintance>();
            Text = "";
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
        public int LevelOfLiking { get; set; }
        bool QuoteFetched = false;
        public string QuotesID;
        [JsonIgnore]
        public string BinImagePointer { get { return (App.Current as App).DataOfApplication.DeleteIcon; } }
        [JsonIgnore]
        public string RepresentingText { get { return FavouriteQuote.Replace('\n', ' ').Substring(0, FavouriteQuote.Length > 17 ? 17 : FavouriteQuote.Length) + (FavouriteQuote.Length > 17 ? "..." : ""); } }
        [JsonIgnore]
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
                if (value != null) { SaveQuote(); }
            }
        }

        public Book()
        {
            QuotesID = (Data.nextID++).ToString();
            AuthorsCompleteName = "";
            Title = "";
            LevelOfLiking = 50;
            favouriteQuote = "";
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
        public string DateToDisplay
        {
            get { return Date.ToString("yyyy/MM/dd HH:mm"); }
        }
        public LayoutOptions Side { get { return SendByUser ? LayoutOptions.End : LayoutOptions.Start; } }
        public Color Color { get { return SendByUser ? (App.Current as App).DataOfApplication.ColorSchemeInColor : Color.FromRgb(56, 54, 49); } }
        public DateTime Date;
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

                if (Date.Year != item.Date.Year || Date.Month != item.Date.Month || Date.Day != item.Date.Day)
                {
                    throw new ArgumentException("Not all messages are from the same day!");
                }

                Messages.Add(item);
            }
        }
    }

    public class MessageManager
    {
        //public ObservableCollection<DayOfMessages> IndividualDays { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public MessageManager()
        {
            Messages = new ObservableCollection<Message>();
        }
        public void Add(Message aMessage)
        {
            /*
            DateTime Date = IndividualDays.Count == 0 ? new DateTime() : IndividualDays[IndividualDays.Count - 1].Date;
            if (Date.Year != aMessage.Date.Year || Date.Month != aMessage.Date.Month || Date.Day != aMessage.Date.Day)
            {
                IndividualDays.Add(new DayOfMessages(new Message[] { aMessage }));
            }
            else { IndividualDays[IndividualDays.Count - 1].Messages.Add(aMessage); }
            */
            Messages.Add(aMessage);
        }
    }

    public class Hobby
    {
        public string Text { get; set; }
        [JsonIgnore]
        public string BinImagePointer { get { return (App.Current as App).DataOfApplication.DeleteIcon; } }

        public Hobby(string aText)
        {
            Text = aText;
        }
    }
}
