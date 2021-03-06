using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonalDataPage : ContentPage
    {
        public Color ColorSchmeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }
        Person displayed;
        public Person Displayed 
        { 
            get { return displayed; }
            set { displayed = value; }
        }

        int last, yearValue, monthValue, dayValue;

        public PersonalDataPage(Person User)
        {
            Displayed = User;
            BindingContext = Displayed;
            InitializeComponent();
            HeadingFrame.BindingContext = this;
            PhoneEntry.Text = Displayed.PhoneNumber;
            EmailEntry.Text = Displayed.Email;

            yearValue = Displayed.BirthDate.Year;
            monthValue = Displayed.BirthDate.Month;
            dayValue = Displayed.BirthDate.Day;
            YearEntry.Text = yearValue.ToString();
            MonthEntry.Text = monthValue.ToString();
            DayEntry.Text = dayValue.ToString();
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Displayed.Hobbys.Remove((sender as ImageButton).CommandParameter as Hobby);
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            BindingContext = Displayed;
        }

        private async void AddHobbyButton_Clicked(object sender, EventArgs e)
        {
            AddHobbyPage current = new AddHobbyPage(ref displayed);
            Navigation.PushAsync(current);
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            BindingContext = Displayed;
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            string promt = "";
            try
            {
                Displayed.PhoneNumber = PhoneEntry.Text;
            }
            catch (Exception f)
            {
                promt += "\nInvallid phone number.";
            }
            try
            {
                Displayed.Email = EmailEntry.Text;
            }
            catch (Exception g)
            {
                promt += "\nInvallid email.";
            }
            try
            {
                Displayed.BirthDate = new DateTime(yearValue, monthValue, dayValue);
            }
            catch (Exception g)
            {
                promt += "\nInvallid date.";
            }

            if (promt == "")
            {
                (App.Current as App).DataOfApplication.User = Displayed;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Invallid input", promt.Substring(1), "OK");
            }
        }

        
        private void YearEntry_Unfocused(object sender, FocusEventArgs e)
        {
            last = yearValue;
            if (YearEntry.Text.Length == 0) { yearValue = 0; }
            else 
            {
                if (!int.TryParse(YearEntry.Text, out yearValue))
                {
                    yearValue = last;
                }
            }
            YearEntry.Text = yearValue.ToString();
        }
        private void DownYearButt_Clicked(object sender, EventArgs e)
        {
            if (yearValue > 0) { yearValue--; }
            YearEntry.Text = yearValue.ToString();
        }
        private void UpYearButt_Clicked(object sender, EventArgs e)
        {
            yearValue++;
            YearEntry.Text = yearValue.ToString();
        }


        private void MonthEntry_Unfocused(object sender, FocusEventArgs e)
        {
            last = monthValue;
            if (MonthEntry.Text.Length == 0) { monthValue = 0; }
            else
            {
                if (!int.TryParse(MonthEntry.Text, out monthValue))
                {
                    monthValue = last;
                }
            }
            MonthEntry.Text = monthValue.ToString();
        }
        private void DownMonthButt_Clicked(object sender, EventArgs e)
        {
            if (monthValue > 0) { monthValue--; }
            MonthEntry.Text = monthValue.ToString();
        }
        private void UpMonthButt_Clicked(object sender, EventArgs e)
        {
            if (monthValue < 12) { monthValue++; }
            MonthEntry.Text = monthValue.ToString();
        }

        
        private void DayEntry_Unfocused(object sender, FocusEventArgs e)
        {
            last = dayValue;
            if (DayEntry.Text.Length == 0) { dayValue = 0; }
            else
            {
                if (!int.TryParse(DayEntry.Text, out dayValue))
                {
                    dayValue = last;
                }
            }
            DayEntry.Text = dayValue.ToString();
        }
        private void DownDayButt_Clicked(object sender, EventArgs e)
        {
            if (dayValue > 0) { dayValue--; }
            DayEntry.Text = dayValue.ToString();
        }
        private void UpDayButt_Clicked(object sender, EventArgs e)
        {
            if (dayValue < 31) { dayValue++; }
            DayEntry.Text = dayValue.ToString();
        }
    }
}