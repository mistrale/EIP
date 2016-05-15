using Caritathelp.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Research : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();


        class UserListResponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public IList<User> response { get; set; }
        }

        class AssociationListResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Association> response { get; set; }
        }

        class EventListResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<EventModel> response { get; set; }
        }

        private EventListResponse events;
        private UserListResponse userList;
        private AssociationListResponse associationList;
        private string responseString;

        private async void searchAssociation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/associations{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                associationList = JsonConvert.DeserializeObject<AssociationListResponse>(responseString);
                if (associationList.status != 200)
                {
                    warningTextBox.Text = associationList.message;
                }
                else
                {
                    resultatText.Text = "Résultat pour : ";
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
            if (associationList != null)
                initResearchAssociation(associationList.response.Count);
            else
                initResearchAssociation(0);
        }

        private async void searchEvent()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/events{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                events = JsonConvert.DeserializeObject<EventListResponse>(responseString);
                if (events.status != 200)
                {
                    warningTextBox.Text = events.message;
                }
                else
                {
                    resultatText.Text = "Résultat pour : ";
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
            if (events != null)
                initResearchEvent(events.response.Count);
            else
                initResearchEvent(0);
        }

        private void initResearchEvent(int nbRows)
        {
            Grid grid = new Grid();
            grid.Height = nbRows * 100;
            Debug.WriteLine(nbRows);
            grid.Width = 375;
            for (int i = 0; i < nbRows; ++i)
                grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int x = 0; x < events.response.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = grid.Width;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.Click += new RoutedEventHandler(EventButtonClick);
                btn.Content = events.response[x].title;
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
            scroll.Content = grid;
        }

        private void initResearchAssociation(int nbRows)
        {
            Grid grid = new Grid();
            grid.Height = nbRows * 100;
            Debug.WriteLine(nbRows);
            grid.Width = 375;
            for (int i = 0; i < nbRows; ++i)
                grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int x = 0; x < associationList.response.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = grid.Width;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.Click += new RoutedEventHandler(AssociationButtonClick);
                btn.Content = associationList.response[x].name;
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
            scroll.Content = grid;
        }

        private void EventButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = events.response[x].id.ToString();
            Frame.Navigate(typeof(EventProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private async void searchUser()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string search = searchBox.Text;
                var template = new UriTemplate("http://52.31.151.160:3000/volunteers/search{?research,token}");
                template.AddParameter("research", search);
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                userList = JsonConvert.DeserializeObject<UserListResponse>(responseString);
                if (Int32.Parse(userList.status) != 200)
                {
                    warningTextBox.Text = userList.message;
                }
                else
                {
                    resultatText.Text = "Résultat pour : " + searchBox.Text;
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
            if (userList != null)
                initResearchUser(userList.response.Count);
            else
                initResearchUser(0);
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = userList.response[x].id.ToString();
            Frame.Navigate(typeof(Volunteer.VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void AssociationButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = associationList.response[x].id.ToString();
            Frame.Navigate(typeof(AssociationProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void initResearchUser(int nbRows)
        {
            Grid grid = new Grid();
            grid.Height = nbRows * 100;
            grid.Width = 375;
            for (int i = 0; i < nbRows; ++i)
                grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int x = 0; x < userList.response.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = grid.Width;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                btn.Click += new RoutedEventHandler(UserButtonClick);
                btn.Content = userList.response[x].firstname + " " + userList.response[x].lastname;
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
            scroll.Content = grid;
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 1)
                searchUser();
            else if (comboBox.SelectedIndex == 2)
                searchAssociation();
            else if (comboBox.SelectedIndex == 3)
                searchEvent();
        }

        public Research()
        {
            this.InitializeComponent();
            resultatText.Text = "";
        }

    }
}
