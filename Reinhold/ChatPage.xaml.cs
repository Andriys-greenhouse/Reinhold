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
        public Data DataOfApplicationConnector
        {
            get { return (App.Current as App).DataOfApplication; }
            set { (App.Current as App).DataOfApplication = value; }
        }
        public string MicOrSendButtonIcon 
        { 
            get
            {
                return MessageBox.IsFocused ? DataOfApplicationConnector.ArrowIcon : DataOfApplicationConnector.MicIcon;
            } 
        }
        public ChatPage()
        {
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
            MicOrSendButton.BindingContext = this;

            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
        }

        private void MicOrSendButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}