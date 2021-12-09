using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinhold
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public string MicOrSaveButtonIcon 
        { 
            get
            {
                if (MessageBox.IsFocused) { return (App.Current as App).DataOfApplication.ArrowIcon; }
                else { return (App.Current as App).DataOfApplication.MicIcon; }
            } 
        }
        public ChatPage()
        {
            InitializeComponent();
        }
    }
}