using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.Message
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Message : Page
    {
        List<MessageInfos> listMsg;
        Grid msgGrid;

        public void newMessage_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(CreateMessage));
        }

        public void search_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Research));
        }

        void initMessage(Newtonsoft.Json.Linq.JArray list)
        {
            msgGrid = new Grid();
            msgGrid.ColumnDefinitions.Add(new ColumnDefinition());
            msgGrid.VerticalAlignment = VerticalAlignment.Top;
            listMsg = new List<MessageInfos>();
            for (int x = 0; x < list.Count; x++)
            {
                msgGrid.RowDefinitions.Add(new RowDefinition());

                MessageInfos msgInfos = new MessageInfos();
                msgInfos.id = (int)list[x]["id"];
                msgInfos.volunteers = (Newtonsoft.Json.Linq.JArray)list[x]["volunteers"];
                msgInfos.name = (string)list[x]["name"];
                Debug.WriteLine("BEFORE get message name : " + msgInfos.name + " id : " + msgInfos.id);
                if (msgInfos.name == "" || msgInfos.name == null)
                {
                    Newtonsoft.Json.Linq.JArray listVlt = (Newtonsoft.Json.Linq.JArray)list[x]["volunteers"];
                    for (int nbUser = 0; nbUser < listVlt.Count; nbUser++)
                    {
                        string fullname = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["firstname"]
                            + " " + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["lastname"];
                        if (!fullname.Equals((string)listVlt[nbUser], StringComparison.Ordinal))
                        {
                            if (nbUser + 1 == listVlt.Count)
                            {  
                                msgInfos.name += (string)listVlt[nbUser];
                            }
                            else
                            {
                                msgInfos.name += (string)listVlt[nbUser] + " - ";
                            }
                        }
                    }
                }
                if (msgInfos.name.Length >= 17)
                {
                    msgInfos.name = msgInfos.name.Substring(0, 17);
                    msgInfos.name += " ...";
                }
                Debug.WriteLine("AFTER get message name : " + msgInfos.name + " id : " + msgInfos.id);

                All.GUI.Message.ListMessageControl ctls = new All.GUI.Message.ListMessageControl(msgInfos, this, err);
                ctls.Margin = new Thickness(0, 0, 0, 10);
                listMsg.Add(msgInfos);
                Grid.SetColumn(ctls, 0);
                Grid.SetRow(ctls, x);
                msgGrid.Children.Add(ctls);
            }
            scroll.Content = msgGrid;
        }

        private async void getMessage()
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest("/chatrooms", null, HttpHandler.TypeRequest.GET);
            if ((int)obj["status"] != 200)
            {
                Debug.WriteLine((string)obj["message"]);
            }
            else
            {
                initMessage((Newtonsoft.Json.Linq.JArray)obj["response"]);
            }
        }

        public Message()
        {
            this.InitializeComponent();
            getMessage();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
