using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class GenericNotification : Page
    {
        class InfosNotif
        {
            public string id { get; set; }
            public string typeNotif { get; set; }
        }

        private InfosModel infos { get; set; }
        private Model model { get; set; }
        private string responseString { get; set; }
        private Grid notifGrid { get; set; }
       // public Newtonsoft.Json.Linq.JArray listObj { get; set; }

        private Dictionary<string, string[] > allowed = new Dictionary<string, string[]>
        {
            { "assoc", new string[] {"InviteMember", "NewMember"}},
            { "event", new string[] {"InviteGuest", "NewGuest"}},
            { "volunteer", new string[] {"JoinAssoc", "JoinEvent","InviteMember", "NewMember", "AddFriend"}},
        };

        public GenericNotification()
        {
            this.InitializeComponent();
        }

        private void ModelButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            InfosModel tmp = (InfosModel)button.Tag;
            Frame.Navigate(typeof(GenericProfil), tmp);
        }

        public async void getNotifications()
        {
            var http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/notifications", null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] == 200)
            {
                Newtonsoft.Json.Linq.JArray newsResponse = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                Grid newsGrid = new Grid();
                newsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                newsGrid.VerticalAlignment = VerticalAlignment.Top;
                for (int i = 0; i < newsResponse.Count; i++)
                {
                    if (!allowed[infos.type].Contains((string)newsResponse[i]["notif_type"]))
                        continue;

                    newsGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.Notification btn = new GUI.Notification((Newtonsoft.Json.Linq.JObject)(newsResponse[i]), this, typeof(GenericNotification), null);
                    btn.Margin = new Thickness(0, 0, 0, 20);
                    Grid.SetColumn(btn, 0);
                    Grid.SetRow(btn, i);
                    newsGrid.Children.Add(btn);
                }
                scroll.Content = newsGrid;
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as InfosModel;

            if (infos.type.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
            }
            else if (infos.type.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
            }
            getNotifications();

        }
    }
}
