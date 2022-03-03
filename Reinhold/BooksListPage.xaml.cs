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
    public partial class BooksListPage : ContentPage
    {
        public ObservableCollection<Book> BooksCopy { get; set; }
        public Color ColorSchemeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        public BooksListPage()
        {
            BooksCopy = (App.Current as App).DataOfApplication.Books;
            BindingContext = this;
            InitializeComponent();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Book ClickedContent = (sender as ImageButton).CommandParameter as Book;
            if (!await DisplayAlert("Confirmation", $"Are you shure that you want to delete this Book?\n({ClickedContent.RepresentingText})", "No", "Yes"))
            {
                await SecureStorage.SetAsync(ClickedContent.QuotesID, "");
                BooksCopy.Remove(ClickedContent);
            }
        }

        private async void BooksListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            BookPage current = new BookPage((Book)BooksListView.SelectedItem);
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            current.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushAsync(current);
            await Task.Run(() => waitHandle.WaitOne());
            if (current.HandedIn)
            {
                BooksCopy.Remove((Book)BooksListView.SelectedItem);
                BooksCopy.Add(current.Displayed);
                BooksListView.ItemsSource = BooksCopy;
            }
            BindingContext = this;
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            BookPage current = new BookPage(new Book());
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            current.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushAsync(current);
            await Task.Run(() => waitHandle.WaitOne());
            if (current.HandedIn)
            {
                BooksCopy.Add(current.Displayed);
                BooksListView.ItemsSource = BooksCopy;
                //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            }
            BindingContext = this;
        }

        private void BooksListPage_Disappearing(object sender, EventArgs e)
        {
            (App.Current as App).DataOfApplication.Books = BooksCopy;
        }
    }
}