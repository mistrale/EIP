using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.Volunteer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VolunteerInformations : Page
    {
        private UserStruct tmp;
        private string responseString;

        class RequeteResponse
        {

            public int status { get; set; }
            public string message { get; set; }
            public User response { get; set; }

        }

        class PictureRequest
        {
            public int status { get; set; }
            public string message { get; set; }
            public All.Picture response { get; set; }
        }

        private PictureRequest pictures;
        private RequeteResponse message;

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
            string url = "http://52.31.151.160:3000/pictures/";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("file", Base64String),
                        new KeyValuePair<string, string>("filename", storageFileWP.Name.ToString()),
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
                    Frame.Navigate(typeof(VolunteerProfil), tmp.id);
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

        public void goBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VolunteerProfil), tmp.id);
        }

        public VolunteerInformations()
        {
            this.InitializeComponent();
            warningTextBox.Text = "";
        }


        public static bool ValidateEmail(string str)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(str, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
        }

        private async void updateProfil()
        {
            TextBlock obj = (TextBlock)(GenreBox.SelectedItem);
            string genre = obj.Text.Equals("Femme", StringComparison.Ordinal) ? "f" : "m";
            String[] data = birthdayEdit.Date.ToString().Split(' ')[0].Split('/');
            string date = data[2] + '-' + data[0] + '-' + data[1] + "T00:00:00Z";
            string url = "http://52.31.151.160:3000/volunteers/" + ((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]);
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("mail", emailEdit.Text),
                        new KeyValuePair<string, string>("password", passwordEdit.Password),
                        new KeyValuePair<string, string>("firstname", firstNameEdit.Text),
                        new KeyValuePair<string, string>("lastname", lastNameEdit.Text),
                        new KeyValuePair<string, string>("birthday", date),
                        new KeyValuePair<string, string>("gender", genre),
                        new KeyValuePair<string, string>("allowgps", allowGPSEdit.IsChecked.ToString().ToLower()),
                        new KeyValuePair<string, string>("city", cityEdit.Text),
                        new KeyValuePair<string, string>("token", (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["token"])
                    };

            var httpClient = new HttpClient(new HttpClientHandler());
            try
            {
                HttpResponseMessage response = await httpClient.PutAsync(url, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
                message = JsonConvert.DeserializeObject<RequeteResponse>(responseString);
                if (message.status != 200)
                {
                    warningTextBox.Text = message.message;
                }
                else
                {
                    try
                    {
                        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                        localSettings.Values["mail"] = emailEdit.Text;
                        localSettings.Values["firstname"] = firstNameEdit.Text;
                        localSettings.Values["lastname"] = lastNameEdit.Text;
                        localSettings.Values["city"] = cityEdit.Text;
                        localSettings.Values["birthday"] = birthdayEdit.Date.ToString();
                        localSettings.Values["gender"] = GenreBox.SelectedValue.ToString();
                        localSettings.Values["allowgps"] = allowGPSEdit.IsChecked.ToString();
                        localSettings.Values["password"] = passwordEdit.Password;
                    }
                    catch (System.Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }
                    warningTextBox.Text = "Profil modifiée !";
                    uploadLogo();
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
                Debug.WriteLine("Fail in update profil");
            }
        }

        private bool checkRegistrationField()
        {
            if (lastNameEdit.Text == String.Empty || lastNameEdit.Text.Equals("Nom", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Name empty.";
                return false;
            }

            if (firstNameEdit.Text == String.Empty || firstNameEdit.Text.Equals("Prénom", StringComparison.Ordinal))
            {
                warningTextBox.Text = "FirstName empty.";
                return false;
            }
            if (emailEdit.Text == String.Empty || emailEdit.Text.Equals("Email", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Email empty.";
                return false;
            }
            if (!ValidateEmail(emailEdit.Text))
            {
                warningTextBox.Text = "Invalid email.";
                return false;
            }
            if (passwordEdit.Password == String.Empty || passwordEdit.Password.Equals("Password", StringComparison.Ordinal))
            {
                warningTextBox.Text = "Password empty.";
                return false;
            }
            if (passwordConfirmationEdit.Password == String.Empty
                || passwordConfirmationEdit.Password.Equals("Confirmation Password", StringComparison.Ordinal))
            {
                warningTextBox.Text = "CofirmationPassword empty.";
                return false;
            }
            if (!passwordEdit.Password.Equals(passwordConfirmationEdit.Password, StringComparison.Ordinal))
            {
                warningTextBox.Text = "Password doesn't match.";
                return false;
            }
            return true;
        }

        private void editProfilButton_Click(object sender, RoutedEventArgs e)
        {
            if (!checkRegistrationField())
                return;
            warningTextBox.Text = "";
            updateProfil();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tmp = e.Parameter as UserStruct;
            if (!tmp.id.Equals((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"], StringComparison.Ordinal))
            {
                editProfilButton.Visibility = Visibility.Collapsed;
                passwordConfirmationEdit.Visibility = Visibility.Collapsed;
                passwordConfirmationText.Visibility = Visibility.Collapsed;
                passwordEdit.Visibility = Visibility.Collapsed;
                passwordText.Visibility = Visibility.Collapsed;
                cityEdit.IsReadOnly = true;
                emailEdit.IsReadOnly = true;
                firstNameEdit.IsReadOnly = true;
                lastNameEdit.IsReadOnly = true;
                emailEdit.IsHitTestVisible = false;
                firstNameEdit.IsHitTestVisible = false;
                lastNameEdit.IsHitTestVisible = false;
                birthdayEdit.IsHitTestVisible = false;
                GenreBox.IsHitTestVisible = false;
                cityEdit.IsHitTestVisible = false;
                allowGPSEdit.IsHitTestVisible = false;
                logoText.Visibility = Visibility.Collapsed;
                button1.Visibility = Visibility.Collapsed;
            }
            emailEdit.Text = tmp.user.mail;
            firstNameEdit.Text = tmp.user.firstname;
            lastNameEdit.Text = tmp.user.lastname;
            passwordEdit.Password = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["password"];
            passwordConfirmationEdit.Password = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["password"];
            if (tmp.user.city != null)
                cityEdit.Text = tmp.user.city;
            if (tmp.user.birthday != null)
            {
                string[] begin = tmp.user.birthday.Split('-');
                birthdayEdit.Date = (DateTime)Convert.ToDateTime(begin[1] + '/' + begin[2] + '/' + begin[0]);
            }
            if (tmp.user.birthday != null)
                birthdayEdit.Date = (DateTime)Convert.ToDateTime(tmp.user.birthday);
            if (tmp.user.gender != null)
            {
                if (tmp.user.gender.Equals("f", StringComparison.Ordinal))
                    GenreBox.SelectedIndex = 0;
                else if (tmp.user.gender.Equals("m", StringComparison.Ordinal))
                    GenreBox.SelectedIndex = 1;
            }
            if (tmp.user.allowgps)
                allowGPSEdit.IsChecked = true;
            else
                allowGPSEdit.IsChecked = false;
        }
    }
}
