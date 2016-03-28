﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class NotificationFriendShip
    {
        public int notif_id { get; set; }
        public int id { get; set; }
        public string mail { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    class NotificationAssocInvite
    {

    }

    class NotificationEventInvite
    {

    }

    class GlobalNotification
    {
        public IList<NotificationFriendShip> add_friend { get; set; }
        public IList<NotificationAssocInvite> assoc_invite { get; set; }
        public IList<NotificationEventInvite> event_invite { get; set; }
    }
}
