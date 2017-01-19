using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericListModelManagement : Page
    {
       // private Model model;
        private InfosListModel infos;
        private Newtonsoft.Json.Linq.JArray searchList;

        private Grid gridUser;

        public GenericListModelManagement()
        {
            this.InitializeComponent();
        }


        private async void initListModel()
        {
            var url = Model.Values[infos.typeModel]["URL"]
                    + infos.id + Models.Model.resourcesTables[infos.typeModel][infos.listTypeModel];
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(url, null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                searchList = (Newtonsoft.Json.Linq.JArray)(jObject["response"]);
                gridUser = new Grid();
                gridUser.VerticalAlignment = VerticalAlignment.Top;
                gridUser.ColumnDefinitions.Add(new ColumnDefinition());
                for (int x = 0; x < searchList.Count; ++x)
                {
                    gridUser.RowDefinitions.Add(new RowDefinition());
                    GUI.ListManagement controls = new GUI.ListManagement(infos, (Newtonsoft.Json.Linq.JObject)searchList[x], err,
                        this, infos.listTypeModel.Equals("volunteer") && !infos.typeModel.Equals("volunteer"), cfBox);
                    controls.Margin = new Thickness(0, 0, 0, 15);
                    Grid.SetColumn(controls, 0);
                    Grid.SetRow(controls, x);
                    gridUser.Children.Add(controls);
                }
            }
            scroll.Content = gridUser;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as InfosListModel;
            if (infos.listTypeModel.Equals("assoc", StringComparison.Ordinal))
            {
                title.Text = "Associations";

            }
            else if (infos.listTypeModel.Equals("event", StringComparison.Ordinal))
            {
                title.Text = "Evenements";
            }
            else if (infos.listTypeModel.Equals("volunteer", StringComparison.Ordinal))
            {
                title.Text = "Volontaires";
            }
            else if (infos.listTypeModel.Equals("shelter", StringComparison.Ordinal))
            {
                title.Text = "Centres";
            }
            initListModel();
        }
    }
}
