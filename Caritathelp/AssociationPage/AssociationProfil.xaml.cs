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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AssociationProfil : Page
    {
        private string id;
        private string notif_id;

        class NotificationResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<GlobalNotification> response { get; set; }
        }

        private NotificationResponse notifications;

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest picture;

        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set;  }
            public string response { get; set; }
        }

        class   AssociationRequest
        {
            public string status { get; set; }
            public string message { get; set; }
            public Association response { get; set; }
        }

        private AssociationRequest assoc;

        private SimpleRequest simple;
        private String responseString;

        private async void joinAssociationRequest()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("assoc_id", id)

                    };
                string url = ("http://api.caritathelp.me/membership/join");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();

                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(AssociationProfil), id);
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

        public void joinAssociationClick(object send, RoutedEventArgs e)
        {
            joinAssociationRequest();
        }

        public void followAssociationClick(object send, RoutedEventArgs e)
        {
            joinAssociationRequest();
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
                string url = ("http://api.caritathelp.me/membership/reply_invite");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(AssociationProfil), id);
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
                string url = ("http://api.caritathelp.me/membership/reply_invite");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(AssociationProfil), id);
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

        private async void leaveAssociation()
        {
            string url = "http://api.caritathelp.me/membership/leave?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString()
                + "&assoc_id=" + id;
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
                    Frame.Navigate(typeof(AssociationProfil), id);
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

        public void leaveAssociationClick(object send, RoutedEventArgs e)
        {
            leaveAssociation();
        }

        public void optionsAssociationClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GestionAssociation), assoc.response);
        }

        public void eventClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventAssociation), id);
        }

        public void memberClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MemberAssociation), id);
        }

        public void getInformation(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InformationAssociation), id);
        }

        private async void getNotification()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string myId = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate("http://api.caritathelp.me/volunteers/" + myId + "/notifications" + "{?token}");
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
                    for (int x = 0; x < notifications.response.Count; x++)
                    {
                        if (notifications.response[x].notif_type.Equals("InviteMember", StringComparison.Ordinal) 
                            && id.Equals(notifications.response[x].assoc_id, StringComparison.Ordinal))
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
                var template = new UriTemplate("http://api.caritathelp.me/associations/" + id + "/main_picture{?token}");
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
                            new BitmapImage(new Uri("http://api.caritathelp.me" + picture.response.picture_path.thumb.url, UriKind.Absolute));
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
                var template = new UriTemplate("http://api.caritathelp.me/associations/" + id + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                assoc = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if (Int32.Parse(assoc.status) != 200)
                {

                }
                else
                {
                    titleText.Text = assoc.response.name;
                    
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"] = id.ToString();
                    if (assoc.response.rights.Equals("owner", StringComparison.Ordinal)
                        || assoc.response.rights.Equals("admin", StringComparison.Ordinal))
                    {
                        OptionsButton.Visibility = Visibility.Visible;
                        
                    }
                    else
                        OptionsButton.Visibility = Visibility.Collapsed;
                    if (assoc.response.rights.Equals("none", StringComparison.Ordinal))
                        joinAssociationButton.Visibility = Visibility.Visible;
                    else if (assoc.response.rights.Equals("invited", StringComparison.Ordinal))
                    {
                        accepteInvitation.Visibility = Visibility.Visible;
                        RefuseIntivation.Visibility = Visibility.Visible;
                        getNotification();
                    }
                    else if (assoc.response.rights.Equals("waiting", StringComparison.Ordinal))
                        informationBox.Text = "En attente de confirmation";
                    else
                        leaveAssociationButton.Visibility = Visibility.Visible;
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

        public AssociationProfil()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            informationBox.Text = "";
            getInformation();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }

    }
}
