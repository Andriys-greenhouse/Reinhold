using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPersonPage : ContentPage
    {
        ObservableCollection<Acquaintance> people { get; set; }
        public Acquaintance Selected = null;
        public AddPersonPage(ObservableCollection<Acquaintance> aPeople)
        {
            people = aPeople;
            BindingContext = people;
            InitializeComponent();
        }

        private async void PersonListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Selected = (Acquaintance)PersonListView.SelectedItem;
            await Navigation.PopAsync();
        }
    }
}