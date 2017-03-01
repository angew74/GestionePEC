using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using SendMailApp.OracleCore.Oracle.GestioneViste;
using SendMail.Model.ComunicazioniMapping;
using Oracle.DataAccess.Client;
using SendMail.Data.OracleDb.OracleObjects;
using ActiveUp.Net.Common.DeltaExt;
using log4net;
using Com.Delta.Logging.Errors;
using SendMail.Model.WebserviceMappings;
using System.Data;
using Com.Delta.Logging;
using System.Xml.Linq;

namespace SendMail.Data.OracleDb
{
    public class ComunicazioniOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IComunicazioniDao
    {
        #region "Private command strings"
        private static readonly ILog _log = LogManager.GetLogger(typeof(ComunicazioniOracleDb));

        private const string cmdInsertComunicazione = "INSERIMENTO_COMUNICAZIONE";

        private const string cmdSelectRoot = "SELECT VALUE(v0) FROM v_comunicazioni_complete_obj v0";

        private const string cmdComStatus = "select c.id_com as ID, c.ref_id_sottotitolo as ID_SOTTOTITOLO, cs.sottotitolo as SOTTOTITOLO,cf.data_operazione as DATA_INS, cf3.stato_comunicazione_new as STATO from comunicazioni c inner join comunicazioni_sottotitoli cs on c.ref_id_sottotitolo = cs.id_sottotitolo inner join comunicazioni_flusso cf on c.id_com=cf.ref_id_com inner join (select ref_id_com, data_operazione ,stato_comunicazione_new from comunicazioni_flusso where (ref_id_com, data_operazione) in ( select ref_id_com, max(data_operazione) from comunicazioni_flusso where ref_id_com in(select id_com from comunicazioni where comunicazioni.orig_uid = :p_ORIG_UID) group by (ref_id_com)))cf3 on cf3.ref_id_com=c.id_com where cf.stato_comunicazione_old is null and c.id_com in(select id_com from comunicazioni where comunicazioni.orig_uid = :p_ORIG_UID)";

        private const string cmdProtStatus = "select c.id_com as ID, c.ref_id_sottotitolo as ID_SOTTOTITOLO, cs.sottotitolo " +
                                              " as SOTTOTITOLO,cf.data_operazione as DATA_INS, cf3.stato_comunicazione_new as STATO " +
                                              " from comunicazioni c inner join comunicazioni_sottotitoli cs on c.ref_id_sottotitolo = cs.id_sottotitolo inner join comunicazioni_flusso cf on c.id_com=cf.ref_id_com " +
                                              " inner join (select ref_id_com, data_operazione ,stato_comunicazione_new from " +
                                              " comunicazioni_flusso where (ref_id_com, data_operazione) in ( select ref_id_com, max(data_operazione) " +
                                              " from comunicazioni_flusso where ref_id_com in(select ref_id_com from comunicazioni_protocollo " +
                                              " where RESP_PROT_TIPO=:TIPO AND RESP_PROT_ANNO=:ANNO AND RESP_PROT_NUMERO=:NUMERO AND PROT_IN_OUT=:PROTINOUT) " +
                                              " group by (ref_id_com)))cf3 on cf3.ref_id_com=c.id_com where cf.stato_comunicazione_old is null and c.id_com " +
                                              " in (select ref_id_com FROM  comunicazioni_protocollo t2 WHERE t2.RESP_PROT_TIPO=:TIPO AND T2.RESP_PROT_ANNO=:ANNO AND T2.RESP_PROT_NUMERO=:NUMERO AND PROT_IN_OUT=:PROTINOUT)";

        #endregion

        #region "C.tor"

        private OracleSessionManager context;
        public ComunicazioniOracleDb(OracleSessionManager daoContext)
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

        #region IComunicazioniDao Membri di

        public int GetCountComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, string utente)
        {
            List<MailStatus> lstatus = new List<MailStatus>();
            lstatus.Add(MailStatus.SENT);
            return GetCountComunicazioni(tipoCanale, lstatus, true, utente);
        }

