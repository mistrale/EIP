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
    public sealed partial class InvitationWaiting : UserControl
    {
        Models.Model infos;
        private int idNotif;
        private int idUser;
        private ConfirmBox cfBox;

        private void viewProfil(object sender, RoutedEventArgs e)
        {
            Models.Model tmp = new Models.Volunteer(idUser);
            ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericProfil), tmp);
        }

        private void cancelRequest(object sender, RoutedEventArgs e)
        {
            cfBox.Visibility = Visibility.Visible;
            cfBox.setRoutedEvent(cancelRequest_real);
        }

        private async void cancelRequest_real(object sender, RoutedEventArgs e)
        {

            HttpHandler http = HttpHandler.getHttp();
            string url = "";
            if (infos.getType().Equals("volunteer", StringComparison.Ordinal))
            {
                url = Models.Model.Values[infos.getType()]["CancelInviteURL"] + "?notif_id=" + idNotif;
            } else
            {
                url = Models.Model.Values[infos.getType()]["CancelInviteURL"] + "?" + Models.Model.Values[infos.getType()]["CancelTypeID"]
                    + "=" + idNotif.ToString() + "&" + Models.Model.Values[infos.getType()]["TypeID"] + "=" + infos.id.ToString();
            }
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.DELETE);
            if ((int)jObject["status"] != 200)
            {
                Debug.WriteLine(jObject["message"]);
            }
            else
            {
                ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericInvitation), infos);
            }
        }

        public InvitationWaiting(Models.Model tmp, Newtonsoft.Json.Linq.JObject obj, ConfirmBox cfBox)
        {
            this.InitializeComponent();
            this.cfBox = cfBox;
            if (obj["notif_id"] != null)
            {
                idNotif = (int)obj["notif_id"];
            }else
            {
                idNotif = (int)obj["id"];
            }
            idUser = (int)obj["id"];
            Debug.WriteLine("id notif : " + idNotif);
            infos = tmp;
            contentButton.Content = (string)obj["fullname"];
            contentButton.Tag = infos;

            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + "" + obj["thumb_path"], UriKind.Absolute));
            logo.Fill = myBrush;
        }

        public InvitationWaiting()
        {
            this.InitializeComponent();

        }
    }
}
