using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ActiveUp.Net.Common.DeltaExt;

namespace ActiveUp.Net.Mail.DeltaExt
{

    public class MailHeaderExtended : MailHeader
    {
        public string CC { get; set; }
        public string CCn { get; set; }
        public string MailPartialText { get; set; }
        public DateTime ReceiveDate { get; set; }
        public MailStatusServer ServerStatus { get; set; }
        public MailStatus MailStatus { get; set; }
        public int MailRating { get; set; }
        public bool HasAttachments { get; set; }
        public string Utente { get; set; }
        public int Dimensione { get; set; }
        public decimal FolderId { get; set; }
        public string FolderTipo { get; set; }
    }
}
