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
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.Message
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateMessage : Page
    {
        public CreateMessage()
        {
            this.InitializeComponent();
            volunteerGrid = new Grid();
            volunteerGrid.VerticalAlignment = VerticalAlignment.Top;
            volunteerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            volunteerGrid.Width = 380;
            scroll.Content = volunteerGrid;
            idVolunter = new List<int>();
        }

        private List<int> idVolunter;
        Grid volunteerGrid;

        private async void searchUsers()
        {
            HttpHandler http = HttpHandler.getHttp();
            var template = "/search?research=" + userBox.Text;
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest(template, null, HttpHandler.TypeRequest.GET);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], All.GUI.ErrorControl.Code.FAILURE);
            }
            else
            {
                Newtonsoft.Json.Linq.JArray list = (Newtonsoft.Json.Linq.JArray)obj["response"];

                if (list.Count == 0 ||
                    !((string)(list[0]["result_type"])).Equals("volunteer", StringComparison.Ordinal))
                    err.printMessage("Utilisateur non trouvé", All.GUI.ErrorControl.Code.FAILURE);
                else if (idVolunter.Count != 0 && idVolunter.Contains((int)list[0]["id"]))
                {
                    err.printMessage("Utilisateur deja ajoute", All.GUI.ErrorControl.Code.FAILURE);
                }
                else
                {
                    volunteerGrid.RowDefinitions.Add(new RowDefinition());
                    Button btn = new Button();
                    btn.FontSize = 14;
                    btn.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                    btn.Content = (string)list[0]["name"];
                    btn.FontWeight = FontWeights.Bold;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.HorizontalAlignment = HorizontalAlignment.Left;
                    btn.Click += RemoveAddedUser;
                    btn.Tag = (int)list[0]["id"];
                    Grid.SetColumn(btn, 1);
                    Grid.SetRow(btn, volunteerGrid.RowDefinitions.Count - 1);
                    volunteerGrid.RowDefinitions.Add(new RowDefinition());
                    volunteerGrid.Children.Add(btn);
                    idVolunter.Add((int)list[0]["id"]);
                    userBox.Text = "Ajouter...";
                }
            }
        }

        public void RemoveAddedUser(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int i = (int)btn.Tag;
            Debug.WriteLine("i = " + i);
            idVolunter.Remove(i);
            volunteerGrid.Children.Remove(btn);
        }

        public void addUserClick(object sender, RoutedEventArgs e)
        {
            if (!userBox.Text.Equals("Ajouter...", StringComparison.Ordinal)
                && !userBox.Text.Equals("", StringComparison.Ordinal))
                searchUsers();
        }

        public void createRoomClick(object sender, RoutedEventArgs e)
        {
            if (idVolunter.Count != 0)
                createRoom();
        }

        private async void createRoom()
        {
            string title = titleBox.Text;
            if (title == null || title.Equals("", StringComparison.Ordinal)
                || title.Equals("Titre", StringComparison.Ordinal))
            {
                title = null;
            }
            if (!idVolunter.Contains((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]))
            {
                idVolunter.Add((int)Windows.Storage.ApplicationData.Current.LocalSettings.Values["id"]);
            }
            string volunteers = "[";
            for (int i = 0; i < idVolunter.Count; i++)
            {
                if (i + 1 == idVolunter.Count)
                {
                    volunteers += idVolunter[i];
                } else
                {
                    volunteers += idVolunter[i] + ",";
                }
            }
            volunteers += "]";
            HttpHandler http = HttpHandler.getHttp();
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("name", title),
                        new KeyValuePair<string, string>("volunteers[]", volunteers)
                    };
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest("/chatrooms", values, HttpHandler.TypeRequest.POST);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], All.GUI.ErrorControl.Code.FAILURE);

            }
            else {
                ((Frame)Window.Current.Content).Navigate(typeof(Message));
            }
        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
