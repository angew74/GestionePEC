using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class BackEndCodeSQLDb : IBackEndCodeDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BackEndCodeSQLDb));

        #region IDao<BackEndRefCode,long> Membri di

        public ICollection<SendMail.Model.BackEndRefCode> GetAll()
        {
            List<BackEndRefCode> entityList = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var allEntity = dbcontext.RUBR_BACKEND.ToList();
                    entityList = AutoMapperConfiguration.FromRubrBackendToModel(allEntity);

                }
            }
            //TODO:Exception Handling
            catch (Exception e0)
            {
                //Allineamento log - Ciro
                if (e0.GetType() != typeof(ManagedException))
                {
                    ManagedException me = new ManagedException(e0.Message, "ORA_ERR001", string.Empty, string.Empty, e0);
                    ErrorLogInfo err = new ErrorLogInfo(me);
                    log.Error(err);
                }
                return null;
            }
            return entityList;
        }

        public SendMail.Model.BackEndRefCode GetById(decimal id)
        {
            BackEndRefCode entity = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_BACKEND b = dbcontext.RUBR_BACKEND.Where(x => x.ID_BACKEND == id).First();
                    if (b != null && b.ID_BACKEND != 0)
                    {
                        entity = AutoMapperConfiguration.FromRubrBackendToSingleModel(b);
                    }
                    else { return null; }
                }
            }
            catch (Exception e0)
            {
                if (e0.GetType() != typeof(ManagedException))
                {
                    ManagedException me = new ManagedException(e0.Message, "ORA_ERR002", string.Empty, string.Empty, e0);
                    ErrorLogInfo err = new ErrorLogInfo(me);
                    log.Error(err);
                }
                return null;
            }
            return entity;
        }

        public void Insert(SendMail.Model.BackEndRefCode entity)
        {
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_BACKEND r = AutoMapperConfiguration.FromRubrBackendToDto(entity);
                    dbcontext.RUBR_BACKEND.Add(r);
                    int risp = dbcontext.SaveChanges();
                    if (risp != 1)
                        throw new InvalidOperationException("Dato non inserito");
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ERR_ORADB111",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                else throw;
            }
        }

        public void Update(SendMail.Model.BackEndRefCode entity)
        {
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_BACKEND b = dbcontext.RUBR_BACKEND.Where(x => x.ID_BACKEND == entity.Id).First();
                    b = AutoMapperConfiguration.FromModelToRubrBackendDto(entity, b);
                    int risp = dbcontext.SaveChanges();
                    if (risp != 1)
                        throw new InvalidOperationException("Oggetto non aggiornato");
                }
            }
            catch (Exception e0)
            {
                if (e0.GetType() != typeof(ManagedException))
                {
                    ManagedException me = new ManagedException(e0.Message, "ORA_ERR022", string.Empty, string.Empty, e0);
                    ErrorLogInfo err = new ErrorLogInfo(me);
                    log.Error(err);
                }
                throw;
            }
        }

        public void Delete(decimal id)
        {
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_BACKEND r = dbcontext.RUBR_BACKEND.Where(x => x.ID_BACKEND == id).First();
                    dbcontext.RUBR_BACKEND.Remove(r);
                    int resp = dbcontext.SaveChanges();
                    if (resp != 1)
                        throw new InvalidOperationException("Oggetto non cancellato");
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR006", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        #endregion

        #region IBackEndCodeDao Membri di

        public BackEndRefCode GetByCode(string backendCode)
        {

            BackEndRefCode entity = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_BACKEND b = dbcontext.RUBR_BACKEND.Where(x => x.BACKEND_CODE.ToUpper() == backendCode.ToUpper()).First();
                    if (b != null && b.ID_BACKEND != 0)
                    {
                        entity = AutoMapperConfiguration.FromRubrBackendToSingleModel(b);
                    }
                    else { return null; }
                }
            }
            catch
            {
                return null;
            }
            return entity;
        }

        public List<BackEndRefCode> GetByDescr(string backendDescr)
        {
            List<BackEndRefCode> entity = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var lb = dbcontext.RUBR_BACKEND.Where(x => x.BACKEND_DESCR.ToUpper().Contains(backendDescr.ToUpper())).ToList();
                    if (lb.Count > 0)
                    {
                        entity = AutoMapperConfiguration.FromRubrBackendToModel(lb);
                    }
                    else { return null; }
                }
            }
            catch
            {
                return null;
            }
            return entity;
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
        // ~BackEndCodeSQLDb() {
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

        #region mapping


        #endregion
    }
}
