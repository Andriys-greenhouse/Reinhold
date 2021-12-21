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
        public bool HandedIn = false;
        public double LevelOfLiking { get { return Math.Round(LevelOfLikingSlider.Value); } }
        public Book Displayed { get; set; }
        public BookPage(Book aBook)
        {
            Displayed = aBook;
            BindingContext = Displayed;
            InitializeComponent();
            LevelOfLikingNumberLabel.BindingContext = this;
            HeadingFrame.BindingContext = this;
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            HandedIn = true;
            await Navigation.PopAsync();
        }

        private void LevelOfLikingSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LevelOfLiking"));
            Displayed.LevelOfLiking = (int)Math.Round(LevelOfLikingSlider.Value);
        }
    }
}