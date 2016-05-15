using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class InformationAssociation : Page
    {
        class AssociationRequest
        {
            public string status { get; set; }
            public string message { get; set; }
            public Association response { get; set; }
        }

        private AssociationRequest assoc;
        private string id;
        private string responseString;
        private AssociationRequest returned;


        public void goBackClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AssociationProfil), id);
        }

        private async void updateAssociation()
        {
            string url = "http://52.31.151.160:3000/associations/" + id;
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("name", title.Text),
                        new KeyValuePair<string, string>("description", description.Text),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                returned = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if (Int32.Parse(returned.status) != 200)
                {
                    warning.Text = returned.message;
                }
                else
                {
                    warning.Text = "Association modifiée !";
                }
            }
            catch (HttpRequestException e)
            {
                warning.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public void updateAssociationClick(object sender, RoutedEventArgs e)
        {
            updateAssociation();
        }

        public void chooseFileClick(object sender, RoutedEventArgs e)
        {
        }


        public InformationAssociation()
        {
            this.InitializeComponent();
        }

        private async void getInformations()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate("http://52.31.151.160:3000/associations/" + id + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                assoc = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if (Int32.Parse(assoc.status) != 200)
                {

                }
                else
                {
                    title.Text = assoc.response.name;
                    description.Text = assoc.response.description;
                   if (assoc.response.birthday != null)
                    {
                        string[] begin = assoc.response.birthday.Split('-');
                        birthday.Date = (DateTime)Convert.ToDateTime(begin[1] + '/' + begin[2] + '/' + begin[0]);
                    }
                    //    birthday.Text = assoc.response.birthday.;
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"] = id.ToString();
                    if (assoc.response.rights.Equals("owner", StringComparison.Ordinal)
                        || assoc.response.rights.Equals("admin", StringComparison.Ordinal))
                    {
                        title.IsReadOnly = false;
                        description.IsReadOnly = false;
                        logoText.Visibility = Visibility.Visible;
                        logoButton.Visibility = Visibility.Visible;
                        updateButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        title.IsReadOnly = true;
                        description.IsReadOnly = true;
                        logoText.Visibility = Visibility.Collapsed;
                        logoButton.Visibility = Visibility.Collapsed;
                        updateButton.Visibility = Visibility.Collapsed;
                    }
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            id = e.Parameter as string;
            warning.Text = "";
            getInformations();
        }
    }
}
