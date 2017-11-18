using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class StatModel
    {
        internal string message;
        internal string success;

        public List<UserResultItem> ElencoStat { get; set; }
        public string Totale { get; set; }
      

    }

}