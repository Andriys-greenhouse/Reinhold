﻿using System;
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
        public MainPage()
        {
            BindingContext = (App.Current as App).DataOfApplication;
            InitializeComponent();
        }
    }
}
