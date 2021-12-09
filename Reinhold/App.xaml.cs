using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Threading.Tasks;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace Reinhold
{
    public partial class App : Application
    {
        public Data DataOfApplication { get; set; }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            //here shall be first-load and load sequences
            DataOfApplication.SetTestValues();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
