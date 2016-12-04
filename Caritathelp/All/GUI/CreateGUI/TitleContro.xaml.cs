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

namespace Caritathelp.All.GUI.CreateGUI
{
    public sealed partial class TitleContro :  UserControl
    {
        public enum Type
        {
            STRING,
            NUMBER
        }

        private Type type;
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
            if (this.type == Type.STRING)
            {
                field = Content.Text;
                return true;
            }
            foreach (char c in Content.Text)
            {
                if (!(c >= '0' && c <= '9'))
                {
                    field = "";
                    err.printMessage("Champs '" + type + "' invalide, vérifier que le nombre soit correct.", ErrorControl.Code.FAILURE);
                    return false;

                }
            }
            field = Content.Text;
            return true;
        }

        public TitleContro(string title, string content, GUI.ErrorControl err, Type type)
        {
            this.InitializeComponent();
            titleBloc.Text = title;
            this.type = type;

            if (content != null)
            {
                Content.Text = content;
            } else
            {
                Content.Text = title;
            }
            this.err = err;
        }
    }
}
