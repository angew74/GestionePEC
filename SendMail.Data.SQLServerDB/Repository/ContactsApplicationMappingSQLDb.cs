using Com.Delta.Data.QueryModel;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
  public class ContactsApplicationMappingSQLDb : IContactsApplicationMappingDao
    {
        private static ILog _log = LogManager.GetLogger(typeof(ContactsApplicationMappingSQLDb));
        #region "Private Fields"

        private const string selFields = "ID_MAP, ID_TITOLO, APP_CODE, TITOLO, TITOLO_PROT_CODE, TITOLO_ACTIVE"
                                     + ", ID_SOTTOTITOLO, SOTTOTITOLO, SOTTOTITOLO_PROT_CODE, SOTTOTITOLO_ACTIVE, COM_CODE"
                                     + ", ID_CONTACT, MAIL, FAX, TELEFONO, REF_ID_REFERRAL, FLG_PEC, REF_ORG, ID_CANALE"
                                     + ", CODICE, ID_BACKEND, BACKEND_CODE, BACKEND_DESCR, CATEGORY, DESCR_PLUS";

        private const String cmdSelectRoot = "SELECT " + selFields + " FROM V_MAP_APPL_CONTATTI_NEW";

        #endregion

        #region IContactsApplicationMappingDao Membri di

        public ICollection<ContactsApplicationMapping> GetContactsByCriteria(QueryCmp query)
        {
            List<ContactsApplicationMapping> listContacts = null;
            using (var dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectRoot + TranslateQuery(query, oCmd);
                    try
                    {
                        using (var r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                listContacts = new List<ContactsApplicationMapping>();
                                while (r.Read())
                                {
                                    listContacts.Add(DaoSQLServerDBHelper.MapToContactsApplication(r));
                                }
                            }
                        }
                    }
                    catch
                    {
                        //todo
                        throw;
                    }
                }
            }
            return listContacts;
        }

        public ICollection<ContactsApplicationMapping> FindByBackendCodeAndCodComunicazione(IEnumerable<string> codes, string codCom)
        {
            List<ContactsApplicationMapping> listContacts = null;
            List<V_MAP_APPL_CONTATTI_NEW> l = new List<V_MAP_APPL_CONTATTI_NEW>();
            string[] arrCodes = codes.ToArray();
            using (var dbcontext = new FAXPECContext())
            {
                var b = dbcontext.V_MAP_APPL_CONTATTI_NEW.Where(x => codes.Contains(x.BACKEND_CODE)).AsQueryable();
                if (!string.IsNullOrEmpty(codCom))
                { l = b.Where(x => x.COM_CODE.ToUpper() == codCom.ToUpper()).ToList(); }
                else { l = b.ToList(); }
                if (l.Count > 0)
                {
                    listContacts = new List<ContactsApplicationMapping>();
                    foreach (V_MAP_APPL_CONTATTI_NEW v in l)
                    { listContacts.Add(AutoMapperConfiguration.MapToContactsApplicationModel(v)); }
                }
            }

            return listContacts;
        }

        public ICollection<ContactsApplicationMapping> GetContactsByIdContact(long idContact)
        {
            List<ContactsApplicationMapping> res = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var v_appl = dbcontext.V_MAP_APPL_CONTATTI_NEW.Where(x => x.ID_CONTACT == idContact).ToList();
                    res = new List<ContactsApplicationMapping>();
                    foreach (V_MAP_APPL_CONTATTI_NEW v in v_appl)
                    {
                        res.Add(AutoMapperConfiguration.MapToContactsApplicationModel(v));
                    }
                }
            }
            catch
            {
                throw;
            }

            return res;
        }

        public void setDefaultContact(long idTitolo, long refOrg, long idContatto)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var v_rubr = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.REF_ID_TITOLO == idTitolo && x.REF_ID_ENTITA == refOrg).First();
                    if (v_rubr != null)
                    {
                        v_rubr.REF_ID_CONTATTO = (int)idContatto;
                        int ret = dbcontext.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ORA_ERR007", string.Empty, string.Empty, ex);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        _log.Error(err);
                        throw mEx;
                    }
                    else throw ex;
                }
            }
        }
        #endregion

        #region IDao<ContactsApplicationMapping,long> Membri di

        public ICollection<ContactsApplicationMapping> GetAll()
        {
            List<ContactsApplicationMapping> res = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var v_appl = dbcontext.V_MAP_APPL_CONTATTI_NEW.ToList();
                    foreach (V_MAP_APPL_CONTATTI_NEW v in v_appl)
                    {
                        res.Add(AutoMapperConfiguration.MapToContactsApplicationModel(v));
                    }
                }
            }
            catch
            {
                throw;
            }

            return res;
        }

        public ContactsApplicationMapping GetById(long id)
        {
            ContactsApplicationMapping app = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var v = dbcontext.V_MAP_APPL_CONTATTI_NEW.Where(x => x.ID_MAP == id).First();
                    if (v != null)
                    {
                        app = AutoMapperConfiguration.MapToContactsApplicationModel(v);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR008", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(id);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
            return app;
        }

        public void Insert(ContactsApplicationMapping entity)
        {

            int risp = 0;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_CONTATTI_BACKEND r = new RUBR_CONTATTI_BACKEND();
                    r.REF_ID_CANALE = (int)entity.IdCanale;
                    r.REF_ID_BACKEND = (int)entity.IdBackend;
                    r.REF_ID_CONTATTO = (int)entity.IdContact;
                    r.REF_ID_TITOLO = (int)entity.IdTitolo;
                    dbcontext.RUBR_CONTATTI_BACKEND.Add(r);
                    risp = dbcontext.SaveChanges();
                    if (risp == 1)
                        entity.IdMap =(long) r.ID_MAP;
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR009", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(entity.RefIdReferral);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }


        public void Update(ContactsApplicationMapping entity)
        {

            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    RUBR_CONTATTI_BACKEND vold = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.ID_MAP == entity.IdMap).First();
                    if (vold != null)
                    {
                        vold.REF_ID_CANALE = (int)entity.IdCanale;
                        vold.REF_ID_BACKEND = (int)entity.IdBackend;
                        vold.REF_ID_CONTATTO = (int)entity.IdContact;
                        vold.REF_ID_TITOLO = (int)entity.IdSottotitolo;
                    }
                    int risp = dbcontext.SaveChanges();
                    if (risp != 1)
                        throw new InvalidOperationException("Aggiornamento non effettuato");
                }
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR009", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = Convert.ToString(entity.RefIdReferral);
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        public void Delete(long id)
        {
            int risp = 0;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var v_old = dbcontext.RUBR_CONTATTI_BACKEND.Where(x => x.ID_MAP == id).First();
                    if (v_old != null)
                    {
                        dbcontext.RUBR_CONTATTI_BACKEND.Remove(v_old);
                        risp = dbcontext.SaveChanges();
                    }
                    if (risp != 1)
                        throw new InvalidOperationException("Cancellazione non effettuata");
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

        #region "Private methods"

        private string TranslateQuery(QueryCmp q, DbCommand oCmd)
        {
            string cmdText = String.Empty;

            if (q.WhereClause != null)
            {
                cmdText += " WHERE";
                int k = 0;
                cmdText += TranslateIClause(q.WhereClause, oCmd, ref k);
            }

            return cmdText;
        }

        private string TranslateIClause(IClause clause, DbCommand oCmd, ref int k)
        {
            String cmdText = "";
            if (clause is SingleClause)
            {
                SingleClause sc = clause as SingleClause;

                if (sc.Operator == CriteriaOperator.IsNull || sc.Operator == CriteriaOperator.IsNotNull)
                {
                    cmdText += String.Format("({0} {1})", sc.PropertyName, DaoSQLServerDBHelper.TranslateOperator(sc.Operator));
                }
                else
                {
                    string par = String.Format("p{0}", k++);
                    cmdText += String.Format("({0} {1} :{2})", sc.PropertyName, DaoSQLServerDBHelper.TranslateOperator(sc.Operator), par);
                    try
                    {
                        ObjectParameter p = new ObjectParameter(par, ((SingleClause)clause).Value.ToString());
                        //oCmd.Parameters.Add(par, ((SingleClause)clause).Value.ToString());
                    }
                    catch
                    {
                    }
                }
            }
            else if (clause is CombinedClause)
            {
                cmdText += "(";
                CombinedClause cc = clause as CombinedClause;
                for (int j = 0; j < cc.SubClauses.Count() - 1; j++)
                {
                    cmdText += String.Format("{0} {1} ", TranslateIClause(cc.SubClauses.ElementAt(j), oCmd, ref k),
                        cc.ClauseType.ToString().ToUpper());
                }
                cmdText += String.Format("{0})", TranslateIClause(cc.SubClauses.Last(), oCmd, ref k));
            }
            return cmdText;
        }

        private String GetColumn(String propertyName)
        {
            String col = "";
            switch (propertyName)
            {
                case "appCode":
                    col = "App_CODE";
                    break;

                case "mail":
                    col = "MAIL";
                    break;

                case "fax":
                    col = "FAX";
                    break;

                case "telefono":
                    col = "TELEFONO";
                    break;

                case "contactRef":
                    col = "CONTACT_REF";
                    break;

                case "codiceCanale":
                    col = "CODICE";
                    break;

                case "codiceBackend":
                    col = "BACKEND_CODE";
                    break;

                case "descrBackend":
                    col = "BACKEND_DESCR";
                    break;

                case "descrBackendPlus":
                    col = "DESCR_PLUS";
                    break;

                case "categoria":
                    col = "CATEGORY";
                    break;
            }
            return col;
        }

        private ObjectParameter[] MapToOracleParameter(ContactsApplicationMapping map)
        {
            ObjectParameter[] pars = new ObjectParameter[4];
            pars[0] = new ObjectParameter("pID_CANALE", map.IdCanale);
            pars[1] = new ObjectParameter("pID_BACKEND", map.IdBackend);
            pars[2] = new ObjectParameter("pID_CONTATTO", map.IdContact);
            pars[3] = new ObjectParameter("pID_TITOLO", map.IdTitolo);
            return pars;
        }

        #endregion

        #region "Private methods"

               


        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Dispose della classe.
        /// </summary>
        public void Dispose()
        {
            
        }

        #endregion
    }
}
