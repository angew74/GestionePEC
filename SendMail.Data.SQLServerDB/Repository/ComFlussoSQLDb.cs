using SendMail.Data.SQLServerDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
   public class ComFlussoSQLDb : IComFlussoDao
    { 
   public ICollection<SendMail.Model.ComunicazioniMapping.ComFlusso> GetAll()
    {
        throw new NotImplementedException();
    }

    public SendMail.Model.ComunicazioniMapping.ComFlusso GetById(long id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///  OK MIGRATO EF
    /// </summary>
    /// <param name="entity"></param>
    public void Insert(Model.ComunicazioniMapping.ComFlusso entity)
    {
        COMUNICAZIONI_FLUSSO flusso = AutoMapperConfiguration.FromComFlussoToDto(entity);
        using (FAXPECContext dbcontext = new FAXPECContext())
        {
            try
            {
                dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);
                dbcontext.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }

    /// <summary>
    /// OK MIGRATO EF
    /// </summary>
    /// <param name="entity"></param>
    public void Update(Model.ComunicazioniMapping.ComFlusso entity)
    {
        int rows = 0;
        using (FAXPECContext dbcontext = new FAXPECContext())
        {
            try
            {
                COMUNICAZIONI_FLUSSO oldflusso = dbcontext.COMUNICAZIONI_FLUSSO.Where(x => x.ID_FLUSSO == double.Parse(entity.IdFlusso.ToString())).FirstOrDefault();
                oldflusso.STATO_COMUNICAZIONE_NEW = entity.StatoComunicazioneNew.ToString();
                oldflusso.STATO_COMUNICAZIONE_OLD = entity.StatoComunicazioneOld.ToString();
                oldflusso.UTE_OPE = entity.UtenteOperazione;
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

    void IDao<Model.ComunicazioniMapping.ComFlusso, long>.Delete(long id)
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
        // ~ComFlussoSQLDb() {
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
