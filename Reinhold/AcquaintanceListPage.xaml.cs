using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Threading;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcquaintanceListPage : ContentPage
    {
        public ObservableCollection<Acquaintance> AcquaintancesCopy { get; set; }
        public Color ColorSchemeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        public AcquaintanceListPage()
        {
            AcquaintancesCopy = (App.Current as App).DataOfApplication.Acquaintances;
            BindingContext = this;
            InitializeComponent();
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Acquaintance ClickedContent = (sender as ImageButton).CommandParameter as Acquaintance;
            if (!await DisplayAlert("Confirmation", $"Are you shure that you want to delete this acquaintance?\n({ClickedContent.FullName})", "No", "Yes"))
            {
                AcquaintancesCopy.Remove(ClickedContent);
            }
        }

        private async void AcquaintancesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new AcquaintanceDataPage((Acquaintance)AcquaintancesListView.SelectedItem));
            AcquaintancesListView.ItemsSource = AcquaintancesCopy;
            BindingContext = this;
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AcquaintanceDataPage());
            AcquaintancesListView.ItemsSource = AcquaintancesCopy;
            BindingContext = this;
        }

        private void AcquaintanceListPage_Disappearing(object sender, EventArgs e)
        {
            //(App.Current as App).DataOfApplication.Acquaintances = AcquaintancesCopy;
        }
    }
}