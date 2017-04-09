using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.BusinessEF.Contracts
{
   public interface IMailRefs
    {
        //service per ottenere un allegato dalla key
        MailRefs GetMailRef(Int64 idRef);       
        //service per update di un allegato
        void UpdateMailRef(MailRefs mailRefs, bool toCommit);
        //service per update di allegati
        void UpdateMailRefs(ICollection<MailRefs> attachmentList, bool toCommit);
        void Save(ICollection<SendMail.Model.MailRefs> mailRefs, bool toCommit);
    }
}
