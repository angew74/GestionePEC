using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.OracleDb;
using SendMail.DataContracts.Interfaces;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Model.RubricaMapping;
using Oracle.DataAccess.Client;
using SendMailApp.OracleCore.Oracle.OrderedQuery;
using SendMail.Data.Utilities;
using System.Data;
using SendMailApp.OracleCore.Oracle.GestioneViste;
using SendMail.Model.Wrappers;
using SendMail.Model;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMail.Data.OracleDb
{
    public class RubricaEntitaOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IRubricaEntitaDao
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(RubricaEntitaOracleDb));
        #region statements

        private const String insertEntitaStatement = "INSERT INTO rubr_entita (ID_PADRE, REFERRAL_TYPE, COGNOME"
                                                  + ", NOME, COD_FIS, P_IVA, RAGIONE_SOCIALE, UFFICIO, NOTE"
                                                  + ", REF_ID_ADDRESS, FLG_IPA, IPA_DN, IPA_ID, DISAMB_PRE"
                                                  + ", DISAMB_POST, REF_ORG, SITO_WEB, AFF_IPA)"
                                                  + " VALUES (:p_ID_PADRE, :p_REF_TYPE, :p_COG, :p_NOME"
                                                  + ", :p_COD_FIS, :p_P_IVA, :p_RAG_SOC, :p_UFF, :p_NOTE"
                                                  + ", :p_REF_ID_ADD, :p_FLG_IPA, :p_IPA_DN, :p_IPA_ID"
                                                  + ", :p_DIS_PRE, :p_DIS_POST, :p_REF_ORG, :p_SITO, :p_AFF_IPA)"
                                                  + " RETURNING ID_REFERRAL INTO :p_ID";

        private const string selectEntitaBaseQuery = "SELECT ID_REFERRAL"
                                                   + ", ID_PADRE"
                                                   + ", REFERRAL_TYPE"
                                                   + ", COGNOME"
                                                   + ", NOME"
                                                   + ", COD_FIS"
                                                   + ", P_IVA"
                                                   + ", NVL(RAGIONE_SOCIALE,"
                                                           + " (SELECT RAGIONE_SOCIALE"
                                                           + " FROM RUBR_ENTITA RE"
                                                           + " WHERE RE.ID_REFERRAL = R.REF_ORG))"
                                                           + " AS \"RAGIONE_SOCIALE\""
                                                   + ", UFFICIO"
                                                   + ", NOTE"
                                                   + ", REF_ID_ADDRESS"
                                                   + ", FLG_IPA"
                                                   + ", IPA_DN"
                                                   + ", IPA_ID"
                                                   + ", DISAMB_PRE"
                                                   + ", DISAMB_POST"
                                                   + ", REF_ORG"
                                                   + ", SITO_WEB"
                                                   + ", AFF_IPA"
                                                   + " FROM RUBR_ENTITA r";

        private const string selectSimilarityEntitaBaseQuery = "SELECT {0}, r.* FROM RUBR_ENTITA r {1} ORDER BY {2}";
        private const string selectEntitaByMailDomain = "WITH ENT AS ("
                                                      + " SELECT DISTINCT r.* FROM RUBR_ENTITA r"
                                                      + " INNER JOIN RUBR_CONTATTI c ON r.id_referral = c.ref_id_referral"
                                                      + " WHERE c.mail_domain= :p_mailDomain)"
                                                      + " SELECT *"
                                                      + " FROM ENT"
                                                      + " WHERE ref_org IS NULL"
                                                      + " UNION"
                                                      + " SELECT DISTINCT *"
                                                      + " FROM RUBR_ENTITA"
                                                      + " WHERE ID_REFERRAL IN (SELECT REF_ORG FROM ENT"
                                                                            + " WHERE REF_ORG IS NOT NULL)";

        private const string selectEntitaIPAByMailDomain = "WITH T_IPA AS"
                                                         + " (SELECT *"
                                                         + " FROM IPA"
                                                         + " WHERE MAIL_DOMAIN = :p_mailDomain)"
                                                         + " SELECT T_IPA.*"
                                                         + " FROM T_IPA"
                                                         + " WHERE ID_PADRE = 1"
                                                         + " UNION"
                                                         + " SELECT IPA.*"
                                                         + " FROM IPA"
                                                         + " WHERE ID_RUB IN (SELECT ID_RUB"
                                                                          + " FROM IPA"
                                                                          + " WHERE ID_PADRE = 1"
                                                                          + " CONNECT BY NOCYCLE PRIOR ID_PADRE = ID_RUB"
                                                                          + " START WITH ID_RUB IN (SELECT ID_RUB FROM T_IPA"
                                                                                                + " WHERE ID_PADRE <> 1))";

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

        private const string selectRubricaEntitaByPadre = "SELECT DISTINCT"
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
                                                        + " WHERE ID_PADRE = :p_ID_PADRE"
                                                        + " ORDER BY RAG_SOC";


        private const string updateEntitaStatement = "UPDATE RUBR_ENTITA SET UFFICIO = :p_UFF, SITO_WEB = :p_SITO, COGNOME"
                                                        + "= :p_COG, NOME = :p_NOME, NOTE = :p_NOTE , COD_FIS=:p_COD_FIS, P_IVA=:p_P_IVA"
                                                        + " WHERE ID_REFERRAL = :p_ID";

        #endregion

        #region "C.tor"

        private OracleSessionManager context;
        public RubricaEntitaOracleDb(OracleSessionManager daoContext)
            : base(daoContext)
        {
            this.context = daoContext;
            //apro la cn se non è già aperta.
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

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

            string query = selectEntitaBaseQuery;

            string orderby = " order by {0} asc nulls last";
            string[] oBy = new string[pars.Count];

            if (pars != null && pars.Count != 0)
            {
                query += " WHERE ";
                string[] wherePars = new string[pars.Count];

                for (int i = 0; i < pars.Count; i++)
                {
                    KeyValuePair<SendMail.Model.FastIndexedAttributes, IList<string>> p = pars.ElementAt(i);
                    if (p.Value == null || p.Value.Count == 0)
                    {
                        throw new ArgumentException("Parametri non validi");
                    }

                    string qPar = null;
                    switch (p.Key)
                    {
                        case SendMail.Model.FastIndexedAttributes.COGNOME:
                            qPar = "lower(r.cognome||' '||r.nome) like";
                            oBy[i] = "cognome, nome";
                            break;

                        case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                            qPar = "lower(r.disamb_pre||' '||r.ragione_sociale||' '||r.disamb_post) like";
                            oBy[i] = "disamb_pre, ragione_sociale, disamb_post";
                            break;

                        case SendMail.Model.FastIndexedAttributes.UFFICIO:
                            qPar = "lower(r.ufficio) like";
                            oBy[i] = "ragione_sociale, ufficio";
                            break;

                        default:
                            throw new NotImplementedException("Tipo di rircerca non implementato");
                    }

                    string[] qCrt = new string[p.Value.Count];
                    for (int j = 0; j < p.Value.Count; j++)
                    {
                        qCrt[j] = String.Format("{0} '%{1}%'", qPar, p.Value[j].ToLower());
                    }
                    wherePars[i] = String.Format("({0})", String.Join(" OR ", qCrt));
                }

                query += String.Join(" AND ", wherePars);

                if (!(tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN)))
                {
                    query += String.Format(" and r.REFERRAL_TYPE IN ({0})", String.Join(", ", tEnt.Select(c => String.Format("'{0}'", c.ToString())).ToArray()));
                }
            }
            else if (!(tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN)))
            {
                query += String.Format(" and r.REFERRAL_TYPE IN ({0})", String.Join(", ", tEnt.Select(c => String.Format("'{0}'", c.ToString())).ToArray()));
            }

            string qryCount = "SELECT COUNT(*) FROM (" + query + ")";

            query += String.Format(orderby, String.Join(", ", oBy));

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = qryCount;
                try
                {
                    int tRig = Convert.ToInt32(oCmd.ExecuteScalar());
                    res.Totale = tRig;
                    res.Per = (tRig > per) ? per : tRig;
                }
                catch
                {
                    res.Per = res.Totale = 0;
                    res.List = null;
                    throw;
                }

                if (res.Totale > 0)
                {
                    if (res.Per > 0)
                    {
                        oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, res.Per);
                    }
                    else
                    {
                        oCmd.CommandText = query;
                    }

                    try
                    {
                        using (OracleDataReader r = oCmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                res.List = new List<RubricaEntita>();
                                while (r.Read())
                                {
                                    res.List.Add(DaoOracleDbHelper.MapToRubricaEntita(r));
                                }
                            }
                        }
                    }
                    catch
                    {
                        res.List = null;
                        throw;
                    }
                }
                else
                {
                    try
                    {
                        res = LoadSimilarityEntitaByParams(tEnt, pars, 1, per);
                    }
                    catch
                    {
                        res = null;
                        throw;
                    }
                }
            }
            return res;
        }

        public ResultList<RubricaEntita> LoadSimilarityEntitaByParams(IList<SendMail.Model.EntitaType> tEnt,
            IDictionary<SendMail.Model.FastIndexedAttributes, IList<string>> pars, int da, int per)
        {
            if (pars == null || pars.Count == 0) return null;

            if (tEnt == null) { tEnt = new List<SendMail.Model.EntitaType>(); }
            if (tEnt.Count == 0) { tEnt.Add(SendMail.Model.EntitaType.ALL); }

            if (pars.Any(x => x.Value == null))
            {
                throw new ArgumentException("Parametri non validi");
            }

            ResultList<RubricaEntita> res = new ResultList<RubricaEntita>();
            res.Da = da;
            res.Per = per;

            int tot = pars.SelectMany(c => c.Value).Count();
            string[] matchPars = new string[tot];
            string utlBase = "utl_match.jaro_winkler_similarity('{0}', {1})";
            string orderby = null;

            for (int i = 0; i < pars.Count; i++)
            {
                KeyValuePair<SendMail.Model.FastIndexedAttributes, IList<string>> p = pars.ElementAt(i);
                if (p.Value == null || p.Value.Count == 0)
                {
                    throw new ArgumentException("Parametri non validi");
                }

                string qPar = null;

                switch (p.Key)
                {
                    case SendMail.Model.FastIndexedAttributes.COGNOME:
                        qPar = "nvl2(cognome, lower(cognome||' '||nome), lower(nome))";
                        break;

                    case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                        qPar = "nvl2(disamb_pre, lower(disamb_pre||' '||ragione_sociale||' '||disamb_post), lower(ragione_sociale))";
                        break;

                    case SendMail.Model.FastIndexedAttributes.UFFICIO:
                        qPar = "ufficio";
                        break;

                    default:
                        throw new NotImplementedException("Tipo di rircerca non implementato");
                }

                for (int j = 0; j < p.Value.Count; j++)
                {
                    int idx = Array.FindIndex<string>(matchPars, x => String.IsNullOrEmpty(x));
                    if (idx != -1)
                    {
                        matchPars[idx] = String.Format(utlBase, p.Value[j], qPar);
                        if (String.IsNullOrEmpty(orderby))
                        {
                            orderby += String.Format("{0} desc", (idx + 1));
                        }
                        else
                        {
                            orderby += String.Format(", {0} desc", (idx + 1));
                        }
                    }
                }
            }

            string where = null;
            if (!(tEnt.Contains(SendMail.Model.EntitaType.UNKNOWN)))
            {
                where = "WHERE REFERRAL_TYPE IN (" + String.Join(", ", tEnt.Select(c => String.Format("'{0}'", c.ToString())).ToArray()) + ")";
            }

            string query = String.Format(selectSimilarityEntitaBaseQuery,
                String.Join(", ", matchPars), where, orderby);

            string complOrderBy = null;
            switch (tEnt[0])
            {
                case SendMail.Model.EntitaType.ALL:
                case SendMail.Model.EntitaType.PA:
                case SendMail.Model.EntitaType.PA_SUB:
                case SendMail.Model.EntitaType.AZ_PRI:
                case SendMail.Model.EntitaType.AZ_PS:
                    complOrderBy = " order by ragione_sociale asc nulls last";
                    break;
                case SendMail.Model.EntitaType.PA_UFF_PF:
                case SendMail.Model.EntitaType.AZ_UFF_PF:
                case SendMail.Model.EntitaType.AZ_PF:
                case SendMail.Model.EntitaType.PA_PF:
                case SendMail.Model.EntitaType.PF:
                case SendMail.Model.EntitaType.PG:
                    complOrderBy = " order by cognome, nome asc nulls last";
                    break;
                case SendMail.Model.EntitaType.PA_UFF:
                case SendMail.Model.EntitaType.AZ_UFF:
                    complOrderBy = " order by ufficio asc nulls last";
                    break;
                case SendMail.Model.EntitaType.GRP:
                case SendMail.Model.EntitaType.UNKNOWN:
                    break;
                default:
                    throw new ArgumentException("Caso non implementato");
            }

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    if (per > 0)
                    {
                        oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per);
                    }
                    else
                    {
                        oCmd.CommandText = query;
                    }

                    oCmd.CommandText += complOrderBy;

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            res.List = new List<RubricaEntita>();

                            while (r.Read())
                            {
                                res.List.Add(DaoOracleDbHelper.MapToRubricaEntita(r));
                            }
                        }
                    }
                }
            }
            catch
            {
                res = null;
                throw;
            }

            return res;
        }

        public SendMail.Model.RubricaMapping.RubricaEntita LoadEntitaCompleteById(long idEntita)
        {
            V_Rubr_Contatti_Obj v = new V_Rubr_Contatti_Obj(this.context);
            return (RubricaEntita)v.GetEntitaByIdReferral(idEntita);

            // commentato (per oracle 10)
            //RubricaEntita res = null;
            //try
            //{
            //    using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            //    {
            //        oCmd.CommandText = "SELECT * FROM RUBR_ENTITA WHERE ID_REFERRAL = " + idEntita;
            //        using (OracleDataReader r = oCmd.ExecuteReader())
            //        {
            //            while (r.Read())
            //            {
            //                res = DaoOracleDbHelper.MapToRubricaEntita(r);
            //            }
            //        }
            //    }
            //    if (res != null)
            //        res.Contatti = (List<RubricaContatti>)this.Context.DaoImpl.ContattoDao.LoadContattiOrgByOrgId(res.IdReferral.Value, false, true, false, true);
            //}
            //catch
            //{
            //    throw;
            //}

            //return res;
        }

        public IList<SimpleTreeItem> LoadRubricaEntitaTree(Int64 idEntita)
        {
            IList<SimpleTreeItem> entitaTree = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = selectRubricaEntitaTree;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("p_ID_ENT", OracleDbType.Decimal, idEntita, ParameterDirection.Input);

                    using (OracleDataReader r = oCmd.ExecuteReader())
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
            catch
            {
                throw;
            }
            return entitaTree;
        }

        public IList<SimpleTreeItem> LoadRubricaEntitaTreeByIdPadre(Int64 idPadre)
        {
            IList<SimpleTreeItem> entitaTree = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = selectRubricaEntitaByPadre;
                    oCmd.BindByName = true;
                    oCmd.Parameters.Add("p_ID_PADRE", OracleDbType.Decimal, idPadre, ParameterDirection.Input);

                    using (OracleDataReader r = oCmd.ExecuteReader())
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
            catch
            {
                throw;
            }
            return entitaTree;
        }

        public ResultList<RubricaEntita> LoadEntitaByMailDomain(IList<EntitaType> tEnt, string mail, int da, int per)
        {
            ResultList<RubricaEntita> r = new ResultList<RubricaEntita>();

            string query = null;

            if (tEnt != null)
                tEnt = tEnt.Where(e => e != EntitaType.ALL).ToList();

            if (tEnt == null || tEnt.Count == 0)
            {
                query = selectEntitaByMailDomain;

            }
            else
            {
                query = "SELECT * FROM (";
                query += selectEntitaByMailDomain;
                query += ") WHERE REFERRAL_TYPE IN ('"
                      + String.Join("','", tEnt.Select(x => x.ToString()).ToArray())
                      + "')";
            }
            string queryCnt = "SELECT COUNT(*) FROM (" + query + ")";

            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                ocmd.CommandText = queryCnt;
                ocmd.BindByName = true;
                ocmd.Parameters.Add("p_mailDomain", mail);

                ocmd.CommandText = queryCnt;
                try
                {
                    int tot = Convert.ToInt32(ocmd.ExecuteScalar());
                    r.Da = ((da == 0) ? ++da : da);
                    r.Per = ((tot < per) ? tot : per);
                    r.Totale = tot;
                }
                catch
                {
                    throw;
                }

                if (r.Totale > 0)
                {
                    ocmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per);

                    try
                    {
                        using (OracleDataReader rr = ocmd.ExecuteReader())
                        {
                            if (rr.HasRows)
                            {
                                r.List = new List<RubricaEntita>();
                                while (rr.Read())
                                {
                                    r.List.Add(DaoOracleDbHelper.MapToRubricaEntita(rr));
                                }
                            }
                        }
                    }
                    catch (Exception excp)
                    {
                        r.List = null;

                        if (excp.GetType() != typeof(ManagedException))
                        {
                            //Allineamento log - Ciro
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
                }
            }
            return r;
        }

        public ResultList<RubricaEntita> LoadEntitaIPAByMailDomain(string mail, int da, int per)
        {
            ResultList<RubricaEntita> re = new ResultList<RubricaEntita>();
            string query = selectEntitaIPAByMailDomain;
            string queryCnt = "SELECT COUNT(*) FROM (" + query + ")";

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryCnt;
                oCmd.BindByName = true;
                oCmd.Parameters.Add("p_mailDomain", OracleDbType.Varchar2, mail, ParameterDirection.Input);

                try
                {
                    int tot = Convert.ToInt32(oCmd.ExecuteScalar());
                    re.Da = ((da == 0) ? ++da : da);
                    re.Per = ((tot < per) ? tot : per);
                    re.Totale = tot;
                }
                catch
                {
                    throw;
                }

                if (re.Totale > 0)
                {
                    oCmd.CommandText = OrderedTOracleDB.GetOrderedQuery(query, da, per);

                    try
                    {
                        using (OracleDataReader rr = oCmd.ExecuteReader())
                        {
                            if (rr.HasRows)
                            {
                                re.List = new List<RubricaEntita>();
                                while (rr.Read())
                                {
                                    re.List.Add(DaoOracleDbHelper.MapIPAToRubricaEntita(rr));
                                }
                            }
                        }
                        if (re.List != null && re.List.Count > 0)
                        {
                            ((List<RubricaEntita>)re.List).ForEach(e =>
                            {
                                if (e.ReferralType == EntitaType.UNKNOWN)
                                {
                                    if (String.IsNullOrEmpty(e.Ufficio))
                                    {
                                        e.ReferralType = EntitaType.PA;
                                    }
                                    else
                                    {
                                        e.ReferralType = EntitaType.PA_UFF;
                                    }
                                }
                            });
                        }
                    }
                    catch (Exception excp)
                    {
                        re.List = null;
                        if (excp.GetType() != typeof(ManagedException))
                        {
                            //Allineamento log - Ciro
                            ManagedException mEx = new ManagedException(excp.Message,
                                "RUB_ORA002",
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
            return re;
        }

        public List<RubricaEntita> LoadEntitaByName(IList<EntitaType> tEnt, string name)
        {
            List<RubricaEntita> lEnt = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText =
                        String.Format("SELECT * FROM RUBR_ENTITA WHERE RAGIONE_SOCIALE LIKE '%{0}%' OR UFFICIO LIKE '%{0}%' OR COGNOME||' '||NOME LIKE '%{0}%'",
                        name);
                    if (tEnt != null && tEnt.Count != 0 && !tEnt.Contains(EntitaType.ALL))
                    {
                        oCmd.CommandText += " AND REFERRAL_TYPE IN ('" + string.Join("','", tEnt.Select(e => e.ToString()).ToArray()) + "')";
                    }

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lEnt = new List<RubricaEntita>();
                            while (r.Read())
                            {
                                lEnt.Add(DaoOracleDbHelper.MapToRubricaEntita(r));
                            }
                        }
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
            V_Rubr_Contatti_Obj obj = new V_Rubr_Contatti_Obj(this.context);
            List<RubricaEntita> re = obj.GetEntitaIPAbyIPAid(IPAid).Cast<RubricaEntita>().ToList();
            return re;
        }

        public List<RubricaEntita> LoadEntitaByPartitaIVA(string partitaIVA)
        {
            V_Rubr_Contatti_Obj obj = new V_Rubr_Contatti_Obj(this.context);
            List<RubricaEntita> re = obj.GetEntitaByPartitaIVA(partitaIVA).Cast<RubricaEntita>().ToList(); ;
            return re;
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
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT * FROM RUBR_ENTITA WHERE ID_REFERRAL = " + id;
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            re = DaoOracleDbHelper.MapToRubricaEntita(r);
                        }
                    }
                }
            }
            catch
            {
            }

            return re;
        }

        public void Insert(SendMail.Model.RubricaMapping.RubricaEntita entity)
        {
            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                ocmd.CommandText = insertEntitaStatement;
                ocmd.BindByName = true;
                ocmd.Parameters.AddRange(MapEntityToParams(entity, true));

                try
                {
                    int r = ocmd.ExecuteNonQuery();
                    if (r == 1)
                    {
                        entity.IdReferral = Convert.ToInt64(ocmd.Parameters["p_ID"].Value.ToString());
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
            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                ocmd.CommandText = updateEntitaStatement;
                ocmd.Parameters.AddRange(MapEntityToParams(entity, false));
                ocmd.BindByName = true;
                try
                {
                    int ret = ocmd.ExecuteNonQuery();
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

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                this.CurrentConnection.Close();
        }

        #endregion

        #region mapping

        private OracleParameter[] MapEntityToParams(RubricaEntita e, bool isInsert)
        {
            List<OracleParameter> op = new List<OracleParameter>();

            op.Add(new OracleParameter("p_UFF", e.Ufficio));
            op.Add(new OracleParameter("p_SITO", e.SitoWeb));
            op.Add(new OracleParameter("p_COG", e.Cognome));
            op.Add(new OracleParameter("p_NOME", e.Nome));
            op.Add(new OracleParameter("p_NOTE", e.Note));
            op.Add(new OracleParameter("p_COD_FIS", e.CodiceFiscale));
            op.Add(new OracleParameter("p_P_IVA", e.PartitaIVA));

            if (!isInsert)
                op.Add(new OracleParameter("p_ID", e.IdReferral));
            else
            {
                op.Add(new OracleParameter("p_ID_PADRE", e.IdPadre));
                op.Add(new OracleParameter("p_REF_TYPE", e.ReferralType.ToString()));
                op.Add(new OracleParameter("p_RAG_SOC", e.RagioneSociale));
                op.Add(new OracleParameter("p_REF_ID_ADD", e.RefIdAddress));
                op.Add(new OracleParameter("p_FLG_IPA", Convert.ToInt16(e.IsIPA).ToString()));
                op.Add(new OracleParameter("p_IPA_DN", e.IPAdn));
                op.Add(new OracleParameter("p_IPA_ID", e.IPAId));
                op.Add(new OracleParameter("p_DIS_PRE", e.DisambPre));
                op.Add(new OracleParameter("p_DIS_POST", e.DisambPost));
                op.Add(new OracleParameter("p_REF_ORG", e.RefOrg));
                op.Add(new OracleParameter("p_AFF_IPA", e.AffIPA));
                op.Add(new OracleParameter("p_ID", OracleDbType.Decimal, ParameterDirection.Output));
            }
            return op.ToArray();
        }

        private SimpleTreeItem MapToSimpleTreeItem(IDataReader dr)
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
