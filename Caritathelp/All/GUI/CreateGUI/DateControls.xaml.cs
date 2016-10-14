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
    public sealed partial class DateControls : UserControl
    {
        GUI.ErrorControl err;

        public bool checkField(string type, out string field)
        {
            String[] data = dateDate.Date.ToString().Split(' ')[0].Split('/');
            string date = data[2] + '-' + data[0] + '-' + data[1] + 'T';
            field = date;
            return true;
        }

        public DateControls(string date, string type, GUI.ErrorControl err)
        {
            this.InitializeComponent();
            typeDateBox.Text = type;
            try
            {
                string[] begin = date.Split('-');
                dateDate.Date = (DateTime)Convert.ToDateTime(begin[1] + '/' + begin[2] + '/' + begin[0]);
            }
            catch (System.IndexOutOfRangeException e)
            {

            }
            this.err = err;
        }
    }
}
