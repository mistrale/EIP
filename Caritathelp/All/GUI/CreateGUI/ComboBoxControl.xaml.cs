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

namespace Caritathelp.All.GUI.CreateGUI
{
    public sealed partial class ComboBoxControl : UserControl
    {
        GUI.ErrorControl err;

        public bool checkField(string type, out string field)
        {
            if (comboBox.SelectedIndex == 0)
            {
                field = "f";
            }
            else
            {
                field = "m";
            }
            return true;
        }

        public ComboBoxControl(string type, string sexe, GUI.ErrorControl err)
        {
            this.InitializeComponent();
            if (sexe == null || sexe.Equals("f", StringComparison.Ordinal))
            {
                comboBox.SelectedIndex = 0;
            } else
            {
                comboBox.SelectedIndex = 1;
            }
            this.err = err;
        }

        private void comboBox_DropDownOpened(object sender, object e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.IsDropDownOpen == true)
            {
                this.Height = 123;
            }
        }

        private void comboBox_DropDownClosed(object sender, object e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.IsDropDownOpen == false)
            {
                this.Height = 60;
            }
        }
    }
}
