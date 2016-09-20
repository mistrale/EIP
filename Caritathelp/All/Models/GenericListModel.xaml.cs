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

        private string responseString;
        private Grid listGrid;

        private void EventButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            InfosModel tmp = (InfosModel)button.Tag;
            Frame.Navigate(typeof(GenericProfil), tmp);
        }

        private async void initListModel<T>()
        {
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + Model.Values[infos.typeModel]["URL"]
                    + infos.id + Model.Values[infos.listTypeModel]["ResourceURL"] + "{?token}");
                template.AddParameter("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"]);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                //events = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if ((int)(jObject["status"]) != 200)
                {

                }
                else
                {

                    Newtonsoft.Json.Linq.JArray listObj  = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                    listGrid = new Grid();
                    listGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    listGrid.VerticalAlignment = VerticalAlignment.Top;
                    listGrid.Height = listObj.Count * 100;
                    listGrid.Width = 375;
                    for (int x = 0; x < listObj.Count; ++x)
                    {
                        listGrid.RowDefinitions.Add(new RowDefinition());
                        Grid grid = new Grid();
                        grid.Margin = new Thickness(0, 10, 0, 10);
                        grid.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));

                        // row
                        var row = new RowDefinition();
                        row.Height = new GridLength(50);
                        grid.RowDefinitions.Add(row);

                        var row2 = new RowDefinition();
                        grid.RowDefinitions.Add(row2);

                        //column
                        var colum = new ColumnDefinition();
                        colum.Width = new GridLength(100);
                        grid.ColumnDefinitions.Add(colum);

                        var column2 = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(column2);


                        // title
                        Button title = new Button();

                        InfosModel tmp = new InfosModel();
                        tmp.id = (int)listObj[x]["id"];
                        tmp.type = Model.Values[infos.listTypeModel]["Model"];

                        title.Content = (string)listObj[x][Model.Values[infos.listTypeModel]["NameType"]];
                        title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        title.BorderThickness = new Thickness(0);
                        title.Tag = tmp;
                        title.Click += new RoutedEventHandler(EventButtonClick);
                        title.FontSize = 18;
                        Grid.SetColumn(title, 1);
                        Grid.SetRow(title, 0);
                        grid.Children.Add(title);

                        // nb ami
                        TextBlock friends = new TextBlock();
                        friends.Text = "Amis en commun " + (string)listObj[x][Model.Values[infos.listTypeModel]["NbRelationType"]];
                        friends.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                        friends.FontSize = 14;
                        friends.Margin = new Thickness(10, 0, 0, 0);
                        Grid.SetColumn(friends, 1);
                        Grid.SetRow(friends, 1);
                        grid.Children.Add(friends);


                        Image btn = new Image();
                        btn.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(Global.API_IRL + (string)listObj[x]["thumb_path"], UriKind.Absolute));
                        btn.Stretch = Stretch.Fill;
                        btn.Width = 100;
                        btn.Height = 100;
                        // image
                        Grid.SetColumn(btn, 0);
                        Grid.SetRow(btn, 0);
                        Grid.SetRowSpan(btn, 2);
                        grid.Children.Add(btn);


                        listGrid.Children.Add(grid);
                        Grid.SetColumn(grid, 0);
                        Grid.SetRow(grid, x);
                    }
                    listGrid.Visibility = Visibility.Visible;
                    scrollList.Content = listGrid;
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
            }
            else if (infos.listTypeModel.Equals("event", StringComparison.Ordinal))
            {
                typeSearch = new Event(infos.id);
            }
            if (infos.typeModel.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
                initListModel<Association>();
            }
            else if (infos.typeModel.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
                initListModel<Event>();
            }
        }
    }
}
