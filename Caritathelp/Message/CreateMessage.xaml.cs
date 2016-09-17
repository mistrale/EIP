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
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.Message
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateMessage : Page
    {
        class MessageResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public MessageInfos response { get; set; }
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

        public CreateMessage()
        {
            this.InitializeComponent();
            informationBox.Text = "";
            volunteerGrid = new Grid();
            volunteerGrid.VerticalAlignment = VerticalAlignment.Top;
            volunteerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            volunteerGrid.Width = 380;
            scroll.Content = volunteerGrid;
            idVolunter = new List<int>();
        }

        private MessageResponse msgResponse;
        private List<int> idVolunter;
        private string responseString;
        private ListResponse userList;
        Grid volunteerGrid;

        private async void searchUsers()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/search{?research,token}");
                template.AddParameter("research", userBox.Text);
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
                    informationBox.Text = userList.message;
                }
                else
                {
                    if (userList.response.Count == 0 ||
                        !userList.response[0].result_type.Equals("volunteer", StringComparison.Ordinal))
                        informationBox.Text = "Utilisateur non trouvé";
                    else if (idVolunter.Count != 0 && idVolunter.Contains(userList.response[0].id)) {
                        informationBox.Text = "Utilisateur deja ajoute";
                    }
                    else
                    {
                        volunteerGrid.RowDefinitions.Add(new RowDefinition());

                        TextBlock name = new TextBlock();
                        name.Margin = new Thickness(10, 10, 10, 10);
                        name.FontSize = 14;
                        name.TextWrapping = TextWrapping.Wrap;
                        name.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                        name.Text = userList.response[0].name;
                        name.FontWeight = FontWeights.Bold;
                        name.VerticalAlignment = VerticalAlignment.Center;
                        name.HorizontalAlignment = HorizontalAlignment.Left;
                        Grid.SetColumn(name, 1);
                        Grid.SetRow(name, volunteerGrid.RowDefinitions.Count + 1);
                        volunteerGrid.RowDefinitions.Add(new RowDefinition());
                        volunteerGrid.Children.Add(name);
                        idVolunter.Add(userList.response[0].id);
                        userBox.Text = "Ajouter un utilisateur ...";
                    }
                }
            }
            catch (HttpRequestException e)
            {
                informationBox.Text = e.Message;
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

        public void addUserClick(object sender, RoutedEventArgs e)
        {
            if (!userBox.Text.Equals("Ajouter un utilisateur ...", StringComparison.Ordinal)
                && !userBox.Text.Equals("", StringComparison.Ordinal))
                searchUsers();
        }

        public void createRoomClick(object sender, RoutedEventArgs e)
        {
            if (idVolunter.Count != 0)
                createRoom();
        }

        private async void createRoom()
        {
            string title = titleBox.Text;
            if (title == null || title.Equals("", StringComparison.Ordinal)
                || title.Equals("Title", StringComparison.Ordinal))
                title = null;
            if (!idVolunter.Contains(Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"])))
            {
                idVolunter.Add(Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]));
            }
            string volunteers = "[";
            for (int i = 0; i < idVolunter.Count; i++)
            {
                if (i + 1 == idVolunter.Count)
                {
                    volunteers += idVolunter[i];
                } else
                {
                    volunteers += idVolunter[i] + ",";
                }
            }
            volunteers += "]";

            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("name", title),
                        new KeyValuePair<string, string>("volunteers[]", volunteers)
                    };
                string url = (Global.API_IRL + "/chatrooms");
                Debug.WriteLine("volontaires : " + volunteers);
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                msgResponse = JsonConvert.DeserializeObject<MessageResponse>(responseString);
                if (msgResponse.status != 200)
                {
                    informationBox.Text = msgResponse.message;
                }
                else
                {
                    ((Frame)Window.Current.Content).Navigate(typeof(Message));
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
                Debug.WriteLine("Fail in publish news friends");
            }
        }


        public void search_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Research));
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
