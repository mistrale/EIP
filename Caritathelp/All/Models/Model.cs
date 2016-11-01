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
        DESCRIPTION,
        PASSWORD,
        CHECKFIELD
    }

    public enum ButtonManagement
    {
        CREATE_RESOURCE = 1,
        UPDATE_RESOURCE = 2,
        MANAGE_INVITATION = 3,
        DELETE_RESOURCE = 4,
        MANAGE_RELATION = 5,
        GET_RESOURCES = 6,
        GET_NOTIFICATION = 7,
    }

    public class FormModel
    {
        public int id { get; set; }
        public string modelType { get; set; }
        public string createdModelType { get; set; }
        public bool isCreation { get; set; }
        public bool isAdmin { get; set; }
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
                    { "Name", "Association"},
                    { "NameType", "name"},
                    { "URL", "/associations/"},
                    { "Model", "assoc"},
                    { "TypeID", "assoc_id"},
                    { "CancelTypeID", "volunteer_id"},
                    { "ResourceManagement", "members"},

                    { "AddURL", "/membership/join"},
                    { "RemoveURL", "/membership/leave"},
                    { "AcceptURL", "/membership/reply_invite"},
                    { "AcceptInvitation", "/membership/reply_member"},
                    { "InviteURL", "/membership/invite"},
                    { "CancelInviteURL", "/membership/uninvite" },
                    { "UpgradeURL", "/membership/upgrade" },
                    { "KickURL", "/membership/kick" },

                    { "NbRelationType", "nb_friends_members"},
                    { "ResourceURL", "/associations"},
                    { "RightsType", "rights" },
                    { "WaitingInvitation", "/membership/waiting" },
                    { "SendInvitation", "/membership/invited" },

                    // creation / update assoc
                    { "CreationType", "Creation d'association"},
                    { "Titre", "name" },
                    { "Description", "description" },
                    { "Date de creation", "birthday" },
                    { "Ville", "city" },
                    { "Logo", "thumb_path" }
            }
         },
         {"volunteer", new Dictionary<string, string>
                {
                    { "Name", "Volontaire"},
                    { "NameType", "fullname"},
                    { "URL", "/volunteers/"},
                    { "Model", "volunteer"},
                    { "TypeID", "volunteer_id"},
                    { "CancelTypeID", "volunteer_id"},
                    { "ResourceManagement", "friendship"},

                    { "AddURL", "/friendship/add"},
                    { "RemoveURL", "/friendship/remove"},
                    { "AcceptURL", "/friendship/reply"},
                    { "AcceptInvitation", "/friendship/reply"},
                    { "InviteURL", "/friendship/add"},
                    { "CancelInviteURL", "/friendship/cancel" },
                    { "UpgradeURL", "" },
                    { "KickURL", "/friendship/remove" },

                    { "NbRelationType", "nb_friends"},
                    { "ResourceURL", "/friends"},
                    { "RightsType", "friendship" },
                    { "WaitingInvitation", "/friendship/received_invitations" },
                    { "SendInvitation", "/friend_requests/" },

                    // creation / update assoc
                    { "CreationType", "Creation d'association"},
                    { "Titre", "name" },
                    { "Description", "description" },
                    { "Date de creation", "birthday" },
                    { "Ville", "city" },
                    { "Logo", "thumb_path" }
            }
         },
            { "event", new Dictionary<string, string>
                {
                    { "Name", "Evenement"},
                    { "NameType", "title"},
                    { "URL", "/events/"},
                    { "Model", "event"},
                    { "TypeID", "event_id"},
                    { "AddURL", "/guests/join"},
                    { "RemoveURL", "/guests/leave"},
                    { "AcceptURL", "/guests/reply_invite"},
                    { "NbRelationType", "nb_friends_members"},
                    { "ResourceURL", "/events"},
                    { "RightsType", "rights" },


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

        virtual public string getType()
        {
            return "";
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

        virtual public string[] getPrivateField()
        {
            string[] array = { };
            return array;
        }
    }
}
