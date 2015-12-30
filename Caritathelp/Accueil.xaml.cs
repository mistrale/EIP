using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Tavis.UriTemplates;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accueil : Page
    {
        bool isVisibile = false;
        private bool flag;

        class RequeteResponse
        {

            public string status { get; set; }
            public string message { get; set; }
            public User response { get; set; }

        }

        private RequeteResponse message;
        private string responseString;

        private async void getNotification()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string id = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate("http://52.31.151.160:3000/volunteers/" + id + "{?token}");
                template.AddParameter("id", id);
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                message = JsonConvert.DeserializeObject<RequeteResponse>(responseString);
                if (Int32.Parse(message.status) != 200)
                {

                }
                else
                {
                    Debug.WriteLine(message.response.notifications.add_friend.Count);
                    if (message.response.notifications.add_friend.Count > (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["notificationsFriends"])
                    {
                        flag = true;
                        updateGUI();
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values["notificationsFriends"] = message.response.notifications.add_friend.Count;
                        Debug.WriteLine("On a recu une nouvelle notification !");
                    }
                    else
                    {
                        flag = false;
                        updateGUI();
                        Debug.WriteLine("0 nouvelles notificaitons");
                    }
                }
            }
            catch (HttpRequestException e)
            {
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            Debug.WriteLine(message.status);
        }

        private async Task updateGUI()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                Debug.WriteLine(message);
                if (flag)
                {
                    alertButtonNotity.Visibility = Visibility.Visible;
                    alertButton.Visibility = Visibility.Collapsed;
                } else
                {
                    alertButtonNotity.Visibility = Visibility.Collapsed;
                    alertButton.Visibility = Visibility;
                }
            });
        }

        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            getNotification();
            autoEvent.Set();
        }

        public void loadCoroutine()
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            // Create an inferred delegate that invokes methods for the timer.
            TimerCallback tcb = this.CheckStatus;

            // Create a timer that signals the delegate to invoke 
            // CheckStatus after one second, and every 1/4 second 
            // thereafter.
            Timer stateTimer = new Timer(tcb, autoEvent, 1000, 10000);

            // When autoEvent signals, change the period to every
            // 1/2 second.
            autoEvent.WaitOne(1000);

            // When autoEvent signals the second time, dispose of 
            // the timer.
            autoEvent.WaitOne(10000);
        }

        public void notificationButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Notification));
        }

        public void passportButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void eventButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void logoutButtonClick(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("mail");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("firstname");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("lastname");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("city");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("genre");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("allowedgps");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("birthday");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("id");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("password");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("token");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("notificationsFriends");
            this.Frame.Navigate(typeof(MainPage));
        }

        public void wtf2ButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void associationButtonClick(object sender, RoutedEventArgs e)
        {

        }


        public void homeButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void profilButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Profil), (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString());
        }

        public void friendsButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void setVisibility(object sender, RoutedEventArgs e)
        {
            if (isVisibile)
            {
                secondBorder.Visibility = Visibility.Collapsed;
                firstBorder.Margin = new Thickness(0, 570, 0, 0);
                isVisibile = false;
            }
            else
            {
                isVisibile = true;
                firstBorder.Margin = new Thickness(0, 500, 0, 70);
                secondBorder.Visibility = Visibility.Visible;
            }
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public Accueil()
        {
            this.InitializeComponent();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            alertButtonNotity.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadCoroutine();
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            searchTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["search"] = searchTextBox.Text;
            Frame.Navigate(typeof(Research));
        }
    }
}
