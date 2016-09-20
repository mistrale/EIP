using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556



namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericProfil : Page
    {
        class NotificationResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<GlobalNotification> response { get; set; }
        }

        class NewsResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Publication> response { get; set; }
        }

        class CommentResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public Comments response { get; set; }
        }

        class PublishResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public Publication response { get; set; }
        }

        class CommentsResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Comments> response { get; set; }
        }

        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        private SimpleRequest simple;
        private CommentsResponse commentsResponse;
        private PublishResponse publishResponse;
        private CommentResponse commentResponse;
        private Grid newsGrid;
        private NewsResponse newsResponse;
        private InfosModel infos;
        private Model model;
        private string responseRequest;
        private string notif_id;
        private NotificationResponse notifications;

        public GenericProfil()
        {
            this.InitializeComponent();
        }

        public void optionsClick(object send, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GenericManagement), infos);
        }

        public void eventClick(object send, RoutedEventArgs e)
        {
            InfosListModel tmp = new InfosListModel();
            tmp.id = infos.id;
            tmp.typeModel = infos.type;
            tmp.listTypeModel = "event";
            Frame.Navigate(typeof(GenericListModel), tmp);
        }

        public void relatedClick(object send, RoutedEventArgs e)
        {
            InfosListModel tmp = new InfosListModel();
            tmp.id = infos.id;
            tmp.typeModel = infos.type;
            tmp.listTypeModel = "user";
            Frame.Navigate(typeof(GenericListModel), tmp);
        }

        public void acceptInvitationClick(object send, RoutedEventArgs e)
        {
            accepteInvit();
        }

        public void RefuseInvitationClick(object send, RoutedEventArgs e)
        {
            refuseInvit();
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
                string url = (Global.API_IRL + Model.Values[infos.type]["AcceptURL"]);
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseRequest);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
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
                string url = (Global.API_IRL + Model.Values[infos.type]["AcceptURL"]);
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseRequest);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void removeClick(object send, RoutedEventArgs e)
        {
            removeModel();
        }

        private async void removeModel()
        {
            string url = Global.API_IRL + Model.Values[infos.type]["RemoveURL"] + "?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString()
                + "&" + Model.Values[infos.type]["TypeID"] + "=" + infos.id;
            Debug.WriteLine(url);
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseRequest);
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseRequest);
                if (simple.status != 200)
                {
                    informationBox.Text = simple.message;
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(responseRequest);
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void addClick(object send, RoutedEventArgs e)
        {
            addModel();
        }

        private async void addModel()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>(Model.Values[infos.type]["TypeID"], infos.id.ToString())

                    };
                string url = (Global.API_IRL + Model.Values[infos.type]["AddURL"]);
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseRequest);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        private void sendClick(object sender, RoutedEventArgs e)
        {
            if (!publishBox.Text.Equals("Publier un message", StringComparison.Ordinal)
                && !publishBox.Text.Equals("", StringComparison.Ordinal))
            {
               PublishNews();
            }
        }



        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(100, 114, 136, 142));
        }

        private async void PublishNews()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("content", publishBox.Text),
                        new KeyValuePair<string, string>("group_id", infos.id.ToString()),
                        new KeyValuePair<string, string>("news_type", "Status"),
                        new KeyValuePair<string, string>("group_type",Model.Values[infos.type]["Model"]),
                        new KeyValuePair<string, string>("as_group",  adminBox.IsChecked.ToString().ToLower()),
                        new KeyValuePair<string, string>("private",  privateBox.IsChecked.ToString().ToLower()),
                };
                string url = (Global.API_IRL + "/news/wall_message");
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                publishResponse = JsonConvert.DeserializeObject<PublishResponse>(responseRequest);
                if (publishResponse.status != 200)
                {
                    informationBox.Text = publishResponse.message;
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Debug.WriteLine("Fail in publish news friends");
            }
        }

        public void seeCommentClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int row = Convert.ToInt32(button.Tag);
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row));

            var gridComment = grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4);
            gridComment.Visibility = Visibility.Visible;
        }

        public void updateNewsClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NewsInfos row = (NewsInfos)(button.Tag);

            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row.id_rows));
            string text = ((TextBox)grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 2)).Text;
            UpdateNew(text, row.id_news);
        }

        private async void UpdateNew(string text, int idNews)
        {
            string url = Global.API_IRL + "/news/" + idNews;
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("id", idNews.ToString()),
                        new KeyValuePair<string, string>("content", text),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseRequest);
                publishResponse = JsonConvert.DeserializeObject<PublishResponse>(responseRequest);
                if (publishResponse.status != 200)
                {
                    informationBox.Text = publishResponse.message;
                }
                else
                {
                    informationBox.Text = "Publication modifié !";
                }
            }
            catch (HttpRequestException e)
            {
                informationBox.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        public void deleteNewClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NewsInfos cmt = (NewsInfos)button.Tag;
            DeleteNew(cmt.id_news);
        }

        private async void DeleteNew(int idNews)
        {
            string url = Global.API_IRL + "/news/" + idNews.ToString() + "?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString();
            Debug.WriteLine(url);
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseRequest);
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseRequest);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(responseRequest);
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        public void publishCommentClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int row = Convert.ToInt32(button.Tag);
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row));

            Grid gridComment = (Grid)(grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 5));
            Grid gridComments = (Grid)(grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4));
            TextBox text = (TextBox)(gridComment.Children.Cast<FrameworkElement>().FirstOrDefault(z => Grid.GetColumn(z) == 1));
            if (!text.Text.Equals("Votre commentaire ...", StringComparison.Ordinal)
                && !text.Text.Equals("", StringComparison.Ordinal))
            {
                PublishComment(text.Text, newsResponse.response[row].id, gridComments, row);
                text.Text = "Votre commentaire ...";
            }
        }

        private async void PublishComment(string text, int idNews, Grid gridComment, int row_news)
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
                responseRequest = await response.Content.ReadAsStringAsync();
                commentResponse = JsonConvert.DeserializeObject<CommentResponse>(responseRequest);
                if (commentResponse.status != 200)
                {
                    informationBox.Text = commentResponse.message;
                }
                else
                {
                    gridComment.RowDefinitions.Add(new RowDefinition());

                    Grid oneComment = new Grid();
                    oneComment.VerticalAlignment = VerticalAlignment.Top;
                    var columnCommentImage = new ColumnDefinition();
                    columnCommentImage.Width = new GridLength(60);
                    oneComment.ColumnDefinitions.Add(columnCommentImage);

                    var nameCol = new ColumnDefinition();
                    nameCol.Width = new GridLength(100);
                    oneComment.ColumnDefinitions.Add(nameCol);

                    var column1 = new ColumnDefinition();
                    column1.Width = new GridLength(50);
                    oneComment.ColumnDefinitions.Add(column1);
                    var column2 = new ColumnDefinition();
                    oneComment.ColumnDefinitions.Add(column2);
                    oneComment.RowDefinitions.Add(new RowDefinition());
                    oneComment.RowDefinitions.Add(new RowDefinition());

                    // image profil
                    Image profil = new Image();
                    profil.Margin = new Thickness(10, 10, 10, 10);
                    profil.Source = new BitmapImage(new Uri(Global.API_IRL + commentResponse.response.thumb_path, UriKind.Absolute));
                    Grid.SetColumn(profil, 0);
                    Grid.SetRow(profil, 0);
                    oneComment.Children.Add(profil);

                    // name
                    TextBlock name = new TextBlock();
                    name.Margin = new Thickness(10, 10, 10, 10);
                    name.FontSize = 12;
                    name.TextWrapping = TextWrapping.Wrap;
                    name.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                    name.Text = commentResponse.response.firstname + " " + commentResponse.response.lastname;
                    name.FontWeight = FontWeights.Bold;
                    name.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumn(name, 1);
                    Grid.SetRow(name, 0);
                    oneComment.Children.Add(name);

                    // content
                    TextBox content = new TextBox();
                    content.Background = new SolidColorBrush(Color.FromArgb(0, 83, 166, 52));
                    content.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 83, 166, 52));
                    content.Margin = new Thickness(10, 10, 10, 10);
                    content.FontSize = 12;
                    content.TextWrapping = TextWrapping.Wrap;
                    content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                    content.Text = commentResponse.response.content;
                    content.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetColumn(content, 2);
                    Grid.SetColumnSpan(content, 2);
                    Grid.SetRow(content, 0);
                    oneComment.Children.Add(content);

                    // update /delete

                    // update comment
                    commentInfos tmp = new commentInfos();
                    tmp.id_news = idNews;
                    tmp.id_comments = commentResponse.response.id;
                    tmp.row_news = row_news;
                    tmp.row_cmt = gridComment.RowDefinitions.Count + 1;

                    Button updateNew = new Button();
                    updateNew.Content = "Modifier";
                    updateNew.Tag = tmp;
                    updateNew.FontSize = 12;
                    updateNew.Click += updateCommentClick;

                    Grid.SetColumn(updateNew, 1);
                    Grid.SetRow(updateNew, 1);
                    Grid.SetColumnSpan(updateNew, 2);
                    oneComment.Children.Add(updateNew);

                    // delete news
                    Button deleteCmt = new Button();
                    deleteCmt.Content = "Supprimer";
                    deleteCmt.Tag = tmp;
                    deleteCmt.Click += deleteCommentClick;
                    deleteCmt.FontSize = 12;
                    deleteCmt.Margin = new Thickness(0, 0, 10, 0);

                    Grid.SetColumn(deleteCmt, 3);
                    Grid.SetRow(deleteCmt, 1);
                    oneComment.Children.Add(deleteCmt);

                    Grid.SetColumn(oneComment, 0);
                    Grid.SetRow(oneComment, gridComment.RowDefinitions.Count + 1);
                    gridComment.Children.Add(oneComment);

                    /// pas la
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
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Debug.WriteLine("Fail in publish news friends");
            }
        }

        public void updateCommentClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            commentInfos cmt = (commentInfos)button.Tag;
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == cmt.row_news));

            Grid gridComment = (Grid)grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4);
            Grid oneComment = (Grid)gridComment.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == cmt.row_cmt);
            string text = ((TextBox)(oneComment.Children.Cast<FrameworkElement>()
                .FirstOrDefault(x => Grid.GetRow(x) == 0 && Grid.GetColumn(x) == 2))).Text;
            Debug.WriteLine("on update avec :  " + text + " d id : " + cmt.id_comments);
            UpdateComment(cmt, text);
        }

        private async void UpdateComment(commentInfos cmt, string content)
        {
            string url = Global.API_IRL + "/comments/" + cmt.id_comments;
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("id", cmt.id_comments.ToString()),
                        new KeyValuePair<string, string>("content", content),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseRequest);
                commentResponse = JsonConvert.DeserializeObject<CommentResponse>(responseRequest);
                if (commentResponse.status != 200)
                {
                    informationBox.Text = commentResponse.message;
                }
                else
                {
                    informationBox.Text = "Commentaire modifié !";
                }
            }
            catch (HttpRequestException e)
            {
                informationBox.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        public void deleteCommentClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            commentInfos cmt = (commentInfos)button.Tag;
            DeleteComment(cmt);
        }

        private async void DeleteComment(commentInfos cmt)
        {
            string url = Global.API_IRL + "/comments/" + cmt.id_comments.ToString() + "?token=" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"].ToString();
            Debug.WriteLine(url);
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseRequest);
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseRequest);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine(responseRequest);
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        private async void getComments(int idNews, Grid commentGrid, int row)
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
                responseRequest = await response.Content.ReadAsStringAsync();
                commentsResponse = JsonConvert.DeserializeObject<CommentsResponse>(responseRequest);
                if (commentsResponse.status != 200)
                {
                    Debug.WriteLine("failed : " + commentsResponse.response);

                }
                else
                {
                    for (int x = 0; x < commentsResponse.response.Count; x++)
                    {
                        commentGrid.RowDefinitions.Add(new RowDefinition());

                        Grid oneComment = new Grid();
                        oneComment.VerticalAlignment = VerticalAlignment.Top;
                        var columnCommentImage = new ColumnDefinition();
                        columnCommentImage.Width = new GridLength(60);
                        oneComment.ColumnDefinitions.Add(columnCommentImage);

                        var nameCol = new ColumnDefinition();
                        nameCol.Width = new GridLength(100);
                        oneComment.ColumnDefinitions.Add(nameCol);

                        var column1 = new ColumnDefinition();
                        column1.Width = new GridLength(50);
                        oneComment.ColumnDefinitions.Add(column1);
                        var column2 = new ColumnDefinition();
                        oneComment.ColumnDefinitions.Add(column2);
                        oneComment.RowDefinitions.Add(new RowDefinition());
                        oneComment.RowDefinitions.Add(new RowDefinition());

                        // image profil
                        Image profil = new Image();
                        profil.Margin = new Thickness(10, 10, 10, 10);
                        profil.Source = new BitmapImage(new Uri(Global.API_IRL + commentsResponse.response[x].thumb_path, UriKind.Absolute));
                        Grid.SetColumn(profil, 0);
                        Grid.SetRow(profil, 0);
                        oneComment.Children.Add(profil);

                        // name
                        TextBlock name = new TextBlock();
                        name.Margin = new Thickness(10, 10, 10, 10);
                        name.FontSize = 12;
                        name.TextWrapping = TextWrapping.Wrap;
                        name.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                        name.Text = commentsResponse.response[x].firstname + " " + commentsResponse.response[x].lastname;
                        name.FontWeight = FontWeights.Bold;
                        name.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetColumn(name, 1);
                        Grid.SetRow(name, 0);
                        oneComment.Children.Add(name);

                        // content
                        TextBox content = new TextBox();
                        content.Background = new SolidColorBrush(Color.FromArgb(0, 83, 166, 52));
                        content.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 83, 166, 52));
                        content.Margin = new Thickness(10, 10, 10, 10);
                        content.FontSize = 12;
                        content.TextWrapping = TextWrapping.Wrap;
                        content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                        content.Text = commentsResponse.response[x].content;
                        content.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetColumn(content, 2);
                        Grid.SetColumnSpan(content, 2);
                        Grid.SetRow(content, 0);
                        oneComment.Children.Add(content);

                        // update /delete

                        // update comment

                        if (commentsResponse.response[x].volunteer_id == Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]))
                        {
                            commentInfos tmp = new commentInfos();
                            tmp.id_news = idNews;
                            tmp.id_comments = commentsResponse.response[x].id;
                            tmp.row_news = row;
                            tmp.row_cmt = x;

                            Button updateNew = new Button();
                            updateNew.Content = "Modifier";
                            updateNew.Tag = tmp;
                            updateNew.FontSize = 12;
                            updateNew.Click += updateCommentClick;

                            Grid.SetColumn(updateNew, 1);
                            Grid.SetRow(updateNew, 1);
                            Grid.SetColumnSpan(updateNew, 2);
                            oneComment.Children.Add(updateNew);

                            // delete news
                            Button deleteCmt = new Button();
                            deleteCmt.Content = "Supprimer";
                            deleteCmt.Tag = tmp;
                            deleteCmt.Click += deleteCommentClick;
                            deleteCmt.FontSize = 12;
                            deleteCmt.Margin = new Thickness(0, 0, 10, 0);

                            Grid.SetColumn(deleteCmt, 3);
                            Grid.SetRow(deleteCmt, 1);
                            oneComment.Children.Add(deleteCmt);
                        }
                        Grid.SetColumn(oneComment, 0);
                        Grid.SetRow(oneComment, x);
                        commentGrid.Children.Add(oneComment);
                    }
                }
            }
            catch (HttpRequestException e)
            {
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(responseRequest);
                System.Diagnostics.Debug.WriteLine(e.Message);
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

                // column image user / share
                var colum = new ColumnDefinition();
                colum.Width = new GridLength(60);
                grid.ColumnDefinitions.Add(colum);

                var colum1 = new ColumnDefinition();
                colum1.Width = new GridLength(75);
                grid.ColumnDefinitions.Add(colum1);

                //column comment
                var colum2 = new ColumnDefinition();
                //  colum2.Width = new GridLength(80);
                grid.ColumnDefinitions.Add(colum2);

                grid.ColumnDefinitions.Add(new ColumnDefinition());

                // add comment
                grid.RowDefinitions.Add(new RowDefinition());

                // image profil
                Image btn = new Image();
                btn.Margin = new Thickness(10, 10, 10, 0);
                btn.Source = new BitmapImage(new Uri(Global.API_IRL + newsResponse.response[i].group_thumb_path, UriKind.Absolute));

                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                grid.Children.Add(btn);

                // username
                // CHANGE BY HYPERLINK
                TextBlock poster = new TextBlock();
                if (newsResponse.response[i].as_group == true)
                {
                    poster.Text = newsResponse.response[i].group_name;
                }
                else if (newsResponse.response[i].volunteer_id != newsResponse.response[i].group_id)
                {
                    poster.Text = newsResponse.response[i].volunteer_name + " > " + newsResponse.response[i].group_name;
                }
                else
                {
                    poster.Text = newsResponse.response[i].group_name;
                }
                poster.Foreground = new SolidColorBrush(Color.FromArgb(100, 114, 136, 142));
                poster.FontWeight = FontWeights.Bold;
                poster.Margin = new Thickness(10, 5, 10, 5);
                poster.FontSize = 14;
                Grid.SetColumn(poster, 1);
                Grid.SetColumnSpan(poster, 3);
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
                TextBox content = new TextBox();
                content.Background = new SolidColorBrush(Color.FromArgb(0, 83, 166, 52));
                content.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 83, 166, 52));
                content.Margin = new Thickness(10, 10, 10, 10);
                content.FontSize = 12;
                content.TextWrapping = TextWrapping.Wrap;
                content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                content.Text = newsResponse.response[i].content;
                Grid.SetColumn(content, 0);
                Grid.SetColumnSpan(content, 4);
                Grid.SetRow(content, 2);
                grid.Children.Add(content);

                // see Comment
                Button seeComment = new Button();
                seeComment.Click += seeCommentClick;
                seeComment.Content = newsResponse.response[i].number_comments.ToString() + " commentaires";
                seeComment.Tag = i;
                seeComment.FontSize = 12;
                seeComment.Margin = new Thickness(10, 0, 10, 0);

                Grid.SetColumn(seeComment, 0);
                Grid.SetRow(seeComment, 3);
                Grid.SetColumnSpan(seeComment, 2);
                grid.Children.Add(seeComment);

                // update news
                if (newsResponse.response[i].volunteer_id == Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]))
                {
                    NewsInfos tmp = new NewsInfos();

                    tmp.id_news = newsResponse.response[i].id;
                    tmp.id_rows = i;

                    Button updateNew = new Button();
                    updateNew.Content = "Modifier";
                    updateNew.Tag = tmp;
                    updateNew.FontSize = 12;
                    updateNew.Click += updateNewsClick;

                    Grid.SetColumn(updateNew, 2);
                    Grid.SetRow(updateNew, 3);
                    grid.Children.Add(updateNew);

                    // delete news
                    Button deleteNews = new Button();
                    deleteNews.Content = "Supprimer";
                    deleteNews.Tag = tmp;
                    deleteNews.Click += deleteNewClick;
                    deleteNews.FontSize = 12;
                    deleteNews.Margin = new Thickness(0, 0, 10, 0);

                    Grid.SetColumn(deleteNews, 3);
                    Grid.SetRow(deleteNews, 3);
                    grid.Children.Add(deleteNews);
                }

                // grid comments 

                Grid commentGrid = new Grid();
                commentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                getComments(newsResponse.response[i].id, commentGrid, i);
                Grid.SetColumn(commentGrid, 0);
                Grid.SetColumnSpan(commentGrid, 4);
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
                ok.Click += publishCommentClick;
                ok.Tag = i;
                ok.Width = 40;
                ok.Margin = new Thickness(10, 0, 10, 0);
                ok.Content = "Envoyer";
                ok.FontSize = 12;
                Grid.SetColumn(ok, 2);
                Grid.SetRow(ok, 0);
                addComment.Children.Add(ok);
                Grid.SetColumn(addComment, 0);
                Grid.SetColumnSpan(addComment, 4);
                Grid.SetRow(addComment, 5);
                grid.Children.Add(addComment);


                grid.Margin = new Thickness(10, 10, 10, 10);
                newsGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, i);
            }
            newsScroll.Content = newsGrid;
        }

        private async void getNews<T>()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + Model.Values[infos.type]["URL"] + model.getID() + "/news{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("news : " + responseRequest);
                newsResponse = JsonConvert.DeserializeObject<NewsResponse>(responseRequest);
                if (newsResponse.status != 200)
                {

                }
                else
                {
                    Debug.WriteLine(newsResponse.response);
                    initNews(newsResponse.response.Count);
                }
            }
            catch (HttpRequestException e)
            {
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(responseRequest);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private async void getNotification<T>()
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
                responseRequest = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseRequest.ToString());
                notifications = JsonConvert.DeserializeObject<NotificationResponse>(responseRequest);
                if (notifications.status != 200)
                {
                    Debug.WriteLine("Failed to get notification");
                }
                else
                {
                    for (int x = 0; x < notifications.response.Count; x++)
                    {
                        if (notifications.response[x].notif_type.Equals("InviteMember", StringComparison.Ordinal)
                            && infos.id.ToString().Equals(notifications.response[x].assoc_id, StringComparison.Ordinal))
                            notif_id = notifications.response[x].id;
                    }
                }
            }
            catch (HttpRequestException e)
            {
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseRequest);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }


        private async void getInformationModel<T>()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + Model.Values[infos.type]["URL"] + model.getID() + "{?token}");
                Debug.WriteLine("ip : " +  template.ToString());
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseRequest = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseRequest.ToString());
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseRequest);
                if ((int)jObject["status"] != 200)
                {
                    informationBox.Text = (string)jObject["message"];
                }
                else
                {
                    jObject = (Newtonsoft.Json.Linq.JObject)jObject["response"];
                    titleBox.Text = Model.Values[infos.type]["Model"];
                    nameBox.Text = (string)jObject[Model.Values[infos.type]["NameType"]];
                    string rights = (string)jObject[Model.Values[infos.type]["RightsType"]];
                    ImageBrush myBrush = new ImageBrush();
                    myBrush.ImageSource =
                        new BitmapImage(new Uri(Global.API_IRL + "" + jObject["thumb_path"], UriKind.Absolute));
                    logo.Fill = myBrush;
                    if (model.isUnknown(rights))
                    {
                        addButton.Visibility = Visibility.Visible;
                    }
                    if (model.isUnknown(rights) || !model.isInRelation(rights))
                    {
                        publishBox.Visibility = Visibility.Collapsed;
                        sendButton.Visibility = Visibility.Collapsed;
                        backgroundSend.Visibility = Visibility.Collapsed;
                        newsScroll.Margin = new Thickness(11, 309, 0, 0);
                        newsScroll.Height = 252;
                    }
                    else if (model.isInRelation(rights))
                    {
                        if (model.isAdmin(rights))
                        {
                            optionButton.Visibility = Visibility;
                            if (Model.Values[infos.type]["Model"].Equals("event", StringComparison.Ordinal)
                                || Model.Values[infos.type]["Model"].Equals("assoc", StringComparison.Ordinal))
                            {
                                privateBox.Visibility = Visibility.Visible;
                                adminBox.Visibility = Visibility.Visible;
                            }
                            newsScroll.Margin = new Thickness(11, 403, 0, 0);
                            newsScroll.Height = 158;
                        }
                        removeButton.Visibility = Visibility.Visible;
                    }
                    else if (model.isInvited(rights))
                    {
                        acceptButton.Visibility = Visibility;
                        refuseButton.Visibility = Visibility;
                        getNotification<T>();
                    }
                    else if (model.isWaiting(rights))
                    {
                        informationBox.Text = "En attente de confirmation";
                    }
                }
            }
            catch (HttpRequestException e)
            {
            }
            catch (JsonReaderException e)
            {
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(responseRequest);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

        }

        public void initProfilPage<T>()
        {
            getInformationModel<T>();
            getNews<T>();
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as InfosModel;
            informationBox.Text = "";
            if (infos.type.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
                initProfilPage<Association>();
            }
            else if (infos.type.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
                initProfilPage<Event>();
            }
        }
    }
}
