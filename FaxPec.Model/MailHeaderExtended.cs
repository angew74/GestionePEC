using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SendMail.Model;

namespace FaxPec.Model
{
    public enum MailStatusServer
    {
        [Description("IGNOTO")]
        UNKNOWN = -1,
        PRESENTE,
        CANCELLATA
    }

    

    public class MailHeaderExtended : ActiveUp.Net.Mail.UnisysExt.MailHeader
    {
        public string CC { get; set; }
        public string CCn { get; set; }
        public string MailPartialText { get; set; }
        public DateTime ReceiveDate { get; set; }
        public MailStatusServer ServerStatus { get; set; }
        public MailStatus MailStatus { get; set; }
        public int MailRating { get; set; }
    }
}
