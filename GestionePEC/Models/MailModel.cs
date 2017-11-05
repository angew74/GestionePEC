using ActiveUp.Net.Mail.DeltaExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ActiveUp.Net.Mail;

namespace GestionePEC.Models
{
    public class MailModel
    {
       public  string message { get; set; }
       public string  success { get; set; }

        public List<CasellaMail> ElencoMails { get; set; }
        public List<ViewMail> Mail { get; set; }
        public string Totale { get; set; }
       
        public class CasellaMail
        {
           public string emailAddress { get; set; }
           public string idMail { get; set; }
        }
    }

    public class ViewMail
    {
        public string Mail { get; set; }
        public string DestinatarioA { get; set; }
        public string DestinatarioCC { get; set; }
        public string Oggetto { get; set; }
        public List<ViewAttachement> Allegati { get; set; }
        public string TestoMail { get; set; }
        public bool DestinatarioABlank { get; set; }
        public bool IncludiAllegati { get; set; }
    }

    public class ViewAttachement
    {
        public string NomeFile { get; set; }
        public string ContentiId { get; set; }
        public long Dimensione { get; set; }
    }
}