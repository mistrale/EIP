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

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventProfil : Page
    {
        private string id;

        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public Event response { get; set; }
        }

        private string responseString;
        private EventRequest eventresponse;

        private async void getInformation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/events/" + id + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                eventresponse = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if (eventresponse.status != 200)
                {

                }
                else
                {
                    eventTitleText.Text = eventresponse.response.title;
                    eventDescriptionText.Text = eventresponse.response.description;
                    eventPlaceText.Text = eventresponse.response.place;
                    if (eventresponse.response.begin != null)
                        beginDate.Date = (DateTime)Convert.ToDateTime(eventresponse.response.begin);
                    if (eventresponse.response.end != null)
                        endDate.Date = (DateTime)Convert.ToDateTime(eventresponse.response.end);
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
            searchBox.Items.Add("Volontaire");
            searchBox.Items.Add("Association");
            searchBox.Items.Add("Event");
            searchBox.SelectedIndex = 0;
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

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            searchTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void associationButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void messageButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void moreButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Options));
        }

        public void homeButtonClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Accueil));
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["search"] = searchTextBox.Text;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["typeSearch"] = searchBox.SelectedItem.ToString();
            Frame.Navigate(typeof(Research));
        }

        public void alertButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Notification));
        }
    }
}
