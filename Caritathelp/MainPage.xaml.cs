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
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        class Token
        {
            public string token { get; set; }
        }
        class RequestResponse
        {
            public string status { get; set; } 
            public string message { get; set; }
            public User response { get; set; }
        }

        private RequestResponse message;
        private string responseString;



        public void HyperlinkButton_forgettenPassword(object sender, RoutedEventArgs e)
        {

        }

        public void Inscription_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Inscription));
        }

        private async void connect()
        {
            string url = "http://52.31.151.160:3000/login/";
            var values = new List<KeyValuePair<string, string>>
                    {

                        //new KeyValuePair<string, string>("mail", Email.Text),
                        //new KeyValuePair<string, string>("password", Password.Password)
                        new KeyValuePair<string, string>("mail", Email.Text),
                        new KeyValuePair<string, string>("password", Password.Password)
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                Loading.IsActive = false;
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                message = JsonConvert.DeserializeObject<RequestResponse>(responseString);
                if (Int32.Parse(message.status) != 200)
                {
                    warningTextBox.Text = message.message;
                }
                else
                {
                    Email.Text = "Email";
                    Password.Password = "password";


                    try
                    {
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        localSettings.Values["mail"] = message.response.mail;
                        localSettings.Values["firstname"] = message.response.firstname;
                        localSettings.Values["lastname"] = message.response.lastname;
                        localSettings.Values["city"] = message.response.city;
                        localSettings.Values["birdthday"] = message.response.birthday;
                        localSettings.Values["gender"] = message.response.gender;
                        localSettings.Values["allowgps"] = message.response.allowgps;
                        localSettings.Values["password"] = Password.Password;
                        localSettings.Values["id"] = message.response.id;
                        localSettings.Values["token"] = message.response.token;
  //                      localSettings.Values["notifications"] = JsonConvert.SerializeObject(message.response.notifications);
                        this.Frame.Navigate(typeof(Accueil));
                    }
                    catch (System.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }

                }
            }
            catch (HttpRequestException e)
            {
                warningTextBox.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                Loading.IsActive = false;
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void Connexion_click(object sender, RoutedEventArgs e)
        {
            warningTextBox.Text = "";
            Loading.IsActive = true;
            connect();
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

        public MainPage()
        {
            this.InitializeComponent();
            warningTextBox.Text = "";
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
