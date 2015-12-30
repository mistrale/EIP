using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tavis.UriTemplates;
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
    public sealed partial class Profil : Page
    {
        bool isMyProfil = false;
        bool isFriend = false;
        bool isVisibile = false;
        private string id;
        private bool flag;

        class RequeteResponse
        {

            public string status { get; set; }
            public string message { get; set; }
            public User response { get; set; }

        }

        class FriendResponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public Friend response { get; set; }
        }

        private RequeteResponse message;
        private string responseString;

        private async void sendRequestToFriend()
        {
            {
                var httpClient = new HttpClient(new HttpClientHandler());
                try
                {
                    var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
                    string url = ("http://52.31.151.160:3000/volunteers/" + id + "/addfriend" + "{?token}");
                    HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                    response.EnsureSuccessStatusCode();
                    responseString = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseString.ToString());
                    message = JsonConvert.DeserializeObject<RequeteResponse>(responseString);
                    if (Int32.Parse(message.status) != 200)
                    {
                        Frame.Navigate(typeof(Profil), id);
                    }
                    else
                    {
                        warningTextBox.Text = message.message;
                    }
                    Debug.WriteLine(message);
                }
                catch (HttpRequestException e)
                {
                    Debug.WriteLine(e.Message);
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
        }

        private async void getFriends()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string id = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate("http://52.31.151.160:3000/volunteers/" + id + "/friends" + "{?token}");
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
                }
                Debug.WriteLine(message);
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
        }

        private async Task updateGUI()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                Debug.WriteLine(message);
                if (flag)
                {
                    alertButtonNotity.Visibility = Visibility.Visible;
                    alertButton.Visibility = Visibility.Collapsed;
                }
                else
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

        public static bool ValidateEmail(string str)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(str, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        }

        private bool checkRegistrationField()
        {
            if (lastNameEdit.Text == String.Empty || lastNameEdit.Text.Equals("Nom", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Name empty.";
                return false;
            }

            if (firstNameEdit.Text == String.Empty || firstNameEdit.Text.Equals("Prénom", StringComparison.Ordinal))
            {
                warningTextBox.Text = "FirstName empty.";
                return false;
            }
            if (emailEdit.Text == String.Empty || emailEdit.Text.Equals("Email", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Email empty.";
                return false;
            }
            if (!ValidateEmail(emailEdit.Text))
            {
                warningTextBox.Text = "Invalid email.";
                return false;
            }
            if (passwordEdit.Password == String.Empty || passwordEdit.Password.Equals("Password", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Password empty.";
                return false;
            }
            if (passwordConfirmationEdit.Password == String.Empty
                || passwordConfirmationEdit.Password.Equals("Confirmation Password", StringComparison.Ordinal))
            {
                warningTextBox.Text = "CofirmationPassword empty.";
                return false;
            }
            if (!passwordEdit.Password.Equals(passwordConfirmationEdit.Password, StringComparison.Ordinal))
            {
                warningTextBox.Text = "Password doesn't match.";
                return false;
            }
            return true;
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
            Frame.Navigate(typeof(Accueil));
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

        private void initializeInformations()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            GenreBox.Items.Add("Femme");
            GenreBox.Items.Add("Homme");
            GenreBox.Items.Add("M. / Mme.");

            emailEdit.Text = (string)localSettings.Values["mail"];
            firstNameEdit.Text = (string)localSettings.Values["firstname"];
            lastNameEdit.Text = (string)localSettings.Values["lastname"];
            if (localSettings.Values["city"] != null)
                cityEdit.Text = (string)localSettings.Values["city"];
            if (localSettings.Values["birthday"] != null)
                birthdayEdit.Date = (DateTime)Convert.ToDateTime(localSettings.Values["birdtday"]);
            if (localSettings.Values["genre"] != null)
            {
                if ((string)localSettings.Values["genre"] == "Femme")
                    GenreBox.SelectedIndex = 0;
                else if ((string)localSettings.Values["genre"] == "Homme")
                    GenreBox.SelectedIndex = 1;
                else
                    GenreBox.SelectedIndex = 2;
            }
            else
                GenreBox.SelectedIndex = 2;
            bool allowed = (bool)localSettings.Values["allowgps"];
            if (allowed)
                allowGPSEdit.IsChecked = true;
            else
                allowGPSEdit.IsChecked = false;
            passwordEdit.Password = (string)localSettings.Values["password"];
            passwordConfirmationEdit.Password = (string)localSettings.Values["password"];
        }

        public Profil()
        {
            this.InitializeComponent();
            initializeInformations();

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadCoroutine();
            getFriends();
            id = e.Parameter as string;
            if (id.Equals((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString())) 
            {

                isMyProfil = true;
            } else
            {
                isMyProfil = false;
            }
            if (!isMyProfil)
            {
                emailEdit.IsHitTestVisible = false;
                firstNameEdit.IsHitTestVisible = false;
                lastNameEdit.IsHitTestVisible = false;
                birthdayEdit.IsHitTestVisible = false;
                GenreBox.IsHitTestVisible = false;
                cityEdit.IsHitTestVisible = false;
                passwordConfirmationEdit.Visibility = Visibility.Collapsed;
                passwordEdit.Visibility = Visibility.Collapsed;
                passwordText.Visibility = Visibility.Collapsed;
                passwordConfirmationText.Visibility = Visibility.Collapsed;
                allowGPSEdit.Visibility = Visibility.Collapsed;
                addFriendButton.Visibility = Visibility.Visible;
                sendMsgButton.Visibility = Visibility.Visible;
                editProfilButton.Visibility = Visibility.Collapsed;

            }
            else
            {
                emailEdit.IsHitTestVisible = true;
                firstNameEdit.IsHitTestVisible = true;
                lastNameEdit.IsHitTestVisible = true;
                birthdayEdit.IsHitTestVisible = true;
                GenreBox.IsHitTestVisible = true;
                cityEdit.IsHitTestVisible = true;
                addFriendButton.Visibility = Visibility.Collapsed;
                sendMsgButton.Visibility = Visibility.Collapsed;
                passwordConfirmationEdit.Visibility = Visibility.Visible;
                passwordEdit.Visibility = Visibility.Visible;
                passwordText.Visibility = Visibility.Visible;
                passwordConfirmationText.Visibility = Visibility.Visible;
                allowGPSEdit.Visibility = Visibility.Visible;
                editProfilButton.Visibility = Visibility.Visible;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void updateProfil()
        {
            string url = "http://52.31.151.160:3000/volunteers/" + ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]).ToString();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("mail", emailEdit.Text),
                        new KeyValuePair<string, string>("password", passwordEdit.Password),
                        new KeyValuePair<string, string>("firstname", firstNameEdit.Text),
                        new KeyValuePair<string, string>("lastname", lastNameEdit.Text),
                        new KeyValuePair<string, string>("birtyhday", birthdayEdit.Date.ToString()),
                        new KeyValuePair<string, string>("genre", GenreBox.SelectedValue.ToString()),
                        new KeyValuePair<string, string>("allowgps", allowGPSEdit.IsChecked.ToString()),
                        new KeyValuePair<string, string>("genre", GenreBox.SelectedValue.ToString()),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };

            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                Loading.IsActive = false;
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                message = JsonConvert.DeserializeObject<RequeteResponse>(responseString);
                if (Int32.Parse(message.status) != 200)
                {
                    warningTextBox.Text = message.message;
                }
                else
                {
                    try
                    {
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        localSettings.Values["mail"] = emailEdit.Text;
                        localSettings.Values["firstname"] = firstNameEdit.Text;
                        localSettings.Values["lastname"] = lastNameEdit.Text;
                        localSettings.Values["city"] = cityEdit.Text;
                        localSettings.Values["birdthday"] = birthdayEdit.Date.ToString();
                        localSettings.Values["genre"] = GenreBox.SelectedValue ;
                        localSettings.Values["allowgps"] = allowGPSEdit.IsChecked;
                        localSettings.Values["password"] = passwordEdit.Password;
                        this.Frame.Navigate(typeof(Profil));
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
                Debug.WriteLine(e.Message);
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

        private void editProfilButton_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRegistrationField())
                return;
            warningTextBox.Text = "";
            Loading.IsActive = true;
            updateProfil();
        }

        public void addFriendButtonClick(object sender, RoutedEventArgs e)
        {
            sendRequestToFriend();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["search"] = searchTextBox.Text;
            Frame.Navigate(typeof(Research));
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            searchTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}
