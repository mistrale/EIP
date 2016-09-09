using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class Comment
    {
        public string autor { get; set; }
        public string date { get; set; }
        public string content { get; set; }
    }

    class NewsInfos
    {
        public string sender_type { get; set; }
        public int sender_id { get; set; }
        public string sender_name { get; set; }
    }
}
