using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    public class Notification
    {
        public IList<string> add_friends;
    }
    class User
    {
        public int id { get; set; }
        public string mail { get; set; }
        public string token { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string birthday { get; set; }
        public string genre { get; set; }
        public string city { get; set; }
        public string latitue { get; set; }
        public string longitude { get; set; }
        public bool allowgps { get; set; }
        public Notification notifications { get; set; }
    }
}
