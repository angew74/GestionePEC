using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Data.SQLServerDB;

namespace SendMail.Data.Contracts.Mail
{
    public interface IMailMessageDao : IDao<Message, string>
    {
        IDictionary<ActiveUp.Net.Common.DeltaExt.MailStatusServer, List<String>> GetAllUIDsByAccount(String account, List<MailStatusServer> serverStatus);
        void Insert(MailUser user, Message m);
        void InsertFlussoInbox(Int64 id, MailStatus oldSt, MailStatus newSt, DateTime? dataOp, string uteOpe);
        Message GetById(string mailID, string mailAccount, string mailFolder);
        Message GetOutBoxMessageByComId(string comId);
        Tuple<Message, string, int,string> GetMessageById(string id);
        void Delete(string mailID, string mailAccount);
        ActiveUp.Net.Mail.DeltaExt.ResultList<Message> GetAllMessageByAccount(string account, int da, int per);
        int UpdateRating(string idMail, string account, int rating);
        int UpdateServerStatus(string account, string mailUid, MailStatusServer status);
        void Update(MailUser u, Message m);       
    }
}
