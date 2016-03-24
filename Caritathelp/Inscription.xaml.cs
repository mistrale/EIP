using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Threading;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Inscription : Page
    {
        public TextBox Name
        {
            get { return NameTextBox; }
        }

        public TextBlock Warning
        {
            get { return warningTextBlock; }
        }

        private bool checkRegistrationField()
        {
            if (NameTextBox.Text == String.Empty || NameTextBox.Text.Equals("Nom", StringComparison.Ordinal))
            {
                warningTextBlock.Text = "Name empty.";
                return false;
            }
            
            if (FirstNameTextBox.Text == String.Empty || FirstNameTextBox.Text.Equals("Prénom", StringComparison.Ordinal))
            {
                warningTextBlock.Text = "FirstName empty.";
                return false;
            }
            if (City.Text == String.Empty || City.Text.Equals("Ville", StringComparison.Ordinal))
            {
                warningTextBlock.Text = "City empty";
                return false;
            }
            return true;
        }

        public void Register_click()
        {
            if (!checkRegistrationField())
                return;
            warningTextBlock.Text = "";
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values["firstname"] = FirstNameTextBox.Text;
            settings.Values["lastname"] = NameTextBox.Text;
            settings.Values["city"] = City.Text;
            settings.Values["genre"] = GenreBox.SelectedValue.ToString();
            settings.Values["birthday"] = birthday.ToString();
            this.Frame.Navigate(typeof(FinalInscription));
        }

        public void Register_click(object sender, RoutedEventArgs e)
        {
            Register_click();
        }


        public void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox tb = (PasswordBox)sender;
            tb.Password = string.Empty;
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public Inscription()
        {
            this.InitializeComponent();
            GenreBox.Items.Add("M. / Mme.");
            GenreBox.Items.Add("m");
            GenreBox.Items.Add("f");
            GenreBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
