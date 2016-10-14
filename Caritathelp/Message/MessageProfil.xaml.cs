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
            public string create_at { get; set; }
            public int sender_id { get; set; }

        }

        private Notification notif;
        static private MessageWebSocket messageWebSocket;
        static private DataWriter messageWriter;
        private MessageInfos infos;
        private string responseString;
        Grid msgGrid;

        public MessageProfil()
        {
            this.InitializeComponent();
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public void sendMessageClick(object sender, RoutedEventArgs e)
        {
            if (!msgBox.Text.Equals("", StringComparison.Ordinal)
                && !msgBox.Text.Equals("Votre message ...", StringComparison.Ordinal))
            sendMessage();
        }

        private async void updateMessageGUI()
        {
            Debug.WriteLine("testasdasdasd");
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                msgGrid.RowDefinitions.Add(new RowDefinition());

                Grid tmpGrid = new Grid();
                tmpGrid.Margin = new Thickness(0, 0, 0, 20);

                if (notif.sender_id != Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]))
                {
                    tmpGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 200, 211, 200));
                }
                else
                {
                    tmpGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 157, 216, 160));
                }
                tmpGrid.VerticalAlignment = VerticalAlignment.Top;
                tmpGrid.Width = 380;

                // col
                var columImg = new ColumnDefinition();
                columImg.Width = new GridLength(70);
                tmpGrid.ColumnDefinitions.Add(columImg);

                var colName = new ColumnDefinition();
                tmpGrid.ColumnDefinitions.Add(colName);

                // rows
                var rowInfos = new RowDefinition();
                rowInfos.Height = new GridLength(70);
                tmpGrid.RowDefinitions.Add(rowInfos);

                var rowContent = new RowDefinition();
                //rowContent.Height = new GridLength(200);
                tmpGrid.RowDefinitions.Add(rowContent);

                // image profil
                Image profil = new Image();
                profil.Margin = new Thickness(10, 10, 10, 10);
                profil.Source = new BitmapImage(new Uri(Global.API_IRL + notif.sender_thumb_path , UriKind.Absolute));
                Grid.SetColumn(profil, 0);
                Grid.SetRow(profil, 0);
                tmpGrid.Children.Add(profil);

                // name
                TextBlock name = new TextBlock();
                name.Margin = new Thickness(10, 10, 10, 10);
                name.FontSize = 14;
                name.TextWrapping = TextWrapping.Wrap;
                name.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                name.Text = notif.sender_firstname + " " + notif.sender_lastname;
                name.FontWeight = FontWeights.Bold;
                name.VerticalAlignment = VerticalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(name, 1);
                Grid.SetRow(name, 0);
                tmpGrid.Children.Add(name);

                // time published
                TextBlock date = new TextBlock();
                DateTime convertedDate;
                date.Margin = new Thickness(10, 10, 0, 10);
                convertedDate = Convert.ToDateTime(notif.create_at);
                date.Text = convertedDate.ToString();
                date.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                date.TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(date, 0);
                Grid.SetRow(date, 1);
                tmpGrid.Children.Add(date);

                // content
                TextBlock msg = new TextBlock();
                msg.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                msg.Text = notif.content;
                msg.FontSize = 14;
                msg.VerticalAlignment = VerticalAlignment.Top;
                msg.Margin = new Thickness(10, 10, 0, 10);
                msg.TextWrapping = TextWrapping.Wrap;

                Grid.SetColumn(msg, 1);
                Grid.SetRow(msg, 1);
                tmpGrid.Children.Add(msg);


                // add to general grid
                Grid.SetColumn(tmpGrid, 0);
                Grid.SetRow(tmpGrid, msgGrid.RowDefinitions.Count + 1);
                msgGrid.RowDefinitions.Add(new RowDefinition());



                msgGrid.Children.Add(tmpGrid);

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
            //msgGrid.Height = responseConv.response.Count * 400;
            msgGrid.Width = 380;
            for (int x = 0; x < list.Count; x++)
            {
                msgGrid.RowDefinitions.Add(new RowDefinition());

                Grid tmpGrid = new Grid();
                tmpGrid.Margin = new Thickness(0, 0, 0, 20);

                if ((int)list[x]["volunteer_id"] != Convert.ToInt32((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]))
                {
                    tmpGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 200, 211, 200));
                }
                else
                {
                    tmpGrid.Background = new SolidColorBrush(Color.FromArgb(0xFF, 157, 216, 160));
                }
                tmpGrid.VerticalAlignment = VerticalAlignment.Top;
                tmpGrid.Width = 380;

                // col
                var columImg = new ColumnDefinition();
                columImg.Width = new GridLength(70);
                tmpGrid.ColumnDefinitions.Add(columImg);

                var colName = new ColumnDefinition();
                tmpGrid.ColumnDefinitions.Add(colName);

                // rows
                var rowInfos = new RowDefinition();
                rowInfos.Height = new GridLength(70);
                tmpGrid.RowDefinitions.Add(rowInfos);

                var rowContent = new RowDefinition();
                //rowContent.Height = new GridLength(200);
                tmpGrid.RowDefinitions.Add(rowContent);

                // image profil
                Image profil = new Image();
                profil.Margin = new Thickness(10, 10, 10, 10);
                profil.Source = new BitmapImage(new Uri(Global.API_IRL + (string)list[x]["thumb_path"], UriKind.Absolute));
                Grid.SetColumn(profil, 0);
                Grid.SetRow(profil, 0);
                tmpGrid.Children.Add(profil);

                // name
                TextBlock name = new TextBlock();
                name.Margin = new Thickness(10, 10, 10, 10);
                name.FontSize = 14;
                name.TextWrapping = TextWrapping.Wrap;
                name.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                name.Text = (string)list[x]["fullname"];
                name.FontWeight = FontWeights.Bold;
                name.VerticalAlignment = VerticalAlignment.Center;
                name.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetColumn(name, 1);
                Grid.SetRow(name, 0);
                tmpGrid.Children.Add(name);

                // time published
                TextBlock date = new TextBlock();
                DateTime convertedDate;
                date.Margin = new Thickness(10, 10, 0, 10);
                convertedDate = Convert.ToDateTime((string)list[x]["created_at"]);
                date.Text = convertedDate.ToString();
                date.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                date.TextWrapping = TextWrapping.Wrap;
                Grid.SetColumn(date, 0);
                Grid.SetRow(date, 1);
                tmpGrid.Children.Add(date);

                // content
                TextBlock msg = new TextBlock();
                msg.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                msg.Text = (string)list[x]["content"];
                msg.FontSize = 14;
                msg.VerticalAlignment = VerticalAlignment.Top;
                msg.Margin = new Thickness(10, 10, 0, 10);
                msg.TextWrapping = TextWrapping.Wrap;

                Grid.SetColumn(msg, 1);
                Grid.SetRow(msg, 1);
                tmpGrid.Children.Add(msg);


                // add to general grid
                Grid.SetColumn(tmpGrid, 0);
                Grid.SetRow(tmpGrid, x);

                msgGrid.Children.Add(tmpGrid);

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
                    Debug.WriteLine("DATA MESSQGE IN MESSAGE : " + read);
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
            getMessage(infos);
            startListening();
        }
    }
}
