using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Data;
namespace SendMail.Data.SQLServerDB.Repository
{
    public class RubrEntitaSQLDb : IRubricaEntitaDao
    {
        private static readonly ILog log = LogManager.GetLogger("RubrEntitaSQLDb");

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region Costanti

        private const string selectRubricaEntitaTree = "WITH T_TREE_C(ID_REF, REF_ORG, LIV) AS"
                                                        + " (SELECT ID_REFERRAL, REF_ORG, LEVEL"
                                                         + " FROM RUBR_ENTITA"
                                                         + " CONNECT BY NOCYCLE ID_REFERRAL = PRIOR ID_PADRE"
                                                         + " START WITH ID_REFERRAL = :p_ID_ENT),"
                                                         + " T_TREE(ID_REF, LIV) AS"
                                                         + " (SELECT ID_REF, LIV"
                                                         + " FROM T_TREE_C"
                                                         + " WHERE REF_ORG IS NOT NULL"
                                                         + " OR (REF_ORG IS NULL AND LIV = (SELECT MIN(LIV)"
                                                                                        + " FROM T_TREE_C"
                                                                                        + " WHERE REF_ORG IS NULL)))"
                                                   + " SELECT DISTINCT"
                                                   + " ID_REFERRAL as \"ID_REF\","
                                                   + " ID_PADRE AS \"ID_PAD\","
                                                   + " DECODE((SELECT COUNT(*) FROM RUBR_ENTITA WHERE ID_PADRE = RE.ID_REFERRAL), 0, 0, 1) AS \"IS_PADRE\","
                                                   + " CASE"
                                                       + " WHEN REFERRAL_TYPE IN ('AZ_PF', 'AZ_UFF_PF', 'PA_PF','PA_UFF_PF', 'PF', 'PG')"
                                                           + " THEN COGNOME||' '||NOME"
                                                       + " WHEN REFERRAL_TYPE IN ('AZ_UFF', 'PA_UFF') THEN UFFICIO"
                                                       + " WHEN DISAMB_PRE IS NOT NULL THEN TRIM(DISAMB_PRE||' '||RAGIONE_SOCIALE||' '||DISAMB_POST)"
                                                       + " ELSE TRIM(RAGIONE_SOCIALE||' '||DISAMB_POST)"
                                                       + " END AS \"RAG_SOC\","
                                                   + " 'RUBR' AS \"SRC\","
                                                   + " REFERRAL_TYPE AS \"REF_TYP\""
                                                   + " FROM RUBR_ENTITA RE"
                                                   + " WHERE ID_REFERRAL IN (SELECT ID_REF"
                                                                         + " FROM T_TREE)"
                                                       + " OR ID_PADRE IN (SELECT ID_REF"
                                                                       + " FROM T_TREE"
                                                                       + " WHERE LIV > 1)";

        #endregion

        #region IRubricaEntitaDao Membri di

        public ResultList<RubricaEntita> LoadEntitaByParams(IList<SendMail.Model.EntitaType> tEnt,
            IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        {
            if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.UNKNOWN); }
            tEnt = tEnt.Where(te => te != EntitaType.ALL).DefaultIfEmpty(EntitaType.UNKNOWN).ToList();

            ResultList<RubricaEntita> res = new ResultList<RubricaEntita>();
            res.Da = da;

            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var matches = dbcontext.RUBR_ENTITA.Select(f => f);
                    if (pars != null && pars.Count != 0)
                    {
                        for (int i = 0; i < pars.Count; i++)
                        {
                            KeyValuePair<SendMail.Model.FastIndexedAttributes, IList<string>> p = pars.ElementAt(i);
                            if (p.Value == null || p.Value.Count == 0)
                            {
                                throw new ArgumentException("Parametri non validi");
                            }
                            switch (p.Key)
                            {
                                case SendMail.Model.FastIndexedAttributes.COGNOME:
                                    string[] vals = p.Value.ToArray();
                                    matches = matches.Where(x => vals.Contains(x.COGNOME.ToLower()) || vals.Contains(x.NOME.ToLower())).OrderBy(z => z.COGNOME).OrderBy(z => z.NOME);
                                    break;
                                case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                                    string[] vals1 = p.Value.ToArray();
                                    matches = matches.Where(x => vals1.Contains(x.DISAMB_PRE.ToLower()) || vals1.Contains(x.DISAMB_POST.ToLower()) || vals1.Contains(x.RAGIONE_SOCIALE)).OrderBy(z => z.DISAMB_PRE).OrderBy(z => z.DISAMB_POST).OrderBy(z => z.RAGIONE_SOCIALE);
                                    break;
                                case SendMail.Model.FastIndexedAttributes.UFFICIO:
                                    string[] vals2 = p.Value.ToArray();
                                    matches = matches.Where(x => vals2.Contains(x.UFFICIO.ToLower())).OrderBy(z => z.RAGIONE_SOCIALE).OrderBy(z => z.UFFICIO);
                                    break;

                                default:
                                    throw new NotImplementedException("Tipo di rircerca non implementato");
                            }
                        }
                    }
                    if (!(tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN)))
                    {
                        string[] ent = tEnt.Select(c => c.ToString()).ToArray();
                        matches = matches.Where(x => ent.Contains(x.REFERRAL_TYPE));
                    }

