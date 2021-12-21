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
        public bool Submitted;
        public string Hobby { get; set; }
        public AddHobbyPage()
        {
            Submitted = false;
            BindingContext = this;
            InitializeComponent();
        }

        private async void AddButton_Clicked(object sender, EventArgs e)
        {
            Submitted = true;
            Hobby = HobbyEntry.Text;
            await Navigation.PopAsync();
        }
    }
}