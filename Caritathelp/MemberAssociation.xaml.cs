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
            int x = Grid.GetRow(button);
            string id = members.response[x].id.ToString();
            Frame.Navigate(typeof(Profil), id);
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
                    membersGrid.Height = members.response.Count * 100;
                    membersGrid.Width = 375;
                    for (int i = 0; i < members.response.Count; ++i)
                        membersGrid.RowDefinitions.Add(new RowDefinition());
                    membersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    for (int x = 0; x < members.response.Count; ++x)
                    {
                        Button btn = new Button();
                        btn.Height = 100;
                        btn.Width = membersGrid.Width;
                        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btn.Click += new RoutedEventHandler(UserButtonClick);
                        btn.Content = members.response[x].firstname + " " + members.response[x].lastname;
                        btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 124, 188, 99));
                        Grid.SetColumn(btn, 1);
                        Grid.SetRow(btn, x);
                        membersGrid.Children.Add(btn);
                        membersGrid.Visibility = Visibility.Visible;
                        scroll.Content = membersGrid;
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

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            searchBox.Foreground = new SolidColorBrush(Colors.Black);
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
