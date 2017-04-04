using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.Contracts;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model.ContactApplicationMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
   public class ContactsBackEndSQLDb : IContactsBackendDao
    {
        private static ILog _log = LogManager.GetLogger("ContactsBackEndSQLDb");

        #region IContactsBackendDao Membri di

        public ICollection<ContactsBackendMap> GetPerEntita(int idEntita)
        {
            List<ContactsBackendMap> m = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var r = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.REF_ID_ENTITA == idEntita).OrderBy(x => x.REF_ID_BACKEND).OrderBy(x => x.REF_ID_TITOLO).ToList();
                    if (r.Count > 0)
                    {
                        m = new List<ContactsBackendMap>();
                        foreach (RUBR_CONTATTI_BACKEND b in r)
                        {
                            var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                              where t.REF_ID_CONTATTO == b.REF_ID_CONTATTO
                                              select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                            var rubrcontatti = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == b.REF_ID_CONTATTO).First();
                            var rubrentita = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == b.REF_ID_ENTITA).First();
                            m.Add(AutoMapperConfiguration.MapToContactsBackendMap(b,rubrcontatti,rubrentita, ListTitoli));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return m;
        }

        public ICollection<ContactsBackendMap> GetPerTitoloEntita(int idTitolo, int idEntita)
        {
            List<ContactsBackendMap> m = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var r = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.REF_ID_ENTITA == idEntita && x.REF_ID_TITOLO == idTitolo).OrderBy(x => x.REF_ID_BACKEND).OrderBy(x => x.REF_ID_TITOLO).ToList();
                    if (r.Count > 0)
                    {
                        m = new List<ContactsBackendMap>();
                        foreach (RUBR_CONTATTI_BACKEND b in r)
                        {
                            var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                              where t.REF_ID_CONTATTO == b.REF_ID_CONTATTO
                                              select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                            var rubrcontatti = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == b.REF_ID_CONTATTO).First();
                            var rubrentita = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == b.REF_ID_ENTITA).First();
                            m.Add(AutoMapperConfiguration.MapToContactsBackendMap(b,rubrcontatti,rubrentita, ListTitoli));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return m;
        }

        public ICollection<ContactsBackendMap> GetPerCanaleTitoloEntita(int idCanale, int idTitolo, int idEntita)
        {
            List<ContactsBackendMap> m = null;
            try
            {

                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var r = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.REF_ID_ENTITA == idEntita && x.REF_ID_TITOLO == idTitolo && x.REF_ID_CANALE == idCanale).OrderBy(x => x.REF_ID_BACKEND).OrderBy(x => x.REF_ID_TITOLO).ToList();
                    if (r.Count > 0)
                    {
                        m = new List<ContactsBackendMap>();
                        foreach (RUBR_CONTATTI_BACKEND b in r)
                        {
                            var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                              where t.REF_ID_CONTATTO == b.REF_ID_CONTATTO
                                              select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                            var rubrcontatti = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == b.REF_ID_CONTATTO).First();
                            var rubrentita = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == b.REF_ID_ENTITA).First();
                            m.Add(AutoMapperConfiguration.MapToContactsBackendMap(b,rubrcontatti,rubrentita, ListTitoli));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return m;
        }

        #endregion

        #region IDao<ContactsBackendMap,int> Membri di

        public ICollection<ContactsBackendMap> GetAll()
        {

            List<ContactsBackendMap> m = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var r = dbcontext.RUBR_CONTATTI_BACKEND.ToList();
                    if (r.Count > 0)
                    {
                        m = new List<ContactsBackendMap>();
                        foreach (RUBR_CONTATTI_BACKEND b in r)
                        {
                            var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                              where t.REF_ID_CONTATTO == b.REF_ID_CONTATTO
                                              select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                            var rubrcontatti = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == b.REF_ID_CONTATTO).First();
                            var rubrentita = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == b.REF_ID_ENTITA).First();
                            m.Add(AutoMapperConfiguration.MapToContactsBackendMap(b,rubrcontatti,rubrentita, ListTitoli));
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return m;
        }

        public ContactsBackendMap GetById(int id)
        {
            ContactsBackendMap m = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var r = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.ID_MAP == id).OrderBy(x => x.REF_ID_BACKEND).OrderBy(x => x.REF_ID_TITOLO).FirstOrDefault();
                    if (r != null)
                    {
                        var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                          where t.REF_ID_CONTATTO == r.REF_ID_CONTATTO
                                          select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                        var rubrcontatti = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == r.REF_ID_CONTATTO).First();
                        var rubrentita = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == r.REF_ID_ENTITA).First();
                        m = AutoMapperConfiguration.MapToContactsBackendMap(r,rubrcontatti,rubrentita, ListTitoli);
                    }
                }
            }
            catch
            {
                throw;
            }
            return m;
        }

        public void Insert(ContactsBackendMap entity)
        {
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    RUBR_CONTATTI_BACKEND b = DaoSQLServerDBHelper.MapToRubrContattiBackend(entity);
                    dbcontext.RUBR_CONTATTI_BACKEND.Add(b);
                    int risp = dbcontext.SaveChanges();
                    if (risp == 1)
                        entity.Id =(int) b.ID_MAP;
                }
            }
            catch
            {
                throw;
            }
        }

        public void Update(ContactsBackendMap entity)
        {
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var oldentities = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.ID_MAP == entity.Id).First();
                    if (oldentities != null)
                    {
                        oldentities.REF_ID_CANALE = (int)entity.Canale;
                        oldentities.REF_ID_CONTATTO = (int)entity.Contatto.IdContact;
                        oldentities.REF_ID_BACKEND = (int)entity.Backend.Id;
                        oldentities.REF_ID_TITOLO = (int)entity.Titolo.Id;
                        int risp = dbcontext.SaveChanges();
                        if (risp != 1)
                            throw new InvalidOperationException("Errore nell'aggiornamento");
                    }

                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR009", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(entity.Id);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        public void Delete(int id)
        {
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    var oldentities = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.ID_MAP == id).First();
                    if (oldentities != null)
                    {
                        dbcontext.RUBR_CONTATTI_BACKEND.Remove(oldentities);
                        int risp = dbcontext.SaveChanges();
                        if (risp != 1)
                            throw new InvalidOperationException("Errore nella cancellazione");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR010", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(id);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        #endregion

        #region IDisposable Membri di
        public void Dispose()
        {

        }
        #endregion
    }
}
