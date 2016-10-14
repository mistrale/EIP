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
        class Notification
        {
            public string type { get; set; }
        }

        class MessageResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<MessageInfos> response { get; set; }
        }

        List<MessageInfos> listMsg;
        static private MessageWebSocket messageWebSocket;
        static private DataWriter messageWriter;
        private Notification notif;
        Grid msgGrid;

        public void newMessage_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(CreateMessage));
        }

        public void search_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Research));
        }

        private void GoToMessageClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag.ToString());
            MessageInfos infos = listMsg[i];
            Frame.Navigate(typeof(MessageProfil), infos);
        }

        void initMessage(Newtonsoft.Json.Linq.JArray list)
        {
            msgGrid = new Grid();
            msgGrid.ColumnDefinitions.Add(new ColumnDefinition());
            msgGrid.VerticalAlignment = VerticalAlignment.Top;
            msgGrid.Height = list.Count * 100;
            msgGrid.Width = 380;
            listMsg = new List<MessageInfos>();
            for (int x = 0; x < list.Count; x++)
            {
                msgGrid.RowDefinitions.Add(new RowDefinition());
                MessageInfos msgInfos = new MessageInfos();
                msgInfos.id = (int)list[x]["id"];
                Button title = new Button();
                title.Height = 100;
                msgInfos.name = (string)list[x]["name"];
                if (msgInfos.name == null)
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
                else
                {

                }
                if (msgInfos.name.Length >= 17)
                {
                    msgInfos.name = msgInfos.name.Substring(0, 17);
                    msgInfos.name += " ...";
                }
                title.Content = msgInfos.name + " ( " + (int)list[x]["number_volunteers"] + " participants )";
                title.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));
                title.HorizontalContentAlignment = HorizontalAlignment.Left;
                title.Width = 380;
                title.Margin = new Thickness(0, 0, 0, 10);
                title.Tag = x;
                title.Click += new RoutedEventHandler(GoToMessageClick);
                listMsg.Add(msgInfos);
                Grid.SetColumn(title, 0);
                Grid.SetRow(title, x);
                msgGrid.Children.Add(title);
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

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    notif = JsonConvert.DeserializeObject<Notification>(read);
                    Debug.WriteLine("DATA MESSQGE : " + read);
                    if (notif.type.Equals("notification", StringComparison.Ordinal))
                    {
                        //updateGUI();
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
