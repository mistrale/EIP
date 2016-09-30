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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericListModel : Page
    {
        private InfosListModel infos;
        private Model model;
        private Model typeSearch;

        private Newtonsoft.Json.Linq.JArray searchList;
        private Grid searchGrid;

        private void EventButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            InfosModel tmp = (InfosModel)button.Tag;
            Frame.Navigate(typeof(GenericProfil), tmp);
        }

        private async void initListModel()
        {
            var url = Model.Values[infos.typeModel]["URL"]
                    + infos.id + Model.Values[infos.listTypeModel]["ResourceURL"];
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                errControl.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                searchList = (Newtonsoft.Json.Linq.JArray)(jObject["response"]);
                searchGrid = new Grid();
                searchGrid.VerticalAlignment = VerticalAlignment.Top;
                searchGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int x = 0; x < searchList.Count; ++x)
                {
                    searchGrid.RowDefinitions.Add(new RowDefinition());
                    GUI.SearchItem controls = new GUI.SearchItem();

                    InfosModel tmp = new InfosModel();
                    tmp.id = (int)searchList[x]["id"];
                    tmp.type = Model.Values[infos.listTypeModel]["Model"];
                    controls.setItem((string)searchList[x]["thumb_path"], (string)searchList[x][Model.Values[infos.listTypeModel]["NameType"]],
                        "Amis en commun " + (string)searchList[x][Model.Values[infos.listTypeModel]["NbRelationType"]], this, tmp);

                    Grid.SetColumn(controls, 0);
                    Grid.SetRow(controls, x);
                    searchGrid.Children.Add(controls);
                }
            }
            scroll.Content = searchGrid;
        }

        public GenericListModel()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as InfosListModel;
           
            Debug.WriteLine("type : " + infos.listTypeModel);
            if (infos.listTypeModel.Equals("assoc", StringComparison.Ordinal))
            {
                typeSearch = new Association(infos.id);
                titleBox.Text = "Associations";

            }
            else if (infos.listTypeModel.Equals("event", StringComparison.Ordinal))
            {
                typeSearch = new Event(infos.id);
                titleBox.Text = "Evenements";
            }
            if (infos.typeModel.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
            }
            else if (infos.typeModel.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
            }
            initListModel();
        }
    }
}
