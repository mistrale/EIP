using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{    
    class GlobalNotification
    {
        public string id { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string receiver_id { get; set; }
        public string receiver_name { get; set; }
        public string assoc_id { get; set; }
        public string assoc_name { get; set; }
        public string event_id { get; set; }
        public string event_name { get; set; }
        public string notif_type { get; set; }
        public string created_at { get; set; }
    }
}
