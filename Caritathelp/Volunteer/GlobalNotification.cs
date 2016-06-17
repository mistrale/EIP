using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    //class NotificationFriendShip
    //{
    //    public int notif_id { get; set; }
    //    public int id { get; set; }
    //    public string mail { get; set; }
    //    public string firstname { get; set; }
    //    public string lastname { get; set; }
    //}

    //class NotificationAssocInvite
    //{
    //    public int notif_id { get; set; }
    //    public int id { get; set; }
    //    public string name { get; set; }
    //    public string city { get; set; }
    //}

    //class NotificationEventInvite
    //{
    //    public int notif_id { get; set; }
    //    public int id { get; set; }
    //    public string title { get; set; }
    //    public string place { get; set; }
    //}

    //class GlobalNotification
    //{
    //    public IList<NotificationFriendShip> add_friend { get; set; }
    //    public IList<NotificationAssocInvite> assoc_invite { get; set; }
    //    public IList<NotificationEventInvite> event_invite { get; set; }
    //}
    
    class GlobalNotification
    {
        public string id { get; set; }
        public string sender_id { get; set; }
        public string receiver_id { get; set; }
        public string assoc_id { get; set; }
        public string event_id { get; set; }
        public string notif_type { get; set; }
        public string created_at { get; set; }
    }
}
