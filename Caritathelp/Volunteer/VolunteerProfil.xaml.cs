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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.Volunteer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VolunteerProfil : Page
    {
        private string id;

        class SimpleResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        class NotificationResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<GlobalNotification> response { get; set; }
        }

        class FriendResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<FriendShip> response { get; set; }
        }

        class UserResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public User response { get; set; }

        }

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest picture;
        private UserResponse message;
        private UserResponse user;
        private FriendResponse friends;
        private SimpleResponse simpleResponse;
        private NotificationResponse notifs;
        private string responseString;
        Grid newsGrid;

        public VolunteerProfil()
        {
            this.InitializeComponent();
            informationBox.Text = "";
        }

        private async void getNotification()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string id = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate("http://api.caritathelp.me/volunteers/" + id + "/notifications{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                notifs = JsonConvert.DeserializeObject<NotificationResponse>(responseString);
                if (notifs.status != 200)
                {
                    informationBox.Text = "Failed to get notifications";
                }
                else
                {

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
            catch (NullReferenceException e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private async void getFriends()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://api.caritathelp.me/volunteers/" + id + "/friends" + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(uri);
                Debug.WriteLine(responseString);
                friends = JsonConvert.DeserializeObject<FriendResponse>(responseString);
                if (friends.status != 200)
                {
                    informationBox.Text = friends.message;
                }
                else
                {
                    var friend = friends.response.FirstOrDefault(c => c.id.ToString().Equals((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString(), StringComparison.Ordinal));
                    if (friend != null)
                    {
                        removeButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        var notif = notifs.response.FirstOrDefault(c => c.sender_id.Equals(id, StringComparison.Ordinal) && c.notif_type.Equals("AddFriend", StringComparison.Ordinal));
                        if (notif != null)
                        {
                            acceptUserButton.Visibility = Visibility.Visible;
                            refuseUserButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (!id.Equals((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString()))
                                addButton.Visibility = Visibility.Visible;
                        }
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
                Debug.WriteLine("Fail in getting friends");
            }
        }

        private async void sendRequestToFriend()
        {
            {
                var httpClient = new HttpClient(new HttpClientHandler());
                try
                {
                    var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("volunteer_id", id)
                    };
                    string url = ("http://api.caritathelp.me/friendship/add");
                    HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                    response.EnsureSuccessStatusCode();
                    responseString = await response.Content.ReadAsStringAsync();

                    message = JsonConvert.DeserializeObject<UserResponse>(responseString);
                    if (message.status != 200)
                    {
                        informationBox.Text = message.message;
                    }
                    else
                    {
                        Frame.Navigate(typeof(VolunteerProfil), id);
                    }
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
            }
        }

        private async void denyFriend()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            string myID = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
            var user = notifs.response.FirstOrDefault(c => c.sender_id.Equals(id));
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", user.id),
                        new KeyValuePair<string, string>("acceptance", "false")
                    };
                string url = ("http://api.caritathelp.me/friendship/reply");
                Debug.WriteLine(url);
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                message = JsonConvert.DeserializeObject<UserResponse>(responseString);
                if (message.status != 200)
                {
                    informationBox.Text = message.message;
                }
                else
                {
                    Frame.Navigate(typeof(VolunteerProfil), id);
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
                Debug.WriteLine("Fail in deny friends");

            }
        }

        private async void acceptFriend()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            string myID = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
            var user = notifs.response.FirstOrDefault(c => c.sender_id.Equals(id));
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", user.id),
                        new KeyValuePair<string, string>("acceptance", "true")
                    };
                string url = ("http://api.caritathelp.me/friendship/reply");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                message = JsonConvert.DeserializeObject<UserResponse>(responseString);
                if (message.status != 200)
                {
                    informationBox.Text = message.message;
                }
                else
                {
                    Frame.Navigate(typeof(VolunteerProfil), id);
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
                Debug.WriteLine("Fail in accept friends");
            }
        }

        private async void removeFriend()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://api.caritathelp.me/friendship/remove" + "{?token,id}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                template.AddParameter("id", id);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.DeleteAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                simpleResponse = JsonConvert.DeserializeObject<SimpleResponse>(responseString);
                if (simpleResponse.status != 200)
                {
                    informationBox.Text = simpleResponse.message;
                }
                else
                {
                    Frame.Navigate(typeof(VolunteerProfil), id);
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
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void assocClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VolunteerAssociation), id);
        }

        public void friendsClick(object sender, RoutedEventArgs e)
        {
            friendsStruct tmp = new friendsStruct();
            tmp.id = id;
            tmp.friends = friends.response;
            Frame.Navigate(typeof(VolunteerFriends), tmp);
        }

        public void eventClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VolunteerEvenement), id);
        }

        public void infosClick(object sender, RoutedEventArgs e)
        {
            UserStruct tmp = new UserStruct();
            tmp.id = id;
            tmp.user = user.response;
            Frame.Navigate(typeof(VolunteerInformations), tmp);
        }

        public void addUserClick(object sender, RoutedEventArgs e)
        {
            sendRequestToFriend();
        }

        private void acceptUserClick(object sender, RoutedEventArgs e)
        {
            acceptFriend();
        }

        private void refuseUserClick(object sender, RoutedEventArgs e)
        {
            denyFriend();
        }

        private void removeUserClick(object sender, RoutedEventArgs e)
        {
            removeFriend();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }

        private async void getPicture()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://api.caritathelp.me/volunteers/" + id + "/main_picture{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                picture = JsonConvert.DeserializeObject<PictureRequest>(responseString);
                if (picture.status != 200)
                {

                }
                else
                {
                    ImageBrush myBrush = new ImageBrush();
                    if (picture.response != null)
                    {
                        myBrush.ImageSource =
                            new BitmapImage(new Uri("http://api.caritathelp.me" + picture.response.picture_path.thumb.url, UriKind.Absolute));
                    }
                    else
                        myBrush.ImageSource = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
                    logo.Fill = myBrush;

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
                Debug.WriteLine(responseString);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void initNews()
        {
            newsGrid = new Grid();
            newsGrid.VerticalAlignment = VerticalAlignment.Top;
            newsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < 10; i++)
            {
                newsGrid.RowDefinitions.Add(new RowDefinition());

                // main new's grid
                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 10, 0, 10);
                grid.Background = new SolidColorBrush(Color.FromArgb(100, 75, 175, 80));

                // grid new's info

                // row username
                var row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                // row time published
                var row2 = new RowDefinition();
                row2.Height = new GridLength(30);
                grid.RowDefinitions.Add(row2);

                // row content
                var rowContent = new RowDefinition();
                grid.RowDefinitions.Add(rowContent);

                // row share / comment
                var row4 = new RowDefinition();
                grid.RowDefinitions.Add(row4);

                //column image user / share
                var colum = new ColumnDefinition();
                colum.Width = new GridLength(60);
                grid.ColumnDefinitions.Add(colum);

                //column comment
                var colum2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(colum2);

                // image profil
                Image btn = new Image();
                btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                grid.Children.Add(btn);

                // username
                // CHANGE BY HYPERLINK
                TextBlock poster = new TextBlock();
                poster.Text = "Aude Sikorav";
                poster.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                poster.Margin = new Thickness(10, 5, 10, 5);
                poster.FontSize = 14;
                Grid.SetColumn(poster, 1);
                Grid.SetRow(poster, 0);
                grid.Children.Add(poster);

                // time published
                TextBlock date = new TextBlock();
                date.Text = "7h25";
                date.Margin = new Thickness(10, 0, 10, 0);
                date.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                Grid.SetColumn(date, 1);
                Grid.SetRow(date, 1);
                grid.Children.Add(date);

                // content
                TextBlock content = new TextBlock();
                content.Margin = new Thickness(10, 10, 10, 10);
                content.FontSize = 12;
                content.TextWrapping = TextWrapping.Wrap;
                content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                content.Text = "Bonjour a tous, voici une publication test pour voir combien en longeur elle va occupée et voir si Windows phone qui pue la merde gere tout seul la mise en place";
                Grid.SetColumn(content, 0);
                Grid.SetColumnSpan(content, 2);
                Grid.SetRow(content, 2);
                grid.Children.Add(content);

                newsGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, i);
            }
            newsScroll.Content = newsGrid;
        }

        private async void getNews()
        {
            // request to getNews();
            initNews();
        }

        private async void getInformation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {

                var template = new UriTemplate("http://api.caritathelp.me/volunteers/" + id + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                user = JsonConvert.DeserializeObject<UserResponse>(responseString);
                if (user.status != 200)
                {
                    informationBox.Text = "Failed to get informations";
                }
                else
                {
                    nameTextBox.Text = user.response.firstname + " " + user.response.lastname;
                    getFriends();
                    getPicture();
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
                System.Diagnostics.Debug.WriteLine(responseString);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            getNotification();
            getInformation();
            getNews();
        }
    }
}
