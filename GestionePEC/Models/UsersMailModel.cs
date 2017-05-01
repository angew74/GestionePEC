using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class UsersMailModel
    {
        public string message;
        public string success;
        public UserMail[] UsersList;

        public string Totale { get; internal set; }
    }

    public class UserMail
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}