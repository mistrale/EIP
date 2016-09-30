using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Caritathelp.All.GUI
{
    class ControlsGenerator
    {
        public static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
        }

        static public Image generateImage(string path)
        {
            Image btn = new Image();
            btn.Margin = new Thickness(10, 10, 10, 10);
                Debug.WriteLine("path :  " + Global.API_IRL +  path);
            btn.Source = new BitmapImage(new Uri("http://staging.caritathelp.me/" + path, UriKind.Absolute));
            return btn;
        }

        static public TextBlock generateNameField(string title)
        {
            TextBlock poster = new TextBlock();
            poster.Text = title;
            poster.Foreground = PolicyGenerator.getBrush(PolicyGenerator.ColorType.DARK_TEXT);
            poster.Margin = new Thickness(10, 5, 10, 5);
            poster.FontSize = 14;
            return poster;
        }

        static public TextBlock generateInfos(string title)
        {
            TextBlock poster = new TextBlock();
            poster.Text = title;
            poster.Foreground = PolicyGenerator.getBrush(PolicyGenerator.ColorType.DEFAULT_TEXT);
            poster.Margin = new Thickness(10, 5, 10, 5);
            poster.FontSize = 12;
            return poster;
        }

        static public TextBox generateInfosBox(string infos)
        {
            TextBox poster = new TextBox();
            if (infos == null)
                Debug.WriteLine("wtf");
            Debug.WriteLine("infos : " + infos);
            poster.Text = infos;
            poster.Foreground = PolicyGenerator.getBrush(PolicyGenerator.ColorType.LIGHT_TEXT);
            poster.Margin = new Thickness(10, 10, 10, 10);
            poster.FontSize = 12;
            poster.TextWrapping = TextWrapping.Wrap;
            poster.GotFocus += TextBox_GotFocus;
            poster.FontWeight = FontWeights.Bold;
            return poster;
        }

        static public Button generateNewsButton(string content, RoutedEventHandler e)
        {
            Button poster = new Button();
            poster.Click += e;
            poster.Content = content;
            poster.FontSize = 12;
            poster.Margin = new Thickness(10, 0, 10, 0);
            return poster;
        }
    }
}
