using System;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.SQLServerProvider
{
   
    public class IdentityUser : System.Security.Principal.GenericIdentity, IUser
    {
        public IdentityUser() : base("ANONYMOUS") { }
        public IdentityUser(string username, string type) : base(username, type) { }
       

        private bool isAuthenticated;

        public override bool IsAuthenticated
        {
            get
            {
                return isAuthenticated;
            }
        }

        internal void setAuthenticated(bool authenticated)
        {
            this.isAuthenticated = authenticated;
        }

        public IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }
    }
}