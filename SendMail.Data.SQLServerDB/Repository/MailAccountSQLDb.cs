using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.SQLServerDB.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class MailAccountSQLDb : IMailAccountDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailAccountSQLDb));

        #region Private Cons Strings


        private const string queryActions = " SELECT ACTIONS.ID, ACTIONS.NOME_AZIONE,ACTIONS.ID_NOME_DESTINAZIONE,ACTIONS.  TIPO_DESTINAZIONE,ACTIONS.TIPO_AZIONE, ACTIONS.NUOVO_STATUS,ACTIONS_FOLDERS.IDFOLDER,ACTIONS.ID_FOLDER_DESTINAZIONE " + " FROM ACTIONS, ACTIONS_FOLDERS,FOLDERS_SENDERS,FOLDERS WHERE FOLDERS.ID=:IDF AND FOLDERS_SENDERS.IDFOLDER=FOLDERS.ID " + " AND FOLDERS_SENDERS.IDSENDER =:IDS AND FOLDERS_SENDERS.IDFOLDER= ACTIONS_FOLDERS.IDFOLDER "
+ " AND ACTIONS.ID= ACTIONS_FOLDERS.IDACTION AND ((ID_FOLDER_DESTINAZIONE IN (SELECT FOLDERS_SENDERS.IDFOLDER " + " FROM FOLDERS_SENDERS WHERE IDSENDER = :IDS)) OR (ID_FOLDER_DESTINAZIONE IS NULL AND FOLDERS.TIPO IN ('I','E')))";

        #endregion
        #region C.tor


        #endregion

        #region IMailAccountDao Membri di

        public ICollection<ActiveUp.Net.Mail.DeltaExt.MailUser> GetAllManaged()
        {

            ICollection<MailUser> users = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    var mailsenders = dbcontext.MAIL_SENDERS.Where(x => x.FLG_MANAGED != null).ToList();
                    foreach (MAIL_SENDERS sender in mailsenders)
                    {
                        int idMailServer = (int)sender.ID_MAILSERVER;
                        int idUser = (int)sender.ID_SENDER;
                        if (idMailServer != 0)
                        {
                            MAILSERVERS m = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == idMailServer).FirstOrDefault();
                            MailServer s = AutoMapperConfiguration.FromMailServersToModel(m);
                            if (idUser != 0)
                            {
                                List<Folder> l = GetMailFolders(idUser);
                                users.Add(DaoSQLServerDBHelper.MapToMailUser(sender, s, l));
                            }
                        }
                    }
                }
                catch
                {
                    users = null;
                    throw;
                }
            }
            return users;
        }

        public ActiveUp.Net.Mail.DeltaExt.MailUser GetManagedUserByAccount(string userName)
        {
            MailUser User = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    var mailsender = dbcontext.MAIL_SENDERS.Where(x => x.FLG_MANAGED == "2" && x.MAIL.ToUpper() == userName.ToUpper()).FirstOrDefault();
                    int idmailserver = (int)mailsender.ID_MAILSERVER;
                    int idmailuser = (int)mailsender.ID_SENDER;
                    if (idmailserver != 0)
                    {
                        MAILSERVERS m = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == idmailserver).FirstOrDefault();
                        MailServer s = AutoMapperConfiguration.FromMailServersToModel(m);
                        if (idmailuser != 0)
                        {
                            List<Folder> l = GetMailFolders(idmailuser);
                            User = DaoSQLServerDBHelper.MapToMailUser(mailsender, s, l);
                        }
                    }
                }
                catch
                {
                    User = null;
                    throw;
                }
            }
            return User;
        }

        public IList<MailUser> GetUserByServerAndUsername(decimal idServer, string userName)
        {
            List<MailUser> lUser = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                var mailsender = dbcontext.MAIL_SENDERS.Where(x => x.ID_MAILSERVER == idServer && x.USERNAME.ToUpper() == userName.ToUpper()).FirstOrDefault();
                try
                {
                    lUser = new List<MailUser>();
                    MAIL_SENDERS m = dbcontext.MAIL_SENDERS.Where(x => x.ID_MAILSERVER == idServer).FirstOrDefault();
                    int idmailserver = (int)m.ID_MAILSERVER;
                    MAILSERVERS ms = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == idmailserver).FirstOrDefault();
                    int idmailuser = (int)mailsender.ID_SENDER;
                    MailServer s = AutoMapperConfiguration.FromMailServersToModel(ms);
                    if (idmailuser != 0)
                    {
                        List<Folder> l = GetMailFolders(idmailuser);
                        lUser.Add(DaoSQLServerDBHelper.MapToMailUser(mailsender, s, l));
                    }
                }
                catch
                {
                    lUser = null;
                    throw;
                }
            }
            return lUser;
        }

        public IList<MailUser> GetUsersByMails(IList<String> mails)
        {
            List<MailUser> lMU = null;
            string[] allMails = mails.ToArray();
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    lMU = new List<MailUser>();
                    List<MAIL_SENDERS> l = (from e in dbcontext.MAIL_SENDERS
                                            where allMails.Contains(e.MAIL)
                                            select e).ToList();
                    foreach (MAIL_SENDERS m in l)
                    {
                        int idmailserver = (int)m.ID_MAILSERVER;
                        MAILSERVERS ms = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == idmailserver).FirstOrDefault();
                        MailServer s = AutoMapperConfiguration.FromMailServersToModel(ms);
                        int idmailuser = (int)m.ID_SENDER;
                        List<Folder> list = GetMailFolders(idmailuser);
                        lMU.Add(DaoSQLServerDBHelper.MapToMailUser(m, s, list));
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_ACC_001", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    err.objectID = string.Empty;
                    log.Error(err);
                }
                lMU = null;
            }
            return lMU;
        }

        public List<Folder> GetAllFoldersByAccount(int UserId)
        {
            List<Folder> list = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        //var foldersSenders = dbcontext.FOLDERS_SENDERS.Where(x => x.IDSENDER == UserId).ToList();
                        //foreach (FOLDERS_SENDERS f in foldersSenders)
                        //{
                        //   List<Folder> a = AutoMapperConfiguration.MapToFolderModel(f);
                        //}
                        string queryFolder = "SELECT FOLDERS.ID, FOLDERS.NOME, TIPO,SYSTEM,IDNOME FROM FOLDERS,FOLDERS_SENDERS WHERE IDFOLDER=FOLDERS.ID AND IDSENDER=" + UserId;
                        oCmd.CommandText = queryFolder;
                        using (DbDataReader r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                list = new List<Folder>();
                                while (r.Read())
                                {
                                    List<ActiveUp.Net.Common.DeltaExt.Action> la = GetActionsFolder(r, UserId);
                                    list.Add(DaoSQLServerDBHelper.MapToFolder(r, la));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_ACC_002", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    err.objectID = UserId.ToString();
                    log.Error(err);
                }
            }
            return list;

        }

        public List<ActiveUp.Net.Common.DeltaExt.Action> GetAllActionsByFolder(int id, string tipo, int IdUser)
        {
            List<ActiveUp.Net.Common.DeltaExt.Action> list = new List<ActiveUp.Net.Common.DeltaExt.Action>();
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        string queryActions = " SELECT ACTIONS.ID, ACTIONS.NOME_AZIONE,ACTIONS.ID_NOME_DESTINAZIONE,ACTIONS.TIPO_DESTINAZIONE, " +
                        " ACTIONS.TIPO_AZIONE, ACTIONS.NUOVO_STATUS,ACTIONS_FOLDERS.IDFOLDER,ACTIONS.ID_FOLDER_DESTINAZIONE " + " FROM ACTIONS, ACTIONS_FOLDERS,FOLDERS_SENDERS,FOLDERS " +
                         " WHERE FOLDERS.ID= " + id + " AND FOLDERS_SENDERS.IDFOLDER=FOLDERS.ID " + " AND FOLDERS_SENDERS.IDSENDER = " + id + " AND FOLDERS_SENDERS.IDFOLDER= ACTIONS_FOLDERS.IDFOLDER "
                        + " AND ACTIONS.ID= ACTIONS_FOLDERS.IDACTION AND ((ID_FOLDER_DESTINAZIONE IN (SELECT FOLDERS_SENDERS.IDFOLDER " + " FROM FOLDERS_SENDERS WHERE IDSENDER = " + IdUser + " )) " +
                         "OR (ID_FOLDER_DESTINAZIONE IS NULL AND FOLDERS.TIPO IN ('I','E')))";
                        oCmd.CommandText = queryActions;
                        using (var r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                list = new List<ActiveUp.Net.Common.DeltaExt.Action>();
                                while (r.Read())
                                {
                                    list.Add(DaoSQLServerDBHelper.MapToAction(r));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_ACC_003", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    err.objectID = id.ToString();
                    log.Error(err);
                }
            }
            return list;
        }

        #endregion

        #region IDao<MailUser,decimal> Membri di

        public ICollection<ActiveUp.Net.Mail.DeltaExt.MailUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public ActiveUp.Net.Mail.DeltaExt.MailUser GetById(decimal id)
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser user = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                var mailsender = dbcontext.MAIL_SENDERS.Where(x => x.ID_SENDER == id).FirstOrDefault();
                try
                {
                    int idmailserver = (int)mailsender.ID_MAILSERVER;
                    MAILSERVERS ms = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == idmailserver).FirstOrDefault();
                    MailServer s = AutoMapperConfiguration.FromMailServersToModel(ms);
                    int idmailuser = (int)mailsender.ID_SENDER;
                    List<Folder> list = GetMailFolders(idmailuser);
                    user = DaoSQLServerDBHelper.MapToMailUser(mailsender, s, list);
                }
                catch
                {
                    user = null;
                    throw;
                }
            }
            return user;
        }

        public void Insert(ActiveUp.Net.Mail.DeltaExt.MailUser entity)
        {
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    MAIL_SENDERS s = new MAIL_SENDERS();
                    s.USERNAME = entity.EmailAddress;
                    s.ID_MAILSERVER = entity.Id;
                    s.PASSWORD = entity.Password;
                    s.ID_RESPONSABILE = 1;
                    s.FLG_MANAGED = "0";
                    dbcontext.MAIL_SENDERS.Add(s);
                    int resp = dbcontext.SaveChanges();
                    if (resp == 1)
                    {
                        entity.UserId = s.ID_SENDER;
                    }
                }
            }
            catch (Exception ex)
            {

                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_ACC_004", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = entity.Id.ToString();
                    log.Error(err);
                }
            }
        }

        public void Update(ActiveUp.Net.Mail.DeltaExt.MailUser entity)
        {
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    MAIL_SENDERS m = dbcontext.MAIL_SENDERS.Where(x => x.ID_SENDER == entity.UserId).First();
                    if (m != null)
                    {
                        m.PASSWORD = entity.Password;
                        m.MAIL = entity.EmailAddress;
                        m.ID_MAILSERVER = entity.Id;
                        m.USERNAME = entity.LoginId;
                        dbcontext.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_ACC_005", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = entity.Id.ToString();
                    log.Error(err);
                }
            }
        }

        public void Delete(decimal id)
        {
            throw new NotImplementedException();
        }

        #endregion

     
        #region Private methods


        private List<Folder> GetMailFolders(int idUser)
        {
            List<Folder> ml = null;
            if (idUser != default(decimal))
            {
                ml = GetAllFoldersByAccount(idUser);
            }
            return ml;
        }

        private List<ActiveUp.Net.Common.DeltaExt.Action> GetActionsFolder(DbDataReader r, int IdUser)
        {
            List<ActiveUp.Net.Common.DeltaExt.Action> a = null;
            string id = r.GetDecimal("ID").ToString();
            string tipo = r.GetString("TIPOFOLDER");
            int idFolder = 0;
            int.TryParse(id, out idFolder);
            if (idFolder != default(decimal))
            {
                a = GetAllActionsByFolder(idFolder, tipo, IdUser);
            }
            return a;

        }

        #region IDisposable Support
        private bool disposedValue = false; // Per rilevare chiamate ridondanti

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: eliminare lo stato gestito (oggetti gestiti).
                }

                // TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire sotto l'override di un finalizzatore.
                // TODO: impostare campi di grandi dimensioni su Null.

                disposedValue = true;
            }
        }

        // TODO: eseguire l'override di un finalizzatore solo se Dispose(bool disposing) include il codice per liberare risorse non gestite.
        // ~MailAccountSQLDb() {
        //   // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
        //   Dispose(false);
        // }

        // Questo codice viene aggiunto per implementare in modo corretto il criterio Disposable.
        public void Dispose()
        {
            // Non modificare questo codice. Inserire il codice di pulizia in Dispose(bool disposing) sopra.
            Dispose(true);
            // TODO: rimuovere il commento dalla riga seguente se è stato eseguito l'override del finalizzatore.
            // GC.SuppressFinalize(this);
        }
        #endregion


        #endregion

    }
}
