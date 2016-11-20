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

        private Model model { get; set; }
        private string responseString { get; set; }
        private Grid notifGrid { get; set; }
       // public Newtonsoft.Json.Linq.JArray listObj { get; set; }

        private Dictionary<string, string[] > allowed = new Dictionary<string, string[]>
        {
            { "assoc", new string[] { "JoinAssoc", "NewMember", "Emergency"}},
            { "event", new string[] { "JoinEvent", "NewGuest",  "Emergency"}},
            { "volunteer", new string[] {"JoinAssoc", "JoinEvent", "InviteGuest", "InviteMember", "NewMember", "AddFriend", "Emergency"}},
        };

        public GenericNotification()
        {
            this.InitializeComponent();
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
                    if (!allowed[model.getType()].Contains((string)newsResponse[i]["notif_type"]))
                        continue;
                    string notiftype = (string)newsResponse[i]["notif_type"];
                    //InfosModel tmp = new InfosModel();
                    Model tmp = null;

                    string sender_name = "";
                    switch (notiftype)
                    {
                        case "AddFriend":
                            tmp = new Volunteer(model.getID());
                            sender_name = "L'utilisateur " + (string)newsResponse[i]["sender_name"] + " vous a envoye une demande d'ajout.";
                            break;
                        case "JoinAssoc":
                            tmp = new Association((int)newsResponse[i]["assoc_id"]);
                            sender_name = "L'utilisateur " + (string)newsResponse[i]["sender_name"] + " a demande a rejoindre " 
                                + (string)newsResponse[i]["assoc_name"];
                            break;
                        case "JoinEvent":
                            tmp = new Event((int)newsResponse[i]["event_id"]);
                            sender_name = "L'utilisateur " + (string)newsResponse[i]["sender_name"] + " a demande a participer a "
                                + (string)newsResponse[i]["event_name"];
                            break;
                        case "InviteMember":
                            tmp = new Volunteer(model.getID());
                            sender_name = "Vous avez ete invite a rejoindre l'association " + (string)newsResponse[i]["assoc_name"];
                            break;
                        case "InviteGuest":
                            tmp = new Association(model.getID());
                            sender_name = "Vous avez ete invite a participer a l'evenement " + (string)newsResponse[i]["event_name"];
                            break;
                        case "NewMember":
                            tmp = new Volunteer((int)newsResponse[i]["sender_id"]);
                            sender_name = "L'utilisateur " + (string)newsResponse[i]["sender_name"] + " a rejoint l'association "
                                + (string)newsResponse[i]["assoc_name"];                     
                            break;
                        case "NewGuest":
                            tmp = new Volunteer((int)newsResponse[i]["sender_id"]);
                            sender_name = "L'utilisateur " + (string)newsResponse[i]["sender_name"] + " participe a  'evenement "
                                + (string)newsResponse[i]["event_name"];
                            break;
                        case "Emergency":
                            tmp = new Event((int)newsResponse[i]["event_id"]);
                            sender_name = "L'utilisateur " + (string)newsResponse[i]["sender_name"] + " a envoye une demande d'urgence pour "
                                + (string)newsResponse[i]["event_name"];
                            break;
                    }

                    newsGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.Notification btn = new GUI.Notification((Newtonsoft.Json.Linq.JObject)(newsResponse[i]), tmp, sender_name, this);
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
            model = e.Parameter as Model;
            getNotifications();

        }
    }
}
