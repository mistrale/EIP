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
    public sealed partial class EventAssociation : Page
    {
        class Event
        {
            public int id { get; set; }
            public string title { get; set; }
            public string place { get; set; }
            public string begin { get; set; }
            public int assoc_id { get; set; }
            public string rights { get; set; }
            public int nb_friends_members { get; set; }
        }

        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Event> response { get; set; }
        }

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest pictures;
        private Grid eventsGrid;
        private EventRequest events;
        private string responseString;
        private string id;

        public EventAssociation()
        {
            this.InitializeComponent();
        }

        private void EventButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Convert.ToInt32(button.Tag.ToString());
            string id = events.response[x].id.ToString();
            Frame.Navigate(typeof(EventProfil), id);
        }

        private async void getPicture(int id, Image btn)
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/events/" + id.ToString() + "/main_picture{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                pictures = JsonConvert.DeserializeObject<PictureRequest>(responseString);
                if (pictures.status != 200)
                {

                }
                else
                {
                    if (pictures.response != null)
                    {
                        btn.Source = new BitmapImage(new Uri("http://52.31.151.160:3000" + pictures.response.picture_path.thumb.url, UriKind.Absolute));
                    }
                    else
                        btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
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

        private async void getEvenement()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/associations/" + id + "/events" + "{?token}");
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
                    Debug.WriteLine(events.response.Count);
                    eventsGrid = new Grid();
                    eventsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    eventsGrid.VerticalAlignment = VerticalAlignment.Top;
                    eventsGrid.Height = events.response.Count * 100;
                    eventsGrid.Width = 375;
                    for (int x = 0; x < events.response.Count; ++x)
                    {
                        eventsGrid.RowDefinitions.Add(new RowDefinition());
                        Grid grid = new Grid();
                        grid.Margin = new Thickness(0, 10, 0, 10);
                        grid.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));

                        // row
                        var row = new RowDefinition();
                        row.Height = new GridLength(30);
                        grid.RowDefinitions.Add(row);

                        var row2 = new RowDefinition();
                        grid.RowDefinitions.Add(row2);

                        var row3 = new RowDefinition();
                        grid.RowDefinitions.Add(row3);

                        //column
                        var colum = new ColumnDefinition();
                        colum.Width = new GridLength(100);
                        grid.ColumnDefinitions.Add(colum);

                        var column2 = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(column2);


                        // title
                        Button title = new Button();
                        title.Content = events.response[x].title;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.BorderThickness = new Thickness(0);
                        title.Tag = x;
                        title.Click += new RoutedEventHandler(EventButtonClick);
                        title.FontSize = 18;
                        Grid.SetColumn(title, 1);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        // date
                        TextBlock rights = new TextBlock();
                        rights.Text = events.response[x].begin;
                        rights.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        rights.FontSize = 14;
                        rights.Margin = new Thickness(10, 0, 0, 0);
                        Grid.SetColumn(rights, 1);
                        Grid.SetRow(rights, 1);
                        grid.Children.Add(rights);

                        // nb ami
                        TextBlock friends = new TextBlock();
                        friends.Text = "Amis membres " + events.response[x].nb_friends_members.ToString();
                        friends.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        friends.FontSize = 14;
                        friends.Margin = new Thickness(10, 0, 0, 0);
                        Grid.SetColumn(friends, 1);
                        Grid.SetRow(friends, 2);
                        grid.Children.Add(friends);


                        Image btn = new Image();
                        getPicture(events.response[x].id, btn);
                        btn.Stretch = Stretch.Fill;
                        btn.Width = 100;
                        btn.Height = 100;
                        // image
                        Grid.SetColumn(btn, 0);
                        Grid.SetRow(btn, 0);
                        Grid.SetRowSpan(btn, 3);
                        grid.Children.Add(btn);


                        eventsGrid.Children.Add(grid);
                        Grid.SetColumn(grid, 0);
                        Grid.SetRow(grid, x);
                    }
                    eventsGrid.Visibility = Visibility.Visible;
                    scroll.Content = eventsGrid;
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

        public void goBackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AssociationProfil), id);
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            getEvenement();
        }
    }
}
