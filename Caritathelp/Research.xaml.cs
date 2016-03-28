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

namespace Caritathelp
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

        private UserListResponse userList;
        private AssociationListResponse associationList;

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
                    warningTextBox.Text = " ";
                    warningTextBox.Foreground = new SolidColorBrush(Colors.Black);
                    warningTextBox.Background = new SolidColorBrush(Colors.Transparent);
                    warningTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    warningTextBox.IsHitTestVisible = false;
                    warningTextBox.BorderBrush = new SolidColorBrush(Colors.Transparent);
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

        private void initResearchAssociation(int nbRows)
        {
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
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 124, 188, 99));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
        }

        private async void searchUser()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string search = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["search"];
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
                } else
                {
                    warningTextBox.Text = search;
                    warningTextBox.Foreground = new SolidColorBrush(Colors.Black);
                    warningTextBox.Background = new SolidColorBrush(Colors.Transparent);
                    warningTextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    warningTextBox.IsHitTestVisible = false;
                    warningTextBox.BorderBrush = new SolidColorBrush(Colors.Transparent);
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
            Frame.Navigate(typeof(Profil), id);
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

        private  void initResearchUser(int nbRows)
        {
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
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 124, 188, 99));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                grid.Children.Add(btn);
            }
        }

        public Research()
        {
            this.InitializeComponent();
            searchBox.Items.Add("Volontaire");
            searchBox.Items.Add("Association");
            searchBox.Items.Add("Event");
            searchBox.SelectedIndex = 0;
            string type = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["typeSearch"];
            if (type.Equals("Volontaire", StringComparison.Ordinal))
                searchUser();
            else if (type.Equals("Association", StringComparison.Ordinal))
                searchAssociation();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("search");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("typeSearch");
        }
        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
       //     loadCoroutine();
        //    notifs = JsonConvert.DeserializeObject<Notifications>((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["notifications"]);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

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
                Debug.WriteLine(message);
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
