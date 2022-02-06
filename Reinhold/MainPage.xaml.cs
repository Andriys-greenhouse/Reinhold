using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Reinhold
{
    public partial class MainPage : Shell
    {
        public Data DataOfApplicationConnector { get; set; } = (App.Current as App).DataOfApplication;
        public MainPage()
        {
            BindingContext = DataOfApplicationConnector;
            InitializeComponent();
        }
    }
}
