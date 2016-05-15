using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.Event
{
    class NotifEvent
    {
        public int notif_id { get; set; }
        public int id { get; set; }
        public string mail { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    class NotificationEventRequest
    {
        public IList<NotifEvent> guest_request { get; set; }
    }
}
