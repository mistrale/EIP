using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class Global
    {
        public const string API_IRL = "http://api.caritathelp.me:3000";

    }

    class Publication
    {
        public int id { get; set; }
        public string volunteer_id { get; set; }
        public string news_type { get; set; }


        public string title { get; set; }
        public string content { get; set; }
        public int groupe_id { get; set; }
        public string groupe_type { get; set; }
        public string group_name { get; set; }
        public string groupe_thumb_path { get; set; }


        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int number_comments { get; set; }
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
}
