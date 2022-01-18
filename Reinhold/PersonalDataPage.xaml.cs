﻿using System;
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
        public Person Displayed { get; set; }
        public PersonalDataPage(Person User)
        {
            Displayed = User;
            BindingContext = Displayed;
            InitializeComponent();
            HeadingFrame.BindingContext = this;
            PhoneEntry.Text = Displayed.PhoneNumber;
            EmailEntry.Text = Displayed.Email;
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            Displayed.Hobbys.Remove((sender as ImageButton).CommandParameter as Hobby);
            //HobbyListView.HeightRequest = 1 + HobbyListView.RowHeight * Displayed.Hobbys.Count;
            BindingContext = Displayed;
        }

        private async void AddHobbyButton_Clicked(object sender, EventArgs e)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            AddHobbyPage current = new AddHobbyPage();
            current.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushAsync(current);
            await Task.Run(() => waitHandle.WaitOne());
            if(current.Submitted && current.Hobby.Length > 0) { Displayed.Hobbys.Add(new Hobby(current.Hobby == null ? "" : current.Hobby)); }
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

            if(promt == "")
            {
                (App.Current as App).DataOfApplication.User = Displayed;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Invallid input", promt.Substring(1), "OK");
            }
        }
    }
}