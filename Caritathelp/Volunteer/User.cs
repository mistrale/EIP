using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class UserStruct
    {
        public string id { get; set; }
        public User user { get; set; }
    }

    class User
    {
        public string id { get; set; }
        public string mail { get; set; }
        public string token { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string birthday { get; set; }
        public string gender { get; set; }
        public string city { get; set; }
        public string latitue { get; set; }
        public string longitude { get; set; }
        public bool allowgps { get; set; }
        public string allow_notifications { get; set; }
        public string friendship { get; set; }
        public string notif_id { get; set; }
        public string thumb_path { get; set; }
    }
}
