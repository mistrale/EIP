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
        Models.InfosModel infos;
        Page page;
        object parameter;


        public enum NotificationType
        {
            ResponseNotification,
            InformationNotification,
        }


        public Dictionary<string, Dictionary<string, string>> d = new Dictionary<string, Dictionary<string, string>>
        {
            {"JoinAssoc", new Dictionary<string, string>
                {
                    { "URL", "/membership/reply_member"},
                    { "Content", "L'utilisateur a demande a rejoindre l'association"}, // invitation
                }
            },
            {"InviteMember", new Dictionary<string, string>
                {
                    { "URL", "/membership/reply_invite"},
                    { "Content", "Vous avez ete invite a rejoindre l'association "}, // invitation
                }
            },
            {"JoinEvent", new Dictionary<string, string>
                {
                    { "URL", "/guests/reply_guest"},
                    { "Content", "L'utilisateur a demande a rejoindre l'evemement"}, // invitation
                }
            },
            {"InviteGuest", new Dictionary<string, string>
                {
                    { "URL", "/guests/reply_invite"},
                    { "Content", "Vous avez ete invite a rejoindre l'evenement "}, // invitation
                }
            },
            { "AddFriend", new Dictionary<string, string>
                {
                    {  "URL", "/friendship/reply"},
                    {  "Content", "L'utilisateur vous a envoye une demande d'invitation"}, // invitation
                }
             },
             {"NewMember", new Dictionary<string, string>
                {
                    { "URL", ""},
                    { "Content", "L'utilisateur a rejoint l'association"}, // rien
                }
            },
            {"NewGuest", new Dictionary<string, string>
                {
                    { "URL", ""},
                    { "Content", "L'utilisateur a rejoint l'evenement"}, //
                }
            },

        };


        public Notification(Newtonsoft.Json.Linq.JObject obj, Models.InfosModel infos, string sender_name, Page page)
        {
            this.InitializeComponent();
            string type = (string)obj["notif_type"];

            if (type.Equals("NewMember", StringComparison.Ordinal)
                || type.Equals("NewGuest", StringComparison.Ordinal))
            {
                button.Click += ProfilClick;
                Debug.WriteLine("OKKKK");
            }
            else
            {
                button.Click += button_Click;
            }
            contentBox.Text = sender_name;
            button.Tag = infos;

            button.Content = (string)obj["sender_name"];
            this.page = page;



            url = (d[(string)obj["notif_type"]])["URL"];
            content = "(" + Convert.ToDateTime((string)obj["created_at"]).ToString() + ") : " + (d[(string)obj["notif_type"]])["Content"];
            contentBox.Text = content;
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + "" + obj["sender_thumb_path"], UriKind.Absolute));
            logo.Fill = myBrush;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Models.InfosModel tmp = (Models.InfosModel)btn.Tag;
            page.Frame.Navigate(typeof(Models.GenericInvitation), tmp);
        }

        private void ProfilClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Models.InfosModel tmp = (Models.InfosModel)btn.Tag;
            page.Frame.Navigate(typeof(Models.GenericProfil), tmp);
        }
    }
}
