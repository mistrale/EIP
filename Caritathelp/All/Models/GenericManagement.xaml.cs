using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class GenericManagement : Page
    {
        private Model model;
        private Grid buttonGrid = new Grid();

        public GenericManagement()
        {
            this.InitializeComponent();
        }

        public void createRessourceClick(object sender, RoutedEventArgs e)
        {
            FormModel tmp = new FormModel();
            Button btn = (Button)sender;
            tmp.modelType = model;
            tmp.createdModelType = Model.createModel((string)btn.Tag, 0);
            tmp.id = model.getID();
            tmp.isAdmin = true;
            tmp.isCreation = true;
            Frame.Navigate(typeof(GenericCreationModel), tmp);
        }

        public void managePhotos(object sender, RoutedEventArgs e)
        {
            PictureModel infos = new PictureModel();
            infos.isAdmin = true;
            infos.model = model;
            this.Frame.Navigate(typeof(GenericAlbum), infos);
        }

        public void updatePasswordClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(PasswordManagement), model);
        }

        public void manageInvitationClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GenericInvitation), model);
        }

        public void getNotification(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GenericNotification), model);
        }

        public void manageRelationClick(object sender, RoutedEventArgs e)
        {
            InfosListModel tmp = new InfosListModel();
            Button btn = (Button)sender;
            tmp.id = model.getID();
            tmp.typeModel = model.getType();
            tmp.listTypeModel = (string)btn.Tag;
            this.Frame.Navigate(typeof(GenericListModelManagement), tmp);
        }

        public void deleteRessourceClick(object sender, RoutedEventArgs e)
        {
            cfBox.Visibility = Visibility.Visible;
            cfBox.setRoutedEvent(deleteRessourceClick_real);
        }


        public async void deleteRessourceClick_real(object sender, RoutedEventArgs e)
        {
            HttpHandler http = HttpHandler.getHttp();
            Newtonsoft.Json.Linq.JObject jObject;
            if (model.getType().Equals("volunteer", StringComparison.Ordinal))
            {
                jObject = await http.sendRequest(Model.Values[model.getType()]["DeleteURL"]
                + "?" + Model.Values[model.getType()]["TypeID"] + "=" + model.getID(), null, HttpHandler.TypeRequest.DELETE);
            }

            else
            {
                jObject = await http.sendRequest(Model.Values[model.getType()]["DeleteURL"] + "/" + model.getID()
+ "?" + Model.Values[model.getType()]["TypeID"] + "=" + model.getID(), null, HttpHandler.TypeRequest.DELETE);
            }

            if ((int)jObject["status"] == 200)
            {
                if (model.getType().Equals("volunteer", StringComparison.Ordinal))
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
                    Frame.Navigate(typeof(Accueil), model);
                }
            }
            else
            {
                err.printMessage((string)jObject["message"], GUI.ErrorControl.Code.FAILURE);
            }
        }

        public void initButtonsManagement()
        {
            buttonGrid = new Grid();
            buttonGrid.VerticalAlignment = VerticalAlignment.Top;
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Dictionary<string, Dictionary<string, ButtonManagement>> buttonsList = model.getButtonsManagement();
            int i = 0;
            foreach (KeyValuePair<string, Dictionary<string, ButtonManagement>> entry in buttonsList)
            {
                buttonGrid.RowDefinitions.Add(new RowDefinition());
                GUI.ManagementButton btn = new GUI.ManagementButton();
                btn.Margin = new Thickness(0, 0, 0, 15);
                ButtonManagement type = entry.Value.First().Value;               
                switch (type)
                {
                    case ButtonManagement.CREATE_RESOURCE:
                        btn.setControls(entry.Key, createRessourceClick, entry.Value.First().Key);
                        break;
                    case ButtonManagement.UPDATE_PASSWORD:
                        btn.setControls(entry.Key, updatePasswordClick, entry.Value.First().Key);
                        break;
                    case ButtonManagement.DELETE_RESOURCE:
                        btn.setControls(entry.Key, deleteRessourceClick, entry.Value.First().Key);
                        break;
                    case ButtonManagement.MANAGE_INVITATION:
                        btn.setControls(entry.Key, manageInvitationClick, entry.Value.First().Key);
                        break;
                    case ButtonManagement.MANAGE_RELATION:
                        btn.setControls(entry.Key, manageRelationClick, entry.Value.First().Key);
                        break;
                    case ButtonManagement.GET_NOTIFICATION:
                        btn.setControls(entry.Key, getNotification, entry.Value.First().Key);
                        break;
                    case ButtonManagement.MANAGE_ALBUM:
                        btn.setControls(entry.Key, managePhotos, entry.Value.First().Key);
                        break;
                }
                Grid.SetColumn(btn, 1);
                Grid.SetRow(btn, i);
                buttonGrid.Children.Add(btn);
                i++;
            }
            scrollButton.Content = buttonGrid;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            model = e.Parameter as Model;
            initButtonsManagement();
        }
    }
}
