using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ActiveUp.Net.Mail;


using Com.Delta.Logging;
using Com.Delta.Mail.Facades;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Business.Base;
using SendMail.Data.Contracts.Mail;
using SendMail.Locator;
using SendMail.DataContracts.Interfaces;
using System.Text.RegularExpressions;
using Com.Delta.Logging.Errors;
using log4net;
namespace SendMail.Business.MailFacades
{
    //questa classe singleton gestisce completamente le configurazioni dei server di posta...
    //TODO: metodo refresh per cancellare la lista statica
    //TODO: sarebbe meglio farlo a livello DAO??
    public class MailServerConfigFacade : BusinessBase, IMailServerConfigFacade
    {

        #region Attributes

        private static readonly ILog _log = LogManager.GetLogger("MailServerConfigFacade");

        private static IList<MailServer> Config = new List<MailServer>();

        private static MailServerConfigFacade facade = new MailServerConfigFacade();


        #endregion


        //Region for singleton
        #region Singleton

        public static MailServerConfigFacade GetInstance()
        {
            if (facade == null)
                facade = new MailServerConfigFacade();
            return facade;
        }

        #endregion

        #region Constructor

        //inizializza la sincronia con la tabella dei server
        private MailServerConfigFacade()
        {
            GetAll();
        }

        #endregion


        #region Mail Server Config

        public IList<MailServer> GetAll()
        {
            if (Config.Count == 0)
            {
                using (IMailServerDao dao = getDaoContext().DaoImpl.MailServerDao)
                {
                    Config = (List<MailServer>)dao.GetAll();
                }
            }

            return Config;
        }

        public MailServer LoadServerConfigById(decimal id)
        {
            MailServer m = Config.Single(delegate(MailServer p) { return p.Id == id; });
            if (m == null)
                using (IMailServerDao dao = getDaoContext().DaoImpl.MailServerDao)
                {
                    Config.Add(dao.GetById(id));
                    m = Config.Single(delegate(MailServer p) { return p.Id == id; });
                }
            return m;
        }

        public void updateServerConfig(MailServer e)
        {
            MailServer m = Config.SingleOrDefault(x => x.Id == e.Id);
            if (m == null)
            {
                //TASK: Allineamento log - Ciro
                ManagedException mEx = new ManagedException("Record cancellato da un'altro utente", "ERR_SM001", string.Empty,
                    string.Empty, null);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                //err.loggingAppCode = "SEND_MAIL";
                //err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                _log.Error(mEx);
                throw mEx;
                //throw new ManagedException("Record cancellato da un'altro utente", "", "", "", "", "", "", null);
            }
            using (IMailServerDao dao = getDaoContext().DaoImpl.MailServerDao)
            {
                dao.Update(e);
                Config.Remove(m);
                Config.Add(e);
            }
        }

        public void insertServerConfig(MailServer e)
        {
            using (IMailServerDao dao = getDaoContext().DaoImpl.MailServerDao)
            {
                dao.Insert(e);
                Config.Add(e);
            }
        }

        public void deleteServerConfig(decimal id)
        {
            using (IMailServerDao dao = getDaoContext().DaoImpl.MailServerDao)
            {
                dao.Delete(id);
                MailServer m = Config.Single(delegate(MailServer p) { return p.Id == id; });
                Config.Remove(m);
            }
        }

        #endregion

        #region MailUsers

        public IList<MailUser> GetAllManagedUsers()
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return (IList<MailUser>)dao.GetAllManaged();
            }
        }

        public MailUser GetManagedUserByAccount(string account)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetManagedUserByAccount(account);
            }

        }

        public MailUser GetUserByUserId(decimal userId)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetById(userId);
            }
        }

        public IList<MailUser> GetUserByServerAndUsername(decimal idServer, string userName)
        {
            using (IMailAccountDao dao = getDaoContext().DaoImpl.MailAccountDao)
            {
                return dao.GetUserByServerAndUsername(idServer, userName);
            }
        }

        public IList<MailUser> GetUserByUserIdList(List<decimal> userIdList)
        {
            throw new NotImplementedException();
        }

        public IList<MailUser> GetManagedAccountByUser(string username)
        {
            SendMail.Model.BackendUser us = null;
            using (IBackendUserDao dao = getDaoContext().DaoImpl.BackendUserDao)
            {
                us = dao.GetByUserName(username);
            }
            IList<MailUser> l = null;
            if (us != null && us.MappedMails != null)
                l = us.MappedMails.Select(x => (MailUser)x).OrderBy(u => u.EmailAddress, new MailComparer()).ToList();
            return l;
        }

        #endregion

        class MailComparer : IComparer<string>
        {

            #region IComparer<string> Membri di

            public int Compare(string x, string y)
            {
                if (y == null) return -1;

                string[] str1 = Regex.Split(x, @"(\d+)");
                string[] str2 = Regex.Split(y, @"(\d+)");
                int resp = 0;
                for (int j = 0, lun1 = str1.Length, lun2 = str2.Length;
                    j < lun1 && j < lun2; j++)
                {
                    int num1 = 0, num2 = 0;
                    if (int.TryParse(str1[j], out num1) && int.TryParse(str2[j], out num2))
                    {
                        if ((resp = num1.CompareTo(num2)) != 0)
                            break;
                    }
                    int lungh = Math.Min(str1[j].Length, str2[j].Length);
                    if((resp = str1[j].Substring(0, lungh).CompareTo(str2[j].Substring(0, lungh))) != 0)
                        break;
                }
                return resp;
            }

            #endregion
        }
    }

}

