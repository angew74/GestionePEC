using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Contracts;
using SendMail.Business.Base;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;

namespace SendMail.Business
{
    public class MailRefsService : BaseSingletonService<MailRefsService>, SendMail.Business.Contracts.IMailRefs
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

            using (IMailRefsDao dao = this.getDaoContext().DaoImpl.MailRefsDao)
            {
                foreach (SendMail.Model.MailRefs mailref in mailRefsList)
                {
                    dao.Insert(mailref);
                }
            }
        }

    }
}
