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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Caritathelp.All.GUI
{
    public sealed partial class ConfirmBox : UserControl
    {
        private RoutedEventHandler evt;

        public ConfirmBox()
        {
            this.InitializeComponent();
        }

        public void setRoutedEvent(RoutedEventHandler e)
        {
            yesButton.Click += e;
            evt = e;
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            yesButton.Click -= evt;
        }
    }
}
