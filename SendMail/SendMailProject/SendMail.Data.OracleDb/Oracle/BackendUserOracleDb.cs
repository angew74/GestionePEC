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
using SendMail.Model.Wrappers;
using log4net;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;


namespace SendMailApp.OracleCore.Oracle
{
    class BackendUserOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IBackendUserDao
    {
        private static readonly ILog log = LogManager.GetLogger("BackendUserOracleDb");

        #region "Command Strings"

        private const string cmdSelectBackendUserByUserName = "SELECT MAIL_USERS_BACKEND.ID_USER, " +
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


        public BackendUserOracleDb(OracleSessionManager daoContext)
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

        #region IDao<BackendUser,long> Membri di

        public ICollection<BackendUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public BackendUser GetById(long id)
        {
            throw new NotImplementedException();
        }

        public string GetTotalePeriodoAccount(string account, string datainizio, string datafine)
        {
            string tot = "0";
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT COUNT(*) AS TOT")
                        .Append(" FROM MAIL_INBOX ")
                        .Append(" WHERE ");
                    sb.Append(" FOLDERID <> 3 AND TRUNC(DATA_RICEZIONE,'DD') ")
                    .Append(" BETWEEN TO_DATE(:V_DATA_INIZIO,'DD/MM/YYYY') ")
                    .Append(" AND TO_DATE(:V_DATA_FINE,'DD/MM/YYYY') ")
                    .Append(" AND (UPPER(MAIL_ACCOUNT)=:V_MAIL_ACCOUNT) ");                   
                    oCmd.CommandText = sb.ToString();                   
                    oCmd.Parameters.Add("V_MAIL_ACCOUNT", account.ToUpper());
                    oCmd.Parameters.Add("V_DATA_INIZIO", datainizio);
                    oCmd.Parameters.Add("V_DATA_FINE", datafine);
                    oCmd.BindByName = true;
                    tot = oCmd.ExecuteScalar().ToString();                    
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti statistica per casella email e periodo: " + account.ToString() + " " + datainizio + "-" + datafine + " ERR_080 Dettagli Errore: " + e.Message,
                        "ERR_080", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista utenti statistica per casella email e periodo: " + account.ToString() + " " + datainizio + "-" + datafine + " E080 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_080";             
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = account.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
            }
            return tot;
        }

