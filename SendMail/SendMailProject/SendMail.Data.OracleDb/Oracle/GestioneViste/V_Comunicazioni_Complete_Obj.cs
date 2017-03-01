using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.OracleDb;
using SendMail.DataContracts.Interfaces;
using SendMail.Data.OracleDb.OracleObjects;
using Oracle.DataAccess.Client;
using SendMail.Model;
using ActiveUp.Net.Common.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMailApp.OracleCore.Oracle.GestioneViste
{
    internal class V_Comunicazioni_Complete_Obj : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(V_Comunicazioni_Complete_Obj));

        #region "Private string commands"

        private const string queryBase = "SELECT VALUE(v0) FROM v_comunicazioni_complete_obj v0";

        #endregion

        #region "C.tor"

        private OracleSessionManager context;

        public V_Comunicazioni_Complete_Obj(OracleSessionManager daoContext)
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

        /// <summary>
        /// Ottiene tutte le comunicazioni della banca dati
        /// </summary>
        /// <returns></returns>
        internal IList<ComunicazioniType> GetAllComunicazioni()
        {
            List<ComunicazioniType> lComunicazioni = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryBase;
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lComunicazioni = new List<ComunicazioniType>();
                            while (r.Read())
                            {
                                lComunicazioni.Add((ComunicazioniType)r.GetValue(0));
                            }
                        }
                    }
                }
                catch
                {
                    lComunicazioni = null;
                }
                return lComunicazioni;
            }
        }

        /// <summary>
        /// Ottiene tutte le comunicazioni che hanno un certo status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        internal IList<ComunicazioniType> GetComunicazioniByStatus(TipoCanale tipoCanale, List<MailStatus> status, bool include, int? minRec, int? maxRec, string utente)
        {
            List<ComunicazioniType> lComunicazioni = new List<ComunicazioniType>();
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                #region "old query"
                /* old query
                oCmd.CommandText = "WITH t_flussi AS"
                                + " (SELECT REF_ID_COM"
                                + ", ROW_NUMBER() OVER (ORDER BY REF_ID_COM) AS RN"
                                + " FROM ("
                                        + "SELECT DISTINCT REF_ID_COM"
                                        + " FROM COMUNICAZIONI_FLUSSO CF"
                                        + " WHERE CANALE = '" + tipoCanale.ToString() + "'"
                                        + " AND"
                                        + ((include == false) ? " NOT" : "")
                                        + " EXISTS (SELECT *"
                                                + " FROM COMUNICAZIONI_FLUSSO CF0"
                                                + " WHERE CF0.REF_ID_COM = CF.REF_ID_COM"
                                                + " AND CF0.CANALE = CF.CANALE"
                                                + " AND CF0.STATO_COMUNICAZIONE_NEW"
                                                + " IN ('" + String.Join("', '", status.Select(s => ((int)s).ToString()).ToArray()) + "'))"
                                        + " ORDER BY REF_ID_COM)) "
                                        + queryBase
                                        + " WHERE v0.com_flussi IS NOT EMPTY"
                                        + " AND v0.id_com IN (SELECT ref_id_com FROM t_flussi"
                                                          //+ " WHERE "
                                                          //+ ((minRec.HasValue) ? (" RN >= " + minRec.Value.ToString()) : " RN >= 0")
                                                          //+ ((maxRec.HasValue) ? (" AND RN <= " + maxRec.Value.ToString()) : "")
                                        + ")"
                                        + (String.IsNullOrEmpty(utente) ? "" : " AND v0.mail_com.mail_sender = '" + utente + "'");
                */
                #endregion

                StringBuilder sb = new StringBuilder("WITH t_flussi AS")
                    .Append(" (SELECT DISTINCT IDX, ROW_NUMBER() OVER (ORDER BY IDX) AS RN")
                    .Append(" FROM (SELECT DISTINCT mc.REF_ID_COM  as IDX FROM COMUNICAZIONI_FLUSSO CF INNER JOIN MAIL_CONTENT MC")
                    .Append(" on cf.ref_id_com = mc.ref_id_com")
                    .Append(" WHERE mc.mail_sender = :p_sender and CANALE = :p_canale AND")
                    .Append(include ? "" : " NOT")
                    .Append(" EXISTS (SELECT 1")
                    .Append(" FROM COMUNICAZIONI_FLUSSO CF0")
                    .Append(" WHERE CF0.REF_ID_COM = CF.REF_ID_COM")
                    .Append(" AND CF0.CANALE = CF.CANALE")
                    .Append(" AND CF0.STATO_COMUNICAZIONE_NEW IN (")
                    .Append(string.Format("'{0}'", string.Join("', '", status.Select(s => ((int)s).ToString()).ToArray())))
                    .Append(")))")
                    .Append(" ORDER BY IDX)")
                    .Append(" SELECT VALUE(v0)")                  
                    .Append(" FROM v_comunicazioni_complete_obj v0")
                    .Append(" WHERE v0.com_flussi IS NOT EMPTY")
                    .Append(" AND v0.id_com IN (SELECT idx FROM t_flussi")
                    .Append(" where RN >= :p_minRec")
                    .Append(" AND RN <= :p_maxRec")
                    .Append(")");

                oCmd.CommandText = sb.ToString();
                oCmd.BindByName = true;
                oCmd.Parameters.Add(new OracleParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    ParameterName = "p_sender",
                    Value = utente
                });
                oCmd.Parameters.Add(new OracleParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    ParameterName = "p_canale",
                    Size = 20,
                    Value = tipoCanale.ToString()
                });
                oCmd.Parameters.Add(new OracleParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    OracleDbType = OracleDbType.Decimal,
                    ParameterName = "p_minRec",
                    Precision = 10,
                    Scale = 0,
                    Value = ((minRec.HasValue) ? minRec.Value : 0)
                });
                oCmd.Parameters.Add(new OracleParameter
                {
                    Direction = System.Data.ParameterDirection.Input,
                    OracleDbType = OracleDbType.Decimal,
                    ParameterName = "p_maxRec",
                    Value = ((maxRec.HasValue) ? maxRec.Value : int.MaxValue)
                });
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            lComunicazioni.Add((ComunicazioniType)r.GetValue(0));
                        }
                    }
                }
                catch (Exception ex)
                {
                    lComunicazioni = null;
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ORA_ERR013", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                        throw mEx;
                    }
                    else throw ex;
                }
            }
            return lComunicazioni;
        }

        /// <summary>
        /// Ottiene tutte le comunicazioni che hanno/non hanno allegati
        /// </summary>
        /// <param name="withAttach"></param>
        /// <returns></returns>
        internal IList<ComunicazioniType> GetComunicazioniAttachmentDepending(bool withAttach)
        {
            List<ComunicazioniType> lComunicazioni = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = queryBase
                                + " WHERE v0.com_allegati IS "
                                + ((withAttach == true) ? "NOT" : "")
                                + " EMPTY";

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lComunicazioni = new List<ComunicazioniType>();
                            while (r.Read())
                            {
                                lComunicazioni.Add((ComunicazioniType)r.GetValue(0));
                            }
                        }
                    }
                }
                catch
                {
                    lComunicazioni = null;
                }
            }
            return lComunicazioni;
        }

        /// <summary>
        /// Ottiene tutte le comunicazioni per canale
        /// </summary>
        /// <param name="tipoCanale"></param>
        /// <returns></returns>
        internal IList<ComunicazioniType> GetComunicazioniByCanale(TipoCanale tipoCanale)
        {
            List<ComunicazioniType> lComunicazioni = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                string query = queryBase;
                switch (tipoCanale)
                {
                    case TipoCanale.A_MANO:
                        break;

                    case TipoCanale.FAX:
                        break;

                    case TipoCanale.MAIL:
                        query += " WHERE v0.mail_com IS NOT NULL";
                        break;

                    case TipoCanale.POSTA:
                        break;

                    case TipoCanale.UNKNOWN:
                        break;
                }

                oCmd.CommandText = query;

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            lComunicazioni = new List<ComunicazioniType>();
                            while (r.Read())
                            {
                                lComunicazioni.Add((ComunicazioniType)r.GetValue(0));
                            }
                        }
                    }
                }
                catch
                {
                    lComunicazioni = null;
                }
            }
            return lComunicazioni;
        }

        internal ComunicazioniType GetComunicazioneByIdMail(Int64 idMail)
        {
            ComunicazioniType com = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = queryBase + " WHERE v0.MAIL_COM.ID_MAIL = " + idMail;
                    using (OracleDataReader r = oCmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (r.Read())
                            com = (ComunicazioniType)r.GetValue(0);
                    }
                }
            }
            catch
            {
            }

            return com;
        }

        #endregion
    }
}
