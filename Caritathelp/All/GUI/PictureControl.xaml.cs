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
    public sealed partial class PictureControl : UserControl
    {
        public int id { get; set; }
        public Models.PictureModel infos;
        private ConfirmBox cfBox;

        public PictureControl(Newtonsoft.Json.Linq.JObject jObject,  Models.PictureModel infos, ConfirmBox cf)
        {
            this.InitializeComponent();
            this.cfBox = cf;
            this.id = (int)jObject["id"];
            this.infos = infos;
            if (!infos.isAdmin)
            {
                grid.Visibility = Visibility.Collapsed;
                image.Height = 357;
                image.Margin = new Thickness(10, 10, 0, 0);
            }
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + ((Newtonsoft.Json.Linq.JObject)jObject["picture_path"])["url"], UriKind.Absolute));
            image.Fill = myBrush;
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            cfBox.Visibility = Visibility.Visible;
            cfBox.setRoutedEvent(delete_Click_real);
        }

        private async void delete_Click_real(object sender, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/pictures/" + id.ToString(), null, HttpHandler.TypeRequest.DELETE);
            if ((int)jObject["status"] != 200)
            {
                Debug.WriteLine(jObject["message"]);
            }
            else
            {
                ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericAlbum), infos);
            }
        }

        private async  void defaut_Click(object sender, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(Models.Model.Values[infos.model.getType()]["TypeID"], infos.model.getID().ToString()),
                        new KeyValuePair<string, string>("id", id.ToString()),
                    };
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/pictures/" + id.ToString(), values, HttpHandler.TypeRequest.PUT);
            if ((int)jObject["status"] != 200)
            {
                Debug.WriteLine(jObject["message"]);
            }
            else
            {
                ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericAlbum), infos);
            }
        }
    }
}
