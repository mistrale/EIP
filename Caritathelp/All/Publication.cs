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
        public int id { get; set; }
        public string assoc_id { get; set; }
        public string event_id { get; set; }
        public string volunteer_id { get; set; }
        public string friend_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string type { get; set; }
    }

    class Comments
    {
        public int id { get; set; }
        public int new_id { get; set; }
        public int volunteer_id { get; set; }
        public string content { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string thumb_path { get; set; }
    }

    class Publications
    {
        public int id { get; set; }
        public string assoc_id { get; set; }
        public string event_id { get; set; }
        public string volunteer_id { get; set; }
        public string friend_id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string news_type { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string thumb_path { get; set; }
    }
}
