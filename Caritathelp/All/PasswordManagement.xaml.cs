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

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PasswordManagement : Page
    {
        public PasswordManagement()
        {
            this.InitializeComponent();
            passwordBox1.Password = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["password"];
            passwordBox2.Password = (string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["password"];

        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox1.Password.Equals((string)Windows.Storage.ApplicationData.Current.LocalSettings.Values["password"], StringComparison.Ordinal))
            {
                err.printMessage("Vous devew choisir un mot de passe différent de l'ancien.", GUI.ErrorControl.Code.FAILURE);
                return;
            }
            if (!passwordBox1.Password.Equals(passwordBox2.Password, StringComparison.Ordinal))
            {
                err.printMessage("Mot de passes différents.", GUI.ErrorControl.Code.FAILURE);
                return;
            }
            if (passwordBox1.Password.Length < 8)
            {
                err.printMessage("Votre mot de passe doit faire au minimum 8 charactères.", GUI.ErrorControl.Code.FAILURE);
                return;
            }
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("password", passwordBox1.Password),
                        new KeyValuePair<string, string>("password_confirmation", passwordBox2.Password)
                    };

            Newtonsoft.Json.Linq.JObject jObject = await http.sendRequest("/auth/password", values, HttpHandler.TypeRequest.PUT);
            if ((int)jObject["status"] != 200)
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            } else
            {
                err.printMessage("Mot de passe modifié !", GUI.ErrorControl.Code.SUCCESS);
                Windows.Storage.ApplicationData.Current.LocalSettings.Values["password"] = passwordBox1.Password;
            }
        }
    }
}
