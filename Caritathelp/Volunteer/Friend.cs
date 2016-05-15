using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class friendsStruct
    {
        public string id { get; set; }
        public IList<FriendShip> friends { get; set; }
    }

    class FriendShip
    {
        public int id { get; set; }
        public string mail { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
}
