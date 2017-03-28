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

    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }
}
}
