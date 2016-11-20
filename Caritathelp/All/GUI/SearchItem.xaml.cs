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
    public sealed partial class SearchItem : UserControl
    {
        Page currentPage;

        public void setItem(string thumb_path, string name, string type, Page page, Models.Model model)
        {
            titleButton.Content = name;
            ImageBrush image = new ImageBrush();
            image.ImageSource = new BitmapImage(new Uri("http://staging.caritathelp.me" + thumb_path, UriKind.Absolute));
            logo.Fill = image;
            typeText.Text = type;
            currentPage = page;
            titleButton.Tag = model;
        }

        public SearchItem()
        {
            this.InitializeComponent();
        }

        private void ResearchButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            All.Models.Model infos = (All.Models.Model)(button.Tag);
            currentPage.Frame.Navigate(typeof(All.Models.GenericProfil), infos);
            // identify which button was clicked and perform necessary actions
        }
    }
}
