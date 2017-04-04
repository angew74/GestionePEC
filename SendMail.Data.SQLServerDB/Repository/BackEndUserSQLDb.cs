using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ActiveUp.Net.Mail.DeltaExt;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class BackEndUserSQLDb : IBackendUserDao
    {
        private static readonly ILog log = LogManager.GetLogger("BackEndUserSQLDb");

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
                using (var dbcontext = new FAXPECContext())
                {
                    DateTime di;
                    DateTime.TryParse(datainizio, out di);
                    DateTime df;
                    DateTime.TryParse(datafine, out df);
                    tot = dbcontext.MAIL_INBOX.Where(x => x.FOLDERID != 3 && x.MAIL_ACCOUNT.ToUpper() == account.ToUpper() && x.DATA_RICEZIONE >= di && x.DATA_RICEZIONE <= df).Count().ToString();
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti statistica per casella email e periodo: " + account.ToString() + " " + datainizio + "-" + datafine + " ERR_080 Dettagli Errore: " + e.Message,
                        "ERR_080", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
            }
            return tot;
        }

        public List<UserResultItem> GetStatsInBox(string account, string utente, string datainizio, string datafine)
        {
            List<UserResultItem> list = new List<UserResultItem>();
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT USER_MAIL AS ACCOUNT, UPPER(USER_ID) AS UTE,")
                    .Append(" COUNT(*) AS TOT  FROM ")
                    .Append(" LOG_ACTIONS WHERE ")
                    .Append(" TRUNC(LOG_DATE,'DD')")
                    .Append("  BETWEEN TO_DATE(" + datainizio + "','DD/MM/YYYY') ")
                    .Append("  AND TO_DATE('" + datainizio + "','DD/MM/YYYY')  ")
                    .Append("  AND (UPPER(USER_MAIL)= '" + account.ToUpper() + "') ")
                    .Append(" AND LOG_CODE IN ('CRB_MOV','CRB_DEL','CRB_ARK','CRB_RIPK','CRB_RIPC')");
                    if (utente.ToUpper().Trim() != "TUTTI")
                    { sb.Append(" AND UPPER(USER_ID)='" + utente.ToUpper() + "' "); }
                    sb.Append("  GROUP BY UPPER(USER_ID),USER_MAIL ");
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        oCmd.CommandText = sb.ToString();
                        using (var r = oCmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                list.Add(AutoMapperConfiguration.MapToUserResult(r));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti statistica per casella email e periodo: " + account.ToString() + " " + datainizio + "-" + datafine + " E079 Dettagli Errore: " + e.Message,
                        "ERR_079", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
            }
            return list;
        }

        public BackendUser GetByUserName(String UserName)
        {
            BackendUser backendUser = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    MAIL_USERS_BACKEND d = dbcontext.MAIL_USERS_BACKEND.Where(x => x.USER_NAME.ToUpper() == UserName.ToUpper()).First();
                    backendUser = AutoMapperConfiguration.FromMailUsersBackendToModel(d);
                    if (backendUser != null && backendUser.UserId >= 0)
                    {
                        var musb = dbcontext.MAIL_USERS_SENDER_BACKEND.Where(x => x.REF_ID_USER == d.ID_USER).ToList();
                        backendUser.MappedMails = (List<BackEndUserMailUserMapping>)this.GetMailUserByUserId(backendUser.UserId, backendUser.UserRole);
                    }
                }
            }
            catch (Exception e)
            {

                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione del profilo utente abilitati per username : " + UserName.ToString() + " E078 Dettagli Errore: " + e.Message,
                        "ERR_078", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                backendUser = null;
            }

            return backendUser;
        }

        public List<BackEndUserMailUserMapping> GetMailUserByUserId(long userId, int userRole)
        {
            List<BackEndUserMailUserMapping> listStart = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    if (userRole < 2)
                    {
                        var v = dbcontext.MAIL_USERS_SENDER_BACKEND.Where(x => x.REF_ID_USER == userId).ToList();
                        if (v.Count > 0)
                        {
                            listStart = new List<BackEndUserMailUserMapping>();
                            BackEndUserMailUserMapping b = new BackEndUserMailUserMapping();
                            foreach (MAIL_USERS_SENDER_BACKEND a in v)
                            {
                                b.MailSenderId = (long)a.REF_ID_SENDER;
                                b.MailAccessLevel = int.Parse(a.ROLE.ToString());                                
                                MailAccountSQLDb mailaccount = new MailAccountSQLDb();
                                MailUser mu = mailaccount.GetById(b.MailSenderId);
                                listStart.Add(new BackEndUserMailUserMapping(mu, b.MailAccessLevel));
                            }
                        }
                    }
                    else
                    {
                        var f = dbcontext.MAIL_SENDERS.OrderBy(x => x.ID_SENDER).ToList();
                        if (f.Count > 0)
                        {
                            listStart = new List<BackEndUserMailUserMapping>();
                            BackEndUserMailUserMapping b = new BackEndUserMailUserMapping();
                            foreach (MAIL_SENDERS s in f)
                            {
                                b.MailSenderId = s.ID_SENDER;
                                b.MailAccessLevel = 1;
                                MailAccountSQLDb mailaccount = new MailAccountSQLDb();
                                MailUser mu = mailaccount.GetById(b.MailSenderId);
                                listStart.Add(new BackEndUserMailUserMapping(mu, b.MailAccessLevel));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista emails abilitate per utente: " + userId.ToString() + " E077 Dettagli Errore: " + e.Message,
                        "ERR_077", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                listStart = null;
            }

            return listStart;
        }

        public List<BackendUser> GetAllDipartimentiByMailAdmin(string UserName)
        {
            List<BackendUser> listaDipartimenti = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var list = dbcontext.MAIL_USERS_BACKEND.Where(x => x.USER_NAME.ToUpper() == UserName.Trim().ToUpper()).OrderBy(x => x.DEPARTMENT).ToList();
                    if (list.Count > 0)
                    {
                        listaDipartimenti = new List<BackendUser>();
                        foreach (MAIL_USERS_BACKEND m in list)
                        {
                            listaDipartimenti.Add(AutoMapperConfiguration.MapToDepartmentModel(m));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista dipartimenti per utente : " + UserName.ToString() + " E076 Dettagli Errore: " + e.Message,
                        "ERR_076", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                listaDipartimenti = null;
            }
            return listaDipartimenti;

        }

        public List<BackendUser> GetAllDipartimenti()
        {
            List<BackendUser> listaDipartimenti = null;

            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var v = dbcontext.MAIL_USERS_BACKEND.ToList();
                    foreach (MAIL_USERS_BACKEND a in v)
                    {
                        listaDipartimenti.Add(AutoMapperConfiguration.MapToDepartmentModel(a));
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista dei dipartimenti : " + " E075 Dettagli Errore: " + e.Message,
                        "ERR_075", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                listaDipartimenti = null;
            }

            return listaDipartimenti;
        }

        public List<BackendUser> GetDipendentiDipartimentoNONAbilitati(String dipartimento, Decimal idSender)
        {
            List<BackendUser> listaDipendenti = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
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
                                         "WHERE MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER = " + idSender + ") " +
                                         "AND MAIL_USERS_BACKEND.DEPARTMENT = '" + dipartimento + "'";
                        using (var r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                listaDipendenti = new List<BackendUser>();
                                while (r.Read())
                                {
                                    BackendUser bUser = AutoMapperConfiguration.MapToBackendUser(r);
                                    listaDipendenti.Add(bUser);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti non abilitati per email e dipartimento : " + idSender.ToString() + " " + dipartimento + " E074 Dettagli Errore: " + e.Message,
                        "ERR_074", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                }
                listaDipendenti = null;
            }

            return listaDipendenti;
        }

        public List<BackendUser> GetDipartimentiByAdmin(string UserName)
        {
            List<BackendUser> listaDipartimenti = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        oCmd.CommandText = "SELECT DISTINCT MAIL_USERS_ADMIN_BACKEND.DEPARTMENT " +
                                           "FROM MAIL_USERS_ADMIN_BACKEND, MAIL_USERS_BACKEND  " +
                                           " WHERE REF_ID_USER=ID_USER  " +
                                           " AND UPPER(MAIL_USERS_BACKEND.USER_NAME)='" + UserName.ToUpper().Trim() + "'" +
                                           " ORDER BY MAIL_USERS_ADMIN_BACKEND.DEPARTMENT";
                        using (var r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                listaDipartimenti = new List<BackendUser>();
                                while (r.Read())
                                {
                                    listaDipartimenti.Add(AutoMapperConfiguration.MapToDepartment(r));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e0)
            {
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
                using (var dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
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
                                           "AND MAIL_USERS_SENDER_BACKEND.REF_ID_SENDER = " + idSender +
                                           " ORDER BY MAIL_USERS_SENDER_BACKEND.DATA_INSERIMENTO DESC";
                        using (var r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                listaDipendenti = new List<BackendUser>();
                                while (r.Read())
                                {
                                    BackendUser bUser = AutoMapperConfiguration.MapToBackendUser(r);

                                    if (bUser != null && bUser.UserId >= 0)
                                    {
                                        BackEndUserMailUserMapping b = new BackEndUserMailUserMapping();
                                        b.MailSenderId = (long)r.GetValue("ID_SENDER");
                                        b.MailAccessLevel = int.Parse(r.GetValue("ROLE_MAIL").ToString());
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
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella creazione lista utenti abilitati per email: " + idSender.ToString() + " E072 Dettagli Errore: " + e.Message,
                        "ERR_072", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                }
                listaDipendenti = null;
            }

            return listaDipendenti;
        }

        public void InsertAbilitazioneEmail(Decimal UserId, Decimal idSender, int role)
        {
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    MAIL_USERS_SENDER_BACKEND m = new MAIL_USERS_SENDER_BACKEND()
                    {
                        REF_ID_USER = int.Parse(UserId.ToString()),
                        REF_ID_SENDER = int.Parse(idSender.ToString()),
                        ROLE = role.ToString(),
                        DATA_INSERIMENTO = System.DateTime.Now
                    };
                    dbcontext.MAIL_USERS_SENDER_BACKEND.Add(m);
                    dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nell'inserimento dell'utente all'abilitazione di una email: " + idSender.ToString() + " " + UserId.ToString() + " E073 Dettagli Errore: " + ex.Message,
                        "ERR_073", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);

                    throw mEx;
                }
                else throw;
            }
        }

        public void RemoveAbilitazioneEmail(Decimal UserId, Decimal idSender)
        {
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var m = dbcontext.MAIL_USERS_SENDER_BACKEND.Where(x => x.REF_ID_USER == UserId && x.REF_ID_SENDER == idSender).First();
                    if (m != null)
                    {
                        dbcontext.MAIL_USERS_SENDER_BACKEND.Remove(m);
                        dbcontext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nella rimozione delle abilitazioni per utente " + UserId.ToString() + " E070 Dettagli Errore: " + ex.Message,
                        "ERR_070", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                    throw mEx;
                }
                else throw;
            }
        }

        public void UpdateAbilitazioneEmail(decimal userId, decimal idsender, int level)
        {
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var m = dbcontext.MAIL_USERS_SENDER_BACKEND.Where(x => x.REF_ID_SENDER == idsender && x.REF_ID_USER == userId).First();
                    if (m != null)
                    {
                        m.ROLE = level.ToString();
                        dbcontext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nell'aggiornamento delle abilitazioni per utente " + userId.ToString() + " E071 Dettagli Errore: " + ex.Message,
                        "ERR_071", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                    throw mEx;
                }
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
