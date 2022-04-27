using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoriesListPage : ContentPage
    {
        public ObservableCollection<Story> StoriesCopy { get; set; }
        public Color ColorSchemeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        public StoriesListPage()
        {
            StoriesCopy = (App.Current as App).DataOfApplication.Stories;
            BindingContext = this;
            InitializeComponent();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Story ClickedContent = (sender as ImageButton).CommandParameter as Story;
            if (!await DisplayAlert("Confirmation", $"Are you shure that you want to delete this story?\n({ClickedContent.RepresentingText})", "No", "Yes"))
            {
                await SecureStorage.SetAsync(ClickedContent.TextID, "");
                StoriesCopy.Remove(ClickedContent);
            }
        }

        private async void StoriesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            StoryPage current = new StoryPage((Story)StoriesListView.SelectedItem);
            Navigation.PushAsync(current);
            if (current.HandedIn)
            {
                StoriesCopy.Remove((Story)StoriesListView.SelectedItem);
                StoriesCopy.Add(current.Displayed);
                StoriesListView.ItemsSource = StoriesCopy;
            }
            BindingContext = this;
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            StoryPage current = new StoryPage(new Story());
            Navigation.PushAsync(current);
            if (current.HandedIn)
            {
                StoriesCopy.Add(current.Displayed);
                StoriesListView.ItemsSource = StoriesCopy;
            }
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            BindingContext = this;
        }

        private void StoriesListPage_Disappearing(object sender, EventArgs e)
        {
            (App.Current as App).DataOfApplication.Stories = StoriesCopy;
        }
    }
}