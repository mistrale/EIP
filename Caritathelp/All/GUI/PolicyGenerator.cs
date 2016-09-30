using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Caritathelp.All.GUI
{
    class PolicyGenerator
    {
        public enum ColorType
        {
            PRIMARY_COLOR,
            PRIMARY_TEXT,
            LIGHT_BACKGROUND,
            DARK_BACKGROUND,
            DEFAULT_TEXT,
            LIGHT_TEXT,
            DARK_TEXT,
            SUCCESS,
            FAILURE,
            WHITE,
        }

        static public SolidColorBrush getBrush(ColorType type)
        {
            SolidColorBrush color = null;
            switch (type)
            {
                case ColorType.PRIMARY_COLOR:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 83, 166, 52));
                    break;
                case ColorType.PRIMARY_TEXT:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 255, 255, 255));
                    break;
                case ColorType.DARK_BACKGROUND:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 67, 100, 53));
                    break;
                case ColorType.LIGHT_BACKGROUND:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 228, 247, 221));
                    break;
                case ColorType.DEFAULT_TEXT:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 114, 146, 142));
                    break;
                case ColorType.LIGHT_TEXT:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 192, 201, 204));
                    break;
                case ColorType.DARK_TEXT:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 63, 84, 97));
                    break;
                case ColorType.SUCCESS:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 74, 215, 167));
                    break;
                case ColorType.FAILURE:
                    color = new SolidColorBrush(Color.FromArgb(0xFF, 215, 110, 110));
                    break;
            }
            return color;
        }

    }
}
