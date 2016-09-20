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

namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericNotification : Page
    {
        class InfosNotif
        {
            public string id { get; set; }
            public string typeNotif { get; set; }
        }

        private InfosModel infos { get; set; }
        private Model model { get; set; }
        private string responseString { get; set; }
        private Grid notifGrid { get; set; }
       // public Newtonsoft.Json.Linq.JArray listObj { get; set; }

        public GenericNotification()
        {
            this.InitializeComponent();
        }

        public enum NotificationType
        {
            ResponseNotification,
            InformationNotification,
        }

        private void ModelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            InfosModel tmp = (InfosModel)button.Tag;
            Frame.Navigate(typeof(GenericProfil), tmp);
        }

        public NotificationType whatNotificationType(string notification)
        {
            string[] response = { "JoinAssoc", "JoinEvent", "AddFriend", "InviteMember", "InviteGuest" };
            string[] infos = { "NewMember", "NewGuest", "" };
            return NotificationType.InformationNotification;
        }

        public string getURLForNotificationResponse(string notif)
        {

            Dictionary<string, string> d = new Dictionary<string, string>()
             {
                {"JoinAssoc",  "/membership/reply_member"},
                {"InviteMember", "/membership/reply_invite"},

                {"JoinEvent",  "/guests/reply_guest"},
                {"InviteGuest", "/guests/reply_invite"},

                { "AddFriend", "/friendship/reply"}
              };
            return "";
        }

        //private async void acceptNotification(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    InfosNotif tmp = (InfosNotif)button.Tag;

        //    var httpClient = new HttpClient(new HttpClientHandler());
        //    try
        //    {
        //        var values = new List<KeyValuePair<string, string>>
        //            {
        //                new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
        //                new KeyValuePair<string, string>("notif_id", tmp.id),
        //                new KeyValuePair<string, string>("acceptance", "false")
        //            };
        //        string url = (Global.API_IRL + tmp.typeNotif);
        //        HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
        //        response.EnsureSuccessStatusCode();
        //        responseString = await response.Content.ReadAsStringAsync();
        //        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
        //        if ((int)jObject["status"] != 200)
        //        {
        //            Debug.WriteLine((string)jObject["message"]);
        //        }
        //        else
        //        {
        //        }
        //    }
        //    catch (HttpRequestException f)
        //    {
        //        Debug.WriteLine(f.Message);
        //    }
        //    catch (JsonReaderException f)
        //    {
        //        System.Diagnostics.Debug.WriteLine(responseString);
        //        Debug.WriteLine("Failed to read json");
        //    }
        //    catch (JsonSerializationException f)
        //    {
        //        System.Diagnostics.Debug.WriteLine(f.Message);
        //    }
        //    Frame.Navigate(typeof(GenericNotification), infos);
        //    // identify which button was clicked and perform necessary actions
        //}

        //private async void refuseNotification(object sender, RoutedEventArgs e)
        //{
        //    Button button = sender as Button;
        //    InfosNotif tmp = (InfosNotif)button.Tag;

        //    var httpClient = new HttpClient(new HttpClientHandler());
        //    try
        //    {
        //        var values = new List<KeyValuePair<string, string>>
        //            {
        //                new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
        //                new KeyValuePair<string, string>("notif_id", tmp.id),
        //                new KeyValuePair<string, string>("acceptance", "false")
        //            };
        //        string url = (Global.API_IRL + tmp.typeNotif);
        //        HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
        //        response.EnsureSuccessStatusCode();
        //        responseString = await response.Content.ReadAsStringAsync();

        //        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
        //        if ((int)jObject["status"] != 200)
        //        {
        //            Debug.WriteLine(jObject["message"]);
        //        }
        //        else
        //        {
        //        }
        //    }
        //    catch (HttpRequestException f)
        //    {
        //        Debug.WriteLine(f.Message);
        //    }
        //    catch (JsonReaderException f)
        //    {
        //        System.Diagnostics.Debug.WriteLine(responseString);
        //        Debug.WriteLine("Failed to read json");
        //    }
        //    catch (JsonSerializationException f)
        //    {
        //        System.Diagnostics.Debug.WriteLine(f.Message);
        //    }
        //    Frame.Navigate(typeof(GenericNotification), infos);
        //    // identify which button was clicked and perform necessary actions
        //}

        //public async void getNotification()
        //{
        //    var httpClient = new HttpClient(new HttpClientHandler());
        //    try
        //    {
        //        string id = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
        //        var template = new UriTemplate(Global.API_IRL + "/notifications{?token}");
        //        template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
        //        var uri = template.Resolve();
        //        Debug.WriteLine(uri);

        //        HttpResponseMessage response = await httpClient.GetAsync(uri);
        //        response.EnsureSuccessStatusCode();
        //        responseString = await response.Content.ReadAsStringAsync();
        //        System.Diagnostics.Debug.WriteLine(responseString.ToString());
        //        Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
        //        if ((int)jObject["status"] != 200)
        //        {
        //        }
        //        else
        //        {
        //            notifGrid = new Grid();
        //            notifGrid.VerticalAlignment = VerticalAlignment.Top;
        //            notifGrid.ColumnDefinitions.Add(new ColumnDefinition());
        //            int joinAssoc = 0;
        //            listObj = (Newtonsoft.Json.Linq.JArray)jObject["response"];
        //            for (int i = 0; i < listObj.Count; i++)
        //            {
        //                string[] notifTypes = model.getNotificationType();
        //                string notif_type = ((string)listObj[i]["notif_type"]);
        //                if (notifTypes.Contains(notif_type))
        //                {
        //                    if (whatNotificationType(notif_type) == NotificationType.InformationNotification)
        //                    {
        //                        notifGrid.RowDefinitions.Add(new RowDefinition());
        //                        Grid grid = new Grid();
        //                        grid.Height = 130;
        //                        grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
        //                        grid.Margin = new Thickness(0, 10, 0, 10);

        //                        var row = new RowDefinition();
        //                        row.Height = new GridLength(60);
        //                        grid.RowDefinitions.Add(row);

        //                        // accept/refuse
        //                        var row2 = new RowDefinition();
        //                        row2.Height = new GridLength(60);
        //                        grid.RowDefinitions.Add(row2);

        //                        // accept
        //                        var colum = new ColumnDefinition();
        //                        grid.ColumnDefinitions.Add(colum);

        //                        // refuse
        //                        var column2 = new ColumnDefinition();
        //                        grid.ColumnDefinitions.Add(column2);

        //                        Button title = new Button();
        //                        title.Content = listObj[i]["sender_name"];
        //                        title.HorizontalAlignment = HorizontalAlignment.Center;
        //                        //title.Click += new RoutedEventHandler(UserButtonClick);
        //                        title.BorderThickness = new Thickness(2.5);
        //                        title.Tag = i;
        //                        title.Width = 350;
        //                        title.Height = 60;

        //                        Button btnYes = new Button();
        //                        btnYes.Tag = i;
        //                       // btnYes.Click += new RoutedEventHandler(addUserToAssoc);
        //                        btnYes.Content = "Ajouter";
        //                        btnYes.Width = 160;
        //                        btnYes.HorizontalAlignment = HorizontalAlignment.Center;
        //                        btnYes.Height = 50;
        //                        btnYes.BorderThickness = new Thickness(2.5);


        //                        Button btnNo = new Button();
        //                        btnNo.Tag = i;
        //                        //btnNo.Click += new RoutedEventHandler(refuseUserToAssoc);
        //                        btnNo.Content = "Refuser";
        //                        btnNo.Width = 160;
        //                        btnNo.HorizontalAlignment = HorizontalAlignment.Center;
        //                        btnNo.Height = 50;
        //                        btnNo.BorderThickness = new Thickness(2.5);

        //                        Grid.SetColumn(title, 0);
        //                        Grid.SetColumnSpan(title, 2);
        //                        Grid.SetRow(title, 0);
        //                        grid.Children.Add(title);

        //                        Grid.SetColumn(btnYes, 0);
        //                        Grid.SetRow(btnYes, 1);
        //                        grid.Children.Add(btnYes);

        //                        Grid.SetColumn(btnNo, 1);
        //                        Grid.SetRow(btnNo, 1);
        //                        grid.Children.Add(btnNo);

        //                        notifGrid.Children.Add(grid);
        //                        Grid.SetColumn(grid, 0);
        //                        Grid.SetRow(grid, joinAssoc);
        //                        joinAssoc++;
        //                    }
        //                    else
        //                    {
        //                        notifGrid.RowDefinitions.Add(new RowDefinition());
        //                        Grid grid = new Grid();
        //                        grid.Height = 130;
        //                        grid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
        //                        grid.Margin = new Thickness(0, 10, 0, 10);

        //                        var row = new RowDefinition();
        //                        row.Height = new GridLength(60);
        //                        grid.RowDefinitions.Add(row);

        //                        // accept/refuse
        //                        var row2 = new RowDefinition();
        //                        row2.Height = new GridLength(60);
        //                        grid.RowDefinitions.Add(row2);

        //                        // accept
        //                        var colum = new ColumnDefinition();
        //                        grid.ColumnDefinitions.Add(colum);

        //                        // refuse
        //                        var column2 = new ColumnDefinition();
        //                        grid.ColumnDefinitions.Add(column2);

        //                        Button title = new Button();
        //                        //title.Content = assocNotif.response[i].sender_name;
        //                        title.HorizontalAlignment = HorizontalAlignment.Center;
        //                        //title.Click += new RoutedEventHandler(UserButtonClick);
        //                        title.BorderThickness = new Thickness(2.5);
        //                        title.Tag = i;
        //                        title.Width = 350;
        //                        title.Height = 60;

        //                        Button btnYes = new Button();
        //                        btnYes.Tag = i;
        //                        //btnYes.Click += new RoutedEventHandler(addUserToAssoc);
        //                        btnYes.Content = "Ajouter";
        //                        btnYes.Width = 160;
        //                        btnYes.HorizontalAlignment = HorizontalAlignment.Center;
        //                        btnYes.Height = 50;
        //                        btnYes.BorderThickness = new Thickness(2.5);


        //                        Button btnNo = new Button();
        //                        btnNo.Tag = i;
        //                        //btnNo.Click += new RoutedEventHandler(refuseUserToAssoc);
        //                        btnNo.Content = "Refuser";
        //                        btnNo.Width = 160;
        //                        btnNo.HorizontalAlignment = HorizontalAlignment.Center;
        //                        btnNo.Height = 50;
        //                        btnNo.BorderThickness = new Thickness(2.5);

        //                        Grid.SetColumn(title, 0);
        //                        Grid.SetColumnSpan(title, 2);
        //                        Grid.SetRow(title, 0);
        //                        grid.Children.Add(title);

        //                        Grid.SetColumn(btnYes, 0);
        //                        Grid.SetRow(btnYes, 1);
        //                        grid.Children.Add(btnYes);

        //                        Grid.SetColumn(btnNo, 1);
        //                        Grid.SetRow(btnNo, 1);
        //                        grid.Children.Add(btnNo);

        //                        notifGrid.Children.Add(grid);
        //                        Grid.SetColumn(grid, 0);
        //                        Grid.SetRow(grid, joinAssoc);
        //                        joinAssoc++;
        //                    }
        //                }
        //            }
        //            scrollList.Content = notifGrid;
        //        }
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Debug.WriteLine(e.Message);
        //    }
        //    catch (JsonReaderException e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(responseString);
        //        Debug.WriteLine("Failed to read json");
        //    }
        //    catch (JsonSerializationException e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(responseString);
        //        Debug.WriteLine("Failed to read json");
        //        System.Diagnostics.Debug.WriteLine(e.Message);
        //    }
        //}


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as InfosModel;

            if (infos.type.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
            }
            else if (infos.type.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
            }
//            getNotification();

        }
    }
}
