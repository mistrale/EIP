using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.Message
{
    public class MessageInfos
    {
        public int id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string name { get; set; }
        public int number_volunteers { get; set; }
        public int number_messages { get; set; }
        public bool is_private { get; set; }
        public IList<string> volunteers { get; set; }
    }

    public class MessageConversation
    {
        public int id { get; set; }
        public int chatroom_id { get; set; }
        public int volunteer_id { get; set; }
        public string content { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string fullname { get; set; }
        public string thumb_path { get; set; }
    }
}
