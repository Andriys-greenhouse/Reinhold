using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Threading.Tasks;

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
            BindingContext = Displayed;
            InitializeComponent();
            PlaceEntry.BindingContext = Displayed.Place;
            HeadingFrame.BindingContext = this;
            //PeopleListView.ItemsSource = Displayed.People;
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            HandedIn = true;
            await Navigation.PopAsync();
        }

        private async void AddPeopleButton_Clicked(object sender, EventArgs e)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            AddPersonPage current = new AddPersonPage((App.Current as App).DataOfApplication.Acquaintances);
            current.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushAsync(current);
            await Task.Run(() => waitHandle.WaitOne());
            if (current.Selected != null) { Displayed.People.Add(current.Selected); }
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            BindingContext = Displayed;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Acquaintance ClickedContent = (sender as ImageButton).CommandParameter as Acquaintance;
            Displayed.People.Remove(ClickedContent);
        }
    }
}