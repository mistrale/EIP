using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class UpdageMessage : Page
    {
        MessageInfos infos;

        public UpdageMessage()
        {
            this.InitializeComponent();
        }

        private List<int> idVolunter;
        private List<int> addVolunteer;
        private List<int> removeVolunteer;

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
                        addVolunteer.Add((int)list[0]["id"]);
                    btn.FontWeight = FontWeights.Bold;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.HorizontalAlignment = HorizontalAlignment.Left;
                    btn.Click += RemoveAddedUser;
                    btn.Tag = (int)list[0]["id"];
                    Grid.SetColumn(btn, 1);
                    idVolunter.Add((int)list[0]["id"]);

                    Grid.SetRow(btn, volunteerGrid.RowDefinitions.Count - 1);
                    volunteerGrid.RowDefinitions.Add(new RowDefinition());
                    volunteerGrid.Children.Add(btn);
                    userBox.Text = "Ajouter...";
                }
            }
        }

        public void RemoveAddedUser(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int i = (int)btn.Tag;
            removeVolunteer.Add(i);
            Debug.WriteLine("i = " + i + " contetnt = " + btn.Content );
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
            bool success = true;

            Newtonsoft.Json.Linq.JObject obj;
            HttpHandler http = HttpHandler.getHttp();
            for (int i = 0; i < removeVolunteer.Count; i++)
            {
                Debug.WriteLine("VOLUNTEER TO REMOVE : " + removeVolunteer[i]);

                obj = await http.sendRequest("/chatrooms/" + infos.id + "/kick?volunteer_id=" + removeVolunteer[i], null, HttpHandler.TypeRequest.DELETE);
                if ((int)obj["status"] != 200)
                {
                    err.printMessage((string)obj["message"], All.GUI.ErrorControl.Code.FAILURE);
                    success = false;
                }
            }
            //obj = await http.sendRequest("/chatrooms/" + infos.id + "/set_name")
            string volunteers = "[";
            for (int i = 0; i < addVolunteer.Count; i++)
            {
                Debug.WriteLine("VOLUNTEER TO AD : " + addVolunteer[i]);
                if (i + 1 == addVolunteer.Count)
                {
                    volunteers += addVolunteer[i];
                }
                else
                {
                    volunteers += addVolunteer[i] + ",";
                }
            }
            volunteers += "]";
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("volunteers[]", volunteers)
                    };
            obj = await http.sendRequest("/chatrooms/" + infos.id + "/add_volunteers", values, HttpHandler.TypeRequest.PUT);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], All.GUI.ErrorControl.Code.FAILURE);
                success = false;
            }

            if (success)
                err.printMessage("Modification enregistree !", All.GUI.ErrorControl.Code.SUCCESS);

        }

        public void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        public async void initUpdateMessage()
        {
            titleBox.Text = infos.name;
            for (int i = 0; i < infos.volunteers.Count; i++)
            {
                HttpHandler http = HttpHandler.getHttp();
                var template = "/search?research=" + (string)infos.volunteers[i];
                Newtonsoft.Json.Linq.JObject obj = await http.sendRequest(template, null, HttpHandler.TypeRequest.GET);
                if ((int)obj["status"] != 200)
                {
                    err.printMessage((string)obj["message"], All.GUI.ErrorControl.Code.FAILURE);
                    continue;
                }
                Newtonsoft.Json.Linq.JArray list = (Newtonsoft.Json.Linq.JArray)obj["response"];
                if (list.Count < 1)
                {
                    continue;
                }
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
            }

        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            infos = (e.Parameter as MessageInfos);
            volunteerGrid = new Grid();
            volunteerGrid.VerticalAlignment = VerticalAlignment.Top;
            volunteerGrid.ColumnDefinitions.Add(new ColumnDefinition());
            volunteerGrid.Width = 380;
            scroll.Content = volunteerGrid;
            idVolunter = new List<int>();
            addVolunteer = new List<int>();
            removeVolunteer = new List<int>();
            initUpdateMessage();

        }
    }
}
