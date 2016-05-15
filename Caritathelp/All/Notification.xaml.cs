using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Notification : Page
    {
        class NotificationResponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public GlobalNotification response { get; set; }
        }

        private string responseString;
        private NotificationResponse notifications;

        private void initNotifications(int nbRows)
        {
            grid.Height = nbRows * 100;
            grid.Width = 375;
            for (int i = 0; i < nbRows; ++i)
                grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int x = 0; x < notifications.response.add_friend.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = grid.Width;
                btn.Content = new TextBlock
                {
                    Text = notifications.response.add_friend[x].firstname + " " + notifications.response.add_friend[x].lastname + " vous a envoyé une demande d'ajout.",
                    TextWrapping = TextWrapping.Wrap
                };
                btn.Click += new RoutedEventHandler(UserButtonClick);
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
            for (int x = 0; x < notifications.response.assoc_invite.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = grid.Width;
                btn.Content = new TextBlock
                {
                    Text = "L'association \"" + notifications.response.assoc_invite[x].name + "\" vous a envoyé une demande d'ajout.",
                    TextWrapping = TextWrapping.Wrap
                };
                btn.Click += new RoutedEventHandler(AssociationButtonClick);
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
            for (int x = 0; x < notifications.response.event_invite.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = grid.Width;
                btn.Content = new TextBlock
                {
                    Text = "Vous avez été invité à participer à l'évènement \"" + notifications.response.event_invite[x].title + "\".",
                    TextWrapping = TextWrapping.Wrap
                };
                btn.Click += new RoutedEventHandler(EventButtonClick);
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
        }

        private void EventButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = notifications.response.event_invite[x].id.ToString();
            Frame.Navigate(typeof(EventProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = notifications.response.add_friend[x].id.ToString();
            Frame.Navigate(typeof(Volunteer.VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void AssociationButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = notifications.response.assoc_invite[x].id.ToString();
            Frame.Navigate(typeof(AssociationProfil), id);
            // identify which button was clicked and perform necessary actions
        }


        private async void getNotifications()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string id = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate("http://52.31.151.160:3000/volunteers/" + id + "/notifications" + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                notifications = JsonConvert.DeserializeObject<NotificationResponse>(responseString);
                if (Int32.Parse(notifications.status) != 200)
                {

                }
                else
                {
                    Debug.WriteLine(responseString);
                    initNotifications(notifications.response.add_friend.Count + notifications.response.assoc_invite.Count
                        + notifications.response.event_invite.Count);
                }
                Debug.WriteLine(notifications);
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

        public Notification()
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
            getNotifications();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Research));
        }
    }
}
