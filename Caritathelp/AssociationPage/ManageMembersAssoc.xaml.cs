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

namespace Caritathelp.AssociationPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageMembersAssoc : Page
    {
        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        class ModelSearch
        {
            public int id { get; set; }
            public string thumb_path { get; set; }
            public string name { get; set; }
            public string rights { get; set; }
            public string result_type { get; set; }
        }

        class ListResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<ModelSearch> response { get; set; }
        }

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

        private All.Models.Association assoc;
        private string responseString;
        private Grid membersGrid;
        private MemberRequest members;
        private SimpleRequest simple;
        private ListResponse userList;


        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Convert.ToInt32(button.Tag.ToString());
            string id = members.response[x].id.ToString();
            Frame.Navigate(typeof(Volunteer.VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void UserKickClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Convert.ToInt32(button.Tag.ToString());
            string id = members.response[x].id.ToString();
            kickUser(id);
            // identify which button was clicked and perform necessary actions
        }
        private void selectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selected = (string)comboBox.SelectedItem;
            int x = Convert.ToInt32(comboBox.Tag.ToString());
            string id = members.response[x].id.ToString();
            Debug.WriteLine(selected + id);
            upgradeUser(id, selected);
        }

        private async void upgradeUser(string id_user, string rights)
        {
            string url = Global.API_IRL + "/membership/upgrade";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("assoc_id", "asa"),
                        new KeyValuePair<string, string>("rights", rights),
                        new KeyValuePair<string, string>("volunteer_id", id_user),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };

            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    warningTextBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(ManageMembersAssoc), assoc);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Debug.WriteLine("Fail in update profil");
            }
        }

        private async void kickUser(string id)
        {

            string url = Global.API_IRL + "/membership/kick?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString()
                         + "&assoc_id="  + "&volunteer_id=" + id;
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
                    warningTextBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(ManageMembersAssoc), assoc);
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

        private async void inviteUserInAssoc(String id)
        {
            string url = Global.API_IRL + "/membership/invite";
            var values = new List<KeyValuePair<string, string>>
                    {
                       // new KeyValuePair<string, string>("assoc_id", assoc.id.ToString()),
                        new KeyValuePair<string, string>("volunteer_id", id),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };

            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    warningTextBox.Text = simple.message;
                }
                else
                {
                    warningTextBox.Text = "Invitation envoyée";
                }
            }
            catch (HttpRequestException e)
            {
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

        private async void searchUsers()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/search{?research,token}");
                template.AddParameter("research", searchUser.Text);
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                userList = JsonConvert.DeserializeObject<ListResponse>(responseString);
                if (userList.status != 200)
                {
                    warningTextBox.Text = userList.message;
                }
                else
                {
                    if (userList.response.Count == 0)
                        warningTextBox.Text = "Utilisateur non trouvé";
                    else
                        inviteUserInAssoc(userList.response[0].id.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                warningTextBox.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        public void inviteUser(object sender, RoutedEventArgs e)
        {
            warningTextBox.Text = "";
            searchUsers();
        }

        public void goBackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GestionAssociation), assoc);
        }

        public void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        private async void getMember()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/associations/"+ "/members" + "{?token}");
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
                    membersGrid.VerticalAlignment = VerticalAlignment.Top;
                    membersGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    membersGrid.Height = members.response.Count * 145;
                    membersGrid.Width = 375;
                    for (int x = 0; x < members.response.Count; ++x)
                    {
                        membersGrid.RowDefinitions.Add(new RowDefinition());
                        Grid grid = new Grid();
                        grid.Margin = new Thickness(0, 10, 0, 10);
                        grid.Height = 145;
                        grid.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));

                        // row
                        var row = new RowDefinition();
                        row.Height = new GridLength(60);
                        grid.RowDefinitions.Add(row);

                        var row2 = new RowDefinition();
                        row2.Height = new GridLength(60);
                        grid.RowDefinitions.Add(row2);

                        //column
                        var colum = new ColumnDefinition();
                        colum.Width = new GridLength(100);
                        grid.ColumnDefinitions.Add(colum);

                        var column2 = new ColumnDefinition();
                        column2.Width = new GridLength(150);
                        grid.ColumnDefinitions.Add(column2);

                        var colum3 = new ColumnDefinition();
                        colum3.Width = new GridLength(125);
                        grid.ColumnDefinitions.Add(colum3);

                        // Username
                        Button title = new Button();
                        title.Tag = x;
                        title.Content = members.response[x].firstname + " " + members.response[x].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.Click += new RoutedEventHandler(UserButtonClick);
                        title.FontSize = 14;
                        title.Width = 140;
                        Grid.SetColumn(title, 1);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        //Ami en commun
                        TextBlock friend = new TextBlock();
                        friend.Tag = x;
                        friend.VerticalAlignment = VerticalAlignment.Center;
                        friend.Text = "2 amis en commun";
                        friend.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        friend.FontSize = 14;
                        Grid.SetColumn(friend, 1);
                        Grid.SetRow(friend, 1);
                        grid.Children.Add(friend);

                        // image
                        Image btn = new Image();
                        btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
                        Grid.SetColumn(btn, 0);
                        Grid.SetRow(btn, 0);
                        Grid.SetRowSpan(btn, 2);
                        grid.Children.Add(btn);

                        // kick member
                        Button kick = new Button();
                        kick.Content = "Supprimer";
                        kick.Tag = x;
                        kick.HorizontalAlignment = HorizontalAlignment.Center;
                        kick.Width = 115;
                        kick.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        kick.Click += new RoutedEventHandler(UserKickClick);
                        kick.FontSize = 14;
                        Grid.SetColumn(kick, 2);
                        Grid.SetRow(kick, 1);
                        grid.Children.Add(kick);

                        //
                        ScrollViewer view = new ScrollViewer();
                        ComboBox rights = new ComboBox();
                        rights.Tag = x;
                        rights.Width = 115;
                        rights.HorizontalAlignment = HorizontalAlignment.Center;
                        rights.BorderThickness = new Thickness(0);
                        rights.Items.Add("owner");
                        rights.Items.Add("admin");
                        rights.Items.Add("member");
                        rights.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));
                        rights.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        if (members.response[x].rights.Equals("owner", StringComparison.Ordinal))
                            rights.SelectedIndex = 0;
                        else if (members.response[x].rights.Equals("admin", StringComparison.Ordinal))
                            rights.SelectedIndex = 1;
                        else
                            rights.SelectedIndex = 2;
                        rights.SelectionChanged += new SelectionChangedEventHandler(selectedIndexChanged);
                        view.Content = rights;
                        Grid.SetColumn(view, 2);
                        Grid.SetRow(view, 0);
                        grid.Children.Add(view);

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

        public ManageMembersAssoc()
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
            assoc = e.Parameter as All.Models.Association;
            warningTextBox.Text = "";
            getMember();
        }
    }
}
