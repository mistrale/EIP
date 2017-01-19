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
    public sealed partial class ReceivedInviation : UserControl
    {
        Models.Model infos;
        private int idNotif;
        private int idUser;

        private void refuseNotification(object sender, RoutedEventArgs e)
        {
            responseToNotification(false);
        }

        private void acceptNotification(object sender, RoutedEventArgs e)
        {
            responseToNotification(true);
        }

        private void viewProfil(object sender, RoutedEventArgs e)
        {
            Models.Model tmp = new Models.Volunteer(idUser);
            ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericProfil), tmp);
        }


        private async void responseToNotification(bool isTrue)
        {
            Debug.WriteLine("STATUS : " + isTrue);
            var values = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("notif_id", idNotif.ToString()),
                            new KeyValuePair<string, string>("acceptance", isTrue.ToString().ToLower())
                        };
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Models.Model.Values[infos.getType()]["AcceptInvitation"], values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] != 200)
            {
                Debug.WriteLine(jObject["message"]);
            }
            else
            {
                ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericInvitation), infos);
            }
        }

        public ReceivedInviation(Models.Model tmp, Newtonsoft.Json.Linq.JObject obj)
        {
            this.InitializeComponent();
            idNotif = (int)obj["notif_id"];
            idUser = (int)obj["id"];
            Debug.WriteLine("id notif : " + idNotif);
            infos = tmp;
            contentButton.Content = (string)obj["fullname"];
            contentButton.Tag = infos;
            dateBox.Text = Convert.ToDateTime((string)obj["sending_date"]).ToString();

            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + "" + obj["thumb_path"], UriKind.Absolute));
            logo.Fill = myBrush;
        }

        public ReceivedInviation()
        {
            this.InitializeComponent();
        }
    }
}
