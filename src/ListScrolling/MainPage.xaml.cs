// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System.Windows;
using System.Windows.Controls;
using Delay;
using Microsoft.Phone.Controls;

namespace ListScrolling
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            var user = new TwitterUser("DavidAns");
            DataContext = user;
            var startFetch = user.Followers;
        }

        private void Set_Button_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            ((FrameworkElement)button.Tag).ClearValue(FrameworkElement.DataContextProperty);
            button.IsEnabled = false;
        }
    }
}
