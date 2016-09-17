using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp
{
    class Global
    {
        public const string API_IRL = "http://staging.caritathelp.me";
        public const string WS_URL = "ws://ws.staging.caritathelp.me";

    }

    class Publication
    {
        public int id { get; set; }
        public int volunteer_id { get; set; }
        public string news_type { get; set; }


        public string title { get; set; }
        public string content { get; set; }
        public int group_id { get; set; }
        public string group_type { get; set; }
        public string group_name { get; set; }
        public string group_thumb_path { get; set; }


        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int number_comments { get; set; }
        public bool as_group { get; set; }
        public string volunteer_name { get; set; }
        public string volunteer_thumb_path { get; set; }
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

    class commentInfos
    {
        public int id_news { get; set; }
        public int id_comments { get; set; }
        public int row_cmt { get; set; }
        public int row_news { get; set; }
    }

    class NewsInfos
    {
        public int id_news { get; set; }
        public int id_rows { get; set; }
    }

}
