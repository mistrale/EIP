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

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventProfil : Page
    {
        private string id;
        private string notif_id;

        class NotificationResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<GlobalNotification> response { get; set; }
        }

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest picture;

        private NotificationResponse notifications;


        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public EventModel response { get; set; }
        }

        private EventRequest events;

        private SimpleRequest simple;
        private String responseString;

        private async void joinEventRequest()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("event_id", id)

                    };
                string url = ("http://api.caritathelp.me/guests/join");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();

                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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

        public void joinEventClick(object send, RoutedEventArgs e)
        {
            joinEventRequest();
        }

        private async void accepteInvit()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", notif_id),
                        new KeyValuePair<string, string>("acceptance", "true")
                    };
                string url = ("http://api.caritathelp.me/guests/reply_invite");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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

        private async void refuseInvit()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", notif_id),
                        new KeyValuePair<string, string>("acceptance", "false")
                    };
                string url = ("http://api.caritathelp.me/guests/reply_invite");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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

        public void acceptInvitationClick(object send, RoutedEventArgs e)
        {
            accepteInvit();
        }

        public void RefuseInvitationClick(object send, RoutedEventArgs e)
        {
            refuseInvit();
        }

        private async void leaveEvent()
        {
            string url = "http://api.caritathelp.me/guests/leave?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString()
                + "&event_id=" + id;
            Debug.WriteLine(url);
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(responseString);
                Debug.WriteLine(e.Message);
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

        public void leaveEventClick(object send, RoutedEventArgs e)
        {
            leaveEvent();
        }

        public void optionsEventClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Event.EventGestion), events.response);
        }

        public void memberClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Event.EventMember), id);
        }

        public void getInformation(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Event.EventInformation), id);
        }

        private async void getNotification()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string myId = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate("http://api.caritathelp.me/notifications{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                notifications = JsonConvert.DeserializeObject<NotificationResponse>(responseString);
                if (notifications.status != 200)
                {
                    Debug.WriteLine("Failed to get notification");
                }
                else
                {

                    Debug.WriteLine(responseString);
                    for (int x = 0; x < notifications.response.Count; x++)
                    {
                        if (notifications.response[x].notif_type.Equals("InviteGuest", StringComparison.Ordinal)
                             && id.Equals(notifications.response[x].event_id, StringComparison.Ordinal))
                            notif_id = notifications.response[x].id;
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

        private async void getPicture()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://api.caritathelp.me/events/" + id + "/main_picture{?token}");
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
                    if (picture.response != null)
                    {
                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource =
                            new BitmapImage(new Uri("http://52.31.151.160:3000" + picture.response.picture_path.thumb.url, UriKind.Absolute));
                        logo.Fill = myBrush;
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
                Debug.WriteLine(responseString);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private async void getInformation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://api.caritathelp.me/events/" + id + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                events = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if (events.status != 200)
                {

                }
                else
                {
                    titleText.Text = events.response.title;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"] = id.ToString();
                    if (events.response.rights.Equals("host", StringComparison.Ordinal)
                        || events.response.rights.Equals("admin", StringComparison.Ordinal))
                    {
                        OptionsButton.Visibility = Visibility.Visible;

                    }
                    else
                        OptionsButton.Visibility = Visibility.Collapsed;
                    if (events.response.rights.Equals("none", StringComparison.Ordinal))
                        joinEventButton.Visibility = Visibility.Visible;
                    else if (events.response.rights.Equals("invited", StringComparison.Ordinal))
                    {
                        accepteInvitation.Visibility = Visibility.Visible;
                        RefuseIntivation.Visibility = Visibility.Visible;
                        getNotification();
                    }
                    else if (events.response.rights.Equals("waiting", StringComparison.Ordinal))
                        informationBox.Text = "En attente de confirmation";
                    else
                        leaveEventButton.Visibility = Visibility.Visible;
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
                Debug.WriteLine(responseString);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public EventProfil()
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
            id = e.Parameter as string;
            Debug.WriteLine(id);
            getInformation();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }
    }
}
