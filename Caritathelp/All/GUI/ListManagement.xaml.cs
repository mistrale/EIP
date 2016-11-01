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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI
{
    public sealed partial class ListManagement : UserControl
    {
        Page page;
        private GUI.ErrorControl err;
        private Models.Model model;
        private int idUser;


        public ListManagement()
        {
            this.InitializeComponent();
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Models.InfosModel tmp = (Models.InfosModel)button.Tag;
            page.Frame.Navigate(typeof(Models.GenericProfil), tmp);
        }

        private async void KickButtonClick(object sender, RoutedEventArgs e)
        {
            string url = Models.Model.Values[model.getType()]["KickURL"] + "?" + Models.Model.Values[model.getType()]["TypeID"]
                + "=" + model.getID() + "&volunteer_id=" + idUser;
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest(url, null, HttpHandler.TypeRequest.DELETE);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], ErrorControl.Code.FAILURE);
            }
            else
            {
                page.Frame.Navigate(typeof(Models.GenericListModelManagement), model);
            }
        }

        private async void selectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selected = (string)comboBox.SelectedItem;
            string url = Models.Model.Values[model.getType()]["UpgradeURL"];
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(Models.Model.Values[model.getType()]["TypeID"], model.getID().ToString()),
                        new KeyValuePair<string, string>("rights", selected.ToLower()),
                        new KeyValuePair<string, string>("volunteer_id", idUser.ToString()),
                    };
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest(url, values, HttpHandler.TypeRequest.PUT);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], ErrorControl.Code.FAILURE);
            } else
            {
                err.printMessage("Grade modifie !", ErrorControl.Code.SUCCESS);
            }
        }

        public ListManagement(Models.Model tmp, Newtonsoft.Json.Linq.JObject obj, GUI.ErrorControl err, Page page, bool canUpgrade)
        {
            this.InitializeComponent();
            this.err = err;
            model = tmp;
            this.page = page;

            userName.Content = (string)obj["firstname"] + " " + (string)obj["lastname"];
            idUser = (int)obj["id"];
            Models.InfosModel tag = new Models.InfosModel();
            tag.id = idUser;
            tag.type = "volunteer";
            userName.Tag = tag;
            if (!canUpgrade)
            {
                comboBox.Visibility = Visibility.Collapsed;
            } else
            {
                if (((string)obj["rights"]).Equals("member", StringComparison.Ordinal))
                    comboBox.SelectedIndex = 0;
                else if (((string)obj["rights"]).Equals("admin", StringComparison.Ordinal))
                    comboBox.SelectedIndex = 1;
                else
                    comboBox.SelectedIndex = 2;
            }
            comboBox.SelectionChanged += new SelectionChangedEventHandler(selectedIndexChanged);
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource =
                new BitmapImage(new Uri(Global.API_IRL + "" + obj["thumb_path"], UriKind.Absolute));
            logo.Fill = myBrush;
        }

        private void comboBox_DropDownOpened(object sender, object e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.IsDropDownOpen == true)
            {
                this.Height = 160;
            }
        }

        private void comboBox_DropDownClosed(object sender, object e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.IsDropDownOpen == false)
            {
                this.Height = 73;
            }
        }
    }
}
