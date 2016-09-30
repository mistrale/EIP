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
        private Newtonsoft.Json.Linq.JArray searchList;
        private Grid searchGrid;

        private async Task searchAll()
        {
            var template = new UriTemplate("/search{?research}");
            template.AddParameter("research", searchBox.Text);
            var url = template.Resolve();

            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                searchList = (Newtonsoft.Json.Linq.JArray)(jObject["response"]);
                initResearchUser(searchList.Count);
            }
        }

        private void initResearchUser(int nbRows)
        {
            searchGrid = new Grid();
            searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
            searchGrid.VerticalAlignment = VerticalAlignment.Top;
            for (int x = 0; x < searchList.Count; ++x)
            {
                searchGrid.RowDefinitions.Add(new RowDefinition());
                GUI.SearchItem controls = new GUI.SearchItem();
                
                Models.InfosModel infos = new Models.InfosModel();
                infos.type = (string)searchList[x]["result_type"];
                infos.id = (int)searchList[x]["id"];
                controls.setItem((string)searchList[x]["thumb_path"], (string)searchList[x]["name"], (string)searchList[x]["result_type"], this, infos);

                Grid.SetColumn(controls, 0);
                Grid.SetRow(controls, x);
                searchGrid.Children.Add(controls);
            }
            scroll.Content = searchGrid;
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
        }

        private async void search_Click(object sender, RoutedEventArgs e)
        {
            searchGrid = new Grid();
            await searchAll();
            scroll.Content = searchGrid;
        }

        public Research()
        {
            this.InitializeComponent();
        }
    }
}
