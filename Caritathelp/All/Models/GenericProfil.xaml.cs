using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556



namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericProfil : Page
    {
        private Grid newsGrid;
        private Newtonsoft.Json.Linq.JArray newsResponse;
        private InfosModel infos;
        private Model model;
        private string notif_id;
        private bool isAdmin;

        public GenericProfil()
        {
            this.InitializeComponent();
        }


        public void infosClick(object send, RoutedEventArgs e)
        {
            FormModel tmp = new FormModel();
            tmp.id = infos.id;
            tmp.createdModelType = infos.type;
            tmp.isCreation = false;
            tmp.modelType = infos.type;
            tmp.isAdmin = isAdmin;
            Frame.Navigate(typeof(GenericCreationModel), tmp);
        }

        public void optionsClick(object send, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GenericManagement), infos);
        }

        public void eventClick(object send, RoutedEventArgs e)
        {
            InfosListModel tmp = new InfosListModel();
            tmp.id = infos.id;
            tmp.typeModel = infos.type;
            tmp.listTypeModel = "event";
            Frame.Navigate(typeof(GenericListModel), tmp);
        }

        public void relatedClick(object send, RoutedEventArgs e)
        {
            InfosListModel tmp = new InfosListModel();
            tmp.id = infos.id;
            tmp.typeModel = infos.type;
            tmp.listTypeModel = "user";
            Frame.Navigate(typeof(GenericListModel), tmp);
        }

        public void acceptInvitationClick(object send, RoutedEventArgs e)
        {
            accepteInvit();
        }

        public async void RefuseInvitationClick(object send, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("notif_id", notif_id),
                        new KeyValuePair<string, string>("acceptance", "false")
                    }; Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.type]["AcceptURL"], values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] == 200)
            {
                Frame.Navigate(typeof(GenericProfil), infos);
            } else
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        private async void accepteInvit()
        {
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("notif_id", notif_id),
                        new KeyValuePair<string, string>("acceptance", "true")
                    }; Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.type]["AcceptURL"], values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] == 200)
            {
                Frame.Navigate(typeof(GenericProfil), infos);
            }
            else
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        public async void removeClick(object send, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.type]["RemoveURL"]
                + "?" + Model.Values[infos.type]["TypeID"] + "=" + infos.id, null, HttpHandler.TypeRequest.DELETE);
            if ((int)jObject["status"] == 200)
            {
                Frame.Navigate(typeof(GenericProfil), infos);
            }
            else
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        public async void addClick(object send, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(Model.Values[infos.type]["TypeID"], infos.id.ToString())
                };
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.type]["AddURL"], values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] == 200)
            {
                Frame.Navigate(typeof(GenericProfil), infos);
            }
            else
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        private async void sendClick(object sender, RoutedEventArgs e)
        {
            if (!publishBox.Text.Equals("Publier un message", StringComparison.Ordinal)
                && !publishBox.Text.Equals("", StringComparison.Ordinal))
            {
                HttpHandler http = HttpHandler.getHttp();
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("content", publishBox.Text),
                        new KeyValuePair<string, string>("group_id", infos.id.ToString()),
                        new KeyValuePair<string, string>("news_type", "Status"),
                        new KeyValuePair<string, string>("group_type",Model.Values[infos.type]["Model"]),
                        new KeyValuePair<string, string>("as_group",  adminBox.IsChecked.ToString().ToLower()),
                        new KeyValuePair<string, string>("private",  privateBox.IsChecked.ToString().ToLower()),
                };
                Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/news/wall_message", values, HttpHandler.TypeRequest.POST);
                if ((int)jObject["status"] == 200)
                {
                    Frame.Navigate(typeof(GenericProfil), infos);
                }
                else
                {
                    errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
                }
            }
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
        }

        private async void getNews()
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.type]["URL"] + model.getID() + "/news", null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] == 200)
            {
                newsResponse = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                newsGrid = new Grid();
                newsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                newsGrid.VerticalAlignment = VerticalAlignment.Top;
                for (int i = 0; i < newsResponse.Count; i++)
                {
                    newsGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.NewControle btn = new GUI.NewControle((Newtonsoft.Json.Linq.JObject)(newsResponse[i]), optionsComment, errControl);


                    Grid.SetColumn(btn, 0);
                    Grid.SetRow(btn, i);
                    newsGrid.Children.Add(btn);
                }
                scroll.Content = newsGrid;
            } else
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        private async void getNotification()
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/notifications", null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                Newtonsoft.Json.Linq.JArray notifications = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                for (int x = 0; x < notifications.Count; x++)
                {
                    if (((string)notifications[x]["notif_type"]).Equals("InviteMember", StringComparison.Ordinal)
                        && infos.id.ToString().Equals((string)notifications[x]["assoc_id"], StringComparison.Ordinal))
                        notif_id = (string)notifications[x]["id"];
                }
            }
        }


        private async void getInformationModel()
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.type]["URL"] + model.getID(), null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                jObject = (Newtonsoft.Json.Linq.JObject)jObject["response"];
                titleBox.Text = Model.Values[infos.type]["Model"];
                nameBox.Text = (string)jObject[Model.Values[infos.type]["NameType"]];
                string rights = (string)jObject[Model.Values[infos.type]["RightsType"]];
                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource =
                    new BitmapImage(new Uri(Global.API_IRL + "" + jObject["thumb_path"], UriKind.Absolute));
                logo.Fill = myBrush;
                if (model.isUnknown(rights))
                {
                    addButton.Visibility = Visibility.Visible;
                }
                if (model.isUnknown(rights) || !model.isInRelation(rights))
                {
                    publishGrid.Visibility = Visibility.Collapsed;
                    scroll.Margin = new Thickness(12, 273, 0, 0);
                    scroll.Height = 318;
                }
                else if (model.isInRelation(rights))
                {
                    
                    if (model.isAdmin(rights))
                    {
                        isAdmin = true;
                        optionButton.Visibility = Visibility;
                        if (Model.Values[infos.type]["Model"].Equals("event", StringComparison.Ordinal)
                            || Model.Values[infos.type]["Model"].Equals("assoc", StringComparison.Ordinal))
                        {
                            adminBox.Visibility = Visibility.Visible;
                        }
                    }
                    removeButton.Visibility = Visibility.Visible;
                }
                if (model.isInvited(rights))
                {
                    acceptButton.Visibility = Visibility;
                    refuseButton.Visibility = Visibility;
                    getNotification();
                }
                if (model.isWaiting(rights))
                {
                    waitingImage.Visibility = Visibility.Visible;
                }
            }
        }

        public void initProfilPage()
        {
            isAdmin = false;
            getInformationModel();
            getNews();
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as InfosModel;
            //informationBox.Text = "";
            if (infos.type.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
                associationButton.Visibility = Visibility.Collapsed;
            }
            else if (infos.type.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
            }
            initProfilPage();
            optionsComment.setCurrentPage(this, typeof(GenericProfil), infos, errControl);
        }
    }
}
