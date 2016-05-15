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

    class Publication
    {
        public string title { get; set; }
        public string content { get; set; }
        public string date { get; set; }
        public string author { get; set; }
        public IList<Comment> comments { get; set; }
    }
}
