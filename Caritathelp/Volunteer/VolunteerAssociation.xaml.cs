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
    public sealed partial class VolunteerAssociation : Page
    {
        private string id;

        class AssociationRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<All.Models.Association> response { get; set; }
        }

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest pictures;
        private Grid assocGrid;
        private AssociationRequest assoc;
        private string responseString;

        private async void getPicture(string id, Image btn)
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/associations/" + id + "/main_picture{?token}");
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

        public void goBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VolunteerProfil), id);
        }

        public VolunteerAssociation()
        {
            this.InitializeComponent();
        }

        private void createAssociationClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateAssociation));
        }

        private void AssocButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Convert.ToInt32(button.Tag.ToString());
            string id = "5";
            Frame.Navigate(typeof(AssociationProfil), id);
        }

        private async void getAssociation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/volunteers/" + id + "/associations" + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                assoc = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if (assoc.status != 200)
                {

                }
                else
                {
                    Debug.WriteLine(assoc.response.Count);
                    assocGrid = new Grid();
                    assocGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    assocGrid.VerticalAlignment = VerticalAlignment.Top;
                    assocGrid.Height = assoc.response.Count * 100;
                    assocGrid.Width = 375;
                    for (int x = 0; x < assoc.response.Count; ++x)
                    {
                        assocGrid.RowDefinitions.Add(new RowDefinition());
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
                       // title.Content = assoc.response[x].name;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.BorderThickness = new Thickness(0);
                        title.Tag = x;
                        title.Click += new RoutedEventHandler(AssocButtonClick);
                        title.FontSize = 18;
                        Grid.SetColumn(title, 1);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        // date
                        TextBlock rights = new TextBlock();
                        //rights.Text = assoc.response[x].rights;
                        rights.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        rights.FontSize = 14;
                        rights.Margin = new Thickness(10, 0, 0, 0);
                        Grid.SetColumn(rights, 1);
                        Grid.SetRow(rights, 1);
                        grid.Children.Add(rights);

                        // nb ami
                        TextBlock friends = new TextBlock();
                       // friends.Text = "Amis membres " + assoc.response[x].nb_members.ToString();
                        friends.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        friends.FontSize = 14;
                        friends.Margin = new Thickness(10, 0, 0, 0);
                        Grid.SetColumn(friends, 1);
                        Grid.SetRow(friends, 2);
                        grid.Children.Add(friends);

                        // image
                        Image btn = new Image();
                        //getPicture(assoc.response[x].id.ToString(), btn);
                        btn.Stretch = Stretch.Fill;
                        btn.Height = 100;
                        btn.Width = 100;
                        Grid.SetColumn(btn, 0);
                        Grid.SetRow(btn, 0);
                        Grid.SetRowSpan(btn, 3);
                        grid.Children.Add(btn);


                        assocGrid.Children.Add(grid);
                        Grid.SetColumn(grid, 0);
                        Grid.SetRow(grid, x);
                    }
                    assocGrid.Visibility = Visibility.Visible;
                    scroll.Content = assocGrid;
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            if (!id.Equals((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"], StringComparison.Ordinal))
                button1.Visibility = Visibility.Collapsed;
            getAssociation();
        }
    }
}
