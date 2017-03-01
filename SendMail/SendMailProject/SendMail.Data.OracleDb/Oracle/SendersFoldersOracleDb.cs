using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.OracleDb;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using SendMail.Data.Utilities;
using Oracle.DataAccess.Client;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;


namespace SendMailApp.OracleCore.Oracle
{
    class SendersFoldersOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, ISendersFoldersDao
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SendersFoldersOracleDb));
        #region "Command Strings"

        private const string cmdSelectSendersFoldersByUserName = "SELECT MAIL_USERS_BACKEND.ID_USER, " +
                                                        "MAIL_USERS_BACKEND.USER_NAME, " +
                                                        "MAIL_USERS_BACKEND.DEPARTMENT, " +
                                                        "MAIL_USERS_BACKEND.MUNICIPIO, " +
                                                        "MAIL_USERS_BACKEND.DOMAIN, " +
                                                        "MAIL_USERS_BACKEND.COGNOME, " +
                                                        "MAIL_USERS_BACKEND.NOME " +
                                                        "FROM MAIL_USERS_BACKEND WHERE USER_NAME = :USER_NAME";

        private const string cmdSelectMailUserListById = "SELECT MAIL_USERS_SENDER_BACKEND.REF_ID_USER, " +
                                                         "MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER ID_SENDER, " +
                                                         "MAIL_USERS_SENDER_BACKEND.ROLE " +
                                                         "FROM MAIL_USERS_SENDER_BACKEND " +
                                                         "WHERE REF_ID_USER = :REF_ID_USER";

        private const string cmdSelectAllDipartimenti = "SELECT DISTINCT MAIL_USERS_BACKEND.DEPARTMENT " +
                                                         "FROM MAIL_USERS_BACKEND " +
                                                         "ORDER BY MAIL_USERS_BACKEND.DEPARTMENT";
        #endregion

        #region "Class Methods"


        public SendersFoldersOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            context = daoContext;
            //todo.. RIVEDERE
            //apro la cn se non è già aperta.
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }
        private OracleSessionManager context;

        #endregion

        #region IDao<SendersFolders,long> Membri di

        public ICollection<SendersFolders> GetAll()
        {
            throw new NotImplementedException();
        }

        public SendersFolders GetById(long id)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFolder"></param>
        /// <param name="idSender"></param>
        public void DeleteAbilitazioneFolder(int idNome, int idSender)
        {
            try
            {
                using (OracleCommand oCmd0 = base.CurrentConnection.CreateCommand())
                {
                    oCmd0.CommandText = "SELECT ID FROM FOLDERS WHERE IDNOME = :IDNOME ";
                    oCmd0.Parameters.Add("IDNOME", OracleDbType.Int32, idNome, System.Data.ParameterDirection.Input);
                    OracleDataReader r = oCmd0.ExecuteReader();

                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                            {
                                oCmd.CommandText = "DELETE FROM folders_senders WHERE idfolder = :IDFOLDER AND idsender = :IDSENDER";
                                oCmd.Parameters.Add("IDFOLDER", Convert.ToInt32(r.GetValue(0)));
                                oCmd.Parameters.Add("IDSENDER", idSender);

                                oCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    r.Close();
                }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                    throw ex;
                ManagedException mEx = new ManagedException(ex.Message, "SND_ORA002", string.Empty, string.Empty, ex);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                log.Error(er);
                throw mEx;
            }
        }

        /// <summary>
        /// Inserisce un record nel tabella FOLDERS_SENDERS
        /// </summary>
        /// <param name="idFolder"></param>
        /// <param name="idSender"></param>
        public int InsertAbilitazioneFolder(int idNome, int idSender, int system)
        {
            try
            {
                using (OracleCommand oCmd0 = base.CurrentConnection.CreateCommand())
                {
                    oCmd0.CommandText = "SELECT ID FROM FOLDERS WHERE IDNOME = :IDNOME ";
                    oCmd0.Parameters.Add("IDNOME", OracleDbType.Int32, idNome,System.Data.ParameterDirection.Input);
                    OracleDataReader r = oCmd0.ExecuteReader();

                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                            {
                                oCmd.CommandText = "INSERT INTO FOLDERS_SENDERS (IDFOLDER,IDSENDER) " +
                                                        "VALUES (:IDFOLDER,:IDSENDER)";
                                oCmd.Parameters.Add("IDFOLDER", Convert.ToInt32(r.GetValue(0)));
                                oCmd.Parameters.Add("IDSENDER", idSender);
                                oCmd.ExecuteNonQuery();
                            }
                        }
                    }
                    r.Close();
                    return 0;
                 }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() == typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "SND_ORA003", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                }
                return -1;
            }
        }

        /// <summary>
        /// Prende la lista di tutte le cartelle non attive per la mail selezionata
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public List<SendersFolders> GetFoldersNONAbilitati(string mail)
        {
            List<SendersFolders> listaCartelle = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT DISTINCT m.ID_SENDER, f.NOME, m.MAIL, f.IDNOME, f.SYSTEM " +
                                            "FROM MAIL_SENDERS m,folders f,folders_senders fs " +
                                            "WHERE m.mail = :MAIL1 " +
                                            "MINUS " +
                                            "SELECT DISTINCT m.ID_SENDER, f.NOME, m.MAIL, f.IDNOME, f.SYSTEM " +
                                            "FROM MAIL_SENDERS m,folders f,folders_senders fs " +
                                            "WHERE m.mail = :MAIL2 " +
                                            "AND m.ID_SENDER = fs.IDSENDER " +
                                            "AND f.ID = fs.IDFOLDER";
                    oCmd.Parameters.Add("MAIL1", mail);
                    oCmd.Parameters.Add("MAIL2", mail);
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaCartelle = new List<SendersFolders>();
                            while (r.Read())
                            {
                                SendersFolders sFold = DaoOracleDbHelper.MapToSendersFolders(r);
                                listaCartelle.Add(sFold);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                listaCartelle = null;
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "SND_ORA004", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                }
            }
            return listaCartelle;

        }

        /// <summary>
        /// Prende la lista di tutte le cartelle attive per la mail selezionata
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public List<SendersFolders> GetFoldersAbilitati(string mail)
        {
            List<SendersFolders> listaCartelle = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT DISTINCT m.ID_SENDER, f.NOME, m.MAIL, f.IDNOME, f.SYSTEM " +
                                            "FROM MAIL_SENDERS m,folders f,folders_senders fs " +
                                            "WHERE m.mail = :MAIL  " +
                                            "AND m.ID_SENDER = fs.IDSENDER " +
                                            "AND f.ID = fs.IDFOLDER";
                    oCmd.Parameters.Add("MAIL", mail);
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaCartelle = new List<SendersFolders>();
                            while (r.Read())
                            {
                                SendersFolders sFold = DaoOracleDbHelper.MapToSendersFolders(r);
                                listaCartelle.Add(sFold);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                listaCartelle = null;
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "SND_ORA005", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                }
            }
            return listaCartelle;
            
        }

        public void Insert(SendersFolders entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SendersFolders entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public int Save(SendersFolders entity)
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
    }
}
