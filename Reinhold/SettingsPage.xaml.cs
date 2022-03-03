using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Data DataOfApplicationCopy { get; set; }
        public double SearchDept { get { return Math.Round(SearchDeptSlider.Value); } }
        bool initialSetting = true;
        public SettingsPage()
        {
            DataOfApplicationCopy = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationCopy;
            InitializeComponent();
            ColorSchemePicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(ColorWord)));
            SearchDeptNumberLabel.BindingContext = this;
            ColorSchemePicker.SelectedIndex = (int)DataOfApplicationCopy.ColorScheme;
            SearchDeptSlider.Value = DataOfApplicationCopy.SearchDept;
        }

        private async void ResetButton_Clicked(object sender, EventArgs e)
        {
            if (!await DisplayAlert("Confirmation", "Are you shure that you want to delete all your data and reset the application?\n(this step is irreversible)", "No", "Yes"))
            {
                for (int i = 0; i < Data.nextID; i++)
                {
                    try
                    {
                        SecureStorage.Remove(i.ToString());
                    }
                    catch (Exception g)
                    { }
                }
                (App.Current as App).DataOfApplication.SetDefaultValues();
                await SecureStorage.SetAsync("Data", JsonConvert.SerializeObject((App.Current as App).DataOfApplication));
                (App.Current as App).Properties.Clear();
                App.Current.MainPage = new MainPage();
            }
        }
        /*
        private void SearchDeptSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchDept"));
            DataOfApplicationCopy.SearchDept = (int)Math.Round(SearchDeptSlider.Value);
        }
        */
        private async void ColorSchemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((int)DataOfApplicationCopy.ColorScheme != ColorSchemePicker.SelectedIndex && !initialSetting)
            {
                if (!await DisplayAlert("Confirmation", "Change of color scheme requires restart of application, change color scheme?", "No", "Yes"))
                {
                    DataOfApplicationCopy.ColorScheme = (ColorWord)ColorSchemePicker.SelectedIndex;
                    SettingsPage_Disappearing(this, new EventArgs());
                    (App.Current as App).Save();
                    App.Current.MainPage = new MainPage();
                }
                else
                {
                    ColorSchemePicker.SelectedIndex = (int)DataOfApplicationCopy.ColorScheme;
                }
            }
        }

        private void SettingsPage_Disappearing(object sender, EventArgs e)
        {
            (App.Current as App).DataOfApplication.ColorScheme = DataOfApplicationCopy.ColorScheme;
            (App.Current as App).DataOfApplication.SearchDept = (int)SearchDept;
            (App.Current as App).DataOfApplication.DisplayNotifications = DataOfApplicationCopy.DisplayNotifications;
        }

        private void SettingsPage_Appearing(object sender, EventArgs e) //basicly updates the values
        {
            initialSetting = true;
            DataOfApplicationCopy = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationCopy;
            ColorSchemePicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(ColorWord)));
            SearchDeptNumberLabel.BindingContext = this;
            ColorSchemePicker.SelectedIndex = (int)DataOfApplicationCopy.ColorScheme;
            SearchDeptSlider.Value = DataOfApplicationCopy.SearchDept;
            initialSetting = false;
        }
    }
}