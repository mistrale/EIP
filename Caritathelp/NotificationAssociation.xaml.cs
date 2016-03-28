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
    public sealed partial class NotificationAssociation : Page
    {
        private string id;
        private string responseString;
        private NotificationRequest assocNotif;
        private Grid mainGrid;

        class NotificationRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public NotificationAssociationRequest response { get; set; }
        }

        public NotificationAssociation()
        {
            this.InitializeComponent();
            searchBox.Items.Add("Volontaire");
            searchBox.Items.Add("Association");
            searchBox.Items.Add("Event");
            searchBox.SelectedIndex = 0;
        }

        public async void getNoficationMemberShip()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/associations/" + id + "/notifications{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);
                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                assocNotif = JsonConvert.DeserializeObject<NotificationRequest>(responseString);
                if (assocNotif.status != 200)
                {
                }
                else
                {
                    mainGrid = new Grid();

                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    for (int i = 0; i < assocNotif.response.member_request.Count; i++)
                    {
                        mainGrid.RowDefinitions.Add(new RowDefinition());
                        Grid grid = new Grid();

                        var row = new RowDefinition();
                        row.Height = new GridLength(30);
                        grid.RowDefinitions.Add(row);

                        var row2 = new RowDefinition();
                        row2.Height = new GridLength(30);
                        grid.RowDefinitions.Add(row2);

                        var colum = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(colum);

                        var column2 = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(column2);

                        TextBlock title = new TextBlock();
                        title.Text = assocNotif.response.member_request[i].firstname + " " + assocNotif.response.member_request[i].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));

                        Button btnYes = new Button();
                        btnYes.Height = 100;
                        btnYes.Width = grid.Width;
                        btnYes.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btnYes.Tag = i;
                        btnYes.Click += new RoutedEventHandler(addUserToAssoc);
                        btnYes.Content = "Ajouter";
                        btnYes.Background = new SolidColorBrush(Color.FromArgb(0xFF, 182, 215, 168));

                        Button btnNo = new Button();
                        btnYes.Height = 100;
                        btnYes.Width = grid.Width;
                        btnYes.HorizontalAlignment = HorizontalAlignment.Stretch;
                        btnYes.Tag = i;
                        btnYes.Click += new RoutedEventHandler(refuseUserToAssoc);
                        btnYes.Content = "Refuser";
                        btnYes.Background = new SolidColorBrush(Color.FromArgb(0xFF, 182, 215, 168));

                        Grid.SetColumn(title, 1);
                        Grid.SetColumnSpan(title, 2);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        Grid.SetColumn(btnYes, 1);
                        Grid.SetRow(btnYes, 1);
                        grid.Children.Add(btnYes);

                        Grid.SetColumn(btnNo, 2);
                        Grid.SetRow(btnNo, 1);
                        grid.Children.Add(btnNo);

                        mainGrid.Children.Add(grid);
                    }
                    scollView.Content = mainGrid;
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

        private void addUserToAssoc(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
         
            string id = button.Tag.ToString();
            Frame.Navigate(typeof(AssociationProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        private void refuseUserToAssoc(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            string id = button.Tag.ToString();
            Frame.Navigate(typeof(AssociationProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            getNoficationMemberShip();
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
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["typeSearch"] = searchBox.SelectedItem.ToString();
            Frame.Navigate(typeof(Research));
        }

        public void alertButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Notification));
        }
    }
}
