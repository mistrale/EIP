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
        private InfosListModel infos;
        private Model model;
        private Model typeSearch;

        private Newtonsoft.Json.Linq.JArray searchList;
        private Grid searchGrid;

        public GenericListModelManagement()
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
            //infos = e.Parameter as InfosListModel;

            //Debug.WriteLine("type : " + infos.listTypeModel);
            //if (infos.listTypeModel.Equals("assoc", StringComparison.Ordinal))
            //{
            //    typeSearch = new Association(infos.id);
            //    titleBox.Text = "Associations";

            //}
            //else if (infos.listTypeModel.Equals("event", StringComparison.Ordinal))
            //{
            //    typeSearch = new Event(infos.id);
            //    titleBox.Text = "Evenements";
            //}
            //if (infos.typeModel.Equals("assoc", StringComparison.Ordinal))
            //{
            //    model = new Association(infos.id);
            //}
            //else if (infos.typeModel.Equals("event", StringComparison.Ordinal))
            //{
            //    model = new Event(infos.id);
            //}
            //initListModel();
        }
    }
}
