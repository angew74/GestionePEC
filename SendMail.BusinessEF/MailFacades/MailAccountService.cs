using System;
using System.Collections.Generic;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.BusinessEF.Base;
using SendMail.Data.Contracts.Mail;
using SendMail.BusinessEF.Contracts;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF.MailFacedes
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
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                return (IList<ActiveUp.Net.Mail.DeltaExt.MailUser>)dao.GetAllManaged();
            }
        }

        public MailUser GetManagedUserByAccount(string account)
        {
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                return dao.GetManagedUserByAccount(account);
            }
        }

        public ActiveUp.Net.Mail.DeltaExt.MailUser GetById(decimal id)
        {
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                return dao.GetById(id);
            }
        }

        public void Insert(MailUser user)
        {
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                dao.Insert(user);
            }
        }

        public void Update(MailUser user)
        {
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
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
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                return dao.GetUserByServerAndUsername(idServer, userName);
            }
        }

        public IList<MailUser> GetUserByUserIdList(List<decimal> userId)
        {
            throw new Exception("Not Implemented Exception!!");          
        }

        public IList<MailUser> GetUsersByMails(IList<String> mails)
        {
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                return dao.GetUsersByMails(mails);
            }
        }

        public List<Folder> GetAllFoldersByAccount(int id)
        {
            using (MailAccountSQLDb dao = new MailAccountSQLDb())
            {
                return dao.GetAllFoldersByAccount(id);
            }
        }


        #endregion
    }
}
