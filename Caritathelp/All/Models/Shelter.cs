using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caritathelp.All.Models
{
    class Shelter : Model
    {
        public Shelter(int id) : base(id)
        {
            mngButton = new Dictionary<string, Dictionary<string, ButtonManagement>>
            {
            };

            typeControls = new Dictionary<string, FormControlType>
            {
                { "Nom", FormControlType.FIELD },
                { "Adresse", FormControlType.FIELD},
                { "Code postal", FormControlType.NUMBER },
                { "Ville", FormControlType.FIELD},
                { "Nombre de places total", FormControlType.NUMBER},
                { "Nombre de places libres", FormControlType.NUMBER},
                { "Téléphone", FormControlType.NUMBER},
                { "Description", FormControlType.DESCRIPTION}
            };
        }

        public override string getType()
        {
            return "shelter";
        }

        public override bool isInRelation(string rights)
        {
            return false;
        }

        public override bool isUnknown(string rights)
        {
            return false;
        }

        public override bool isWaiting(string rights)
        {
            return false;
        }

        public override bool hasNotif(string rights)
        {
            return false;
        }

        public override bool isAdmin(string rights)
        {
            return false;
        }

        public override bool isInvited(string rights)
        {
            return false;
        }

        public override string[] getNotificationType()
        {
            string[] array = {};
            return array;
        }

        public override string[] getPrivateField()
        {
            string[] array = { };
            return array;
        }
    }
}
