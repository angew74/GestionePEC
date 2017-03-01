using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Business.Base;
using SendMail.Business.Contracts;
using SendMail.Data.Contracts.Mail;
using SendMail.Model;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Business.MailFacades
{
    public sealed class MailAccountService:BaseSingletonService<MailAccountService>, IMailAccountService
    {

        #region IMailAccountService Membri di

        public IList<MailUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<MailUser> GetAllManaged()
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return (IList<ActiveUp.Net.Mail.DeltaExt.MailUser>)dao.GetAllManaged();
            }
        }

        public MailUser GetManagedUserByAccount(string account)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetManagedUserByAccount(account);
            }
        }

        public ActiveUp.Net.Mail.DeltaExt.MailUser GetById(decimal id)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetById(id);
            }
        }

        public void Insert(MailUser user)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                dao.Insert(user);
            }
        }

        public void Update(MailUser user)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                dao.Update(user);
            }
        }

        public void Delete(decimal id)
        {
            throw new NotImplementedException();
        }

        public IList<MailUser> GetUserByServerAndUsername(decimal idServer, string userName)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetUserByServerAndUsername(idServer, userName);
            }
        }

        public IList<MailUser> GetUserByUserIdList(List<decimal> userId)
        {
            throw new Exception("Not Implemented Exception!!");
            //RAFFAELE
        }

        public IList<MailUser> GetUsersByMails(IList<String> mails)
        {
            using (IMailAccountDao dao = this.getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetUsersByMails(mails);
            }
        }

        public List<Folder> GetAllFoldersByAccount(int id)
        {
            using (IMailAccountDao dao = this.getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetAllFoldersByAccount(id);
            }
        }


        #endregion
    }
}
