using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage, INotifyPropertyChanged
    {
        public Data DataOfApplicationConnector { get; set; }
        public string MicOrSendButtonIcon 
        { 
            get
            {
                try
                {
                    return MessageBox.IsFocused || MessageBox.Text.Length > 0 ? DataOfApplicationConnector.ArrowIcon : DataOfApplicationConnector.MicIcon;
                }
                catch (Exception)
                {
                    return DataOfApplicationConnector.MicIcon;
                }
            } 
        }
        public ChatPage()
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
            MicOrSendButton.BindingContext = this;

            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
        }

        private void MicOrSendButton_Clicked(object sender, EventArgs e)
        {
            if (MessageBox.IsFocused || (MessageBox != null && MessageBox.Text.Length > 0))
            {
                DataOfApplicationConnector.Messages.Messages.Add(new Message(MessageBox.Text, true));
                MessageBox.Text = "";
                MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
                MicOrSendButton.Source = MicOrSendButtonIcon;
            }
        }

        private void MessageBox_Focus(object sender, FocusEventArgs e)
        {
            MicOrSendButton.Source = MicOrSendButtonIcon;
            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
        }

        private void ChatPage_Appearing(object sender, EventArgs e)
        {
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
            MicOrSendButton.BindingContext = this;
            MicOrSendButton.Source = MicOrSendButtonIcon;
            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
        }
    }
}