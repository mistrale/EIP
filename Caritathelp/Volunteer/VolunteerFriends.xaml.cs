﻿using System;
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

namespace Caritathelp.Volunteer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VolunteerFriends : Page
    {
        Grid friendsGrid;
        friendsStruct tmp;

        public void goBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VolunteerProfil), tmp.id);
        }

        public VolunteerFriends()
        {
            this.InitializeComponent();
        }

        private void UserButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int i = Convert.ToInt32(button.Tag.ToString());
            string id = tmp.friends[i].id.ToString();
            Frame.Navigate(typeof(VolunteerProfil), id);
            // identify which button was clicked and perform necessary actions
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tmp = e.Parameter as friendsStruct;
            friendsGrid = new Grid();
            friendsGrid.ColumnDefinitions.Add(new ColumnDefinition());
            friendsGrid.VerticalAlignment = VerticalAlignment.Top;
            friendsGrid.Height = tmp.friends.Count * 100;
            friendsGrid.Width = 375;
            for (int x = 0; x < tmp.friends.Count; ++x)
            {
                friendsGrid.RowDefinitions.Add(new RowDefinition());
                Grid grid = new Grid();
                grid.Margin = new Thickness(0, 10, 0, 10);
                grid.Background = new SolidColorBrush(Color.FromArgb(250, 75, 175, 80));

                // row
                var row = new RowDefinition();
                row.Height = new GridLength(50);
                grid.RowDefinitions.Add(row);

                var row2 = new RowDefinition();
                row2.Height = new GridLength(40);
                grid.RowDefinitions.Add(row2);

                //column
                var colum = new ColumnDefinition();
                colum.Width = new GridLength(80);
                grid.ColumnDefinitions.Add(colum);

                var column2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column2);


                // Username
                Button title = new Button();
                title.Tag = x;
                title.Content = tmp.friends[x].firstname + " " + tmp.friends[x].lastname;
                title.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                title.Click += new RoutedEventHandler(UserButtonClick);
                title.FontSize = 14;
                title.Height = 50;
                title.Width = 200;
                title.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetColumn(title, 1);
                Grid.SetRow(title, 0);
                grid.Children.Add(title);

                //Ami en commun
                TextBlock friend = new TextBlock();
                friend.Text = "2 amis en commun";
                friend.VerticalAlignment = VerticalAlignment.Top;
                friend.Foreground = new SolidColorBrush(Color.FromArgb(250, 250, 250, 250));
                friend.FontSize = 14;
                friend.Height = 30;
                friend.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetColumn(friend, 1);
                Grid.SetRow(friend, 1);
                grid.Children.Add(friend);

                Image btn = new Image();
                btn.Source = new BitmapImage(new Uri("ms-appx:/Assets/avatar.png"));
                btn.Height = 80;
                btn.Width = 80;
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                btn.HorizontalAlignment = HorizontalAlignment.Center;
                grid.Children.Add(btn);

                friendsGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, x);
            }
            friendsGrid.Visibility = Visibility.Visible;
            scroll.Content = friendsGrid;
        }
    }
}
