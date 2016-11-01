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
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.Message
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessageProfil : Page
    {
        class Notification
        {
            public string type { get; set; }
            public int chatroom_id { get; set; }
            public string sender_firstname { get; set; }
            public string sender_lastname { get; set; }
            public string content { get; set; }
            public string sender_thumb_path { get; set; }
            public string created_at { get; set; }
            public int sender_id { get; set; }

        }

        private Notification notif;
        private MessageWebSocket messageWebSocket;
        private DataWriter messageWriter;
        private MessageInfos infos { get; set; }
        Grid msgGrid;

        public MessageProfil()
        {
            this.InitializeComponent();
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 146, 142));
        }

        public void sendMessageClick(object sender, RoutedEventArgs e)
        {
            if (!msgBox.Text.Equals("", StringComparison.Ordinal)
                && !msgBox.Text.Equals("Votre message ...", StringComparison.Ordinal))
            {
                sendMessage();
                msgBox.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 191, 202, 204));
            }
        }

        private async void updateMessageGUI()
        {
            Debug.WriteLine("testasdasdasd");
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Newtonsoft.Json.Linq.JObject tmp = new Newtonsoft.Json.Linq.JObject();
                tmp["id"] = notif.chatroom_id;
                tmp["volunteer_id"] = notif.sender_id;
                tmp["thumb_path"] = notif.sender_thumb_path;
                tmp["fullname"] = notif.sender_firstname + " " + notif.sender_lastname;
                tmp["created_at"] = notif.created_at;
                tmp["content"] = notif.content;
                msgGrid.RowDefinitions.Add(new RowDefinition());

                All.GUI.Message.MessageControl ctls = new All.GUI.Message.MessageControl((Newtonsoft.Json.Linq.JObject)tmp, this);
                ctls.Margin = new Thickness(0, 0, 0, 10);

                Grid.SetColumn(ctls, 0);
                Grid.SetRow(ctls, msgGrid.RowDefinitions.Count - 1);
                msgGrid.Children.Add(ctls);

                scroll.UpdateLayout();
                scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
            });
        }

        private async void sendMessage()
        {
            string url =  "/chatrooms/" + infos.id + "/new_message";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("id", infos.id.ToString()),
                        new KeyValuePair<string, string>("content", msgBox.Text),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest(url, values, HttpHandler.TypeRequest.PUT);
            if ((int)obj["status"] != 200)
            {
                Debug.WriteLine((string)obj["message"]);
            }
            else
            {
                msgBox.Text = "Votre message ...";
            }
        }


        public void search_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Research));
        }

        private void initMessage(Newtonsoft.Json.Linq.JArray list)
        {
            msgGrid = new Grid();
            msgGrid.ColumnDefinitions.Add(new ColumnDefinition());
            msgGrid.VerticalAlignment = VerticalAlignment.Top;
            for (int x = 0; x < list.Count; x++)
            {
                msgGrid.RowDefinitions.Add(new RowDefinition());

                All.GUI.Message.MessageControl ctls = new All.GUI.Message.MessageControl((Newtonsoft.Json.Linq.JObject)list[x], this);
                ctls.Margin = new Thickness(0, 0, 0, 10);

                Grid.SetColumn(ctls, 0);
                Grid.SetRow(ctls, x);
                msgGrid.Children.Add(ctls);

            }
            scroll.Content = msgGrid;
            scroll.UpdateLayout();
            scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
        }

        private async void getMessage(MessageInfos id)
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest("/chatrooms/" + infos.id, null, HttpHandler.TypeRequest.GET);
            if ((int)obj["status"] != 200)
            {
                Debug.WriteLine((string)obj["message"]);
            }
            else
            {
                initMessage((Newtonsoft.Json.Linq.JArray)obj["response"]);
            }
        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    notif = JsonConvert.DeserializeObject<Notification>(read);
                    Debug.WriteLine("UPDATE GUI Name : " + infos.name + " id : " + infos.id);

                    Debug.WriteLine("DATA MESSQGE IN MESSAGE : " + read);
                    Debug.WriteLine("chat id : " + infos.id + " chat id notif : " + notif.chatroom_id);
                    if (notif.type.Equals("message", StringComparison.Ordinal)
                        && notif.chatroom_id == infos.id)
                    {
                        
                        updateMessageGUI();
                    }

                }
            }
            catch (Exception ex) // For debugging
            {
                Debug.WriteLine(ex.Message);
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                // Add your specific error-handling code here.
            }
        }

        private void Closed(IWebSocket sender, WebSocketClosedEventArgs args)
        {
            // You can add code to log or display the code and reason
            // for the closure (stored in args.Code and args.Reason)

            // This is invoked on another thread so use Interlocked 
            // to avoid races with the Start/Close/Reset methods.
            MessageWebSocket webSocket = Interlocked.Exchange(ref messageWebSocket, null);
            if (webSocket != null)
            {
                Debug.WriteLine("Disconnect");
                webSocket.Dispose();
            }
        }

        private async void startListening()
        {
            try
            {
                // Make a local copy to avoid races with Closed events.
                MessageWebSocket webSocket = messageWebSocket;

                // Have we connected yet?
                if (webSocket == null)
                {
                    Uri server = new Uri(Global.WS_URL);

                    webSocket = new MessageWebSocket();
                    // MessageWebSocket supports both utf8 and binary messages.
                    // When utf8 is specified as the messageType, then the developer
                    // promises to only send utf8-encoded data.
                    webSocket.Control.MessageType = SocketMessageType.Utf8;
                    // Set up callbacks
                    webSocket.MessageReceived += MessageReceived;
                    webSocket.Closed += Closed;
                    await webSocket.ConnectAsync(server);
                    messageWebSocket = webSocket; // Only store it after successfully connecting.
                    messageWriter = new DataWriter(webSocket.OutputStream);
                    // Buffer any data we want to send.*
                    string message = "{ \"token\" : \"token\", \"user_uid\" : \"" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["mail"] + "\"}";
                    messageWriter.WriteString(message);

                    // Send the data as one complete message.
                    await messageWriter.StoreAsync();
                    Debug.WriteLine("Connection ok");
                }
            }
            catch (Exception ex) // For debugging
            {
                Debug.WriteLine(ex.Message);
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(status);
                // Add your specific error-handling code here.
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as MessageInfos;
            Debug.WriteLine("Name : " + infos.name + " id : " + infos.id);
            getMessage(infos);
            nameBox.Text = infos.name;
            startListening();
        }
    }
}
