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
    public sealed partial class AssociationProfil : Page
    {
        private string id;

        class Member
        {
            public string id { get; set; } 
            public string mail { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string rights { get; set; }
        }

        class   AssociationRequest
        {
            public string status { get; set; }
            public string message { get; set; }
            public Association response { get; set; }
        }

        class MemberRequest
        {
            public string status { get; set; }
            public string message { get; set; }
            public IList<Member> response { get; set; }
        }

        class Event
        {
            public int id { get; set; }
            public string title { get; set; }
            public string place { get; set; }
        }

        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Event> response { get; set; }
        }

        private Grid membersGrid;
        private Grid eventsGrid;
        private AssociationRequest assoc;
        private MemberRequest members;
        private EventRequest events;

        public void joinAssociationClick(object send, RoutedEventArgs e)
        {

        }

        public void leaveAssociationClick(object send, RoutedEventArgs e)
        {

        }

        public void optionsAssociationClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GestionAssociation));
        }

        private async void getEvent()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/associations/" + id + "/events" + "{?token}");
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
                    eventsGrid.Height = events.response.Count * 100;
                    eventsGrid.Width = 375;
                    for (int i = 0; i < events.response.Count; ++i)
                        eventsGrid.RowDefinitions.Add(new RowDefinition());
                    eventsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    for (int x = 0; x < events.response.Count; ++x)
                    {
                        Button btn = new Button();
                        btn.Height = 100;
                        btn.Width = eventsGrid.Width;
                        btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btn.Click += new RoutedEventHandler(EventButtonClick);
                        btn.Content = events.response[x].title;
                        btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 124, 188, 99));
                        Grid.SetColumn(btn, 1);
                        Grid.SetRow(btn, x);
                        eventsGrid.Children.Add(btn);
                        eventsGrid.Visibility = Visibility.Visible;
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

        private void EventButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = events.response[x].id.ToString();
            Frame.Navigate(typeof(EventProfil), id);
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int x = Grid.GetRow(button);
            string id = members.response[x].id.ToString();
            Frame.Navigate(typeof(Profil), id);
            // identify which button was clicked and perform necessary actions
        }

        public void eventClick(object sender, RoutedEventArgs e)
        {
            scroll.Content = eventsGrid;
            membersGrid.Visibility = Visibility.Collapsed;
            eventsGrid.Visibility = Visibility.Visible;
        }

        public void memberClick(object sender, RoutedEventArgs e)
        {
            scroll.Content = membersGrid;
            membersGrid.Visibility = Visibility.Visible;
            eventsGrid.Visibility = Visibility.Collapsed;
        }

        private async void getInformation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/associations/" + id + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                assoc = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if (Int32.Parse(assoc.status) != 200)
                {

                }
                else
                {
                    titleText.Text = assoc.response.name;
                    
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"] = id.ToString();
                    if (assoc.response.rights.Equals("owner", StringComparison.Ordinal)
                        || assoc.response.rights.Equals("admin", StringComparison.Ordinal))
                    {
                        OptionsButton.Visibility = Visibility.Visible;
                        
                    }
                    else
                        OptionsButton.Visibility = Visibility.Collapsed;
                    if (assoc.response.rights.Equals("none", StringComparison.Ordinal))
                        joinAssociationButton.Visibility = Visibility.Visible;
                    else
                        leaveAssociationButton.Visibility = Visibility.Visible;
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

        public AssociationProfil()
        {
            this.InitializeComponent();
            membersGrid = new Grid();
            eventsGrid = new Grid();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
   //         loadCoroutine();
            id = e.Parameter as string;
            Debug.WriteLine(id);
     //       notifs = JsonConvert.DeserializeObject<Notifications>((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["notifications"]);
            getInformation();
            getMember();
            getEvent();
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
            Frame.Navigate(typeof(Research));
        }

        public void alertButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Notification));
        }

        bool isVisibile = false;
        private bool flag;
        bool doCoroutine = true;
        private Notifications notifs;

        class RequeteResponse
        {

            public string status { get; set; }
            public string message { get; set; }
            public User response { get; set; }

        }

        private RequeteResponse message;
        private string responseString;

        private async void getNotification()
        {
            if (doCoroutine)
            {
                var httpClient = new HttpClient(new HttpClientHandler());
                try
                {
                    string id = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                    var template = new UriTemplate("http://52.31.151.160:3000/volunteers/" + id + "{?token}");
                    template.AddParameter("id", id);
                    template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                    var uri = template.Resolve();
                    Debug.WriteLine(uri);

                    HttpResponseMessage response = await httpClient.GetAsync(uri);
                    response.EnsureSuccessStatusCode();
                    responseString = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine(responseString.ToString());
                    message = JsonConvert.DeserializeObject<RequeteResponse>(responseString);
                    if (Int32.Parse(message.status) != 200)
                    {

                    }
                    else
                    {

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
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                catch (NullReferenceException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private async Task updateGUI()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                if (flag)
                {
                    alertButtonNotity.Visibility = Visibility.Visible;
                    alertButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    alertButtonNotity.Visibility = Visibility.Collapsed;
                    alertButton.Visibility = Visibility;
                }
            });
        }

        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            getNotification();
            autoEvent.Set();
        }

        public void loadCoroutine()
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            // Create an inferred delegate that invokes methods for the timer.
            TimerCallback tcb = this.CheckStatus;

            // Create a timer that signals the delegate to invoke 
            // CheckStatus after one second, and every 1/4 second 
            // thereafter.
            Timer stateTimer = new Timer(tcb, autoEvent, 1000, 10000);

            // When autoEvent signals, change the period to every
            // 1/2 second.
            autoEvent.WaitOne(1000);

            // When autoEvent signals the second time, dispose of 
            // the timer.
            autoEvent.WaitOne(10000);
        }

    }
}
