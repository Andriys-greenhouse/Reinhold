using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public Data DataOfApplicationConnector
        {
            get { return (App.Current as App).DataOfApplication; }
            set { (App.Current as App).DataOfApplication = value; }
        }
        public double SearchDept { get { return SearchDeptSlider.Value; } }
        public SettingsPage()
        {
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
            ColorSchemePicker.BindingContext = new ObservableCollection<string>(Enum.GetNames(typeof(ColorWord)));
            SearchDeptNumberLabel.BindingContext = this;
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

            }
        }

        private void SearchDeptSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SearchDept"));
        }

        private void ColorSchemePicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}