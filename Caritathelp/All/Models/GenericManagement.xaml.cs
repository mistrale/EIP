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
        private InfosModel infos;
        private Model model;
        private Grid buttonGrid = new Grid();

        public GenericManagement()
        {
            this.InitializeComponent();
        }

        public void createRessourceClick(object sender, RoutedEventArgs e)
        {
            FormModel tmp = new FormModel();
            tmp.modelType = infos.type;
            tmp.createdModelType = "assoc";
            tmp.id = infos.id;
            tmp.isAdmin = true;
            tmp.isCreation = true;
            Frame.Navigate(typeof(GenericCreationModel), tmp);
        }

        public void manageInvitationClick(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GenericInvitation), infos);
        }

        public void getNotification(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GenericNotification), infos);
        }

        public void manageRelationClick(object sender, RoutedEventArgs e)
        {

        }

        public void deleteRessourceClick(object sender, RoutedEventArgs e)
        {

        }

        public void initButtonsManagement()
        {
            buttonGrid = new Grid();
            buttonGrid.VerticalAlignment = VerticalAlignment.Top;
            buttonGrid.Margin = new Thickness(10, 10, 10, 10);
            var columnImage = new ColumnDefinition();
            columnImage.Width = new GridLength(100);
            buttonGrid.ColumnDefinitions.Add(columnImage);

            var otherColumn = new ColumnDefinition();
            otherColumn.Width = new GridLength(270);
            buttonGrid.ColumnDefinitions.Add(otherColumn);
            Dictionary<string, Models.ButtonManagement> buttonsList = model.getButtonsManagement();
            int i = 0;
            foreach (KeyValuePair<string, ButtonManagement> entry in buttonsList)
            {
                buttonGrid.RowDefinitions.Add(new RowDefinition());

                Image image = new Image();
                image.Height = 100;
                image.Source = new BitmapImage(new Uri("ms-appx:/Assets/logo.png"));
                // do something with entry.Value or entry.Key

                Grid.SetColumn(image, 0);
                Grid.SetRow(image, i);
                buttonGrid.Children.Add(image);

                Button button = new Button();
                button.Margin = new Thickness(0, 0, 0, 0);
                button.Height = 115;
                button.Width = 280;
                button.Content = entry.Key;
                button.Background = new SolidColorBrush(Color.FromArgb(250, 255, 255, 255));
                button.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
                ButtonManagement type = entry.Value;
                switch (type)
                {
                    case ButtonManagement.CREATE_RESOURCE:
                        button.Click += createRessourceClick;
                        break;
                    case ButtonManagement.DELETE_RESOURCE:
                        button.Click += deleteRessourceClick;
                        break;
                    case ButtonManagement.MANAGE_INVITATION:
                        button.Click += manageInvitationClick;
                        break;
                    case ButtonManagement.MANAGE_RELATION:
                        button.Click += manageRelationClick;
                        break;
                    case ButtonManagement.GET_NOTIFICATION:
                        button.Click += getNotification;
                        break;
                }
                Grid.SetColumn(button, 1);
                Grid.SetRow(button, i);
                buttonGrid.Children.Add(button);
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
            infos = e.Parameter as InfosModel;
            if (infos.type.Equals("assoc", StringComparison.Ordinal))
            {
                model = new Association(infos.id);
                initButtonsManagement();
            }
            else if (infos.type.Equals("event", StringComparison.Ordinal))
            {
                model = new Event(infos.id);
                initButtonsManagement();
            }
        }
    }
}
