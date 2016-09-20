using Caritathelp.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Research : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        class ModelSearch
        {
            public int id { get; set; }
            public string thumb_path { get; set; }
            public string name { get; set; }
            public string rights { get; set; }
            public string result_type { get; set; }
        }

        class ListResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public IList<ModelSearch> response { get; set; }
        }

        private ListResponse searchList;
        private string responseString;

        private Grid searchGrid;

        private async Task searchAll()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                string search = searchBox.Text;
                var template = new UriTemplate(Global.API_IRL + "/search{?research,token}");
                template.AddParameter("research", search);
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                searchList = JsonConvert.DeserializeObject<ListResponse>(responseString);
                if (searchList.status != 200)
                {
                    resultatText.Text = searchList.message;
                }
                else
                {
                    if (searchList != null)
                        initResearchUser(searchList.response.Count);
                    else
                        initResearchUser(0);
                }
            }
            catch (HttpRequestException e)
            {
                resultatText.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
                Debug.WriteLine("Failed to read json");
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void ResearchButtonClick(object sender, RoutedEventArgs e)
        {
           Button button = sender as Button;
            All.Models.InfosModel infos = (All.Models.InfosModel)(button.Tag);
            Frame.Navigate(typeof(All.Models.GenericProfil), infos);
            // identify which button was clicked and perform necessary actions
        }

        private void initResearchUser(int nbRows)
        {
            searchGrid = new Grid();
            searchGrid.Height = nbRows * 100;
            searchGrid.Width = 375;
            for (int i = 0; i < nbRows; ++i)
                searchGrid.RowDefinitions.Add(new RowDefinition());
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int x = 0; x < searchList.response.Count; ++x)
            {
                Button btn = new Button();
                btn.Height = 100;
                btn.Width = searchGrid.Width;
                btn.HorizontalAlignment = HorizontalAlignment.Stretch;
                Models.InfosModel infos = new Models.InfosModel();
                infos.type = searchList.response[x].result_type;
                infos.id = searchList.response[x].id;
                btn.Tag = infos;

                btn.Click += new RoutedEventHandler(ResearchButtonClick);
                btn.Content = searchList.response[x].name;
                btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 75, 175, 80));
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, x);
                searchGrid.Children.Add(btn);
            }
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(100, 114, 136, 142));
        }

        private async void search_Click(object sender, RoutedEventArgs e)
        {
            searchGrid = new Grid();
            await searchAll();
            scroll.Content = searchGrid;
            resultatText.Text = "Résultat pour : " + searchBox.Text;
        }

        public Research()
        {
            this.InitializeComponent();
            resultatText.Text = "";
        }
    }
}
