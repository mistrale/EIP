﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Tavis.UriTemplates;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accueil : Page
    {
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
                        if (message.response.notifications.add_friend.Count > notifs.add_friend.Count)
                        {
                            flag = true;
                            updateGUI();
                            notifs = message.response.notifications;
                            Windows.Storage.ApplicationData.Current.LocalSettings.Values["notifications"] = JsonConvert.SerializeObject(message.response.notifications);
                            Debug.WriteLine("On a recu une nouvelle notification !");
                        }
                        else
                        {
                            flag = false;
                            updateGUI();
                            notifs = message.response.notifications;
                            Debug.WriteLine("0 nouvelles notificaitons");
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
                } else
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

        public void notificationButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Notification));
        }

        public void passportButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void eventButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void logoutButtonClick(object sender, RoutedEventArgs e)
        {
            doCoroutine = false;
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("mail");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("firstname");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("lastname");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("city");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("genre");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("allowedgps");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("birthday");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("id");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("password");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("token");
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("notifications");
            this.Frame.Navigate(typeof(MainPage));
        }

        public void wtf2ButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void associationButtonClick(object sender, RoutedEventArgs e)
        {

        }


        public void homeButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void profilButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Profil), (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString());
        }

        public void friendsButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Friend));
        }

        public void setVisibility(object sender, RoutedEventArgs e)
        {
            if (isVisibile)
            {
                secondBorder.Visibility = Visibility.Collapsed;
                firstBorder.Margin = new Thickness(0, 570, 0, 0);
                isVisibile = false;
            }
            else
            {
                isVisibile = true;
                firstBorder.Margin = new Thickness(0, 500, 0, 70);
                secondBorder.Visibility = Visibility.Visible;
            }
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        private void initAccueil()
        {
            //Publication test = new Publication();
            //test.author = "Aude Sikorav";
            //test.date = "17h25";
            //test.content = "Bonjour a tous, voici une publication test pour voir combien en longeur elle va occupée et voir si Windows phone qui pue la merde gere tout seul la mise en place";
            //test.title = "Welcome in Caritathelp";

            //Comment comment = new Comment();
            //comment.autor = "Robin Vassuer";
            //comment.content = "Tg aude tu pues la merde";
            //comment.date = "17h28";
            //test.comments.Add(comment);
            Grid mainGrid = new Grid();

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            int i = 0;

            while (i < 10)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                Grid grid = new Grid();
                // grid.Background = new SolidColorBrush(Color.FromArgb(250, 100, 100, 100));

                // row
                var row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                var row2 = new RowDefinition();
                row2.Height = new GridLength(30);
                grid.RowDefinitions.Add(row2);

                var row3 = new RowDefinition();
                grid.RowDefinitions.Add(row3);

                var row4 = new RowDefinition();
                grid.RowDefinitions.Add(row4);

                //column
                var colum = new ColumnDefinition();
                colum.Width = new GridLength(50);
                grid.ColumnDefinitions.Add(colum);

                var column2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column2);

                var column3 = new ColumnDefinition();
                column3.Width = new GridLength(60);
                grid.ColumnDefinitions.Add(column3);

                // title
                TextBlock title = new TextBlock();
                title.Text = "Aude Sikorav";
                title.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                title.Margin = new Thickness(10, 5, 10, 5);
                title.FontSize = 14;
                Grid.SetColumn(title, 1);
                Grid.SetColumnSpan(title, 2);
                Grid.SetRow(title, 0);
                grid.Children.Add(title);

                // hour
                TextBlock date = new TextBlock();
                date.Text = "7h25";
                date.Margin = new Thickness(10, 0, 10, 0);
                date.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                Grid.SetColumn(date, 1);
                Grid.SetColumnSpan(date, 2);
                Grid.SetRow(date, 1);
                grid.Children.Add(date);

                // image
                Button btn = new Button();
                btn.Background = new SolidColorBrush(Color.FromArgb(250, 100, 100, 100));
                btn.Margin = new Thickness(10, 10, 0, 10);
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                grid.Children.Add(btn);

                // content
                TextBlock content = new TextBlock();
                content.Margin = new Thickness(10, 10, 10, 10);
                content.FontSize = 12;
                content.TextWrapping = TextWrapping.Wrap;
                content.Width = 340;
                content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                content.Text = "Bonjour a tous, voici une publication test pour voir combien en longeur elle va occupée et voir si Windows phone qui pue la merde gere tout seul la mise en place";
                Grid.SetColumn(content, 0);
                Grid.SetColumnSpan(content, 3);
                Grid.SetRow(content, 2);
                grid.Children.Add(content);

                // add comment
                Button me = new Button();
                me.Background = new SolidColorBrush(Colors.Blue);
                me.Margin = new Thickness(10, 0, 10, 0);
                Grid.SetColumn(me, 0);
                Grid.SetRow(me, 3);
                grid.Children.Add(me);

                TextBox newComment = new TextBox();
                newComment.BorderBrush = new SolidColorBrush(Colors.Black);
               // newComment.BorderThickness = new Thickness(1, 1, 1, 1);
                newComment.Text = "Votre commentaire ...";
                newComment.Foreground = new SolidColorBrush(Colors.Black);
                newComment.FontSize = 12;
                newComment.Margin = new Thickness(0, 10, 0, 0);
                Grid.SetColumn(newComment, 1);
                Grid.SetColumnSpan(newComment, 1);
                Grid.SetRow(newComment, 3);
                grid.Children.Add(newComment);

                Button ok = new Button();
                ok.Content = "Ok";
                ok.Foreground = new SolidColorBrush(Colors.Black);
                ok.Background = new SolidColorBrush(Colors.Gray);
                ok.Margin = new Thickness(10, 0, 10, 0);
                Grid.SetColumn(ok, 2);
                Grid.SetRow(ok, 3);

                grid.Background = new SolidColorBrush(Color.FromArgb(255, 182, 215, 168));
                grid.Children.Add(ok);


                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(250, 100, 100, 100));
                border.BorderThickness = new Thickness(1, 1, 1, 1);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 3);
                Grid.SetRow(border, 0);
                Grid.SetRowSpan(border, 4);
                grid.Margin = new Thickness(10, 10, 10, 10);
                grid.Children.Add(border);

                mainGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, i);
                i++;
            }

            scroll.Content = mainGrid;
        }

        public Accueil()
        {
            this.InitializeComponent();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            alertButtonNotity.Visibility = Visibility.Collapsed;
            initAccueil();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loadCoroutine();
            notifs = JsonConvert.DeserializeObject<Notifications>((string)(Windows.Storage.ApplicationData.Current.LocalSettings.Values["notifications"]));

        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            searchTextBox.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["search"] = searchTextBox.Text;
            Frame.Navigate(typeof(Research));
        }
    }
}
