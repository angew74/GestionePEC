using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Data.SQLServerDB;
using SendMail.Model;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Data.Contracts.Mail
{
    public interface IMailAccountDao : IDao<MailUser, decimal>
    {
        ICollection<MailUser> GetAllManaged();
        IList<MailUser> GetUserByServerAndUsername(decimal idServer, string userName);
        IList<MailUser> GetUsersByMails(IList<String> mails);
        ActiveUp.Net.Mail.DeltaExt.MailUser GetManagedUserByAccount(string userName);
        List<Folder> GetAllFoldersByAccount(int id);
    }
}
