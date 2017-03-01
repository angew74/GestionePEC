using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Mail;


namespace SendMail.Business.Contracts
{
    public interface IMailServerService
    {
        MailUser AccSettings { get; }
        DateTime LastRefresh { get; }

        MailUser AccountInfoCurrentGet();
        Message getMessage(string id, bool refresh);
        Message getMessageByOrdinal(String psOrdinal, bool refresh);
        Message getMessageByIndex(int index, bool refresh);
        int GetMessageSize(string uid);
        void IncomingConnect();
        bool IsIncomingConnected();
        void IncomingDisconnect();
        int MailHeaderLoad();
        void MailHeader_ReloadForce();
        List<MailHeader> MailHeader_ArrayList_Fetch(Boolean Reload, int pidxStart, int piCount);
        List<MailHeader> MailHeader_ArrayList_Fetch(int pidxStart, int piCount);
        List<MailHeader> ListCurrentGet();
        List<MessageUniqueId> RetrieveUIds();
        bool MailHeader_ItemRemove(int piOrdinal);
        bool MailHeader_ItemRemove(String pUId);
        void ReConnect();
        //List<MailHeader> Retrieves(bool refresh);
        void sendMail(ActiveUp.Net.Mail.Message message);

        void DeleteMessageFromServer(string uid);
    }
}
