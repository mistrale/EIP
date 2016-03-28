using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GestionAssociation : Page
    {
        private string id;

        public void manageMemberClick(object sender, RoutedEventArgs e)
        {

        }

        public void notificationClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotificationAssociation), id);
        }

        public void newEventClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventCreation));
        }

        public void newPublicationClick(object send, RoutedEventArgs e)
        {

        }

        public GestionAssociation()
        {
            this.InitializeComponent();
            searchBox.Items.Add("Volontaire");
            searchBox.Items.Add("Association");
            searchBox.Items.Add("Event");
            searchBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            searchTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void associationButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void messageButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void moreButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Options));
        }

        public void homeButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Accueil));
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["search"] = searchTextBox.Text;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["typeSearch"] = searchBox.SelectedItem.ToString();
            Frame.Navigate(typeof(Research));
        }

        public void alertButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Notification));
        }
    }
}
