using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.All.Models
{
    public class Event : Model
    {
        public Event(int id) : base(id)
        {
            mngButton = new Dictionary<string, Dictionary<string, ButtonManagement>>
            {
                { "Gérer les membres", new Dictionary<string, ButtonManagement> { { "volunteer", ButtonManagement.MANAGE_RELATION } } },
                { "Gérer les invitations", new Dictionary<string, ButtonManagement> { { "volunteer", ButtonManagement.MANAGE_INVITATION } } },
                { "Gérer les photos", new Dictionary<string, ButtonManagement> { { "event", ButtonManagement.MANAGE_ALBUM } } },
                { "Notifications", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.GET_NOTIFICATION } } },
                { "Voir les réponses à une urgence", new Dictionary<string, ButtonManagement> { { "event", ButtonManagement.SEE_EMERGENCY } } },
                { "Supprimer l'évènement", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.DELETE_RESOURCE } } }
            };


            typeControls = new Dictionary<string, FormControlType>
            {
                { "Titre", FormControlType.FIELD },
                { "Description", FormControlType.DESCRIPTION},
                { "Lieu", FormControlType.FIELD },
                { "début", FormControlType.HOUR},
                { "fin", FormControlType.HOUR}
            };
        }

        public override string getType()
        {
            return "event";
        }

        public override bool isInRelation(string rights)
        {
            if (rights != null && (rights.Equals("admin") || rights.Equals("host")
                || rights.Equals("member")))
                return true;
            return false;
        }

        public override bool isUnknown(string rights)
        {
            if (rights == null || (rights.Equals("none")))
                return true;
            return false;
        }

        public override bool isWaiting(string rights)
        {
            if (rights != null && rights.Equals("waiting"))
                return true;
            return false;
        }

        public override bool hasNotif(string rights)
        {

            if (rights != null && rights.Equals("invited"))
                return true;
            return false;
        }

        public override bool isAdmin(string rights)
        {

            if (rights != null && (rights.Equals("admin") || rights.Equals("host")))
                return true;
            return false;
        }

        public override bool isInvited(string rights)
        {
            if (rights != null && rights.Equals("invited"))
                return true;
            return false;
        }

        public override string[] getNotificationType()
        {
            string[] array = { "JoinEvent", "NewGuest" };
            return array;
        }

        public override string[] getPrivateField()
        {
            string[] array = { "Logo" };
            return array;
        }
    }

}
