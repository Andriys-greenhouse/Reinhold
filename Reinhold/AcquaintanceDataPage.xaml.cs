using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcquaintanceDataPage : ContentPage, INotifyPropertyChanged
    {
        public Color ColorSchmeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        public Acquaintance Displayed { get; set; }
        public bool HandedIn = false;
        public double LevelOfAcq { get { return Math.Round(LevelOfAcqSlider.Value); } }
        public event PropertyChangedEventHandler PropertyChanged;
        public AcquaintanceDataPage(Acquaintance aAcquaintence)
        {
            Displayed = aAcquaintence;
            BindingContext = Displayed;
            InitializeComponent();
            HeadingFrame.BindingContext = this;
            PhoneEntry.Text = Displayed.PhoneNumber;
            EmailEntry.Text = Displayed.Email;
            RelationPicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(Relation)));
            RelationPicker.SelectedIndex = (int)Displayed.Relation;
            LevelOfRelationPicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(LevelOfLiking)));
            LevelOfRelationPicker.SelectedIndex = (int)Displayed.LevelOfRelarion;
            LevelOfAcqLabel.BindingContext = this;
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Displayed.Hobbys.Remove((sender as ImageButton).CommandParameter as Hobby);
        }

        private async void AddHobbyButton_Clicked(object sender, EventArgs e)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            AddHobbyPage current = new AddHobbyPage();
            current.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushAsync(current);
            await Task.Run(() => waitHandle.WaitOne());
            if (current.Submitted && current.Hobby.Length > 0) { Displayed.Hobbys.Add(new Hobby(current.Hobby == null ? "" : current.Hobby)); }
            BindingContext = Displayed;
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

        private async void RelationPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Displayed.Relation = (Relation)RelationPicker.SelectedIndex;
        }

        private void LevelOfRelationPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Displayed.LevelOfRelarion = (LevelOfLiking)LevelOfRelationPicker.SelectedIndex;
        }

        private void LevelOfAcqSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LevelOfAcq"));
            Displayed.LevelOfAcquaintance = (int)Math.Round(LevelOfAcqSlider.Value);
        }
    }
}