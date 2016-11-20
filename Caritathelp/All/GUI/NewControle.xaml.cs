using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI
{
    public sealed partial class NewControle : UserControl
    {
        private GUI.PopField options;
        private GUI.ErrorControl err;
        private Grid commentsGrid;
        private int idNews;
        private int nbComments = 0;
        private Page page;

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
        }

        public async void sendCommentClick(object sender, RoutedEventArgs e)
        {
            if (!commentBox.Text.Equals("Votre commentaire ...", StringComparison.Ordinal)
                           && !commentBox.Text.Equals("", StringComparison.Ordinal))
            {
                var http = HttpHandler.getHttp();
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("new_id", idNews.ToString()),
                        new KeyValuePair<string, string>("content", commentBox.Text)
                    };
                string url = ("/comments");
                Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.POST);
                if ((int)jObject["status"] != 200)
                {
                    err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
                    return;
                }
                nbComments++;
                commentsGrid.RowDefinitions.Add(new RowDefinition());
                GUI.CommentContro cmt = new CommentContro((Newtonsoft.Json.Linq.JObject)(jObject["response"]), options, err, page);

                Grid.SetColumn(cmt, 0);
                Grid.SetRow(cmt, commentsGrid.RowDefinitions.Count - 1);
                commentsGrid.Children.Add(cmt);
                scroll.Height += 54;
                this.Height += 54;
                seeButton.Content = "Voir les commentaires (" + +nbComments + ")";
                commentBox.Text = "Votre commentaire ...";
            }
        }

        public void seeCommentClick(object sender, RoutedEventArgs e)
        {
            scroll.Height += nbComments * 54;
            this.Height += nbComments * 54;
        }

        public void optionsClick(object sender, RoutedEventArgs e)
        {
            options.Visibility = Visibility.Visible;
            Newtonsoft.Json.Linq.JObject obj = new Newtonsoft.Json.Linq.JObject();
            obj.Add("id", idNews);
            obj.Add("content", content.Text);
            obj.Add("url", "/news/");
            options.setObject(obj);
        }

        private async void getComments(GUI.PopField e, GUI.ErrorControl err)
        {
            HttpHandler http = HttpHandler.getHttp();
            string url = "/news/" + idNews + "/comments";
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
                return;
            }
            Newtonsoft.Json.Linq.JArray listObj = (Newtonsoft.Json.Linq.JArray)jObject["response"];

            commentsGrid = new Grid();
            commentsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            commentsGrid.VerticalAlignment = VerticalAlignment.Top;
            for (int x = 0; x < listObj.Count; x++)
            {
                commentsGrid.RowDefinitions.Add(new RowDefinition());
                GUI.CommentContro cmt = new CommentContro((Newtonsoft.Json.Linq.JObject)(listObj[x]), options, err, page);

                nbComments++;
                Grid.SetColumn(cmt, 0);
                Grid.SetRow(cmt, x);
                commentsGrid.Children.Add(cmt);
            }
            scroll.Content = commentsGrid;
        }

        public NewControle()
        {
            this.InitializeComponent();
        }

        public NewControle(Newtonsoft.Json.Linq.JObject obj, GUI.PopField e, GUI.ErrorControl err, Page page)
        {
            this.InitializeComponent();
            options = e;
            this.err = err;
            string name = null;
            string thumb_path = null;
            idNews = (int)obj["id"];
            if ((bool)obj["as_group"] == true)
            {
                name = (string)obj["group_name"];
                thumb_path = (string)obj["group_thumb_path"];
            }
            else if ((int)obj["volunteer_id"] != (int)obj["group_id"])
            {
                thumb_path = (string)obj["volunteer_thumb_path"];
                name = obj["volunteer_name"] + " > " + obj["group_name"];
            }
            else
            {
                thumb_path = (string)obj["volunteer_thumb_path"];
                name = (string)obj["group_name"];
            }
            button.Tag = Models.Model.createModel(((string)obj["group_type"]).ToLower(), (int)obj["group_id"]);
            title.Text = name;

            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri("http://staging.caritathelp.me" + thumb_path, UriKind.Absolute));
            logo.Fill = image;
            this.page = page;
            date.Text = Convert.ToDateTime((string)obj["created_at"]).ToString();
            try
            {
                content.Text = (string)obj["content"];
            } catch (Exception excp)
            {
                content.Text = "";
            }

            if ((int)obj["volunteer_id"] != (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"])
            {
                optionButton.Visibility = Visibility.Collapsed;
            }

            ImageBrush image2 = new ImageBrush();
            image2.ImageSource = new BitmapImage(new Uri("http://staging.caritathelp.me" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["thumb_path"], UriKind.Absolute));
            myLogo.Fill = image2;
            seeButton.Content += " (" + +(int)obj["number_comments"] + ")";
            getComments(e, err);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button tb = (Button)sender;
            Models.Model tmp = (Models.Model)tb.Tag;
            page.Frame.Navigate(typeof(Models.GenericProfil), tmp);

        }
    }
}
