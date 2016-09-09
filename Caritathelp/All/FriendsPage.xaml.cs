using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendsPage : Page
    {
        class FriendsRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<FriendShip> response { get; set; }
        }

        class FriendsWaitingRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<FriendWaiting> response { get; set; }
        }

        private FriendsRequest friends;
        private FriendsWaitingRequest friendsPending;

        Grid friendsGrid;
        private string responseString;


        public FriendsPage()
        {
            this.InitializeComponent();
            getFriends();
        }

        public void sentButtonClick(object sender, RoutedEventArgs e)
        {
            getSent();
        }

        private async void getSent()
        {
            friendsGrid = new Grid();
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/friend_requests" + "{?token,sent}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                template.AddParameter("sent", "true");
                var uri = template.Resolve();
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(uri);
                Debug.WriteLine(responseString);
                friendsPending = JsonConvert.DeserializeObject<FriendsWaitingRequest>(responseString);
                if (friendsPending.status != 200)
                {
                    Debug.WriteLine(friendsPending.message);
                }
                else
                {
                    friendsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    friendsGrid.VerticalAlignment = VerticalAlignment.Top;
                    friendsGrid.Width = 375;
                    for (int i = 0; i < friendsPending.response.Count; i++)
                    {
                        friendsGrid.RowDefinitions.Add(new RowDefinition());

                        // Username
                        Button title = new Button();
                        title.Tag = i;
                        title.Content = friendsPending.response[i].firstname + " " + friendsPending.response[i].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));
                        title.Click += new RoutedEventHandler(UserWaitingButtonClick);
                        title.FontSize = 20;
                        title.Height = 100;
                        title.Width = 350;
                        title.HorizontalAlignment = HorizontalAlignment.Center;
                        Debug.WriteLine("tata");
                        friendsGrid.Children.Add(title);
                        Grid.SetColumn(title, 0);
                        Grid.SetRow(title, i);
                    }
                    friendsGrid.Visibility = Visibility.Visible;
                    scroll.Content = friendsGrid;
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
                Debug.WriteLine("Fail in getting friends");
            }
        }

        public void invitButtonClick(object sender, RoutedEventArgs e)
        {
            getInvit();
        }

        private async void getInvit()
        {
            friendsGrid = new Grid();
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/friend_requests" + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(uri);
                Debug.WriteLine(responseString);
                friendsPending = JsonConvert.DeserializeObject<FriendsWaitingRequest>(responseString);
                if (friendsPending.status != 200)
                {
                    Debug.WriteLine(friendsPending.message);
                }
                else
                {
                    friendsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    friendsGrid.VerticalAlignment = VerticalAlignment.Top;
                    friendsGrid.Width = 375;
                    for (int i = 0; i < friendsPending.response.Count; i++)
                    {
                        friendsGrid.RowDefinitions.Add(new RowDefinition());

                        // Username
                        Button title = new Button();
                        title.Tag = i;
                        title.Content = friendsPending.response[i].firstname + " " + friendsPending.response[i].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));
                        title.Click += new RoutedEventHandler(UserWaitingButtonClick);
                        title.FontSize = 20;
                        title.Height = 100;
                        title.Width = 350;
                        title.HorizontalAlignment = HorizontalAlignment.Center;
                        Debug.WriteLine("tata");
                        friendsGrid.Children.Add(title);
                        Grid.SetColumn(title, 0);
                        Grid.SetRow(title, i);
                    }
                    friendsGrid.Visibility = Visibility.Visible;
                    scroll.Content = friendsGrid;
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
                Debug.WriteLine("Fail in getting friends");
            }
        }


        public void friendsButtonClick(object sender, RoutedEventArgs e)
        {
            getFriends();
        }

        private async void getFriends()
        {
            friendsGrid = new Grid();
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/volunteers/" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"] + "/friends" + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(uri);
                Debug.WriteLine(responseString);
                friends = JsonConvert.DeserializeObject<FriendsRequest>(responseString);
                if (friends.status != 200)
                {
                    Debug.WriteLine(friends.message);
                }
                else
                {
                    friendsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    friendsGrid.VerticalAlignment = VerticalAlignment.Top;
                    friendsGrid.Width = 375;
                    for (int i =0; i < friends.response.Count; i++)
                    {
                        friendsGrid.RowDefinitions.Add(new RowDefinition());

                        // Username
                        Button title = new Button();
                        title.Tag = i;
                        title.Content = friends.response[i].firstname + " " + friends.response[i].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));
                        title.Click += new RoutedEventHandler(UserButtonClick);
                        title.FontSize = 20;
                        title.Height = 100;
                        title.Width = 350;
                        title.HorizontalAlignment = HorizontalAlignment.Center;
                        Debug.WriteLine("tata");
                        friendsGrid.Children.Add(title);
                        Grid.SetColumn(title, 0);
                        Grid.SetRow(title, i);
                    }
                    friendsGrid.Visibility = Visibility.Visible;
                    scroll.Content = friendsGrid;
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
                Debug.WriteLine("Fail in getting friends");
            }
        }

        private void UserWaitingButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag.ToString());
            string id = friendsPending.response[i].id.ToString();
            Frame.Navigate(typeof(Volunteer.VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag.ToString());
            string id = friends.response[i].id.ToString();
            Frame.Navigate(typeof(Volunteer.VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Research));
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
