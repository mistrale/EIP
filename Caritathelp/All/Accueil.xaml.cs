﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Tavis.UriTemplates;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Caritathelp.All
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accueil : Page
    { 
        private void initAccueil()
        {
            //Publication test = new Publication();
            //test.author = "Aude Sikorav";
            //test.date = "17h25";
            //test.content = "Bonjour a tous, voici une publication test pour voir combien en longeur elle va occupée et voir si Windows phone qui pue la merde gere tout seul la mise en place";
            //test.title = "Welcome in Caritathelp";

            //Comment comment = new Comment();
            //comment.autor = "Robin Vassuer";
            //comment.content = "Tg aude tu pues la merde";
            //comment.date = "17h28";
            //test.comments.Add(comment);
            Grid mainGrid = new Grid();

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            int i = 0;

            while (i < 10)
            {
                mainGrid.RowDefinitions.Add(new RowDefinition());
                Grid grid = new Grid();
                // grid.Background = new SolidColorBrush(Color.FromArgb(250, 100, 100, 100));

                // row
                var row = new RowDefinition();
                row.Height = new GridLength(30);
                grid.RowDefinitions.Add(row);

                var row2 = new RowDefinition();
                row2.Height = new GridLength(30);
                grid.RowDefinitions.Add(row2);

                var row3 = new RowDefinition();
                grid.RowDefinitions.Add(row3);

                var row4 = new RowDefinition();
                grid.RowDefinitions.Add(row4);

                //column
                var colum = new ColumnDefinition();
                colum.Width = new GridLength(50);
                grid.ColumnDefinitions.Add(colum);

                var column2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column2);

                var column3 = new ColumnDefinition();
                column3.Width = new GridLength(60);
                grid.ColumnDefinitions.Add(column3);

                // title
                TextBlock title = new TextBlock();
                title.Text = "Aude Sikorav";
                title.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                title.Margin = new Thickness(10, 5, 10, 5);
                title.FontSize = 14;
                Grid.SetColumn(title, 1);
                Grid.SetColumnSpan(title, 2);
                Grid.SetRow(title, 0);
                grid.Children.Add(title);

                // hour
                TextBlock date = new TextBlock();
                date.Text = "7h25";
                date.Margin = new Thickness(10, 0, 10, 0);
                date.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                Grid.SetColumn(date, 1);
                Grid.SetColumnSpan(date, 2);
                Grid.SetRow(date, 1);
                grid.Children.Add(date);

                // image
                Button btn = new Button();
                btn.Background = new SolidColorBrush(Color.FromArgb(250, 100, 100, 100));
                btn.Margin = new Thickness(10, 10, 0, 10);
                Grid.SetColumn(btn, 0);
                Grid.SetRow(btn, 0);
                Grid.SetRowSpan(btn, 2);
                grid.Children.Add(btn);

                // content
                TextBlock content = new TextBlock();
                content.Margin = new Thickness(10, 10, 10, 10);
                content.FontSize = 12;
                content.TextWrapping = TextWrapping.Wrap;
                content.Width = 340;
                content.Foreground = new SolidColorBrush(Color.FromArgb(250, 0, 0, 0));
                content.Text = "Bonjour a tous, voici une publication test pour voir combien en longeur elle va occupée et voir si Windows phone qui pue la merde gere tout seul la mise en place";
                Grid.SetColumn(content, 0);
                Grid.SetColumnSpan(content, 3);
                Grid.SetRow(content, 2);
                grid.Children.Add(content);

                // add comment
                Button me = new Button();
                me.Background = new SolidColorBrush(Colors.Blue);
                me.Margin = new Thickness(10, 0, 10, 0);
                Grid.SetColumn(me, 0);
                Grid.SetRow(me, 3);
                grid.Children.Add(me);

                TextBox newComment = new TextBox();
                newComment.BorderBrush = new SolidColorBrush(Colors.Black);
               // newComment.BorderThickness = new Thickness(1, 1, 1, 1);
                newComment.Text = "Votre commentaire ...";
                newComment.Foreground = new SolidColorBrush(Colors.Black);
                newComment.FontSize = 12;
                newComment.Margin = new Thickness(0, 10, 0, 0);
                Grid.SetColumn(newComment, 1);
                Grid.SetColumnSpan(newComment, 1);
                Grid.SetRow(newComment, 3);
                grid.Children.Add(newComment);

                Button ok = new Button();
                ok.Content = "Ok";
                ok.Foreground = new SolidColorBrush(Colors.Black);
                ok.Background = new SolidColorBrush(Colors.Gray);
                ok.Margin = new Thickness(10, 0, 10, 0);
                Grid.SetColumn(ok, 2);
                Grid.SetRow(ok, 3);

                grid.Background = new SolidColorBrush(Color.FromArgb(255, 182, 215, 168));
                grid.Children.Add(ok);


                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Color.FromArgb(250, 100, 100, 100));
                border.BorderThickness = new Thickness(1, 1, 1, 1);
                Grid.SetColumn(border, 0);
                Grid.SetColumnSpan(border, 3);
                Grid.SetRow(border, 0);
                Grid.SetRowSpan(border, 4);
                grid.Margin = new Thickness(10, 10, 10, 10);
                grid.Children.Add(border);

                mainGrid.Children.Add(grid);
                Grid.SetColumn(grid, 0);
                Grid.SetRow(grid, i);
                i++;
            }

            scroll.Content = mainGrid;
        }

        public Accueil()
        {
            this.InitializeComponent();
            initAccueil();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void search_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Research));
        }
    }
}