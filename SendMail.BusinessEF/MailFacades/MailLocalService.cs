using System;
using System.Collections.Generic;
using SendMail.BusinessEF.Base;
using ActiveUp.Net.Mail.DeltaExt;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Data.Contracts.Mail;
using SendMail.Model;
using SendMail.Model.Wrappers;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF.MailFacedes
{
    public sealed class MailLocalService : BaseSingletonService<MailLocalService>
    {
        #region IMailHeaderService Membri di


        public ResultList<MailHeaderExtended> GetAllMailsReceivedByAccount(string account, string folder, int da, int per)
        {

            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetAllMailsReceivedByAccount(account, folder, da, per);
            }
        }

        public ResultList<MailHeaderExtended> GetAllMailsSentByAccount(string account, string mailfolder, int da, int per)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetAllMailsSentByAccount(account, mailfolder, da, per);
            }
        }

        public ResultList<MailHeaderExtended> GetAllMailsCanceledByAccount(string account, string mailfolder, int da, int per)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetAllMailsCanceledByAccount(account, mailfolder, da, per);
            }
        }

        public ResultList<MailHeaderExtended> GetAllMailArchivedByAccount(string account, string mailfolder, string parentFolder, int da, int per)
        {

            ResultList<MailHeaderExtended> ret = new ResultList<MailHeaderExtended>();
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                switch (parentFolder)
                {
                    default:
                        ret = dao.GetAllMailArchivedByAccount(account, mailfolder, da, per);
                        break;
                    //case "O":
                    //    ret = dao.GetAllMailsSentArchivedByAccount(account, mailfolder, da, per);
                    //    break;
                    //case "I":
                    //    ret =dao.GetAllMailsReceivedArchivedByAccount(account, mailfolder, da, per);
                    //    break;
                }
            }
            return ret;
        }

        public ResultList<MailHeaderExtended> GetMailsByParams(string account, string folder, string parentFolder, Dictionary<MailIndexedSearch, List<string>> searchValues, int da, int per)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetMailsByParams(account, folder, parentFolder, searchValues, da, per);
            }
        }

        public ResultList<MailHeaderExtended> GetMailsGridByParams(string account, string folder,string box,string tipo, Dictionary<MailTypeSearch, string> searchValues, int da, int per)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetMailsGridByParams(account, folder, box,tipo, searchValues, da, per);
            }
        }

   


        public int UpdateMailStatus(String account, List<String> idMails, MailStatus newStatus, string actionid, String utente, string parentFolder)
        {
            int i = 0;
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                switch (parentFolder)
                {
                    case "I":
                    case "A":
                        i = dao.UpdateMailStatus(account, idMails, newStatus, actionid, utente);
                        break;
                    case "O":
                    case "AO":
                        i = dao.UpdateMailSentStatus(account, idMails, newStatus, utente, actionid, parentFolder);
                        break;
                    case "C":
                        switch(actionid)
                        {
                            case "4":
                            case "14":
                                i = dao.UpdateMailStatus(account, idMails, newStatus, actionid, utente);
                                break;
                            case "6":
                                i = dao.UpdateMailSentStatus(account, idMails, newStatus, utente, actionid, parentFolder);
                                break;
                        }
                        break;
                }
            }
            return i;
        }

        public int UpdateMailSentStatus(string account, List<string> idMails, MailStatus newStatus, string utente, string action, string parentFolder)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.UpdateMailSentStatus(account, idMails, newStatus, utente, action, parentFolder);
            }
        }

        public int RipristinaMail(String account, List<String> idMail, string parentFolder, String utente)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.RipristinoMailStatus(account, idMail, parentFolder, utente);
            }
        }

        public List<ActiveUp.Net.Common.DeltaExt.Action> GetFolderDestinationForAction()
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetFolderDestinationForAction();
            }
        }

        public int RipristinaMailOutBox(String account, List<String> idMail, string parentFolder, String utente)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.RipristinoMailStatusOutBox(account, idMail, parentFolder, utente);
            }
        }

        public List<ActiveUp.Net.Common.DeltaExt.Folder> getAllFolders()
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.getAllFolders();
            }
        }

        public ICollection<ActiveUp.Net.Mail.Message> GetAll()
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.GetAll();
            }
        }


        public IDictionary<MailStatusServer, List<string>> GetAllUIDsByAccount(string account, List<MailStatusServer> stServer)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.GetAllUIDsByAccount(account, stServer);
            }
        }

        public ActiveUp.Net.Mail.Message GetById(long idMail)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.GetById(idMail.ToString());
            }
        }

        public ActiveUp.Net.Mail.Message GetById(string mailID, string mailAccount, string mailFolder)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.GetById(mailID, mailAccount, mailFolder);
            }
        }


        public System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string> GetMessageById(string idMail)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.GetMessageById(idMail);
            }
        }

        public void Insert(ActiveUp.Net.Mail.Message entity)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                dao.Insert(entity);
            }
        }

        public void Insert(ActiveUp.Net.Mail.DeltaExt.MailUser us, ActiveUp.Net.Mail.Message mex)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                dao.Insert(us, mex);
            }
        }

        public void Update(ActiveUp.Net.Mail.Message entity)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                dao.Update(entity);
            }
        }

        public void Update(MailUser user, ActiveUp.Net.Mail.Message entity)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                dao.Update(user, entity);
            }
        }

        public int UpdateMessageRating(string idMail, string account, int rating)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.UpdateRating(idMail, account, rating);
            }
        }

        public int UpdateMessageServerStatus(string account, string mailUid, MailStatusServer status)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.UpdateServerStatus(account, mailUid, status);
            }
        }

        public void Delete(string mailID, string mailAccount)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                dao.Delete(mailID, mailAccount);
            }
        }

        public ResultList<ActiveUp.Net.Mail.Message> GetAllMessageByAccount(string account, int da, int per)
        {
            using (MailMessageDaoSQLDb dao = new MailMessageDaoSQLDb())
            {
                return dao.GetAllMessageByAccount(account, da, per);
            }
        }

        public IList<SimpleTreeItem> LoadMailTree(string account, long idMail)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetMailTree(account, idMail);
            }
        }
        #endregion

        public List<Folder> getFoldersByAccount(decimal IdAccount)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.getFoldersByAccount(IdAccount);
            }
        }

        public bool UpdateAllMails(MailStatus mailStatus, string account, string folder, string utente, Dictionary<MailTypeSearch, string> idx)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.UpdateAllMails(mailStatus,account,folder,utente,idx);
            }
        }

        public bool MoveAllMails(string account,decimal idaccount, string newFolder, string oldFolder, string utente, string parentFolder, Dictionary<MailTypeSearch, string> idx)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.MoveAllMails(account,idaccount, newFolder, oldFolder, utente,parentFolder, idx);
            }
        }

        public ResultList<MailHeaderExtended> GetMailsMoveGridByParams(string account, string folder, string box, string tipo, Dictionary<MailTypeSearch, string> searchValues,bool chkUfficio, int da, int per)
        {
            using (MailHeaderDaoSQLDb dao = new MailHeaderDaoSQLDb())
            {
                return dao.GetMailsMoveGridByParams(account, folder, box, tipo, searchValues,chkUfficio, da, per);
            }
        }

    }
}
