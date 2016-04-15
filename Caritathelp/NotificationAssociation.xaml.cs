﻿using Newtonsoft.Json;
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
        private SimpleRequest simpleResponse;

        class NotificationRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public NotificationAssociationRequest response { get; set; }
        }

        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
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

                        // name
                        var row = new RowDefinition();
                        row.Height = new GridLength(30);
                        grid.RowDefinitions.Add(row);

                        // accept/refuse
                        var row2 = new RowDefinition();
                        row2.Height = new GridLength(30);
                        grid.RowDefinitions.Add(row2);

                        // accept
                        var colum = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(colum);

                        // refuse
                        var column2 = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(column2);

                        TextBlock title = new TextBlock();
                        title.Text = assocNotif.response.member_request[i].firstname + " " + assocNotif.response.member_request[i].lastname;
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));

                        Button btnYes = new Button();
                        btnYes.Tag = i;
                        btnYes.Click += new RoutedEventHandler(addUserToAssoc);
                        btnYes.Content = "Ajouter";
                        btnYes.Background = new SolidColorBrush(Color.FromArgb(0xFF, 182, 215, 168));

                        Button btnNo = new Button();
                        btnNo.Tag = i;
                        btnNo.Click += new RoutedEventHandler(refuseUserToAssoc);
                        btnNo.Content = "Refuser";
                        btnNo.Background = new SolidColorBrush(Color.FromArgb(0xFF, 182, 215, 168));

                        Grid.SetColumn(title, 0);
                        Grid.SetColumnSpan(title, 2);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        Grid.SetColumn(btnYes, 0);
                        Grid.SetRow(btnYes, 1);
                        grid.Children.Add(btnYes);

                        Grid.SetColumn(btnNo, 1);
                        Grid.SetRow(btnNo, 1);
                        grid.Children.Add(btnNo);

                        mainGrid.Children.Add(grid);
                        Grid.SetColumn(grid, 0);
                        Grid.SetRow(grid, i);
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

        private async void addUserToAssoc(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
         
            int i = Convert.ToInt32(button.Tag.ToString());
            Debug.WriteLine(i);
            string id_notif = assocNotif.response.member_request[i].notif_id.ToString();
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", id_notif),
                        new KeyValuePair<string, string>("acceptance", "true")
                    };
                string url = ("http://52.31.151.160:3000/membership/reply_member");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();

                simpleResponse = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simpleResponse.status != 200)
                {
                    warningTextBox.Text = simpleResponse.message;
                }
                else
                {
                    warningTextBox.Text = "User accepted";
                }
            }
            catch (HttpRequestException f)
            {
                Debug.WriteLine(f.Message);
            }
            catch (JsonReaderException f)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException f)
            {
                System.Diagnostics.Debug.WriteLine(f.Message);
            }
            Frame.Navigate(typeof(NotificationAssociation), id);
            // identify which button was clicked and perform necessary actions
        }

        private async void refuseUserToAssoc(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            int i = Convert.ToInt32(button.Tag.ToString());
            Debug.WriteLine(i);
            string id_notif = assocNotif.response.member_request[i].notif_id.ToString();
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", id_notif),
                        new KeyValuePair<string, string>("acceptance", "false")
                    };
                string url = ("http://52.31.151.160:3000/membership/reply_member");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();

                simpleResponse = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simpleResponse.status != 200)
                {
                    warningTextBox.Text = simpleResponse.message;
                }
                else
                {
                    warningTextBox.Text = "User refused";
                }
            }
            catch (HttpRequestException f)
            {
                Debug.WriteLine(f.Message);
            }
            catch (JsonReaderException f)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException f)
            {
                System.Diagnostics.Debug.WriteLine(f.Message);
            }
            Frame.Navigate(typeof(NotificationAssociation), id);
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
