using SendMail.Data.SQLServerDB.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendMail.Model;
using SendMail.Data.SQLServerDB.Mapping;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class ActionsFoldersSQLDb : IActionsFoldersDao
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ActionsFoldersSQLDb));
        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public ICollection<ActionFolder> GetAll()
        {
            throw new NotImplementedException();
        }

        public ActionFolder GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(List<ActionFolder> af)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.BeginTransaction())
                {
                    foreach (ActionFolder a in af)
                    {
                        try
                        {                           
                            ACTIONS_FOLDERS actionFolder = DaoSQLServerDBHelper.MapToActionFolder(a, true);
                            dbcontext.ACTIONS_FOLDERS.Add(actionFolder);
                            dbcontext.SaveChanges();
                            a.iD = (double)dbcontext.ACTIONS_FOLDERS.Select(x => x.ID).DefaultIfEmpty(0).Max();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            if (ex.GetType() == typeof(ManagedException))
                            {
                                ManagedException mEx = new ManagedException(ex.Message, "ACF_ORA001", string.Empty, string.Empty, ex);
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

        public void Insert(ActionFolder entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ActionFolder entity)
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
        // ~ActionsFoldersSQLDb() {
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
