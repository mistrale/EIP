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

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventCreation : Page
    {
        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public EventModel response { get; set; }
        }

        private string responseString;
        private EventRequest eventresponse;

        public EventCreation()
        {
            this.InitializeComponent();
        }

        public async void createEvent()
        {
            String[] data = beginDate.Date.ToString().Split(' ')[0].Split('/');
            string date = data[2] + '-' + data[0] + '-' + data[1] + 'T' + timeBegin.Time.ToString() + 'Z';
            String[] data1 = endDate.Date.ToString().Split(' ')[0].Split('/');
            string date1 = data1[2] + '-' + data1[0] + '-' + data1[1] + 'T' + timeEnd.Time.ToString() + 'Z';
            string url = "http://52.31.151.160:3000/events/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("assoc_id", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"]),
                        new KeyValuePair<string, string>("title", eventTitleText.Text),
                        new KeyValuePair<string, string>("description", eventDescriptionText.Text),
                        new KeyValuePair<string, string>("place", eventPlaceText.Text),
                        new KeyValuePair<string, string>("begin", date),
                        new KeyValuePair<string, string>("end", date1),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                eventresponse = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if (eventresponse.status != 200)
                {
                    warningTextBox.Text = eventresponse.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), (string)(eventresponse.response.id.ToString()));
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
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public bool checkCreationEvent()
        {
            if (eventTitleText.Text == String.Empty || eventTitleText.Text.Equals("Titre de l'évènement", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Titre vide.";
                return false;
            }
            if (eventDescriptionText.Text == String.Empty || eventDescriptionText.Text.Equals("Description", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Description vide.";
                return false;
            }
            return true;
        }

        public void createEventClick(object send, RoutedEventArgs e)
        {
            if (!checkCreationEvent())
                return;
            warningTextBox.Text = "";
            createEvent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            warningTextBox.Text = "";
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }

    }
}
