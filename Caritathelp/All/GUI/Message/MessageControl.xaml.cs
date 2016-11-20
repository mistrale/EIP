using System;
using System.Collections.Generic;
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

namespace Caritathelp.All.GUI.Message
{
    public sealed partial class MessageControl : UserControl
    {
        Page page;
        Newtonsoft.Json.Linq.JObject obj;

        public MessageControl(Newtonsoft.Json.Linq.JObject obj, Page page)
        {
            this.InitializeComponent();
            this.obj = obj;
            this.page = page;

            if ((int)obj["volunteer_id"] != (int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"])
            {
                container.Background = new SolidColorBrush(Color.FromArgb(0xFF, 255, 255, 255));
                container.Opacity = 1;
            }
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri(Global.API_IRL + (string)obj["thumb_path"], UriKind.Absolute));
            logo.Fill = image;
            userBox.Text = (string)obj["fullname"] + " (" + Convert.ToDateTime((string)obj["created_at"]).ToString() + ")";
            contentBox.Text = (string)obj["content"];
        }

        private void link_Click(object sender, RoutedEventArgs e)
        {
            Models.Model tmp = new Models.Association((int)obj["id"]);
            page.Frame.Navigate(typeof(Models.GenericProfil), tmp);
        }
    }
}
