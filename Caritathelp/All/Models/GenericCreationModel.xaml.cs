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
        public class ImageSourcePath : INotifyPropertyChanged
        {
            private string _path;

            public event PropertyChangedEventHandler PropertyChanged;

            public String PathToImage
            {
                get { return _path; }
                set
                {
                    _path = value;
                    NotifyPropertyChanged("PathToImage");
                }
            }

            public void NotifyPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,
                        new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public static ImageSourcePath filePath;
        Image file;
        Dictionary<string, FormControlType> forms;
        private Model model;
        private Model typeSearch;
        private FormModel infos;
        private string responseString;
        private Grid formsGrid;

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

        CoreApplicationView view;
        StorageFile storageFileWP;
        Binding myBinding;

        public void chooseFileClick(object sender, RoutedEventArgs e)
        {
            view = CoreApplication.GetCurrentView();

            string ImagePath = string.Empty;
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            filePicker.ViewMode = PickerViewMode.Thumbnail;

            // Filter to include a sample subset of file types
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".pdf");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".gif");
            filePicker.PickSingleFileAndContinue();
            view.Activated += viewActivated;

        }

        private async void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                storageFileWP = args.Files[0];

            }
            Debug.WriteLine("testasdasdasd");
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                filePath.PathToImage = storageFileWP.Name.ToString();
            });
        }


        private async void uploadLogo(InfosModel tmp)
        {
            string Base64String = "";

            if (storageFileWP != null)
            {
                IRandomAccessStream fileStream = await storageFileWP.OpenAsync(FileAccessMode.Read);
                var reader = new DataReader(fileStream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)fileStream.Size);
                byte[] byteArray = new byte[fileStream.Size];
                reader.ReadBytes(byteArray);
                Base64String = Convert.ToBase64String(byteArray);
            }
            else
            {
                infosBox.Text = "Fichier invalide.";
                Frame.Navigate(typeof(GenericProfil), tmp);

                return;
            }
            string url = Global.API_IRL + "/pictures/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("file", Base64String),
                        new KeyValuePair<string, string>("filename", storageFileWP.Name.ToString()),
                        new KeyValuePair<string, string>(Model.Values[infos.createdModelType]["TypeID"], tmp.id.ToString()),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);

                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);
                if ((int)jObject["status"] != 200)
                {
                    infosBox.Text = (string)jObject["message"];
                }
                else
                {
                    Frame.Navigate(typeof(GenericProfil), tmp);
                }
            }
            catch (HttpRequestException e)
            {
                infosBox.Text = e.Message;
                Debug.WriteLine(e.Message);
            }
        }

        private bool checkFile(int row, string type, out string field)
        {
            Grid grid = (Grid)((Border)(formsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row))).Child;
            string text = ((TextBox)(grid.Children.Cast<FrameworkElement>()
                .FirstOrDefault(x => Grid.GetRow(x) == 1 && Grid.GetColumn(x) == 1))).Text;
            if (text.Equals(type, StringComparison.Ordinal)
                || text.Equals("", StringComparison.Ordinal))
            {
                field = "";
                return true;
            }
            field = text;
            return true;
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

        private bool checkDate(int row, string type, out string field)
        {
            Grid grid = (Grid)((Border)(formsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row))).Child;
            DatePicker beginDate = ((DatePicker)(grid.Children.Cast<FrameworkElement>()
                .FirstOrDefault(x => Grid.GetRow(x) == 1 && Grid.GetColumn(x) == 1)));
            String[] data = beginDate.Date.ToString().Split(' ')[0].Split('/');
            string date = data[2] + '-' + data[0] + '-' + data[1] + 'T';
            field = date;
            return true;
        }

        private bool checkCheckField(int row, string type, out string field)
        {
            field = "";

            return true;
        }

        private bool checkTextField(int row, string type, out string field)
        {
            Grid grid = (Grid)((Border)(formsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row))).Child;
            string text = ((TextBox)(grid.Children.Cast<FrameworkElement>()
                .FirstOrDefault(x => Grid.GetRow(x) == 1 && Grid.GetColumn(x) == 1))).Text;
            if (text.Equals(type, StringComparison.Ordinal)
                || text.Equals("", StringComparison.Ordinal))
            {
                field = "";

                infosBox.Text = "Champs '" + type + "' vide.";
                return false;
            }
            field = text;
            return true;
        }

        private bool checkDescription(int row, string type, out string field)
        {
            Grid grid = (Grid)((Border)(formsGrid.Children.Cast<FrameworkElement>().
                         FirstOrDefault(y => Grid.GetRow(y) == row))).Child;
            string text = ((TextBox)(grid.Children.Cast<FrameworkElement>()
                .FirstOrDefault(x => Grid.GetRow(x) == 1))).Text;
            if (text.Equals(type, StringComparison.Ordinal)
                || text.Equals("", StringComparison.Ordinal))
            {
                field = "";

                infosBox.Text = "Champs '" + type + "' vide.";
                return false;
            }
            field = text;
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
                        if (!checkDescription(i, entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.FIELD:
                        if (!checkTextField(i, entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.PASSWORD:
                        if (!checkPassword(i, entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.CHECKFIELD:
                        if (!checkCheckField(i, entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.DATE:
                        if (!checkDate(i, entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.HOUR:
                        if (!checkHour(i, entry.Key, out field))
                            return false;
                        break;
                    case FormControlType.FILE:
                        if (!checkFile(i, entry.Key, out field))
                            return false;
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
                return;
            }

            string token = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"];
            if (token != null) 
                values.Add(new KeyValuePair<string, string>("token", token));
            
                Debug.WriteLine("token : " + token);
            Debug.WriteLine("url : " + (string)Model.Values[infos.createdModelType]["URL"]);
            foreach(KeyValuePair<string, string> na in values)
            {
                Debug.WriteLine("Key : " + na.Key + " value : " + na.Value);
            }

            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response;
                if (infos.isCreation)
                    response = await httpClient.PostAsync(Global.API_IRL + (string)Model.Values[infos.createdModelType]["URL"], new FormUrlEncodedContent(values));
                else
                    response = await httpClient.PutAsync(Global.API_IRL + (string)Model.Values[infos.createdModelType]["URL"] + infos.id.ToString(), new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(responseString);

                //returned = JsonConvert.DeserializeObject<AssociationRequest>(responseString);
                if ((int)jObject["status"] != 200)
                {
                    infosBox.Text = (string)jObject["message"];
                }
                else
                {
                    InfosModel tmp = new InfosModel();
                    tmp.id = (int)jObject["response"]["id"];
                    tmp.type = infos.createdModelType;
                    uploadLogo(tmp);
                }
            }
            catch (HttpRequestException err)
            {
                infosBox.Text = err.Message;
            }
            catch (System.InvalidOperationException err)
            {
                Debug.WriteLine("wtf :  " + err.Message);
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

        public Grid addFileType(string type, Grid tmp)
        {
            var row2 = new RowDefinition();
            row2.Height = new GridLength(60);

            var row3 = new ColumnDefinition();
            row3.Width = new GridLength(110);

            tmp.RowDefinitions.Add(row2);
            tmp.ColumnDefinitions.Add(row3);
            tmp.ColumnDefinitions.Add(new ColumnDefinition());

            // a voir a remplace par un bouton
            Button btn = new Button();
            btn.BorderThickness = new Thickness(10, 0, 0, 0);
            btn.Content = "Choisir";
            btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 83, 166, 52));
            btn.Click += chooseFileClick;
            btn.Width = 100;
            Grid.SetColumn(btn, 0);
            Grid.SetRow(btn, 1);
            tmp.Children.Add(btn);

            TextBox image = new TextBox();
            image.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 191, 202, 204));
            image.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
            image.Margin = new Thickness(10, 10, 10, 10);
            image.TextWrapping = TextWrapping.Wrap;
            filePath.PathToImage = type;
            image.Text = filePath.PathToImage;
            image.GotFocus += searchTextBox_GotFocus;
            

            Binding myBinding = new Binding();
            myBinding.Source = filePath;
            myBinding.Path = new PropertyPath("PathToImage");
            myBinding.Mode = BindingMode.TwoWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            BindingOperations.SetBinding(image, TextBox.TextProperty, myBinding);

            Grid.SetColumn(image, 1);
            Grid.SetRow(image, 1);
            tmp.Children.Add(image);

            return tmp;

        }

        public Grid addDateType(string date, Grid tmp)
        {
            var row2 = new RowDefinition();
            row2.Height = new GridLength(60);
            tmp.RowDefinitions.Add(row2);

            var col1 = new ColumnDefinition();
            col1.Width = new GridLength(60);
            tmp.ColumnDefinitions.Add(col1);
            tmp.ColumnDefinitions.Add(new ColumnDefinition());

            // a voir a remplace par un bouton
            Image btn = new Image();
            btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/date.png"));
            btn.Margin = new Thickness(10, 0, 0, 0);

            Grid.SetColumn(btn, 0);
            Grid.SetRow(btn, 1);
            tmp.Children.Add(btn);

            DatePicker field = new DatePicker();
            field.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
            field.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
            field.Margin = new Thickness(10, 0, 10, 0);

            try
            {
                string[] begin = date.Split('-');
                field.Date = (DateTime)Convert.ToDateTime(begin[1] + '/' + begin[2] + '/' + begin[0]);
            }
            catch (System.IndexOutOfRangeException e)
            {

            }

            Grid.SetColumn(field, 1);
            Grid.SetRow(field, 1);
            tmp.Children.Add(field);

            return tmp;
        }

        public Grid addDescribeType(string description, Grid tmp)
        {
            var row2 = new RowDefinition();
            row2.Height = new GridLength(220);
            tmp.RowDefinitions.Add(row2);

            var col1 = new ColumnDefinition();
            tmp.ColumnDefinitions.Add(col1);

            TextBox field = new TextBox();
            field.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 191, 202, 204));
            field.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
            field.Margin = new Thickness(10, 10, 10, 10);
            //field.BorderThickness = new Thickness(0, 0, 0, 0);
            field.Text = description;
            field.Height = 200;
            field.TextWrapping = TextWrapping.Wrap;
            field.GotFocus += searchTextBox_GotFocus;

            Grid.SetColumn(field, 0);
            Grid.SetRow(field, 1);
            tmp.Children.Add(field);
            return tmp;

        }

        public Grid addFieldType(string title, Grid tmp)
        {
            var row2 = new RowDefinition();
            row2.Height = new GridLength(60);
            tmp.RowDefinitions.Add(row2);

            var col1 = new ColumnDefinition();
            col1.Width = new GridLength(60);
            tmp.ColumnDefinitions.Add(col1);
            tmp.ColumnDefinitions.Add(new ColumnDefinition());

            // a voir a remplace par un bouton
            Image btn = new Image();
            btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
            btn.Margin = new Thickness(10, 0, 0, 0);


            Grid.SetColumn(btn, 0);
            Grid.SetRow(btn, 1);
            tmp.Children.Add(btn);

            TextBox field = new TextBox();
            field.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 191, 202, 204));
            field.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
            field.Margin = new Thickness(10, 10, 10, 10);
            field.TextWrapping = TextWrapping.Wrap;
            field.Text = title;
            field.GotFocus += searchTextBox_GotFocus;


            Grid.SetColumn(field, 1);
            Grid.SetRow(field, 1);
            tmp.Children.Add(field);

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
                    infosBox.Text = (string)jObject["message"];
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
                formsGrid.RowDefinitions.Add(new RowDefinition());
                Border border = new Border();
                border.Margin = new Thickness(10, 10, 10, 10);
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 191, 202, 204));
                border.BorderThickness = new Thickness(1.5, 1.5, 1.5, 1.5);
                border.CornerRadius = new CornerRadius(5);
                Grid tmp = new Grid();
                border.Child = tmp;
                tmp.Background = new SolidColorBrush(Color.FromArgb(0xFF, 255, 255, 255));
               // tmp.Margin = new Thickness(10, 10, 10, 10);


                var row = new RowDefinition();
                row.Height = new GridLength(60);
                tmp.RowDefinitions.Add(row);

                TextBox type = new TextBox();
                type.Text = entry.Key;
                type.FontWeight = FontWeights.Bold;
                type.HorizontalAlignment = HorizontalAlignment.Left;
                type.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));

                Grid.SetColumn(type, 0);
                Grid.SetColumnSpan(type, 2);
                Grid.SetRow(type, 0);
                tmp.Children.Add(type);
                string resources = entry.Key;
                if (values != null)
                {
                    //resources = values[(string)(Model.Values[infos.createdModelType][entry.Key]).ToSt];
                    resources = values.First(x => x.Key.Equals((string)(Model.Values[infos.createdModelType][entry.Key]), StringComparison.Ordinal)).Value;
                }
                switch (entry.Value)
                {
                    case FormControlType.FIELD:
                        tmp = addFieldType(resources, tmp);
                        break;
                    case FormControlType.DESCRIPTION:
                        tmp = addDescribeType(resources, tmp);
                        break;
                    case FormControlType.DATE:
                        tmp = addDateType(resources, tmp);
                        break;
                    case FormControlType.FILE:
                        tmp = addFileType(resources, tmp);
                        break;
                    case FormControlType.HOUR:
                        tmp = addHourType(resources, tmp);
                        break;
                    case FormControlType.PASSWORD:
                        tmp = addPasswordType(resources, tmp);
                        break;
                    case FormControlType.CHECKFIELD:
                        tmp = addCheckField(resources, tmp);
                        break;
                }
                Grid.SetColumn(border, 0);
                Grid.SetRow(border, i);

                formsGrid.Children.Add(border);

                i++;
            }
            if (infos.isAdmin)
                createButtonType(formsGrid, i);
            scrollButton.Content = formsGrid;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as FormModel;

            filePath = new ImageSourcePath();
            filePath.PathToImage = "Logo";
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
                title.Text = Model.Values[infos.createdModelType]["CreationType"];
            }

            else
            {
                title.Text = "Informations";
                infosBox.Text = Model.Values[infos.createdModelType]["Name"];
            }
            initFormulaire();
        }
    }
}
