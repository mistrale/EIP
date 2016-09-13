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
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accueil : Page
    {
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

        class CommentsResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<Comments> response { get; set; }
        }

        class PublishResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public Publication response { get; set; }
        }

        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        private SimpleRequest simple;
        Grid newsGrid;
        private PublishResponse publishreponse;
        private CommentsResponse commentsResponse;
        private CommentResponse commentResponse;
        private string responseString;
        private NewsResponse newsResponse;

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void updateComment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            commentInfos cmt = (commentInfos)button.Tag;
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == cmt.row_news));

            Grid gridComment = (Grid)grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4);
            Grid oneComment = (Grid)gridComment.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == cmt.row_cmt);
            string text = ((TextBox)(oneComment.Children.Cast<FrameworkElement>()
                .FirstOrDefault(x => Grid.GetRow(x) == 0 && Grid.GetColumn(x) == 2))).Text;
            Debug.WriteLine("on update avec :  " + text + " d id : " + cmt.id_comments );
            //string text = (TextBox)
            UpdateComment(cmt, text);
        }

        public void deleteComment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            commentInfos cmt = (commentInfos)button.Tag;
            DeleteComment(cmt);
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
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                commentResponse = JsonConvert.DeserializeObject<CommentResponse>(responseString);
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
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
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
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(Accueil));
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

        private async void getComments(int idNews, Grid commentGrid, int row, string idVolunteer)
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
                        if (commentsResponse.response[x].volunteer_id != Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]))
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
                            updateNew.Click += updateComment;

                            Grid.SetColumn(updateNew, 1);
                            Grid.SetRow(updateNew, 1);
                            Grid.SetColumnSpan(updateNew, 2);
                            oneComment.Children.Add(updateNew);

                            // delete news
                            Button deleteCmt = new Button();
                            deleteCmt.Content = "Supprimer";
                            deleteCmt.Tag = tmp;
                            deleteCmt.Click += deleteComment;
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
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(responseString);
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private async void comments(string text, int idNews, Grid gridComment, int row_news)
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
                    updateNew.Click += updateComment;

                    Grid.SetColumn(updateNew, 1);
                    Grid.SetRow(updateNew, 1);
                    Grid.SetColumnSpan(updateNew, 2);
                    oneComment.Children.Add(updateNew);

                    // delete news
                    Button deleteCmt = new Button();
                    deleteCmt.Content = "Supprimer";
                    deleteCmt.Tag = tmp;
                    deleteCmt.Click += deleteComment;
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
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Debug.WriteLine("Fail in publish news friends");
            }
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

        public void updateNews(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NewsInfos row = (NewsInfos)(button.Tag);

            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row.id_rows));
            string text = ((TextBox)grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 2)).Text;
            Debug.WriteLine("ON va modifier la news : " + row + " par : " + text);
            UpdateNew(text, row.id_news);
        }

        public void deleteNew(object sender, RoutedEventArgs e)
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
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                simple = JsonConvert.DeserializeObject<SimpleRequest>(responseString);
                if (simple.status != 200)
                {
                    Debug.WriteLine(simple.message);
                }
                else
                {
                    Frame.Navigate(typeof(Accueil));
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
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                publishreponse = JsonConvert.DeserializeObject<PublishResponse>(responseString);
                if (publishreponse.status != 200)
                {
                    informationBox.Text = publishreponse.message;
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
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void publish(object sender, RoutedEventArgs e)
        {
            publishNews();
        }

        private async void publishNews()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]),
                        new KeyValuePair<string, string>("content", publicationText.Text),
                        new KeyValuePair<string, string>("group_id",  (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]),
                        new KeyValuePair<string, string>("news_type", "Status"),
                        new KeyValuePair<string, string>("group_type", "Volunteer"),
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
                    Frame.Navigate(typeof(Accueil));
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

        public void publishComment(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int row = Convert.ToInt32(button.Tag);
            Grid grid = (Grid)(newsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row));

            Grid gridComment = (Grid)(grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 5));
            Grid gridComments = (Grid)(grid.Children.Cast<FrameworkElement>().FirstOrDefault(x => Grid.GetRow(x) == 4));
            TextBox text = (TextBox)(gridComment.Children.Cast<FrameworkElement>().FirstOrDefault(z => Grid.GetColumn(z) == 1));
            comments(text.Text, newsResponse.response[row].id, gridComments, row);
            text.Text = "Votre commentaire ...";
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
                btn.Source = new BitmapImage(new Uri(Global.API_IRL + newsResponse.response[i].groupe_thumb_path, UriKind.Absolute));
      
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                grid.Children.Add(btn);

                // username
                // CHANGE BY HYPERLINK
                TextBlock poster = new TextBlock();
                if (newsResponse.response[i].news_type.Equals("New::Volunteer::FriendWallMessage", StringComparison.Ordinal))
                {
                    poster.Text = newsResponse.response[i].group_name + " a publie : ";
                }
                else
                {
                    poster.Text = newsResponse.response[i].group_name;
                }
                poster.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
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
                seeComment.Click += seeCommentEvent;
                seeComment.Content = newsResponse.response[i].number_comments.ToString() + " commentaires";
                seeComment.Tag = i;
                seeComment.FontSize = 12;
                seeComment.Margin = new Thickness(10, 0, 10, 0);

                Grid.SetColumn(seeComment, 0);
                Grid.SetRow(seeComment, 3);
                Grid.SetColumnSpan(seeComment, 2);
                grid.Children.Add(seeComment);

                // update news
                if (newsResponse.response[i].volunteer_id == (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"])
                {
                    NewsInfos tmp = new NewsInfos();

                    tmp.id_news = newsResponse.response[i].id;
                    tmp.id_rows = i;

                    Button updateNew = new Button();
                    updateNew.Content = "Modifier";
                    updateNew.Tag = tmp;
                    updateNew.FontSize = 12;
                    updateNew.Click += updateNews;

                    Grid.SetColumn(updateNew, 2);
                    Grid.SetRow(updateNew, 3);
                    grid.Children.Add(updateNew);

                    // delete news
                    Button deleteNews = new Button();
                    deleteNews.Content = "Supprimer";
                    deleteNews.Tag = tmp;
                    deleteNews.Click += deleteNew;
                    deleteNews.FontSize = 12;
                    deleteNews.Margin = new Thickness(0, 0, 10, 0);

                    Grid.SetColumn(deleteNews, 3);
                    Grid.SetRow(deleteNews, 3);
                    grid.Children.Add(deleteNews);
                }

                // grid comments 

                Grid commentGrid = new Grid();
                commentGrid.ColumnDefinitions.Add(new ColumnDefinition());
                getComments(newsResponse.response[i].id, commentGrid, i, newsResponse.response[i].volunteer_id);
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
                Grid.SetColumnSpan(addComment, 4);
                Grid.SetRow(addComment, 5);
                grid.Children.Add(addComment);


                grid.Margin = new Thickness(0, 0, 0, 10);
                newsGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, i);
            }
            scroll.Content = newsGrid;
            scroll.Background = new SolidColorBrush(Color.FromArgb(250, 229, 255, 235));
        }

        private async void getNews()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/news{?token}");
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

        public Accueil()
        {
            this.InitializeComponent();
            imageUser.Source = new BitmapImage(new Uri(Global.API_IRL + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["thumb_path"], UriKind.Absolute));
            getNews();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Research));
        }
    }
}
