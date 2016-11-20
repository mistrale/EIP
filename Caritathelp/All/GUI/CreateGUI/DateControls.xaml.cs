using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public DateControls(string type, string date, GUI.ErrorControl err)
        {
            this.InitializeComponent();
            typeDateBox.Text = type;
            if (date !=  null)
            {
                try
                {
                    string[] begin = date.Split('-');
                    dateDate.Date = (DateTime)Convert.ToDateTime(begin[1] + '/' + begin[2] + '/' + begin[0]);
                }
                catch (System.IndexOutOfRangeException e)
                {
                    Debug.WriteLine("Fail dans DateControl mais pas important : " + e.Message);
                }
            }
            this.err = err;
        }
    }
}
