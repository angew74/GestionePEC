using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;

namespace SendMail.BusinessEF.Contracts
{
    public interface IMailService
    {
        Mail GetMail(Int64 idMail);
        ICollection<Mail> GetAllMails();
        ICollection<Mail> GetMailsWithAttachment();
        ICollection<Mail> GetMailsWithoutAttachment();
        ICollection<Mail> GetSentMails();
        ICollection<Mail> GetUnsentMails();    
        void Update(Mail mail);
        void SendMail(Int64 idRubrica);      
        int Save(Mail mail);
        Mail GetMailByComId(string comId);     
    }
}
