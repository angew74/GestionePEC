using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.SQLServerDB.Mapping;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class MailMessageDaoSQLDb : IMailMessageDao
    {

        private static readonly ILog _log = LogManager.GetLogger("MailMessageDaoSQLDb");

        public IDictionary<ActiveUp.Net.Common.DeltaExt.MailStatusServer, List<string>> GetAllUIDsByAccount(string account, List<ActiveUp.Net.Common.DeltaExt.MailStatusServer> serverStatus)
        {
            Dictionary<MailStatusServer, List<string>> list = null;
            string[] allStatus = serverStatus.Select(ss => ((int)ss).ToString()).ToArray();
            using (FAXPECContext entities = new FAXPECContext())
            {
                var mailServer = (from m in entities.MAIL_INBOX
                                  where m.MAIL_ACCOUNT.ToUpper() == account &&
                                  allStatus.Contains(m.STATUS_MAIL)
                                  select new { mailServerId = m.MAIL_SERVER_ID, statusServer = m.STATUS_SERVER });
                try
                {
                    if (mailServer.Count() > 0)
                    {
                        list = new Dictionary<MailStatusServer, List<string>>();
                        foreach (var v in mailServer)
                        {
                            MailStatusServer stServer = MailStatusServer.UNKNOWN;
                            if (v.statusServer != null)
                            {
                                int val = -1;
                                if (int.TryParse(v.statusServer, out val))
                                {
                                    if (Enum.IsDefined(typeof(MailStatusServer), val))
                                        stServer = (MailStatusServer)val;
                                }
                            }
                            if (!list.ContainsKey(stServer))
                                list.Add(stServer, new List<string>());
                            list[stServer].Add(v.mailServerId);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo"
                        + "GetAllUIDsByAccount(string account, List<MailStatusServer> serverStatus). Dettaglio: " + e.Message,
                            "E001", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                        list = null;
                        throw mEx;
                    }
                    list = null;
                    throw e;
                }
            }

            return list;
        }

        public void Insert(ActiveUp.Net.Mail.DeltaExt.MailUser user, ActiveUp.Net.Mail.Message m)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var dbTransaction = dbcontext.Database.BeginTransaction())
                {

                    MAIL_INBOX c = AutoMapperConfiguration.MapToMailInBoxDto(user, m);
                    int rows = 0;
                    try
                    {
                        
                        dbcontext.MAIL_INBOX.Add(c);
                        dbcontext.SaveChanges();
                        var mailinboxnew = dbcontext.MAIL_INBOX.Where(z => z.MAIL_SERVER_ID == c.MAIL_SERVER_ID).First();
                        int newid =(int) mailinboxnew.ID_MAIL;
                        MailStatus newStatus = MailStatus.SCARICATA;
                        MailStatus oldStatus = MailStatus.UNKNOWN;
                        if (String.IsNullOrEmpty(m.MessageId))
                        { newStatus = MailStatus.SCARICATA_INCOMPLETA; }
                        string os = ((int)oldStatus).ToString();
                        string ns = ((int)newStatus).ToString();
                        MAIL_INBOX_FLUSSO f = AutoMapperConfiguration.MapToMailInboxFlussoDto(newid, os, ns, m.ReceivedDate, "SYSTEM");
                        dbcontext.MAIL_INBOX_FLUSSO.Add(f);
                        rows = dbcontext.SaveChanges();
                        dbTransaction.Commit();
                    }
                    catch (SqlException oEx)
                    {
                        if (dbTransaction.UnderlyingTransaction.Connection != null)
                        { dbTransaction.Rollback(); }
                        ManagedException mEx = mEx = new ManagedException("Errore nell'inserimento su DB della mail con uid: " + m.Uid
                                + " della casella " + user.EmailAddress,
                                "ERR_INS_ML_001", string.Empty, string.Empty, oEx.InnerException);
                        mEx.addEnanchedInfosTag("DETTAGLIO", "Classe: MailMessageDaoOracleDb " + "Metodo: Insert(MailUser user, Message m) " +
                            "Dettaglio: Salvataggio della mail su Oracle DB " +
                            "User Login: " + user.LoginId + " Mail UID: " + m.Uid);
                        ErrorLogInfo err;

                        if (oEx.Message.StartsWith("ORA-00001", StringComparison.InvariantCultureIgnoreCase))
                        {
                            mEx.CodiceEccezione = "WRN_INS_ML_001";
                            err = new ErrorLogInfo(mEx);
                            _log.Warn(err);
                        }
                        else
                        {
                            err = new ErrorLogInfo(mEx);
                            _log.Error(err);
                        }
                        if (dbTransaction.UnderlyingTransaction.Connection != null)
                        { dbTransaction.Rollback(); }
                        throw mEx;
                    }
                    catch (Exception ex)
                    {
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException(ex.Message,
                                "ERR_COM_006", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);

                            _log.Error(err);
                        }
                        if (dbTransaction.UnderlyingTransaction.Connection != null)
                        { dbTransaction.Rollback(); }
                        throw;
                    }
                }
            }
        }

        public void InsertFlussoInbox(long id, ActiveUp.Net.Common.DeltaExt.MailStatus oldSt, ActiveUp.Net.Common.DeltaExt.MailStatus newSt, DateTime? dataOp, string uteOpe)
        {
            throw new NotImplementedException();
        }

        public ActiveUp.Net.Mail.Message GetById(string mailID, string mailAccount, string mailFolder)
        {
            Message msg = null;

            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    switch (mailFolder.Substring(0, 1))
                    {
                        case "1":
                        case "3":
                            MAIL_INBOX inbox = dbcontext.MAIL_INBOX.Where(x => x.MAIL_SERVER_ID == mailID && x.MAIL_ACCOUNT.ToString() == mailAccount.ToString()).FirstOrDefault();
                            msg = AutoMapperConfiguration.MapToMessageModel(inbox);
                            //   GetInboxMessage(mailID, mailAccount);
                            break;
                        case "2":
                            //msg = GetOutBoxMessage(mailID, mailAccount);
                            MAIL_CONTENT content = dbcontext.MAIL_CONTENT.Where(x => x.ID_MAIL == int.Parse(mailID)).FirstOrDefault();
                            List<COMUNICAZIONI_ALLEGATI> allegati = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.REF_ID_COM == content.REF_ID_COM).ToList();
                            msg = AutoMapperConfiguration.MapToMessageModelOut(content, allegati);
                            break;
                        default:
                            switch (mailFolder.Substring(1, 1))
                            {
                                case "I":
                                    MAIL_INBOX inbox1 = dbcontext.MAIL_INBOX.Where(x => x.MAIL_SERVER_ID == mailID && x.MAIL_ACCOUNT.ToString() == mailAccount.ToString()).FirstOrDefault();
                                    msg = AutoMapperConfiguration.MapToMessageModel(inbox1);
                                    //  msg = GetInboxMessage(mailID, mailAccount);
                                    break;
                                case "O":
                                    // msg = GetOutBoxMessage(mailID, mailAccount);
                                    MAIL_CONTENT content1 = dbcontext.MAIL_CONTENT.Where(x => x.ID_MAIL == int.Parse(mailID)).FirstOrDefault();
                                    List<COMUNICAZIONI_ALLEGATI> allegati1 = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.REF_ID_COM == content1.REF_ID_COM).ToList();
                                    msg = AutoMapperConfiguration.MapToMessageModelOut(content1, allegati1);
                                    break;
                                default:
                                    //  msg = GetInboxMessage(mailID, mailAccount);
                                    MAIL_INBOX inbox2 = dbcontext.MAIL_INBOX.Where(x => x.MAIL_SERVER_ID == mailID && x.MAIL_ACCOUNT.ToString() == mailAccount.ToString()).FirstOrDefault();
                                    if (inbox2.ID_MAIL != 0)
                                    {
                                        msg = AutoMapperConfiguration.MapToMessageModel(inbox2);
                                    }
                                    else
                                    {
                                        MAIL_CONTENT content2 = dbcontext.MAIL_CONTENT.Where(x => x.ID_MAIL == int.Parse(mailID)).FirstOrDefault();
                                        if (content2.REF_ID_COM != 0)
                                        {
                                            List<COMUNICAZIONI_ALLEGATI> allegati2 = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.REF_ID_COM == content2.REF_ID_COM).ToList();
                                            msg = AutoMapperConfiguration.MapToMessageModelOut(content2, allegati2);
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo GetById(string mailID, string mailAccount, string mailFolder). Dettaglio: " + e.Message,
                            "E006", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                }
            }
            return msg;
        }

        public ActiveUp.Net.Mail.Message GetOutBoxMessageByComId(string comId)
        {
            Message msg = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    MAIL_CONTENT content = dbcontext.MAIL_CONTENT.Where(x => x.REF_ID_COM == int.Parse(comId)).FirstOrDefault();
                    List<COMUNICAZIONI_ALLEGATI> allegati = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.REF_ID_COM == content.REF_ID_COM).ToList();
                    msg = AutoMapperConfiguration.MapToMessageModelOut(content, allegati);
                }
                catch (Exception e)
                {
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo GetOutBoxMessageByComId(string comId) Dettaglio: " + e.Message,
                            "E016", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                }
            }
            return msg;
        }

        public System.Tuples.Tuple<ActiveUp.Net.Mail.Message, string, int, string> GetMessageById(string id)
        {
            System.Tuples.Tuple<Message, string, int, string> tuple = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    MAIL_INBOX inbox = dbcontext.MAIL_INBOX.Where(x => x.ID_MAIL == int.Parse(id)).FirstOrDefault();
                    tuple = AutoMapperConfiguration.MapToMailTuple(inbox);

                }
                if (tuple == null)
                {
                    Message msg = GetById(id, null, "2");
                    if (msg != null)
                    {
                        tuple = new System.Tuples.Tuple<Message, string, int, string>();
                        tuple.Element1 = msg;
                        tuple.Element2 = msg.FolderId.ToString() + msg.FolderTipo;
                        tuple.Element3 = -1;
                        tuple.Element4 = "2";
                    }
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nel metodo GetMessageById(string id). Dettaglio: " + e.Message,
                        "E005", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(err);
                }
            }
            return tuple;
        }

        public void Delete(string mailID, string mailAccount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dismessa 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="da"></param>
        /// <param name="per"></param>
        /// <returns></returns>
        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.Message> GetAllMessageByAccount(string account, int da, int per)
        {
            throw new NotImplementedException();
        }

        public int UpdateRating(string idMail, string account, int rating)
        {
            int result = 0;
            if (String.IsNullOrEmpty(idMail) && String.IsNullOrEmpty(account) &&
                 (rating < 0 || rating > 4))
            {
                throw new ArgumentException("Errore nei parametri");
            }
            using (var dbcontext = new FAXPECContext())
            {
                using (var dbTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        MAIL_INBOX inbox = dbcontext.MAIL_INBOX.Where(x => x.MAIL_SERVER_ID == idMail && x.MAIL_ACCOUNT.ToUpper() == account.ToUpper()).First();
                        inbox.FLG_RATING = rating;
                        result = dbcontext.SaveChanges();
                        if (result != 1)
                        {
                            if (dbTransaction.UnderlyingTransaction.Connection != null)
                            { dbTransaction.Rollback(); }
                        }
                        else
                        {
                            if (dbTransaction.UnderlyingTransaction.Connection != null)
                            { dbTransaction.Commit(); }
                        }
                    }
                    catch (Exception e)
                    {
                        if (!e.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel metodo UpdateRating(string idMail, string account, int rating) Dettaglio: " + e.Message,
                                "E017", string.Empty, string.Empty, e.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            _log.Error(err);
                        }
                    }
                }
            }
            return result;
        }

        public int UpdateServerStatus(string account, string mailUid, ActiveUp.Net.Common.DeltaExt.MailStatusServer status)
        {

            int rows = 0;

            using (var dbcontext = new FAXPECContext())
            {
                using (var dbTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        MAIL_INBOX inbox = dbcontext.MAIL_INBOX.Where(x => x.MAIL_SERVER_ID == mailUid && x.MAIL_ACCOUNT.ToUpper() == account.ToUpper()).First();
                        inbox.STATUS_SERVER = ((int)status).ToString();
                        rows = dbcontext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        if (dbTransaction.UnderlyingTransaction.Connection != null)
                        { dbTransaction.Rollback(); }
                        if (!e.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel metodo UpdateServerStatus(string account, string mailUid, ActiveUp.Net.Common.DeltaExt.MailStatusServer status) Dettaglio: " + e.Message,
                                "E018", string.Empty, string.Empty, e.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            _log.Error(err);
                        }
                    }
                }
            }
            return rows;
        }

        public void Update(ActiveUp.Net.Mail.DeltaExt.MailUser u, ActiveUp.Net.Mail.Message m)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    MAIL_INBOX inbox = dbcontext.MAIL_INBOX.Where(x => x.ID_MAIL == int.Parse(m.Uid) && x.MAIL_ACCOUNT.ToUpper() == u.EmailAddress.ToUpper()).First();
                    inbox = DaoSQLServerDBHelper.MapToMailInBox(u, m);
                    dbcontext.SaveChanges();
                }
                catch (Exception e)
                {
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo Update(ActiveUp.Net.Mail.DeltaExt.MailUser u, ActiveUp.Net.Mail.Message m) Dettaglio: " + e.Message,
                            "E015", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                }
            }
        }

        public ICollection<ActiveUp.Net.Mail.Message> GetAll()
        {
            throw new NotImplementedException();
        }

        public ActiveUp.Net.Mail.Message GetById(string id)
        {
            Message msg = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    MAIL_INBOX inbox = dbcontext.MAIL_INBOX.Where(x => x.ID_MAIL == int.Parse(id)).First();
                    msg = AutoMapperConfiguration.MapToMessageModel(inbox);
                }
            }
            catch (Exception e)
            {
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nel metodo GetById(string id). Dettaglio: " + e.Message,
                        "E004", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(err);
                }
            }
            return msg;
        }

        public void Insert(ActiveUp.Net.Mail.Message entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ActiveUp.Net.Mail.Message entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
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
        // ~MailMessageDaoSQLDb() {
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

    }
}

