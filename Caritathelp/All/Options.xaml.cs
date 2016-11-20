using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Options : Page
    {
        public Options()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void profil_Click(object sender, RoutedEventArgs e)
        {
            Models.Model model = new Models.Volunteer((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]);
            this.Frame.Navigate(typeof(Models.GenericProfil), model);
        }

        private void friends_Click(object sender, RoutedEventArgs e)
        {
            Models.InfosListModel tmp = new Models.InfosListModel();
            tmp.id = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"];
            tmp.typeModel = "volunteer";
            tmp.listTypeModel = "volunteer";
            Frame.Navigate(typeof(Models.GenericListModelManagement), tmp);
        }

        private void association_Click(object sender, RoutedEventArgs e)
        {
            Models.InfosListModel tmp = new Models.InfosListModel();
            tmp.id = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"];
            tmp.typeModel = "volunteer";
            tmp.listTypeModel = "assoc";
            Frame.Navigate(typeof(Models.GenericListModelManagement), tmp);
        }

        private void events_Click(object sender, RoutedEventArgs e)
        {
            Models.InfosListModel tmp = new Models.InfosListModel();
            tmp.id = (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"];
            tmp.typeModel = "volunteer";
            tmp.listTypeModel = "event";
            Frame.Navigate(typeof(Models.GenericListModelManagement), tmp);
        }

        private void invitation_Click(object sender, RoutedEventArgs e)
        {
            Models.Model tmp = new Models.Volunteer((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]);
            Frame.Navigate(typeof(Models.GenericInvitation), tmp);
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
