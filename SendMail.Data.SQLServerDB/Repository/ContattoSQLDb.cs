using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class ContattoSQLDb : IContattoDao
    {
        private static ILog _log = LogManager.GetLogger("ContattoSQLDb");
        #region IContattoDao Membri di

        public List<SendMail.Model.RubricaMapping.RubricaContatti> LoadContattiOrgByOrgId(long identita, bool startFromOrg, bool includeDescendant, bool includeIPA, bool includeAppMappings)
        {

            List<RubricaContatti> lContatti = null;
            try
            {
                List<RUBR_CONTATTI> list = null;
                using (var dbcontext = new FAXPECContext())
                {
                    var matches = dbcontext.RUBR_CONTATTI.Select(f => f);
                    if (startFromOrg)
                    {
                        matches = dbcontext.RUBR_CONTATTI.Where(x => x.RUBR_ENTITA.ID_REFERRAL == (int)identita);
                        //var a = (from r in dbcontext.RUBR_CONTATTI 
                        //     where r.RUBR_ENTITA.ID_REFERRAL == (int)identita); 
                    }
                    else
                    {
                        matches = dbcontext.RUBR_CONTATTI.Where(x => x.REF_ID_REFERRAL == (int)identita);
                        //var a = (from r in dbcontext.RUBR_CONTATTI
                        //        where r.REF_ID_REFERRAL ==(int) identita);
                    }
                    if (includeDescendant)
                    { matches = dbcontext.RUBR_CONTATTI.Where(x => x.RUBR_ENTITA.ID_PADRE == (int)identita); }
                    list = matches.ToList();
                    if (list.Count > 0)
                    {
                        lContatti = new List<RubricaContatti>();
                        foreach (RUBR_CONTATTI r in list)
                        {
                            if (includeAppMappings)
                            {
                                var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                                  where t.REF_ID_CONTATTO == r.ID_CONTACT
                                                  select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                                lContatti.Add(AutoMapperConfiguration.MapToRubrContatti(r, ListTitoli));
                            }
                            else
                            {

                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {

                ManagedException mEx = new ManagedException("Errore nella ricerca in rubrica dei contatti per organizzazione  Data Layer E077 Dettagli Errore: " + ex.Message,
                            "ERR_077", string.Empty, string.Empty, ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);

                _log.Error(err);
                throw mEx;
            }

            return lContatti;
        }

        public IList<SendMail.Model.RubricaMapping.RubricaContatti> LoadContattiOrgByName(string identita, bool startFromOrg, bool includeDescendant, bool includeIPA)
        {
            List<RubricaContatti> lCont = null;

            List<SendMail.Model.EntitaType> tEnt = new List<SendMail.Model.EntitaType>();
            tEnt.Add(SendMail.Model.EntitaType.ALL);
            try
            {
                RubrEntitaSQLDb sql = new RubrEntitaSQLDb();
                List<RubricaEntita> lEntita = sql.LoadEntitaByName(tEnt, identita);               
                if (lEntita != null && lEntita.Count != 0)
                {
                    foreach (RubricaEntita e in lEntita)
                    {
                        if (e.IdReferral.HasValue)
                        {
                            lCont.AddRange(this.LoadContattiOrgByOrgId(e.IdReferral.Value, startFromOrg, includeDescendant, includeIPA, false));
                        }
                    }
                }
            }
            catch (Exception ex)
            {               
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR011", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
            return lCont;
        }

        public IEnumerable<RubricaContatti> LoadContattiByTitoloAndBackendCode(string titolo, string backendCode)
        {            
            List<RubricaContatti> list = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var rubrcontattibackend = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.COMUNICAZIONI_TITOLI.APP_CODE.ToUpper() == titolo.ToUpper() && x.RUBR_BACKEND.BACKEND_CODE.ToUpper() == backendCode.ToUpper()).ToList();
                    foreach (var r in rubrcontattibackend)
                    {
                        var contatto = r.RUBR_CONTATTI;
                        list.Add(new RubricaContatti
                        {
                            IdContact = (long)contatto.ID_CONTACT,
                            RefIdReferral = (long)contatto.REF_ID_REFERRAL,
                            Mail = contatto.MAIL,
                            T_isMappedAppDefault = true
                        });

                    }
                }
                catch { }
            }

            return list;
        }

        public ResultList<RubricaContatti> LoadContattiByParams(List<SendMail.Model.EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita)
        {
            if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }
            if (tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN))
                throw new ArgumentException("Tipo entità errato");
           
            List<RubricaContatti> lContatti = null;
            List<RubricaEntita> lEntita = null;
            ResultList<RubricaContatti> res = new ResultList<RubricaContatti>();

            var pp = from k in pars.Keys
                     where k == SendMail.Model.FastIndexedAttributes.COGNOME ||
                           k == SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE ||
                           k == SendMail.Model.FastIndexedAttributes.UFFICIO
                     select k;

            var entPar = (from ep in pars
                          where pp.Contains(ep.Key)
                          select ep).ToDictionary((x =>
                                  x.Key), (y => (IList<string>)y.Value));

            if (pp.Count() != 0)
            {
                RubrEntitaSQLDb sql = new RubrEntitaSQLDb();
                ResultList<RubricaEntita> rE = sql.LoadEntitaByParams((IList<SendMail.Model.EntitaType>)tEnt,
                    entPar, da, per);
                   lEntita = (List<RubricaEntita>)rE.List;
            }

            var cPars = from p in pars
                        where !entPar.Keys.Contains(p.Key)
                        select p;

            //as IEnumerable<KeyValuePair<SendMail.Model.FastIndexedAttributes,
            //List<string>>>;
            if (cPars.Count() != 0)
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var query = dbcontext.RUBR_CONTATTI.AsQueryable();

                    List<RUBR_CONTATTI> list = new List<RUBR_CONTATTI>();
                    foreach (var kvp in cPars)
                    {

                        switch (kvp.Key)
                        {
                            case SendMail.Model.FastIndexedAttributes.FAX:
                                string[] faxies = kvp.Value.ToArray();
                               list = query.Where(x => faxies.Contains(x.FAX)).ToList();
                                break;

                            case SendMail.Model.FastIndexedAttributes.MAIL:
                                string[] mailies = kvp.Value.ToArray();
                                list= query.Where(x => mailies.Contains(x.MAIL)).ToList();
                                break;

                            case SendMail.Model.FastIndexedAttributes.TELEFONO:
                                string[] telefoni = kvp.Value.ToArray();
                                list = query.Where(x => telefoni.Contains(x.TELEFONO)).ToList();
                                break;

                            default:
                                break;
                        }
                    }
                    lContatti = new List<RubricaContatti>();
                    foreach (var c in list)
                    {
                        var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                          where t.REF_ID_CONTATTO == c.ID_CONTACT
                                          select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();

                        lContatti.Add(AutoMapperConfiguration.MapToRubrContatti(c,ListTitoli));                        
                    }                    
                }
            }

            if (lEntita != null)
            {
                if (lContatti == null) lContatti = new List<RubricaContatti>();
                lEntita.ForEach(e => lContatti.AddRange(LoadContattiOrgByOrgId(e.IdReferral.Value, true, true, false, true)));
            }

            if (lContatti != null)
            {
                res.List = new List<RubricaContatti>();
                foreach (var c in lContatti)
                {
                    if (res.List.SingleOrDefault(x => x.IdContact == c.IdContact) == null)
                    {
                        res.List.Add(c);
                    }
                }

                res.Da = da;
                res.Totale = res.List.Count;
                res.Per = (per > res.Totale) ? res.Totale : per;
            }
            return res;
        }

        public ResultList<RubricaContatti> LoadContattiIPAByParams(List<SendMail.Model.EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita)
        {
            if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }
            if (tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN))
                throw new ArgumentException("Tipo entità errato");

            //commentato oracle 10
            //V_Rubr_Contatti_Obj v = new V_Rubr_Contatti_Obj(this.context);
            //ResultList<RubricaContatti> res =
            //    v.ConvertToRubricaContatti(v.GetContattiIPAByParams(tEnt, pars, da, per, withEntita));

            ResultList<RubricaContatti> res = null;
            return res;
        }

        public ResultList<SimpleResultItem> LoadFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per)
        {
            if (String.IsNullOrEmpty(par.Value)) return null;
            if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }

            ResultList<SimpleResultItem> res = new ResultList<SimpleResultItem>();
            res.Da = da;
            string field = string.Empty;
            using (var dbcontext = new FAXPECContext())
            {
                var querable = dbcontext.RUBR_ENTITA.AsQueryable();
                switch (par.Key)
                {
                    case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                        field = "RAGIONE_SOCIALE";
                        querable = dbcontext.RUBR_ENTITA.Where(x => x.RAGIONE_SOCIALE.ToUpper().Contains(par.Value.ToUpper()));
                        break;
                    case SendMail.Model.FastIndexedAttributes.COGNOME:
                        field = "COGNOME";
                        querable = dbcontext.RUBR_ENTITA.Where(x => x.COGNOME.ToUpper().Contains(par.Value.ToUpper()));
                        break;
                    case SendMail.Model.FastIndexedAttributes.FAX:
                        field = "FAX";
                        querable = dbcontext.RUBR_CONTATTI.Where(x => x.FAX.ToUpper().Contains(par.Value)).Select(z=>z.RUBR_ENTITA);
                        break;
                    case SendMail.Model.FastIndexedAttributes.MAIL:
                        field = "MAIL";
                        querable = dbcontext.RUBR_CONTATTI.Where(x => x.MAIL.ToUpper().Contains(par.Value)).Select(z=>z.RUBR_ENTITA);
                        break;
                    case SendMail.Model.FastIndexedAttributes.TELEFONO:
                        field = "TELEFONO";
                        querable = dbcontext.RUBR_CONTATTI.Where(x => x.TELEFONO.ToUpper().Contains(par.Value)).Select(z=>z.RUBR_ENTITA);
                        break;
                    case SendMail.Model.FastIndexedAttributes.UFFICIO:
                        querable = dbcontext.RUBR_ENTITA.Where(x => x.UFFICIO.ToUpper().Contains(par.Value));
                        break;
                    default:
                        throw new ArgumentException("Parametro non implementato");
                }
                if (!tEnt.Contains(SendMail.Model.EntitaType.ALL) && !tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN))
                {
                    var referrals = tEnt.Select(t => t.ToString()).ToArray();
                    querable = querable.Where(x => referrals.Contains(x.REFERRAL_TYPE));
                }
                try
                {
                    int tot = querable.Count();
                    res.Per = (tot > per) ? per : tot;
                    res.Totale = tot;
                    List<RUBR_ENTITA> list = querable.OrderBy(f=>f.ID_REFERRAL).Skip(res.Da).Take(res.Per).ToList();
                    foreach (RUBR_ENTITA r in list)
                    {
                        res.List.Add(
                                         new SimpleResultItem(
                                           //  r.GetValue("descr").ToString(),
                                           r.RAGIONE_SOCIALE,
                                           r.ID_REFERRAL.ToString(),
                                             String.Format("{0} {1} {2}", r.DISAMB_PRE, r.RAGIONE_SOCIALE, r.DISAMB_POST),
                                           r.REFERRAL_TYPE,
                                           "R" ,
                                             100));
                    }
                }
                catch(Exception ex)
                {
                    ManagedException mEx = new ManagedException("Errore nella ricerca in rubrica dell'entità o contatto  Data Layer E075 Dettagli Errore: " + ex.Message,
                                "ERR_075", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    _log.Error(err);
                    int tot = 0;
                    res.List = null;
                    throw mEx;
                   
                }


            }      
            return res;
        }

        public ResultList<SimpleResultItem> LoadSimilarityFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per)
        {
            throw new NotImplementedException();
            //if (String.IsNullOrEmpty(par.Value)) return null;
            //if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            //if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }

            //ResultList<SimpleResultItem> res = new ResultList<SimpleResultItem>();
            //res.Da = da;
            //res.Per = per;
            //res.Totale = per;

            //string queryRubrica = "SELECT distinct r.RAGIONE_SOCIALE AS rag_soc"
            //                    + ", {0} as descr"
            //                    + ", r.DISAMB_PRE as prefix"
            //                    + ", r.DISAMB_POST as suffix"
            //                    + ", id_referral AS ids"
            //                    + ", 'R' as SRC"
            //                    + ", REFERRAL_TYPE as subtype"
            //                    + ", utl_match.edit_distance_similarity('{1}', lower({0})) AS sim"
            //                    + " FROM rubr_entita r {2}"
            //                    + " WHERE {3}"
            //                    + " order by 8 desc, 1";
            //string campi = "";
            //switch (par.Key)
            //{
            //    case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
            //    case SendMail.Model.FastIndexedAttributes.COGNOME:
            //    case SendMail.Model.FastIndexedAttributes.FAX:
            //    case SendMail.Model.FastIndexedAttributes.MAIL:
            //    case SendMail.Model.FastIndexedAttributes.TELEFONO:
            //    case SendMail.Model.FastIndexedAttributes.UFFICIO:
            //        campi += par.Key.ToString();
            //        break;
            //    default:
            //        throw new ArgumentException("Parametro non implementato");
            //}

            //string innerJoin = "";
            //if (par.Key.Equals(SendMail.Model.FastIndexedAttributes.FAX) ||
            //    par.Key.Equals(SendMail.Model.FastIndexedAttributes.MAIL) ||
            //    par.Key.Equals(SendMail.Model.FastIndexedAttributes.TELEFONO))
            //{
            //    innerJoin = "INNER JOIN rubr_contatti c ON c.REF_ID_REFERRAL = r.id_referral";
            //}

            //string whereConds = null;

            //if (!tEnt.Contains(SendMail.Model.EntitaType.ALL) && !tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN))
            //{
            //    whereConds += "REFERRAL_TYPE in (";
            //    whereConds += String.Join(", ", tEnt.Select(t => String.Format("'{0}'", t.ToString())).ToArray());
            //    whereConds += ") and ";
            //}

            //whereConds += "length(" + par.Key.ToString() + ") >= " + par.Value.Length;

            //string query = string.Format(queryRubrica, campi, par.Value, innerJoin, whereConds);

            //using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //{
            //    if (per > 0) { oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per); }
            //    else { oCmd.CommandText = query; }

            //    try
            //    {
            //        using (OracleDataReader r = oCmd.ExecuteReader())
            //        {
            //            if (r.HasRows)
            //            {
            //                res.List = new List<SimpleResultItem>();
            //                while (r.Read())
            //                {
            //                    res.List.Add(
            //                        new SimpleResultItem(
            //                            r.GetValue("descr").ToString(),
            //                            r.GetValue("ids").ToString(),
            //                            String.Format("{0} {1} {2}", r.GetValue("prefix"), r.GetValue("rag_soc"), r.GetValue("suffix")),
            //                            r.GetValue("subtype").ToString(),
            //                            r.GetValue("SRC").ToString(),
            //                            Convert.ToInt64(r.GetValue("sim"))));

            //                }
            //            }
            //        }
            //    }
            //    catch
            //    {
            //        res.List = null;
            //    }

            //}

            //return res;
        }

        #endregion

        #region IDao<RubricaContatti,long> Membri di

        public ICollection<SendMail.Model.RubricaMapping.RubricaContatti> GetAll()
        {
            throw new NotImplementedException();
        }
               
        public SendMail.Model.RubricaMapping.RubricaContatti GetById(long id)
        {
            RubricaContatti c = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    RUBR_CONTATTI v = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == id).First();
                    var ListTitoli = (from t in dbcontext.RUBR_CONTATTI_BACKEND
                                      where t.REF_ID_CONTATTO == v.ID_CONTACT
                                      select t.COMUNICAZIONI_TITOLI.ID_TITOLO).ToList();
                    c = AutoMapperConfiguration.MapToRubrContatti(v, ListTitoli);
                } catch (Exception e)
                {
                    throw e;
                }
            }
            return c;

        }

        public void Insert(SendMail.Model.RubricaMapping.RubricaContatti entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    if (entity.Entita != null)
                    {
                        try
                        {
                            RUBR_ENTITA e = DaoSQLServerDBHelper.MapToRubrEntita(entity.Entita, true);
                            dbcontext.RUBR_ENTITA.Add(e);
                            entity.Entita.IdReferral = (long)e.ID_REFERRAL;
                            entity.RefIdReferral = (long)e.ID_REFERRAL;
                            RUBR_CONTATTI c = DaoSQLServerDBHelper.MapToRubrContatti(entity,true);
                            entity.IdContact = (long)c.ID_CONTACT;
                            dbcontext.RUBR_CONTATTI.Add(c);
                            dbcontext.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException(ex.Message,
                                    "ORA_ERR012", string.Empty, string.Empty, ex);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                _log.Error(err);
                                throw mEx;
                            }
                            else throw ex;
                        }


                        dbContextTransaction.Commit();
                    }
                }
            }
        } 

        public void Update(SendMail.Model.RubricaMapping.RubricaContatti entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var transaction = dbcontext.Database.Connection.BeginTransaction())
                try
                {
                    var rubrOld = dbcontext.RUBR_CONTATTI.Where(x => x.ID_CONTACT == entity.IdContact).First();
                    var rubrNew = DaoSQLServerDBHelper.MapToRubrContatti(entity, false);
                    dbcontext.RUBR_CONTATTI.Remove(rubrOld);
                    dbcontext.RUBR_CONTATTI.Add(rubrNew);
                    int res = dbcontext.SaveChanges();
                }
                catch
                {
                        transaction.Rollback();
                    //todo
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
        // ~ContattoSQLDb() {
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
