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
            await Navigation.PushAsync(new PersonalDataPage(DataOfApplicationConnector.User));
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
            Navigation.PushAsync(new StoryPage());
        }

        private async void PersonButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AcquaintanceDataPage());
        }

        private async void BookButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BookPage());
        }

        private void DataManagementPage_Appearing(object sender, EventArgs e)
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
        }
    }
}