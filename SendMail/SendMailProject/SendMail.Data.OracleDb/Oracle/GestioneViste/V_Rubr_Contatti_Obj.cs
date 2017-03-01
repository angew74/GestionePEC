using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.OracleDb;
using SendMail.Data.OracleDb.OracleObjects;
using SendMail.DataContracts.Interfaces;
using Oracle.DataAccess.Client;
using SendMail.Model.RubricaMapping;
using Oracle.DataAccess.Types;
using ActiveUp.Net.Mail.DeltaExt;
using SendMailApp.OracleCore.Oracle.OrderedQuery;

namespace SendMailApp.OracleCore.Oracle.GestioneViste
{
    internal class V_Rubr_Contatti_Obj : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IDisposable
    {
        private OracleSessionManager context;

        #region "C.tor"

        internal V_Rubr_Contatti_Obj(OracleSessionManager daoContext)
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

        #region "Internal Methods"

        private bool intoIPA = false;

        internal IList<RubricaEntitaType> GetContattiByIdOrg(Int64 idOrg)
        {
            List<RubricaEntitaType> listEntities = null;

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = "SELECT VALUE(v0)"
                                + " FROM v_rubr_entita_obj v0"
                                + " WHERE :p_id_org IN (v0.id_referral, v0.ref_org)";
                oCmd.Parameters.Add("p_id_org", idOrg);

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listEntities = new List<RubricaEntitaType>();

                            while (r.Read())
                            {
                                RubricaEntitaType ent = (RubricaEntitaType)(r.GetOracleValue(0));
                                //if (!ent.oRUBR_CONTATTI_REFS.IsNull)
                                //{
                                //    ent.Contatti = new List<RubricaContatti>();
                                //    foreach (OracleRef or in ent.oRUBR_CONTATTI_REFS.ListContatti)
                                //    {
                                //        RubricaContattiType rc = (RubricaContattiType)or.GetCustomObject(OracleUdtFetchOption.Server);
                                //        rc.SetEntita(ent);
                                //        ent.Contatti.Add(rc);
                                //    }
                                //}
                                listEntities.Add(ent);
                            }
                        }
                    }
                }
                catch
                {
                    listEntities = null;
                }
            }

            return listEntities;
        }

        internal IList<RubricaEntitaType> GetContattiOrgByName(string nomeEntita)
        {
            List<RubricaEntitaType> listEntities = null;

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = "SELECT VALUE(v0)"
                                + " FROM v_rubr_entita_obj v0"
                                + " WHERE :p_id_org IN (upper(v0.ragione_sociale), upper(v0.ufficio))";
                oCmd.Parameters.Add("p_id_org", nomeEntita.ToUpper());

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            listEntities = new List<RubricaEntitaType>();

                            while (r.Read())
                            {
                                RubricaEntitaType ent = (RubricaEntitaType)(r.GetOracleValue(0));
                                if (!ent.RUBR_CONTATTI_REFS.IsNull)
                                {
                                    ent.Contatti = new List<RubricaContatti>();
                                    foreach (string o in ent.RUBR_CONTATTI_REFS.ListContattiRef)
                                    {
                                        OracleRef or = new OracleRef(base.CurrentConnection, o);
                                        RubricaContattiType rc = (RubricaContattiType)or.GetCustomObject(OracleUdtFetchOption.Server);
                                        rc.SetEntita(ent);
                                        ent.Contatti.Add(rc);
                                    }
                                }
                                listEntities.Add(ent);
                            }
                        }
                    }
                }
                catch
                {
                    listEntities = null;
                }
            }

            return listEntities;
        }

        internal RubricaEntitaType GetEntitaByIdReferral(Int64 idReferral)
        {
            RubricaEntitaType ret = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText =
                        String.Format("SELECT VALUE(V0) FROM V_RUBR_ENTITA_OBJ V0 WHERE ID_REFERRAL = {0}", idReferral);

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            ret = (RubricaEntitaType)(r.GetOracleValue(0));
                            ret.SetContatti(base.CurrentConnection);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return ret;
        }

        internal List<RubricaEntitaType> GetEntitaIPAbyIPAid(string IPAid)
        {
            List<RubricaEntitaType> l = new List<RubricaEntitaType>();
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    string cmdT = "SELECT VALUE(V0)"
                               + " FROM V_RUBR_ENTITA_IPA_OBJ V0"
                               + " WHERE IPA_ID = '{0}'"
                               + " OR ID_REFERRAL IN ("
                                    + "SELECT REF_ORG"
                                   + " FROM V_RUBR_ENTITA_IPA_OBJ"
                                   + " WHERE IPA_ID = '{0}' AND REF_ORG IS NOT NULL)";

                    oCmd.CommandText = string.Format(cmdT, IPAid);

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            RubricaEntitaType ret = (RubricaEntitaType)(r.GetOracleValue(0));
                            ret.SetContatti(base.CurrentConnection);
                            l.Add(ret);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return l;
        }

        internal List<RubricaEntitaType> GetEntitaByPartitaIVA(string partitaIVA)
        {
            List<RubricaEntitaType> l = new List<RubricaEntitaType>();
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    string cmdT = "SELECT VALUE(V0)"
                               + " FROM V_RUBR_ENTITA_OBJ V0"
                               + " WHERE P_IVA = '{0}' AND REFERRAL_TYPE IN ('AZ_PS', 'PG', 'AZ_PRI')"
                               + " OR ID_REFERRAL IN ("
                                    + "SELECT REF_ORG"
                                   + " FROM V_RUBR_ENTITA_OBJ"
                                   + " WHERE P_IVA = '{0}' AND REFERRAL_TYPE IN ('AZ_PS', 'PG', 'AZ_PRI')"
                                   + " AND REF_ORG IS NOT NULL)";
                    oCmd.CommandText = String.Format(cmdT, partitaIVA);

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            RubricaEntitaType ret = (RubricaEntitaType)(r.GetOracleValue(0));
                            ret.SetContatti(base.CurrentConnection);
                            l.Add(ret);
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return l;
        }

        internal ResultList<RubricaContattiType> GetContattiByParams(List<SendMail.Model.EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita)
        {
            ResultList<RubricaContattiType> res = new ResultList<RubricaContattiType>();
            if (da == 0) ++da;
            res.Da = da;

            string query = String.Format("SELECT VALUE(V0) FROM {0} V0", ((intoIPA) ? "V_RUBR_CONTATTI_IPA_OBJ" : "V_RUBR_CONTATTI_OBJ"));

            string orderby = " order by {0} asc nulls last";
            string[] oBy = new string[pars.Count];

            if (pars != null && pars.Count != 0)
            {
                query += " WHERE ";
            }

            string[] wherePars = new string[pars.Count];

            for (int i = 0; i < pars.Count; i++)
            {
                KeyValuePair<SendMail.Model.FastIndexedAttributes, List<string>> p = pars.ElementAt(i);
                if (p.Value == null || p.Value.Count == 0)
                {
                    throw new ArgumentException("Parametri non validi");
                }

                string qPar = null;
                switch (p.Key)
                {
                    case SendMail.Model.FastIndexedAttributes.FAX:
                        qPar = "V0.FAX =";
                        oBy[i] = "V0.fax";
                        break;
                    case SendMail.Model.FastIndexedAttributes.MAIL:
                        qPar = "V0.MAIL =";
                        oBy[i] = "V0.mail";
                        break;
                    case SendMail.Model.FastIndexedAttributes.TELEFONO:
                        qPar = "V0.TELEFONO =";
                        oBy[i] = "V0.telefono";
                        break;
                    case SendMail.Model.FastIndexedAttributes.COGNOME:
                        qPar = "V0.ENTITA_REF.COGNOME =";
                        oBy[i] = "V0.ENTITA_REF.COGNOME";
                        break;
                    case SendMail.Model.FastIndexedAttributes.RAGIONE_SOCIALE:
                        qPar = "V0.ENTITA_REF.RAGIONE_SOCIALE =";
                        oBy[i] = "V0.ENTITA_REF.RAGIONE_SOCIALE";
                        break;
                    case SendMail.Model.FastIndexedAttributes.UFFICIO:
                        qPar = "V0.ENTITA_REF.UFFICIO =";
                        oBy[i] = "V0.ENTITA_REF.UFFICIO";
                        break;
                    default:
                        throw new NotImplementedException("Tipo di rircerca non implementato");
                }

                string[] qCrt = new string[p.Value.Count];
                for (int j = 0; j < p.Value.Count; j++)
                {
                    qCrt[j] = String.Format("{0} '{1}'", qPar, p.Value[j]);
                }

                wherePars[i] = String.Format("({0})", String.Join(" OR ", qCrt));
            }

            query += String.Join(" AND ", wherePars);

            string queryCount = query.Replace(" VALUE(V0) ", " count(*) ");

            query += String.Format(orderby, String.Join(", ", oBy));

            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryCount;
                try
                {
                    int tot = Convert.ToInt32(oCmd.ExecuteScalar());
                    res.Totale = tot;
                    res.Per = (tot > per) ? per : tot;
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
                                res.List = new List<RubricaContattiType>();
                                while (r.Read())
                                {
                                    RubricaContattiType rc = r.GetValue(1) as RubricaContattiType;
                                    if (withEntita)
                                    {
                                        rc.SetEntita(base.CurrentConnection);
                                    }
                                    res.List.Add(rc);
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
            }
            return res;
        }

        internal ResultList<RubricaContattiType> GetContattiIPAByParams(List<SendMail.Model.EntitaType> tEnt, Dictionary<SendMail.Model.FastIndexedAttributes, List<string>> pars, int da, int per, bool withEntita)
        {
            intoIPA = true;
            return GetContattiByParams(tEnt, pars, da, per, withEntita);
        }

        internal RubricaContattiType GetContattiById(long idContact)
        {
            RubricaContattiType cont = null;
            using (OracleCommand cmd = CurrentConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT VALUE(V0) FROM V_RUBR_CONTATTI_OBJ V0 WHERE ID_CONTACT = :pID_CONTACT";
                cmd.BindByName = true;
                cmd.Parameters.Add("pID_CONTACT", OracleDbType.Decimal, idContact, System.Data.ParameterDirection.Input);
                using (OracleDataReader rd = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    while (rd.Read())
                    {
                        cont = (RubricaContattiType)rd.GetValue(0);
                        cont.SetEntita(CurrentConnection);
                    }
                }
            }
            return cont;
        }

        #region "COnversion methods"

        internal IList<RubricaEntita> ConvertToRubricaEntita(IList<RubricaEntitaType> ret)
        {
            if (ret == null || ret.Count == 0) return null;
            return ret.Cast<RubricaEntita>().ToList();
        }

        internal IList<RubricaContatti> ConvertToRubricaContatti(IList<RubricaEntitaType> ret)
        {
            if (ret == null || ret.Count == 0) return null;

            return ret.SelectMany<RubricaEntitaType, RubricaContatti>(c => c.Contatti).ToList();
        }

        internal ResultList<RubricaContatti> ConvertToRubricaContatti(ResultList<RubricaContattiType> ret)
        {
            ResultList<RubricaContatti> res = new ResultList<RubricaContatti>
            {
                Da = ret.Da,
                Per = ret.Per,
                Totale = ret.Totale,
                List = ((ret.List == null) ? null : ret.List.Cast<RubricaContatti>().ToList())
            };
            return res;
        }

        #endregion

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (!base.Context.TransactionModeOn)
                base.CurrentConnection.Close();
        }

        #endregion
    }
}
