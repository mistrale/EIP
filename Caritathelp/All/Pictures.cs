using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.All
{
    class Thumb
    {
        public string url { get; set; }
    }

    class PicturePath
    {
        public string url { get; set; }
        public Thumb thumb { get; set; }
    }

    class Picture
    {
        public int id { get; set; }
        public int file_size { get; set; }
        public PicturePath picture_path { get; set; }
        public string event_id { get; set; }
        public string volunteer_id { get; set; }
        public string assoc_id { get; set; }
        public bool is_main { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
