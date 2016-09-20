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
                {"Gerer les demandes d'invitation", ButtonManagement.MANAGE_INVITATION }
            };
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
    }
}
