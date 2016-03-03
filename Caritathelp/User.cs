using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{

    public class AddFriend
    {
        public int id_notif { get; set; }
        public int id_sender { get; set; }
        public string mail { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    public class Notifications
    {
        public IList<AddFriend> add_friend;
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
        public string allowgps { get; set; }
        public string allow_notifications { get; set; }
         public Notifications notifications { get; set; }
    }
}
