using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Tavis.UriTemplates;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All.Models
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GenericCreationModel : Page
    {


        Dictionary<string, FormControlType> forms;
        List<UserControl> elements;
        private Model model;
        private Model typeSearch;
        private FormModel infos;
        private string responseString;
        private Grid formsGrid;
        string Base64String;

        public GenericCreationModel()
        {
            this.InitializeComponent();
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
        }







        private async void uploadLogo(InfosModel tmp)
        {
            string Base64String = "";

            HttpHandler http = HttpHandler.getHttp();
            string url = Global.API_IRL + "/pictures/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("file", Base64String),
                        new KeyValuePair<string, string>("filename", ""),
                        new KeyValuePair<string, string>(Model.Values[infos.createdModelType]["TypeID"], tmp.id.ToString()),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/pictures/", values, HttpHandler.TypeRequest.POST);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                Frame.Navigate(typeof(GenericProfil), tmp);
            }
        }


        private bool checkPassword(int row, string type, out string field)
        {
            field = "";
            return true;
        }

        private bool checkHour(int row, string type, out  string field)
        {
            field = "";

            return true;
        }


        private bool checkCheckField(int row, string type, out string field)
        {
            field = "";

            return true;
        }



        private bool checkField(List<KeyValuePair<string, string>> values)
        {
            int i = 0;
            foreach (KeyValuePair<string, FormControlType> entry in forms)
            {
                string field = "";
                switch (entry.Value)
                {
                    case FormControlType.DESCRIPTION:
                        if (!((GUI.CreateGUI.DescriptionControls)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.FIELD:
                        if (!((GUI.CreateGUI.TitleContro)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.DATE:
                        if (!((GUI.CreateGUI.DateControls)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.FILE:
                        //if (!((GUI.CreateGUI.FileControls)(elements[i])).checkField(entry.Key, out field))
                        //{
                        //    Base64String = field;
                        //    return false;
                        //}
                        break;

                }
                if (Model.Values[infos.createdModelType].ContainsKey(entry.Key))
                    values.Add(new KeyValuePair<string, string>(Model.Values[infos.createdModelType][entry.Key], field));
                else
                    Debug.WriteLine("field : " + field);
                i++;
            }
            return true;
        }

        private async void setResources(object sender, RoutedEventArgs e)
        {
            var values = new List<KeyValuePair<string, string>>();
            if (!checkField(values))
            {
                Debug.WriteLine("test ca marche pas");
                return;
            }
            foreach(KeyValuePair<string, string> na in values)
            {
                Debug.WriteLine("Key : " + na.Key + " value : " + na.Value);
            }
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = null;
            if (infos.isCreation)
                jObject = await http.sendRequest((string)Model.Values[infos.createdModelType]["URL"], values, HttpHandler.TypeRequest.POST);
            else
                jObject = await http.sendRequest((string)Model.Values[infos.createdModelType]["URL"] + infos.id.ToString(), values, HttpHandler.TypeRequest.PUT);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                InfosModel tmp = new InfosModel();
                tmp.id = (int)jObject["response"]["id"];
                tmp.type = infos.createdModelType;
                //uploadLogo(tmp);
            }
        }

        public Grid addCheckField(string hour, Grid tmp)
        {


            return tmp;
        }

        public Grid addPasswordType(string hour, Grid tmp)
        {


            return tmp;
        }

        public Grid addHourType(string hour, Grid tmp)
        {


            return tmp;
        }


        public void createButtonType(Grid tmp, int i)
        {
            tmp.RowDefinitions.Add(new RowDefinition());

            Button click = new Button();
            click.Content = "Valider";
            click.Click += setResources;
            click.Background =new SolidColorBrush(Color.FromArgb(0xFF, 83, 166, 52));
            click.BorderThickness = new Thickness(10, 0, 10, 0);
            
            click.Height = 70;
            click.Width = 380;

            Grid.SetColumn(click, 0);
            Grid.SetRow(click, i);
            tmp.Children.Add(click);
        }

        async Task<List<KeyValuePair<string, string>>> getInfos()
        {
            string token = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"];
            if (token == null || infos.isCreation == true)
            {
                return null;
            }
            var values = new List<KeyValuePair<string, string>>();
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                var template = new UriTemplate(Global.API_IRL + Model.Values[infos.modelType]["URL"] + model.getID() + "{?token}");
                template.AddParameter("token", token);
                var uri = template.Resolve();
                Debug.WriteLine(uri);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(responseString.ToString());
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if ((int)jObject["status"] != 200)
                {
                    err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
                }
                else
                {
                    jObject = (Newtonsoft.Json.Linq.JObject)jObject["response"];
                    foreach (var x in jObject)
                    {
                        values.Add(new KeyValuePair<string, string>(x.Key, (string)x.Value));
                        Debug.WriteLine("Key : " + x.Key + " Value : " + x.Value);

                    }
                }
            }
            catch (HttpRequestException e)
            {
            }

            return values;
        }

        public async void initFormulaire()
        {
            elements = new List<UserControl>();
            forms = typeSearch.getFormControlType();
            int i = 0;
            formsGrid = new Grid();
            formsGrid.VerticalAlignment = VerticalAlignment.Top;
            List<KeyValuePair<string, string>> values = await getInfos();
            formsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            foreach (KeyValuePair<string, FormControlType> entry in forms)
            {
                if (!infos.isAdmin && typeSearch.getPrivateField().Contains(entry.Key))
                    continue;
                string resources = entry.Key;
                UserControl ctls = null;
                if (values != null)
                {
                    //resources = values[(string)(Model.Values[infos.createdModelType][entry.Key]).ToSt];
                    resources = values.First(x => x.Key.Equals((string)(Model.Values[infos.createdModelType][entry.Key]), StringComparison.Ordinal)).Value;
                }
                switch (entry.Value)
                {
                    case FormControlType.FIELD:
                        ctls = new GUI.CreateGUI.TitleContro(resources, entry.Key, err);
                        break;
                    case FormControlType.DESCRIPTION:
                        ctls = new GUI.CreateGUI.DescriptionControls(resources, entry.Key, err);
                        break;
                    case FormControlType.DATE:
                        ctls = new GUI.CreateGUI.DateControls(resources, entry.Key, err);
                        break;
                    case FormControlType.FILE:
                        ctls = new GUI.CreateGUI.FileControls(resources, entry.Key, err);
                        break;
                }

                formsGrid.RowDefinitions.Add(new RowDefinition());
                elements.Add(ctls);
                ctls.Margin = new Thickness(0, 0, 0, 20);
                Grid.SetColumn(ctls, 0);
                Grid.SetRow(ctls, i);
                formsGrid.Children.Add(ctls);

                i++;
            }
            if (infos.isAdmin)
                createButtonType(formsGrid, i);
            scroll.Content = formsGrid;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as FormModel;

            if (infos.createdModelType.Equals("assoc", StringComparison.Ordinal))
            {
                typeSearch = new Association(infos.id);
            }
            else if (infos.createdModelType.Equals("event", StringComparison.Ordinal))
            {
                typeSearch = new Event(infos.id);
            }
            if (infos.modelType.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
            }
            else if (infos.modelType.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
            }
            if (infos.isCreation)
            {
                typeBox.Text = Model.Values[infos.createdModelType]["CreationType"];
            }
            else
            {
                typeBox.Text = "Informations";
            }
            initFormulaire();
        }
    }
}
