using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model.ComunicazioniMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
   public class ComAllegatoSQLDb :  IComAllegatoDao
    {
        public void Update(IList<Model.ComunicazioniMapping.ComAllegato> entities)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (ComAllegato comAllegato in entities)
                        {
                            COMUNICAZIONI_ALLEGATI oldallegato = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.ID_ALLEGATO == comAllegato.IdAllegato).FirstOrDefault();
                            oldallegato.ALLEGATO_EXT = comAllegato.AllegatoExt;
                            oldallegato.ALLEGATO_FILE = comAllegato.AllegatoFile;
                        }
                        dbcontext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }

        }

        public ICollection<Model.ComunicazioniMapping.ComAllegato> GetAll()
        {
            throw new NotImplementedException();
        }

        public Model.ComunicazioniMapping.ComAllegato GetById(long id)
        {
            SendMail.Model.ComunicazioniMapping.ComAllegato alleg = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    COMUNICAZIONI_ALLEGATI allegato = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.ID_ALLEGATO == id).FirstOrDefault();
                    alleg = AutoMapperConfiguration.FromAllegatoToModel(allegato);
                }
                catch { }
            }
            return alleg;
        }

        public void Insert(Model.ComunicazioniMapping.ComAllegato entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Model.ComunicazioniMapping.ComAllegato entity)
        {
            int rows = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    COMUNICAZIONI_ALLEGATI oldallegato = dbcontext.COMUNICAZIONI_ALLEGATI.Where(x => x.ID_ALLEGATO == entity.IdAllegato).FirstOrDefault();
                    oldallegato.ALLEGATO_EXT = entity.AllegatoExt;
                    oldallegato.ALLEGATO_FILE = entity.AllegatoFile;
                    rows = dbcontext.SaveChanges();
                    if (rows != 1)
                        throw new Exception("Nessun record aggiornato");
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Delete(long id)
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
        // ~ComAllegatoSQLDb() {
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
