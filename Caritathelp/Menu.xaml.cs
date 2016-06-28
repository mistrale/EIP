using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using System.ComponentModel;


// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp
{
    public sealed partial class Menu : UserControl
    {
        class Notification
        {
            public string type { get; set; }
        }

        static private MessageWebSocket messageWebSocket;
        static private DataWriter messageWriter;
        private Notification notif;
        public class ImageSourcePath : INotifyPropertyChanged
        {
            private string _path;

            // Declare the PropertyChanged event.
            public event PropertyChangedEventHandler PropertyChanged;

            // Create the property that will be the source of the binding.
            public String PathToImage
            {
                get { return _path; }
                set
                {
                    _path = value;
                    // Call NotifyPropertyChanged when the source property 
                    // is updated.
                    NotifyPropertyChanged("PathToImage");
                }
            }


            // NotifyPropertyChanged will raise the PropertyChanged event, 
            // passing the source property that is being updated.
            public void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,
                        new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public static ImageSourcePath test;

        public void messageButtonClick(object sender, RoutedEventArgs e)
        {

        }

        public void moreButtonClick(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Options));
        }

        public void homeButtonClick(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Accueil));
        }

        public void alertButtonClick(object sender, RoutedEventArgs e)
        {
            test.PathToImage = "ms-appx:/Assets/alert.png";
            ((Frame)Window.Current.Content).Navigate(typeof(All.Notification));
        }

        private async void updateGUI()
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                BindingExpression expression = notifButton.GetBindingExpression(Button.DataContextProperty);
                test.PathToImage = "ms-appx:/Assets/alertON.png";
            });
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
                    Debug.WriteLine(read);
                    if (notif.type.Equals("notification", StringComparison.Ordinal))
                    {
                        updateGUI();
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
                    Uri server = new Uri("ws://api.caritathelp.me:8080");

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

        public Menu()
        {
            this.InitializeComponent();
            if (test == null)
            {
                test = new ImageSourcePath();
                test.PathToImage = "ms-appx:/Assets/alert.png";
            }
            notifButton.DataContext = test;
            startListening();
        }
    }
}
