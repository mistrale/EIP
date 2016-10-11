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
        Page currentPage;
        object parameter;
        System.Type type;

        public enum NotificationType
        {
            ResponseNotification,
            InformationNotification,
        }

        public NotificationType whatNotificationType(string notification)
        {
            string[] response = { "JoinAssoc", "JoinEvent", "AddFriend", "InviteMember", "InviteGuest" };
            string[] infos = { "NewMember", "NewGuest"};
            if (response.Contains(notification))
                return NotificationType.ResponseNotification;
            return NotificationType.InformationNotification;
        }

        public Dictionary<string, Dictionary<string, string>> d = new Dictionary<string, Dictionary<string, string>>
        {
            {"JoinAssoc", new Dictionary<string, string>
                {
                    { "URL", "/membership/reply_member"},
                    { "Content", "L'utilisateur a demande a rejoindre l'association"},
                }
            },
            {"InviteMember", new Dictionary<string, string>
                {
                    { "URL", "/membership/reply_invite"},
                    { "Content", "Vous avez ete invite a rejoindre l'association "},
                }
            },
            {"JoinEvent", new Dictionary<string, string>
                {
                    { "URL", "/guests/reply_guest"},
                    { "Content", "L'utilisateur a demande a rejoindre l'evemement"},
                }
            },
            {"InviteGuest", new Dictionary<string, string>
                {
                    { "URL", "/guests/reply_invite"},
                    { "Content", "Vous avez ete invite a rejoindre l'evenement "},
                }
            },
            { "AddFriend", new Dictionary<string, string>
                {
                    {  "URL", "/friendship/reply"},
                    {  "Content", "L'utilisateur vous a envoye une demande d'invitation"},
                }
             },
             {"NewMember", new Dictionary<string, string>
                {
                    { "URL", ""},
                    { "Content", "L'utilisateur a rejoint l'association"},
                }
            },
            {"NewGuest", new Dictionary<string, string>
                {
                    { "URL", ""},
                    { "Content", "L'utilisateur a rejoint l'evenement"},
                }
            },

        };

        private void refuseNotification(object sender, RoutedEventArgs e)
        {
            responseToNotification(false);
        }

        private void acceptNotification(object sender, RoutedEventArgs e)
        {
            responseToNotification(true);
        }

        private async void responseToNotification(bool isTrue)
        {
            var values = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("notif_id", idNotif.ToString()),
                            new KeyValuePair<string, string>("acceptance", isTrue.ToString().ToLower())
                        };
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] != 200)
            {
                Debug.WriteLine(jObject["message"]);
            }
            else
            {
                currentPage.Frame.Navigate(type, parameter);
            }
        }

        public Notification(Newtonsoft.Json.Linq.JObject obj, Page page, object parameter, System.Type type)
        {
            this.InitializeComponent();
            idNotif = (int)obj["id"];
            Debug.WriteLine("asdsa sa" + (string)obj["notif_type"]);
            url = (d[(string)obj["notif_type"]])["URL"];
            content = (d[(string)obj["notif_type"]])["Content"];
            currentPage = page;
            this.parameter = parameter;
            this.type = type;
            contentBox.Text = (string)obj["sender_name"] + " : " + content;
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + "" + obj["sender_thumb_path"], UriKind.Absolute));
            logo.Fill = myBrush;
            if (whatNotificationType((string)obj["notif_type"]) == NotificationType.InformationNotification) {
                secondCol.Width = new GridLength(0);
                contentBox.Width = 277;
            }
        }
    }
}