                    int tRig = matches.ToList().Count();
                    if (tRig > 0)
                    {
                        int Per = (tRig > per) ? per : tRig;
                        var tabkes = matches.ToList().Take(Per);
                        res.List = new List<RubricaEntita>();
                        res.Per = Per;
                        res.Totale = tRig;
                        foreach (RUBR_ENTITA r in tabkes)
                        {
                            res.List.Add(AutoMapperConfiguration.MapToRubrEntita(r));
                        }
                    }
                }
                catch (Exception e)
                {
                    res = null;
                    throw;
                }
            }
            return res;


        }
        /// NON SO SE SERVE 
        //public ResultList<RubricaEntita> LoadSimilarityEntitaByParams(IList<SendMail.Model.EntitaType> tEnt,
        //    IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        //{
        //    if (pars == null || pars.Count == 0) return null;

        //    if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
        //    if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }

        //    if (pars.Any(x => x.Value == null))
        //    {
        //        throw new ArgumentException("Parametri non validi");
        //    }

        //    ResultList<RubricaEntita> res = new ResultList<RubricaEntita>();
        //    res.Da = da;
        //    res.Per = per;

        //    int tot = pars.SelectMany(c => c.Value).Count();
        //    string[] matchPars = new string[tot];
        //    string utlBase = "utl_match.jaro_winkler_similarity('{0}', {1})";
        //    string orderby = null;

        //    for (int i = 0; i < pars.Count; i++)
        //    {
        //        KeyValuePair<SendMail.Model.FastIndexedAttributes, IList<string>> p = pars.ElementAt(i);
        //        if (p.Value == null || p.Value.Count == 0)
        //        {
        //            throw new ArgumentException("Parametri non validi");
        //        }

        //        string qPar = null;

        //        switch (p.Key)
        //        {
        //            case SendMail.Model.FastIndexedAttributes.COGNOME:
        //                qPar = "nvl2(cognome, lower(cognome||' '||nome), lower(nome))";
        //                break;

        //            case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
        //                qPar = "nvl2(disamb_pre, lower(disamb_pre||' '||ragione_sociale||' '||disamb_post), lower(ragione_sociale))";
        //                break;

        //            case SendMail.Model.FastIndexedAttributes.UFFICIO:
        //                qPar = "ufficio";
        //                break;

        //            default:
        //                throw new NotImplementedException("Tipo di rircerca non implementato");
        //        }

        //        for (int j = 0; j < p.Value.Count; j++)
        //        {
        //            int idx = Array.FindIndex<string>(matchPars, x => String.IsNullOrEmpty(x));
        //            if (idx != -1)
        //            {
        //                matchPars[idx] = String.Format(utlBase, p.Value[j], qPar);
        //                if (String.IsNullOrEmpty(orderby))
        //                {
        //                    orderby += String.Format("{0} desc", (idx + 1));
        //                }
        //                else
        //                {
        //                    orderby += String.Format(", {0} desc", (idx + 1));
        //                }
        //            }
        //        }
        //    }

        //    string where = null;
        //    if (!(tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN)))
        //    {
        //        where = "WHERE REFERRAL_TYPE IN (" + String.Join(", ", tEnt.Select(c => String.Format("'{0}'", c.ToString())).ToArray()) + ")";
        //    }

        //    string query = String.Format(selectSimilarityEntitaBaseQuery,
        //        String.Join(", ", matchPars), where, orderby);

        //    string complOrderBy = null;
        //    switch (tEnt[0])
        //    {
        //        case SendMail.Model.EntitaType.ALL:
        //        case SendMail.Model.EntitaType.PA:
        //        case SendMail.Model.EntitaType.PA_SUB:
        //        case SendMail.Model.EntitaType.AZ_PRI:
        //        case SendMail.Model.EntitaType.AZ_PS:
        //            complOrderBy = " order by ragione_sociale asc nulls last";
        //            break;
        //        case SendMail.Model.EntitaType.PA_UFF_PF:
        //        case SendMail.Model.EntitaType.AZ_UFF_PF:
        //        case SendMail.Model.EntitaType.AZ_PF:
        //        case SendMail.Model.EntitaType.PA_PF:
        //        case SendMail.Model.EntitaType.PF:
        //        case SendMail.Model.EntitaType.PG:
        //            complOrderBy = " order by cognome, nome asc nulls last";
        //            break;
        //        case SendMail.Model.EntitaType.PA_UFF:
        //        case SendMail.Model.EntitaType.AZ_UFF:
        //            complOrderBy = " order by ufficio asc nulls last";
        //            break;
        //        case SendMail.Model.EntitaType.GRP:
        //        case SendMail.Model.EntitaType.UNKNOWN:
        //            break;
        //        default:
        //            throw new ArgumentException("Caso non implementato");
        //    }

        //    try
        //    {
        //        using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
        //        {
        //            if (per > 0)
        //            {
        //                oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per);
        //            }
        //            else
        //            {
        //                oCmd.CommandText = query;
        //            }

        //            oCmd.CommandText += complOrderBy;

        //            using (OracleDataReader r = oCmd.ExecuteReader())
        //            {
        //                if (r.HasRows)
        //                {
        //                    res.List = new List<RubricaEntita>();

        //                    while (r.Read())
        //                    {
        //                        res.List.Add(DaoOracleDbHelper.MapToRubricaEntita(r));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        res = null;
        //        throw;
        //    }

        //    return res;
        //}

        public SendMail.Model.RubricaMapping.RubricaEntita LoadEntitaCompleteById(long idEntita)
        {
            RubricaEntita res = null;
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    var entita = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == idEntita).First();
                    res = AutoMapperConfiguration.MapToRubrEntita(entita);
                    ContattoSQLDb d = new ContattoSQLDb();
                    long id = res.IdReferral.Value;
                    res.Contatti = d.LoadContattiOrgByOrgId(id, false, true, false, true);

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return res;
        }

        public IList<SimpleTreeItem> LoadRubricaEntitaTree(Int64 idEntita)
        {
            IList<SimpleTreeItem> entitaTree = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        string selectRubricaEntitaTree = "WITH T_TREE_C(ID_REF, REF_ORG, LIV) AS"
                                                       + " (SELECT ID_REFERRAL, REF_ORG, LEVEL"
                                                        + " FROM RUBR_ENTITA"
                                                        + " CONNECT BY NOCYCLE ID_REFERRAL = PRIOR ID_PADRE"
                                                        + " START WITH ID_REFERRAL = " + idEntita + "),"
                                                        + " T_TREE(ID_REF, LIV) AS"
                                                        + " (SELECT ID_REF, LIV"
                                                        + " FROM T_TREE_C"
                                                        + " WHERE REF_ORG IS NOT NULL"
                                                        + " OR (REF_ORG IS NULL AND LIV = (SELECT MIN(LIV)"
                                                                                       + " FROM T_TREE_C"
                                                                                       + " WHERE REF_ORG IS NULL)))"
                                                  + " SELECT DISTINCT"
                                                  + " ID_REFERRAL as \"ID_REF\","
                                                  + " ID_PADRE AS \"ID_PAD\","
                                                  + " DECODE((SELECT COUNT(*) FROM RUBR_ENTITA WHERE ID_PADRE = RE.ID_REFERRAL), 0, 0, 1) AS \"IS_PADRE\","
                                                  + " CASE"
                                                      + " WHEN REFERRAL_TYPE IN ('AZ_PF', 'AZ_UFF_PF', 'PA_PF','PA_UFF_PF', 'PF', 'PG')"
                                                          + " THEN COGNOME||' '||NOME"
                                                      + " WHEN REFERRAL_TYPE IN ('AZ_UFF', 'PA_UFF') THEN UFFICIO"
                                                      + " WHEN DISAMB_PRE IS NOT NULL THEN TRIM(DISAMB_PRE||' '||RAGIONE_SOCIALE||' '||DISAMB_POST)"
                                                      + " ELSE TRIM(RAGIONE_SOCIALE||' '||DISAMB_POST)"
                                                      + " END AS \"RAG_SOC\","
                                                  + " 'RUBR' AS \"SRC\","
                                                  + " REFERRAL_TYPE AS \"REF_TYP\""
                                                  + " FROM RUBR_ENTITA RE"
                                                  + " WHERE ID_REFERRAL IN (SELECT ID_REF"
                                                                        + " FROM T_TREE)"
                                                      + " OR ID_PADRE IN (SELECT ID_REF"
                                                                      + " FROM T_TREE"
                                                                      + " WHERE LIV > 1)";
                        using (var r = oCmd.ExecuteReader())
                        {
                            entitaTree = new List<SimpleTreeItem>();
                            while (r.Read())
                            {
                                entitaTree.Add(MapToSimpleTreeItem(r));
                            }
                        }

                        if (entitaTree.Count == 0) entitaTree = null;
                    }
                }
            }
            catch
            {
                throw;
            }
            return entitaTree;
        }

        public IList<SimpleTreeItem> LoadRubricaEntitaTreeByIdPadre(Int64 idPadre)
        {
            IList<SimpleTreeItem> entitaTree = null;
            using (var dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    try
                    {

                        string selectRubricaEntitaByPadre = "SELECT DISTINCT"
                                                                             + " ID_REFERRAL as \"ID_REF\","
                                                                             + " ID_PADRE AS \"ID_PAD\","
                                                                             + " DECODE((SELECT COUNT(*) FROM RUBR_ENTITA WHERE ID_PADRE = RE.ID_REFERRAL), 0, 0, 1) AS \"IS_PADRE\","
                                                                             + " CASE"
                                                                                 + " WHEN REFERRAL_TYPE IN ('AZ_PF', 'AZ_UFF_PF', 'PA_PF','PA_UFF_PF', 'PF', 'PG')"
                                                                                     + " THEN COGNOME||' '||NOME"
                                                                                 + " WHEN REFERRAL_TYPE IN ('AZ_UFF', 'PA_UFF') THEN UFFICIO"
                                                                                 + " WHEN DISAMB_PRE IS NOT NULL THEN TRIM(DISAMB_PRE||' '||RAGIONE_SOCIALE||' '||DISAMB_POST)"
                                                                                 + " ELSE TRIM(RAGIONE_SOCIALE||' '||DISAMB_POST)"
                                                                             + " END AS \"RAG_SOC\","
                                                                             + " 'RUBR' AS \"SRC\","
                                                                             + " REFERRAL_TYPE AS \"REF_TYP\""
                                                                             + " FROM RUBR_ENTITA RE"
                                                                             + " WHERE ID_PADRE = " + idPadre
                                                                             + " ORDER BY RAG_SOC";

                        using (var r = oCmd.ExecuteReader())
                        {
                            entitaTree = new List<SimpleTreeItem>();
                            while (r.Read())
                            {
                                entitaTree.Add(MapToSimpleTreeItem(r));
                            }
                        }

                        if (entitaTree.Count == 0) entitaTree = null;
                    }
                    catch (Exception excp)
                    {
                        if (excp.GetType() != typeof(ManagedException))
                        {
                            ManagedException mEx = new ManagedException(excp.Message,
                                "RUB_ORA005",
                                string.Empty,
                                string.Empty,
                                excp);
                            ErrorLogInfo er = new ErrorLogInfo(mEx);
                            log.Error(er);
                            throw mEx;
                        }
                        else throw excp;
                    }
                }
            }
            return entitaTree;
        }

        public ResultList<RubricaEntita> LoadEntitaByMailDomain(IList<EntitaType> tEnt, string mail, int da, int per)
        {

            ResultList<RubricaEntita> r = new ResultList<RubricaEntita>();
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var query = dbcontext.RUBR_ENTITA.AsQueryable();
                    query = dbcontext.RUBR_CONTATTI.Where(x => x.MAIL.ToUpper().Contains(mail.ToUpper())).Select(z => z.RUBR_ENTITA).AsQueryable();
                    if (tEnt != null && tEnt.Count != 0 && !tEnt.Contains(EntitaType.ALL))
                    {
                        string[] wherees = tEnt.Select(e => e.ToString()).ToArray();
                        query.Where(x => wherees.Contains(x.REFERRAL_TYPE));
                    }
                    int tot = Convert.ToInt32(query.Count());
                    r.Da = ((da == 0) ? ++da : da);
                    r.Per = ((tot < per) ? tot : per);
                    r.Totale = tot;
                    var entities = query.Skip(r.Da).Take(r.Per).ToList();
                    r.List = new List<RubricaEntita>();
                    foreach (var e in entities)
                    {
                        RubricaEntita ent = AutoMapperConfiguration.MapToRubrEntita(e);
                        r.List.Add(ent);
                    }
                }
            }
            catch (Exception excp)
            {
                r.List = null;
                if (excp.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(excp.Message,
                        "RUB_ORA001",
                        string.Empty,
                        string.Empty,
                        excp);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                else throw excp;
            }
            return r;
        }

        public ResultList<RubricaEntita> LoadEntitaIPAByMailDomain(string mail, int da, int per)
        {
            throw new NotImplementedException();
            //ResultList<RubricaEntita> re = new ResultList<RubricaEntita>();
            //string query = selectEntitaIPAByMailDomain;
            //string queryCnt = "SELECT COUNT(*) FROM (" + query + ")";

            //using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //{
            //    oCmd.CommandText = queryCnt;
            //    oCmd.BindByName = true;
            //    oCmd.Parameters.Add("p_mailDomain", OracleDbType.Varchar2, mail, ParameterDirection.Input);

            //    try
            //    {
            //        int tot = Convert.ToInt32(oCmd.ExecuteScalar());
            //        re.Da = ((da == 0) ? ++da : da);
            //        re.Per = ((tot < per) ? tot : per);
            //        re.Totale = tot;
            //    }
            //    catch
            //    {
            //        throw;
            //    }

            //    if (re.Totale > 0)
            //    {
            //        oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per);

            //        try
            //        {
            //            using (OracleDataReader rr = oCmd.ExecuteReader())
            //            {
            //                if (rr.HasRows)
            //                {
            //                    re.List = new List<RubricaEntita>();
            //                    while (rr.Read())
            //                    {
            //                        re.List.Add(DaoOracleDbHelper.MapIPAToRubricaEntita(rr));
            //                    }
            //                }
            //            }
            //            if (re.List != null && re.List.Count > 0)
            //            {
            //                ((List<RubricaEntita>)re.List).ForEach(e =>
            //                {
            //                    if (e.ReferralType == EntitaType.UNKNOWN)
            //                    {
            //                        if (String.IsNullOrEmpty(e.Ufficio))
            //                        {
            //                            e.ReferralType = EntitaType.PA;
            //                        }
            //                        else
            //                        {
            //                            e.ReferralType = EntitaType.PA_UFF;
            //                        }
            //                    }
            //                });
            //            }
            //        }
            //        catch (Exception excp)
            //        {
            //            re.List = null;
            //            if (excp.GetType() != typeof(ManagedException))
            //            {
            //                //Allineamento log - Ciro
            //                ManagedException mEx = new ManagedException(excp.Message,
            //                    "RUB_ORA002",
            //                    string.Empty,
            //                    string.Empty,
            //                    excp);
            //                ErrorLogInfo er = new ErrorLogInfo(mEx);
            //                log.Error(er);
            //                throw mEx;
            //            }
            //            else throw excp;
            //        }
            //    }

            //}
            //return re;
        }

        public List<RubricaEntita> LoadEntitaByName(IList<EntitaType> tEnt, string name)
        {
            List<RubricaEntita> lEnt = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var query = dbcontext.RUBR_ENTITA.Where(x => x.RAGIONE_SOCIALE.ToUpper().Contains(name.ToUpper()) || x.COGNOME.ToUpper().Contains(name.ToUpper()) || x.NOME.ToUpper().Contains(name.ToUpper()));
                    if (tEnt != null && tEnt.Count != 0 && !tEnt.Contains(EntitaType.ALL))
                    {
                        string[] wherees = tEnt.Select(e => e.ToString()).ToArray();
                        query.Where(x => wherees.Contains(x.REFERRAL_TYPE));
                    }
                    var entities = query.ToList();
                    foreach (var e in entities)
                    {
                        RubricaEntita ent = AutoMapperConfiguration.MapToRubrEntita(e);
                        lEnt.Add(ent);
                    }
                }
            }
            catch
            {
                throw;
            }

            return lEnt;
        }

        public List<RubricaEntita> LoadEntitaIPAbyIPAid(string IPAid)
        {
            throw new NotImplementedException();
            //V_Rubr_Contatti_Obj obj = new V_Rubr_Contatti_Obj(this.context);
            //List<RubricaEntita> re = obj.GetEntitaIPAbyIPAid(IPAid).Cast<RubricaEntita>().ToList();
            //return re;
        }

        public List<RubricaEntita> LoadEntitaByPartitaIVA(string partitaIVA)
        {
            // V_Rubr_Contatti_Obj obj = new V_Rubr_Contatti_Obj(this.context);
            // List<RubricaEntita> re = obj.GetEntitaByPartitaIVA(partitaIVA).Cast<RubricaEntita>().ToList(); ;
            List<RubricaEntita> lEnt = null;
            try
            {
                using (var dbcontext = new FAXPECContext())
                {
                    var query = dbcontext.RUBR_ENTITA.Where(x => x.P_IVA.ToUpper().Contains(partitaIVA.ToUpper()));
                    var entities = query.ToList();
                    foreach (var e in entities)
                    {
                        RubricaEntita ent = AutoMapperConfiguration.MapToRubrEntita(e);
                        lEnt.Add(ent);
                    }
                }
            }
            catch
            {
                throw;
            }
            return lEnt;

        }
        #endregion

        #region IDao<RubricaEntita,long> Membri di

        public ICollection<SendMail.Model.RubricaMapping.RubricaEntita> GetAll()
        {
            throw new NotImplementedException();
        }

        public SendMail.Model.RubricaMapping.RubricaEntita GetById(long id)
        {
            RubricaEntita re = null;

            try
            {
                using (var dbcontext = new FAXPECContext())
                {

                    var r = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == id).First();
                    re = AutoMapperConfiguration.MapToRubrEntita(r);

                }
            }
            catch
            {
            }

            return re;
        }

        public void Insert(SendMail.Model.RubricaMapping.RubricaEntita entity)
        {
            using (var dbcontext = new FAXPECContext())
            {

                try
                {
                    var rubr = DaoSQLServerDBHelper.MapToRubrEntita(entity, true);
                    dbcontext.RUBR_ENTITA.Add(rubr);
                    int r = dbcontext.SaveChanges();
                    if (r == 1)
                    {
                        entity.IdReferral = (long)rubr.ID_REFERRAL;
                    }
                    else
                    {

                        //Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("Errore inserimento ",
                            "RUB_ORA003",
                            string.Empty,
                            string.Empty,
                            null);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        public void Update(SendMail.Model.RubricaMapping.RubricaEntita entity)
        {
            using (var dbcontext = new FAXPECContext())
            {
                var rubr = dbcontext.RUBR_ENTITA.Where(x => x.ID_REFERRAL == entity.IdReferral).First();
                if (rubr != null)
                {
                    rubr.UFFICIO = entity.Ufficio;
                    rubr.SITO_WEB = entity.SitoWeb;
                    rubr.COGNOME = entity.Cognome;
                    rubr.NOME = entity.Nome;
                    rubr.NOTE = entity.Note;
                    rubr.COD_FIS = entity.CodiceFiscale;
                    rubr.P_IVA = entity.PartitaIVA;
                    try
                    {
                        int ret = dbcontext.SaveChanges();
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }


        public void Delete(long id)
        {
            throw new NotImplementedException();
        }

        private SimpleTreeItem MapToSimpleTreeItem(DbDataReader dr)
        {
            SimpleTreeItem item = new SimpleTreeItem();
            item.Value = dr.GetInt64("ID_REF").ToString();
            item.Padre = dr.GetInt64("ID_PAD").ToString();
            item.Text = dr.GetString("RAG_SOC");
            item.SubType = dr.GetValue("IS_PADRE").ToString();
            item.Source = dr.GetString("SRC");
            item.Description = dr.GetString("REF_TYP");
            return item;
        }

        #endregion
    }
}
