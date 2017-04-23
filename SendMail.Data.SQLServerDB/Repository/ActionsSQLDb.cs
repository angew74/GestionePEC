using SendMail.Data.SQLServerDB.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Data.SQLServerDB.Mapping;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class ActionsSQLDb : IActionsDao
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ActionsSQLDb));
        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public ICollection<ActiveUp.Net.Common.DeltaExt.Action> GetAll()
        {
            throw new NotImplementedException();
        }

        public ActiveUp.Net.Common.DeltaExt.Action GetById(long id)
        {
            ActiveUp.Net.Common.DeltaExt.Action action = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    ACTIONS a = dbcontext.ACTIONS.Where(x => x.ID == id).FirstOrDefault();
                    action = AutoMapperConfiguration.MapToAction(a);
                }
                catch (Exception ex)
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ACT_ORA003", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                return action;
            }
        }

        public void Insert(ActiveUp.Net.Common.DeltaExt.Action entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {                   
                    ACTIONS action = DaoSQLServerDBHelper.MapToActionDto(entity, true);
                    dbcontext.ACTIONS.Add(action);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message, "ACT_ORA001", string.Empty, string.Empty, ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                }
            }
        }

        public void Insert(List<ActiveUp.Net.Common.DeltaExt.Action> entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.BeginTransaction())
                {
                    foreach (ActiveUp.Net.Common.DeltaExt.Action a in entity)
                    {
                        try
                        {                           
                            ACTIONS action = DaoSQLServerDBHelper.MapToActionDto(a, true);
                            dbcontext.ACTIONS.Add(action);
                            dbcontext.SaveChanges();
                            a.Id =(decimal) dbcontext.ACTIONS.Select(x => x.ID).DefaultIfEmpty(0).Max();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            if (ex.GetType() == typeof(ManagedException))
                            {
                                ManagedException mEx = new ManagedException(ex.Message, "ACT_ORA004", string.Empty, string.Empty, ex);
                                ErrorLogInfo er = new ErrorLogInfo(mEx);
                                log.Error(er);
                                throw mEx;
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        public void Update(ActiveUp.Net.Common.DeltaExt.Action entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var action = dbcontext.ACTIONS.Where(x => x.ID == (double)entity.Id).FirstOrDefault();
                    action = DaoSQLServerDBHelper.MapToActionDto(entity, false);
                    int resp = dbcontext.SaveChanges();
                    if (resp == 0)
                    {
                        throw new Exception("Nessun record aggiornato");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message, "ACT_ORA002", string.Empty, string.Empty, ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
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
        // ~ActionsSQLDb() {
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
