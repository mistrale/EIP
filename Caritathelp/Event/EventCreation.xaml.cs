using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
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

namespace Caritathelp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EventCreation : Page
    {
        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        class EventRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public EventModel response { get; set; }
        }

        private PictureRequest pictures;
        private string responseString;
        private EventRequest eventresponse;
        private string event_id;

        public EventCreation()
        {
            this.InitializeComponent();
        }

        private void viewActivated(CoreApplicationView sender, IActivatedEventArgs args1)
        {
            FileOpenPickerContinuationEventArgs args = args1 as FileOpenPickerContinuationEventArgs;

            if (args != null)
            {
                if (args.Files.Count == 0) return;

                view.Activated -= viewActivated;
                storageFileWP = args.Files[0];

            }
            logoText.Text = storageFileWP.Name.ToString();
        }

        CoreApplicationView view;
        StorageFile storageFileWP;

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
                Frame.Navigate(typeof(EventProfil), event_id);
                return;
            }
            Debug.WriteLine("TATA" + Base64String);
            string url = Global.API_IRL + "/pictures/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("file", Base64String),
                        new KeyValuePair<string, string>("filename", storageFileWP.Name.ToString()),
                        new KeyValuePair<string, string>("event_id", event_id),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                pictures = JsonConvert.DeserializeObject<PictureRequest>(responseString);
                if (pictures.status != 200)
                {
                    warningTextBox.Text = pictures.message;
                }
                else
                {
                    Frame.Navigate(typeof(EventProfil), event_id);
                }
            }
            catch (HttpRequestException e)
            {
                warningTextBox.Text = e.Message;
                Debug.WriteLine(e.Message);
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public async void createEvent()
        {
            String[] data = beginDate.Date.ToString().Split(' ')[0].Split('/');
            string date = data[2] + '-' + data[0] + '-' + data[1] + 'T' + timeBegin.Time.ToString() + 'Z';
            String[] data1 = endDate.Date.ToString().Split(' ')[0].Split('/');
            string date1 = data1[2] + '-' + data1[0] + '-' + data1[1] + 'T' + timeEnd.Time.ToString() + 'Z';
            string url = Global.API_IRL + "/events/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("assoc_id", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["currentAssociation"]),
                        new KeyValuePair<string, string>("title", eventTitleText.Text),
                        new KeyValuePair<string, string>("description", eventDescriptionText.Text),
                        new KeyValuePair<string, string>("place", eventPlaceText.Text),
                        new KeyValuePair<string, string>("begin", date),
                        new KeyValuePair<string, string>("end", date1),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };
            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseString);
                eventresponse = JsonConvert.DeserializeObject<EventRequest>(responseString);
                if (eventresponse.status != 200)
                {
                    warningTextBox.Text = eventresponse.message;
                }
                else
                {
                    event_id = eventresponse.response.id.ToString();
                    uploadLogo();
                }
            }
            catch (HttpRequestException e)
            {
                warningTextBox.Text = e.Message;
            }
            catch (JsonReaderException e)
            {
                System.Diagnostics.Debug.WriteLine(responseString);
            }
            catch (JsonSerializationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public bool checkCreationEvent()
        {
            if (eventTitleText.Text == String.Empty || eventTitleText.Text.Equals("Titre de l'évènement", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Titre vide.";
                return false;
            }
            if (eventDescriptionText.Text == String.Empty || eventDescriptionText.Text.Equals("Description", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Description vide.";
                return false;
            }
            return true;
        }

        public void createEventClick(object send, RoutedEventArgs e)
        {
            if (!checkCreationEvent())
                return;
            warningTextBox.Text = "";
            createEvent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            warningTextBox.Text = "";
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(All.Research));
        }

    }
}
