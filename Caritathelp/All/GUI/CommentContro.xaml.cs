using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class CommentContro : UserControl
    {
        private GUI.PopField options;
        private GUI.ErrorControl err;
        private int idComment;
        private Page page;

        public void optionsClick(object sender, RoutedEventArgs e)
        {
            options.Visibility = Visibility.Visible;
            Newtonsoft.Json.Linq.JObject obj = new Newtonsoft.Json.Linq.JObject();
            obj.Add("id", idComment);
            obj.Add("content", commentBox.Text);
            obj.Add("url", "/comments/");
            options.setObject(obj);
        }

        public CommentContro(Newtonsoft.Json.Linq.JObject obj, GUI.PopField e, GUI.ErrorControl err, Page page)
        {
            this.InitializeComponent();
            this.page = page;
            options = e;
            this.err = err;
            userTitle.Text = (string)obj["firstname"] + " " + (string)obj["lastname"];
            commentBox.Text = (string)obj["content"];
            ImageBrush image2 = new ImageBrush();
            image2.ImageSource = new BitmapImage(new Uri("http://staging.caritathelp.me" + (string)obj["thumb_path"], UriKind.Absolute));
            myLogo.Fill = image2;
            button.Tag = new Models.Volunteer((int)obj["volunteer_id"]);
            idComment = (int)obj["id"];
            if ((int)obj["volunteer_id"] != (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"])
            {
                optionsButton.Visibility = Visibility.Collapsed;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button tb = (Button)sender;
            Models.Model tmp = (Models.Model)tb.Tag;
            page.Frame.Navigate(typeof(Models.GenericProfil), tmp);
        }
    }
}
