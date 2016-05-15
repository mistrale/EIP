using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class EventModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string place { get; set; }
        public string begin { get; set; }
        public string end { get; set; }
        public int assoc_id { get; set; }
        public string rights { get; set; }
        public string nb_friends_members { get; set; }
    }
}
