using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;
using ActiveUp.Net.Mail.DeltaExt;

namespace SendMail.DataContracts.Interfaces
{
    public interface IMailDao : IDao<Mail, Int64>
    {
        ICollection<Mail> GetMailsWithAttachment();
        ICollection<Mail> GetMailsWithoutAttachment();
        ICollection<Mail> GetSentMails();
        ICollection<Mail> GetUnsentMails();
    }
}
