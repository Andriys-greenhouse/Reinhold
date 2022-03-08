using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Threading.Tasks;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.IO;
using CoreLab;

namespace Reinhold
{
    public partial class App : Application
    {
        Data dataOfApplication;
        public Data DataOfApplication 
        {
            get { return dataOfApplication; }
            set { dataOfApplication = value; }
        }

        public App()
        {
            DataOfApplication = new Data();
            InitializeComponent();

            OnStart();
            MainPage = new MainPage();
        }

        protected async override void OnStart()
        {
            //first-load and load sequence
            if (!Properties.ContainsKey("FirstStarted"))
            {
                Properties.Add("FirstStarted", DateTime.Now);
                Properties.Add("LastSession", DateTime.Now);
                DataOfApplication.SetDefaultValues();
                using (Stream fileStream = await FileSystem.OpenAppPackageFileAsync("Core_0_2.json"))
                {
                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        DataOfApplication.Core = JsonConvert.DeserializeObject<Core>(sr.ReadToEnd());
                    }
                }
            }
            else
            {
                string sth = await SecureStorage.GetAsync("Data");
                JsonConvert.DeserializeObject<Data>(sth).CopyTo(ref dataOfApplication);
            }
        }

        protected async override void OnSleep()
        {
            Save();
        }

        protected override void OnResume()
        {

        }

        public async void Save()
        {
            Properties["LastSession"] = DateTime.Now;
            if (!Properties.ContainsKey("FirstStarted"))
            {
                Properties.Add("FirstStarted", DateTime.Now);
            }
            await SecureStorage.SetAsync("Data", JsonConvert.SerializeObject(DataOfApplication));
        }
    }
}
