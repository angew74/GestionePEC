using SendMail.Data.SQLServerDB.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Common.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using SendMail.Data.SQLServerDB.Mapping;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class FolderSQLDb : IFoldersDao
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(FolderSQLDb));
        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public ICollection<Folder> GetAll()
        {
            throw new NotImplementedException();
        }

        public Folder GetFolderByName(string name)
        {

            Folder folder = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    FOLDERS a = dbcontext.FOLDERS.Where(x => x.NOME.ToUpper() == name.ToUpper()).FirstOrDefault();
                    if (a != null)
                    { folder = AutoMapperConfiguration.MapToFolder(a); }
                }
                catch (Exception ex)
                {
                    ManagedException mEx = new ManagedException(ex.Message, "FOL_ORA005", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                return folder;
            }

        }

        public Folder GetById(long id)
        {
            ActiveUp.Net.Common.DeltaExt.Folder folder = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    FOLDERS a = dbcontext.FOLDERS.Where(x => x.ID == id).FirstOrDefault();
                    folder = AutoMapperConfiguration.MapToFolder(a);
                }
                catch (Exception ex)
                {
                    ManagedException mEx = new ManagedException(ex.Message, "FOL_ORA003", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                return folder;
            }
        }

        public void Insert(Folder entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    if (string.IsNullOrEmpty(entity.IdNome))
                    {
                        double? idnome = dbcontext.FOLDERS.Select(x => x.IDNOME).DefaultIfEmpty(0).Max();
                        decimal newidnome = default(decimal);
                        decimal.TryParse(idnome.ToString(), out newidnome);
                        entity.IdNome = newidnome.ToString();
                    }
                    decimal idnew = (decimal)dbcontext.FOLDERS.Select(x => x.ID).DefaultIfEmpty(0).Max();
                    entity.Id = idnew;
                    FOLDERS folder = DaoSQLServerDBHelper.MapToFolderDto(entity, true);
                    dbcontext.FOLDERS.Add(folder);
                    entity.Id = folder.ID;
                    dbcontext.SaveChanges();                   
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(ex.Message, "FOL_ORA001", string.Empty, string.Empty, ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                }
            }
        }

        public void Insert(List<Folder> entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        double? idnome = dbcontext.FOLDERS.Select(x => x.IDNOME).DefaultIfEmpty(0).Max();
                        decimal newidnome = default(decimal);
                        decimal.TryParse(idnome.ToString(), out newidnome);
                        foreach (Folder f in entity)
                        {
                            f.IdNome = newidnome.ToString();
                            FOLDERS folder = DaoSQLServerDBHelper.MapToFolderDto(f, true);
                            dbcontext.FOLDERS.Add(folder);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(ManagedException))
                        {
                            ManagedException mEx = new ManagedException(ex.Message, "FOL_ORA001", string.Empty, string.Empty, ex);
                            ErrorLogInfo er = new ErrorLogInfo(mEx);
                            log.Error(er);
                            throw mEx;
                        }
                    }
                }
            }
        }

        public void Update(Folder entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var folder = dbcontext.FOLDERS.Where(x => x.ID == (decimal)entity.Id).FirstOrDefault();
                    folder = DaoSQLServerDBHelper.MapToFolderDto(entity, false);
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
                        ManagedException mEx = new ManagedException(ex.Message, "FOL_ORA002", string.Empty, string.Empty, ex);
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
        // ~FolderSQLDb() {
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
