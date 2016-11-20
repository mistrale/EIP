using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.All.Models
{
    class Volunteer : Model
    {
        public Volunteer(int id) : base(id)
        {
            mngButton = new Dictionary<string, Dictionary<string, ButtonManagement>>
            {
                { "Créer une association", new Dictionary<string, ButtonManagement> { { "assoc", ButtonManagement.CREATE_RESOURCE } } },

                { "Mes associations", new Dictionary<string, ButtonManagement> { { "assoc", ButtonManagement.MANAGE_RELATION } } },
                { "Mes évènements", new Dictionary<string, ButtonManagement> { { "event", ButtonManagement.MANAGE_RELATION } } },
                { "Mes amis", new Dictionary<string, ButtonManagement> { { "volunteer", ButtonManagement.MANAGE_RELATION } } },

  //              { "Invitations associations", new Dictionary<string, ButtonManagement> { { "assoc", ButtonManagement.MANAGE_INVITATION } } },
//                { "Invitations évènement", new Dictionary<string, ButtonManagement> { { "event", ButtonManagement.MANAGE_INVITATION } } },
                { "Demandes de contact", new Dictionary<string, ButtonManagement> { { "volunteer", ButtonManagement.MANAGE_INVITATION } } },


         //       { "Modifier mon mot de passe", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.DELETE_RESOURCE } } },
                { "Notification", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.GET_NOTIFICATION } } },
                { "Se déconnecter", new Dictionary<string, ButtonManagement> { { "", ButtonManagement.DELETE_RESOURCE } } }
            };

            typeControls = new Dictionary<string, FormControlType>
            {
                { "Prénom", FormControlType.FIELD },
                { "Nom", FormControlType.FIELD },
                { "Ville", FormControlType.FIELD },

                { "Sexe", FormControlType.COMBOX },
                { "Date de naissance", FormControlType.DATE },

                { "Email", FormControlType.FIELD },

                { "Recevoir les notifications", FormControlType.CHECKFIELD },
                { "Activer la géolocalisation", FormControlType.CHECKFIELD },
            };
        }

        public override string getType()
        {
            return "volunteer";
        }

        public override bool isInRelation(string rights)
        {
            if (rights != null && ((rights.Equals("friend")) || (rights.Equals("yourself"))))
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
            if (rights != null && rights.Equals("invitation sent"))
                return true;
            return false;
        }

        public override bool hasNotif(string rights)
        {

            if (rights != null && rights.Equals("invitation received"))
                return true;
            return false;
        }

        public override bool isAdmin(string rights)
        {

            if (rights != null && (rights.Equals("yourself")))
                return true;
            return false;
        }

        public override bool isInvited(string rights)
        {
            if (rights != null && rights.Equals("invitation received"))
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
