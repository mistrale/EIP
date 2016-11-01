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
            mngButton = new Dictionary<string, ButtonManagement>
            {
                {"Creer un evenement", ButtonManagement.CREATE_RESOURCE },
                {"Supprimer l'association", ButtonManagement.DELETE_RESOURCE },
                {"Gerer les membres", ButtonManagement.MANAGE_RELATION },
                {"Gerer les invitations", ButtonManagement.MANAGE_INVITATION },
                {"Notifications", ButtonManagement.GET_NOTIFICATION }
            };

            typeControls = new Dictionary<string, FormControlType>
            {
                { "Titre", FormControlType.FIELD },
                { "Description", FormControlType.DESCRIPTION},
                { "Ville", FormControlType.FIELD },
                { "Date de creation", FormControlType.DATE},
                { "Logo", FormControlType.FILE},
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
