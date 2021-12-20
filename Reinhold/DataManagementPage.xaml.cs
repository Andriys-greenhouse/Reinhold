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
    public partial class DataManagementPage : ContentPage
    {
        public Data DataOfApplicationConnector
        {
            get { return (App.Current as App).DataOfApplication; }
            set { (App.Current as App).DataOfApplication = value; }
        }
        public DataManagementPage()
        {
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
        }

        private async void Personal_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            PersonalDataPage curent = new PersonalDataPage(DataOfApplicationConnector.User);
            await Navigation.PushAsync(curent);
        }

        private void Acquaintances_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private void Books_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private async void Stories_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StoriesListPage());
        }

        private void StoryButton_Clicked(object sender, EventArgs e)
        {

        }

        private void PersonButton_Clicked(object sender, EventArgs e)
        {

        }

        private void BookButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}