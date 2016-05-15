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
    public sealed partial class MemberAssociation : Page
    {
        class Member
        {
            public string id { get; set; }
            public string mail { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string rights { get; set; }
        }

        class MemberRequest
        {
            public string status { get; set; }
            public string message { get; set; }
            public IList<Member> response { get; set; }
        }

        private string id;
        private string responseString;
        private Grid membersGrid;
        private MemberRequest members;

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag.ToString());
            string id = members.response[i].id.ToString();
            Frame.Navigate(typeof(Volunteer.VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        public void goBackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AssociationProfil), id);
        }

        public MemberAssociation()
        {
            this.InitializeComponent();
        }

        private async void getMember()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/associations/" + id + "/members" + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                members = JsonConvert.DeserializeObject<MemberRequest>(responseString);
                if (Int32.Parse(members.status) != 200)
                {

                }
                else
                {
                    Debug.WriteLine(members.response.Count);
                    membersGrid = new Grid();
                    membersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    membersGrid.VerticalAlignment = VerticalAlignment.Top;
                    membersGrid.Height = members.response.Count * 100;
                    membersGrid.Width = 375;
                    for (int x = 0; x < members.response.Count; ++x)
                    {
                        membersGrid.RowDefinitions.Add(new RowDefinition());
                        Grid grid = new Grid();
                        grid.Margin = new Thickness(0, 10, 0, 10);
                        grid.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));

                        // row
                        var row = new RowDefinition();
                        row.Height = new GridLength(50);
                        grid.RowDefinitions.Add(row);

                        var row2 = new RowDefinition();
                        row2.Height = new GridLength(40);
                        grid.RowDefinitions.Add(row2);

                        //column
                        var colum = new ColumnDefinition();
                        colum.Width = new GridLength(80);
                        grid.ColumnDefinitions.Add(colum);

                        var column2 = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(column2);


                        // Username
                        Button title = new Button();
                        title.Tag = x;
                        title.Content = members.response[x].firstname + " " + members.response[x].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.Click += new RoutedEventHandler(UserButtonClick);
                        title.FontSize = 14;
                        title.Height = 50;
                        title.Width = 200;
                        title.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetColumn(title, 1);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        //Ami en commun
                        TextBlock friend = new TextBlock();
                        friend.Text = "2 amis en commun";
                        friend.VerticalAlignment = VerticalAlignment.Top;
                        friend.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        friend.FontSize = 14;
                        friend.Height = 30;
                        friend.HorizontalAlignment = HorizontalAlignment.Center;
                        Grid.SetColumn(friend, 1);
                        Grid.SetRow(friend, 1);
                        grid.Children.Add(friend);

                        Image btn = new Image();
                        btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
                        btn.Height = 80;
                        btn.Width = 80;
                        Grid.SetColumn(btn, 0);
                        Grid.SetRow(btn, 0);
                        Grid.SetRowSpan(btn, 2);
                        btn.HorizontalAlignment = HorizontalAlignment.Center;
                        grid.Children.Add(btn);

                        membersGrid.Children.Add(grid);
                        Grid.SetColumn(grid, 0);
                        Grid.SetRow(grid, x);
                    }
                    membersGrid.Visibility = Visibility.Visible;
                    scroll.Content = membersGrid;
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
            getMember();
        }
    }
}
