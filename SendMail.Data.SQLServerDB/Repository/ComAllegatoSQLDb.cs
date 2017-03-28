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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
