using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.BusinessEF.Contracts
{
    public interface IMailAccountService
    {
        IList<MailUser> GetAll();
        IList<MailUser> GetAllManaged();
        MailUser GetById(decimal id);
        void Insert(MailUser user);
        void Update(MailUser user);
        void Delete(decimal id);
        //IList<MailUser> GetUserByUserId(decimal userId);
        IList<MailUser> GetUserByUserIdList(List<decimal> userId);
        IList<MailUser> GetUserByServerAndUsername(decimal idServer, string userName);
        IList<MailUser> GetUsersByMails(IList<String> mails);
        List<Folder> GetAllFoldersByAccount(int id);
    }
}
