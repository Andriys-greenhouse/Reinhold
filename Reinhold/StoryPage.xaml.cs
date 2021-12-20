using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoryPage : ContentPage
    {
        public Story Displayed { get; set; }
        public bool HandedIn = false;
        public Color ColorSchmeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }

        public StoryPage(Story aStory)
        {
            Displayed = aStory;
            BindingContext = this;
            InitializeComponent();
            PeopleListView.ItemsSource = Displayed.People;
            TextEditor.BindingContext = Displayed;
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            HandedIn = true;
            await Navigation.PopAsync();
        }

        private async void AddPeopleButton_Clicked(object sender, EventArgs e)
        {
            AddPersonPage curent = new AddPersonPage((App.Current as App).DataOfApplication.Acquaintances);
            await Navigation.PushAsync(curent);
            if (curent.Selected != null) { Displayed.People.Add(curent.Selected); }
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Acquaintance ClickedContent = (sender as ImageButton).CommandParameter as Acquaintance;
            Displayed.People.Remove(ClickedContent);
        }
    }
}