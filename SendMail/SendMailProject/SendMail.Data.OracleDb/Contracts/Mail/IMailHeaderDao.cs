using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.DataContracts.Interfaces;
using ActiveUp.Net.Mail;
using SendMail.Model;
using SendMail.Model.Wrappers;

namespace SendMail.Data.Contracts.Mail
{
    public interface IMailHeaderDao : IDao<MailHeaderExtended, string>
    {
        
        ResultList<MailHeaderExtended> GetAllMailsReceivedByAccount(String account, string folder, int da, int per);
        ResultList<MailHeaderExtended> GetAllMailsSentByAccount(String account,string folder, int da, int per);
        ResultList<MailHeaderExtended> GetAllMailsCanceledByAccount(string account,string folder, int da, int per);
        ResultList<MailHeaderExtended> GetAllMailsReceivedArchivedByAccount(string account,string folder, int da, int per);
        ResultList<MailHeaderExtended> GetAllMailsSentArchivedByAccount(string account,string folder, int da, int per);
        ResultList<MailHeaderExtended> GetMailsByParams(string account, string folder,string parentFolder, Dictionary<MailIndexedSearch, List<string>> searchValues, int da, int per);
        IList<SimpleTreeItem> GetMailTree(string account, long idMail);
        List<ActiveUp.Net.Common.DeltaExt.Action> GetFolderDestinationForAction();
        int UpdateMailStatus(String account, List<String> idMails, MailStatus newStatus,string actionid, string utente);
        int UpdateMailSentStatus(String account, List<String> idMails, MailStatus newStatus, string utente,string action, string parentFolder);
        int RipristinoMailStatus(String account, List<String> idMails,string parentFolder, string utente);
        ResultList<MailHeaderExtended> GetAllMailArchivedByAccount(string account, string folder, int da, int per);
        ResultList<MailHeaderExtended> GetAllMailsReceivedCanceledByAccount(string account, string idfolder, int da, int per);
        ResultList<MailHeaderExtended> GetAllMailsSentCanceledByAccount(string account, string idfolder, int da, int per);
        List<Folder> getAllFolders();      
        int RipristinoMailStatusOutBox(string account, List<string> idMail, string parentFolder, string utente);
        ResultList<MailHeaderExtended> GetMailsGridByParams(string account, string folder,string box,string tipo, Dictionary<MailTypeSearch, string> searchValues, int da, int per);
        List<Folder> getFoldersByAccount(decimal IdAccount);


        bool UpdateAllMails(MailStatus mailStatus, string account, string folder, string utente, Dictionary<MailTypeSearch, string> idx);

        bool MoveAllMails(string account,decimal idAccount, string newFolder, string oldFolder, string utente, string parentFolder, Dictionary<MailTypeSearch, string> idx);

        ResultList<MailHeaderExtended> GetMailsMoveGridByParams(string account, string folder, string box, string tipo, Dictionary<MailTypeSearch, string> searchValues,bool chkUfficio, int da, int per);
    }
}
