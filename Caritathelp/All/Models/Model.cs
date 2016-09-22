using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Caritathelp.All.Models
{
    public enum FormControlType
    {
        FIELD,
        DATE,
        HOUR,
        FILE,
        DESCRIPTION
    }

    public enum ButtonManagement
    {
        CREATE_RESOURCE = 1,
        UPDATE_RESOURCE = 2,
        MANAGE_INVITATION = 3,
        DELETE_RESOURCE = 4,
        MANAGE_RELATION = 5,
        GET_RESOURCES = 6
    }

    public class InfosListModel
    {
        public string typeModel { get; set; }
        public int id { get; set; }
        public string listTypeModel { get; set; }
    }

    public class InfosModel
    {
        public string type { get; set; }
        public int id { get; set; }
    }

    public class Model
    {
        public static Dictionary<string, Dictionary<string, string>> Values = new Dictionary<string, Dictionary<string, string>>
        {
            {"assoc", new Dictionary<string, string>
                {
                    { "NameType", "name"},
                    { "URL", "/associations/"},
                    { "Model", "assoc"},
                    { "TypeID", "assoc_id"},
                    { "AddURL", "/membership/join"},
                    { "RemoveURL", "/membership/leave"},
                    { "AcceptURL", "/membership/reply_invite"},
                    { "NbRelationType", "nb_friends_members"},
                    { "ResourceURL", "/associations"},
                    { "RightsType", "rights" },

                    // creation / update assoc
                    { "CreationType", "Creation d'association"},
                    { "Titre", "name" },
                    { "Description", "description" },
                    { "Date de creation", "birthday" },
                    { "Ville", "city" },
            }
            },
            { "event", new Dictionary<string, string>
                {
                    { "", "" }
                }

             }
        }; 

        protected Dictionary<string, ButtonManagement> mngButton;
        protected Dictionary<string, FormControlType> typeControls;

        public int id { get; set; }

        public Model()
        {
        }

        public Model(int id)
        {
            this.id = id;
        }

        virtual public Dictionary<string, FormControlType> getFormControlType()
        {
            return typeControls;
        }

        virtual public Dictionary<string, ButtonManagement> getButtonsManagement()
        {
            return mngButton;
        }

        virtual public int getID()
        {
            return id;
        }

        virtual public bool isInRelation(string rights)
        {
            return false;
        }

        virtual public bool isUnknown(string rights)
        {
            return false;
        }

        virtual public bool isWaiting(string rights)
        {
            return false;
        }

        virtual public bool isInvited(string rights)
        {
            return false;
        }

        virtual public bool hasNotif(string rights)
        {
            return false;
        }

        virtual public bool isAdmin(string rights)
        {
            return false;
        }

        virtual public string[] getNotificationType()
        {
            string[] array = {  };
            return array;
        }
    }
}