        public List<UserResultItem> GetStatsInBox(string account, string utente, string datainizio, string datafine)
        {
            List<UserResultItem> list = new List<UserResultItem>();
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT USER_MAIL AS ACCOUNT, UPPER(USER_ID) AS UTE,")
                    .Append(" COUNT(*) AS TOT  FROM ")
                    .Append(" LOG_ACTIONS WHERE ")
                    .Append(" TRUNC(LOG_DATE,'DD')")
                    .Append("  BETWEEN TO_DATE(:V_DATA_INIZIO,'DD/MM/YYYY') ")
                    .Append("  AND TO_DATE(:V_DATA_FINE,'DD/MM/YYYY')  ")
                    .Append("  AND (UPPER(USER_MAIL)=:V_MAIL_ACCOUNT) ")
                    .Append(" AND LOG_CODE IN ('CRB_MOV','CRB_DEL','CRB_ARK','CRB_RIPK','CRB_RIPC')");
                    if(utente.ToUpper().Trim() != "TUTTI")
                    { sb.Append(" AND UPPER(USER_ID)=:V_UTE_OPE "); }
                    sb.Append("  GROUP BY UPPER(USER_ID),USER_MAIL ");
                    oCmd.CommandText = sb.ToString();
                    if (utente.ToUpper().Trim() != "TUTTI")
                    { oCmd.Parameters.Add("V_UTE_OPE", utente.ToUpper()); }
                    oCmd.Parameters.Add("V_MAIL_ACCOUNT", account.ToUpper());
                    oCmd.Parameters.Add("V_DATA_INIZIO", datainizio);
                    oCmd.Parameters.Add("V_DATA_FINE", datafine);
                    oCmd.BindByName = true;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(DaoOracleDbHelper.MapToUserResult(r));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti statistica per casella email e periodo: " + account.ToString() + " " + datainizio + "-" + datafine + " E079 Dettagli Errore: " + e.Message,
                        "ERR_079", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista utenti statistica per casella email e periodo: " + account.ToString() + " " + datainizio + "-" + datafine +  " E079 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_079";             
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = account.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
            }
            return list;
        }

        public BackendUser GetByUserName(String UserName)
        {
            BackendUser backendUser = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT MAIL_USERS_BACKEND.ID_USER, " +
                                       "MAIL_USERS_BACKEND.USER_NAME, " +
                                       "MAIL_USERS_BACKEND.DEPARTMENT, " +
                                       "MAIL_USERS_BACKEND.MUNICIPIO, " +
                                       "MAIL_USERS_BACKEND.DOMAIN, " +
                                       "MAIL_USERS_BACKEND.COGNOME, " +
                                       "MAIL_USERS_BACKEND.NOME, " +
                                       "MAIL_USERS_BACKEND.ROLE ROLE_USER, " +    
                                       " '' AS ROLE_MAIL " + 
                                       "FROM MAIL_USERS_BACKEND WHERE UPPER(USER_NAME) = :USER_NAME";

                    oCmd.Parameters.Add("USER_NAME", UserName.ToUpper());
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            backendUser = DaoOracleDbHelper.MapToBackendUser(r);
                        }
                    }
                }

                if (backendUser != null && backendUser.UserId >= 0)
                {
                    backendUser.MappedMails = (List<BackEndUserMailUserMapping>)this.GetMailUserByUserId(backendUser.UserId, backendUser.UserRole);
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione del profilo utente abilitati per username : " + UserName.ToString() + " E078 Dettagli Errore: " + e.Message,
                        "ERR_078", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione del profilo utente abilitati per username : " + UserName.ToString() + " E078 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_078";               
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = UserName.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                backendUser = null;
            }

            return backendUser;
        }

        public List<BackEndUserMailUserMapping> GetMailUserByUserId(long userId, int userRole)
        {
            List<BackEndUserMailUserMapping> listStart = null;

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    if (userRole < 2)
                    {
                        oCmd.CommandText = "SELECT MAIL_USERS_SENDER_BACKEND.REF_ID_USER, " +
                                           "MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER ID_SENDER, " +
                                           "MAIL_USERS_SENDER_BACKEND.ROLE AS ROLE_MAIL " +
                                           "FROM MAIL_USERS_SENDER_BACKEND " +
                                           "WHERE REF_ID_USER = :REF_ID_USER";
                        oCmd.Parameters.Add("REF_ID_USER", userId);
                    }
                    else
                    {
                        oCmd.CommandText = "SELECT DISTINCT ID_SENDER, 1 AS \"ROLE_MAIL\"" +
                                          " FROM MAIL_SENDERS" +
                                          " ORDER BY 1";
                    }

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        listStart = new List<BackEndUserMailUserMapping>();
                        while (r.Read())
                        {
                            BackEndUserMailUserMapping b = DaoOracleDbHelper.MapToBackEndUserMailUserMapping(r);
                            MailUser mu = this.Context.DaoImpl.MailAccountDao.GetById(b.MailSenderId);
                            listStart.Add(new BackEndUserMailUserMapping(mu, b.MailAccessLevel));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista emails abilitate per utente: " + userId.ToString() + " E077 Dettagli Errore: " + e.Message,
                        "ERR_077", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista emails abilitate per utente: " + userId.ToString() + " E077 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_077";           
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = userId.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                listStart = null;
            }

            return listStart;
        }

        public List<BackendUser> GetAllDipartimentiByMailAdmin(string UserName)
        {
            List<BackendUser> listaDipartimenti = null;

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT DISTINCT MAIL_USERS_BACKEND.DEPARTMENT FROM MAIL_USERS_BACKEND " +
                                       " WHERE UPPER(USER_NAME)='" + UserName.Trim().ToUpper() + "' ORDER BY MAIL_USERS_BACKEND.DEPARTMENT";

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaDipartimenti = new List<BackendUser>();
                            while (r.Read())
                            {
                                listaDipartimenti.Add(DaoOracleDbHelper.MapToDepartment(r));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista dipartimenti per utente : " + UserName.ToString() + " E076 Dettagli Errore: " + e.Message,
                        "ERR_076", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista dipartimenti per utente : " + UserName.ToString() + " E076 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_076";           
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = UserName.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                listaDipartimenti = null;
            }

            return listaDipartimenti;

        }

        public List<BackendUser> GetAllDipartimenti()
        {
            List<BackendUser> listaDipartimenti = null;

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT DISTINCT MAIL_USERS_BACKEND.DEPARTMENT " +
                                       "FROM MAIL_USERS_BACKEND " +
                                       "ORDER BY MAIL_USERS_BACKEND.DEPARTMENT";

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaDipartimenti = new List<BackendUser>();
                            while (r.Read())
                            {
                                listaDipartimenti.Add(DaoOracleDbHelper.MapToDepartment(r));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista dei dipartimenti : " + " E075 Dettagli Errore: " + e.Message,
                        "ERR_075", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista dei dipartimenti : " + " E075 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_075";             
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = "";
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                listaDipartimenti = null;
            }

            return listaDipartimenti;
        }

        public List<BackendUser> GetDipendentiDipartimentoNONAbilitati(String dipartimento, Decimal idSender)
        {
            List<BackendUser> listaDipendenti = null;

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT DISTINCT MAIL_USERS_BACKEND.ID_USER, " +
                                       "MAIL_USERS_BACKEND.USER_NAME, " +
                                       "MAIL_USERS_BACKEND.COGNOME, " +
                                       "MAIL_USERS_BACKEND.NOME, " +
                                       "MAIL_USERS_BACKEND.DEPARTMENT, " +
                                       "MAIL_USERS_BACKEND.MUNICIPIO, " +
                                       "MAIL_USERS_BACKEND.DOMAIN, " +
                                       "0 as  ROLE_USER, " +
                                       "0 AS ROLE_MAIL" +
                                       " FROM MAIL_USERS_BACKEND " +
                                       " LEFT OUTER JOIN MAIL_USERS_SENDER_BACKEND ON ID_USER=REF_ID_USER " +
                                       " WHERE MAIL_USERS_BACKEND.ID_USER NOT IN " +
                                       " (SELECT MAIL_USERS_SENDER_BACKEND.REF_ID_USER FROM MAIL_USERS_SENDER_BACKEND " +
                                       "WHERE MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER = :ID_SENDER) " +
                                       "AND MAIL_USERS_BACKEND.DEPARTMENT = :DEPARTMENT";

                    oCmd.Parameters.Add("ID_SENDER", idSender);
                    oCmd.Parameters.Add("DEPARTMENT", dipartimento);
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaDipendenti = new List<BackendUser>();
                            while (r.Read())
                            {
                                BackendUser bUser = DaoOracleDbHelper.MapToBackendUser(r);
                                listaDipendenti.Add(bUser);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti non abilitati per email e dipartimento : " + idSender.ToString() + " " + dipartimento + " E074 Dettagli Errore: " + e.Message,
                        "ERR_074", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista utenti non abilitati per email e dipartimento : " + idSender.ToString() + " " + dipartimento + " E074 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_074";               
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = idSender.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                listaDipendenti = null;
            }

            return listaDipendenti;
        }

        public List<BackendUser> GetDipartimentiByAdmin(string UserName)
        {
            List<BackendUser> listaDipartimenti = null;

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT DISTINCT MAIL_USERS_ADMIN_BACKEND.DEPARTMENT " +
                                       "FROM MAIL_USERS_ADMIN_BACKEND, MAIL_USERS_BACKEND  " +
                                       " WHERE REF_ID_USER=ID_USER  " +
                                       " AND UPPER(MAIL_USERS_BACKEND.USER_NAME)='" + UserName.ToUpper().Trim() + "'" +
                                       " ORDER BY MAIL_USERS_ADMIN_BACKEND.DEPARTMENT";

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaDipartimenti = new List<BackendUser>();
                            while (r.Read())
                            {
                                listaDipartimenti.Add(DaoOracleDbHelper.MapToDepartment(r));
                            }
                        }
                    }
                }
            }
            catch (Exception e0)
            {
                //Allineamento log - Ciro
                if (e0.GetType() != typeof(ManagedException))
                {
                    ManagedException me = new ManagedException(e0.Message, "ORA_ERR003", string.Empty, string.Empty, e0);
                    ErrorLogInfo err = new ErrorLogInfo(me);
                    log.Error(err);
                }
                listaDipartimenti = null;
            }

            return listaDipartimenti;
        }

        public List<BackendUser> GetDipendentiDipartimentoAbilitati(Decimal idSender)
        {
            List<BackendUser> listaDipendenti = null;

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT MAIL_USERS_BACKEND.ID_USER, " +
                                       "MAIL_USERS_BACKEND.USER_NAME, " +
                                       "MAIL_USERS_BACKEND.COGNOME, " +
                                       "MAIL_USERS_BACKEND.NOME, " +
                                       "MAIL_USERS_BACKEND.DEPARTMENT, " +
                                       "MAIL_USERS_BACKEND.MUNICIPIO, " +
                                       "MAIL_USERS_BACKEND.DOMAIN, " +
                                       "MAIL_USERS_BACKEND.ROLE ROLE_USER, " +
                                       "MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER ID_SENDER, " +
                                       "MAIL_USERS_SENDER_BACKEND.ROLE AS ROLE_MAIL " +                                     
                                       "FROM MAIL_USERS_BACKEND, MAIL_USERS_SENDER_BACKEND " +
                                       "WHERE MAIL_USERS_BACKEND.ID_USER = MAIL_USERS_SENDER_BACKEND.REF_ID_USER " +
                                       "AND MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER = :ID_SENDER " +
                                       "ORDER BY MAIL_USERS_SENDER_BACKEND.DATA_INSERIMENTO DESC";

                    oCmd.Parameters.Add("ID_SENDER", idSender);
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaDipendenti = new List<BackendUser>();
                            while (r.Read())
                            {
                                BackendUser bUser = DaoOracleDbHelper.MapToBackendUser(r);

                                if (bUser != null && bUser.UserId >= 0)
                                {
                                    BackEndUserMailUserMapping b = DaoOracleDbHelper.MapToBackEndUserMailUserMapping(r);
                                    List<BackEndUserMailUserMapping> bList = new List<BackEndUserMailUserMapping>();
                                    bList.Add(b);
                                    bUser.MappedMails = bList;
                                }
                                listaDipendenti.Add(bUser);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti abilitati per email: " + idSender.ToString() + " E072 Dettagli Errore: " + e.Message,
                        "ERR_072", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella creazione lista utenti abilitati per email: " + idSender.ToString() + " E072 Dettagli Errore: " + e.Message;
                //error.logCode = "ERR_072";             
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = idSender.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                listaDipendenti = null;
            }

            return listaDipendenti;
        }

        public void InsertAbilitazioneEmail(Decimal UserId, Decimal idSender, int role)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "INSERT INTO MAIL_USERS_SENDER_BACKEND " +
                                       "(REF_ID_USER, REF_ID_SENDER, ROLE, DATA_INSERIMENTO) " +
                                       "VALUES (:REF_ID_USER, :REF_ID_SENDER, " + role.ToString() + ", SYSDATE)";

                    oCmd.Parameters.Add("REF_ID_USER", UserId);
                    oCmd.Parameters.Add("REF_ID_SENDER", idSender);
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nell'inserimento dell'utente all'abilitazione di una email: " + idSender.ToString() + " " + UserId.ToString() + " E073 Dettagli Errore: " + ex.Message,
                        "ERR_073", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);

                    throw mEx;
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nell'inserimento dell'utente all'abilitazione di una email: " + idSender.ToString() + " " + UserId.ToString() + " E073 Dettagli Errore: " + ex.Message;
                //error.logCode = "ERR_073";            
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = idSender.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                else throw;
            }
        }

        public void RemoveAbilitazioneEmail(Decimal UserId, Decimal idSender)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "DELETE FROM MAIL_USERS_SENDER_BACKEND " +
                                       "WHERE MAIL_USERS_SENDER_BACKEND.REF_ID_USER = :REF_ID_USER " +
                                       "AND MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER = :REF_ID_SENDER";

                    oCmd.Parameters.Add("REF_ID_USER", UserId);
                    oCmd.Parameters.Add("REF_ID_SENDER", idSender);
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella rimozione delle abilitazioni per utente " + UserId.ToString() + " E070 Dettagli Errore: " + ex.Message,
                        "ERR_070", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
      
                    log.Error(err);
                    throw mEx;
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nella rimozione delle abilitazioni per utente " + UserId.ToString() + " E070 Dettagli Errore: " + ex.Message;
                //error.logCode = "ERR_070";            
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = UserId.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                 else throw;
            }
        }

        public void UpdateAbilitazioneEmail(decimal userId, decimal idsender, int level)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "UPDATE MAIL_USERS_SENDER_BACKEND SET ROLE=:REF_ROLE " +
                                       "WHERE MAIL_USERS_SENDER_BACKEND.REF_ID_USER = :REF_ID_USER " +
                                       "AND MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER = :REF_ID_SENDER";

                    oCmd.Parameters.Add("REF_ID_USER", userId);
                    oCmd.Parameters.Add("REF_ID_SENDER", idsender);
                    oCmd.Parameters.Add("REF_ROLE", level);
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nell'aggiornamento delle abilitazioni per utente " + userId.ToString() + " E071 Dettagli Errore: " + ex.Message,
                        "ERR_071", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
   
                    log.Error(err);
                    throw mEx;
                }
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nell'aggiornamento delle abilitazioni per utente " + userId.ToString() + " E071 Dettagli Errore: " + ex.Message;
                //error.logCode = "ERR_071";              
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = userId.ToString();
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                else throw;
            }
        }

        public void Insert(BackendUser entity)
        {
            throw new NotImplementedException();
        }

        public void Update(BackendUser entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public int Save(BackendUser entity)
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
