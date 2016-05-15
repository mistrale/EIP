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

namespace Caritathelp.Event
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventInformation : Page
    {
        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public EventModel response { get; set; }
        }

        private EventRequest events;
        private string id;
        private string responseString;

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void goBackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EventProfil), id);
        }

        public EventInformation()
        {
            this.InitializeComponent();
        }

        private async void updateEvent()
        {
            String[] data = beginDate.Date.ToString().Split(' ')[0].Split('/');
            string date = data[2] + '-' + data[0] + '-' + data[1] + 'T' + timeBegin.Time.ToString() + 'Z';
            String[] data1 = endDate.Date.ToString().Split(' ')[0].Split('/');
            string date1 = data1[2] + '-' + data1[0] + '-' + data1[1] + 'T' + timeEnd.Time.ToString() + 'Z';
            string url = "http://52.31.151.160:3000/events/" + id;
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("title", eventTitleText.Text),
                        new KeyValuePair<string, string>("description", eventDescriptionText.Text),
                        new KeyValuePair<string, string>("begin", date),
                        new KeyValuePair<string, string>("end", date1),
                        new KeyValuePair<string, string>("place", eventPlaceText.Text),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                events = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if (events.status != 200)
                {
                    warning.Text = events.message;
                }
                else
                {
                    warning.Text = "Evènement modifié !";
                }
            }
            catch (HttpRequestException e)
            {
                warning.Text = e.Message;
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

        public void updateEventClick(object sender, RoutedEventArgs e)
        {
            updateEvent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        /// 
        private async void getInformations()
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
                events = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if (events.status != 200)
                {

                }
                else
                {
                    eventTitleText.Text = events.response.title;
                    eventDescriptionText.Text = events.response.description;
                    eventPlaceText.Text = events.response.place;
                    if (events.response.begin != null)
                    {
                        string[] data = events.response.begin.Split('T');
                        string[] begin = data[0].Split('-');
                        beginDate.Date = (DateTime)Convert.ToDateTime(begin[1] + '/' + begin[2] + '/' + begin[0]);
                        string[] hour = data[1].Split('.');
                        string[] tmp = hour[1].Split('+');
                        TimeSpan faketime = TimeSpan.Parse(hour[0]);
                        TimeSpan decallage = TimeSpan.Parse(tmp[1]);
                        if (faketime < decallage)
                        {
                            TimeSpan realTime = new TimeSpan(24, faketime.Minutes, faketime.Seconds);
                            timeBegin.Time = realTime - decallage;

                        } else
                        {
                            timeBegin.Time = TimeSpan.Parse(hour[0]) - TimeSpan.Parse(tmp[1]);
                        }
                    }
                    if (events.response.end != null)
                    {
                        string[] data = events.response.end.Split('T');
                        string[] end = data[0].Split('-');
                        endDate.Date = (DateTime)Convert.ToDateTime(end[1] + '/' + end[2] + '/' + end[0]);
                        string[] hour = data[1].Split('.');
                        string[] tmp = hour[1].Split('+');
                        TimeSpan faketime = TimeSpan.Parse(hour[0]);
                        TimeSpan decallage = TimeSpan.Parse(tmp[1]);
                        if (faketime < decallage)
                        {
                            TimeSpan realTime = new TimeSpan(24, faketime.Minutes, faketime.Seconds);
                            timeEnd.Time = realTime - decallage;

                        }
                        else
                        {
                            timeEnd.Time = TimeSpan.Parse(hour[0]) - TimeSpan.Parse(tmp[1]);
                        }
                    }
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"] = id.ToString();
                    if (events.response.rights.Equals("host", StringComparison.Ordinal)
                        || events.response.rights.Equals("admin", StringComparison.Ordinal))
                    {
                        eventTitleText.IsReadOnly = false;
                        eventDescriptionText.IsReadOnly = false;
                        eventPlaceText.IsReadOnly = false;
                        updateButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        eventTitleText.IsReadOnly = true;
                        eventDescriptionText.IsReadOnly = true;
                        eventPlaceText.IsReadOnly = true;
                        updateButton.Visibility = Visibility.Collapsed;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            warning.Text = "";
            getInformations();
        }
    }
}
