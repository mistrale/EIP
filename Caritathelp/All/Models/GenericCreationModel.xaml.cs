using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
        Newtonsoft.Json.Linq.JObject jObject;

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

        private async Task addLocation(List<KeyValuePair<string, string>> values, string place)
        {
            try
            {
                string addressToGeocode = "Paris";
                double lat = 0;
                double longi = 0;
                System.Net.Http.HttpResponseMessage response = null;
                System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
                response = await httpClient.GetAsync("https://maps.googleapis.com/maps/api/geocode/json?address=" + place + "&key=AIzaSyD_-W6ZlmJ36TyvN_hdbos9LEwGI6UWYTc");
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();

                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);

                if ((string)jObject["status"] == "OK")
                {
                    Newtonsoft.Json.Linq.JObject result = (Newtonsoft.Json.Linq.JObject)((Newtonsoft.Json.Linq.JArray)jObject["results"])[0];

                    Newtonsoft.Json.Linq.JObject location = (Newtonsoft.Json.Linq.JObject)((Newtonsoft.Json.Linq.JObject)result["geometry"])["location"];

                    lat = (double)location["lat"];

                    longi = (double)location["lng"];
                }
                values.Add(new KeyValuePair<string, string>("latitude", lat.ToString()));
                values.Add(new KeyValuePair<string, string>("longitude", longi.ToString()));


                Debug.WriteLine("lat : " + lat + " et long : " + longi);
            } catch (Exception e)
            {
                Debug.WriteLine("wtf : " + e.Message);
            }

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
                    case FormControlType.NUMBER:
                        if (!((GUI.CreateGUI.TitleContro)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.DATE:
                        if (!((GUI.CreateGUI.DateControls)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.CHECKFIELD:
                        if (!((GUI.CreateGUI.CheckField)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.COMBOX:
                        if (!((GUI.CreateGUI.ComboBoxControl)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.HOUR:
                        if (!((GUI.CreateGUI.HourControl)(elements[i])).checkField(entry.Key, out field))
                            return false;
                        break;
                }
                if (Model.Values[typeSearch.getType()].ContainsKey(entry.Key))
                    values.Add(new KeyValuePair<string, string>(Model.Values[typeSearch.getType()][entry.Key], field));
                else
                    Debug.WriteLine("field : " + field);
                i++;
            }
            values.Add(new KeyValuePair<string, string>(Model.Values[model.getType()]["TypeID"], model.getID().ToString()));

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
            for (int i = 0; i < values.Count; i++ )
            {
                if (values[i].Key.Equals("place", StringComparison.Ordinal) || values[i].Key.Equals("address", StringComparison.Ordinal))
                {
                    await addLocation(values, values[i].Value);
                }
            }

            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject1 = null;

            if (infos.isCreation)
                jObject1 = await http.sendRequest((string)Model.Values[typeSearch.getType()]["URL"], values, HttpHandler.TypeRequest.POST);
            else
            {
                if (infos.modelType.getType().Equals("volunteer", StringComparison.Ordinal))
                {
                    jObject1 = await http.sendRequest("/auth", values, HttpHandler.TypeRequest.PUT);

                }
                else if (!infos.modelType.getType().Equals("shelter", StringComparison.Ordinal))
                {
                    jObject1 = await http.sendRequest((string)Model.Values[typeSearch.getType()]["URL"] + infos.id.ToString(), values, HttpHandler.TypeRequest.PUT);
                } else
                {
                    values.Add(new KeyValuePair<string, string>("assoc_id", jObject["assoc_id"].ToString()));
                    jObject1 = await http.sendRequest((string)Model.Values[typeSearch.getType()]["URL"] + infos.id.ToString(), values, HttpHandler.TypeRequest.PUT);
                }

            }
            if ((int)jObject1["status"] != 200)
            {

  
                err.printMessage((string)jObject1["message"], GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                typeSearch.id = (int)jObject1["response"]["id"];
                if (typeSearch.getType().Equals("shelter", StringComparison.Ordinal))
                {
                    FormModel form = new FormModel();
                    form.id = typeSearch.getID();
                    form.createdModelType = typeSearch;
                    form.isCreation = false;
                    form.modelType = typeSearch;
                    form.isAdmin = true;
                    Frame.Navigate(typeof(GenericCreationModel), form);
                }
                else
                {
                    if (infos.modelType.getType().Equals("volunteer", StringComparison.Ordinal))
                    {
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("password");
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("id");
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("thumb_path");
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("mail");
                        Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove("allowgps");
                        HttpHandler.resetHttp();
                        this.Frame.Navigate(typeof(MainPage));
                    } else
                    {
                        Frame.Navigate(typeof(GenericProfil), typeSearch);
                    }
                }
            }
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
            Debug.WriteLine("values : " + infos.isCreation);
            if (infos.isCreation == true)
            {
                return null;
            }
            var values = new List<KeyValuePair<string, string>>();
            HttpHandler http = HttpHandler.getHttp();
            jObject = await http.sendRequest(Model.Values[model.getType()]["URL"] + model.getID(), null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);                
            }
            else
            {
                jObject = (Newtonsoft.Json.Linq.JObject)jObject["response"];
                foreach (var x in jObject)
                {
                    if (x.Key.Equals("tags", StringComparison.Ordinal))
                        continue;
                    values.Add(new KeyValuePair<string, string>(x.Key, (string)x.Value));

                    Debug.WriteLine("Key : " + x.Key + " Value : " + x.Value);

                }
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
                    resources = values.First(x => x.Key.Equals((string)(Model.Values[typeSearch.getType()][entry.Key]), StringComparison.Ordinal)).Value;
                }
                switch (entry.Value)
                {
                    case FormControlType.FIELD:
                        ctls = new GUI.CreateGUI.TitleContro(entry.Key, resources, err, GUI.CreateGUI.TitleContro.Type.STRING);
                        break;
                    case FormControlType.NUMBER:
                        ctls = new GUI.CreateGUI.TitleContro(entry.Key, resources, err, GUI.CreateGUI.TitleContro.Type.NUMBER);
                        break;
                    case FormControlType.DESCRIPTION:
                        ctls = new GUI.CreateGUI.DescriptionControls(entry.Key, resources, err);
                        break;
                    case FormControlType.DATE:
                        ctls = new GUI.CreateGUI.DateControls(entry.Key, resources, err);
                        break;
                    case FormControlType.CHECKFIELD:
                        ctls = new GUI.CreateGUI.CheckField(entry.Key, resources, err);
                        break;
                    case FormControlType.COMBOX:
                        ctls = new GUI.CreateGUI.ComboBoxControl(entry.Key, resources, err);
                        break;
                    case FormControlType.HOUR:
                        ctls = new GUI.CreateGUI.HourControl(entry.Key, resources, err);
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
            typeSearch = infos.createdModelType;
            model = infos.modelType;
            if (infos.isCreation)
            {
                typeBox.Text = Model.Values[typeSearch.getType()]["CreationType"];
            }
            else
            {
                typeBox.Text = "Informations";
            }
            initFormulaire();
        }
    }
}
