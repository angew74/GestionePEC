using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendMail.Model;

namespace GestionePEC.Models
{
    public class BackendUserModel
    {
        internal string message;
        internal string success;

        public List<BackEndUserMailUserMapping> ListBackendUsers { get; internal set; }
        public string Totale { get; internal set; }
    }
}