using ActiveUp.Net.Mail.DeltaExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class MailModel
    {
        internal string message;
        internal string success;

        public List<CasellaMail> ElencoMails { get; internal set; }
        public string Totale { get; internal set; }

        public class CasellaMail
        {
           public string emailAddress { get; set; }
           public string idMail { get; set; }
        }
    }
}