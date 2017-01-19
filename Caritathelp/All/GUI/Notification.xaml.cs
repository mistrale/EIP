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
        Page page;
        private ConfirmBox conf;
        private ErrorControl err;


        public enum NotificationType
        {
            ResponseNotification,
            InformationNotification,
        }

        public Notification(Newtonsoft.Json.Linq.JObject obj, Models.Model infos, string sender_name, Page page, ConfirmBox confir, ErrorControl err)
        {
            this.InitializeComponent();
            string type = (string)obj["notif_type"];
            idNotif = (int)obj["id"];
            conf = confir;
            this.err = err;
            if (type.Equals("NewMember", StringComparison.Ordinal)
                || type.Equals("NewGuest", StringComparison.Ordinal)
                || type.Equals("InviteMember", StringComparison.Ordinal)
                || type.Equals("InviteGuest", StringComparison.Ordinal))
            {
                button.Click += ProfilClick;
                Debug.WriteLine("OKKKK");
            }
            else if (type.Equals("Emergency", StringComparison.Ordinal))
            {
                button.Click += emergency_Click;
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

        private void emergency_Click(object sender, RoutedEventArgs e)
        {
            conf.Visibility = Visibility.Visible;
            conf.setRoutedEvent(emergency_confirm, "Voulez-vous confirmer votre présence à cet évènement ?");
        }


        private async void emergency_confirm(object sender, RoutedEventArgs e) {
            HttpHandler http = HttpHandler.getHttp();
            Debug.WriteLine("on test ici ");
            string url = "/notifications/" + idNotif + "/reply_emergency";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("id", idNotif.ToString()),
                        new KeyValuePair<string, string>("accept", "true")
                    };

            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.PUT);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], ErrorControl.Code.FAILURE);
            } else
            {
                err.printMessage((string)jObject["response"], ErrorControl.Code.SUCCESS);
            }
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