        public ICollection<StatoComunicazioneItem> GetStatoComunicazione(string originalUid)
        {
            ICollection<StatoComunicazioneItem> list = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = cmdComStatus;
                oCmd.Parameters.Add("p_ORIG_UID", originalUid);

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            list = new List<StatoComunicazioneItem>();
                            while (r.Read())
                            {
                                StatoComunicazioneItem itm = new StatoComunicazioneItem();
                                itm.Id = r["ID"].ToString();
                                itm.SottoTitolo = r["ID_SOTTOTITOLO"].ToString();
                                itm.SottoTitoloDescr = r["SOTTOTITOLO"].ToString();
                                itm.DataInserimento = r["DATA_INS"].ToString();
                                itm.StatoInvio = r["STATO"].ToString();
                                list.Add(itm);
                            }
                        }
                    }
                }
                catch (Exception e0)
                {
                    list = null;

                    //Allineamento log - Ciro
                    if (e0.GetType() != typeof(ManagedException))
                    {
                        ManagedException me = new ManagedException(String.Format("Errore lettura stato comunicazione UID: {0}. Dettaglio: {1}",
                            ((originalUid!=null)?originalUid:" vuoto! "), e0.Message), 
                            "ORA_ERR004", 
                            string.Empty, 
                            string.Empty, 
                            e0.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(me);
                        err.objectID = (originalUid!=null) ? originalUid: "";
                        _log.Error(err);
                        throw me;
                    }
                    else throw e0;                    
                }
            }

            return list;
        }

        public Int32 GetCountComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, string utente)
        {
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            return GetCountComunicazioni(tipoCanale, lStatus, false, utente);
        }

        private int GetCountComunicazioni(SendMail.Model.TipoCanale tipoCanale, List<MailStatus> status, bool include, string utente)
        {
            int cnt = 0;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = "SELECT count(*)"
                                + " FROM comunicazioni_flusso cf"
                                    + " INNER JOIN ("
                                        + " SELECT ref_id_com, MAX(data_operazione) AS dta_ope"
                                        + " FROM comunicazioni_flusso"
                                        + " WHERE canale = '" + tipoCanale + "'"
                                        + " GROUP BY ref_id_com"
                                        + " ) gr"
                                    + " ON cf.ref_id_com = gr.ref_id_com AND cf.data_operazione = gr.dta_ope"
                                + " WHERE"
                                + " cf.stato_comunicazione_new"
                                + ((include == false) ? " NOT" : "")
                                + " IN (" + String.Join(", ", status.Select(s => ((int)s).ToString()).ToArray()) + ")"
                                + " AND cf.ute_ope = '" + utente + "'";
                try
                {
                    cnt = Convert.ToInt32(oCmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    cnt = 0;
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(string.Format("Errore count comunicazioni utente: {0}. Details: {1}", utente, ex.Message),
                            "ERR_COM_001",
                            string.Empty,
                            string.Empty,
                            ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                    else throw ex;
                    //ErrorLogInfo error = new ErrorLogInfo();
                    //error.freeTextDetails = ex.Message;
                    //error.logCode = "ERR_COM_001";
                    //_log.Error(error);
                    
                }
            }
            return cnt;
        }

        public ICollection<StatoComunicazioneItem> GetComunicazioniByProtocollo(ComunicazioniProtocollo p)
        {
            ICollection<StatoComunicazioneItem> list = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
              string s =  "select c.id_com as ID, c.ref_id_sottotitolo as ID_SOTTOTITOLO, cs.sottotitolo " +
                                              " as SOTTOTITOLO,cf.data_operazione as DATA_INS, cf3.stato_comunicazione_new as STATO " +
                                              " from comunicazioni c inner join comunicazioni_sottotitoli cs on c.ref_id_sottotitolo = cs.id_sottotitolo inner join comunicazioni_flusso cf on c.id_com=cf.ref_id_com " +
                                              " inner join (select ref_id_com, data_operazione ,stato_comunicazione_new from " +
                                              " comunicazioni_flusso where (ref_id_com, data_operazione) in ( select ref_id_com, max(data_operazione) " +
                                              " from comunicazioni_flusso where ref_id_com in (select ref_id_com from comunicazioni_protocollo " +
                                              " where RESP_PROT_TIPO='" + p.ResponseProtocolloTipo + "' AND RESP_PROT_ANNO=" + p.ResponseProtocolloAnno + " AND RESP_PROT_NUMERO='" + p.ResponseProtocolloNumero + "' AND PROT_IN_OUT='" + p.ProtocolloInOut + "') " +
                                              " group by (ref_id_com)))cf3 on cf3.ref_id_com=c.id_com where cf.stato_comunicazione_old is null and c.id_com " +
                                              " in (select ref_id_com FROM  comunicazioni_protocollo t2 WHERE t2.RESP_PROT_TIPO='" + p.ResponseProtocolloTipo + 
                                              "' AND T2.RESP_PROT_ANNO=" + p.ResponseProtocolloAnno + " AND T2.RESP_PROT_NUMERO='" + p.ResponseProtocolloNumero + "' AND PROT_IN_OUT='" + p.ProtocolloInOut + "')";

                oCmd.CommandText = s;               
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        list = new List<StatoComunicazioneItem>();
                        while (r.Read())
                        {
                            StatoComunicazioneItem itm = new StatoComunicazioneItem();
                            itm.Id = r["ID"].ToString();
                            itm.SottoTitolo = r["ID_SOTTOTITOLO"].ToString();
                            itm.SottoTitoloDescr = r["SOTTOTITOLO"].ToString();
                            itm.DataInserimento = r["DATA_INS"].ToString();
                            itm.StatoInvio = r["STATO"].ToString();
                            list.Add(itm);
                        }
                    }
                }
                catch (Exception e)
                {
                    base.Context.Dispose();
                    //TASK: Allineamento log - Ciro
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(String.Format("Errore nell'estrazione delle emails per protocollo {0} {1}. Dettaglio: ", 
                            p.ResponseProtocolloAnno, p.ResponseProtocolloNumero) + e.Message, 
                            "ORA_ERR005", 
                            string.Empty, 
                            string.Empty, 
                            e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.objectID = p.RefIdCom.ToString(); //foreign key ID_COMUNICAZIONE
                        _log.Error(err);
                        throw mEx;
                    }
                    else throw e;
                    //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                    //error.freeTextDetails = "Errore nell'estrazione delle emails per protocollo " + p.ResponseProtocolloAnno + " " + p.ResponseProtocolloNumero + " E078 Dettagli Errore: " + e.Message;
                    //error.logCode = "ERR_078";              
                    //error.passiveparentcodeobjectID = string.Empty;
                    //error.passiveobjectGroupID = p.ResponseProtocolloAnno + " " + p.ResponseProtocolloNumero;
                    //error.passiveobjectID = string.Empty;
                    //error.passiveapplicationID = string.Empty;
                    //_log.Error(error);
                    //throw;
                }
            }

            return list;
        }

        public ICollection<Comunicazioni> GetComunicazioniInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            V_Comunicazioni_Complete_Obj v = new V_Comunicazioni_Complete_Obj(context);
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            return v.GetComunicazioniByStatus(tipoCanale, lStatus, true, minRec, maxRec, utente).Cast<Comunicazioni>().ToList();
        }

        public ICollection<Comunicazioni> GetComunicazioniNonInviate(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            V_Comunicazioni_Complete_Obj v = new V_Comunicazioni_Complete_Obj(context);
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            IList<ComunicazioniType> lC = v.GetComunicazioniByStatus(tipoCanale, lStatus, false, minRec, maxRec, utente);
            if (lC != null)
                return lC.Cast<Comunicazioni>().ToList();
            else return null;
        }

        public ICollection<Comunicazioni> GetComunicazioniDaInviare(SendMail.Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            V_Comunicazioni_Complete_Obj v = new V_Comunicazioni_Complete_Obj(context);
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            lStatus.Add(MailStatus.ERROR);
            lStatus.Add(MailStatus.CANCELLED);
            IList<ComunicazioniType> lC = v.GetComunicazioniByStatus(tipoCanale, lStatus, false, minRec, maxRec, utente);
            if (lC != null)
                return lC.Cast<Comunicazioni>().ToList();
            else return null;
        }

        public ICollection<Comunicazioni> GetComunicazioniConAllegati()
        {
            V_Comunicazioni_Complete_Obj v = new V_Comunicazioni_Complete_Obj(context);
            return v.GetComunicazioniAttachmentDepending(true).Cast<Comunicazioni>().ToList();
        }

        public ICollection<Comunicazioni> GetComunicazioniSenzaAllegati()
        {
            V_Comunicazioni_Complete_Obj v = new V_Comunicazioni_Complete_Obj(context);
            return v.GetComunicazioniAttachmentDepending(false).Cast<Comunicazioni>().ToList();
        }

        public Comunicazioni GetComunicazioneByIdMail(Int64 idMail)
        {
            V_Comunicazioni_Complete_Obj v = new V_Comunicazioni_Complete_Obj(context);
            return v.GetComunicazioneByIdMail(idMail);
        }

        public void Insert(long idSottotitolo, long idCanale, bool isToNotify, string mailNotifica,
            string utenteInserimento, IList<ComAllegato> allegati, string mailSender, string oggetto, string testo,
            IList<SendMail.Model.RubricaMapping.RubricaContatti> refs)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdInsertComunicazione;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    oCmd.Parameters.Add("V_REF_ID_SOTTOTITOLO", idSottotitolo);
                    oCmd.Parameters.Add("V_REF_ID_CANALE", idCanale);
                    oCmd.Parameters.Add("V_FLG_NOTIFICA", Convert.ToInt32(isToNotify).ToString());
                    oCmd.Parameters.Add("V_MAIL_NOTIFICA", mailNotifica);
                    oCmd.Parameters.Add("V_UTE_INS", utenteInserimento);

                    OracleParameter pAll = new OracleParameter("V_ALLEGATI", OracleDbType.Object);
                    pAll.UdtTypeName = "COM_ALLEGATO_LIST_TYPE";
                    oCmd.Parameters.Add(pAll);

                    if (allegati == null || allegati.Count == 0)
                    {
                        pAll.Value = ComAllegatoListType.Null;
                    }
                    else
                    {
                        ComAllegatoListTypeFactory allFac = new ComAllegatoListTypeFactory();
                        ComAllegatoListType allT = (ComAllegatoListType)allFac.CreateObject();
                        pAll.Value = allT;

                        if (allegati.All(a => a is ComAllegatoType))
                        {
                            allT.ComAllegati = allegati.Cast<ComAllegatoType>().ToArray();
                        }
                        else
                        {
                            allT.ComAllegati = allegati.Select(a => new ComAllegatoType(a)).ToArray();
                        }
                    }

                    oCmd.Parameters.Add("V_MAIL_SENDER", mailSender);
                    oCmd.Parameters.Add("V_MAIL_SUBJECT", oggetto);
                    oCmd.Parameters.Add("V_MAIL_TEXT", testo);
                    oCmd.Parameters.Add("V_FOLLOWS", null);

                    OracleParameter pRefs = new OracleParameter("V_RUBRICA_CONTATTI_LIST", OracleDbType.Object);
                    pRefs.UdtTypeName = "RUBR_CONTATTI_LIST_TYPE";
                    oCmd.Parameters.Add(pRefs);

                    if (refs == null || refs.Count == 0)
                    {
                        pRefs.Value = RubricaContattiListType.Null;
                    }
                    else
                    {
                        RubricaContattiListTypeFactory rubFac = new RubricaContattiListTypeFactory();
                        RubricaContattiListType rubLis = (RubricaContattiListType)rubFac.CreateObject();
                        pRefs.Value = rubLis;

                        if (refs.All(a => a is RubricaContattiType))
                        {
                            rubLis.RubricaContatti = refs.Cast<RubricaContattiType>().ToArray();
                        }
                        else
                        {
                            rubLis.RubricaContatti = refs.Select(a => new RubricaContattiType(a)).ToArray();
                        }
                    }

                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore inserimento mail. Dettaglio: " + ex.Message, 
                        "ERR_COM_002", 
                        string.Empty, 
                        string.Empty, 
                        ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = idCanale.ToString();
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
                //ErrorLogInfo error = new ErrorLogInfo();
                //error.freeTextDetails = ex.Message;
                //error.logCode = "ERR_COM_002";
                //_log.Error(error);
            }
        }

        public void UpdateFlussoComunicazione(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione)
        {
            using (IComFlussoDao dao = this.Context.DaoImpl.ComFlussoDao)
            {

                ComFlusso f = comunicazione.ComFlussi[tipoCanale].OrderBy(x => !x.IdFlusso.HasValue).ThenBy(x => x.IdFlusso).Last();
                if (f.IdFlusso.HasValue)
                {
                    try
                    {
                        dao.Update(f);
                    }
                    catch (Exception ex)
                    {
                        //TASK: Allineamento log - Ciro
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nell\'aggiornamento del flusso della comunicazione." + 
                                " Successiva chiamata al metodo di inserimento. Dettaglio: " 
                                + ex.Message, "ERR_COM_003", string.Empty, string.Empty, ex.InnerException);
                            mEx.addEnanchedInfosTag("DETAILS", new XElement("info",
                                new XElement("user_msg", "Errore nell\'aggiornamento del flusso della comunicazione." +
                                " Successiva chiamata al metodo di inserimento. Dettaglio: "
                                + ex.Message),
                                new XElement("exception",
                                    new XElement("message", ex.Message),
                                    new XElement("source", ex.Source),
                                    new XElement("stack", ex.StackTrace),
                                    new XElement("innerException", ex.InnerException)),
                                    new XElement("IdComunicazione",(comunicazione.IdComunicazione!=null)?comunicazione.IdComunicazione.ToString():" vuoto. "),
                                    new XElement("UniqueId",(comunicazione.UniqueId!=null)?comunicazione.UniqueId.ToString():" vuoto. ")).ToString(SaveOptions.DisableFormatting));
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            err.objectID = comunicazione.IdComunicazione.ToString();
                            _log.Error(err);
                            dao.Insert(f);
                            throw mEx;  //aggiunto il 26/02/2016
                        }
                        else
                        {
                            dao.Insert(f);
                            throw ex; //aggiunto il 26/02/2016
                        }
                        
                        //Codice Originario

                        //ErrorLogInfo error = new ErrorLogInfo();
                        //error.freeTextDetails = ex.Message;
                        //error.logCode = "ERR_COM_003";
                        //_log.Error(error);
                        //dao.Insert(f);
                    }
                }
                else
                    dao.Insert(f);
            }
        }

        public void UpdateAllegati(SendMail.Model.TipoCanale tipoCanale, Comunicazioni comunicazione)
        {
            this.Context.StartTransaction(this.GetType());

            try
            {
                using (IComAllegatoDao allDao = this.Context.DaoImpl.ComAllegatoDao)
                {
                    allDao.Update(comunicazione.ComAllegati);
                }
            }
            catch (Exception ex)
            {
                
                if (this.Context.TransactionRootElement == this.GetType())
                {
                    this.Context.RollBackTransaction(this.GetType());
                }

                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(String.Format("Errore update allegati. Dettaglio: {0}", ex.Message),
                        "ERR_COM_004", 
                        string.Empty, 
                        string.Empty, 
                        ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = comunicazione.IdComunicazione.ToString();
                    _log.Error(err);
                    throw mEx;
                }
                //ErrorLogInfo error = new ErrorLogInfo();
                //error.freeTextDetails = ex.Message;
                //error.logCode = "ERR_COM_004";
                //_log.Error(error);
                else
                    throw ex;
            }

            if (this.Context.TransactionRootElement == this.GetType())
            {
                this.Context.EndTransaction(this.GetType());
            }
        }

        public void UpdateMailBody(long idMail, string mailBody)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = "UPDATE MAIL_CONTENT SET MAIL_TEXT = :pMAIL_TEXT"
                                    + " WHERE ID_MAIL = :pID_MAIL";
                    cmd.BindByName = true;
                    cmd.Parameters.Add("pMAIL_TEXT", OracleDbType.Clob, mailBody, System.Data.ParameterDirection.Input);
                    cmd.Parameters.Add("pID_MAIL", OracleDbType.Decimal, idMail, System.Data.ParameterDirection.Input);
                    int risp = cmd.ExecuteNonQuery();
                    if (risp != 1)
                        throw new InvalidOperationException("Errore nell'aggiornamento del testo mail. IdMail: " + idMail);
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(String.Format("Errore aggiornamento body mail. >> {0}", e.Message),
                        "ERR_COM_022",
                        string.Empty,
                        string.Empty,
                        e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = idMail.ToString();
                    _log.Error(err);
                    throw mEx;
                }
                else throw e;
                //ErrorLogInfo error = new ErrorLogInfo();
                //error.freeTextDetails = e.Message;
                //error.logCode = "ERR_COM_022";
                //_log.Error(error);
            }
        }

        #endregion

        #region IDao<Comunicazioni,long> Membri di

        public ICollection<Comunicazioni> GetAll()
        {
            List<Comunicazioni> l = null;
            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                ocmd.CommandText = cmdSelectRoot;

                using (OracleDataReader r = ocmd.ExecuteReader())
                {
                    if (r.HasRows)
                    {
                        l = new List<Comunicazioni>();
                        while (r.Read())
                        {
                            l.Add(((Comunicazioni)(r.GetValue(0))));
                        }
                    }
                }
            }
            return l;
        }

        public Comunicazioni GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Comunicazioni entity)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdInsertComunicazione;
                    oCmd.BindByName = true;
                    oCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    if (entity.RefIdSottotitolo.HasValue)
                    {
                        oCmd.Parameters.Add("v_ref_id_sottotitolo", entity.RefIdSottotitolo.Value);
                    }
                    else
                    {
                        oCmd.Parameters.Add("v_ref_id_sottotitolo", null);
                    }

                    if (entity.ComFlussi == null)
                    {
                        oCmd.Parameters.Add("v_ref_id_canale", ((int)(SendMail.Model.TipoCanale.MAIL)));
                    }
                    else
                    {
                        oCmd.Parameters.Add("v_ref_id_canale", ((int)(entity.ComFlussi.Last().Key)));
                    }

                    oCmd.Parameters.Add("v_flg_notifica", Convert.ToInt32(entity.IsToNotify).ToString());
                    oCmd.Parameters.Add("v_mail_notifica", entity.MailNotifica);
                    oCmd.Parameters.Add("v_ute_ins", entity.UtenteInserimento);
                    oCmd.Parameters.Add("v_orig_uid", entity.OrigUID);
                    oCmd.Parameters.Add("v_unique_id_mapper", entity.UniqueId);

                    OracleParameter pAllegati = new OracleParameter("v_allegati", OracleDbType.Object);
                    pAllegati.UdtTypeName = "COM_ALLEGATO_LIST_TYPE";
                    oCmd.Parameters.Add(pAllegati);
                    // aggiunta gestione protocollo
                    OracleParameter pProtocollo = new OracleParameter("V_PROT_LIST", OracleDbType.Object);
                    pProtocollo.UdtTypeName = "COM_PROTOCOLLO_LIST_TYPE";
                    ComProtocolloListTypeFactory protFac = new ComProtocolloListTypeFactory();
                    ComProtocolloListType protList = null;
                    if (entity.ComunicazioniProtocollo == null)
                    {
                        protList = ComProtocolloListType.Null;
                    }
                    else
                    {
                        protList = (ComProtocolloListType)protFac.CreateObject();
                        List<ComProtocolloType> l = new List<ComProtocolloType>();
                        l.Add(new ComProtocolloType(entity.ComunicazioniProtocollo));
                        protList.ComProtocolli = l.ToArray();
                    }
                    pProtocollo.Value = protList;
                    oCmd.Parameters.Add(pProtocollo);
                    // fine aggiunta
                    if (entity is ComunicazioniType)
                    {
                        pAllegati.Value = ((ComunicazioniType)entity).COM_ALLEGATI;
                    }
                    else
                    {
                        ComAllegatoListType allList = null;
                        if (entity.ComAllegati == null || entity.ComAllegati.Count == 0)
                        {
                            allList = ComAllegatoListType.Null;
                        }
                        else
                        {
                            ComAllegatoListTypeFactory allFac = new ComAllegatoListTypeFactory();
                            allList = (ComAllegatoListType)allFac.CreateObject();

                            if (entity.ComAllegati.All(x => x is ComAllegatoType))
                            {
                                allList.ComAllegati = entity.ComAllegati.Cast<ComAllegatoType>().ToArray();
                            }
                            else
                            {
                                allList.ComAllegati = entity.ComAllegati
                                                        .Select<ComAllegato, ComAllegatoType>(x => new ComAllegatoType(x)).ToArray();
                            }
                        }
                        pAllegati.Value = allList;
                    }

                    if (entity.MailComunicazione == null)
                    {
                        oCmd.Parameters.Add("v_mail_sender", null);
                        oCmd.Parameters.Add("v_mail_subject", null);
                        oCmd.Parameters.Add("v_mail_text", null);
                    }
                    else
                    {
                        oCmd.Parameters.Add("v_mail_sender", entity.MailComunicazione.MailSender);
                        oCmd.Parameters.Add("v_mail_subject", entity.MailComunicazione.MailSubject);
                        oCmd.Parameters.Add("v_mail_text", OracleDbType.Clob, entity.MailComunicazione.MailText, System.Data.ParameterDirection.Input);
                    }

                    oCmd.Parameters.Add("v_follows", entity.MailComunicazione.Follows);
                    oCmd.Parameters.Add("V_FOLDERID", entity.FolderId);
                    oCmd.Parameters.Add("V_FOLDERTIPO", entity.FolderTipo);
                    OracleParameter pContatti = new OracleParameter("v_rubrica_contatti_list", OracleDbType.Object);
                    pContatti.UdtTypeName = "FAXPEC.RUBR_CONTATTI_LIST_TYPE";
                    oCmd.Parameters.Add(pContatti);

                    RubricaContattiListTypeFactory contFac = new RubricaContattiListTypeFactory();
                    RubricaContattiListType contList = null;

                    if (entity.RubricaEntitaUsed == null || entity.RubricaEntitaUsed.Count == 0)
                    {
                        contList = RubricaContattiListType.Null;
                    }
                    else
                    {
                        contList = (RubricaContattiListType)contFac.CreateObject();
                        contList.RubricaContatti = (from r in entity.RubricaEntitaUsed
                                                    select new RubricaContattiType()
                                                    {
                                                        IdContact = r.IdEntUsed,
                                                        Mail = r.Mail,
                                                        TIPO_REF = r.TipoContatto.ToString()
                                                    }).ToArray();
                    }

                    pContatti.Value = contList;
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore inserimento comunicazione. Dettaglio: " + ex.Message,
                    "ERR_COM_032",
                    string.Empty,
                    string.Empty,
                    ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = (entity.IdComunicazione != null) ? entity.IdComunicazione.ToString() : "";
                    _log.Error(err);
                    throw mEx;
                }
                else throw ex;
                //ErrorLogInfo error = new ErrorLogInfo();
                //error.freeTextDetails = ex.Message;
                //error.logCode = "ERR_COM_032";
                //_log.Error(error);
            }
        }

        public void Update(Comunicazioni entity)
        {
            throw new NotImplementedException();
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
                base.CurrentConnection.Close();
        }

        #endregion

    }
}
