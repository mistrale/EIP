using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Tavis.UriTemplates;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accueil : Page
    {
        class SimpleRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public string response { get; set; }
        }

        Grid newsGrid;
        private Newtonsoft.Json.Linq.JArray newsResponse;

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = GUI.PolicyGenerator.getBrush(GUI.PolicyGenerator.ColorType.DEFAULT_TEXT);
        }

        public void publish(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("text : " + publicationText.Text);
            if (!publicationText.Text.Equals("Publier ...", StringComparison.Ordinal)
                && !publicationText.Text.Equals("", StringComparison.Ordinal))
            {
                publishNews();
            }
        }

        private async void publishNews()
        {
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("content", publicationText.Text),
                        new KeyValuePair<string, string>("group_id",  ((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]).ToString()),
                        new KeyValuePair<string, string>("news_type", "Status"),
                        new KeyValuePair<string, string>("as_group", "false"),
                        new KeyValuePair<string, string>("group_type", "Volunteer"),
                };
            string url = ("/news/wall_message");
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] == 200)
            {
                Frame.Navigate(typeof(Accueil));
            }
            else
            {
                errorControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        private async void getNews()
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/news", null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] == 200)
            {
                newsResponse = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                newsGrid = new Grid();
                newsGrid.ColumnDefinitions.Add(new ColumnDefinition());

                newsGrid.VerticalAlignment = VerticalAlignment.Top;
                for (int i = 0; i <  newsResponse.Count; i++)
                {
                    newsGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.NewControle btn = new GUI.NewControle((Newtonsoft.Json.Linq.JObject)(newsResponse[i]), optionsComment, errorControl, this);
                    Grid.SetColumn(btn, 0);
                    Grid.SetRow(btn, i);
                    newsGrid.Children.Add(btn);
                }
                scroll.Content = newsGrid;
            } else
            {
                errorControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        public Accueil()
        {
            this.InitializeComponent();
            getNews();
            optionsComment.setCurrentPage(this, typeof(Accueil), null, errorControl);
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
