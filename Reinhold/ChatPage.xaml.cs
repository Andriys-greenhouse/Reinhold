using Reinhold.ViewModels;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinhold
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        ChatPageViewModel ViewModel = new ChatPageViewModel();
        public ChatPage()
        {
            InitializeComponent();
            MicOrSendButton.BindingContext = this;
            MessageListView.ItemsSource = ChatPageViewModel.DataOfApplicationConnector.Messages.Messages;
            BindingContext = ViewModel;
            MicOrSendButton.Source = ViewModel.MicOrSendButtonIcon;
        }



        private async void MicOrSendButton_Clicked(object sender, EventArgs e)
        {
            //func
            if (ViewModel.ButtonClick.CanExecute(null))
            {
                ViewModel.ButtonClick.Execute(null);
            }
            //else { throw new Exception("This was not supposed to happen, for some reason the ButtonClick command can't be executed..."); }

            MessageBox.Text = "";
            MessageListView.ItemsSource = ChatPageViewModel.DataOfApplicationConnector.Messages.Messages;
            /*if (ViewModel.UpdateIcon.CanExecute(null))
            {
                ViewModel.UpdateIcon.Execute(null);
            }*/
            MicOrSendButton.Source = ViewModel.MicOrSendButtonIcon;
            MessageListView.ScrollTo(ChatPageViewModel.DataOfApplicationConnector.Messages.Messages[ChatPageViewModel.DataOfApplicationConnector.Messages.Messages.Count - 1], ScrollToPosition.End, false);
        }

        private void MessageBox_Focus(object sender, FocusEventArgs e)
        {
            /*if (ViewModel.UpdateIcon.CanExecute(null))
            {
                ViewModel.UpdateIcon.Execute(null);
            }*/
            if (e.VisualElement != MessageBox) 
            {
                ViewModel.MsgBxFocused = !e.VisualElement.IsFocused;
            } 
            else
            {
                ViewModel.MsgBxFocused = e.VisualElement.IsFocused;
            }

            MicOrSendButton.Source = ViewModel.MicOrSendButtonIcon;
        }

        private void ChatPage_Appearing(object sender, EventArgs e)
        {
            /*
             //BUDE TO FUNGOVAT BEZ TOHO?
            DataOfApplicationConnector = (App.Current as App).DataOfApplication;
            BindingContext = DataOfApplicationConnector;
            MicOrSendButton.BindingContext = this;
            MicOrSendButton.Source = MicOrSendButtonIcon;
            MessageListView.ItemsSource = DataOfApplicationConnector.Messages.Messages;
            */
            MessageListView.ItemsSource = ChatPageViewModel.DataOfApplicationConnector.Messages.Messages;
            //MessageBox_Focus(this, new FocusEventArgs(MessageBox, MessageBox.IsFocused));
        }


    }
}