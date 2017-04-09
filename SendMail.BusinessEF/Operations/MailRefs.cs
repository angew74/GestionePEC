using System;
using System.Collections.Generic;
using SendMail.Model;
using SendMail.BusinessEF.Base;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
    public class MailRefsService : BaseSingletonService<MailRefsService>, SendMail.BusinessEF.Contracts.IMailRefs
    {

        //service per ottenere un allegato dalla key
        public MailRefs GetMailRef(Int64 idRef)
        {
            throw new NotImplementedException();
        }
        //service per update di un allegato
        public void UpdateMailRef(MailRefs mailref, bool toCommit)
        {
            throw new NotImplementedException();
        }
        //service per update di allegati
        public void UpdateMailRefs(ICollection<MailRefs> mailRefsList, bool toCommit)
        {
            throw new NotImplementedException();
        }
        public void Save(ICollection<SendMail.Model.MailRefs> mailRefsList, bool toCommit)
        {

            using (MailRefsSQLDb dao = new MailRefsSQLDb())
            {
                foreach (SendMail.Model.MailRefs mailref in mailRefsList)
                {
                    dao.Insert(mailref);
                }
            }
        }

    }
}
