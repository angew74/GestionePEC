using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class SendersFoldersSQLDb : ISendersFoldersDao
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SendersFoldersSQLDb));
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
                using (var dbcontext = new FAXPECContext())
                {
                    using (var t = dbcontext.Database.Connection.BeginTransaction())
                    {
                        List<FOLDERS> folders = dbcontext.FOLDERS.Where(x => x.IDNOME == idNome).ToList();
                        if (folders.Count > 0)
                        {
                            foreach (FOLDERS f in folders)
                            {

                                string text = "DELETE FROM folders_senders WHERE idfolder = " + f.ID + " AND idsender = " + idSender;
                                dbcontext.Database.ExecuteSqlCommand(text);
                            }
                        }
                    }
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
            using (var dbcontext = new FAXPECContext())
            {
                using (var t = dbcontext.Database.Connection.BeginTransaction())
                {
                    try
                    {
                        List<FOLDERS> folders = dbcontext.FOLDERS.Where(x => x.IDNOME == idNome).ToList();
                        if (folders.Count > 0)
                        {
                            foreach (FOLDERS f in folders)
                            {
                                Guid d = new Guid();
                                Guid.TryParse(system.ToString(), out d);
                                FOLDERS_SENDERS s = new FOLDERS_SENDERS()
                                {
                                    IDFOLDER = f.ID,
                                    IDSENDER = idSender,
                                    ROWID = d
                                };
                                dbcontext.FOLDERS_SENDERS.Add(s);
                            }
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
                    t.Commit();
                }
                dbcontext.SaveChanges();

            }
            return 0;
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
                using (var dbcontext = new FAXPECContext())
                {
                    var oCmd = dbcontext.Database.Connection.CreateCommand();
                        oCmd.CommandText = "SELECT DISTINCT m.ID_SENDER, f.NOME, m.MAIL, f.IDNOME, f.SYSTEM " +
                                            "FROM MAIL_SENDERS m,folders f,folders_senders fs " +
                                            "WHERE m.mail = '" + mail + "' " +
                                            "MINUS " +
                                            "SELECT DISTINCT m.ID_SENDER, f.NOME, m.MAIL, f.IDNOME, f.SYSTEM " +
                                            "FROM MAIL_SENDERS m,folders f,folders_senders fs " +
                                            "WHERE m.mail = '" + mail + "' "+
                                            "AND m.ID_SENDER = fs.IDSENDER " +
                                            "AND f.ID = fs.IDFOLDER";                  
                    using (var r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaCartelle = new List<SendersFolders>();
                            while (r.Read())
                            {
                                SendersFolders sFold = DaoSQLServerDBHelper.MapToSendersFolders(r);
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
                using (var dbcontext = new FAXPECContext())
                {
                    var oCmd = dbcontext.Database.Connection.CreateCommand();
                    oCmd.CommandText = "SELECT DISTINCT m.ID_SENDER, f.NOME, m.MAIL, f.IDNOME, f.SYSTEM " +
                                            "FROM MAIL_SENDERS m,folders f,folders_senders fs " +
                                            "WHERE m.mail = '" +mail + "'  " +
                                            "AND m.ID_SENDER = fs.IDSENDER " +
                                            "AND f.ID = fs.IDFOLDER";                 
                    using (var r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listaCartelle = new List<SendersFolders>();
                            while (r.Read())
                            {
                                SendersFolders sFold = DaoSQLServerDBHelper.MapToSendersFolders(r);
                                listaCartelle.Add(sFold);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                listaCartelle = null;                
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
        // ~SendersFoldersSQLDb() {
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
    }
}
