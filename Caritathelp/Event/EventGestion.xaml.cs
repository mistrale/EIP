using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Caritathelp.Event
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventGestion : Page
    {
        private EventModel events;

        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        private string responseString;
        private SimpleRequest simple;

        public void manageMemberClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Event.EventManagerMember), events);
        }

        public void notificationClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Event.EventNotification), events);
        }

        private async void deleteEvents()
        {
            string url = Global.API_IRL + "/events/" + events.id.ToString() + "?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString();
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
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(AssociationProfil), events.assoc_id.ToString());
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

        public void deleteEventClick(object send, RoutedEventArgs e)
        {
            deleteEvents();
        }

        public void newPublicationClick(object send, RoutedEventArgs e)
        {

        }

        public EventGestion()
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
            events = e.Parameter as EventModel;
            if (events.rights.Equals("host", StringComparison.Ordinal))
                deleteEvent.Visibility = Visibility.Visible;
            else
                deleteEvent.Visibility = Visibility.Collapsed;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }
    }
}
