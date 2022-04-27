using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Threading;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataManagementPage : ContentPage
    {
        public Data DataOfApplicationConnector { get; set; }
        public DataManagementPage()
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
        }

        private async void Personal_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            PersonalDataPage current = new PersonalDataPage(DataOfApplicationConnector.User);
            await Navigation.PushAsync(current);
        }

        private async void Acquaintances_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            AcquaintanceListPage current = new AcquaintanceListPage();
            await Navigation.PushAsync(current);
        }

        private async void Books_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            BooksListPage current = new BooksListPage();
            await Navigation.PushAsync(current);
        }

        private async void Stories_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            StoriesListPage current = new StoriesListPage();
            await Navigation.PushAsync(current);
        }

        private async void StoryButton_Clicked(object sender, EventArgs e)
        {
            StoryPage current = new StoryPage(new Story());
            Navigation.PushAsync(current);
            if (current.HandedIn)
            {
                (App.Current as App).DataOfApplication.Stories.Add(current.Displayed);
            }
        }

        private async void PersonButton_Clicked(object sender, EventArgs e)
        {
            AcquaintanceDataPage current = new AcquaintanceDataPage(new Acquaintance());
            Navigation.PushAsync(current);
            if (current.HandedIn)
            {
                (App.Current as App).DataOfApplication.Acquaintances.Add(current.Displayed);
            }
        }

        private async void BookButton_Clicked(object sender, EventArgs e)
        {
            BookPage current = new BookPage(new Book());
            Navigation.PushAsync(current);
            if (current.HandedIn)
            {
                (App.Current as App).DataOfApplication.Books.Add(current.Displayed);
            }
        }

        private void DataManagementPage_Appearing(object sender, EventArgs e)
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
        }
    }
}