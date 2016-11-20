using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public sealed partial class Notification : UserControl
    {
        private int idNotif;
        private string url;
        private string content;
        private Models.Model model;
        Page page;
        object parameter;


        public enum NotificationType
        {
            ResponseNotification,
            InformationNotification,
        }

        public Notification(Newtonsoft.Json.Linq.JObject obj, Models.Model infos, string sender_name, Page page)
        {
            this.InitializeComponent();
            string type = (string)obj["notif_type"];

            if (type.Equals("NewMember", StringComparison.Ordinal)
                || type.Equals("NewGuest", StringComparison.Ordinal)
                || type.Equals("Emergency", StringComparison.Ordinal))
            {
                button.Click += ProfilClick;
                Debug.WriteLine("OKKKK");
            }
            else
            {
                button.Click += button_Click;
            }
            contentBox.Text = "(" + Convert.ToDateTime((string)obj["created_at"]).ToString() + ") : " + sender_name;
            button.Tag = infos;


            button.Content = (string)obj["sender_name"];
            if ((string)obj["assoc_name"] != null)
            {
                button.Content = (string)obj["assoc_name"];
            }
            if ((string)obj["event_name"] != null)
            {
                button.Content = (string)obj["event_name"];
            }

            this.page = page;
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + "" + obj["sender_thumb_path"], UriKind.Absolute));
            logo.Fill = myBrush;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Models.Model tmp = (Models.Model)btn.Tag;
            page.Frame.Navigate(typeof(Models.GenericInvitation), tmp);
        }

        private void ProfilClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Models.Model tmp = (Models.Model)btn.Tag;
            page.Frame.Navigate(typeof(Models.GenericProfil), tmp);
        }
    }
}
