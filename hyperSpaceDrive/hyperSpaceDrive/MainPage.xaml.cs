using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace hyperSpaceDrive
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void playGame(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void aboutEvent(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit the application ?", "Warning", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
            else
            {
                //add code to add info to isolated storage
            }
            base.OnBackKeyPress(e);
        }

        private void helpPage(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

        private void highScore(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/HighScores.xaml", UriKind.Relative));
        }

    }
}