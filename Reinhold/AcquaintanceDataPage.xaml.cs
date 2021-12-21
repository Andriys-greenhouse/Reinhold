using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcquaintanceDataPage : ContentPage
    {
        public Color ColorSchmeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        public Acquaintance Displayed { get; set; }
        public bool HandedIn = false;
        public double LevelOfAcq { get { return Math.Round(LevelOfAcqSlider.Value); } }
        public AcquaintanceDataPage(Acquaintance aAcquaintence)
        {
            Displayed = aAcquaintence;
            BindingContext = Displayed;
            InitializeComponent();
            HeadingFrame.BindingContext = this;
            PhoneEntry.Text = Displayed.PhoneNumber;
            EmailEntry.Text = Displayed.PhoneNumber;
            RelationPicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(Relation)));
            RelationPicker.SelectedIndex = (int)Displayed.Relation;
            LevelOfRelationPicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(LevelOfLiking)));
            LevelOfRelationPicker.SelectedIndex = (int)Displayed.LevelOfRelarion;
            LevelOfAcqLabel.BindingContext = this;
            LevelOfAcqSlider.Value = Displayed.LevelOfAcquaintance;
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Displayed.Hobbys.Remove((sender as ImageButton).CommandParameter as Hobby);
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
        }

        private async void AddHobbyButton_Clicked(object sender, EventArgs e)
        {
            AddHobbyPage curent = new AddHobbyPage();
            await Navigation.PushAsync(curent);
            if(curent.Submitted && curent.Hobby.Length > 0) { Displayed.Hobbys.Add(new Hobby(curent.Hobby)); }
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            string promt = "";
            try
            {
                Displayed.PhoneNumber = PhoneEntry.Text;
            }
            catch (Exception)
            {
                promt += "\nInvallid phone number.";
            }
            try
            {
                Displayed.Email = EmailEntry.Text;
            }
            catch (Exception)
            {
                promt += "\nInvallid email.";
            }

            if(promt == "")
            {
                HandedIn = true;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Invallid input", promt.Substring(1), "OK");
            }
        }

        private void RelationPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LevelOfRelationPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LevelOfAcqSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {

        }
    }
}