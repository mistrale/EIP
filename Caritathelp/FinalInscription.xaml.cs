using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinalInscription : Page
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
            if (conditionBox.IsChecked == false)
            {
                warningTextBlock.Text = "Conditions not checked";
                return false;
            }
            return true;
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

        private async void createNewUser()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string url = "http://52.31.151.160:3000/volunteers/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("lastname", (string)settings.Values["lastname"]),
                        new KeyValuePair<string, string>("firstname", (string)settings.Values["firstname"]),
                        new KeyValuePair<string, string>("gender", (string)settings.Values["genre"]),
                        new KeyValuePair<string, string>("city", (string)settings.Values["city"]),
                        new KeyValuePair<string, string>("allowgps", geolocalisationBox.IsChecked.ToString()),
                        new KeyValuePair<string, string>("mail", EmailTextBox.Text),
                        new KeyValuePair<string, string>("birthday", (string)settings.Values["birthday"]),
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

        public FinalInscription()
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
