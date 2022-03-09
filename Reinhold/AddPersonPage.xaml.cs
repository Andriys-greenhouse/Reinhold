using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPersonPage : ContentPage
    {
        public ObservableCollection<Acquaintance> people { get; set; }
        public Acquaintance Selected = null;
        //I'll got to make an object, which keeps fields for displaying as well as click command and than picks a right Acquaintance
        ICommand tapCommand;
        public ICommand TapCommand { get { return tapCommand; } }

        public AddPersonPage(ObservableCollection<Acquaintance> aPeople)
        {
            people = aPeople;
            tapCommand = new Command(OnTap);
            BindingContext = this;
            InitializeComponent();
        }

        /*private async void PersonListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Selected = (Acquaintance)PersonListView.SelectedItem;
            await Navigation.PopAsync();
        }*/

        private async void Item_TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Selected = (e as TappedEventArgs).Parameter as Acquaintance;
            await Navigation.PopAsync();
        }

        async void OnTap(object s)
        {
            Selected = (s as TapGestureRecognizer).CommandParameter as Acquaintance;
            await Navigation.PopAsync();
        }
    }
}