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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventProfil : Page
    {
        private string id;
        private string notif_id;

        class NotificationResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<GlobalNotification> response { get; set; }
        }

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest picture;

        private NotificationResponse notifications;


        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        class CommentResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public Comments response { get; set; }
        }

        class CommentsResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Comments> response { get; set; }
        }

        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public EventModel response { get; set; }
        }

        class PublishResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public Publication response { get; set; }
        }

        class NewsResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Publication> response { get; set; }
        }

        private PublishResponse publishreponse;
        private EventRequest events;
        private NewsResponse newsResponse;
        private SimpleRequest simple;
        private String responseString;
        private CommentResponse commentResponse;
        private CommentsResponse commentsResponse;
        Grid newsGrid;

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void seeCommentEvent(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int row = Convert.ToInt32(button.Tag);
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row));

            var gridComment = grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4);
            gridComment.Visibility = Visibility.Visible;
        }

        private async void getComments(int idNews, Grid commentGrid)
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/news/" + idNews + "/comments{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                commentsResponse = JsonConvert.DeserializeObject<CommentsResponse>(responseString);
                if (commentsResponse.status != 200)
                {
                    Debug.WriteLine("failed : " + commentsResponse.response);

                }
                else
                {
                    for (int x = 0; x < commentsResponse.response.Count; x++)
                    {
                        commentGrid.RowDefinitions.Add(new RowDefinition());

                        // image profil
                        Image profil = new Image();
                        profil.Margin = new Thickness(10, 10, 10, 10);
                        profil.Source = new BitmapImage(new Uri(Global.API_IRL + "" + commentsResponse.response[x].thumb_path, UriKind.Absolute));
                        Grid.SetColumn(profil, 0);
                        Grid.SetRow(profil, x);
                        commentGrid.Children.Add(profil);

                        TextBlock contentComment = new TextBlock();
                        contentComment.Margin = new Thickness(10, 10, 10, 10);
                        contentComment.FontSize = 12;
                        contentComment.TextWrapping = TextWrapping.Wrap;
                        contentComment.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                        Run test = new Run();
                        test.FontWeight = FontWeights.Bold;
                        test.Text = commentsResponse.response[x].firstname + " " + commentsResponse.response[x].lastname + " ";
                        contentComment.Inlines.Add(test);
                        contentComment.Inlines.Add(new Run { Text = commentsResponse.response[x].content });

                        Grid.SetColumn(contentComment, 1);
                        Grid.SetRow(contentComment, x);
                        commentGrid.Children.Add(contentComment);
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

        public void publishComment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int row = Convert.ToInt32(button.Tag);
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row));

            Grid gridComment = (Grid)(grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 5));
            Grid gridComments = (Grid)(grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4));
            TextBox text = (TextBox)(gridComment.Children.Cast<FrameworkElement>().FirstOrDefault(z => Grid.GetColumn(z) == 1));
            comments(text.Text, newsResponse.response[row].id, gridComments);
            text.Text = "Votre commentaire ...";
        }

        private async void comments(string text, int idNews, Grid gridComment)
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("new_id", idNews.ToString()),
                        new KeyValuePair<string, string>("content", text)
                    };
                string url = (Global.API_IRL + "/comments");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                commentResponse = JsonConvert.DeserializeObject<CommentResponse>(responseString);
                if (commentResponse.status != 200)
                {
                    informationBox.Text = commentResponse.message;
                }
                else
                {
                    gridComment.RowDefinitions.Add(new RowDefinition());
                    // image profil
                    Image profil = new Image();
                    profil.Margin = new Thickness(10, 10, 10, 10);
                    profil.Source = new BitmapImage(new Uri(Global.API_IRL + "" + commentResponse.response.thumb_path, UriKind.Absolute));
                    Grid.SetColumn(profil, 0);
                    Grid.SetRow(profil, gridComment.RowDefinitions.Count + 1);
                    gridComment.Children.Add(profil);

                    TextBlock contentComment = new TextBlock();
                    contentComment.Margin = new Thickness(10, 10, 10, 10);
                    contentComment.FontSize = 12;
                    contentComment.TextWrapping = TextWrapping.Wrap;
                    contentComment.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                    Run test = new Run();
                    test.FontWeight = FontWeights.Bold;
                    test.Text = commentResponse.response.firstname + " " + commentResponse.response.lastname + " ";
                    contentComment.Inlines.Add(test);
                    contentComment.Inlines.Add(new Run { Text = commentResponse.response.content });

                    Grid.SetColumn(contentComment, 1);
                    Grid.SetRow(contentComment, gridComment.RowDefinitions.Count + 1);
                    gridComment.Children.Add(contentComment);
                    gridComment.RowDefinitions.Add(new RowDefinition());
                    gridComment.Visibility = Visibility.Visible;
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


        private void initNews(int nbNews)
        {
            newsGrid = new Grid();
            newsGrid.VerticalAlignment = VerticalAlignment.Top;
            newsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < nbNews; i++)
            {
                newsGrid.RowDefinitions.Add(new RowDefinition());

                // main new's grid
                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 10, 0, 10);
                grid.Background = new SolidColorBrush(Color.FromArgb(100, 83, 166, 52));

                // grid new's info

                // row username
                var row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                // row time published
                var row2 = new RowDefinition();
                row2.Height = new GridLength(30);
                grid.RowDefinitions.Add(row2);

                // row content
                var rowContent = new RowDefinition();
                grid.RowDefinitions.Add(rowContent);

                // row share / see comment
                var row4 = new RowDefinition();
                grid.RowDefinitions.Add(row4);

                // all comment
                var row5 = new RowDefinition();
                grid.RowDefinitions.Add(row5);

                //column image user / share
                var colum = new ColumnDefinition();
                colum.Width = new GridLength(60);
                grid.ColumnDefinitions.Add(colum);

                var colum1 = new ColumnDefinition();
                colum1.Width = new GridLength(200);
                grid.ColumnDefinitions.Add(colum1);

                //column comment
                var colum2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(colum2);

                // add comment
                grid.RowDefinitions.Add(new RowDefinition());

                // image profil
                Image btn = new Image();
                btn.Margin = new Thickness(10, 10, 10, 0);
                btn.Source = new BitmapImage(new Uri(Global.API_IRL + "" + newsResponse.response[i].group_thumb_path, UriKind.Absolute));
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                grid.Children.Add(btn);

                // username
                // CHANGE BY HYPERLINK
                TextBlock poster = new TextBlock();
                poster.Text = newsResponse.response[i].group_name;
                poster.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                poster.Margin = new Thickness(10, 5, 10, 5);
                poster.FontSize = 14;
                Grid.SetColumn(poster, 1);
                Grid.SetRow(poster, 0);
                grid.Children.Add(poster);

                // time published
                TextBlock date = new TextBlock();
                DateTime convertedDate;
                convertedDate = Convert.ToDateTime(newsResponse.response[i].created_at);
                date.Text = convertedDate.ToString();
                date.Margin = new Thickness(10, 0, 10, 0);
                date.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                Grid.SetColumn(date, 1);
                Grid.SetRow(date, 1);
                grid.Children.Add(date);

                // content
                TextBlock content = new TextBlock();
                content.Margin = new Thickness(10, 10, 10, 10);
                content.FontSize = 12;
                content.TextWrapping = TextWrapping.Wrap;
                content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                content.Text = newsResponse.response[i].content;
                Grid.SetColumn(content, 0);
                Grid.SetColumnSpan(content, 3);
                Grid.SetRow(content, 2);
                grid.Children.Add(content);

                // see Comment
                Button seeComment = new Button();
                seeComment.Click += seeCommentEvent;
                seeComment.Content = "Voir les commentaires";
                seeComment.Tag = i;
                seeComment.Width = 150;
                seeComment.FontSize = 12;
                seeComment.Margin = new Thickness(10, 0, 10, 0);

                Grid.SetColumn(seeComment, 0);
                Grid.SetRow(seeComment, 3);
                Grid.SetColumnSpan(seeComment, 2);
                grid.Children.Add(seeComment);

                // grid comments 

                Grid commentGrid = new Grid();
                commentGrid.VerticalAlignment = VerticalAlignment.Top;
                var columnCommentImage = new ColumnDefinition();
                columnCommentImage.Width = new GridLength(60);
                commentGrid.ColumnDefinitions.Add(columnCommentImage);
                commentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                getComments(newsResponse.response[i].id, commentGrid);
                Grid.SetColumn(commentGrid, 0);
                Grid.SetColumnSpan(commentGrid, 3);
                Grid.SetRow(commentGrid, 4);
                commentGrid.Visibility = Visibility.Collapsed;
                grid.Children.Add(commentGrid);

                // add Comment
                Grid addComment = new Grid();
                addComment.RowDefinitions.Add(new RowDefinition());
                ColumnDefinition myimage = new ColumnDefinition();
                myimage.Width = new GridLength(60);
                addComment.ColumnDefinitions.Add(myimage);
                ColumnDefinition mycomment = new ColumnDefinition();
                mycomment.Width = new GridLength(175);
                addComment.ColumnDefinitions.Add(mycomment);
                addComment.ColumnDefinitions.Add(new ColumnDefinition());
                // image
                Image myprofil = new Image();
                myprofil.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
                Grid.SetColumn(myprofil, 0);
                Grid.SetRow(myprofil, 0);
                addComment.Children.Add(myprofil);
                // comment
                TextBox comment = new TextBox();
                comment.GotFocus += TextBox_GotFocus;
                comment.Text = "Votre commentaire ...";
                comment.FontSize = 12;
                Grid.SetColumn(comment, 1);
                Grid.SetRow(comment, 0);
                addComment.Children.Add(comment);
                // valide
                Button ok = new Button();
                ok.Click += publishComment;
                ok.Tag = i;
                ok.Width = 40;
                ok.Margin = new Thickness(10, 0, 10, 0);
                ok.Content = "Envoyer";
                ok.FontSize = 12;
                Grid.SetColumn(ok, 2);
                Grid.SetRow(ok, 0);
                addComment.Children.Add(ok);
                Grid.SetColumn(addComment, 0);
                Grid.SetColumnSpan(addComment, 3);
                Grid.SetRow(addComment, 5);
                grid.Children.Add(addComment);


                grid.Margin = new Thickness(10, 10, 10, 10);
                newsGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, i);
            }
            newsScroll.Content = newsGrid;
            newsScroll.Background = new SolidColorBrush(Color.FromArgb(250, 229, 255, 235));
        }

        private async void getNews()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/events/" + id + "/news{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine("NEWS : " + responseString.ToString());
                newsResponse = JsonConvert.DeserializeObject<NewsResponse>(responseString);
                if (newsResponse.status != 200)
                {

                }
                else
                {
                    Debug.WriteLine(newsResponse.response);
                    Debug.WriteLine(responseString);
                    initNews(newsResponse.response.Count);
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

        public void publishNews(object send, RoutedEventArgs e)
        {
            PublishNews();
        }

        private async void PublishNews()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("content", news.Text),
                        new KeyValuePair<string, string>("group_id", id),
                        new KeyValuePair<string, string>("news_type", "Status"),
                        new KeyValuePair<string, string>("group_type", "Event"),
                };
                string url = (Global.API_IRL + "/news/wall_message");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                publishreponse = JsonConvert.DeserializeObject<PublishResponse>(responseString);
                if (publishreponse.status != 200)
                {
                    informationBox.Text = publishreponse.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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


        private async void joinEventRequest()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("event_id", id)

                    };
                string url = (Global.API_IRL + "/guests/join");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();

                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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
            }
        }

        public void joinEventClick(object send, RoutedEventArgs e)
        {
            joinEventRequest();
        }

        private async void accepteInvit()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", notif_id),
                        new KeyValuePair<string, string>("acceptance", "true")
                    };
                string url = (Global.API_IRL + "/guests/reply_invite");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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
            }
        }

        private async void refuseInvit()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("notif_id", notif_id),
                        new KeyValuePair<string, string>("acceptance", "false")
                    };
                string url = (Global.API_IRL + "/guests/reply_invite");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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
            }
        }

        public void acceptInvitationClick(object send, RoutedEventArgs e)
        {
            accepteInvit();
        }

        public void RefuseInvitationClick(object send, RoutedEventArgs e)
        {
            refuseInvit();
        }

        private async void leaveEvent()
        {
            string url = Global.API_IRL + "/guests/leave?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString()
                + "&event_id=" + id;
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
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), id);
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

        public void leaveEventClick(object send, RoutedEventArgs e)
        {
            leaveEvent();
        }

        public void optionsEventClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Event.EventGestion), events.response);
        }

        public void memberClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Event.EventMember), id);
        }

        public void getInformation(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Event.EventInformation), id);
        }

        private async void getNotification()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string myId = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"].ToString();
                var template = new UriTemplate(Global.API_IRL + "/notifications{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                notifications = JsonConvert.DeserializeObject<NotificationResponse>(responseString);
                if (notifications.status != 200)
                {
                    Debug.WriteLine("Failed to get notification");
                }
                else
                {

                    Debug.WriteLine(responseString);
                    for (int x = 0; x < notifications.response.Count; x++)
                    {
                        if (notifications.response[x].notif_type.Equals("InviteGuest", StringComparison.Ordinal)
                             && id.Equals(notifications.response[x].event_id, StringComparison.Ordinal))
                            notif_id = notifications.response[x].id;
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
        }

        private async void getPicture()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/events/" + id + "/main_picture{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                picture = JsonConvert.DeserializeObject<PictureRequest>(responseString);
                if (picture.status != 200)
                {

                }
                else
                {
                    if (picture.response != null)
                    {
                        ImageBrush myBrush = new ImageBrush();
                        myBrush.ImageSource =
                            new BitmapImage(new Uri("http://52.31.151.160:3000" + picture.response.picture_path.thumb.url, UriKind.Absolute));
                        logo.Fill = myBrush;
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

        private async void getInformation()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/events/" + id + "{?token}");
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
                    titleText.Text = events.response.title;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"] = id.ToString();
                    if (events.response.rights.Equals("host", StringComparison.Ordinal)
                        || events.response.rights.Equals("admin", StringComparison.Ordinal))
                    {
                        OptionsButton.Visibility = Visibility.Visible;

                    }
                    else
                        OptionsButton.Visibility = Visibility.Collapsed;
                    if (events.response.rights.Equals("none", StringComparison.Ordinal))
                        joinEventButton.Visibility = Visibility.Visible;
                    else if (events.response.rights.Equals("invited", StringComparison.Ordinal))
                    {
                        accepteInvitation.Visibility = Visibility.Visible;
                        RefuseIntivation.Visibility = Visibility.Visible;
                        getNotification();
                    }
                    else if (events.response.rights.Equals("waiting", StringComparison.Ordinal))
                        informationBox.Text = "En attente de confirmation";
                    else
                        leaveEventButton.Visibility = Visibility.Visible;
                    getPicture();
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

        public EventProfil()
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
            id = e.Parameter as string;
            Debug.WriteLine(id);
            getInformation();
            getNews();
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }
    }
}
