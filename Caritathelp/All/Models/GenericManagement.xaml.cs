using System;
using System.Collections.Generic;
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
            tmp.modelType = model.getType();
            tmp.createdModelType = "assoc";
            tmp.id = model.getID();
            tmp.isAdmin = true;
            tmp.isCreation = true;
            Frame.Navigate(typeof(GenericCreationModel), tmp);
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
            this.Frame.Navigate(typeof(GenericListModelManagement), model);
        }

        public void deleteRessourceClick(object sender, RoutedEventArgs e)
        {

        }

        public void initButtonsManagement()
        {
            buttonGrid = new Grid();
            buttonGrid.VerticalAlignment = VerticalAlignment.Top;
            buttonGrid.ColumnDefinitions.Add(new ColumnDefinition());
            Dictionary<string, Models.ButtonManagement> buttonsList = model.getButtonsManagement();
            int i = 0;
            foreach (KeyValuePair<string, ButtonManagement> entry in buttonsList)
            {
                buttonGrid.RowDefinitions.Add(new RowDefinition());
                GUI.ManagementButton btn = new GUI.ManagementButton();
                btn.Margin = new Thickness(0, 0, 0, 15);
                ButtonManagement type = entry.Value;
                switch (type)
                {
                    case ButtonManagement.CREATE_RESOURCE:
                        btn.setControls(entry.Key, createRessourceClick);
                        break;
                    case ButtonManagement.DELETE_RESOURCE:
                        btn.setControls(entry.Key, deleteRessourceClick);
                        break;
                    case ButtonManagement.MANAGE_INVITATION:
                        btn.setControls(entry.Key, manageInvitationClick);
                        break;
                    case ButtonManagement.MANAGE_RELATION:
                        btn.setControls(entry.Key, manageRelationClick);
                        break;
                    case ButtonManagement.GET_NOTIFICATION:
                        btn.setControls(entry.Key, getNotification);
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
