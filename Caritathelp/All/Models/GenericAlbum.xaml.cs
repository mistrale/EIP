using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
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
    public sealed partial class GenericAlbum : Page
    {
        public PictureModel infos;
        public Grid grid;

        CoreApplicationView view;
        StorageFile storageFileWP;

        private void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                storageFileWP = args.Files[0];

            }
            textBox.Text = storageFileWP.Name.ToString();
        }

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

        private async void uploadLogo()
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
                return;
            }
            string url = Global.API_IRL + "/pictures/";
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("file", Base64String),
                        new KeyValuePair<string, string>("filename", storageFileWP.Name.ToString()),
                        new KeyValuePair<string, string>(Model.Values[infos.model.getType()]["TypeID"], infos.model.getID().ToString()),

                    };
            Newtonsoft.Json.Linq.JObject res = await http.sendRequest("/pictures", values, HttpHandler.TypeRequest.POST);
            if ((int)res["status"] == 200) {
                Frame.Navigate(typeof(GenericAlbum), infos);
            } else
            {
                err.printMessage((string)res["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        public GenericAlbum()
        {
            this.InitializeComponent();
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
        }

        private async void getPictures()
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest(Model.Values[infos.model.getType()]["URL"] + infos.model.getID() + "/pictures", null, HttpHandler.TypeRequest.GET);
            if ((int)jObject["status"] == 200)
            {
                Newtonsoft.Json.Linq.JArray newsResponse = (Newtonsoft.Json.Linq.JArray)jObject["response"];
                grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                grid.VerticalAlignment = VerticalAlignment.Top;
                for (int i = 0; i < newsResponse.Count; i++)
                {
                    grid.RowDefinitions.Add(new RowDefinition());
                    GUI.PictureControl pic = new GUI.PictureControl((Newtonsoft.Json.Linq.JObject)(newsResponse[i]), infos, cfBox);
                    pic.Margin = new Thickness(0, 0, 0, 20);
                    Grid.SetColumn(pic, 0);
                    Grid.SetRow(pic, i);
                    grid.Children.Add(pic);
                }
                scroll.Content = grid;
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = e.Parameter as PictureModel;
            if (!infos.isAdmin)
            {
                addPicture.Visibility = Visibility.Collapsed;
                scroll.Margin = new Thickness(10, 69, 0, 0);
                scroll.Height = 511;
            }
            getPictures();
        }

        private void addPictureButton_Click(object sender, RoutedEventArgs e)
        {
            if (!textBox.Text.Equals("", StringComparison.Ordinal) && !textBox.Text.Equals("Choisir une image ...", StringComparison.Ordinal))
            {
                uploadLogo();
            }
        }
    }
}
