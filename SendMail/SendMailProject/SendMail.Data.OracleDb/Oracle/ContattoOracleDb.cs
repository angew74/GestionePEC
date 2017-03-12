using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMailApp.OracleCore.Contracts;
using SendMail.DataContracts.Interfaces;
using SendMail.Data.OracleDb;
using SendMailApp.OracleCore.Oracle.GestioneViste;
using Oracle.DataAccess.Client;
using System.Data;
using SendMail.Model.RubricaMapping;
using SendMail.Data.Utilities;
using SendMailApp.OracleCore.Oracle.OrderedQuery;
using SendMail.Model.Wrappers;
using ActiveUp.Net.Mail.DeltaExt;
using log4net;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.OracleDb
{
    public class ContattoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IContattoDao
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ContattoOracleDb));

        #region statements

        private const String updateStatement = " UPDATE RUBR_CONTATTI SET MAIL = :pMAIL, FAX=:pFAX, TELEFONO = :pTELEFONO, FLG_PEC=:pFLGPEC, SOURCE=:pSOURCE, NOTE=:pNOTE, CONTACT_REF=:pCONTACT_REF  WHERE " +
           " ID_CONTACT = :pIDCONTACT ";
        private const String insertStatement = " INSERT INTO RUBR_CONTATTI (MAIL,FAX,TELEFONO,REF_ID_REFERRAL,FLG_PEC,SOURCE,NOTE,CONTACT_REF,FLG_IPA) VALUES (:pMAIL,:pFAX,:pTELEFONO,:pREFIDREFERRAL,:pFLGPEC,:pSOURCE,:pNOTE,:pCONTACT_REF,0) RETURNING ID_CONTACT INTO :pIDCONTACT";

        private const string deleteStatement = null;
        private const string deleteLogicStatement = null;
        private const string selectAll = null;
        private const string selectById = null;
        private const string selectOrgContactsByUfficioId = null;
        private const string selectOrgContactsByUfficioName = null;

        #endregion

        #region "C.tor"

        private OracleSessionManager context;
        public ContattoOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            this.context = daoContext;
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

        #endregion

        #region IContattoDao Membri di

        public IList<SendMail.Model.RubricaMapping.RubricaContatti> LoadContattiOrgByOrgId(long identita, bool startFromOrg, bool includeDescendant, bool includeIPA, bool includeAppMappings)
        {
            //commentato per oracle 10
            //V_Rubr_Contatti_Obj v = new V_Rubr_Contatti_Obj(this.context);
            //return v.ConvertToRubricaContatti(v.GetContattiByIdOrg((long)IdEntita));

            string cmdBase = "SELECT DISTINCT * FROM RUBR_CONTATTI WHERE REF_ID_REFERRAL IN ({0})";
            string cmdText = "";
            if (startFromOrg)
            {
                cmdText = String.Format(cmdBase, "SELECT ID_REFERRAL FROM RUBR_ENTITA WHERE " + identita + " IN (ID_REFERRAL, REF_ORG)");
            }
            else
            {
                cmdText = String.Format(cmdBase, identita);
            }

            if (includeDescendant)
            {
                cmdText += " OR REF_ID_REFERRAL IN (SELECT ID_REFERRAL FROM RUBR_ENTITA WHERE ID_PADRE = " + identita + ")";
            }

            if (includeIPA)
            {
                //non gestito
            }

            List<RubricaContatti> lContatti = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdText;

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {

                        if (r.HasRows)
                        {
                            lContatti = new List<RubricaContatti>();

                            while (r.Read())
                            {
                                lContatti.Add(DaoOracleDbHelper.MapToRubricaContatti(r));
                            }
                        }
                    }


                    if (lContatti != null)
                    {
                        lContatti.ForEach(x =>
                        {
                            if (x.RefIdReferral.HasValue)
                                x.Entita = this.Context.DaoImpl.RubricaEntitaDao.GetById(x.RefIdReferral.Value);
                        });
                        if (includeAppMappings)
                        {
                            lContatti.ForEach(x =>
                            {
                                x.MappedAppsId = new List<long>();
                                List<SendMail.Model.ContactsApplicationMapping> lAM = (List<SendMail.Model.ContactsApplicationMapping>)
                                    this.Context.DaoImpl.ContactsApplicationMappingDao.GetContactsByIdContact(x.IdContact.Value);
                                if (lAM != null)
                                    x.MappedAppsId.AddRange(lAM.Select(a => a.IdTitolo).Distinct());
                            });
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return lContatti;
        }

        public IList<SendMail.Model.RubricaMapping.RubricaContatti> LoadContattiOrgByName(string identita, bool startFromOrg, bool includeDescendant, bool includeIPA)
        {
            //commentata per oracle 10
            //V_Rubr_Contatti_Obj v = new V_Rubr_Contatti_Obj(this.context);
            //return v.ConvertToRubricaContatti(v.GetContattiOrgByName(nomeEntita));

            List<RubricaContatti> lCont = null;

            List<SendMail.Model.EntitaType> tEnt = new List<SendMail.Model.EntitaType>();
            tEnt.Add(SendMail.Model.EntitaType.ALL);

            try
            {
                List<RubricaEntita> lEntita = this.Context.DaoImpl.RubricaEntitaDao.LoadEntitaByName(tEnt, identita);

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
                //TASK: Allineamento log - Ciro
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
            StringBuilder sb = new StringBuilder("with t_tit(id_tit) as (")
                .AppendLine("select id_titolo")
                .AppendLine("from comunicazioni_titoli")
                .AppendLine("where app_code = :p_titolo),")
                .AppendLine("t_back(id_back) as (")
                .AppendLine("select id_backend")
                .AppendLine("from rubr_backend")
                .AppendLine("where backend_code = :p_backend_code),")
                .AppendLine("t_predef(id_cont) as (")
                .AppendLine("select ref_id_contatto")
                .AppendLine("from rubr_contatti_backend")
                .AppendLine("where ref_id_titolo = (select id_tit from t_tit)")
                .AppendLine("and ref_id_backend in (select id_back from t_back)),")
                .AppendLine("t_ent_mapped(id_ent) as (")
                .AppendLine("select distinct ref_id_entita")
                .AppendLine("from rubr_contatti_backend")
                .AppendLine("where ref_id_backend in (select id_back from t_back)")
                .AppendLine("and ref_id_entita is not null),")
                .AppendLine("t_ent(id_ent, id_pad) as (")
                .AppendLine("select id_referral, id_padre")
                .AppendLine("from rubr_entita")
                .AppendLine("where id_referral = (select nvl(ref_org, id_referral)")
                .AppendLine("                     from rubr_entita")
                .AppendLine("                     where id_referral in (select id_ent")
                .AppendLine("                                           from t_ent_mapped)")
                .AppendLine("                    )")
                .AppendLine("union all")
                .AppendLine("select id_referral, id_padre")
                .AppendLine("from rubr_entita, t_ent")
                .AppendLine("where id_padre = id_ent)")
                .AppendLine("select id_contact, ref_id_referral, mail,")
                .AppendLine("case")
                .AppendLine("  when (select count(*) from t_predef where id_cont = id_contact) != 0 then 1")
                .AppendLine("  else 0")
                .AppendLine("end as is_predef")
                .AppendLine("from rubr_contatti")
                .AppendLine("where ref_id_referral in (select id_ent")
                .AppendLine("                          from t_ent)");

            List<RubricaContatti> list = null;
            using (OracleCommand cmd = CurrentConnection.CreateCommand())
            {
                cmd.CommandText = sb.ToString();
                cmd.BindByName = true;
                cmd.Parameters.Add("p_titolo", titolo);
                cmd.Parameters.Add("p_backend_code", backendCode);
                try
                {
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                            list = new List<RubricaContatti>();
                        while (rd.Read())
                        {
                            list.Add(new RubricaContatti
                            {
                                IdContact = rd.GetInt64("id_contact"),
                                RefIdReferral = rd.GetInt64("ref_id_referral"),
                                Mail = rd.GetString("mail"),
                                T_isMappedAppDefault = Convert.ToBoolean(rd.GetDecimal("is_predef"))
                            });
                        }
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

            //commentato oracle 10
            //V_Rubr_Contatti_Obj v = new V_Rubr_Contatti_Obj(this.context);
            //ResultList<RubricaContatti> res =
            //    v.ConvertToRubricaContatti(v.GetContattiByParams(tEnt, pars, da, per, withEntita));

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
                ResultList<RubricaEntita> rE =
                    this.Context.DaoImpl.RubricaEntitaDao.LoadEntitaByParams((IList<SendMail.Model.EntitaType>)tEnt,
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
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT * FROM RUBR_CONTATTI WHERE";

                    List<string> where = new List<string>();
                    foreach (var kvp in cPars)
                    {
                        switch (kvp.Key)
                        {
                            case SendMail.Model.FastIndexedAttributes.FAX:
                                where.Add(string.Format(" FAX IN ('{0}')",
                                    string.Join("','", kvp.Value.ToArray())));
                                break;

                            case SendMail.Model.FastIndexedAttributes.MAIL:
                                where.Add(string.Format(" MAIL IN ('{0}')",
                                    string.Join("','", kvp.Value.ToArray())));
                                break;

                            case SendMail.Model.FastIndexedAttributes.TELEFONO:
                                where.Add(string.Format(" TELEFONO IN ('{0}')",
                                    string.Join("','", kvp.Value.ToArray())));
                                break;

                            default:
                                break;
                        }
                    }

                    if (where.Count != 0)
                    {
                        oCmd.CommandText += String.Join(" AND ", where.ToArray());
                    }

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lContatti = new List<RubricaContatti>();
                            while (r.Read())
                            {
                                lContatti.Add(DaoOracleDbHelper.MapToRubricaContatti(r));
                            }
                        }
                    }

                    if (lContatti != null)
                    {
                        lContatti.ForEach(c => c.Entita = this.Context.DaoImpl.RubricaEntitaDao.GetById(c.RefIdReferral.Value));
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

            string queryCountBase = "SELECT count(*) from ({0})";

            string queryRubrica = "SELECT distinct r.RAGIONE_SOCIALE AS rag_soc"
                                + ", r.DISAMB_PRE as prefix"
                                + ", r.DISAMB_POST as suffix"
                                + ", {0} as descr"
                                + ", LISTAGG(r.ID_REFERRAL, ';') within group (order by r.ID_REFERRAL) over (partition by NVL(r.DISAMB_PRE,' ')||r.RAGIONE_SOCIALE||NVL(r.DISAMB_POST,' ')) AS ids" //(partition by {0})
                                + ", 'R' as SRC"
                                + ", REFERRAL_TYPE as subtype"
                                + " FROM rubr_entita r {1}"
                                + " WHERE {2}"
                                + " order by 1";
            string campi = "";
            switch (par.Key)
            {
                case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                case SendMail.Model.FastIndexedAttributes.COGNOME:
                case SendMail.Model.FastIndexedAttributes.FAX:
                case SendMail.Model.FastIndexedAttributes.MAIL:
                case SendMail.Model.FastIndexedAttributes.TELEFONO:
                case SendMail.Model.FastIndexedAttributes.UFFICIO:
                    campi += par.Key.ToString();
                    break;
                default:
                    throw new ArgumentException("Parametro non implementato");
            }

            string innerJoin = "";
            if (par.Key.Equals(SendMail.Model.FastIndexedAttributes.FAX) ||
                par.Key.Equals(SendMail.Model.FastIndexedAttributes.MAIL) ||
                par.Key.Equals(SendMail.Model.FastIndexedAttributes.TELEFONO))
            {
                innerJoin = "INNER JOIN rubr_contatti c ON c.REF_ID_REFERRAL = r.id_referral";
            }

            string whereConds = null;

            if (!tEnt.Contains(SendMail.Model.EntitaType.ALL) && !tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN))
            {
                whereConds += "REFERRAL_TYPE in (";
                whereConds += String.Join(", ", tEnt.Select(t => String.Format("'{0}'", t.ToString())).ToArray());
                whereConds += ") and ";
            }

            whereConds += "length(" + par.Key.ToString() + ") >= " + par.Value.Length + " and ";

            switch (par.Key)
            {
                case SendMail.Model.FastIndexedAttributes.COGNOME:
                case SendMail.Model.FastIndexedAttributes.MAIL:
                case SendMail.Model.FastIndexedAttributes.UFFICIO:
                case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                    whereConds += "lower(" + par.Key.ToString() + ") like '%" + par.Value.ToLower() + "%'";
                    break;
                case SendMail.Model.FastIndexedAttributes.FAX:
                case SendMail.Model.FastIndexedAttributes.TELEFONO:
                    whereConds += par.Key.ToString() + " like '%" + par.Value.ToLower() + "%'";
                    break;
                default:
                    throw new ArgumentException("Parametro non implementato");
            }


            string query = string.Format(queryRubrica, campi, innerJoin, whereConds);
            string queryCount = String.Format(queryCountBase, query);

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                //count
                int tot = 0;
                oCmd.CommandText = queryCount;
                try
                {
                    tot = Convert.ToInt32(oCmd.ExecuteScalar());
                    res.Per = (tot > per) ? per : tot;
                    res.Totale = tot;
                }
                catch
                {
                    tot = 0;
                    res.List = null;
                }

                if (tot > 0)
                {
                    if (per > 0) { oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per); }
                    else { oCmd.CommandText = query; }
                    try
                    {
                        using (OracleDataReader r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                res.List = new List<SimpleResultItem>();
                                while (r.Read())
                                {
                                    res.List.Add(
                                        new SimpleResultItem(
                                            r.GetValue("descr").ToString(),
                                            r.GetValue("ids").ToString(),
                                            String.Format("{0} {1} {2}", r.GetValue("prefix"), r.GetValue("rag_soc"), r.GetValue("suffix")),
                                            r.GetValue("subtype").ToString(),
                                            r.GetValue("SRC").ToString(),
                                            100));

                                }
                            }
                        }
                    }
                    catch
                    {
                        res.List = null;
                    }
                }
                else if ((par.Key != SendMail.Model.FastIndexedAttributes.FAX) &&
                    (par.Key != SendMail.Model.FastIndexedAttributes.TELEFONO))
                {
                    res = LoadSimilarityFieldsByParams(ctg, tEnt, par, 1, per);
                }
            }

            return res;
        }

        public ResultList<SimpleResultItem> LoadSimilarityFieldsByParams(SendMail.Model.IndexedCatalogs ctg, IList<SendMail.Model.EntitaType> tEnt, KeyValuePair<SendMail.Model.FastIndexedAttributes, string> par, int da, int per)
        {
            if (String.IsNullOrEmpty(par.Value)) return null;
            if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }

            ResultList<SimpleResultItem> res = new ResultList<SimpleResultItem>();
            res.Da = da;
            res.Per = per;
            res.Totale = per;

            string queryRubrica = "SELECT distinct r.RAGIONE_SOCIALE AS rag_soc"
                                + ", {0} as descr"
                                + ", r.DISAMB_PRE as prefix"
                                + ", r.DISAMB_POST as suffix"
                                + ", id_referral AS ids"
                                + ", 'R' as SRC"
                                + ", REFERRAL_TYPE as subtype"
                                + ", utl_match.edit_distance_similarity('{1}', lower({0})) AS sim"
                                + " FROM rubr_entita r {2}"
                                + " WHERE {3}"
                                + " order by 8 desc, 1";
            string campi = "";
            switch (par.Key)
            {
                case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                case SendMail.Model.FastIndexedAttributes.COGNOME:
                case SendMail.Model.FastIndexedAttributes.FAX:
                case SendMail.Model.FastIndexedAttributes.MAIL:
                case SendMail.Model.FastIndexedAttributes.TELEFONO:
                case SendMail.Model.FastIndexedAttributes.UFFICIO:
                    campi += par.Key.ToString();
                    break;
                default:
                    throw new ArgumentException("Parametro non implementato");
            }

            string innerJoin = "";
            if (par.Key.Equals(SendMail.Model.FastIndexedAttributes.FAX) ||
                par.Key.Equals(SendMail.Model.FastIndexedAttributes.MAIL) ||
                par.Key.Equals(SendMail.Model.FastIndexedAttributes.TELEFONO))
            {
                innerJoin = "INNER JOIN rubr_contatti c ON c.REF_ID_REFERRAL = r.id_referral";
            }

            string whereConds = null;

            if (!tEnt.Contains(SendMail.Model.EntitaType.ALL) && !tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN))
            {
                whereConds += "REFERRAL_TYPE in (";
                whereConds += String.Join(", ", tEnt.Select(t => String.Format("'{0}'", t.ToString())).ToArray());
                whereConds += ") and ";
            }

            whereConds += "length(" + par.Key.ToString() + ") >= " + par.Value.Length;

            string query = string.Format(queryRubrica, campi, par.Value, innerJoin, whereConds);

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                if (per > 0) { oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per); }
                else { oCmd.CommandText = query; }

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            res.List = new List<SimpleResultItem>();
                            while (r.Read())
                            {
                                res.List.Add(
                                    new SimpleResultItem(
                                        r.GetValue("descr").ToString(),
                                        r.GetValue("ids").ToString(),
                                        String.Format("{0} {1} {2}", r.GetValue("prefix"), r.GetValue("rag_soc"), r.GetValue("suffix")),
                                        r.GetValue("subtype").ToString(),
                                        r.GetValue("SRC").ToString(),
                                        Convert.ToInt64(r.GetValue("sim"))));

                            }
                        }
                    }
                }
                catch
                {
                    res.List = null;
                }

            }

            return res;
        }

        #endregion

        #region IDao<RubricaContatti,long> Membri di

        public ICollection<SendMail.Model.RubricaMapping.RubricaContatti> GetAll()
        {
            throw new NotImplementedException();
        }

        public SendMail.Model.RubricaMapping.RubricaContatti GetById(long id)
        {
            V_Rubr_Contatti_Obj rubrObj = new V_Rubr_Contatti_Obj(context);
            return (RubricaContatti)rubrObj.GetContattiById(id);
        }

        public void Insert(SendMail.Model.RubricaMapping.RubricaContatti entity)
        {
            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                base.Context.StartTransaction(this.GetType());

                if (entity.Entita != null)
                {
                    try
                    {
                        this.Context.DaoImpl.RubricaEntitaDao.Insert(entity.Entita);
                        entity.RefIdReferral = entity.Entita.IdReferral;
                    }
                    catch
                    {
                        base.Context.RollBackTransaction(this.GetType());
                        throw;
                    }
                }

                ocmd.CommandText = insertStatement;
                ocmd.BindByName = true;
                ocmd.Parameters.Clear();
                ocmd.Parameters.AddRange(MapObjectToParams(entity, true));

                try
                {
                    int ret = ocmd.ExecuteNonQuery();
                    entity.IdContact = long.Parse(ocmd.Parameters["pIDCONTACT"].Value.ToString());
                }
                catch (Exception ex)
                {
                    base.Context.RollBackTransaction(this.GetType());
                    //TASK: Allineamento log - Ciro
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

                base.Context.EndTransaction(this.GetType());
            }
        }

        public void Update(SendMail.Model.RubricaMapping.RubricaContatti entity)
        {
            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                ocmd.CommandText = updateStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(entity, false));
                ocmd.BindByName = true;
                ocmd.Connection = base.CurrentConnection;
                try
                {
                    int ret = ocmd.ExecuteNonQuery();
                }
                catch
                {
                    //todo
                    throw;
                }
            }
        }

        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region mapping

        private OracleParameter[] MapObjectToParams(RubricaContatti r, bool isInsert)
        {
            OracleParameter[] oparams;
            if (isInsert)
                oparams = new OracleParameter[9];
            else oparams = new OracleParameter[8];

            oparams[0] = new OracleParameter("pMAIL", OracleDbType.NVarchar2, r.Mail, ParameterDirection.Input);
            oparams[1] = new OracleParameter("pTELEFONO", OracleDbType.NVarchar2, r.Telefono, ParameterDirection.Input);
            oparams[2] = new OracleParameter("pFAX", OracleDbType.NVarchar2, r.Fax, ParameterDirection.Input);
            oparams[3] = new OracleParameter("pFLGPEC", OracleDbType.Int64, Convert.ToInt64(r.IsPec), ParameterDirection.Input);
            oparams[4] = new OracleParameter("pSOURCE", OracleDbType.NVarchar2, r.Source, ParameterDirection.Input);
            oparams[5] = new OracleParameter("pNOTE", OracleDbType.NVarchar2, r.Note, ParameterDirection.Input);
            oparams[6] = new OracleParameter("pCONTACT_REF", OracleDbType.NVarchar2, r.ContactRef, ParameterDirection.Input);
            if (isInsert)
            {
                oparams[7] = new OracleParameter("pIDCONTACT", OracleDbType.Int64, r.IdContact, ParameterDirection.Output);
                oparams[8] = new OracleParameter("pREFIDREFERRAL", OracleDbType.Int64, r.RefIdReferral, ParameterDirection.Input);
            }
            else
                oparams[7] = new OracleParameter("pIDCONTACT", OracleDbType.Int64, r.IdContact, ParameterDirection.Input);

            return oparams;
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #endregion
    }
}
