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
    public sealed partial class PopField : UserControl
    {
        public delegate bool Navigate(object content, object extraData);

        private Page currentPage;
        private System.Type type;
        private object parameter;
        private GUI.ErrorControl err;
        private Newtonsoft.Json.Linq.JObject obj;

        public void closeWindow(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        public async void updateClick(object sender, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            string url = (string)obj["url"] + (int)obj["id"];
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("id", ((int)obj["id"]).ToString()),
                        new KeyValuePair<string, string>("content", contentBox.Text),
                    };

            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.PUT);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
                return;
            } else
            {
                currentPage.Frame.Navigate(type, parameter);
            }
        }

        public async void deleteClick(object sender, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest((string)obj["url"] + (int)obj["id"], 
                null, HttpHandler.TypeRequest.DELETE);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
                return;
            }
            currentPage.Frame.Navigate(type, parameter);

        }

        public void setObject(Newtonsoft.Json.Linq.JObject obj)
        {
            this.obj = obj;
            contentBox.Text = (string)obj["content"];
        }

        public void setCurrentPage(Page page, System.Type type, object parameter, GUI.ErrorControl err)
        {
            currentPage = page;
            this.type = type;
            this.err = err;
            this.parameter = parameter;
        }

        public PopField()
        {
            this.InitializeComponent();
        }
    }
}
