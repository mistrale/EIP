using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateAssociation : Page
    {
        class AssociationRequest
        {
            public string status { get; set; }
            public string message { get; set; }
            public Association response { get; set; }
        }

        private AssociationRequest returned;
        private String responseString;

        private bool isValid()
        {
            if (titleText.Text == String.Empty || titleText.Text.Equals("Nom de l'association", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Nom vide.";
                return false;
            }
            if (descriptionText.Text == String.Empty || descriptionText.Text.Equals("Description", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Description vide.";
                return false;
            }
            return true;
        }

        private async void createAssociationRequest()
        {
            string fullDate;
            string date = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            string year = DateTime.Today.Year.ToString();
            fullDate = month + "-" + date + "-" + year + "T00:00:00Z";
            string url = "http://52.31.151.160:3000/associations/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("name", titleText.Text),
                        new KeyValuePair<string, string>("description", descriptionText.Text),
                        new KeyValuePair<string, string>("birthday", fullDate),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                Loading.IsActive = false;
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                returned = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if (Int32.Parse(returned.status) != 200)
                {
                    warningTextBox.Text = returned.message;
                }
                else
                {
                    Frame.Navigate(typeof(AssociationProfil), (string)returned.response.id.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                warningTextBox.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                Loading.IsActive = false;
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void createAssociationClick(object sender, RoutedEventArgs e)
        {
            if (!isValid())
                return;
            warningTextBox.Text = "";
            Loading.IsActive = true;
            createAssociationRequest();
        }

        public void chooseFileClick(object sender, RoutedEventArgs e)
        {

        }

        public CreateAssociation()
        {
            this.InitializeComponent();
            warningTextBox.Text = "";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }
        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }
    }
}
