using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;

namespace Com.Delta.Mail.Facades
{
    public interface IMailServerConfigFacade
    {
        IList<MailServer> GetAll();
        MailServer LoadServerConfigById(decimal id);
        void updateServerConfig(MailServer e);
        void insertServerConfig(MailServer e);
        void deleteServerConfig(decimal id);
        IList<MailUser> GetAllManagedUsers();
        MailUser GetManagedUserByAccount(string account);
        IList<MailUser> GetManagedAccountByUser(string username);
        MailUser GetUserByUserId(decimal userId);
        IList<MailUser> GetUserByUserIdList(List<decimal> userId);
        IList<MailUser> GetUserByServerAndUsername(decimal idServer, string userName);
    }
}
