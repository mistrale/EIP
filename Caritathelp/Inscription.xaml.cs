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

        class RequeteResponse
        {

            public string status { get; set; }
            public string message { get; set; }
            public User response { get; set; }

        }

        private RequeteResponse message;
        private string responseString;

        public static bool ValidateEmail(string str)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(str, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
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
            if (EmailTextBox.Text == String.Empty || EmailTextBox.Text.Equals("Email", StringComparison.Ordinal))
            {
                warningTextBlock.Text = "Email empty.";
                return false;
            }
            if (!ValidateEmail(EmailTextBox.Text))
            {
                warningTextBlock.Text = "Invalid email.";
                return false;
            }
            if (PasswordPasswordBox.Password == String.Empty || PasswordPasswordBox.Password.Equals("Password", StringComparison.Ordinal))
            {
                warningTextBlock.Text = "Password empty.";
                return false;
            }
            if (PasswordConfirmationPasswordBox.Password == String.Empty
                || PasswordConfirmationPasswordBox.Password.Equals("Confirmation Password", StringComparison.Ordinal))
            {
                warningTextBlock.Text = "CofirmationPassword empty.";
                return false;
            }
            if (!PasswordPasswordBox.Password.Equals(PasswordConfirmationPasswordBox.Password, StringComparison.Ordinal))
            {
                warningTextBlock.Text = "Password doesn't match.";
                return false;
            }
            return true;
        }

        private async void createNewUser()
        {
            string url = "http://52.31.151.160:3000/volunteers/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("lastname", NameTextBox.Text),
                        new KeyValuePair<string, string>("firstname", FirstNameTextBox.Text),
                        new KeyValuePair<string, string>("mail", EmailTextBox.Text),
                        new KeyValuePair<string, string>("birthday", DateBox.ToString()),
                        new KeyValuePair<string, string>("password", PasswordPasswordBox.Password)
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                progressBar.IsActive = false;
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                message = JsonConvert.DeserializeObject<RequeteResponse>(responseString);

                if (Int32.Parse(message.status) != 200)
                {
                    warningTextBlock.Text = message.message;
                }
                else
                {
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
            catch (HttpRequestException e)
            {
                progressBar.IsActive = false;
                warningTextBlock.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void Register_click(object sender, RoutedEventArgs e)
        {
            if (!checkRegistrationField())
                return;
            progressBar.IsActive = true;
            createNewUser();
            warningTextBlock.Text = "";       
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
