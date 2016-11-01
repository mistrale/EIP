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
            this.Frame.Navigate(typeof(All.Inscription));
        }

        private async void connect()
        {
            HttpHandler http = HttpHandler.getHttp();
            string url = "/auth/sign_in/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("email", Email.Text),
                        new KeyValuePair<string, string>("password", Password.Password)
                    };

            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.POST);
            if (jObject == null)
            {
                Debug.WriteLine("failed to connect");
                Loading.IsActive = false;
                return;
            }
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            try
            {
                localSettings.Values["password"] = (string)Password.Password;
                localSettings.Values["id"] = jObject["id"].ToString();
                localSettings.Values["thumb_path"] = (string)jObject["thumb_path"];
                localSettings.Values["mail"] = Email.Text;
                localSettings.Values["allowgps"] = (bool)jObject["allowgps"];
                //SocketHandler.getWS();
                Loading.IsActive = false;
                this.Frame.Navigate(typeof(All.Accueil));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
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
