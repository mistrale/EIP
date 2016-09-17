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

        static private MessageWebSocket messageWebSocket;
        static private DataWriter messageWriter;
        private Notification notif;
        public string responseString;
        private MessageResponse msgResponse;
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
            MessageInfos infos = msgResponse.response[i];
            Frame.Navigate(typeof(MessageProfil), infos);
        }

        void initMessage(int nbMessages)
        {
            msgGrid = new Grid();
            msgGrid.ColumnDefinitions.Add(new ColumnDefinition());
            msgGrid.VerticalAlignment = VerticalAlignment.Top;
            msgGrid.Height = msgResponse.response.Count * 100;
            msgGrid.Width = 380;
            for (int x = 0; x < nbMessages; x++)
            {
                msgGrid.RowDefinitions.Add(new RowDefinition());

                Button title = new Button();
                title.Height = 100;
                if (msgResponse.response[x].name == null)
                {
                    for (int nbUser = 0; nbUser < msgResponse.response[x].volunteers.Count; nbUser++)
                    {
                        string fullname = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["firstname"]
                            + " " + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["lastname"];
                        if (!fullname.Equals(msgResponse.response[x].volunteers[nbUser], StringComparison.Ordinal))
                        {
                            if (nbUser + 1 == msgResponse.response[x].volunteers.Count)
                            {
                                msgResponse.response[x].name += msgResponse.response[x].volunteers[nbUser];
                            }
                            else
                            {
                                msgResponse.response[x].name += msgResponse.response[x].volunteers[nbUser] + " - ";
                            }
                        }
                    }
                }
                else
                {

                }
                if (msgResponse.response[x].name.Length >= 17)
                {
                    msgResponse.response[x].name = msgResponse.response[x].name.Substring(0, 17);
                    msgResponse.response[x].name += " ...";
                }
                title.Content = msgResponse.response[x].name + " ( " + msgResponse.response[x].number_volunteers + " participants )";
                title.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));
                title.HorizontalContentAlignment = HorizontalAlignment.Left;
                title.Width = 380;
                title.Margin = new Thickness(0, 0, 0, 10);
                title.Tag = x;
                title.Click += new RoutedEventHandler(GoToMessageClick);

                Grid.SetColumn(title, 0);
                Grid.SetRow(title, x);
                msgGrid.Children.Add(title);
            }
            scroll.Content = msgGrid;
        }

        private async void getMessage()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + "/chatrooms{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                msgResponse = JsonConvert.DeserializeObject<MessageResponse>(responseString);
                if (msgResponse.status != 200)
                {
                    Debug.WriteLine("failed : " + msgResponse.message);
                }
                else
                {
                    initMessage(msgResponse.response.Count);
                }
            }
            catch (HttpRequestException e)
            {
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(responseString);
                System.Diagnostics.Debug.WriteLine(e.Message);
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
                    string message = "{ \"token\" : \"token\", \"token_user\" : \"" + (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"] + "\"}";
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
