using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AspNet.Identity.SQLServerProvider;

namespace GestionePEC.Models
{
    public class RolesModel
    {
        public string message;
        public string success;

        public List<IdentityRole> RolesList { get; internal set; }
        public string Totale { get; internal set; }
    }
}