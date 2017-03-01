using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;


namespace Com.Delta.Mail.Facades
{
    public interface IMailServerFacade
    {
        MailUser AccountInfoCurrentGet();
        Message getMessage(string id, bool refresh);
        Message getMessageByOrdinal(String psOrdinal, bool refresh);
        Message getMessageByIndex(int index, bool refresh);
        void IncomingConnect();
        void IncomingDisconnect();
        int MailHeaderLoad();
        void MailHeader_ReloadForce();
        List<MailHeader> MailHeader_ArrayList_Fetch(Boolean Reload, int pidxStart, int piCount);
        List<MailHeader> MailHeader_ArrayList_Fetch(int pidxStart, int piCount);
        List<MailHeader> ListCurrentGet();
        bool MailHeader_ItemRemove(int piOrdinal);
        bool MailHeader_ItemRemove(String pUId);
        void ReConnect();
        //List<MailHeader> Retrieves(bool refresh);
        void sendMail(ActiveUp.Net.Mail.Message message);
        DateTime LastRefresh { get; }
        string MailBoxName { get; set; }

        ResultList<MailHeader> MailHeader_ResultList_Fetch(Boolean Reload, int pidxStart, int piCount);
        ResultList<MailHeader> MailHeader_ResultList_Fetch(int pidxStart, int piCount);
        ResultList<MailHeader> MailHeader_ResultList_Fetch(string mailFolder,string parentFolder, Boolean Reload, int pidxStart, int piCount);
        ResultList<MailHeader> MailHeader_ResultList_Fetch(string mailFolder,string parentFolder, int pidxStart, int piCount);       
        int MailMove(List<string> idMail, MailStatus newStatus,string action, string utente,string parentFolder);
        int MailArchivia(List<string> idMail, string utente,string mailAction,string parentFolder);
        //int MailRipristina(List<string> idMail,string parentFolder, string utente);

        int AssignMessageRating(string idMail,int rating);

        int MailCancella(List<string> idMail, string utente, string p, string parentFolder);
    }
}
