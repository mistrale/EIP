using Caritathelp.All.Models;
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
        private Models.InfosListModel infos;
        private int idUser;


        public ListManagement()
        {
            this.InitializeComponent();
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Models.Model tmp = (Models.Model)button.Tag;
            if (tmp.getType().Equals("shelter", StringComparison.Ordinal))
            {
                FormModel form = new FormModel();
                form.id = tmp.getID();
                form.createdModelType = tmp;
                form.isCreation = false;
                form.modelType = tmp;
                form.isAdmin = true;
                page.Frame.Navigate(typeof(All.Models.GenericCreationModel), form);
            }
            else
            {
                page.Frame.Navigate(typeof(All.Models.GenericProfil), tmp);
            }
        }

        private async void KickButtonClick(object sender, RoutedEventArgs e)
        {
            var routes = new Dictionary<string, Dictionary<string, string>>
            {
                {"assoc", new Dictionary<string, string>
                    {
                        { "volunteer", "/membership/kick"},
                        { "shelter", "/shelters"},
                    }
                },
                {"volunteer", new Dictionary<string, string>
                    {
                        { "volunteer", "/friendship/remove"},
                        { "assoc", "/membership/leave"},
                        { "event", "/guests/leave"},
                    }
                },
                {"event", new Dictionary<string, string>
                    {
                        { "volunteer", "/guests/kick"},
                    }
                }
           };

            string url = "";
            if (infos.listTypeModel.Equals("shelter", StringComparison.Ordinal))
            {
                url = "/shelters/" + idUser + "?assoc_id=" + infos.id;
            }
            else if (!infos.typeModel.Equals("volunteer", StringComparison.Ordinal))
            {
                 url = routes[infos.typeModel][infos.listTypeModel] + "?" + Models.Model.Values[infos.typeModel]["TypeID"]
                    + "=" + infos.id + "&volunteer_id=" + idUser;
            } else
            {
                url = routes[infos.typeModel][infos.listTypeModel] + "?" + Models.Model.Values[infos.listTypeModel]["TypeID"]
                   + "=" + idUser;
            }

            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject obj = await http.sendRequest(url, null, HttpHandler.TypeRequest.DELETE);
            if ((int)obj["status"] != 200)
            {
                err.printMessage((string)obj["message"], ErrorControl.Code.FAILURE);
            }
            else
            {
                page.Frame.Navigate(typeof(Models.GenericListModelManagement), infos);
            }
        }

        private async void selectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            string selected = (string)comboBox.SelectedItem;
            string url = Models.Model.Values[infos.typeModel]["UpgradeURL"];
            var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>(Models.Model.Values[infos.typeModel]["TypeID"], infos.id.ToString()),
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

        public ListManagement(Models.InfosListModel tmp, Newtonsoft.Json.Linq.JObject obj, GUI.ErrorControl err, Page page, bool canUpgrade)
        {
            this.InitializeComponent();
            this.err = err;
            infos = tmp;
            this.page = page;

            userName.Content = (string)obj[Models.Model.Values[tmp.listTypeModel]["NameType"]];
            idUser = (int)obj["id"];
            Models.Model tag = Models.Model.createModel(tmp.listTypeModel, idUser);
            userName.Tag = tag;
            if (!canUpgrade)
            {
                comboBox.Visibility = Visibility.Collapsed;
                userName.Width = 240;
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
            if (obj["thumb_path"] != null)
            {
                myBrush.ImageSource =
                    new BitmapImage(new Uri(Global.API_IRL + "" + obj["thumb_path"], UriKind.Absolute));
            } else
            {
                myBrush.ImageSource =
                    new BitmapImage(new Uri(Global.API_IRL + "" + "/uploads/picture/logo_caritathelp.png", UriKind.Absolute));
            }

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
