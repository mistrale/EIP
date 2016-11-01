using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All
{
    public sealed partial class BasePage : UserControl
    {
        class Notification
        {
            public string type { get; set; }
        }

        private MessageWebSocket messageWebSocket;
        private DataWriter messageWriter;
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

        public static ImageSourcePath notifs;
        public static ImageSourcePath msg;

        public void searchButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(All.Research));
        }

        public void messageButtonClick(object sender, RoutedEventArgs e)
        {
            msg.PathToImage = "ms-appx:/Assets/message.png";
            ((Frame)Window.Current.Content).Navigate(typeof(Message.Message));
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
            notifs.PathToImage = "ms-appx:/Assets/alert.png";
            Models.InfosModel infos = new Models.InfosModel();
            infos.type = "volunteer";
            ((Frame)Window.Current.Content).Navigate(typeof(Models.GenericNotification), infos);
        }

        private async void updateNotificationGUI()
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                notifs.PathToImage = "ms-appx:/Assets/alertON.png";
            });
        }

        private async void updateMessageGUI()
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                msg.PathToImage = "ms-appx:/Assets/messageON.png";
            });
        }

        private void MessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            Debug.WriteLine("test notif");
            try
            {
                using (DataReader reader = args.GetDataReader())
                {
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string read = reader.ReadString(reader.UnconsumedBufferLength);
                    Debug.WriteLine("read : " + read);
                    notif = JsonConvert.DeserializeObject<Notification>(read);
                    Debug.WriteLine("DATA : " + read);
                    if (notif.type.Equals("notification", StringComparison.Ordinal))
                    {
                        updateNotificationGUI();
                    }
                    else
                    {
                        updateMessageGUI();
                    }

                }
            }
            catch (Exception ex) // For debugging
            {
                Debug.WriteLine("BIG ERROR " + ex.Message);
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
                Debug.WriteLine("HUGHE ERROR " + ex.Message);
                WebErrorStatus status = WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(status);
                // Add your specific error-handling code here.
            }
        }


        private async void GetPOSITION()
        {
            if ((bool)Windows.Storage.ApplicationData.Current.LocalSettings.Values["allowgps"] != true)
            {
                return;
            }
            while (true)
            {
                Geolocator geolocator = new Geolocator();
                geolocator.DesiredAccuracyInMeters = 50;

                try
                {
                    HttpHandler http = HttpHandler.getHttp();

                    Geoposition geoposition = await geolocator.GetGeopositionAsync(
                         maximumAge: TimeSpan.FromMinutes(5),
                         timeout: TimeSpan.FromSeconds(10)
                        );
                    string url = "/auth/";
                    var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("latitude", geoposition.Coordinate.Latitude.ToString("0.00")),
                        new KeyValuePair<string, string>("longitude",  geoposition.Coordinate.Longitude.ToString("0.00"))
                    };

                    Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.PUT);
                    if (jObject == null)
                    {
                        Debug.WriteLine("failed to connect");
                    }
                    Debug.WriteLine("x : " + geoposition.Coordinate.Latitude.ToString("0.00") + " et y : " + geoposition.Coordinate.Longitude.ToString("0.00"));
                    //With this 2 lines of code, the app is able to write on a Text Label the Latitude and the Longitude, given by {{Icode|geoposition}}
                }
                //If an error is catch 2 are the main causes: the first is that you forgot to include ID_CAP_LOCATION in your app manifest. 
                //The second is that the user doesn't turned on the Location Services
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed getting position : " + ex.Message);
                }
                await Task.Delay(30000);
            }

        }

        public BasePage()
        {
            this.InitializeComponent();
            if (notifs == null)
            {
                notifs = new ImageSourcePath();
                notifs.PathToImage = "ms-appx:/Assets/alert.png";
            }
            notifButton.DataContext = notifs;

            if (msg == null)
            {
                msg = new ImageSourcePath();
                msg.PathToImage = "ms-appx:/Assets/message.png";
            }
            GetPOSITION();
            messageButton.DataContext = msg;
           startListening();

        }
    }
}

