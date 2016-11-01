using System;
using System.Collections.Generic;
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
    public sealed partial class InputBox : UserControl
    {
        int id;
        ErrorControl err;

        public async void cancel(object send, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        public async void confirmeSender(object send, RoutedEventArgs e)
        {
            if (textBox.Text.Equals("") || textBox.Text.Equals("Nombre de volontaires a inviter")
                || textBox2.Text.Equals("") || textBox2.Text.Equals("Distance maximum(km2)"))
            {
                return;
            }

            var url = "/events/" + id + "/raise_emergency";
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("number_volunteers", textBox.Text),
                        new KeyValuePair<string, string>("zone", textBox2.Text)
                    }; Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] == 200)
            {
                err.printMessage("Demandes envoyees a " + (Newtonsoft.Json.Linq.JArray)jObject["response"] + " volontaires", GUI.ErrorControl.Code.SUCCESS);
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        public void setIdEvent(int id, ErrorControl err)
        {
            this.id = id;
            this.err = err;
        }

        public InputBox()
        {
            this.InitializeComponent();
        }
    }
}
