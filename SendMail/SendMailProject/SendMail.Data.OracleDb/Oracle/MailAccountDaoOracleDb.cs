using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using SendMail.DataContracts.Interfaces;
using SendMail.Data.Contracts.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Data.Utilities;
using SendMail.Model;
using ActiveUp.Net.Common.DeltaExt;
using log4net;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.OracleDb
{
    public class MailAccountDaoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IMailAccountDao
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(MailAccountDaoOracleDb));
        #region Query strings

        private const string queryBase = "SELECT ID_SENDER, MAIL, ID_MAILSERVER, USERNAME, PASSWORD, FLG_MANAGED FROM MAIL_SENDERS";

        private const string queryFolder = "SELECT FOLDERS.ID, FOLDERS.NOME, TIPO,SYSTEM,IDNOME FROM FOLDERS,FOLDERS_SENDERS WHERE IDFOLDER=FOLDERS.ID AND IDSENDER=:IDS";

        private const string queryActions = " SELECT ACTIONS.ID, ACTIONS.NOME_AZIONE,ACTIONS.ID_NOME_DESTINAZIONE,ACTIONS.  TIPO_DESTINAZIONE,ACTIONS.TIPO_AZIONE, ACTIONS.NUOVO_STATUS,ACTIONS_FOLDERS.IDFOLDER,ACTIONS.ID_FOLDER_DESTINAZIONE " + " FROM ACTIONS, ACTIONS_FOLDERS,FOLDERS_SENDERS,FOLDERS WHERE FOLDERS.ID=:IDF AND FOLDERS_SENDERS.IDFOLDER=FOLDERS.ID " + " AND FOLDERS_SENDERS.IDSENDER =:IDS AND FOLDERS_SENDERS.IDFOLDER= ACTIONS_FOLDERS.IDFOLDER "
