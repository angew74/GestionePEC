using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using SendMail.Model.RubricaMapping;
using SendMail.Model.WebserviceMappings;
using log4net;
using AutoMapper.QueryableExtensions;
using SendMail.Data.SQLServerDB.Mapping;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using ActiveUp.Net.Common.DeltaExt;
using System.Data.Common;
using System.Xml.Linq;

namespace SendMail.Data.SQLServerDB.Repository
{

   
    public class ComunicazioniSQLDb : IComunicazioniDao
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ComunicazioniSQLDb));

        public int GetCountComunicazioniInviate(Model.TipoCanale tipoCanale, string utente)
        {
            List<MailStatus> lstatus = new List<MailStatus>();
            lstatus.Add(MailStatus.SENT);
            return GetCountComunicazioni(tipoCanale, lstatus, true, utente);
        }

        public int GetCountComunicazioniNonInviate(Model.TipoCanale tipoCanale, string utente)
        {
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            return GetCountComunicazioni(tipoCanale, lStatus, false, utente);
        }

        public ICollection<Model.ComunicazioniMapping.Comunicazioni> GetComunicazioniInviate(Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            IList<Comunicazioni> lC = GetComunicazioniByStatus(tipoCanale, lStatus, false, minRec, maxRec, utente);
            if (lC != null)
            { return lC; }
            else { return null; }
        }

        public ICollection<Model.ComunicazioniMapping.Comunicazioni> GetComunicazioniNonInviate(Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            IList<Comunicazioni> lC = GetComunicazioniByNotStatus(tipoCanale, lStatus, false, minRec, maxRec, utente);
            if (lC != null)
            { return lC; }
            else { return null; }
        }

        private int GetCountComunicazioni(SendMail.Model.TipoCanale tipoCanale, List<MailStatus> status, bool include, string utente)
        {
            int cnt = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {

                    string sql = "SELECT count(*)"
                               + " FROM  [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf"
                                   + " INNER JOIN ("
                                       + " SELECT ref_id_com, MAX(data_operazione) AS dta_ope"
                                       + " FROM  [FAXPEC].[FAXPEC].[comunicazioni_flusso] "
                                       + " WHERE canale = '" + tipoCanale + "'"
                                       + " GROUP BY ref_id_com"
                                       + " ) gr"
                                   + " ON cf.ref_id_com = gr.ref_id_com AND cf.data_operazione = gr.dta_ope"
                               + " WHERE"
                               + " cf.stato_comunicazione_new"
                               + ((include == false) ? " NOT" : "")
                               + " IN (" + String.Join(", ", status.Select(s => ((int)s).ToString()).ToArray()) + ")"
                               + " AND cf.ute_ope = '" + utente + "'";
                    cnt = dbcontext.Database.SqlQuery<int>(sql).First();
                }
                catch (Exception ex)
                {
                    cnt = 0;
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
                }
            }
            return cnt;
        }

        private IList<Comunicazioni> GetComunicazioniByNotStatus(TipoCanale tipoCanale, List<MailStatus> status, bool include, int? minRec, int? maxRec, string utente)
        {

            List<Comunicazioni> lComunicazioni = new List<Comunicazioni>();
            int skip = 0;
            if (minRec > 0)
            { skip = (int)(minRec - 1); }
            int take = (int)(maxRec - minRec);
            string[] stati = null;
            if (status.Count == 0)
            {
                 stati = Enum.GetValues(typeof(MailStatus))
                      .Cast<int>()
                      .Select(x => x.ToString())
                      .ToArray();
            }
            else
            {
                stati = status.ToArray().Cast<int>().Select(x => x.ToString()).ToArray();
            }
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                List<MAIL_CONTENT> l = new List<MAIL_CONTENT>();
                try
                {
                    l = (from c in dbcontext.COMUNICAZIONI_FLUSSO
                         join m in dbcontext.MAIL_CONTENT
                             on c.REF_ID_COM equals m.REF_ID_COM
                         join cm in dbcontext.COMUNICAZIONI
                         on c.REF_ID_COM equals cm.ID_COM
                         where m.MAIL_SENDER.ToUpper() == utente.ToUpper()
                         && c.CANALE.ToUpper() == tipoCanale.ToString().ToUpper()
                         && !(stati.Contains(c.STATO_COMUNICAZIONE_NEW))
                         orderby c.REF_ID_COM
                         select m).Skip(skip).Take(take).ToList();
                    foreach (MAIL_CONTENT com in l)
                    {
                        Comunicazioni coMp = AutoMapperConfiguration.fromComunicazioniCompleteToDto(com);
                        lComunicazioni.Add(coMp);
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
                        _log.Error(err);
                        throw mEx;
                    }
                    else throw ex;
                }
            }
            return lComunicazioni;
        }

        private IList<Comunicazioni> GetComunicazioniByStatus(TipoCanale tipoCanale, List<MailStatus> status, bool include, int? minRec, int? maxRec, string utente)
        {

            List<Comunicazioni> lComunicazioni = new List<Comunicazioni>();
            int skip = (int)(minRec - 1);
            int take = (int)(maxRec - minRec);
            string[] stati = Enum.GetValues(typeof(MailStatus))
                    .Cast<string>()
                    .Select(x => x.ToString())
                    .ToArray();
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                List<MAIL_CONTENT> l = new List<MAIL_CONTENT>();
                try
                {
                    l = (from c in dbcontext.COMUNICAZIONI_FLUSSO
                         join m in dbcontext.MAIL_CONTENT
                             on c.REF_ID_COM equals m.REF_ID_COM
                         join cm in dbcontext.COMUNICAZIONI
                         on c.REF_ID_COM equals cm.ID_COM
                         where m.MAIL_SENDER == utente.ToUpper()
                         && c.CANALE == tipoCanale.ToString()
                         && (stati.Contains(c.STATO_COMUNICAZIONE_NEW))
                         orderby c.REF_ID_COM
                         select m).Skip(skip).Take(take).ToList();
                    foreach (MAIL_CONTENT com in l)
                    {
                        Comunicazioni coMp = AutoMapperConfiguration.fromComunicazioniCompleteToDto(com);
                        lComunicazioni.Add(coMp);
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
                        _log.Error(err);
                        throw mEx;
                    }
                    else throw ex;
                }
            }
            return lComunicazioni;
        }

        public ICollection<Model.ComunicazioniMapping.Comunicazioni> GetComunicazioniDaInviare(Model.TipoCanale tipoCanale, int? minRec, int? maxRec, string utente)
        {
            List<MailStatus> lStatus = new List<MailStatus>();
            lStatus.Add(MailStatus.SENT);
            lStatus.Add(MailStatus.ERROR);
            lStatus.Add(MailStatus.CANCELLED);
            IList<Comunicazioni> lC = GetComunicazioniByNotStatus(tipoCanale, lStatus, false, minRec, maxRec, utente);
            if (lC != null)
            { return lC; }
            else { return null; }
        }

        public ICollection<Model.ComunicazioniMapping.Comunicazioni> GetComunicazioniConAllegati()
        {
            List<Comunicazioni> lComunicazioni = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    List<MAIL_CONTENT> l = (from e in dbcontext.MAIL_CONTENT
                                            join f in dbcontext.COMUNICAZIONI
                                            on e.REF_ID_COM equals f.ID_COM
                                            where (from a in dbcontext.COMUNICAZIONI_ALLEGATI
                                                   where a.REF_ID_COM == f.ID_COM
                                                   select a).Count() > 0
                                            select e).ToList();
                    foreach (MAIL_CONTENT m in l)
                    {
                        Comunicazioni c = AutoMapperConfiguration.fromComunicazioniCompleteToDto(m);
                        lComunicazioni.Add(c);
                    }

                }
                catch
                {
                    lComunicazioni = null;
                }
            }
            return lComunicazioni;
        }

        public ICollection<Model.ComunicazioniMapping.Comunicazioni> GetComunicazioniSenzaAllegati()
        {
            List<Comunicazioni> lComunicazioni = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    List<MAIL_CONTENT> l = (from e in dbcontext.MAIL_CONTENT
                                            join f in dbcontext.COMUNICAZIONI
                                            on e.REF_ID_COM equals f.ID_COM
                                            where (from a in dbcontext.COMUNICAZIONI_ALLEGATI
                                                   where a.REF_ID_COM == f.ID_COM
                                                   select a).Count() == 0
                                            select e).ToList();
                    foreach (MAIL_CONTENT m in l)
                    {
                        Comunicazioni c = AutoMapperConfiguration.fromComunicazioniCompleteToDto(m);
                        lComunicazioni.Add(c);
                    }

                }
                catch
                {
                    lComunicazioni = null;
                }
            }
            return lComunicazioni;
        }

        public Model.ComunicazioniMapping.Comunicazioni GetComunicazioneByIdMail(long idMail)
        {
            Comunicazioni comunicazione = new Comunicazioni();
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    MAIL_CONTENT content = dbcontext.MAIL_CONTENT.Where(x => x.ID_MAIL == idMail).FirstOrDefault();
                    if (content != null && content.ID_MAIL > 0)
                    {
                        comunicazione = AutoMapperConfiguration.fromComunicazioniCompleteToDto(content);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return comunicazione;
        }

        public void Insert(long idSottotitolo, long idCanale, bool isToNotify, string mailNotifica, string utenteInserimento, IList<Model.ComunicazioniMapping.ComAllegato> allegati, string mailSender, string oggetto, string testo, IList<Model.RubricaMapping.RubricaContatti> refs)
        {
            throw new NotImplementedException();
        }

        public void UpdateFlussoComunicazione(Model.TipoCanale tipoCanale, Model.ComunicazioniMapping.Comunicazioni comunicazione)
        {
            ComFlusso f = comunicazione.ComFlussi[tipoCanale].OrderBy(x => !x.IdFlusso.HasValue).ThenBy(x => x.IdFlusso).Last();
            using (var dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        if (f.IdFlusso.HasValue)
                        {
                            COMUNICAZIONI_FLUSSO flusso = AutoMapperConfiguration.FromComFlussoToDto(f);
                            COMUNICAZIONI_FLUSSO old_flusso = dbcontext.COMUNICAZIONI_FLUSSO.Find(f.IdFlusso);
                            dbcontext.COMUNICAZIONI_FLUSSO.Remove(old_flusso);
                            dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);

                        }
                        else
                        {
                            COMUNICAZIONI_FLUSSO flusso = AutoMapperConfiguration.FromComFlussoToDto(f);
                            dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);
                        }

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
                                    new XElement("IdComunicazione", (comunicazione.IdComunicazione != null) ? comunicazione.IdComunicazione.ToString() : " vuoto. "),
                                    new XElement("UniqueId", (comunicazione.UniqueId != null) ? comunicazione.UniqueId.ToString() : " vuoto. ")).ToString(SaveOptions.DisableFormatting));
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            err.objectID = comunicazione.IdComunicazione.ToString();
                            _log.Error(err);
                            COMUNICAZIONI_FLUSSO flusso = AutoMapperConfiguration.FromComFlussoToDto(f);
                            dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);
                            throw mEx;  //aggiunto il 26/02/2016
                        }
                        else
                        {
                            COMUNICAZIONI_FLUSSO flusso = AutoMapperConfiguration.FromComFlussoToDto(f);
                            dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);
                            throw ex; //aggiunto il 26/02/2016
                        }
                    }
                    finally
                    {
                        dbcontext.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                }
            }
        }

        public void UpdateAllegati(Model.TipoCanale tipoCanale, Model.ComunicazioniMapping.Comunicazioni comunicazione)
        {
            using (var dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (ComAllegato comAllegato in comunicazione.ComAllegati)
                        {
                            COMUNICAZIONI_ALLEGATI allegato = AutoMapperConfiguration.FromComAllegatoToDto(comAllegato);
                            COMUNICAZIONI_ALLEGATI allegato_old = dbcontext.COMUNICAZIONI_ALLEGATI.Find(comAllegato.IdAllegato);
                            dbcontext.COMUNICAZIONI_ALLEGATI.Remove(allegato_old);
                            dbcontext.COMUNICAZIONI_ALLEGATI.Add(allegato);
                        }
                        dbcontext.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
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
                        else
                            throw ex;
                    }
                }
            }
        }

        public void UpdateMailBody(long idMail, string mailBody)
        {
            using (var dbcontext = new FAXPECContext())
            {
                try
                {
                    MAIL_CONTENT content = dbcontext.MAIL_CONTENT.Where(x => x.ID_MAIL == idMail).FirstOrDefault();
                    content.MAIL_TEXT = mailBody;
                    dbcontext.SaveChanges();
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
                }
            }
        }

        public ICollection<Model.WebserviceMappings.StatoComunicazioneItem> GetStatoComunicazione(string originalUid)
        {
            string cmdComStatus = "select c.id_com as ID, c.ref_id_sottotitolo as ID_SOTTOTITOLO, " +
            "   cs.sottotitolo " +
            "   as SOTTOTITOLO,cf.data_operazione as DATA_INS, cf3.stato_comunicazione_new as " +
            "   STATO from  [FAXPEC].[FAXPEC].[comunicazioni] c inner join  [FAXPEC].[FAXPEC].[comunicazioni_sottotitoli] cs on c.ref_id_sottotitolo " +
            "    = cs.id_sottotitolo inner join  [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf on c.id_com=cf.ref_id_com inner " +
            "   join (select ref_id_com, data_operazione ,stato_comunicazione_new from  [FAXPEC].[FAXPEC].[comunicazioni_flusso]  " +
            "   where (ref_id_com, data_operazione) in ( select ref_id_com, max(data_operazione) from comunicazioni_flusso " +
            "   where ref_id_com in(select id_com " +
            "   from  [FAXPEC].[FAXPEC].[comunicazioni] where comunicazioni.orig_uid = :p_ORIG_UID)  " +
            "   group by (ref_id_com)))cf3 on cf3.ref_id_com=c.id_com where cf.stato_comunicazione_old is null " +
            "   and c.id_com in(select id_com from  [FAXPEC].[FAXPEC].[comunicazioni] where comunicazioni.orig_uid ='" + originalUid + "')";

            ICollection<StatoComunicazioneItem> list = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                var oCmd = dbcontext.Database.Connection.CreateCommand();
                oCmd.CommandText = cmdComStatus;
                try
                {
                    using (DbDataReader r = oCmd.ExecuteReader())
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
                            ((originalUid != null) ? originalUid : " vuoto! "), e0.Message),
                            "ORA_ERR004",
                            string.Empty,
                            string.Empty,
                            e0.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(me);
                        err.objectID = (originalUid != null) ? originalUid : "";
                        _log.Error(err);
                        throw me;
                    }
                    else throw e0;
                }
            }
            return list;
        }

        public ICollection<Model.WebserviceMappings.StatoComunicazioneItem> GetComunicazioniByProtocollo(Model.ComunicazioniMapping.ComunicazioniProtocollo prot)
        {
            ICollection<StatoComunicazioneItem> list = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                string s = "select c.id_com as ID, c.ref_id_sottotitolo as ID_SOTTOTITOLO, cs.sottotitolo " +
                                                " as SOTTOTITOLO,cf.data_operazione as DATA_INS, cf3.stato_comunicazione_new as STATO " +
                                                " from  [FAXPEC].[FAXPEC].[comunicazioni] c inner join  [FAXPEC].[FAXPEC].[comunicazioni_sottotitoli] cs on c.ref_id_sottotitolo = cs.id_sottotitolo inner join comunicazioni_flusso cf on c.id_com=cf.ref_id_com " +
                                                " inner join (select ref_id_com, data_operazione ,stato_comunicazione_new from " +
                                                "  [FAXPEC].[FAXPEC].[comunicazioni]_flusso where (ref_id_com, data_operazione) in ( select ref_id_com, max(data_operazione) " +
                                                " from  [FAXPEC].[FAXPEC].[comunicazioni_flusso] where ref_id_com in (select ref_id_com from  [FAXPEC].[FAXPEC].[comunicazioni_protocollo] " +
                                                " where RESP_PROT_TIPO='" + prot.ResponseProtocolloTipo + "' AND RESP_PROT_ANNO=" + prot.ResponseProtocolloAnno + " AND RESP_PROT_NUMERO='" + prot.ResponseProtocolloNumero + "' AND PROT_IN_OUT='" + prot.ProtocolloInOut + "') " +
                                                " group by (ref_id_com)))cf3 on cf3.ref_id_com=c.id_com where cf.stato_comunicazione_old is null and c.id_com " +
                                                " in (select ref_id_com FROM   [FAXPEC].[FAXPEC].[comunicazioni_protocollo] t2 WHERE t2.RESP_PROT_TIPO='" + prot.ResponseProtocolloTipo +
                                                "' AND T2.RESP_PROT_ANNO=" + prot.ResponseProtocolloAnno + " AND T2.RESP_PROT_NUMERO='" + prot.ResponseProtocolloNumero + "' AND PROT_IN_OUT='" + prot.ProtocolloInOut + "')";

                var oCmd = dbcontext.Database.Connection.CreateCommand();
                oCmd.CommandText = s;
                try
                {
                    using (DbDataReader r = oCmd.ExecuteReader())
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
                    dbcontext.Dispose();
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(String.Format("Errore nell'estrazione delle emails per protocollo {0} {1}. Dettaglio: ",
                            prot.ResponseProtocolloAnno, prot.ResponseProtocolloNumero) + e.Message,
                            "ORA_ERR005",
                            string.Empty,
                            string.Empty,
                            e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.objectID = prot.RefIdCom.ToString(); //foreign key ID_COMUNICAZIONE
                        _log.Error(err);
                        throw mEx;
                    }
                    else throw e;
                }
            }
            return list;
        }

        public ICollection<Model.ComunicazioniMapping.Comunicazioni> GetAll()
        {
            List<Comunicazioni> l = new List<Comunicazioni>();
            using (FAXPECContext ocmd = new FAXPECContext())
            {
                List<MAIL_CONTENT> list = (from c in ocmd.MAIL_CONTENT
                                           select c).ToList();
                foreach (MAIL_CONTENT c in list)
                {
                    Comunicazioni com = AutoMapperConfiguration.fromComunicazioniCompleteToDto(c);
                    l.Add(com);
                }
            }
            return l;
        }

        public Model.ComunicazioniMapping.Comunicazioni GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Model.ComunicazioniMapping.Comunicazioni entity)
        {

            using (var dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        AutoMapperConfiguration.Configure();
                        //   COMUNICAZIONI comunicazione = AutoMapperConfiguration.fromComunicazioniToDto(entity,false,false);
                        COMUNICAZIONI comunicazione = AutoMapperConfiguration.fromComunicazioniToSimpleDto(entity);
                        decimal idComOld = 0;
                        string v_cod_app = string.Empty;
                        if (entity.MailComunicazione.Follows != null)
                        {
                            MAIL_CONTENT oldcontent = dbcontext.MAIL_CONTENT.Where(x => x.ID_MAIL == entity.MailComunicazione.Follows).FirstOrDefault();
                            idComOld = (oldcontent == null) ? 0 : oldcontent.REF_ID_COM;
                        }
                        if (idComOld == 0)
                        {
                            v_cod_app = dbcontext.COMUNICAZIONI_SOTTOTITOLI.Where(x => x.ID_SOTTOTITOLO == entity.RefIdSottotitolo).First().COMUNICAZIONI_TITOLI.APP_CODE;
                            entity.CodAppInserimento = v_cod_app;
                        }
                        else
                        {
                            COMUNICAZIONI old_comunicazione = dbcontext.COMUNICAZIONI.Where(x => x.ID_COM == idComOld).First();
                            entity.CodAppInserimento = old_comunicazione.COD_APP_INS;
                            entity.RefIdSottotitolo =long.Parse(old_comunicazione.REF_ID_SOTTOTITOLO.ToString());
                        }        
                        dbcontext.COMUNICAZIONI.Add(comunicazione);
                        dbcontext.SaveChanges();
                        decimal idcomnew = dbcontext.COMUNICAZIONI.Max(x => x.ID_COM);
                        entity.IdComunicazione = (long)idcomnew;
                        MAIL_CONTENT content = AutoMapperConfiguration.FromComunicazioniToMailContent(entity);
                        dbcontext.MAIL_CONTENT.Add(content);
                        dbcontext.SaveChanges();
                        decimal newidmail = dbcontext.MAIL_CONTENT.Select(c => c.ID_MAIL).DefaultIfEmpty(0).Max();
                        if (entity.ComFlussi != null && entity.ComFlussi.Count > 0)
                        {
                            var list = entity.ComFlussi.Where(x => x.Key == TipoCanale.MAIL).SelectMany(z => z.Value);
                            foreach (ComFlusso comFlusso in list)
                            {
                                COMUNICAZIONI_FLUSSO flusso = new COMUNICAZIONI_FLUSSO
                                {
                                    CANALE = comFlusso.Canale.ToString(),
                                    DATA_OPERAZIONE = (comFlusso.DataOperazione == null ? DateTime.Now : Convert.ToDateTime(comFlusso.DataOperazione)),
                                    STATO_COMUNICAZIONE_NEW = ((int)comFlusso.StatoComunicazioneNew).ToString(),
                                    STATO_COMUNICAZIONE_OLD = ((int)comFlusso.StatoComunicazioneOld).ToString(),
                                    UTE_OPE = comFlusso.UtenteOperazione
                                };

                                if (entity.IdComunicazione.HasValue)
                                { flusso.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                                else { flusso.REF_ID_COM = idcomnew; }
                                if (comFlusso.IdFlusso.HasValue)
                                { flusso.ID_FLUSSO = LinqExtensions.TryParseDouble(comFlusso.IdFlusso); }
                                dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);
                                dbcontext.SaveChanges();
                            }
                        }
                        else
                        {
                            COMUNICAZIONI_FLUSSO flusso = new COMUNICAZIONI_FLUSSO
                            {
                                CANALE = TipoCanale.MAIL.ToString(),
                                DATA_OPERAZIONE = System.DateTime.Now,
                                STATO_COMUNICAZIONE_NEW = ((int)(MailStatus.INSERTED)).ToString(),
                                STATO_COMUNICAZIONE_OLD = null,
                                UTE_OPE = entity.UtenteInserimento
                            };
                            flusso.REF_ID_COM = idcomnew;
                            dbcontext.COMUNICAZIONI_FLUSSO.Add(flusso);
                            dbcontext.SaveChanges();
                        }
                        if (entity.ComFlussiProtocollo != null && entity.ComFlussiProtocollo.Count > 0)
                        {
                            foreach (ComFlussoProtocollo comFlussoProtocollo in entity.ComFlussiProtocollo)
                            {
                                COMUNICAZIONI_FLUSSO_PROT flussoprotocollo = new COMUNICAZIONI_FLUSSO_PROT
                                {
                                    DATA_OPERAZIONE = (DateTime)comFlussoProtocollo.DataOperazione,
                                    STATO_NEW = LinqExtensions.TryParseByte(comFlussoProtocollo.StatoNew.ToString()),
                                    STATO_OLD = LinqExtensions.TryParseByte(comFlussoProtocollo.StatoOld.ToString()),
                                    UTE_OPE = comFlussoProtocollo.UtenteOperazione
                                };
                                if (entity.IdComunicazione.HasValue)
                                { flussoprotocollo.REF_ID_COM = LinqExtensions.TryParseInt(entity.IdComunicazione); }
                                else { flussoprotocollo.REF_ID_COM = idcomnew; }
                                dbcontext.COMUNICAZIONI_FLUSSO_PROT.Add(flussoprotocollo);
                            }
                        }
                        // gestione rubrica
                        if (entity.RubricaEntitaUsed != null && entity.RubricaEntitaUsed.Count > 0)
                        {
                            foreach (RubrEntitaUsed entitaused in entity.RubricaEntitaUsed)
                            {
                                // se ho usato la rubrica
                                if (entitaused.IdEntUsed != null && entitaused.IdEntUsed != 0)
                                {
                                    V_RUBR_CONTATTI v_rubr_contatti = dbcontext.V_RUBR_CONTATTI.Where(x => x.ID_CONTACT == entitaused.IdEntUsed).First();
                                    MAIL_REFS_NEW mailrefsnew = new MAIL_REFS_NEW()
                                    {
                                        REF_ID_MAIL = comunicazione.MAIL_CONTENT.First().ID_MAIL,
                                        TIPO_REF = entitaused.TipoContatto.ToString(),
                                        MAIL_DESTINATARIO = v_rubr_contatti.MAIL
                                    };
                                    dbcontext.MAIL_REFS_NEW.Add(mailrefsnew);
                                    COMUNICAZIONI_DESTINATARI destinatari = dbcontext.COMUNICAZIONI_DESTINATARI.Where(x => x.CAP == v_rubr_contatti.CAP &&
                                        x.CIVICO == v_rubr_contatti.CIVICO && x.COD_FIS == v_rubr_contatti.COD_FIS
                                        && x.COD_ISO_STATO == v_rubr_contatti.COD_ISO_STATO && x.COGNOME == v_rubr_contatti.COGNOME
                                        && x.COMUNE == v_rubr_contatti.COMUNE && x.CONTACT_REF == v_rubr_contatti.CONTACT_REF
                                        && x.FAX == v_rubr_contatti.FAX && x.ID_REFERRAL == v_rubr_contatti.REF_ID_REFERRAL
                                        && x.INDIRIZZO == v_rubr_contatti.INDIRIZZO && x.MAIL == v_rubr_contatti.MAIL
                                        && x.NOME == v_rubr_contatti.NOME && x.P_IVA == v_rubr_contatti.P_IVA && x.RAGIONE_SOCIALE == v_rubr_contatti.RAGIONE_SOCIALE
                                        && x.REFERRAL_TYPE == v_rubr_contatti.REFERRAL_TYPE && x.SIGLA_PROV == v_rubr_contatti.SIGLA_PROV
                                        && x.TELEFONO == v_rubr_contatti.TELEFONO && x.UFFICIO == v_rubr_contatti.UFFICIO).FirstOrDefault();
                                    if (destinatari.ID_REFERRAL > 0)
                                    {
                                        COMUNICAZIONI_ENTITA_USED comunicazioni_entita_used = new COMUNICAZIONI_ENTITA_USED()
                                        {
                                            REF_ID_COMUNICAZIONE = comunicazione.ID_COM,
                                            REF_ID_ENTITA = destinatari.ID_REFERRAL,
                                            REF_ID_ENT_USED = destinatari.ID_COM_DEST
                                        };
                                        dbcontext.COMUNICAZIONI_ENTITA_USED.Add(comunicazioni_entita_used);
                                    }
                                    else
                                    {
                                        COMUNICAZIONI_DESTINATARI destinatari_new = AutoMapperConfiguration.fromRubrContattiToComunicazioniDestinatari(v_rubr_contatti);
                                        dbcontext.COMUNICAZIONI_DESTINATARI.Add(destinatari);
                                    }
                                }
                                // se ho il contatto custom 
                                else
                                {
                                    MAIL_REFS_NEW mailrefsnew = new MAIL_REFS_NEW()
                                    {
                                        REF_ID_MAIL = newidmail,
                                        TIPO_REF = entitaused.TipoContatto.ToString(),
                                        MAIL_DESTINATARIO = entitaused.Mail
                                    };
                                    dbcontext.MAIL_REFS_NEW.Add(mailrefsnew);
                                    dbcontext.SaveChanges();
                                }
                            }
                        }
                        // fine rubrica                       
                        //inizio allegati
                        if (entity.ComAllegati != null && entity.ComAllegati.Count > 0)
                        {
                            foreach (ComAllegato a in entity.ComAllegati)
                            {
                                COMUNICAZIONI_ALLEGATI all = AutoMapperConfiguration.FromComAllegatoToDto(a);
                                all.REF_ID_COM = idcomnew;
                                dbcontext.COMUNICAZIONI_ALLEGATI.Add(all);
                            }
                            dbcontext.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    // fine try            
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
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

                    }
                    // fine using  transaction
                } // fine using
            }
        }

        public void Update(Model.ComunicazioniMapping.Comunicazioni entity)
        {
            throw new NotImplementedException();
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
        // ~ComunicazioniSQLDb() {
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


    }
}

