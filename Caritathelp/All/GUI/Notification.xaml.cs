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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI
{
    public sealed partial class Notification : UserControl
    {
        private int idNotif;
        private string url;
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

        Dictionary<string, string> d = new Dictionary<string, string>()
             {
                {"JoinAssoc",  "/membership/reply_member"},
                {"InviteMember", "/membership/reply_invite"},

                {"JoinEvent",  "/guests/reply_guest"},
                {"InviteGuest", "/guests/reply_invite"},

                { "AddFriend", "/friendship/reply"}
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
            url = d[(string)obj["notif_type"]];
            currentPage = page;
            this.parameter = parameter;
            this.type = type;
            contentBox.Text = (string)obj["sender_name"];
            if (whatNotificationType((string)obj["notif_type"]) == NotificationType.InformationNotification) {
                secondCol.Width = new GridLength(0);
                contentBox.Width = 277;
            }
        }
    }
}
