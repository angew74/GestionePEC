using AspNet.Identity.SQLServerProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Delta.Security
{
   public class MyIdentity : IdentityUser
    {
        internal MyIdentity(string username, string dipartimento, string passwordHash, string type)
           : base(username)
        {
            base.SecurityStamp = System.DateTime.Now.Ticks.ToString();
            base.PasswordHash= passwordHash;
            this.dipartimento = dipartimento;
            authenticated = true;
        }
        public MyIdentity(string username, string dipartimento)
           : base(username)
        {
            identityCreationDate = System.DateTime.Now;
            authenticated = false;
            this.dipartimento = dipartimento;
        }

        public override bool IsAuthenticated
        {
            get
            {
                return authenticated;
            }            
        }
              

        public string NomeCognome
        {
            get
            {
                return _NomeCognome;
            }
            set
            {
                _NomeCognome = value;
            }
        }

        private bool authenticated;
        private DateTime identityCreationDate;      
        public string dipartimento;   
        private string _NomeCognome;       

        public DateTime IdentityCreationDate
        {
            get { return identityCreationDate; }
            set { identityCreationDate = value; }
        }

        public bool checkIdentity(string username, string password)
        {
            if (this.PasswordHash != null && base.UserName.ToLower().Equals(username.ToLower()) && this.PasswordHash.ToLower().Equals(password.ToLower())) return true;
            else return false;
        }
    }
}

