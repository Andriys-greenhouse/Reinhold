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
        //I'll got to make an object, which keeps fields for displaying as well as click command and than picks a right Acquaintance
        Story Owner;
        ICommand tapCommand;
        public ICommand TapCommand { get { return tapCommand; } }

        public AddPersonPage(ref Story aOwner)
        {
            Owner = aOwner;
            people = (App.Current as App).DataOfApplication.Acquaintances;
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
            Acquaintance Selected = (e as TappedEventArgs).Parameter as Acquaintance;
            if (!Owner.People.Contains(Selected)) { Owner.People.Add(Selected); }
            await Navigation.PopAsync();
        }

        async void OnTap(object s)
        {
            Acquaintance Selected = (s as TapGestureRecognizer).CommandParameter as Acquaintance;
            if (!Owner.People.Contains(Selected)) { Owner.People.Add(Selected); }
            await Navigation.PopAsync();
        }
    }
}