+ " AND ACTIONS.ID= ACTIONS_FOLDERS.IDACTION AND ((ID_FOLDER_DESTINAZIONE IN (SELECT FOLDERS_SENDERS.IDFOLDER " + " FROM FOLDERS_SENDERS WHERE IDSENDER = :IDS)) OR (ID_FOLDER_DESTINAZIONE IS NULL AND FOLDERS.TIPO IN ('I','E')))";
        #endregion

        #region C.tor

        private OracleSessionManager context;
        public MailAccountDaoOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            context = daoContext;
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

        #endregion

        #region IMailAccountDao Membri di

        public ICollection<ActiveUp.Net.Mail.DeltaExt.MailUser> GetAllManaged()
        {
            ICollection<MailUser> users = null;
            using (OracleCommand cmd = base.CurrentConnection.CreateCommand())
            {
                cmd.CommandText = queryBase + " WHERE FLG_MANAGED IS NOT NULL";
                try
                {
                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            users = new List<MailUser>();

                            while (r.Read())
                            {
                                MailServer s = GetMailServer(r);
                                List<Folder> l = GetMailFolders(r);
                                users.Add(DaoOracleDbHelper.MapToMailUser(r, s,l));
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
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryBase + " WHERE  upper(MAIL) = '" + userName.ToUpper().Trim() + "' AND FLG_MANAGED=2";
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {                           
                            while (r.Read())
                            {
                                MailServer s = GetMailServer(r);
                                List<Folder> l = GetMailFolders(r);
                                User = DaoOracleDbHelper.MapToMailUser(r, s, l);
                            }
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
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryBase + " WHERE ID_MAILSERVER = " + idServer + " AND USERNAME = '" + userName + "'";
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lUser = new List<MailUser>();
                            while (r.Read())
                            {
                                MailServer s = GetMailServer(r);
                                List<Folder> l = GetMailFolders(r);
                                lUser.Add(DaoOracleDbHelper.MapToMailUser(r, s,l));
                            }
                        }
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
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = queryBase;
                    oCmd.CommandText += " WHERE MAIL IN ('" + String.Join("','", mails.ToArray()) + "')";

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lMU = new List<MailUser>();
                            while (r.Read())
                            {
                                MailServer s = GetMailServer(r);
                                List<Folder> list = GetMailFolders(r);
                                lMU.Add(DaoOracleDbHelper.MapToMailUser(r, s,list));
                                
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_ACC_001", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    
                    err.objectID = string.Empty;
                    //err.userID = string.Empty;
                    log.Error(err);
                }
                //log.Error(ex.Message);
                //if(ex.InnerException != null)
                //{
                //    log.Error(ex.InnerException.Message);
                //}
                lMU = null;
            }
            return lMU;
        }       

        public List<Folder> GetAllFoldersByAccount(int UserId)
        {
            List<Folder> list = null;           
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = queryFolder;
                    oCmd.Parameters.Add(new OracleParameter("IDS", OracleDbType.NVarchar2, UserId, ParameterDirection.Input));
                    oCmd.BindByName = true;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {

                            list = new List<Folder>();
                            while (r.Read())
                            {
                                List<ActiveUp.Net.Common.DeltaExt.Action> la = GetActionsFolder(r,UserId);
                                list.Add(DaoOracleDbHelper.MapToFolder(r,la));                             
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //TASK: Allineamento log - Ciro
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

        public List<ActiveUp.Net.Common.DeltaExt.Action> GetAllActionsByFolder(int id, string tipo,int IdUser)
        {
            List<ActiveUp.Net.Common.DeltaExt.Action> list = new List<ActiveUp.Net.Common.DeltaExt.Action>();
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                   oCmd.CommandText = queryActions;
                   oCmd.Parameters.Add(new OracleParameter("IDF", OracleDbType.NVarchar2, id, ParameterDirection.Input));
                   oCmd.Parameters.Add(new OracleParameter("IDS", OracleDbType.Decimal, IdUser, ParameterDirection.Input));
                   oCmd.BindByName = true;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            list = new List<ActiveUp.Net.Common.DeltaExt.Action>();
                            while (r.Read())
                            {                               
                                list.Add(DaoOracleDbHelper.MapToAction(r));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
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
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryBase + " WHERE ID_SENDER = " + id;
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        while (r.Read())
                        {
                            MailServer s = GetMailServer(r);
                            List<Folder> l = GetMailFolders(r);
                            user = DaoOracleDbHelper.MapToMailUser(r, s,l);
                        }
                    }
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
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "INSERT INTO MAIL_SENDERS " +
                                       "(ID_SENDER,MAIL,ID_MAILSERVER,USERNAME,PASSWORD,ID_RESPONSABILE,FLG_MANAGED) " +
                                       "VALUES (MAIL_SENDERS_SEQ.NEXTVAL,:MAIL,:ID_MAILSERVER,:USERNAME,:PASSWORD, 1, 0)" +
                                       " RETURNING ID_SENDER INTO :P_UID";

                    oCmd.Parameters.Add("MAIL", entity.EmailAddress);
                    oCmd.Parameters.Add("ID_MAILSERVER", entity.Id);
                    oCmd.Parameters.Add("USERNAME", entity.LoginId);
                    oCmd.Parameters.Add("PASSWORD", entity.Password);
                    oCmd.Parameters.Add("P_UID", OracleDbType.Int64, ParameterDirection.Output); 
                    oCmd.BindByName = true;
                    int resp = oCmd.ExecuteNonQuery();
                    if (resp == 1)
                    {
                        entity.UserId = Convert.ToDecimal(oCmd.Parameters["P_UID"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
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
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "UPDATE MAIL_SENDERS " +
                                       "SET MAIL_SENDERS.MAIL = :MAIL, " +
                                       "MAIL_SENDERS.ID_MAILSERVER = :ID_MAILSERVER, " +
                                       "MAIL_SENDERS.USERNAME = :USERNAME, " +
                                       "MAIL_SENDERS.PASSWORD = :PASSWORD " +
                                       "WHERE MAIL_SENDERS.ID_SENDER = :ID_SENDER";

                    oCmd.Parameters.Add("MAIL", entity.EmailAddress);
                    oCmd.Parameters.Add("ID_MAILSERVER", entity.Id);
                    oCmd.Parameters.Add("USERNAME", entity.LoginId);
                    oCmd.Parameters.Add("PASSWORD", entity.Password);
                    oCmd.Parameters.Add("ID_SENDER", entity.UserId);
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
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

        #region IDisposable Membri di

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #endregion


        #region Private methods

        private MailServer GetMailServer(OracleDataReader r)
        {
            MailServer s = null;
            decimal idServer = r.GetDecimal("ID_MAILSERVER");
            if (idServer != default(decimal))
            {
                s = this.Context.DaoImpl.MailServerDao.GetById(idServer);
            }
            return s;
        }

        private List<Folder> GetMailFolders(OracleDataReader r)
        {
            List<Folder> ml = null;
            string id = r.GetDecimal("ID_SENDER").ToString();
            int idUser = 0;
            int.TryParse(id, out idUser);
            if (idUser != default(decimal))
            {
                ml = GetAllFoldersByAccount(idUser);  
            }
            return ml;
        }

        private List<ActiveUp.Net.Common.DeltaExt.Action> GetActionsFolder(OracleDataReader r,int IdUser)
        {
            List<ActiveUp.Net.Common.DeltaExt.Action> a = null;
            string id = r.GetDecimal("ID").ToString();
            string tipo = r.GetString("TIPOFOLDER");
            int idFolder = 0;
            int.TryParse(id, out idFolder);
            if (idFolder != default(decimal))
            {
                a = GetAllActionsByFolder(idFolder,tipo,IdUser);
            }
            return a;

        }

      
        #endregion
    }
}
