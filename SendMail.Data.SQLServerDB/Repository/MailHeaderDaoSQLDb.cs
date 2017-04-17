using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Logging.Mail;
using Com.Delta.Web.Cache;
using log4net;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.SQLServerDB.Mapping;
using SendMail.Model;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Repository
{
    public class MailHeaderDaoSQLDb : IMailHeaderDao
    {
        private static readonly ILog log = LogManager.GetLogger("MailHeaderDaoOracleDb");

        #region Private query strings

        private string queryInboxCountBase = "SELECT count(*) AS \"TOT\"  FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] ";
        private const string selectInboxBase = "SELECT MAIL_SERVER_ID AS \"MAIL_ID\", MAIL_ACCOUNT, MAIL_FROM, MAIL_TO, MAIL_CC, MAIL_CCN"
                                           + ", MAIL_SUBJECT, MAIL_TEXT, DATA_INVIO, DATA_RICEZIONE, STATUS_SERVER"
                                           + ", STATUS_MAIL, FLG_RATING, FLG_ATTACHMENTS,FOLDERID,FOLDERTIPO,"
                                           + "(SELECT DISTINCT LAST_VALUE(UTE_OPE) OVER (ORDER BY DATA_OPERAZIONE"
                                                                                     + " ROWS BETWEEN UNBOUNDED PRECEDING"
                                                                                     + " AND UNBOUNDED FOLLOWING)"
                                           + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] MF"
                                           + " WHERE MF.REF_ID_MAIL = MI.ID_MAIL) AS \"UTENTE\","
                                           + " msg_length"
                                           + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] MI";

        private const string selectInboxBaseModificata = "SELECT MAIL_ID,MAIL_ACCOUNT,MAIL_FROM, MAIL_TO,MAIL_CC,MAIL_CCN,MAIL_SUBJECT,MAIL_TEXT, DATA_INVIO,DATA_RICEZIONE, STATUS_SERVER, STATUS_MAIL,FOLDERID,FOLDERTIPO, FLG_RATING, FLG_ATTACHMENTS,  UTENTE,MSG_LENGTH,  ROWNUM AS ROWNUMBER FROM ( " +
            "SELECT MAIL_SERVER_ID AS \"MAIL_ID\", MAIL_ACCOUNT, MAIL_FROM, MAIL_TO, MAIL_CC, MAIL_CCN"
                                     + ", MAIL_SUBJECT, MAIL_TEXT, DATA_INVIO, DATA_RICEZIONE, STATUS_SERVER"
                                     + ", STATUS_MAIL,FOLDERID,FOLDERTIPO, FLG_RATING, FLG_ATTACHMENTS,"
                                     + "(SELECT DISTINCT LAST_VALUE(UTE_OPE) OVER (ORDER BY DATA_RICEZIONE DESC "
                                                                               + " ROWS BETWEEN UNBOUNDED PRECEDING"
                                                                               + " AND UNBOUNDED FOLLOWING)"
                                     + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] MF"
                                     + " WHERE MF.REF_ID_MAIL = MI.ID_MAIL) AS \"UTENTE\" , msg_length"
                                     + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] MI";

        /// <summary>
        /// DA MODIFICARE PER REF_ID_FLUSSO
        /// </summary>
        //private readonly string queryOutboxCountBase = "SELECT COUNT(*) AS \"TOT\""
        //                                          + " FROM MAIL_CONTENT m"
        //                                          + " WHERE"
        //                                          + " {0} EXISTS (SELECT 1"
        //                                                      + " FROM COMUNICAZIONI_FLUSSO FL"
        //                                                      + " WHERE FL.REF_ID_COM = M.REF_ID_COM"
        //                                                      + " AND STATO_COMUNICAZIONE_NEW IN ('" + (int)MailStatus.CANCELLED + "','" + (int)MailStatus.ARCHIVIATA + "'))";

        // nuova da provare
        private readonly string queryOutbuxCountBaseNew = "SELECT COUNT(*) AS \"TOT\""
                                                  + " FROM  [FAXPEC].[FAXPEC].[MAIL_CONTENT] m"
                                                  + " WHERE FOLDERID={0} AND FOLDERTIPO='{1}' ";

        //DA MODIFICARE PER REF_ID_FLUSSO
        private readonly string selectOutboxBase = "SELECT TO_CHAR(mc.ID_MAIL) AS \"MAIL_ID\", mc.MAIL_SENDER AS \"MAIL_FROM\""
                                              + ", mc.MAIL_SUBJECT, substr(mc.MAIL_TEXT, 1 ,150) as \"MAIL_TEXT\""
                                              + ", r.\"MAIL_TO\" "
                                               + ", '' as \"MAIL_CC\""
                                               + ", '' as \"MAIL_CCN\""
                                              + ", mc.FOLDERID, mc.FOLDERTIPO "
                                              + ", FL.STATO_COMUNICAZIONE_NEW "
                                               + " AS \"STATUS_MAIL\""
                                              + ", FL1.DATA_OPERAZIONE "
                                              + " AS \"DATA_INVIO\""
                                              + ",  NULL AS \"DATA_RICEZIONE\""
                                              + ", MC.FLG_ANNULLAMENTO AS \"STATUS_SERVER\""
                                             + ", NULL as \"FLG_RATING\""
                                             + ", DECODE((SELECT COUNT(*)"
                                               + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_ALLEGATI] AL "
                                               + " WHERE AL.REF_ID_COM = MC.REF_ID_COM), 0, '0', '1') AS \"FLG_ATTACHMENTS\""
                                             + ", (SELECT UTE_OPE"
                                             + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FU "
                                            + " WHERE FU.REF_ID_COM = MC.REF_ID_COM"
                                            + " AND STATO_COMUNICAZIONE_NEW = '" + (int)MailStatus.INSERTED + "'"
                                            + ") AS \"UTENTE\" ,0 AS MSG_LENGTH "
                                           + " FROM MAIL_CONTENT MC"
                                            + " LEFT OUTER JOIN [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FL ON  MC.REF_ID_FLUSSO_ATTUALE=FL.ID_FLUSSO "
                                             + " LEFT OUTER JOIN  [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FL1 ON MC.REF_ID_FLUSSO_INSERIMENTO=FL1.ID_FLUSSO "
                                             + " LEFT OUTER JOIN (SELECT mm.REF_ID_MAIL, \"MAIL_TO\""
                                              + " FROM"
                                                    + " (SELECT RF.REF_ID_MAIL, RF.TIPO_REF, RF.MAIL_DESTINATARIO"
                                                    + " FROM MAIL_REFS_NEW RF)"
                                                    + " PIVOT"
                                                    + " (listagg(MAIL_DESTINATARIO, ';') within group (order by TIPO_REF)"
                                                    + " FOR TIPO_REF in"
                                                        + " ('TO' as \"MAIL_TO\")) mm  "
                                                    + " order by REF_ID_MAIL) r"
                                                + " ON mc.id_mail = r.ref_id_mail";

        #endregion

        string APP_CODE = (ConfigurationManager.GetSection("ApplicationCode") as ApplicationCodeConfigSection).AppCode;

        #region IMailHeaderDao Membri di

        public ICollection<string> GetAllUIDsByAccount(string account)
        {
            List<string> uids = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var r = dbcontext.Database.Connection.CreateCommand())
                {
                    r.CommandText = "SELECT MAIL_SERVER_ID FROM MAIL_INBOX WHERE MAIL_ACCOUNT = '" + account + "'";
                    try
                    {
                        using (DbDataReader reader = r.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                uids = new List<string>();
                                while (reader.Read())
                                {
                                    uids.Add(reader.GetString(0));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        uids = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel ripristino delle emails inviate per account " + account + " E060 Dettagli Errore: " + ex.Message,
                                "ERR_060", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }
                }
            }
            return uids;
        }
        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsReceivedByAccount(string account, string folder, int da, int per)
        {
            ResultList<MailHeaderExtended> res = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {                
                queryInboxCountBase += " WHERE MAIL_ACCOUNT = '" + account + "' ";
                if (folder != "99")
                {
                    queryInboxCountBase += " AND FOLDERID=" + int.Parse(folder);
                }
                try
                {
                    using (var r = dbcontext.Database.Connection.CreateCommand())
                    {
                        r.CommandText = queryInboxCountBase;
                        r.Connection.Open();
                        int tot = Convert.ToInt32(r.ExecuteScalar());
                        r.Connection.Close();
                        res = new ResultList<MailHeaderExtended>();
                        res.Da = da;
                        res.Per = (tot > per) ? per : tot;
                        res.Totale = (tot > per) ? tot : per;
                        if (tot == default(int))
                        {
                            res.List = null;
                            return res;
                        }
                    }              
                }
                catch (Exception ex)
                {
                    res = null;
                    dbcontext.Dispose();
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento delle emails ricevute per account " + account + " E061 Dettagli Errore: " + ex.Message,
                            "ERR_061", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                        throw mEx;
                    }
                    else
                        throw ex;
                }

                string query = "WITH T AS (SELECT ID_MAIL, ROW_NUMBER() OVER (ORDER BY DATA_RICEZIONE DESC) AS RN FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] "
                    + " WHERE MAIL_ACCOUNT='" + account + "' "
                    + " AND FOLDERID=" + int.Parse(folder) + ") "
                    + selectInboxBase
                    + " WHERE ID_MAIL IN (SELECT ID_MAIL FROM T WHERE RN BETWEEN " + da + " AND " + (da + per - 1) + ")"
                    + " ORDER BY DATA_RICEZIONE DESC";

                try
                {
                    using (var r = dbcontext.Database.Connection.CreateCommand())
                    {
                        r.CommandText = query;
                        r.Connection.Open();                      
                        using (var reader = r.ExecuteReader())
                        {
                            res.List = new List<MailHeaderExtended>();
                            while (reader.Read())
                            {
                                res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(reader));
                            }

                        }
                        r.Connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    res.List = null;
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(
                            "Errore nel caricamento delle email ricevute per account E038 Dettagli Errore: " + ex.Message,
                            "ERR_038",
                            string.Empty,
                            string.Empty,
                            ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                        throw mEx;
                    }
                    else throw ex;

                }
            }
            return res;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsSentByAccount(string account, string folder, int da, int per)
        {
            ResultList<MailHeaderExtended> res = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    oCmd.CommandText = String.Format(queryOutbuxCountBaseNew, folder, 'O') + " AND m.MAIL_SENDER = '" + account + "'";
                    try
                    {
                        oCmd.Connection.Open();
                        int tot = Convert.ToInt32(oCmd.ExecuteScalar());
                        res = new ResultList<MailHeaderExtended>();
                        res.Da = da;
                        res.Per = (tot > per) ? per : tot;
                        res.Totale = (tot > per) ? tot : per;
                        if (tot == default(int))
                        {
                            res.List = null;
                            return res;
                        }
                        oCmd.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        res = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento delle email inviate per account " + account + " E042 Dettagli Errore: " + ex.Message,
                                "ERR_042", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }

                    oCmd.CommandText = "WITH M_CON AS (SELECT REF_ID_COM,"
                                                          + " ROW_NUMBER() OVER (ORDER BY REF_ID_COM DESC) AS RN"
                                                   + " FROM [FAXPEC].[FAXPEC].[MAIL_CONTENT] MC"
                                                   + " WHERE MAIL_SENDER = '" + account +
                                     "' AND FOLDERTIPO='O' AND FOLDERID=" + folder + ")"
                                    + " SELECT TO_CHAR(MC.ID_MAIL) AS \"MAIL_ID\""
                                          + ", MC.MAIL_SENDER AS \"MAIL_FROM\""
                                          + ", MC.MAIL_SUBJECT"
                                          + ", SUBSTR(MC.MAIL_TEXT, 1 ,150) as \"MAIL_TEXT\""
                                          + ", FOLDERID "
                                          + ", FOLDERTIPO "
                                          + ", R.\"MAIL_TO\""
                                          + ", '' as \"MAIL_CC\""
                                          + ", '' as \"MAIL_CCN\""
                                          + ", FL.STATO_COMUNICAZIONE_NEW "
                                          + " AS \"STATUS_MAIL\""
                                          + ", FL1.DATA_OPERAZIONE "
                                          + " AS \"DATA_INVIO\""
                                          + ", (SELECT MAX(DATA_OPERAZIONE)"
                                            + " FROM COMUNICAZIONI_FLUSSO"
                                            + " WHERE REF_ID_COM = MC.REF_ID_COM"
                                            + " AND STATO_COMUNICAZIONE_NEW IN ('" + (int)MailStatus.AVVENUTA_CONSEGNA + "', '" + (int)MailStatus.ERRORE_CONSEGNA + "')"
                                            + ") AS \"DATA_RICEZIONE\""
                                          + ", MC.FLG_ANNULLAMENTO AS \"STATUS_SERVER\""
                                          + ", NULL as \"FLG_RATING\""
                                          + ", DECODE((SELECT COUNT(*)"
                                                   + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_ALLEGATI] al "
                                                   + " WHERE al.REF_ID_COM = MC.REF_ID_COM), 0, '0', '1') AS \"FLG_ATTACHMENTS\""
                                          + ", (SELECT UTE_OPE"
                                            + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] fu "
                                            + " WHERE fu.REF_ID_COM = MC.REF_ID_COM"
                                            + " AND STATO_COMUNICAZIONE_NEW = '" + (int)MailStatus.INSERTED + "'"
                                            + ") AS \"UTENTE\" ,0 AS MSG_LENGTH "
                                    + " FROM [FAXPEC].[FAXPEC].[MAIL_CONTENT] MC"
                                    + " LEFT OUTER JOIN [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FL ON  MC.REF_ID_FLUSSO_ATTUALE=FL.ID_FLUSSO "
                                    + " LEFT OUTER JOIN [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FL1 ON MC.REF_ID_FLUSSO_INSERIMENTO=FL1.ID_FLUSSO "
                                    + " LEFT OUTER JOIN (SELECT REF_ID_MAIL, \"MAIL_TO\""
                                                     + " FROM (SELECT REF_ID_MAIL, TIPO_REF, MAIL_DESTINATARIO"
                                                           + " FROM [FAXPEC].[FAXPEC].[MAIL_REFS_NEW])"
                                                     + " PIVOT"
                                                     + " (LISTAGG(MAIL_DESTINATARIO, ';') within group (order by TIPO_REF)"
                                                     + " FOR TIPO_REF in"
                                                     + " ('TO' as \"MAIL_TO\"))"
                                                     + " ORDER BY REF_ID_MAIL) R"
                                    + " ON MC.ID_MAIL = R.REF_ID_MAIL"
                                    + " WHERE MAIL_SENDER = '" + account +
                                     "' AND FOLDERTIPO='O' AND FOLDERID=" + folder
                                    + " AND MC.REF_ID_COM  IN (SELECT REF_ID_COM"
                                                            + " FROM M_CON"
                                                            + " WHERE RN BETWEEN " + da + " AND " + (da + per - 1) + ")"
                                    + " ORDER BY DATA_INVIO DESC";

                    try
                    {
                        res.List = new List<MailHeaderExtended>();
                        oCmd.Connection.Open();
                        using (DbDataReader r = oCmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                            }
                        }
                        oCmd.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        res.List = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento delle email inviate per account " + account + " >> E043 >> Dettagli Errore: " + ex.Message,
                                "ERR_043", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }
                }
            }
            return res;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsCanceledByAccount(string account, string idfolder, int da, int per)
        {
            ResultList<MailHeaderExtended> res = new ResultList<MailHeaderExtended>();
            List<Folder> list = getAllFolders();
            int id = 0;
            int.TryParse(idfolder, out id);
            var t = (from a in list
                     where a.Id == id
                     select a).ToList();
            switch (t[0].IdNome)
            {
                case "1":
                case "3":
                    res = GetAllMailsReceivedCanceledByAccount(account, idfolder, da, per);
                    break;
                case "2":
                    res = GetAllMailsSentCanceledByAccount(account, idfolder, da, per);
                    break;
                default:
                    break;
            }
            return res;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsReceivedArchivedByAccount(string account, string folder, int da, int per)
        {
            ResultList<MailHeaderExtended> result = null;
            StringBuilder sbCount = new StringBuilder("SELECT count(*) FROM ({0}) as subquery ");
            StringBuilder sbInbox = new StringBuilder("SELECT id_mail FROM [FAXPEC].[FAXPEC].[mail_inbox] ")
                .Append(" WHERE mail_account = '" + account + "'")
                .AppendFormat(" AND FOLDERID = '{0}'", folder)
              .Append(" AND FOLDERTIPO='A' ");
            result = new ResultList<MailHeaderExtended>();
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {

                    using (var cmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        cmd.Connection.Open();
                        cmd.CommandText = string.Format(sbCount.ToString(), sbInbox.ToString());
                        int count = (int)cmd.ExecuteScalar();
                        result.Da = da;
                        result.Per = (count == 0) ? 0 : ((count - da + 1) < per ? (count - da + 1) : per);
                        if (count == 0)
                            return result;
                        result.Totale = count;
                        result.List = new List<MailHeaderExtended>(result.Per);
                        /*  PROVA */
                        string query = "WITH T AS (SELECT ID_MAIL, ROW_NUMBER() OVER (ORDER BY DATA_RICEZIONE DESC) AS RN FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] "
           + " WHERE MAIL_ACCOUNT='" + account + "' AND FOLDERTIPO='A' "
           + " AND FOLDERID=" + folder + " ) "
          + selectInboxBase
          + " WHERE ID_MAIL IN (SELECT ID_MAIL FROM T WHERE RN BETWEEN " + da + " AND " + (da + per - 1) + ")"
          + " ORDER BY DATA_RICEZIONE DESC";
                        cmd.CommandText = query;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.HasRows)
                            {
                                while (r.Read())
                                {
                                    result.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                                }
                            }
                        }
                        cmd.Connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    result.List = null;                    
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento delle email ricevute e archiviate per account " + account + " E046 Dettagli Errore: " + ex.Message,
                            "ERR_046", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                        throw mEx;
                    }
                    else
                        throw ex;
                }
            }
            return result;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsSentArchivedByAccount(string account, string folder, int da, int per)
        {
            ResultList<MailHeaderExtended> result = null;
            StringBuilder sbCount = new StringBuilder("SELECT count(*) FROM ({0}) as subquery ");
            StringBuilder sbFlussoOutboxNew = new StringBuilder(" SELECT ref_id_com from  [faxpec].[faxpec].MAIL_CONTENT ")
            .Append(" WHERE MAIL_SENDER = '" + account + "'")
            .Append(" AND FOLDERID = '" + folder + "'")
            .Append(" AND FOLDERTIPO = 'A' ");
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    result = new ResultList<MailHeaderExtended>();
                    using (var cmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        cmd.CommandText = string.Format(sbCount.ToString(), sbFlussoOutboxNew.ToString());
                        cmd.Connection.Open();
                        int count = (int)cmd.ExecuteScalar();
                        result.Totale = count;
                        result.Da = da;
                        result.Per = (count == 0) ? 0 : ((count - da + 1) < per ? (count - da + 1) : per);
                        if (count == 0)
                            return result;

                        result.List = new List<MailHeaderExtended>(result.Per);

                        cmd.CommandText = "WITH M_CON AS (SELECT REF_ID_COM,"
                                                        + " ROW_NUMBER() OVER (ORDER BY REF_ID_COM DESC) AS RN"
                                                 + " FROM [FAXPEC].[FAXPEC].[MAIL_CONTENT] MC"
                                                 + " WHERE MAIL_SENDER = '" + account +
                                   "' AND FOLDERTIPO='A' AND FOLDERID=" + folder + ")"
                                  + " SELECT TO_CHAR(MC.ID_MAIL) AS \"MAIL_ID\""
                                        + ", MC.MAIL_SENDER AS \"MAIL_FROM\""
                                        + ", MC.MAIL_SUBJECT"
                                        + ", SUBSTR(MC.MAIL_TEXT, 1 ,150) as \"MAIL_TEXT\""
                                        + ", FOLDERID "
                                        + ", FOLDERTIPO "
                                        + ", R.\"MAIL_TO\""
                                        + ", '' as \"MAIL_CC\""
                                        + ", '' as \"MAIL_CCN\""
                                        + ", FL.STATO_COMUNICAZIONE_NEW "
                                        + " AS \"STATUS_MAIL\""
                                        + ", FL1.DATA_OPERAZIONE "
                                        + " AS \"DATA_INVIO\""
                                        + ", (SELECT MAX(DATA_OPERAZIONE)"
                                          + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] "
                                          + " WHERE REF_ID_COM = MC.REF_ID_COM"
                                          + " AND STATO_COMUNICAZIONE_NEW IN ('" + (int)MailStatus.AVVENUTA_CONSEGNA + "', '" + (int)MailStatus.ERRORE_CONSEGNA + "')"
                                          + ") AS \"DATA_RICEZIONE\""
                                        + ", MC.FLG_ANNULLAMENTO AS \"STATUS_SERVER\""
                                        + ", NULL as \"FLG_RATING\""
                                        + ", DECODE((SELECT COUNT(*)"
                                                 + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_ALLEGATI] "
                                                 + " WHERE REF_ID_COM = MC.REF_ID_COM), 0, '0', '1') AS \"FLG_ATTACHMENTS\""
                                        + ", (SELECT UTE_OPE"
                                          + " FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] "
                                          + " WHERE REF_ID_COM = MC.REF_ID_COM"
                                          + " AND STATO_COMUNICAZIONE_NEW = '" + (int)MailStatus.INSERTED + "'"
                                          + ") AS \"UTENTE\" ,0 AS MSG_LENGTH "
                                  + " FROM MAIL_CONTENT MC"
                                  + " LEFT OUTER JOIN [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FL ON  MC.REF_ID_FLUSSO_ATTUALE=FL.ID_FLUSSO "
                                  + " LEFT OUTER JOIN  [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] FL1 ON MC.REF_ID_FLUSSO_INSERIMENTO=FL1.ID_FLUSSO "
                                  + " LEFT OUTER JOIN (SELECT REF_ID_MAIL, \"MAIL_TO\""
                                                   + " FROM (SELECT REF_ID_MAIL, TIPO_REF, MAIL_DESTINATARIO"
                                                         + " FROM [FAXPEC].[FAXPEC].[MAIL_REFS_NEW] )"
                                                   + " PIVOT"
                                                   + " (LISTAGG(MAIL_DESTINATARIO, ';') within group (order by TIPO_REF)"
                                                   + " FOR TIPO_REF in"
                                                   + " ('TO' as \"MAIL_TO\"))"
                                                   + " ORDER BY REF_ID_MAIL) R"
                                  + " ON MC.ID_MAIL = R.REF_ID_MAIL"
                                  + " WHERE MAIL_SENDER = '" + account +
                                   "' AND FOLDERTIPO='A' AND FOLDERID=" + folder
                                  + " AND MC.REF_ID_COM  IN (SELECT REF_ID_COM"
                                                          + " FROM M_CON"
                                                          + " WHERE RN BETWEEN " + da + " AND " + (da + per - 1) + ")"
                                  + " ORDER BY DATA_INVIO DESC";

                        try
                        {
                            result.List = new List<MailHeaderExtended>();
                            using (var r = cmd.ExecuteReader(CommandBehavior.SingleResult))
                            {
                                while (r.Read())
                                {
                                    result.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email inviate e archiviate per account " + account + " E047 Dettagli Errore: " + ex.Message,
                                    "ERR_047", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                        cmd.Connection.Close();
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    result.List = null;
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento delle email archiviate e inviate per account " + account + " E037 Dettagli Errore: " + ex.Message,
                            "ERR_037", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                        throw mEx;
                    }
                    else throw ex;
                }
            }
        }


        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetMailsByParams(string account, string folder, string parentFolder, Dictionary<Model.MailIndexedSearch, List<string>> searchValues, int da, int per)
        {
            ResultList<MailHeaderExtended> res = null;
            da = (da == 0) ? 1 : da;
            int a = da + per - 1;
            int totIn = 0, totOut = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    string cmdTextIn = "", cmdTextOut = "";
                    List<SqlParameter> parsIn = new List<SqlParameter>(),
                        parsOut = new List<SqlParameter>();
                    switch (parentFolder)
                    {
                        case "I":
                        case "A":
                        case "C":
                            if (folder != "6")
                            { cmdTextIn = ConstructInboxCountQuery(account, folder, searchValues, out parsIn); }
                            break;
                        case "O":
                        case "OA":
                        case "AO":
                            cmdTextOut = ConstructOutboxCountQuery(account, folder, searchValues, out parsOut);
                            break;
                        default:
                            if (folder == "6")
                            { cmdTextOut = ConstructOutboxCountQuery(account, folder, searchValues, out parsOut); }
                            break;
                    }
                    try
                    {
                        oCmd.Connection.Open();
                        // ((SqlCommand)oCmd). = true;
                        if (!string.IsNullOrEmpty(cmdTextIn))
                        {
                            oCmd.CommandText = cmdTextIn;
                            oCmd.Parameters.AddRange(parsIn.ToArray());
                            totIn += Convert.ToInt32(oCmd.ExecuteScalar());
                        }
                        if (!string.IsNullOrEmpty(cmdTextOut))
                        {
                            oCmd.CommandText = cmdTextOut;
                            oCmd.Parameters.Clear();
                            oCmd.Parameters.AddRange(parsOut.ToArray());
                            totOut += Convert.ToInt32(oCmd.ExecuteScalar());
                        }
                        int tot = totIn + totOut;
                        oCmd.Connection.Close();
                        res = new ResultList<MailHeaderExtended>();
                        res.Da = da;
                        res.Per = (tot > per) ? per : tot;
                        res.Totale = tot;
                        if (tot == default(int))
                        {
                            res.List = new List<MailHeaderExtended>();
                            return res;
                        }
                    }
                    catch (Exception ex)
                    {
                        res = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento delle email con filtro per account " + account + " E049 Dettagli Errore: " + ex.Message,
                                "ERR_049", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }

                    string queryIn = "", queryOut = "";
                    parsIn.Clear();
                    parsOut.Clear();
                    switch (parentFolder)
                    {
                        case "I":
                        case "A":
                        case "C":
                            if (folder != "6")
                            { queryIn = ConstructInboxQuery(account, folder, searchValues, da, per, out parsIn); }
                            break;
                        case "O":
                        case "OA":
                        case "AO":
                            queryOut = ConstructOutboxQuery(account, folder, searchValues, da, per, out parsOut);
                            break;
                        default:
                            if (folder == "6")
                            { cmdTextOut = ConstructOutboxCountQuery(account, folder, searchValues, out parsOut); }
                            break;

                    }
                    res.List = new List<MailHeaderExtended>();
                    if (totIn > 0 && !string.IsNullOrEmpty(queryIn))
                    {
                        oCmd.CommandText = queryIn;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddRange(parsIn.ToArray());
                        try
                        {
                            oCmd.Connection.Open();
                            using (DbDataReader r = oCmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                                }
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            res.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email con filtro per account " + account + " E050 Dettagli Errore: " + ex.Message,
                                    "ERR_050", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                    }
                    if (totOut > 0 && !string.IsNullOrEmpty(queryOut))
                    {
                        oCmd.CommandText = queryOut;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddRange(parsOut.ToArray());
                        try
                        {
                            oCmd.Connection.Open();
                            using (DbDataReader rd = oCmd.ExecuteReader())
                            {
                                while (rd.Read())
                                {
                                    res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(rd));
                                }
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            res.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email con filtro per account " + account + " E051 Dettagli Errore: " + ex.Message,
                                    "ERR_051", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                            throw;
                        }
                    }
                }
            }
            return res;
        }

        public IList<Model.Wrappers.SimpleTreeItem> GetMailTree(string account, long idMail)
        {
            IList<SimpleTreeItem> mailTree = null;

            string query = "WITH T_MAIL(ID_MAIL, FOLLOWS, MAIL_SUBJECT, IND_MAIL, FOLDER) AS"
                            + " (SELECT ID_MAIL, FOLLOWS, MAIL_SUBJECT, COALESCE(MAIL_TO, MAIL_CC, MAIL_CCN), 'I'"
                             + " FROM MAIL_INBOX"
                             + " WHERE MAIL_ACCOUNT = @p_MAIL_ACCOUNT"
                             + " UNION"
                             + " SELECT ID_MAIL, FOLLOWS, TO_CHAR(MAIL_SUBJECT), COALESCE(MAIL_TO, MAIL_CC, MAIL_CCN), 'O'"
                             + " FROM [FAXPEC].[FAXPEC].[MAIL_CONTENT] MC INNER JOIN ("
                                  + " SELECT *"
                                  + " FROM MAIL_REFS_NEW"
                                  + " PIVOT"
                                  + " (LISTAGG(MAIL_DESTINATARIO, ';') WITHIN GROUP (ORDER BY ID_REF)"
                                    + " FOR TIPO_REF IN"
                                        + " ('TO' AS \"MAIL_TO\","
                                         + " 'CC' AS \"MAIL_CC\","
                                         + " 'CCN' AS \"MAIL_CCN\")"
                                   + " )"
                             + " ) MRN"
                             + " ON MC.ID_MAIL = MRN.REF_ID_MAIL"
                             + " WHERE MAIL_SENDER = '" + account + "' ),"
                         + " T_MAIL_ROOT(ID_MAIL) AS"
                            + " (SELECT DISTINCT LAST_VALUE(ID_MAIL) OVER ()"
                             + " FROM T_MAIL"
                             + " START WITH ID_MAIL =" + idMail
                             + " CONNECT BY NOCYCLE PRIOR FOLLOWS = ID_MAIL)"
                         + " SELECT *"
                         + " FROM T_MAIL"
                         + " START WITH ID_MAIL = (SELECT ID_MAIL"
                                               + " FROM T_MAIL_ROOT"
                                               + " WHERE ROWNUM = 1)"
                         + " CONNECT BY NOCYCLE FOLLOWS = PRIOR ID_MAIL";

            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    oCmd.CommandText = query;
                    oCmd.Connection.Open();
                    using (DbDataReader r = oCmd.ExecuteReader())
                    {
                        mailTree = new List<SimpleTreeItem>();
                        while (r.Read())
                        {
                            mailTree.Add(DaoSQLServerDBHelper.MapToSimpleTreeItem(r));
                        }
                    }
                    oCmd.Connection.Close();
                }
            }

            return mailTree;
        }


        public List<ActiveUp.Net.Common.DeltaExt.Action> GetFolderDestinationForAction()
        {
            List<ActiveUp.Net.Common.DeltaExt.Action> list = null;
            try
            {
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        oCmd.CommandText = " SELECT * FROM [FAXPEC].[FAXPEC].[ACTIONS] ";
                        oCmd.Connection.Open();
                        using (DbDataReader r = oCmd.ExecuteReader())
                        {
                            list = new List<ActiveUp.Net.Common.DeltaExt.Action>();
                            while (r.Read())
                            {
                                list.Add(DaoSQLServerDBHelper.MapToAction(r));
                            }
                        }
                        oCmd.Connection.Close();
                    }
                }
            }catch(Exception ex)
            {
                ManagedException mEx = new ManagedException("Errore nell'estrazione della cartella di destinazione E356 Dettagli Errore: " + ex.Message,
                                   "ERR_356", string.Empty, string.Empty, ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                log.Error(err);
                throw mEx;
            }
            return list;
        }


        public int UpdateMailStatus(string account, List<string> idMail, ActiveUp.Net.Common.DeltaExt.MailStatus newStatus, string actionid, string utente)
        {
            int resp = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    List<MailLogInfo> linfo = new List<MailLogInfo>();
                    MailLogInfo info = new MailLogInfo();
                    string folder = string.Empty;
                    string logcode = string.Empty;
                    //using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    //{
                        //oCmd.CommandText = "INSERT INTO [FAXPEC].[FAXPEC].[MAIL_INBOX] (REF_ID_MAIL, STATUS_MAIL_OLD, STATUS_MAIL_NEW, UTE_OPE)"
                        //                + " SELECT ID_MAIL, STATUS_MAIL, '"
                        //                + createStatusMail(actionid) + "', '" + (utente ?? "SYSTEM") + "'"
                        //                + " FROM MAIL_INBOX"
                        //                + " WHERE MAIL_SERVER_ID IN ('" + String.Join("','", idMail.ToArray()) + "')"
                        //                + " AND MAIL_ACCOUNT = '" + account + "'";
                        try
                        {                                                                                           
                            resp = dbcontext.Database.ExecuteSqlCommand("INSERT INTO [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] (REF_ID_MAIL, STATUS_MAIL_OLD, STATUS_MAIL_NEW, UTE_OPE)"
                                        + " SELECT ID_MAIL, STATUS_MAIL, '"
                                        + createStatusMail(actionid) + "', '" + (utente ?? "SYSTEM") + "'"
                                        + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] "
                                        + " WHERE MAIL_SERVER_ID IN ('" + String.Join("','", idMail.ToArray()) + "')"
                                        + " AND MAIL_ACCOUNT = '" + account + "'");
                            //ExecuteNonQuery();                         
                            if (resp != idMail.Count)
                            {
                                throw new Exception("Errore nell'inserimento delle mail su mail_inbox_flussi");
                            } 
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                         //   oCmd.Connection.Close();
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nell'aggiornamento delle emails  per account " + account + " E056 Dettagli Errore: " + ex.Message,
                                    "ERR_056", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else throw ex;
                        }

                        string upd = createUpdateFolderId(actionid, "I", true, ref folder, ref logcode);
                        StringBuilder b = new StringBuilder();
                        b.Append("UPDATE [FAXPEC].[FAXPEC].[MAIL_INBOX] SET STATUS_MAIL = '" + (int)newStatus + "'");
                        if (upd != string.Empty)
                        { b.Append(upd); }
                        b.Append(" WHERE MAIL_SERVER_ID IN ('" + String.Join("','", idMail.ToArray()) + "')");
                        b.Append(" AND MAIL_ACCOUNT = '" + account + "'");
                        if (folder != "")
                        {
                            string operazione = "TO:" + "INBOX/" + folder;
                            for (int i = 0; i < idMail.Count; i++)
                            {
                                info = new MailLogInfo(APP_CODE, logcode, utente, account, operazione, idMail[i]);
                                linfo.Add(info);
                            }
                        }
                        // oCmd.CommandText = b.ToString();
                        try
                        {
                        resp = dbcontext.Database.ExecuteSqlCommand(b.ToString());
                            if (resp != idMail.Count)
                                throw new Exception("Errore nell'aggiornamento delle mail su mail_inbox");
                            dbContextTransaction.Commit();
                            foreach (MailLogInfo i in linfo)
                            {
                                log.Info(i);
                            }                           
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();                          
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nell'aggiornamento dello status emails inviate per account " + account + " E057 Dettagli Errore: " + ex.Message,
                                    "ERR_057", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else throw ex;
                        }
                   // }
                }
            }
            return resp;
        }

        public int UpdateMailSentStatus(string account, List<string> idMails, ActiveUp.Net.Common.DeltaExt.MailStatus newStatus, string utente, string action, string foldertipo)
        {
            int resp = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    MailLogInfo info = new MailLogInfo();
                    List<MailLogInfo> linfo = new List<MailLogInfo>();
                    string folder = string.Empty;
                    string logcode = string.Empty;
                    try
                    {
                        using (var cmd = dbcontext.Database.Connection.CreateCommand())
                        {
                            cmd.Connection.Open();
                            StringBuilder sb = new StringBuilder(" insert into [FAXPEC].[FAXPEC].[comunicazioni_flusso] ")
                           .Append(" (ref_id_com, stato_comunicazione_old, stato_comunicazione_new, ute_ope, canale) ")
                           .Append("select (select ref_id_com from [FAXPEC].[FAXPEC].[mail_content] where id_mail = @p_id_com),")
                           .Append("(select cf.stato_comunicazione_new from comunicazioni_flusso cf, mail_content m where ")
                           .Append(" m.id_mail=@p_id_com and  ")
                           .Append(" m.ref_id_flusso_attuale=cf.id_flusso), @p_new_status, @p_ute_ope,'MAIL' from dual ");
                            cmd.CommandText = sb.ToString();
                            // ((SqlCommand)cmd).BindByName = true;
                            SqlParameter p_id_com = new SqlParameter();
                            p_id_com.ParameterName = "p_id_com";
                            p_id_com.DbType = DbType.Decimal;
                            p_id_com.Value = idMails.ToArray();
                            //   ((SqlCommand)cmd).ArrayBindCount = idMails.Count;
                            cmd.Parameters.Add(p_id_com);
                            int[] status = new int[idMails.Count];
                            string[] ute = new string[idMails.Count];
                            for (int j = 0; j < idMails.Count; j++)
                            {
                                status[j] = (int)newStatus;
                                ute[j] = utente;
                            }
                            ((SqlCommand)cmd).Parameters.Add("p_new_status", SqlDbType.NVarChar).Value = status;
                            ((SqlCommand)cmd).Parameters.Add("p_ute_ope", SqlDbType.NVarChar).Value = ute;
                            if (newStatus != MailStatus.CANCELLATA && newStatus != MailStatus.ARCHIVIATA)
                            { resp = cmd.ExecuteNonQuery(); }
                            StringBuilder s = new StringBuilder();
                            s.Append(" UPDATE [FAXPEC].[FAXPEC].[MAIL_CONTENT] SET ");
                            s.Append(createUpdateFolderId(action, foldertipo, false, ref folder, ref logcode));
                            s.Append(" where ");
                            s.Append(" id_mail in (");
                            for (int j = 0; j < idMails.Count; j++)
                            {
                                if (j > 0)
                                { s.Append(","); }
                                s.Append(idMails[j]);
                            }
                            s.Append(")");
                            string operazione = "TO:" + "OUTBOX/" + folder;
                            for (int i = 0; i < idMails.Count; i++)
                            {
                                info = new MailLogInfo(APP_CODE, logcode, utente, account, operazione, idMails[i]);
                                linfo.Add(info);
                            }
                            cmd.CommandText = s.ToString();
                            cmd.Parameters.Clear();
                            int al = cmd.ExecuteNonQuery();
                            if (al < 1)
                            {
                                dbContextTransaction.Rollback();
                                throw new Exception(" Record non aggiornati ");
                            }
                            foreach (MailLogInfo i in linfo)
                            {
                                log.Info(i);
                            }

                            cmd.Connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nell'aggiornamento delle emails inviate per account " + account + " E055 Dettagli Errore: " + ex.Message,
                                "ERR_055", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else throw ex;
                    }
                    dbContextTransaction.Commit();
                }
            }
            return resp;
        }

        public int RipristinoMailStatus(string account, List<string> idMails, string parentFolder, string utente)
        {
            int resp = 0;
            List<MailLogInfo> linfo = new List<MailLogInfo>();
            MailLogInfo info = new MailLogInfo();
            string folder = string.Empty;
            string logcode = string.Empty;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        oCmd.CommandText = "INSERT INTO [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] "
                                         + " (REF_ID_MAIL, STATUS_MAIL_OLD, STATUS_MAIL_NEW, UTE_OPE)"
                                         + " SELECT * FROM ("
                                            + " SELECT"
                                            + " REF_ID_MAIL"
                                            + ", STATUS_MAIL_NEW"
                                            + ", STATUS_MAIL_OLD"
                                            + ", LAG(UTE_OPE) OVER (ORDER BY DATA_OPERAZIONE)"
                                            + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] "
                                            + " WHERE REF_ID_MAIL IN"
                                                + " (SELECT ID_MAIL"
                                                + " FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] "
                                                + " WHERE MAIL_SERVER_ID IN ('" + String.Join("','", idMails.ToArray()) + "')"
                                                + " AND MAIL_ACCOUNT = '" + account + "')"
                                            + " ORDER BY DATA_OPERAZIONE DESC)"
                                        + " WHERE ROWNUM = 1";
                        try
                        {
                            oCmd.Connection.Open();
                            resp = oCmd.ExecuteNonQuery();
                            if (resp != idMails.Count)
                                throw new Exception("Errore nell'inserimento su mail_inbox_flusso");
                            string upd = createUpdateFolderId("I", "", true, ref folder, ref logcode);
                            string operazione = "TO:" + "INBOX/" + folder;
                            for (int i = 0; i < idMails.Count; i++)
                            {
                                info = new MailLogInfo(APP_CODE, logcode, utente, account, operazione, idMails[i]);
                                linfo.Add(info);
                            }
                            StringBuilder b = new StringBuilder();
                            b.Append("UPDATE [FAXPEC].[FAXPEC].[MAIL_INBOX] MI SET STATUS_MAIL = (SELECT STATUS_MAIL_NEW");
                            b.Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO]  WHERE (REF_ID_MAIL, DATA_OPERAZIONE) =");
                            b.Append(" (SELECT REF_ID_MAIL, MAX(DATA_OPERAZIONE) ");
                            b.Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] MIF WHERE MIF.REF_ID_MAIL = MI.ID_MAIL ");
                            b.Append(" GROUP BY REF_ID_MAIL)) ");
                            if (upd != string.Empty)
                            { b.Append(upd); }
                            b.Append(" WHERE MAIL_SERVER_ID IN ('" + String.Join("','", idMails.ToArray()) + "')");
                            b.Append(" AND MAIL_ACCOUNT = '" + account + "'");
                            oCmd.CommandText = b.ToString();
                            resp = oCmd.ExecuteNonQuery();
                            if (resp != idMails.Count)
                                throw new Exception("Errore nell'aggironamento della mail_inbox");
                            dbContextTransaction.Commit();
                            foreach (MailLogInfo i in linfo)
                            {
                                log.Info(i);
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel ripristino dello status delle emails  per account " + account + " E059 Dettagli Errore: " + ex.Message,
                                    "ERR_059", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else throw ex;
                        }
                    }
                }
            }
            return resp;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailArchivedByAccount(string account, string folder, int da, int per)
        {
            ResultList<MailHeaderExtended> res = new ResultList<MailHeaderExtended>();
            List<Folder> list = getAllFolders();
            int id = 0;
            int.TryParse(folder, out id);
            var t = (from a in list
                     where a.Id == id
                     select a).ToList();
            switch (t[0].IdNome)
            {
                case "1":
                case "3":
                    res = GetAllMailsReceivedArchivedByAccount(account, folder, da, per);
                    break;
                case "2":
                    res = GetAllMailsSentArchivedByAccount(account, folder, da, per);
                    break;
                default:
                    switch (t[0].TipoFolder)
                    {
                        case "E":
                            res = GetAllMailsReceivedArchivedByAccount(account, folder, da, per);
                            break;
                        case "D":
                            res = GetAllMailsSentArchivedByAccount(account, folder, da, per);
                            break;
                    }
                    break;
            }
            return res;
        }


        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsReceivedCanceledByAccount(string account, string idfolder, int da, int per)
        {
            ResultList<MailHeaderExtended> res = null;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    oCmd.CommandText = queryInboxCountBase + " WHERE MAIL_ACCOUNT = '" + account + "' AND FOLDERTIPO='C' " +
                        " AND FOLDERID=" + idfolder;
                    try
                    {
                        oCmd.Connection.Open();
                        int tot = Convert.ToInt32(oCmd.ExecuteScalar());
                        res = new ResultList<MailHeaderExtended>();
                        res.Da = da;
                        res.Per = (tot > per) ? per : tot;
                        res.Totale = (tot > per) ? tot : per;
                        if (tot == default(int))
                        {
                            res.List = null;
                            return res;
                        }
                        oCmd.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        res = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel recupero delle email ricevute e cancellate per account " + account + " E063 Dettagli Errore: " + ex.Message,
                                "ERR_063", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }

                    string query = "WITH T AS (SELECT ID_MAIL, ROW_NUMBER() OVER (ORDER BY DATA_RICEZIONE DESC) AS RN FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] "
                        + " WHERE MAIL_ACCOUNT='" + account + "' AND FOLDERTIPO='C' AND FOLDERID=" + idfolder + ")"
                        + selectInboxBase
                        + " WHERE ID_MAIL IN (SELECT ID_MAIL FROM T WHERE RN BETWEEN " + da + " AND " + (da + per - 1) + ")"
                        + " ORDER BY DATA_RICEZIONE DESC";

                    oCmd.CommandText = query;
                    try
                    {
                        oCmd.Connection.Open();
                        using (DbDataReader r = oCmd.ExecuteReader())
                        {
                            res.List = new List<MailHeaderExtended>();
                            while (r.Read())
                            {
                                res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                            }
                        }
                        oCmd.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        res.List = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento delle email ricevute e cancellate per account " + account + " E044 Dettagli Errore: " + ex.Message,
                                "ERR_044", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }
                }
            }
            return res;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAllMailsSentCanceledByAccount(string account, string idfolder, int da, int per)
        {
            ResultList<MailHeaderExtended> result = null;
            StringBuilder sbCount = new StringBuilder("SELECT count(*) FROM (select id_mail from [FAXPEC].[FAXPEC].[MAIL_CONTENT] ")
            .Append(" WHERE MAIL_SENDER = '" + account + "'")
            .Append(" AND FOLDERID = " + idfolder + "")
            .Append(" AND FOLDERTIPO = 'C') as subquery ");
            StringBuilder sbFlussoOutboxNew = new StringBuilder(" SELECT ref_id_com FROM [FAXPEC].[FAXPEC].[MAIL_CONTENT] ")
          .Append(" WHERE MAIL_SENDER = @p_account")
          .Append(" AND FOLDERID = @p_folderid ")
          .Append(" AND FOLDERTIPO = 'C' ");
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                try
                {
                    result = new ResultList<MailHeaderExtended>();
                    using (var cmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        cmd.Connection.Open();
                        cmd.CommandText = string.Format(sbCount.ToString());
                        int count = (int)cmd.ExecuteScalar();
                        result.Da = da;
                        result.Per = (count == 0) ? 0 : ((count - da + 1) < per ? (count - da + 1) : per);
                        if (count == 0)
                            return result;

                        result.List = new List<MailHeaderExtended>(result.Per);

                        cmd.CommandText = "BEGIN OPEN :1 FOR " + selectOutboxBase +
                            " WHERE mc.ref_id_com IN (" + sbFlussoOutboxNew.ToString() + ");END;";
                        // SqlParameter pRc = ((SqlCommand)cmd).Parameters.Add("1", SqlDbType., DBNull.Value, ParameterDirection.Output);
                        cmd.ExecuteNonQuery();
                        using (var rd = cmd.ExecuteReader())
                        {
                            int row = 1;
                            while (row < da && rd.NextResult())
                                row++;
                            while (rd.Read() && row < (da + per))
                            {
                                row++;
                                result.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(rd));
                            }
                        }
                        cmd.Connection.Close();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    result.List = null;
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento delle email inviate e cancellate per account " + account + " E045 Dettagli Errore: " + ex.Message,
                            "ERR_045", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(err);
                        throw mEx;
                    }
                    else
                        throw ex;
                }
            }
        }

        public List<ActiveUp.Net.Common.DeltaExt.Folder> getAllFolders()
        {
            if (CacheManager<List<Folder>>.exist(CacheKeys.FOLDERS))
            {
                return CacheManager<List<Folder>>.get(CacheKeys.FOLDERS, VincoloType.NONE);
            }
            else
            {
                List<Folder> list = new List<Folder>();
                using (FAXPECContext dbcontext = new FAXPECContext())
                {
                    using (var cmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT ID,NOME,TIPO,SYSTEM,IDNOME FROM [FAXPEC].[FAXPEC].[FOLDERS] ";
                        try
                        {
                            cmd.Connection.Open();
                            using (DbDataReader r = cmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    list.Add(DaoSQLServerDBHelper.MapToFolder(r, null));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento dei folders E048 Dettagli Errore: " + ex.Message,
                                    "ERR_048", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                        cmd.Connection.Close();
                    }
                    CacheManager<List<Folder>>.set(Com.Delta.Web.Cache.CacheKeys.FOLDERS, list);
                    return list;
                }
            }
        }

        public int RipristinoMailStatusOutBox(string account, List<string> idMail, string parentFolder, string utente)
        {
            int resp = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    List<MailLogInfo> linfo = new List<MailLogInfo>();
                    MailLogInfo info = new MailLogInfo();
                    string folder = string.Empty;
                    string logcode = string.Empty;
                    using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                    {
                        oCmd.Connection.Open();
                        StringBuilder b = new StringBuilder();
                        b.Append("UPDATE [FAXPEC].[FAXPEC].[MAIL_CONTENT] SET FOLDERID = 2, FOLDERTIPO='O'");
                        b.Append(" WHERE ID_MAIL IN ('" + String.Join("','", idMail.ToArray()) + "')");
                        b.Append(" AND MAIL_SENDER = '" + account + "'");
                        oCmd.CommandText = b.ToString();
                        string operazione = "TO:" + "OUTBOX/2";
                        logcode = "CRB_RIP";
                        for (int i = 0; i < idMail.Count; i++)
                        {
                            info = new MailLogInfo(APP_CODE, logcode, utente, account, operazione, idMail[i]);
                            linfo.Add(info);
                        }
                        try
                        {
                            resp = oCmd.ExecuteNonQuery();
                            if (resp != idMail.Count)
                                throw new Exception("Errore nell'aggiornamento delle mail su mail_OUTBOX");
                            dbContextTransaction.Commit();
                            foreach (MailLogInfo i in linfo)
                            {
                                log.Info(i);
                            }
                        }
                        catch (Exception ex)
                        {

                            dbContextTransaction.Rollback();
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel ripristino delle emails inviate per account " + account + " E058 Dettagli Errore: " + ex.Message,
                                    "ERR_058", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else throw ex;
                        }
                        oCmd.Connection.Close();
                    }
                }
            }
            return resp;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetMailsGridByParams(string account, string folder, string box, string tipo, Dictionary<Model.MailTypeSearch, string> searchValues, int da, int per)
        {
            ResultList<MailHeaderExtended> res = null;
            da = (da == 0) ? 1 : da;
            int a = da + per - 1;
            int totIn = 0, totOut = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    string cmdTextIn = "", cmdTextOut = "";
                    List<SqlParameter> parsIn = new List<SqlParameter>(),
                        parsOut = new List<SqlParameter>();
                    switch (box)
                    {
                        case "I":
                            cmdTextIn = ConstructGridInboxCountQuery(account, folder, tipo, searchValues, out parsIn);
                            break;
                        case "O":
                            cmdTextOut = ConstructGridOutboxCountQuery(account, folder, tipo, searchValues, out parsOut);
                            break;
                    }
                    try
                    {
                        // ((SqlCommand)oCmd).BindByName = true;
                        oCmd.Connection.Open();
                        if (!string.IsNullOrEmpty(cmdTextIn))
                        {
                            oCmd.CommandText = cmdTextIn;
                            oCmd.Parameters.AddRange(parsIn.ToArray());
                            totIn += Convert.ToInt32(oCmd.ExecuteScalar());
                        }
                        if (!string.IsNullOrEmpty(cmdTextOut))
                        {
                            oCmd.CommandText = cmdTextOut;
                            oCmd.Parameters.Clear();
                            oCmd.Parameters.AddRange(parsOut.ToArray());
                            totOut += Convert.ToInt32(oCmd.ExecuteScalar());
                        }
                        int tot = totIn + totOut;
                        res = new ResultList<MailHeaderExtended>();
                        res.Da = da;
                        res.Per = (tot > per) ? per : tot;
                        res.Totale = tot;
                        if (tot == default(int))
                        {
                            res.List = new List<MailHeaderExtended>();
                            oCmd.Connection.Close();
                            return res;
                        }
                        oCmd.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        res = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento delle email sulla griglia distinta per account " + account + " E052 Dettagli Errore: " + ex.Message,
                                "ERR_052", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }
                    string queryIn = "", queryOut = "";
                    parsIn.Clear();
                    parsOut.Clear();
                    switch (box)
                    {
                        case "I":
                            queryIn = ConstructGridInboxQuery(account, folder, tipo, searchValues, da, per, out parsIn);
                            break;
                        case "O":
                            queryOut = ConstructGridOutboxQuery(account, folder, tipo, searchValues, da, per, out parsOut);
                            break;
                        default:
                            break;

                    }
                    res.List = new List<MailHeaderExtended>();
                    if (totIn > 0 && !string.IsNullOrEmpty(queryIn))
                    {
                        oCmd.CommandText = queryIn;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddRange(parsIn.ToArray());
                        try
                        {
                            oCmd.Connection.Open();
                            using (var r = oCmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                                }
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            res.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email sulla griglia distinta per account " + account + " E053 Dettagli Errore: " + ex.Message,
                                    "ERR_053", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                    }
                    if (totOut > 0 && !string.IsNullOrEmpty(queryOut))
                    {
                        oCmd.CommandText = queryOut;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddRange(parsOut.ToArray());
                        try
                        {
                            oCmd.Connection.Open();
                            using (var rd = oCmd.ExecuteReader())
                            {
                                while (rd.Read())
                                {
                                    res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(rd));
                                }
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            res.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email sulla griglia distinta per account " + account + " E054 Dettagli Errore: " + ex.Message,
                                    "ERR_054", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                    }

                }
            }
            return res;
        }

        public List<ActiveUp.Net.Common.DeltaExt.Folder> getFoldersByAccount(decimal idAccount)
        {
            List<Folder> list = new List<Folder>();
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var cmd = dbcontext.Database.Connection.CreateCommand())
                {
                    StringBuilder s = new StringBuilder();
                    s.Append("SELECT a.ID,a.NOME,a.TIPO,a.SYSTEM,a.IDNOME FROM [FAXPEC].[FAXPEC].[FOLDERS] a, [FAXPEC].[FAXPEC].[FOLDERS_SENDERS] b WHERE b.IDSENDER=" + idAccount)
                      .Append(" and a.ID=b.IDFOLDER ");
                    cmd.CommandText = s.ToString();
                    try
                    {
                        cmd.Connection.Open();
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                list.Add(DaoSQLServerDBHelper.MapToFolder(r, null));
                            }
                        }
                        cmd.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento dal database dell'albero account " + idAccount + " Data Layer E036 Dettagli Errore: " + ex.Message,
                                "ERR_036", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else throw;
                    }
                }
            }
            return list;
        }

        public bool UpdateAllMails(ActiveUp.Net.Common.DeltaExt.MailStatus mailStatus, string account, string folder, string utente, Dictionary<Model.MailTypeSearch, string> idx)
        {
            List<SqlParameter> pars = new List<SqlParameter>();
            bool ok = false;
            int idmailstatus = (int)mailStatus;
            string elenco = UpdatesId(account, folder, idx, out pars);
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = dbcontext.Database.Connection.CreateCommand())
                        {
                            cmd.CommandText = elenco;
                            cmd.Parameters.AddRange(pars.ToArray());
                            cmd.Connection.Open();
                            //   ((SqlCommand)cmd).BindByName = true;
                            var reader = cmd.ExecuteReader();
                            List<string> ids = new List<string>();
                            while (reader.Read())
                            {
                                ids.Add(reader.GetDecimal(0).ToString());
                            }
                            string listStr = string.Join(",", ids.ToArray());
                            StringBuilder s = new StringBuilder();

                            s.Append(" INSERT INTO [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] (REF_ID_MAIL,STATUS_MAIL_OLD,STATUS_MAIL_NEW,")
                            .Append(" DATA_OPERAZIONE,UTE_OPE) SELECT ID_MAIL,NVL(STATUS_MAIL_NEW,0),")
                            .Append(idmailstatus + ", SYSDATE,'" + utente + "' FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] A ")
                            .Append(" LEFT OUTER JOIN [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] C ON A.ID_MAIL=C.REF_ID_MAIL")
                            .Append(" WHERE ID_MAIL IN (" + listStr + ") ")
                            .Append(" AND DATA_OPERAZIONE= (SELECT MAX(DATA_OPERAZIONE) ")
                            .Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] B   ")
                            .Append(" WHERE A.ID_MAIL=B.REF_ID_MAIL) ");
                            cmd.CommandText = s.ToString();
                            cmd.Parameters.Clear();
                            // ((SqlCommand)cmd).BindByName = true;
                            int al = cmd.ExecuteNonQuery();
                            StringBuilder sb = new StringBuilder();
                            sb.Append(" UPDATE [FAXPEC].[FAXPEC].[MAIL_INBOX] SET STATUS_MAIL=" + idmailstatus + " WHERE ID_MAIL IN (" + listStr + ") ");
                            cmd.CommandText = sb.ToString();
                            int al1 = cmd.ExecuteNonQuery();
                            if (al1 != ids.Count)
                            {
                                dbContextTransaction.Rollback();
                                throw new Exception(" Record non aggiornati ");
                            }
                            else { ok = true; }
                            cmd.Connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nell'aggiornamento massivo delle email ricevute per status E039 Dettagli Errore: " + ex.Message,
                                "ERR_039", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else throw;
                    }
                    dbContextTransaction.Commit();
                }
            }
            return ok;
        }

        public bool MoveAllMails(string account, decimal idAccount, string newFolder, string oldFolder, string utente, string parentFolder, Dictionary<Model.MailTypeSearch, string> idx)
        {
            List<MailLogInfo> linfo = new List<MailLogInfo>();
            MailLogInfo info = new MailLogInfo();
            string logcode = string.Empty;
            string operazione = string.Empty;
            List<SqlParameter> pars = new List<SqlParameter>();
            bool ok = false;
            string elenco = string.Empty;
            if (parentFolder == "I")
            { elenco = UpdatesId(account, oldFolder, idx, out pars); }
            else { elenco = UpdatesIdOutBox(account, oldFolder, idx, out pars); }
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var dbContextTransaction = dbcontext.Database.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = dbcontext.Database.Connection.CreateCommand())
                        {
                            List<Folder> list = getFoldersByAccount(idAccount);
                            decimal idfolder = decimal.Parse(newFolder);
                            string tipoFolder = list.Where(x => x.Id == idfolder).FirstOrDefault().TipoFolder;
                            cmd.CommandText = elenco;
                            cmd.Parameters.AddRange(pars.ToArray());
                            // ((SqlCommand)cmd).BindByName = true;
                            cmd.Connection.Open();
                            var reader = cmd.ExecuteReader();
                            List<string> ids = new List<string>();
                            while (reader.Read())
                            {
                                ids.Add(reader.GetDecimal(0).ToString());
                            }
                            cmd.Connection.Close();
                            string listStr = string.Join(",", ids.ToArray());
                            StringBuilder s = new StringBuilder();
                            if (parentFolder == "I")
                            {
                                s.Append(" UPDATE [FAXPEC].[FAXPEC].MAIL_INBOX] SET FOLDERID = " + newFolder)
                                .Append(" , FOLDERTIPO = '" + tipoFolder + "' ")
                                .Append(" WHERE ID_MAIL IN (" + listStr + ") ");
                            }
                            else
                            {
                                s.Append(" UPDATE [FAXPEC].[FAXPEC].[MAIL_CONTENT] SET FOLDERID = " + newFolder)
                                .Append(" , FOLDERTIPO = '" + tipoFolder + "' ")
                                .Append(" WHERE ID_MAIL IN (" + listStr + ") ");
                            }
                            cmd.CommandText = s.ToString();
                            switch (tipoFolder)
                            {
                                case "I":
                                    logcode = "CRB_MOV";
                                    break;
                                case "O":
                                    logcode = "CRB_MOV";
                                    break;
                                case "C":
                                    logcode = "CRB_DEL";
                                    break;
                                case "A":
                                    logcode = "CRB_ARK";
                                    break;
                            }
                            if (parentFolder == "I" || parentFolder == "R")
                            { operazione = "TO:" + "INBOX/" + newFolder; }
                            else { operazione = "TO:" + "INBOX/" + newFolder; }
                            for (int i = 0; i < ids.Count; i++)
                            {
                                info = new MailLogInfo(APP_CODE, logcode, utente, account, operazione, ids[i]);
                                linfo.Add(info);
                            }
                            cmd.Parameters.Clear();
                            // ((SqlCommand)cmd).BindByName = true;
                            cmd.Connection.Open();
                            int al = cmd.ExecuteNonQuery();
                            if (al != ids.Count)
                            {
                                dbContextTransaction.Rollback();
                                throw new Exception(" Record non aggiornati ");
                            }
                            else { ok = true; }
                            cmd.Connection.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        //TASK: Allineamento log - Ciro
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nell'aggiornamento massivo delle email ricevute per folder per account " + account + " E040 Dettagli Errore: " + ex.Message,
                                "ERR_040", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else throw;
                    }
                    dbContextTransaction.Commit();
                }
            }
            return ok;
        }

        public ActiveUp.Net.Mail.DeltaExt.ResultList<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetMailsMoveGridByParams(string account, string folder, string box, string tipo, Dictionary<Model.MailTypeSearch, string> searchValues, bool chkUfficio, int da, int per)
        {
            ResultList<MailHeaderExtended> res = null;
            da = (da == 0) ? 1 : da;
            int a = da + per - 1;
            int totIn = 0, totOut = 0;
            using (FAXPECContext dbcontext = new FAXPECContext())
            {
                using (var oCmd = dbcontext.Database.Connection.CreateCommand())
                {
                    string cmdTextIn = "", cmdTextOut = "";
                    List<SqlParameter> parsIn = new List<SqlParameter>(),
                        parsOut = new List<SqlParameter>();
                    switch (box)
                    {
                        case "I":
                            cmdTextIn = ConstructGridInboxMoveCountQuery(account, folder, box, searchValues, chkUfficio, out parsIn);
                            break;
                        case "O":
                            cmdTextOut = ConstructGridOutboxMoveCountQuery(account, folder, box, searchValues, out parsOut);
                            break;
                    }
                    try
                    {
                        //  ((SqlCommand)oCmd).BindByName = true;
                        if (!string.IsNullOrEmpty(cmdTextIn))
                        {
                            oCmd.CommandText = cmdTextIn;
                            oCmd.Parameters.AddRange(parsIn.ToArray());
                            oCmd.Connection.Open();
                            totIn += Convert.ToInt32(oCmd.ExecuteScalar());
                            oCmd.Connection.Close();
                        }
                        if (!string.IsNullOrEmpty(cmdTextOut))
                        {
                            oCmd.CommandText = cmdTextOut;
                            oCmd.Parameters.Clear();
                            oCmd.Parameters.AddRange(parsOut.ToArray());
                            oCmd.Connection.Open();
                            totOut += Convert.ToInt32(oCmd.ExecuteScalar());
                            oCmd.Connection.Close();
                        }
                        int tot = totIn + totOut;
                        res = new ResultList<MailHeaderExtended>();
                        res.Da = da;
                        res.Per = (tot > per) ? per : tot;
                        res.Totale = tot;
                        if (tot == default(int))
                        {
                            res.List = new List<MailHeaderExtended>();
                            return res;
                        }
                    }
                    catch (Exception ex)
                    {
                        res = null;
                        if (!ex.GetType().Equals(typeof(ManagedException)))
                        {
                            ManagedException mEx = new ManagedException("Errore nel caricamento delle email sulla griglia movimentazioni per account " + account + " E052 Dettagli Errore: " + ex.Message,
                                "ERR_152", string.Empty, string.Empty, ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                            throw mEx;
                        }
                        else
                            throw ex;
                    }
                    string queryIn = "", queryOut = "";
                    parsIn.Clear();
                    parsOut.Clear();
                    switch (box)
                    {
                        case "I":
                            queryIn = ConstructGridMoveInboxQuery(account, folder, box, searchValues, da, per, chkUfficio, out parsIn);
                            break;
                        case "O":
                            queryOut = ConstructGridMoveOutboxQuery(account, folder, box, searchValues, da, per, out parsOut);
                            break;
                        default:
                            break;

                    }
                    res.List = new List<MailHeaderExtended>();
                    if (totIn > 0 && !string.IsNullOrEmpty(queryIn))
                    {
                        oCmd.CommandText = queryIn;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddRange(parsIn.ToArray());
                        try
                        {
                            oCmd.Connection.Open();
                            using (var r = oCmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(r));
                                }
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            res.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email sulla griglia movimentazioni per account " + account + " E053 Dettagli Errore: " + ex.Message,
                                    "ERR_153", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                    }
                    if (totOut > 0 && !string.IsNullOrEmpty(queryOut))
                    {
                        oCmd.CommandText = queryOut;
                        oCmd.Parameters.Clear();
                        oCmd.Parameters.AddRange(parsOut.ToArray());
                        try
                        {
                            oCmd.Connection.Open();
                            using (var rd = oCmd.ExecuteReader())
                            {
                                while (rd.Read())
                                {
                                    res.List.Add(DaoSQLServerDBHelper.MapToMailHeaderExtended(rd));
                                }
                            }
                            oCmd.Connection.Close();
                        }
                        catch (Exception ex)
                        {
                            res.List = null;
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                ManagedException mEx = new ManagedException("Errore nel caricamento delle email sulla griglia movimentazioni per account " + account + " E054 Dettagli Errore: " + ex.Message,
                                    "ERR_154", string.Empty, string.Empty, ex.InnerException);
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                log.Error(err);
                                throw mEx;
                            }
                            else
                                throw ex;
                        }
                    }

                }
            }
            return res;
        }
        #endregion



        #region "CRUD OPERATIONS"
        public ICollection<ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended> GetAll()
        {
            throw new NotImplementedException();
        }

        public ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Insert(ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ActiveUp.Net.Mail.DeltaExt.MailHeaderExtended entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region "Private Methods"

        private string UpdatesIdOutBox(string account, string folder, Dictionary<MailTypeSearch, string> searchValues, out List<SqlParameter> parsOut)
        {
            parsOut = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder();
            sb.Append(" (SELECT id_mail ")
              .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
              .Append(" JOIN comunicazioni_flusso cf")
              .Append(" ON mc.ref_id_flusso_attuale = cf.id_flusso")
              .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] ff ON mc.ref_id_flusso_inserimento=ff.id_flusso ")
              .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni] cm ON mc.ref_id_com=cm.id_com ")
              .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_sottotitoli] st ON cm.ref_id_sottotitolo=st.id_sottotitolo ")
              .Append(" JOIN [FAXPEC].[FAXPEC].[mail_refs_new] rf ON mc.id_mail=rf.ref_id_mail ")
              .Append(" WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mc.mail_sender = @p_ACCOUNT ");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account.ToLower()
                });
            }
            if (folder != "0")
            { sb.Append(" and mc.FOLDERID=" + folder); }
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Titolo:
                        if (val != string.Empty)
                        {
                            sb.Append(" and st.ref_id_titolo=@p_IDTITOLO");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Decimal,
                                ParameterName = "p_IDTITOLO",
                                Value = decimal.Parse(val)
                            });
                        }
                        break;
                    case MailTypeSearch.SottoTitolo:
                        if (val != string.Empty)
                        {
                            sb.Append(" and cm.ref_id_sottotitolo=@p_IDSOTTOTITOLO");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Decimal,
                                ParameterName = "p_IDSOTTOTITOLO",
                                Value = decimal.Parse(val)
                            });
                        }
                        break;
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(rf.mail_destinatario) like @p_MAIL ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.Oggetto:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mc.mail_subject) like @p_SUBJECT ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_SUBJECT",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(ff.data_operazione as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" and lower(ff.ute_ope)=@p_UTENTE ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.Status:
                        if (val != string.Empty)
                        {
                            sb.Append(" and cf.STATO_COMUNICAZIONE_NEW=@p_STATUS ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_STATUS",
                                Value = val.ToLower()
                            });
                        }
                        break;
                }
            }
            return sb.ToString();

        }


        private string ConstructGridMoveOutboxQuery(string account, string folder, string box, Dictionary<MailTypeSearch, string> searchValues, int da, int per, out List<SqlParameter> parsOut)
        {
            parsOut = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder();
            //mail_content
            sb.Append("WITH t_mc(id_m, id_c, rn) AS")
                .Append(" (SELECT id_mail, mc.ref_id_com, row_number() OVER (ORDER BY mc.ref_id_com DESC)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append("  inner join [FAXPEC].[FAXPEC].[LOG_ACTIONS] ON to_char(mc.ID_MAIL)=LOG_ACTIONS.OBJECT_ID ")
                .Append(" WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" lower(mc.mail_sender) = @p_ACCOUNT  and lower(USER_MAIL) = @p_ACCOUNT ");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account.ToLower()
                });
            }
            if (folder != "0")
            {
                sb.Append(" and LOG_DETAILS LIKE @p_FOLDER");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_FOLDER",
                    Value = string.Format("%{0}%",
                                      folder.ToLower())
                });
            }
            else
            {
                if (box == "O")
                { sb.Append(" and LOG_DETAILS LIKE '%OUTBOX/%' "); }
            }
            foreach (var key in searchValues.Keys)
            {
                string val = searchValues[key];
                switch (key)
                {
                    case MailTypeSearch.DataFine:
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(log_date as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" and lower(user_id)=@p_UTENTE ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                }
            }
            sb.Append("),");
            //mail_refs_new
            sb.Append("t_dest(id_m, tipo, mail_dest) AS")
                .Append(" (SELECT ref_id_mail, tipo_ref, mail_destinatario")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_refs_new] ")
                .Append(" WHERE ref_id_mail IN (SELECT id_m")
                .Append(" FROM t_mc where ");
            if (searchValues.Keys.Contains(MailTypeSearch.Mail))
            {
                string val = searchValues[MailTypeSearch.Mail];
                if (val != string.Empty)
                {
                    sb.Append(" lower(mail_destinatario) LIKE lower(@p_MAIL) and ");
                    parsOut.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "p_MAIL",
                        Value = string.Format("%{0}%",
                            val.ToString().ToLower().Replace("_", @"\_"))
                    });
                }
            }
            sb.Append(" rn BETWEEN @p_DA AND @p_A)");
            parsOut.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_DA",
                Value = da
            });
            parsOut.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_A",
                Value = ((da == 0) ? 1 : da) + per - 1
            });
            sb.Append(")");
            sb.Append("SELECT to_char(mc.id_mail) AS \"MAIL_ID\",")
                .Append(" mc.mail_sender AS \"MAIL_ACCOUNT\",")
                .Append(" mc.mail_sender AS \"MAIL_FROM\",")
                .Append(" R.\"MAIL_TO\",")
                .Append(" R.\"MAIL_CC\",")
                .Append(" R.\"MAIL_CCN\",")
                .Append(" to_char(mc.mail_subject) AS \"MAIL_SUBJECT\",")
                .Append(" lg.log_date as data_ricezione, ")
                .Append(" lg.log_date as DATA_INVIO, ")
                .Append(" mc.flg_annullamento AS \"STATUS_SERVER\",")
                .Append(" (SELECT DISTINCT last_value(stato_comunicazione_new) OVER ()")
                .Append(" FROM [FAXPEC].[FAXPEC].[Comunicazioni_flusso] cf")
                .Append(" START WITH cf.ref_id_com = mc.ref_id_com AND cf.stato_comunicazione_old IS NULL")
                .Append(" CONNECT BY NOCYCLE PRIOR cf.ref_id_com = cf.ref_id_com")
                .Append(" AND PRIOR cf.stato_comunicazione_new = cf.stato_comunicazione_old")
                .AppendFormat(" and CF.STATO_COMUNICAZIONE_NEW != '{0}') AS \"STATUS_MAIL\",",
                    (int)MailStatus.CANCELLED)
                .Append(" NULL as \"FLG_RATING\",")
                .Append(" '0' AS \"FLG_ATTACHMENTS\" ,")
                .Append(" 0 AS MSG_LENGTH, ")
                .Append(" to_number(trim(substr(lg.log_details, instr(lg.log_details,'/')+1,3))) as folderid, ")
                .Append(" lg.user_id as Utente,")
                .Append(" f.nome,f.tipo as foldertipo")
                .Append(" FROM mail_content mc")
                .Append(" inner join [FAXPEC].[FAXPEC].[LOG_ACTIONS] lg on to_char(mc.ID_MAIL)=lg.OBJECT_ID ")
                .Append(" JOIN [FAXPEC].[FAXPEC].[FOLDERS] F on to_number(trim(substr(lg.log_details, instr(lg.log_details,'/')+1,3)))=f.id")
                .Append(" LEFT OUTER JOIN (SELECT ID_M, \"MAIL_TO\", \"MAIL_CC\", \"MAIL_CCN\"")
                .Append(" FROM (SELECT id_m, tipo, mail_dest FROM t_dest) PIVOT")
                .Append(" (LISTAGG(mail_dest, ';') WITHIN GROUP (ORDER BY tipo)")
                .Append(" FOR TIPO IN ('TO' AS \"MAIL_TO\", 'CC' AS \"MAIL_CC\", 'CCN' AS \"MAIL_CCN\"))")
                .Append(" ORDER BY id_m) R")
                .Append(" ON mc.id_mail = r.id_m")
                .Append(" WHERE mc.ref_id_com IN (SELECT id_c")
                .Append(" FROM t_mc");
            if (!searchValues.Keys.Contains(MailTypeSearch.Mail))
            {
                sb.Append(" WHERE rn BETWEEN @p_DA AND @p_A");
                if (parsOut.SingleOrDefault(p => p.ParameterName == "p_DA") == null)
                {
                    parsOut.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_DA",
                        Value = da
                    });
                    parsOut.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_A",
                        Value = ((da == 0) ? 1 : da) + per - 1
                    });
                }
            }
            else
            {
                sb.Append(" WHERE EXISTS (SELECT 1 FROM t_dest WHERE t_dest.id_m = t_mc.id_m)");
            }
            sb.Append(")");
            sb.Append(" ORDER BY log_date desc");
            return sb.ToString();
        }

        private string ConstructGridMoveInboxQuery(string account, string folder, string box, Dictionary<MailTypeSearch, string> searchValues, int da, int per, bool chkUfficio, out List<SqlParameter> parsIn)
        {
            parsIn = new List<SqlParameter>();
            string utente = string.Empty;
            StringBuilder sb = new StringBuilder("WITH T AS")
                .Append("(SELECT id_mail, row_number() OVER (ORDER BY LOG_DATE DESC) AS rn")
                .Append(" from [FAXPEC].[FAXPEC].[MAIL_INBOX] INNER JOIN [FAXPEC].[FAXPEC].[LOG_ACTIONS]  ")
                .Append(" ON MAIL_INBOX.MAIL_SERVER_ID=LOG_ACTIONS.OBJECT_ID where ");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mail_account = @p_ACCOUNT and lower(user_mail) = @p_ACCOUNT  ");
                parsIn.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 150,
                    Value = account.ToLower()
                });
            }
            if (folder != "0")
            {
                sb.Append(" and LOG_DETAILS LIKE @p_FOLDER");
                parsIn.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_FOLDER",
                    Value = string.Format("%{0}%",
                                      folder.ToLower())
                });
            }
            else
            {
                if (box == "I")
                { sb.Append(" and LOG_DETAILS LIKE '%INBOX/%' "); }
            }
            if (chkUfficio)
            {
                sb.Append(" and lower(user_id) in  ('vigliairis', 'dessian','tagliaferrm','novelliri') ");
            }
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_from) like @p_MAIL ");
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        parsIn.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and max(cast(log_date as Date)) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsIn.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")) && chkUfficio != true)
                        {
                            sb.Append(" AND lower(user_id)=lower(@p_UTENTE) ");
                            utente = val.ToLower();
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                }
            }
            sb.Append(" order by log_actions.log_date desc ) SELECT mail_server_id AS \"MAIL_ID\",")
                .Append(" mail_account, mail_from, mail_to, mail_cc, mail_ccn,")
                .Append(" mail_subject, mail_text, data_invio, lg.log_date as data_ricezione,")
                .Append(" status_server, status_mail, flg_rating, flg_attachments,")
                // .Append(" to_number(trim(substr(lg.log_details, instr(lg.log_details,'/')+1,3))) as folderid, msg_length, ")
                .Append(" folderid, msg_length, ")
                .Append(" lg.user_id as Utente, f.nome,f.tipo as foldertipo ")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_inbox] mi")
                .Append(" inner join [FAXPEC].[FAXPEC].[LOG_ACTIONS] lg on mi.MAIL_SERVER_ID=lg.OBJECT_ID ")
                //  .Append(" inner join folders f on to_number(trim(substr(lg.log_details, instr(lg.log_details,'/')+1,3)))=f.id ")
                .Append(" inner join [FAXPEC].[FAXPEC].[folders] f on mi.folderid=f.id ")
                .Append(" WHERE id_mail IN ")
                .Append(" (SELECT id_mail FROM T WHERE rn BETWEEN @p_DA AND @p_A) ")
                .Append(" and cast(log_date as Date) between  @p_DATAINIZIO and @p_DATAFINE "); 
            if (folder != "0")
            {
                sb.Append(" and LOG_DETAILS LIKE @p_FOLDER  ");
            }
            if (utente != string.Empty)
            {
                sb.Append(" AND lower(user_id)=lower(@p_UTENTE) ");
            }
            sb.Append(" order by lg.log_date desc ");
            parsIn.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_DA",
                Value = da
            });
            parsIn.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_A",
                Value = ((da == 0) ? 1 : da) + per - 1
            });
            return sb.ToString();
        }

        private string UpdatesId(string account, string folder, Dictionary<MailTypeSearch, string> searchValues, out List<SqlParameter> pars)
        {
            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("SELECT ID_MAIL FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] mi WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mail_account = @p_ACCOUNT ");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 150,
                    Value = account
                });
            }
            if (folder != "0")
            {
                sb.Append(" and folderid = @p_FOLDER");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Decimal,
                    ParameterName = "p_FOLDER",
                    Value = int.Parse(folder)
                });
            }
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_from) like @p_MAIL ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.Oggetto:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_subject) like @p_SUBJECT ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_SUBJECT",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        pars.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(data_ricezione as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        pars.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" AND ID_MAIL IN (SELECT DISTINCT LAST_VALUE(REF_ID_MAIL) OVER (ORDER BY DATA_RICEZIONE DESC ROWS BETWEEN UNBOUNDED PRECEDING  AND UNBOUNDED FOLLOWING) FROM MAIL_INBOX_FLUSSO MF WHERE MF.REF_ID_MAIL = MI.ID_MAIL and lower(ute_ope)=lower(@p_UTENTE)) ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.Marcatori:
                        if (val != string.Empty)
                        {
                            sb.Append(" and mi.FLG_RATING=@p_FLGRATING ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_FLGRATING",
                                Value = decimal.Parse(val.ToLower())
                            });
                        }
                        break;
                    case MailTypeSearch.StatusInbox:
                        if (val != string.Empty)
                        {
                            sb.Append(" and mi.Status_Mail=@p_STATUS_INBOX ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_STATUS_INBOX",
                                Value = decimal.Parse(val.ToLower())
                            });
                        }
                        break;
                }
            }
            return sb.ToString(); ;
        }


        private string ConstructGridOutboxQuery(string account, string folder, string tipo, Dictionary<MailTypeSearch, string> searchValues, int da, int per, out List<SqlParameter> parsOut)
        {
            parsOut = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder();
            //mail_content
            sb.Append("WITH t_mc(id_m, id_c, rn) AS")
                .Append(" (SELECT id_mail, mc.ref_id_com, row_number() OVER (ORDER BY mc.ref_id_com DESC)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf")
                .Append(" ON mc.ref_id_flusso_attuale = cf.id_flusso")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] ff ON mc.ref_id_flusso_inserimento=ff.id_flusso ")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni] cm ON mc.ref_id_com=cm.id_com ")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_sottotitoli] st ON cm.ref_id_sottotitolo=st.id_sottotitolo ");
            if (searchValues.Keys.Contains(MailTypeSearch.Mail))
            {
                if (searchValues[MailTypeSearch.Mail] != string.Empty)
                { sb.Append(" JOIN [FAXPEC].[FAXPEC].[mail_refs_new] rf ON mc.id_mail=rf.ref_id_mail "); }
            }
            sb.Append(" WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mc.mail_sender = @p_ACCOUNT ");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account
                });
            }
            if (folder != "0")
            { sb.Append(" and mc.FOLDERID=" + folder); }
            sb.Append(" and mc.FOLDERTIPO='" + tipo + "'");
            foreach (var key in searchValues.Keys)
            {
                string val = searchValues[key];
                switch (key)
                {
                    case MailTypeSearch.Titolo:
                        if (val != string.Empty)
                        {
                            sb.Append(" and st.ref_id_titolo=@p_IDTITOLO");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Decimal,
                                ParameterName = "p_IDTITOLO",
                                Value = decimal.Parse(val)
                            });
                        }
                        break;
                    case MailTypeSearch.SottoTitolo:
                        if (val != string.Empty)
                        {
                            sb.Append(" and cm.ref_id_sottotitolo=@p_IDSOTTOTITOLO");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Decimal,
                                ParameterName = "p_IDSOTTOTITOLO",
                                Value = decimal.Parse(val)
                            });
                        }
                        break;
                    case MailTypeSearch.Oggetto:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_subject) like @p_SUBJECT ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_SUBJECT",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(ff.data_operazione as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" and lower(ff.ute_ope)=@p_UTENTE ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.Status:
                        if (val != string.Empty)
                        {
                            sb.Append(" and cf.STATO_COMUNICAZIONE_NEW=@p_STATUS ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_STATUS",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_destinatario) LIKE lower(@p_MAIL) ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToString().ToLower().Replace("_", @"\_"))
                            });
                        }
                        break;
                }
            }
            sb.Append("),");
            //mail_refs_new
            sb.Append("t_dest(id_m, tipo, mail_dest) AS")
                .Append(" (SELECT ref_id_mail, tipo_ref, mail_destinatario")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_refs_new] ")
                .Append(" WHERE ref_id_mail IN (SELECT id_m")
                .Append(" FROM t_mc where ");
            if (searchValues.Keys.Contains(MailTypeSearch.Mail))
            {
                string val = searchValues[MailTypeSearch.Mail];
                if (val != string.Empty)
                {
                    sb.Append(" lower(mail_destinatario) LIKE lower(@p_MAIL) and ");
                    parsOut.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "p_MAIL",
                        Value = string.Format("%{0}%",
                            val.ToString().ToLower().Replace("_", @"\_"))
                    });
                }
            }
            sb.Append(" rn BETWEEN @p_DA AND @p_A)");
            parsOut.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_DA",
                Value = da
            });
            parsOut.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_A",
                Value = ((da == 0) ? 1 : da) + per - 1
            });

            sb.Append(")");
            sb.Append("SELECT to_char(mc.id_mail) AS \"MAIL_ID\",")
                .Append(" mc.mail_sender AS \"MAIL_ACCOUNT\",")
                .Append(" mc.mail_sender AS \"MAIL_FROM\",")
                .Append(" R.\"MAIL_TO\",")
                .Append(" R.\"MAIL_CC\",")
                .Append(" R.\"MAIL_CCN\",")
                .Append(" mc.foldertipo as \"FOLDERTIPO\",")
                .Append(" mc.folderid as \"FOLDERID\",")
                .Append(" to_char(mc.mail_subject) AS \"MAIL_SUBJECT\",")
                .Append(" to_char(substr(mc.mail_text, 1 ,150)) AS \"MAIL_TEXT\",")
                .Append(" f.nome, ")
                .Append(" (SELECT MAX(data_operazione) FROM comunicazioni_flusso")
                .Append(" WHERE ref_id_com = mc.ref_id_com")
                .AppendFormat(" AND stato_comunicazione_new IN ('{0}', '{1}', '{2}')) AS \"DATA_INVIO\",",
                    (int)MailStatus.SENT, (int)MailStatus.ACCETTAZIONE, (int)MailStatus.NON_ACCETTAZIONE)
                .Append(" (SELECT MAX(data_operazione) FROM comunicazioni_flusso")
                .Append(" WHERE ref_id_com = mc.ref_id_com")
                .AppendFormat(" AND stato_comunicazione_new IN ('{0}', '{1}')) AS \"DATA_RICEZIONE\",",
                    (int)MailStatus.AVVENUTA_CONSEGNA, (int)MailStatus.ERRORE_CONSEGNA)
                .Append(" mc.flg_annullamento AS \"STATUS_SERVER\",")
                .Append(" (SELECT DISTINCT last_value(stato_comunicazione_new) OVER ()")
                .Append(" FROM [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf")
                .Append(" START WITH cf.ref_id_com = mc.ref_id_com AND cf.stato_comunicazione_old IS NULL")
                .Append(" CONNECT BY NOCYCLE PRIOR cf.ref_id_com = cf.ref_id_com")
                .Append(" AND PRIOR cf.stato_comunicazione_new = cf.stato_comunicazione_old")
                .AppendFormat(" and CF.STATO_COMUNICAZIONE_NEW != '{0}') AS \"STATUS_MAIL\",",
                    (int)MailStatus.CANCELLED)
                .Append(" NULL as \"FLG_RATING\",")
                .Append(" DECODE((SELECT count(*) FROM comunicazioni_allegati")
                .Append(" WHERE ref_id_com = mc.ref_id_com), 0, '0', '1') AS \"FLG_ATTACHMENTS\",")
                .Append(" (SELECT ute_ope FROM [FAXPEC].[FAXPEC].[COMUNICAZIONI_FLUSSO] WHERE ref_id_com = mc.ref_id_com")
                .AppendFormat(" AND stato_comunicazione_new = '{0}') AS \"UTENTE\", 0 AS MSG_LENGTH ", (int)MailStatus.INSERTED)
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" JOIN [FAXPEC].[FAXPEC].[FOLDERS] F on MC.FOLDERID=F.ID")
                .Append(" LEFT OUTER JOIN (SELECT ID_M, \"MAIL_TO\", \"MAIL_CC\", \"MAIL_CCN\"")
                .Append(" FROM (SELECT id_m, tipo, mail_dest FROM t_dest) PIVOT")
                .Append(" (LISTAGG(mail_dest, ';') WITHIN GROUP (ORDER BY tipo)")
                .Append(" FOR TIPO IN ('TO' AS \"MAIL_TO\", 'CC' AS \"MAIL_CC\", 'CCN' AS \"MAIL_CCN\"))")
                .Append(" ORDER BY id_m) R")
                .Append(" ON mc.id_mail = r.id_m")
                .Append(" WHERE mc.ref_id_com IN (SELECT id_c")
                .Append(" FROM t_mc");
            if (!searchValues.Keys.Contains(MailTypeSearch.Mail))
            {
                sb.Append(" WHERE rn BETWEEN @p_DA AND @p_A");
                if (parsOut.SingleOrDefault(p => p.ParameterName == "p_DA") == null)
                {
                    parsOut.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_DA",
                        Value = da
                    });
                    parsOut.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_A",
                        Value = ((da == 0) ? 1 : da) + per - 1
                    });
                }
            }
            else
            {
                sb.Append(" WHERE EXISTS (SELECT 1 FROM t_dest WHERE t_dest.id_m = t_mc.id_m)");
            }
            sb.Append(")");
            sb.Append(" ORDER BY data_invio desc");
            return sb.ToString();
        }

        private string ConstructGridOutboxMoveCountQuery(string account, string folder, string box, Dictionary<MailTypeSearch, string> searchValues, out List<SqlParameter> parsOut)
        {
            parsOut = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("SELECT COUNT(*)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" JOIN [FAXPEC].[FAXPEC].[LOG_ACTIONS] ")
                .Append(" ON to_char(mc.ID_MAIL)=LOG_ACTIONS.OBJECT_ID ")
                .Append(" WHERE ");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mc.mail_sender = @p_ACCOUNT ");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account.ToLower()
                });
            }
            if (folder != "0")
            {
                sb.Append(" and LOG_DETAILS LIKE @p_FOLDER");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_FOLDER",
                    Value = string.Format("%{0}%",
                                      folder.ToLower())
                });
            }
            else
            {
                if (box == "O")
                { sb.Append(" and LOG_DETAILS LIKE '%OUTBOX/%' "); }
            }
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {

                    case MailTypeSearch.DataFine:
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(log_date as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" and lower(user_id)=@p_UTENTE ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        private string ConstructGridInboxMoveCountQuery(string account, string folder, string box, Dictionary<MailTypeSearch, string> searchValues, bool ChkUfficio, out List<SqlParameter> pars)
        {

            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("SELECT count(*) FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] INNER JOIN [FAXPEC].[FAXPEC].[LOG_ACTIONS] ON MAIL_SERVER_ID=LOG_ACTIONS.OBJECT_ID WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mail_account = @p_ACCOUNT and lower(user_mail) = @p_ACCOUNT ");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 150,
                    Value = account.ToLower()
                });
            }
            if (folder != "0")
            {

                sb.Append(" and LOG_DETAILS LIKE @p_FOLDER");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_FOLDER",
                    Value = string.Format("%{0}%",
                                      folder.ToLower())
                });
            }
            else
            {
                if (box == "I")
                { sb.Append(" and LOG_DETAILS LIKE '%INBOX/%' "); }
            }
            if (ChkUfficio)
            {
                sb.Append(" and lower(user_id) in  ('vigliairis','novelliri', 'dessian','tagliaferrm','ciofierne') ");
            }
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_from) like @p_MAIL");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        pars.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(log_date as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        pars.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")) && ChkUfficio != true)
                        {
                            sb.Append(" AND lower(user_id)=lower(@p_UTENTE) ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                }
            }
            return sb.ToString(); ;
        }

        private string ConstructGridInboxCountQuery(string account, string folder, string box, Dictionary<MailTypeSearch, string> searchValues, out List<SqlParameter> pars)
        {

            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("SELECT count(*) FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mail_account = @p_ACCOUNT ");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",                  
                    Value = account
                });
            }
            if (folder != "0")
            {
                sb.Append(" and folderid = @p_FOLDER");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Decimal,
                    ParameterName = "p_FOLDER",
                    Value = decimal.Parse(folder)
                });
            }
            string tipo = string.Empty;
            if (box == "O")
            { tipo = "I"; }
            else { tipo = box; }
            sb.Append(" and foldertipo = @p_TIPO");
            pars.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.NVarChar,
                ParameterName = "p_TIPO",
                Value = tipo
            });
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_from) like @p_MAIL ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.Oggetto:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_subject) like @p_SUBJECT ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_SUBJECT",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        pars.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value =  val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(data_ricezione as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        pars.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value =  val 
                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" AND ID_MAIL IN (SELECT DISTINCT LAST_VALUE(REF_ID_MAIL) OVER (ORDER BY DATA_RICEZIONE DESC ROWS BETWEEN UNBOUNDED PRECEDING  AND UNBOUNDED FOLLOWING) FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] MF WHERE MF.REF_ID_MAIL = MI.ID_MAIL and lower(ute_ope)=lower(@p_UTENTE)) ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.Marcatori:
                        if (val != string.Empty)
                        {
                            sb.Append(" and mi.FLG_RATING=@p_FLGRATING ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_FLGRATING",
                                Value = decimal.Parse(val.ToLower())
                            });
                        }
                        break;
                    case MailTypeSearch.StatusInbox:
                        if (val != string.Empty)
                        {
                            sb.Append(" AND (SELECT DISTINCT LAST_VALUE(MF1.STATUS_MAIL_NEW) ");
                            sb.Append("  OVER (ORDER BY MF1.DATA_OPERAZIONE ASC ROWS BETWEEN UNBOUNDED PRECEDING  AND UNBOUNDED FOLLOWING) ");
                            sb.Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] MF1 WHERE MF1.REF_ID_MAIL = MI.ID_MAIL) = @p_STATUS_INBOX ");
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_STATUS_INBOX",
                                Value = decimal.Parse(val.ToLower())
                            });
                        }
                        break;
                }
            }          
            return sb.ToString(); ;
        }

        private string ConstructGridInboxQuery(string account, string folder, string box, Dictionary<MailTypeSearch, string> searchValues, int da, int per, out List<SqlParameter> parsIn)
        {
            parsIn = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("WITH T AS")
                .Append("(SELECT id_mail, row_number() OVER (ORDER BY data_ricezione DESC) AS rn")
                .Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] mi where ");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mail_account = @p_ACCOUNT ");
                parsIn.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 150,
                    Value = account
                });
            }
            if (folder != "0")
            { sb.AppendFormat(" and folderid = {0}", int.Parse(folder)); }
            string tipo = string.Empty;
            if (box == "O")
            { tipo = "I"; }
            else { tipo = box; }
            sb.AppendFormat(" and foldertipo='" + tipo + "'");
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_from) like @p_MAIL ");
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.Oggetto:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mail_subject) like @p_SUBJECT ");
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_SUBJECT",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" AND ID_MAIL IN (SELECT DISTINCT LAST_VALUE(REF_ID_MAIL) OVER (ORDER BY DATA_RICEZIONE DESC ROWS BETWEEN UNBOUNDED PRECEDING  AND UNBOUNDED FOLLOWING) FROM [FAXPEC].[FAXPEC].[MAIL_INBOX_FLUSSO] MF WHERE MF.REF_ID_MAIL = MI.ID_MAIL and lower(ute_ope)=lower(@p_UTENTE)) ");
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        parsIn.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(data_ricezione as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsIn.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val
                        });
                        break;
                    case MailTypeSearch.Marcatori:
                        if (val != string.Empty)
                        {
                            sb.Append(" and mi.FLG_RATING=@p_FLGRATING ");
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_FLGRATING",
                                Value = decimal.Parse(val.ToLower())
                            });
                        }
                        break;
                    case MailTypeSearch.StatusInbox:
                        if (val != string.Empty)
                        {
                            sb.Append(" and mi.Status_Mail=@p_STATUS_INBOX ");
                            parsIn.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_STATUS_INBOX",
                                Value = decimal.Parse(val.ToLower())
                            });
                        }
                        break;


                }
            }
            sb.Append(")");
            sb.Append("SELECT mail_server_id AS \"MAIL_ID\",")
                .Append(" mail_account, mail_from, mail_to, mail_cc, mail_ccn,")
                .Append(" mail_subject, mail_text, data_invio, data_ricezione,")
                .Append(" status_server, status_mail, flg_rating, flg_attachments, folderid,foldertipo,msg_length,")
                .Append(" (SELECT DISTINCT LAST_VALUE(ute_ope) OVER ")
                .Append(" (ORDER BY data_operazione ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING)")
                .Append(" FROM  [FAXPEC].[FAXPEC].[mail_inbox_flusso] mf")
                .Append(" WHERE mf.ref_id_mail = mi.id_mail) AS \"UTENTE\",")
                .Append(" msg_length,nome")
                .Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] mi")
                .Append(" JOIN [FAXPEC].[FAXPEC].[folders] on mi.FOLDERID=FOLDERS.ID")
                .Append(" WHERE id_mail IN ")
                .Append(" (SELECT id_mail FROM T WHERE rn BETWEEN @p_DA AND @p_A)")
                .Append(" ORDER BY DATA_RICEZIONE DESC");
            parsIn.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_DA",
                Value = da
            });
            parsIn.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_A",
                Value = ((da == 0) ? 1 : da) + per - 1
            });
            return sb.ToString();
        }

        private string ConstructGridOutboxCountQuery(string account, string folder, string tipo, Dictionary<MailTypeSearch, string> searchValues, out List<SqlParameter> parsOut)
        {
            parsOut = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("WITH t_mc(id_m, id_c, rn) AS")
                .Append(" (SELECT mc.id_mail, mc.ref_id_com, row_number() OVER (ORDER BY mc.ref_id_com DESC)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf")
                .Append(" ON mc.ref_id_flusso_attuale = cf.id_flusso")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] ff ON mc.ref_id_flusso_inserimento=ff.id_flusso ")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni] cm ON mc.ref_id_com=cm.id_com ")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_sottotitoli] st ON cm.ref_id_sottotitolo=st.id_sottotitolo ");
            if (searchValues.Keys.Contains(MailTypeSearch.Mail))
            {
                if (searchValues[MailTypeSearch.Mail] != string.Empty)
                { sb.Append(" JOIN [FAXPEC].[FAXPEC].[mail_refs_new] rf ON mc.id_mail=rf.ref_id_mail "); }
            }
            sb.Append(" WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mc.mail_sender = @p_ACCOUNT ");
                parsOut.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account.ToLower()
                });
            }
            if (folder != "0")
            { sb.Append(" and mc.FOLDERID=" + folder); }
            sb.Append(" and mc.FOLDERTIPO='" + tipo + "'");
            foreach (MailTypeSearch ind in searchValues.Keys)
            {
                string val = searchValues[ind];
                switch (ind)
                {
                    case MailTypeSearch.Titolo:
                        if (val != string.Empty)
                        {
                            sb.Append(" and st.ref_id_titolo=@p_IDTITOLO");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Decimal,
                                ParameterName = "p_IDTITOLO",
                                Value = decimal.Parse(val)
                            });
                        }
                        break;
                    case MailTypeSearch.SottoTitolo:
                        if (val != string.Empty)
                        {
                            sb.Append(" and cm.ref_id_sottotitolo=@p_IDSOTTOTITOLO");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.Decimal,
                                ParameterName = "p_IDSOTTOTITOLO",
                                Value = decimal.Parse(val)
                            });
                        }
                        break;
                    case MailTypeSearch.Mail:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(rf.mail_destinatario) like @p_MAIL ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MAIL",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.Oggetto:
                        if (val != string.Empty)
                        {
                            sb.Append(" and lower(mc.mail_subject) like @p_SUBJECT ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_SUBJECT",
                                Value = string.Format("%{0}%",
                                    val.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                            });
                        }
                        break;
                    case MailTypeSearch.DataFine:
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAFINE",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.DataInzio:
                        sb.Append(" and cast(ff.data_operazione as Date) between  @p_DATAINIZIO and @p_DATAFINE ");
                        parsOut.Add(new SqlParameter
                        {
                            Direction = ParameterDirection.Input,
                            SqlDbType = SqlDbType.NVarChar,
                            ParameterName = "p_DATAINIZIO",
                            Value = val

                        });
                        break;
                    case MailTypeSearch.Utente:
                        if (val != string.Empty && !(val.ToUpper().Contains("SELEZIONARE")))
                        {
                            sb.Append(" and lower(ff.ute_ope)=@p_UTENTE ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_UTENTE",
                                Value = val.ToLower()
                            });
                        }
                        break;
                    case MailTypeSearch.Status:
                        if (val != string.Empty)
                        {
                            sb.Append(" and cf.STATO_COMUNICAZIONE_NEW=@p_STATUS ");
                            parsOut.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_STATUS",
                                Value = val.ToLower()
                            });
                        }
                        break;
                }
            }
            sb.Append(")");
            sb.Append(" SELECT count(*) FROM (SELECT distinct id_c FROM t_mc");
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// RImodificata gestione folders
        /// </summary>
        /// <param name="account"></param>
        /// <param name="folder"></param>
        /// <param name="searchValues"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        private string ConstructInboxCountQuery(string account, string folder, Dictionary<MailIndexedSearch, List<string>> searchValues, out List<SqlParameter> pars)
        {
            int idfolder = int.Parse(folder);
            switch (idfolder)
            {
                case (int)MailFolder.INBOX_C:
                case (int)MailFolder.RICEVUTE_PEC_C:
                    if (searchValues.Keys.Contains(MailIndexedSearch.STATUS_MAIL))
                    {
                        if (searchValues[MailIndexedSearch.STATUS_MAIL].All(st =>
                            int.Parse(st) > (int)MailStatus.SCARICATA_INCOMPLETA))
                        {
                            pars = new List<SqlParameter>();
                            return null;
                        }
                    }
                    break;
            }

            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("SELECT count(*) AS \"TOT\" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] WHERE");

            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mail_account = @p_ACCOUNT AND");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 150,
                    Value = account
                });
            }

            switch (folder)
            {
                default:
                    sb.Append(" folderid = @p_FOLDER");
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_FOLDER",
                        Value = int.Parse(folder)
                    });
                    if (searchValues != null && searchValues.Keys.Contains(MailIndexedSearch.STATUS_MAIL))
                    {
                        sb.AppendFormat(" AND status_mail IN ('{0}')",
                            string.Join("', '", searchValues[MailIndexedSearch.STATUS_MAIL].ToArray()));
                    }
                    break;
                    // modificata NR 12/02/2015

                    //case MailFolder.ArchivioInviate:
                    //case MailFolder.ArchivioRicevute:
                    //case MailFolder.Cestino:
                    //    sb.AppendFormat(" status_mail = '{0}'",
                    //        (folder == MailFolder.ArchivioInviate || folder == MailFolder.ArchivioRicevute) ? (int)MailStatus.ARCHIVIATA
                    //        : (int)MailStatus.CANCELLATA);
                    //    break;
                    //default:
                    //    sb.Append(" mail_folder = :p_FOLDER");
                    //    pars.Add(new SqlParameter
                    //    {
                    //        Direction = ParameterDirection.Input,
                    //        SqlDbType = SqlDbType.NVarChar,
                    //        ParameterName = "p_FOLDER",
                    //        Size = 2,
                    //        Value = (int)folder
                    //    });
                    //    if (searchValues != null && searchValues.Keys.Contains(MailIndexedSearch.STATUS_MAIL))
                    //    {
                    //        sb.AppendFormat(" AND status_mail IN ('{0}')",
                    //            string.Join("', '", searchValues[MailIndexedSearch.STATUS_MAIL].ToArray()));
                    //    }
                    //    else
                    //    {
                    //        sb.AppendFormat(" AND status_mail NOT IN ('{0}','{1}')",
                    //            (int)MailStatus.ARCHIVIATA, (int)MailStatus.CANCELLATA);
                    //    }
                    //    break;
            }

            foreach (MailIndexedSearch ind in searchValues.Keys.Where(k => k != MailIndexedSearch.STATUS_MAIL))
            {
                string campo;

                List<string> likeString = new List<string>();
                switch (ind)
                {
                    case MailIndexedSearch.MAIL:
                        campo = "mail_from";
                        break;
                    case MailIndexedSearch.SUBJECT:
                        campo = "mail_subject";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                List<string> vals = searchValues[ind].Distinct().ToList();
                foreach (string st in vals)
                {
                    int idx = vals.IndexOf(st);
                    likeString.Add(String.Format(@" lower({0}) LIKE @p_SRH{1} ESCAPE '\'", campo, idx));
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "p_SRH" + idx,
                        Value = string.Format("%{0}%",
                            st.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                    });
                }
                sb.AppendFormat(" AND ({0})", string.Join(" OR ", likeString.ToArray()));
            }
            return sb.ToString(); ;
        }

        private string createUpdateFolderId(string actionid, string parentFolder, bool virgola, ref string folder, ref string logcode)
        {
            string query = string.Empty;
            List<ActiveUp.Net.Common.DeltaExt.Action> getActions = CacheManager<List<ActiveUp.Net.Common.DeltaExt.Action>>.get(CacheKeys.FOLDERS_ACTIONS, VincoloType.NONE);
            if (getActions == null)
            {
                getActions = GetFolderDestinationForAction();
            }
            int id = 0;
            int.TryParse(actionid, out id);
            var f = (from a in getActions
                     where a.Id == id
                     select a.IdFolderDestinazione);
            var t = (from a in getActions
                     where a.Id == id
                     select a.NuovoStatus);
            if (f.First() != 0)
            {
                if (virgola == true)
                { query = ","; }
                query += " FOLDERID=" + f.First();
                folder = f.First().ToString();
            }
            if (t.First() != string.Empty)
            { query += ", FOLDERTIPO='" + t.First() + "'"; }
            switch (t.First())
            {
                case "A":
                    logcode = "CRB_ARK";
                    break;
                case "C":
                    logcode = "CRB_DEL";
                    break;
                default:
                    logcode = "CRB_MOV";
                    break;
            }
            return query;
        }

        private int createStatusMail(string actionid)
        {
            int status = 0;
            List<ActiveUp.Net.Common.DeltaExt.Action> getActions = CacheManager<List<ActiveUp.Net.Common.DeltaExt.Action>>.get(Com.Delta.Web.Cache.CacheKeys.FOLDERS_ACTIONS, VincoloType.NONE);
            if (getActions == null)
            {
                getActions = GetFolderDestinationForAction();
            }
            int id = 0;
            int.TryParse(actionid, out id);
            var t = (from a in getActions
                     where a.Id == id
                     select a.NuovoStatus);
            switch (t.First())
            {
                case "C":
                    status = (int)MailStatus.CANCELLED;
                    break;
                case "A":
                    status = (int)MailStatus.ARCHIVIATA;
                    break;
                case "I":
                    status = (int)MailStatus.SCARICATA;
                    break;
                default:
                    switch (id)
                    {
                        case 2:
                            status = (int)MailStatus.SCARICATA;
                            break;
                        default:
                            status = (int)MailStatus.LETTA;
                            break;
                    }
                    break;
            }
            return status;
        }

        /// <summary>
        /// RiMODIFICATO GESTIONE FOLDERS
        /// </summary>
        /// <param name="account"></param>
        /// <param name="folder"></param>
        /// <param name="searchValues"></param>
        /// <param name="da"></param>
        /// <param name="per"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        private string ConstructInboxQuery(string account, string folder, Dictionary<MailIndexedSearch, List<string>> searchValues, int da, int per, out List<SqlParameter> pars)
        {
            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("WITH T AS")
                .Append("(SELECT id_mail, row_number() OVER (ORDER BY data_ricezione DESC) AS rn")
                .Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] ")
                .Append(" WHERE ");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append("mail_account = @p_ACCOUNT AND");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 150,
                    Value = account
                });
            }
            switch (folder)
            {
                default:
                    sb.AppendFormat(" folderid = {0}", int.Parse(folder));
                    if (searchValues != null && searchValues.ContainsKey(MailIndexedSearch.STATUS_MAIL))
                    {
                        sb.AppendFormat(" and status_mail IN ('{0}')",
                            string.Join("', '", searchValues[MailIndexedSearch.STATUS_MAIL].ToArray()));
                    }
                    break;
                    // MODIFICATO NR 12/02/2015 
                    //case MailFolder.ArchivioInviate:
                    //case MailFolder.ArchivioRicevute:
                    //case MailFolder.Cestino:
                    //    sb.AppendFormat(" status_mail = '{0}'",
                    //        (folder == MailFolder.ArchivioInviate || folder == MailFolder.ArchivioRicevute) ? (int)MailStatus.ARCHIVIATA
                    //        : (int)MailStatus.CANCELLATA);
                    //    break;
                    //default:
                    //    sb.AppendFormat(" mail_folder = '{0}'", (int)folder);
                    //    if (searchValues != null && searchValues.ContainsKey(MailIndexedSearch.STATUS_MAIL))
                    //    {
                    //        sb.AppendFormat(" and status_mail IN ('{0}')",
                    //            string.Join("', '", searchValues[MailIndexedSearch.STATUS_MAIL].ToArray()));
                    //    }
                    //    else
                    //    {
                    //        sb.AppendFormat(" AND status_mail NOT IN ('{0}','{1}')",
                    //            (int)MailStatus.ARCHIVIATA, (int)MailStatus.CANCELLATA);
                    //    }
                    //    break;
            }
            foreach (MailIndexedSearch mis in searchValues.Keys.Where(k => k != MailIndexedSearch.STATUS_MAIL))
            {
                string campo = "";
                List<string> l = new List<string>();
                switch (mis)
                {
                    case MailIndexedSearch.MAIL:
                        campo = "mail_from";
                        break;
                    case MailIndexedSearch.SUBJECT:
                        campo = "mail_subject";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                List<string> vals = searchValues[mis].Distinct().ToList();
                foreach (string s in vals)
                {
                    int idx = vals.IndexOf(s);
                    l.Add(String.Format(@" LOWER({0}) LIKE @p_SRH{1} ESCAPE '\'", campo, idx));
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "p_SRH" + idx,
                        Value = string.Format("%{0}%",
                            s.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace("'", "''"))
                    });
                }
                sb.AppendFormat(" AND ({0})", string.Join(" OR ", l.ToArray()));
            }
            sb.Append(")");
            sb.Append("SELECT mail_server_id AS \"MAIL_ID\",")
                .Append(" mail_account, mail_from, mail_to, mail_cc, mail_ccn,")
                .Append(" mail_subject, mail_text, data_invio, data_ricezione,")
                .Append(" status_server, status_mail, flg_rating, flg_attachments, folderid,foldertipo,")
                .Append(" (SELECT DISTINCT LAST_VALUE(ute_ope) OVER ")
                .Append(" (ORDER BY data_operazione ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_inbox_flusso] mf")
                .Append(" WHERE mf.ref_id_mail = mi.id_mail) AS \"UTENTE\",")
                .Append(" msg_length")
                .Append(" FROM [FAXPEC].[FAXPEC].[MAIL_INBOX] mi")
                .Append(" WHERE id_mail IN ")
                .Append(" (SELECT id_mail FROM T WHERE rn BETWEEN @p_DA AND @p_A)")
                .Append(" ORDER BY DATA_RICEZIONE DESC");
            pars.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_DA",
                Value = da
            });
            pars.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                SqlDbType = SqlDbType.Decimal,
                ParameterName = "p_A",
                Value = ((da == 0) ? 1 : da) + per - 1
            });
            return sb.ToString();
        }

        /// <summary>
        /// rimodificata gestione folders
        /// DA MODIFICARE PER REF_ID_FLUSSO
        /// </summary>
        /// <param name="account"></param>
        /// <param name="folder"></param>
        /// <param name="searchValues"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        private string ConstructOutboxCountQuery(string account, string folder, Dictionary<MailIndexedSearch, List<string>> searchValues, out List<SqlParameter> pars)
        {
            int idfolder = int.Parse(folder);
            switch (idfolder)
            {
                case (int)MailFolder.OUTBOX_C:
                    if (searchValues.Keys.Contains(MailIndexedSearch.STATUS_MAIL))
                    {
                        if (searchValues[MailIndexedSearch.STATUS_MAIL].All(st =>
                            int.Parse(st) < (int)MailStatus.INSERTED))
                        {
                            pars = new List<SqlParameter>();
                            return null;
                        }
                    }
                    break;
            }
            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder("WITH t_mc(id_m, id_c, rn) AS")
                .Append(" (SELECT id_mail, mc.ref_id_com, row_number() OVER (ORDER BY mc.ref_id_com DESC)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf")
                .Append(" ON mc.ref_id_com = cf.ref_id_com")
                .Append(" WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mc.mail_sender = @p_ACCOUNT ");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account.ToLower()
                });
            }
            sb.Append(" AND FOLDERID=" + folder);
            foreach (var srhIdxKey in searchValues.Keys)
            {
                switch (srhIdxKey)
                {
                    case MailIndexedSearch.STATUS_MAIL:
                        sb.AppendFormat(" AND cf.stato_comunicazione_new IN ('{0}')",
                            string.Join("', '", searchValues[srhIdxKey].ToArray()));
                        break;
                    case MailIndexedSearch.SUBJECT:
                        foreach (string sub in searchValues[srhIdxKey])
                        {
                            int idx = searchValues[srhIdxKey].IndexOf(sub);
                            if (idx == 0)
                            {
                                sb.AppendFormat(" AND (lower(mail_subject) LIKE @p_MS{0} ESCAPE '\\'", idx);
                            }
                            else
                            {
                                sb.AppendFormat(" OR lower(mail_subject) LIKE @p_MS{0} ESCAPE '\\'", idx);
                            }
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MS" + idx,
                                Value = string.Format("%{0}%",
                                    sub.ToLower().Replace("%", @"\%").Replace("_", @"\_").Replace(@"\", @"\\"))
                            });
                        }
                        sb.Append(")");
                        break;
                }
            }
            sb.Append(")");
            if (searchValues.Keys.Contains(MailIndexedSearch.MAIL))
            {
                sb.Append(",");
                sb.Append("t_dest(id_m, tipo, mail_dest) AS")
                    .Append(" (SELECT ref_id_mail, tipo_ref, mail_destinatario")
                    .Append(" FROM [FAXPEC].[FAXPEC].[mail_refs_new] ")
                    .Append(" WHERE ref_id_mail IN (SELECT id_m")
                    .Append(" FROM t_mc)");
                foreach (var mail in searchValues[MailIndexedSearch.MAIL])
                {
                    int idx = searchValues[MailIndexedSearch.MAIL].IndexOf(mail);
                    if (idx == 0)
                    {
                        sb.AppendFormat(" AND (");
                    }
                    else
                    {
                        sb.AppendFormat(" OR");
                    }
                    sb.AppendFormat(" mail_destinatario LIKE @p_MAIL{0} ESCAPE '\\'", idx);
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "p_MAIL" + idx,
                        Value = string.Format("%{0}%",
                            mail.ToLower().Replace("_", @"\_"))
                    });
                }
                sb.Append("))");
            }
            sb.Append(" SELECT count(*) FROM (SELECT distinct id_c FROM t_mc");
            if (searchValues.Keys.Contains(MailIndexedSearch.MAIL))
            {
                sb.Append(" WHERE EXISTS (SELECT 1")
                    .Append(" FROM t_dest")
                    .Append(" WHERE t_dest.id_m = t_mc.id_m)");
            }
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// rimodificata gestione folders
        /// DA MODIFICARE PER REF_ID_FLUSSO
        /// </summary>
        /// <param name="account"></param>
        /// <param name="folder"></param>
        /// <param name="searchValues"></param>
        /// <param name="da"></param>
        /// <param name="per"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        private string ConstructOutboxQuery(string account, string folder, Dictionary<MailIndexedSearch, List<string>> searchValues, int da, int per, out List<SqlParameter> pars)
        {
            pars = new List<SqlParameter>();
            StringBuilder sb = new StringBuilder();
            //mail_content
            sb.Append("WITH t_mc(id_m, id_c, rn) AS")
                .Append(" (SELECT id_mail, mc.ref_id_com, row_number() OVER (ORDER BY mc.ref_id_com DESC)")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" JOIN [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf")
                .Append(" ON mc.ref_id_com = cf.ref_id_com")
                .Append(" WHERE");
            if (!string.IsNullOrEmpty(account))
            {
                sb.Append(" mc.mail_sender = @p_ACCOUNT ");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.NVarChar,
                    ParameterName = "p_ACCOUNT",
                    Size = 250,
                    Value = account
                });
            }
            sb.Append(" AND FOLDERID=" + folder);
            foreach (var key in searchValues.Keys)
            {
                switch (key)
                {
                    case MailIndexedSearch.STATUS_MAIL:
                        sb.AppendFormat(" AND cf.stato_comunicazione_new IN ('{0}')",
                            string.Join("', '", searchValues[key].ToArray()));
                        break;
                    case MailIndexedSearch.SUBJECT:
                        foreach (string subj in searchValues[key])
                        {
                            int idx = searchValues[key].IndexOf(subj);
                            if (idx == 0)
                                sb.Append(" AND (");
                            else sb.Append(" OR");
                            sb.AppendFormat(" lower(mail_subject) LIKE @p_MS{0} ESCAPE '\\'", idx);
                            pars.Add(new SqlParameter
                            {
                                Direction = ParameterDirection.Input,
                                SqlDbType = SqlDbType.NVarChar,
                                ParameterName = "p_MS" + idx,
                                Value = string.Format("%{0}%",
                                    subj.ToLower().Replace("_", @"\_").Replace("%", @"\%").Replace(@"\", @"\\"))
                            });
                        }
                        sb.AppendFormat(")");
                        break;
                }
            }
            sb.Append("),");
            //mail_refs_new
            sb.Append("t_dest(id_m, tipo, mail_dest) AS")
                .Append(" (SELECT ref_id_mail, tipo_ref, mail_destinatario")
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_refs_new] ")
                .Append(" WHERE ref_id_mail IN (SELECT id_m")
                .Append(" FROM t_mc");
            if (searchValues.Keys.Contains(MailIndexedSearch.MAIL))
            {
                sb.Append(") AND (");
                foreach (var mail in searchValues[MailIndexedSearch.MAIL])
                {
                    int idx = searchValues[MailIndexedSearch.MAIL].IndexOf(mail);
                    if (idx != 0) sb.Append(" OR ");
                    sb.AppendFormat(" mail_destinatario LIKE @p_MAIL{0} ESCAPE '\\'", idx);
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.NVarChar,
                        ParameterName = "p_MAIL" + idx,
                        Value = string.Format("%{0}%",
                            mail.ToLower().Replace("_", @"\_"))
                    });
                }
                sb.Append(")");
            }
            else
            {
                sb.Append(" WHERE rn BETWEEN @p_DA AND @p_A)");
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Decimal,
                    ParameterName = "p_DA",
                    Value = da
                });
                pars.Add(new SqlParameter
                {
                    Direction = ParameterDirection.Input,
                    SqlDbType = SqlDbType.Decimal,
                    ParameterName = "p_A",
                    Value = ((da == 0) ? 1 : da) + per - 1
                });
            }
            sb.Append(")");
            sb.Append("SELECT to_char(mc.id_mail) AS \"MAIL_ID\",")
                .Append(" mc.mail_sender AS \"MAIL_ACCOUNT\",")
                .Append(" mc.mail_sender AS \"MAIL_FROM\",")
                .Append(" mc.folderid , ")
                .Append(" mc.foldertipo ,")
                .Append(" R.\"MAIL_TO\",")
                .Append(" '' as \"MAIL_CC\",")
                .Append(" '' as \"MAIL_CCN\",")
                .Append(" to_char(mc.mail_subject) AS \"MAIL_SUBJECT\",")
                .Append(" to_char(substr(mc.mail_text, 1 ,150)) AS \"MAIL_TEXT\",")
                .Append(" (SELECT MAX(data_operazione) FROM [FAXPEC].[FAXPEC].[comunicazioni_flusso] ")
                .Append(" WHERE ref_id_com = mc.ref_id_com")
                .AppendFormat(" AND stato_comunicazione_new IN ('{0}', '{1}', '{2}')) AS \"DATA_INVIO\",",
                    (int)MailStatus.SENT, (int)MailStatus.ACCETTAZIONE, (int)MailStatus.NON_ACCETTAZIONE)
                .Append(" (SELECT MAX(data_operazione) FROM comunicazioni_flusso")
                .Append(" WHERE ref_id_com = mc.ref_id_com")
                .AppendFormat(" AND stato_comunicazione_new IN ('{0}', '{1}')) AS \"DATA_RICEZIONE\",",
                    (int)MailStatus.AVVENUTA_CONSEGNA, (int)MailStatus.ERRORE_CONSEGNA)
                .Append(" mc.flg_annullamento AS \"STATUS_SERVER\",")
                .Append(" (SELECT DISTINCT last_value(stato_comunicazione_new) OVER ()")
                .Append(" FROM [FAXPEC].[FAXPEC].[comunicazioni_flusso] cf")
                .Append(" START WITH cf.ref_id_com = mc.ref_id_com AND cf.stato_comunicazione_old IS NULL")
                .Append(" CONNECT BY NOCYCLE PRIOR cf.ref_id_com = cf.ref_id_com")
                .Append(" AND PRIOR cf.stato_comunicazione_new = cf.stato_comunicazione_old")
                .AppendFormat(" and CF.STATO_COMUNICAZIONE_NEW != '{0}') AS \"STATUS_MAIL\",",
                    (int)MailStatus.CANCELLED)
                .Append(" NULL as \"FLG_RATING\",")
                .Append(" DECODE((SELECT count(*) FROM [FAXPEC].[FAXPEC].[comunicazioni_allegati] ")
                .Append(" WHERE ref_id_com = mc.ref_id_com), 0, '0', '1') AS \"FLG_ATTACHMENTS\",")
                .Append(" (SELECT ute_ope FROM comunicazioni_flusso WHERE ref_id_com = mc.ref_id_com")
                .AppendFormat(" AND stato_comunicazione_new = '{0}') AS \"UTENTE\", 0 AS MSG_LENGTH ", (int)MailStatus.INSERTED)
                .Append(" FROM [FAXPEC].[FAXPEC].[mail_content] mc")
                .Append(" LEFT OUTER JOIN (SELECT ID_M, \"MAIL_TO\"")
                .Append(" FROM (SELECT id_m, tipo, mail_dest FROM t_dest) PIVOT")
                .Append(" (LISTAGG(mail_dest, ';') WITHIN GROUP (ORDER BY tipo)")
                .Append(" FOR TIPO IN ('TO' AS \"MAIL_TO\"))")
                .Append(" ORDER BY id_m) R")
                .Append(" ON mc.id_mail = r.id_m")
                .Append(" WHERE mc.ref_id_com IN (SELECT id_c")
                .Append(" FROM t_mc");
            if (!searchValues.Keys.Contains(MailIndexedSearch.MAIL))
            {
                sb.Append(" WHERE rn BETWEEN @p_DA AND @p_A");
                if (pars.SingleOrDefault(p => p.ParameterName == "p_DA") == null)
                {
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_DA",
                        Value = da
                    });
                    pars.Add(new SqlParameter
                    {
                        Direction = ParameterDirection.Input,
                        SqlDbType = SqlDbType.Decimal,
                        ParameterName = "p_A",
                        Value = ((da == 0) ? 1 : da) + per - 1
                    });
                }
            }
            else
            {
                sb.Append(" WHERE EXISTS (SELECT 1 FROM t_dest WHERE t_dest.id_m = t_mc.id_m)");
            }
            sb.Append(")");
            sb.Append(" ORDER BY data_invio desc");
            return sb.ToString();
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
        // ~MailHeaderDaoSQLDb() {
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

