﻿using System;
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
    public sealed partial class CheckField : UserControl
    {
        GUI.ErrorControl err;

        public bool checkField(string type, out string field)
        {
            if (checkBox.IsChecked == true)
            {
                field = "true";
            } else
            {
                field = "false";
            }
            return true;
        }

        public CheckField(string type, string isTrue, GUI.ErrorControl err)
        {
            this.InitializeComponent();
            checkBox.Content = type;
            if (isTrue == null)
            {
                
            }
            else if (isTrue.Equals("True", StringComparison.Ordinal))
            {
                checkBox.IsChecked = true;
            }
            this.err = err;
        }
    }
}