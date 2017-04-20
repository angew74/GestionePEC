using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class UsersModel
    {
        public string Totale { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public List<UserRoles> UtentiList { get; set; }
        public class UserRoles
        {
            public string Id { get; set; }
            public string Role { get; set; }
            public string UserName { get; set; }
        }
    }
}