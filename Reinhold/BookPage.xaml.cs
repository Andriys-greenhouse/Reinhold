using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        bool Editing = false;
        bool HandedIn = false;
        public double LevelOfLiking { get { return Math.Round(LevelOfLikingSlider.Value); } }
        public Book Displayed { get; set; }
        Book Original { get; set; }
        public Color ColorSchemeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        public BookPage(Book aBook)
        {
            if (aBook == new Book()) { throw new ArgumentException("This constructor is must be used for editing not for adding new Books."); }
            Original = aBook.GetCopy();
            Displayed = aBook;
            Editing = true;
            BindingContext = Displayed;
            InitializeComponent();
            LevelOfLikingNumberLabel.BindingContext = this;
            HeadingFrame.BindingContext = this;
        }
        public BookPage()
        {
            Displayed = new Book();
            BindingContext = Displayed;
            InitializeComponent();
            LevelOfLikingNumberLabel.BindingContext = this;
            HeadingFrame.BindingContext = this;
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            if (Displayed != new Book())
            {
                if (!Editing) { (App.Current as App).DataOfApplication.Books.Add(Displayed); }
            }
            HandedIn = true;
            await Navigation.PopAsync();
        }

        private void LevelOfLikingSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LevelOfLiking"));
            Displayed.LevelOfLiking = (int)Math.Round(LevelOfLikingSlider.Value);
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            if (!HandedIn && Editing && Displayed != Original && Displayed != new Book())
            {
                Displayed = Original;
            }
        }
    }
}