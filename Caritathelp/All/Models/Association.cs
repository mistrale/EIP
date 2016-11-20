using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Caritathelp.All.Models
{
    public class Association : Model
    {
        public Association(int id) : base(id)
        {
            mngButton = new Dictionary<string, Dictionary<string, ButtonManagement>>
            {
                { "Créer un évènement", new Dictionary<string, ButtonManagement> { { "event", ButtonManagement.CREATE_RESOURCE } } },
                { "Gérer les membres", new Dictionary<string, ButtonManagement> { { "volunteer", ButtonManagement.MANAGE_RELATION } } },
 //               { "Gérer les centres", new Dictionary<string, ButtonManagement> { { "assoc", ButtonManagement.MANAGE_RELATION } } },

                { "Gérer les invitations", new Dictionary<string, ButtonManagement> { { "volunteer", ButtonManagement.MANAGE_INVITATION } } },


         //       { "Modifier mon mot de passe", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.DELETE_RESOURCE } } },
                { "Notifications", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.GET_NOTIFICATION } } },
                { "Supprimer l'association", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.DELETE_RESOURCE } } }
            };

            typeControls = new Dictionary<string, FormControlType>
            {
                { "Titre", FormControlType.FIELD },
                { "Description", FormControlType.DESCRIPTION},
                { "Ville", FormControlType.FIELD },
                { "Date de création", FormControlType.DATE}
            };
        }

        public override string getType()
        {
            return "assoc";
        }

        public override bool isInRelation(string rights)
        {
            if (rights != null && (rights.Equals("admin") || rights.Equals("owner")
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

            if (rights != null && (rights.Equals("admin") || rights.Equals("owner")))
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
            string[] array = { "JoinAssoc", "NewMember" };
            return array;
        }

        public override string[] getPrivateField()
        {
            string[] array = { "Logo" };
            return array;
        }
    }
}
