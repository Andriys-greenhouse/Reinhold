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
            DataOfApplication = new Data();
            DataOfApplication.SetTestValues();
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            //here shall be first-load and load sequences
            if (!Properties.ContainsKey("FirstStarted"))
            {
                Properties.Add("FirstStarted", DateTime.Now);
                Properties.Add("LastSession", DateTime.Now);
                DataOfApplication.SetDefaultValues();
            }
            else
            {
                string sth = await SecureStorage.GetAsync("Data");
                DataOfApplication = JsonConvert.DeserializeObject<Data>(sth);
            }
        }

        protected async override void OnSleep()
        {
            Properties["LastSession"] = DateTime.Now;
            await SecureStorage.SetAsync("Data", JsonConvert.SerializeObject(DataOfApplication));
        }

        protected override void OnResume()
        {
        }
    }
}
