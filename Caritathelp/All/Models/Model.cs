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
        NUMBER,
        DATE,
        HOUR,
        COMBOX,
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
        MANAGE_ALBUM = 8,
        UPDATE_PASSWORD = 9,
        SEE_EMERGENCY = 10,
        MY_PROFIL = 11,
    }

    public class PictureModel
    {
        public bool isAdmin { get; set; }
        public Model model { get; set; }
    }

    public class FormModel
    {
        public int id { get; set; }
        public Model modelType { get; set; }
        public Model createdModelType { get; set; }
        public bool isCreation { get; set; }
        public bool isAdmin { get; set; }
    }

    public class InfosListModel
    {
        public string typeModel { get; set; }
        public int id { get; set; }
        public string listTypeModel { get; set; }
    }

    public class Model
    {
        public static Model createModel(string type, int id)
        {
            if (type.Equals("volunteer", StringComparison.Ordinal))
            {
                return new Volunteer(id);
            }
            if (type.Equals("assoc", StringComparison.Ordinal))
            {
                return new Association(id);
            }
            if (type.Equals("event", StringComparison.Ordinal))
            {
                return new Event(id);
            }
            if (type.Equals("shelter", StringComparison.Ordinal))
            {
                return new Shelter(id);
            }
            return null;
        }

        public static Dictionary<string, Dictionary<string, string>> resourcesTables = new Dictionary<string, Dictionary<string, string>>
            {
                {"assoc", new Dictionary<string, string>
                    {
                        { "volunteer", "/members"},
                        { "shelter", "/shelters"},
                         { "event", "/events"},

                    }
                },
                {"volunteer", new Dictionary<string, string>
                    {
                        { "volunteer", "/friends"},
                        { "assoc", "/associations"},
                        { "event", "/events"},
                    }
                },
                {"event", new Dictionary<string, string>
                    {
                        { "volunteer", "/guests"},
                    }
                }
           };

        public static Dictionary<string, Dictionary<string, string>> Values = new Dictionary<string, Dictionary<string, string>>
        {
            {"assoc", new Dictionary<string, string>
                {
                    { "Name", "Association"},
                    { "NameType", "name"},
                    { "URL", "/associations/"},
                    { "DeleteURL", "/associations"},
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
                    { "CreationType", "Création d'association"},
                    { "Titre", "name" },
                    { "Description", "description" },
                    { "Date de création", "birthday" },
                    { "Ville", "city" },
                    { "Logo", "thumb_path" }
            }
         },
         {"volunteer", new Dictionary<string, string>
                {
                    { "Name", "Volontaire"},
                    { "NameType", "fullname"},
                    { "URL", "/volunteers/"},
                     { "DeleteURL", "/auth/sign_out"},

                    { "Model", "volunteer"},
                    { "TypeID", "volunteer_id"},
                    { "CancelTypeID", "none"},
                    { "ResourceManagement", "friendship"},

                    { "AddURL", "/friendship/add"},
                    { "RemoveURL", "/friendship/remove"},
                    { "AcceptURL", "/friendship/reply"},
                    { "AcceptInvitation", "/friendship/reply"},
                    { "InviteURL", "/friendship/add"},
                    { "CancelInviteURL", "/friendship/cancel_request" },
                    { "UpgradeURL", "" },
                    { "KickURL", "/friendship/remove" },

                    { "NbRelationType", "nb_common_friends"},
                    { "ResourceURL", "/friends"},
                    { "RightsType", "friendship" },
                    { "WaitingInvitation", "/friendship/received_invitations" },
                    { "SendInvitation", "/friend_requests/" },

                    // creation / update assoc
                    { "CreationType", "Creation d'association"},
                    { "Prénom", "firstname" },
                    { "Nom", "lastname" },
                    { "Date de naissance", "birthday" },
                    { "Ville", "city" },
                    { "Sexe", "gender" },
                    { "Email", "email" },
                    { "Recevoir les notifications", "allow_notifications" },
                    { "Activer la géolocalisation", "allowgps" }
            }
         },
            { "event", new Dictionary<string, string>
                {
                    { "Name", "Evenement"},
                    { "NameType", "title"},
                    { "URL", "/events/"},
                    { "DeleteURL", "/events"},

                    { "Model", "event"},
                    { "TypeID", "event_id"},
                    { "CancelTypeID", "volunteer_id"},
                    { "ResourceManagement", "guests"},

                    { "AddURL", "/guests/join"},
                    { "RemoveURL", "/guests/leave"},
                    { "AcceptURL", "/guests/reply_invite"},
                    { "AcceptInvitation", "/guests/reply_guest"},
                    { "InviteURL", "/guests/invite"},
                    { "CancelInviteURL", "/guests/uninvite" },
                    { "UpgradeURL", "/guests/upgrade" },
                    { "KickURL", "/guests/remove" },

                    { "NbRelationType", "nb_friends_members"},
                    { "ResourceURL", "/events"},
                    { "RightsType", "rights" },
                    { "WaitingInvitation", "/guests/waiting" },
                    { "SendInvitation", "/guests/invited" },

                    // creation / update assoc
                    { "CreationType", "Création d'évènement"},
                    { "Titre", "title" },
                    { "Description", "description" },
                    { "début", "begin" },
                    { "fin", "end" },
                    { "Lieu", "place" }
                }

             },
            { "shelter", new Dictionary<string, string>
                {
                    { "Name", "Centre"},
                    { "NameType", "name"},
                    { "URL", "/shelters/"},
                    { "DeleteURL", "/shelters"},

                    { "Model", "shelter"},
                    { "TypeID", "shelter_id"},

                    { "CreationType", "Création de centre"},
                    { "Nom", "name" },
                    { "Adresse", "address" },
                    { "Code postal", "zipcode" },
                    { "Ville", "city" },
                    { "Nombre de places total", "total_places" },
                    { "Nombre de places libres", "free_places" },
                    { "Téléphone", "phone" },
                    { "Description", "description" },
                }

             }
        }; 

        protected Dictionary<string, Dictionary<string, ButtonManagement>> mngButton;
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

        virtual public Dictionary<string, Dictionary<string, ButtonManagement> > getButtonsManagement()
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
