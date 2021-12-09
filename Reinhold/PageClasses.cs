using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Reinhold
{
    public enum ColorWord { Red, Green, Blue }
    public enum Relation { Sibling, Parent, Grandparent, Aunt, Cousin, BrawderFamily, Friend, SuperiorCoworker, LevelCoworker, InferiorCoworker }
    public enum LevelOfLiking { Dislike, Suspiton, Neutral, Pozitive, Great }

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
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool DateIsAcurate { get; set; }
        public string PlaceOfResidence { get; set; }
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

    }

    public class Book
    {

    }
}
