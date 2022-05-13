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
    public partial class AddHobbyPage : ContentPage
    {
        Person Owner;
        public AddHobbyPage(ref Person aOwner)
        {
            Owner = aOwner;
            BindingContext = this;
            InitializeComponent();
        }

        public AddHobbyPage(ref Acquaintance aOwner)
        {
            Owner = aOwner;
            BindingContext = this;
            InitializeComponent();
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            if (HobbyEntry.Text != null && HobbyEntry.Text.Length > 0) { Owner.Hobbys.Add(new Hobby(HobbyEntry.Text)); }
            await Navigation.PopAsync();
        }
    }
}