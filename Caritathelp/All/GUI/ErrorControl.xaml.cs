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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI
{
    public sealed partial class ErrorControl : UserControl
    {
        public enum Code
        {
            SUCCESS,
            FAILURE
        }

        public void closeWindow(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void printMessage(string message, Code code)
        {
            this.Visibility = Visibility.Visible;
            errorBlock.Text = message;
            if (code == Code.SUCCESS)
            {
                textBlock1.Text = "Succes";
                border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 83, 166, 52));
            }
        }

        public ErrorControl()
        {
            this.InitializeComponent();
        }
    }
}
