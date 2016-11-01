using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericInvitation : Page
    {
        RadioButton tmp;
        Grid receivedGrid;
        Grid sendGrid;

        class InfosNotif
        {
            public string id { get; set; }
            public string typeNotif { get; set; }
        }

        private int idSearch { get; set; }
        private Model model { get; set; }
        private string responseString { get; set; }

        public GenericInvitation()
        {
            this.InitializeComponent();
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = GUI.PolicyGenerator.getBrush(GUI.PolicyGenerator.ColorType.DEFAULT_TEXT);
        }

        private void receivedClick(object sender, RoutedEventArgs e)
        {
            if (tmp != receivedRB)
            {
                tmp.IsChecked = false;
                receivedRB.IsChecked = true;
                tmp = receivedRB;
                getReceivedInvitation();
            }
        }

        private void sendClick(object sender, RoutedEventArgs e)
        {
            if (tmp != sendRB)
            {
                tmp.IsChecked = false;
                sendRB.IsChecked = true;
                tmp = sendRB;
                getSendInvitation();
            }
        }

        private async Task searchUser()
        {
            var template = new UriTemplate("/search{?research}");
            template.AddParameter("research", nameBox.Text);
            var url = template.Resolve();

            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                Newtonsoft.Json.Linq.JArray searchList = (Newtonsoft.Json.Linq.JArray)(jObject["response"]);
                if (searchList.Count > 0)
                {
                    idSearch =  (int)(((Newtonsoft.Json.Linq.JObject)(searchList[0]))["id"]);
                    return;
                } else
                {
                    err.printMessage("Benevole non trouve.", GUI.ErrorControl.Code.FAILURE);
                }
            }
            idSearch=  0;
        }

        private async void inviteVolunteer(object sender, RoutedEventArgs e)
        {
            await searchUser();
            if (idSearch == 0)
            {
                return;
            }
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(Model.Values[model.getType()]["TypeID"], model.getID().ToString()),
                        new KeyValuePair<string, string>("volunteer_id", idSearch.ToString()),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[model.getType()]["InviteURL"], values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] == 200)
            {
                this.Frame.Navigate(typeof(GenericInvitation), model);
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        private async void getReceivedInvitation()
        {
            var http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("TypeID", model.getID().ToString()),
                    };
            string url = Model.Values[model.getType()]["WaitingInvitation"] + "?" + Model.Values[model.getType()]["TypeID"] + "=" + model.getID().ToString();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] == 200)
            {
                Newtonsoft.Json.Linq.JArray newsResponse = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                receivedGrid = new Grid();
                receivedGrid.ColumnDefinitions.Add(new ColumnDefinition());

                receivedGrid.VerticalAlignment = VerticalAlignment.Top;
                for (int i = 0; i < newsResponse.Count; i++)
                {
                    Debug.WriteLine("2");
                    receivedGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.ReceivedInviation btn = new GUI.ReceivedInviation(model, (Newtonsoft.Json.Linq.JObject)(newsResponse[i]));
                    btn.Margin = new Thickness(0, 0, 0, 20);
                    Grid.SetColumn(btn, 0);
                    Grid.SetRow(btn, i);
                    receivedGrid.Children.Add(btn);
                }
                scrollInvit.Content = receivedGrid;
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        private async void getSendInvitation()
        {
            var http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("TypeID", model.getID().ToString()),
                    };
            string url = Model.Values[model.getType()]["SendInvitation"] + "?" + Model.Values[model.getType()]["TypeID"] + "=" + model.getID().ToString();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] == 200)
            {
                Newtonsoft.Json.Linq.JArray newsResponse = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                sendGrid = new Grid();
                sendGrid.ColumnDefinitions.Add(new ColumnDefinition());

                sendGrid.VerticalAlignment = VerticalAlignment.Top;
                for (int i = 0; i < newsResponse.Count; i++)
                {
                    sendGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.InvitationWaiting btn = new GUI.InvitationWaiting(model, (Newtonsoft.Json.Linq.JObject)(newsResponse[i]));
                    btn.Margin = new Thickness(0, 0, 0, 20);
                    Grid.SetColumn(btn, 0);
                    Grid.SetRow(btn, i);
                    sendGrid.Children.Add(btn);
                }
                scrollInvit.Content = sendGrid;
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.InitializeComponent();

            model = e.Parameter as Model;
            tmp = receivedRB;
            getSendInvitation();
            getReceivedInvitation();
            scrollInvit.Content = receivedGrid;
        }
    }
}
