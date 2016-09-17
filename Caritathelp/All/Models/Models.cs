using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.All.Models
{
    class InfosModel
    {
        public string type { get; set; }
        public int id { get; set; }
    }

    class Model
    {
        private int _id { get; set; }
        private string _thumb_path { get; set; }

        virtual public  string getModel()
        {
            return "Model";
        }

        virtual public int getID()
        {
            return _id;
        }

        virtual public string getName()
        {
            return "";
        }

        virtual public string getRights()
        {
            return "";
        }
        
        virtual public string getThumbPath()
        {
            return _thumb_path;
        }        
    }

    class Association :  Model
    {
        public string _rights { get; set; }

        public override string getRights()
        {
            return _rights;
        }

        public override string getModel()
        {
            return "Association";
        }
    }
}
