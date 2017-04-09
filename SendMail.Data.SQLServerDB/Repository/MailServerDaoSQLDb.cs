using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.SQLServerDB.Mapping;
using SendMailApp.Contracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
  public class MailServerDaoSQLDb :IMailServerDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailServerDaoSQLDb));
        public ICollection<MailServer> GetAll()
        {
            List<MailServer> mailservers = null;
            using (var dbcontext = new FAXPECContext())
            {
                // preparo il command
                try
                {
                    var list = dbcontext.MAILSERVERS.ToList();
                    foreach(MAILSERVERS s in list)
                    {
                        MailServer m = AutoMapperConfiguration.FromMailServersToModel(s);
                        mailservers.Add(m);
                    }                   
                }
                catch (SqlException oex)
                {
                    //Allineamento log - Ciro
                    if (oex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO + oex.Message,
                                        "DAL_TIT_001",
                                        string.Empty,
                                        string.Empty,
                                        oex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_001", "", "", "", "", "", oex);
                }
            }
            return mailservers;
        }

        public MailServer GetById(decimal id)
        {
            MailServer mailserver = null;
            using (var dbcontext = new FAXPECContext())
            {
             
                try
                {
                   var m = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == (double)id).First();
                    mailserver = AutoMapperConfiguration.FromMailServersToModel(m);                   
                }
                catch (SqlException oex)
                {
                    //Allineamento log - Ciro
                    if (oex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO + oex.Message,
                                        "DAL_TIT_010",
                                        string.Empty,
                                        string.Empty,
                                        oex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_010", "", "", "", "", "", oex);
                }
            }
            return mailserver;
        }

        public void Insert(MailServer entity)
        {
            using (var dbcontext = new FAXPECContext())
            {

                try
                {
                    MAILSERVERS m = AutoMapperConfiguration.FromMailServerToDto(entity,true);
                    dbcontext.MAILSERVERS.Add(m);
                    dbcontext.SaveChanges();
                    //param out
                    decimal iNewID = default(decimal);
                    decimal.TryParse(m.ID_SVR.ToString(), out iNewID);

                    //todo.. MIGLIORARE
                    if (iNewID != default(int))
                    {
                        entity.Id = iNewID;
                    }
                    else
                        throw new Exception(DalExMessages.ID_NON_RESTITUITO);
                }
                catch (InvalidOperationException ioex)
                {
                    //Allineamento log - Ciro
                    if (ioex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA + ioex.Message,
                                        "DAL_RUB_002",
                                        string.Empty,
                                        string.Empty,
                                        ioex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_002", "", "", "", "", "", ioex);

                }
                catch (SqlException oex)
                {
                    //Allineamento log - Ciro
                    if (oex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA + oex.Message,
                                        "DAL_RUB_001",
                                        string.Empty,
                                        string.Empty,
                                        oex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_001", "", "", "", "", "", oex);

                }
            }
        }

        public void Update(MailServer entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        var oldserver = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == (double)entity.Id).First();
                        var newserver = AutoMapperConfiguration.FromMailServerToDto(entity, false);
                        dbcontext.MAILSERVERS.Remove(oldserver);
                        dbcontext.MAILSERVERS.Add(newserver);
                        int rowAff = dbcontext.SaveChanges();
                        //todo.. MIGLIORARE
                        if (rowAff != 1)
                        {
                            transaction.Rollback();
                            //Allineamento log - Ciro
                            ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                                "DAL_TIT_009",
                                string.Empty,
                                string.Empty,
                                null);
                            ErrorLogInfo er = new ErrorLogInfo(mEx);
                            log.Error(er);
                            throw mEx;
                        }
                        //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);

                    }
                    catch (InvalidOperationException ex)
                    {
                        transaction.Rollback();
                        //Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA + " " + ex.Message,
                            "DAL_UNIQUE_CODE",
                            string.Empty,
                            string.Empty,
                            ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                        //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                    }
                }
            }
        }

        public void Delete(decimal id)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var oldserver = dbcontext.MAILSERVERS.Where(x => x.ID_SVR == (double)id).First();
                    dbcontext.MAILSERVERS.Remove(oldserver);
                    dbcontext.SaveChanges();
                }
                catch (Exception ex)
                {
                    //Allineamento log - Ciro
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA + " " + ex.Message,
                                    "DAL_UNIQUE_CODE_1",
                                    string.Empty,
                                    string.Empty,
                                    ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                }
            }
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
        // ~MailServerDaoSQLDb() {
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
