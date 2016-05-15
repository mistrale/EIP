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
        private Association assoc;

        public void manageMemberClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Caritathelp.AssociationPage.ManageMembersAssoc), assoc);
        }

        public void notificationClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NotificationAssociation), assoc);
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
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            assoc = e.Parameter as Association;
            if (assoc.rights.Equals("owner", StringComparison.Ordinal))
                deleteAssoc.Visibility = Visibility.Visible;
            else
                deleteAssoc.Visibility = Visibility.Collapsed;
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }
    }
}
