using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoriesListPage : ContentPage
    {
        public Data DataOfApplicationConnector
        {
            get { return (App.Current as App).DataOfApplication; }
            set { (App.Current as App).DataOfApplication = value; }
        }
        public StoriesListPage()
        {
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
            StoriesListView.ItemsSource = DataOfApplicationConnector.Stories;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert("Confirmation", $"Are you shure that you want to delete this story?\n({((sender as Button).CommandParameter as Story).RepresentingText})", "No", "Yes"))
            {
                DataOfApplicationConnector.Stories.Remove((sender as Button).CommandParameter as Story);
            }
        }

        private void StoriesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {

        }
    }
}