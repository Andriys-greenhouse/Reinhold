using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using System.Threading.Tasks;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoryPage : ContentPage
    {
        public Story Displayed { get; set; }
        public bool HandedIn = false;
        public Color ColorSchmeInColor { get { return (App.Current as App).DataOfApplication.ColorSchemeInColor; } }

        int last, yearValue, monthValue, dayValue;

        public StoryPage(Story aStory)
        {
            Displayed = aStory;
            BindingContext = Displayed;
            InitializeComponent();
            PlaceEntry.BindingContext = Displayed.Place;
            HeadingFrame.BindingContext = this;
            //PeopleListView.ItemsSource = Displayed.People;

            yearValue = Displayed.Date.Year;
            monthValue = Displayed.Date.Month;
            dayValue = Displayed.Date.Day;
            YearEntry.Text = yearValue.ToString();
            MonthEntry.Text = monthValue.ToString();
            DayEntry.Text = dayValue.ToString();
        }

        private async void DoneButton_Clicked(object sender, EventArgs e)
        {
            string promt = "";
            try
            {
                Displayed.Date = new DateTime(yearValue, monthValue, dayValue);
            }
            catch (Exception g)
            {
                promt += "\nInvallid date.";
            }

            if (promt == "")
            {
                HandedIn = true;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Invallid input", promt.Substring(1), "OK");
            }
        }

        private async void AddPeopleButton_Clicked(object sender, EventArgs e)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            AddPersonPage current = new AddPersonPage((App.Current as App).DataOfApplication.Acquaintances);
            current.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushAsync(current);
            await Task.Run(() => waitHandle.WaitOne());
            if (current.Selected != null) { Displayed.People.Add(current.Selected); }
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            BindingContext = Displayed;
        }

        private async void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Acquaintance ClickedContent = (sender as ImageButton).CommandParameter as Acquaintance;
            Displayed.People.Remove(ClickedContent);
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