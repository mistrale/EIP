﻿using System;
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

namespace Caritathelp.All.GUI.CreateGUI
{
    public sealed partial class DescriptionControls : UserControl
    {
        GUI.ErrorControl err;

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 114, 136, 142));
        }

        public bool checkField(string type, out string field)
        {
            if (Content.Text.Equals(type, StringComparison.Ordinal)
                || Content.Text.Equals("", StringComparison.Ordinal))
            {
                field = "";
                err.printMessage("Champs '" + type + "' vide.", ErrorControl.Code.FAILURE);
                return false;
            }
            field = Content.Text;
            return true;
        }

        public DescriptionControls(string title, string content, GUI.ErrorControl err)
        {
            this.InitializeComponent();
            titleBloc.Text = title;
            if (content == null)
            {
                Content.Text = title;
            }
            else
            {
                Content.Text = content;
            }
            this.err = err;
        }

        private void Content_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}