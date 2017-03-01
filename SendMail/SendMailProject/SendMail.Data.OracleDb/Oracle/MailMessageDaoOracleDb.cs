using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ActiveUp.Net.Mail;
using Oracle.DataAccess.Client;
using System.Collections;

using ActiveUp.Net.Mail.DeltaExt;
using SendMail.DataContracts.Interfaces;
using ActiveUp.Net.Common.DeltaExt;
using SendMailApp.OracleCore.Oracle.OrderedQuery;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.Utilities;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using System.Configuration;
using log4net;
using System.Runtime;
using Oracle.DataAccess.Types;
using System.IO;
using System.Tuples;
using System.Data;

namespace SendMail.Data.OracleDb
{
    public class MailMessageDaoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IMailMessageDao
    {
        private static readonly ILog _log = LogManager.GetLogger("MailMessageDaoOracleDb");

        const int BUFFER_SIZE = 65536;

        #region Private Oracle Commands String

        private const string insertMessage = "INSERT INTO MAIL_INBOX"
                                          + " (MAIL_SERVER_ID"
                                          + ", MAIL_ACCOUNT"
                                          + ", MAIL_FROM"
                                          + ", MAIL_TO"
                                          + ", MAIL_CC"
                                          + ", MAIL_CCN"
                                          + ", MAIL_SUBJECT"
                                          + ", MAIL_TEXT"
                                          + ", DATA_INVIO"
                                          + ", DATA_RICEZIONE"
                                          + ", STATUS_SERVER"
                                          + ", STATUS_MAIL"
                                          + ", FLG_INTEG_APPL"
                                          + ", MAIL_FILE"
                                          + ", FLG_ATTACHMENTS"
                                  //        + ", MAIL_FOLDER"
                                          + ", FOLDERID "
                                          + ", FOLDERTIPO "
                                          + ", FOLLOWS,MSG_LENGTH)"
                                          + " VALUES (:pMAIL_SERVER_ID, :pMAIL_ACCOUNT, :pMAIL_FROM, :pMAIL_TO, :pMAIL_CC, :pMAIL_CCN"
                                          + ", :pMAIL_SUBJECT, :pMAIL_TEXT, :pDATA_INVIO, :pDATA_RICEZIONE, :pSTATUS_SERVER"
                                          + ", :pSTATUS_MAIL, :pFLG_INTEG_APPL, :pMAIL_FILE, :pFLG_ATTACHMENTS, :pFOLDERID, :pFOLDERTIPO, :pFOLLOWS, :pMSGLENGTH)"
                                          + " RETURNING ID_MAIL INTO :pID_MAIL";

        private const string insertFlusso = "INSERT INTO MAIL_INBOX_FLUSSO"
                                         + " (REF_ID_MAIL"
                                         + ", STATUS_MAIL_OLD"
                                         + ", STATUS_MAIL_NEW"
                                         + ", DATA_OPERAZIONE"
                                         + ", UTE_OPE)"
                                         + " VALUES (:pREF_ID_MAIL, :pSTATUS_MAIL_OLD, :pSTATUS_MAIL_NEW"
                                         + ", LOCALTIMESTAMP, :pUTE_OPE)";

        private const string selectBase = "SELECT MAIL_FILE FROM MAIL_INBOX";
        private const string selectOutboxMessage = "SELECT m.ID_MAIL, m.MAIL_SENDER"
                                                + ", m.MAIL_SUBJECT, m.MAIL_TEXT"
                                                + ", r.\"MAIL_TO\", r.\"MAIL_CC\", r.\"MAIL_CCN\""
                                                + ", m.FLG_ANNULLAMENTO"
                                                + ", m.FLG_CUSTOM_REFS"
                                                + ", m.FOLLOWS"
                                                + ", CA.ALLEG"
                                                + ", m.FOLDERID"
                                                + ", m.FOLDERTIPO"
                                                + " FROM MAIL_CONTENT m"
                                                + " LEFT OUTER JOIN ("
                                                + " SELECT *"
                                                + " FROM MAIL_REFS_NEW"
                                                + " PIVOT"
                                                + " (LISTAGG(MAIL_DESTINATARIO, ';') WITHIN GROUP (ORDER BY ID_REF)"
                                                + " FOR TIPO_REF IN"
                                                + " ('TO' AS \"MAIL_TO\","
                                                + " 'CC' AS \"MAIL_CC\","
                                                + " 'CCN' AS \"MAIL_CCN\")"
                                                + " )"
                                                + " ) r"
                                                + " ON m.id_mail = r.ref_id_mail"
                                                + " LEFT OUTER JOIN"
                                                + " (select ref_id_com, LISTAGG(ma.id_allegato||'#'||ma.allegato_name||'.'||ma.allegato_ext, ';') WITHIN GROUP (ORDER BY ID_ALLEGATO)"
                                                + " AS ALLEG"
                                                + " FROM comunicazioni_allegati ma"
                                                + " GROUP BY ref_id_com"
                                                + ") ca"
                                                + " ON m.ref_id_com = ca.ref_id_com";
        private const string cmdUpdateMessage = "UPDATE MAIL_INBOX"
                                           + " SET MAIL_FROM = :p_MAIL_FROM"
                                              + ", MAIL_TO = :p_MAIL_TO"
                                              + ", MAIL_CC = :p_MAIL_CC"
                                              + ", MAIL_CCN = :p_MAIL_CCN"
                                              + ", MAIL_SUBJECT = :p_MAIL_SUBJECT"
                                              + ", MAIL_TEXT = :p_MAIL_TEXT"
                                              + ", DATA_INVIO = :p_DATA_INVIO"
                                              + ", DATA_RICEZIONE = :p_DATA_RICEZIONE"
                                              + ", STATUS_SERVER = :p_STATUS_SERVER"
                                              + ", STATUS_MAIL = :p_STATUS_MAIL"
                                              + ", MAIL_FILE = :p_MAIL_FILE"
                                              + ", FLG_ATTACHMENTS = :p_FLG_ATTACHMENTS"
                                           //    + ", MAIL_FOLDER = :p_MAIL_FOLDER"
                                              + ", FOLDERID= :p_FOLDERID "
                                              + ", FOLDERTIPO = :p_FOLDERTIPO "
                                              + ", FOLLOWS = :p_FOLLOWS"
                                           + " WHERE MAIL_SERVER_ID = :p_MAIL_SERVER_ID"
                                                 + " AND MAIL_ACCOUNT = :p_MAIL_ACCOUNT";

        #endregion

        #region C.tor

        private OracleSessionManager context;

        public MailMessageDaoOracleDb(OracleSessionManager daoContext)
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

        #region IDao<Message,string> Membri di

        public ICollection<Message> GetAll()
        {
            throw new NotImplementedException();
        }

        public IDictionary<MailStatusServer, List<string>> GetAllUIDsByAccount(string account, List<MailStatusServer> serverStatus)
        {
            Dictionary<MailStatusServer, List<string>> list = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = "SELECT MAIL_SERVER_ID, STATUS_SERVER from MAIL_INBOX where MAIL_ACCOUNT = '" + account + "' AND STATUS_SERVER IN ('" + String.Join("', '", serverStatus.Select(ss => ((int)ss).ToString()).ToArray()) + "')";

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            list = new Dictionary<MailStatusServer, List<string>>();
                            while (r.Read())
                            {
                                MailStatusServer stServer = MailStatusServer.UNKNOWN;
                                if (!r.IsDBNull(1))
                                {
                                    int val = -1;
                                    if (int.TryParse(r.GetString(1), out val))
                                    {
                                        if (Enum.IsDefined(typeof(MailStatusServer), val))
                                            stServer = (MailStatusServer)val;
                                    }

                                }
                                if (!list.ContainsKey(stServer))
                                    list.Add(stServer, new List<string>());
                                list[stServer].Add(r.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //TASK: Allineamento log - Ciro
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo" 
                        + "GetAllUIDsByAccount(string account, List<MailStatusServer> serverStatus). Dettaglio: " + e.Message,
                            "E001", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        
                        _log.Error(err);
                        list = null;
                        throw mEx;
                    }
                    //ManagedException m = new ManagedException(e.Message,"E001",account,"GetAllUIDsByAccount",e); 
                    //ErrorLog error = new ErrorLog(m);
                    //_log.Error(error);
                    list = null;
                    throw e;
                    //throw m;
                }
            }

            return list;
        }

        public ResultList<Message> GetAllMessageByAccount(string account, int da, int per)
        {
            ResultList<Message> listMex = null;
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                StringBuilder sb = new StringBuilder("BEGIN");
                sb.Append(" OPEN :cur_1 FOR SELECT count(*) FROM mail_inbox WHERE mail_account = :p_account;");
                sb.Append(" OPEN :cur_2 FOR SELECT PERCENTILE_DISC(0.8) WITHIN GROUP (ORDER BY msg_length ASC)")
                    .Append(" FROM mail_inbox WHERE mail_account = :p_account;");
                sb.Append(" END;");

                oCmd.CommandType = System.Data.CommandType.Text;
                oCmd.CommandText = sb.ToString();
                oCmd.BindByName = true;
                oCmd.Parameters.Add("p_account", account);
                oCmd.Parameters.Add("cur_1", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);
                oCmd.Parameters.Add("cur_2", OracleDbType.RefCursor, System.Data.ParameterDirection.Output);

                int lobMaxSize = -1;
                try
                {
                    oCmd.ExecuteNonQuery();
                    OracleDataReader r1 = ((OracleRefCursor)oCmd.Parameters["cur_1"].Value).GetDataReader();
                    r1.Read();
                    int tot = Convert.ToInt32(r1.GetDecimal(0));
                    OracleDataReader r2 = ((OracleRefCursor)oCmd.Parameters["cur_2"].Value).GetDataReader();
                    r2.Read();
                    lobMaxSize = (int)r2.GetDecimal(0);
                    r1.Close();
                    r2.Close();
                    r1.Dispose();
                    r2.Dispose();

                    listMex = new ResultList<Message>();
                    listMex.Da = da;
                    listMex.Per = (tot > per) ? per : tot;
                    listMex.Totale = (tot > per) ? tot : per;
                    if (tot == default(int))
                    {
                        listMex.List = null;
                        return listMex;
                    }
                }
                catch (Exception e)
                {
                    //TASK: Allineamento log - Ciro
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo GetAllMessageByAccount(). Dettaglio: " + e.Message,
                            "E002", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                        listMex = null;
                        throw mEx;
                    }
                    //ManagedException m = new ManagedException(e.Message, "E002", account, "GetAllMessageByAccount", e); 
                    //ErrorLog error = new ErrorLog(m);
                    //_log.Error(error);
                    listMex = null;
                    throw e;
                    //throw m;
                }

                sb = new StringBuilder();
                sb.Append("SELECT * FROM (")
                    .Append("SELECT ID_MAIL, MAIL_SERVER_ID, MAIL_FILE, rownum as \"rn\" FROM MAIL_INBOX WHERE mail_account = :p_account order by id_mail")
                    .Append(") WHERE \"rn\" between :p_da and :p_a");

                oCmd.CommandText = sb.ToString();
                oCmd.InitialLOBFetchSize = lobMaxSize;
                oCmd.BindByName = true;
                da = (da == 0) ? 1 : da;
                int a = da + per - 1;
                oCmd.Parameters.Add("p_da", da);
                oCmd.Parameters.Add("p_a", a);

                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                    {
                        if (r.HasRows)
                        {
                            listMex.List = new List<Message>();
                            byte[] buffer = null;
                            long charRead, readStartIndex = 0, bufferStartIndex = 0;
                            while (r.Read())
                            {
                                Message mex = new Message();
                                mex.Id = r.GetInt32("ID_MAIL");
                                mex.Uid = r.GetString("MAIL_SERVER_ID");

                                long clobDataSize = r.GetBytes(0, 0, null, 0, 0);
                                buffer = new byte[clobDataSize];
                                charRead = r.GetBytes(0, readStartIndex, buffer, (int)bufferStartIndex, BUFFER_SIZE);
                                while (charRead == BUFFER_SIZE)
                                {
                                    bufferStartIndex += BUFFER_SIZE;
                                    readStartIndex += charRead;
                                    charRead = r.GetBytes(0, readStartIndex, buffer, (int)bufferStartIndex, BUFFER_SIZE);
                                }
                                mex.OriginalData = buffer;
                                listMex.List.Add(mex);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    //TASK: Allineamento log - Ciro
                    if (!e.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel metodo GetAllMessageByAccount(). Dettaglio: " + e.Message,
                            "E003", string.Empty, string.Empty, e.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        //err.userID = account;
                        _log.Error(err);
                        listMex = null;
                        throw mEx;
                    }
                    //ManagedException m = new ManagedException(e.Message, "E003", account, "GetAllMessageByAccount", e); 
                    //ErrorLog error = new ErrorLog(m);
                    //_log.Error(error);
                    listMex.List = null;
                    throw e;
                }
            }
            return listMex;
        }
        
        public Message GetById(string id)
        {
            Message msg = null;
            try
            {
                using (OracleCommand cmd = base.CurrentConnection.CreateCommand())  
                {
                    cmd.CommandText = "SELECT ID_MAIL, MAIL_FILE, MAIL_SERVER_ID FROM MAIL_INBOX WHERE ID_MAIL = :p_id_mail";
                    cmd.Parameters.Add("p_id_mail", id);
                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            msg = new Message();
                            msg.Id = (int)r.GetInt64("ID_MAIL");
                            msg.Uid = r.GetString("MAIL_SERVER_ID");
                            long lobSize = r.GetBytes(r.GetOrdinal("MAIL_FILE"), 0, null, 0, 0);
                            OracleClob file = r.GetOracleClob(r.GetOrdinal("MAIL_FILE"));
                            msg.OriginalData = new byte[lobSize];
                            file.Read(msg.OriginalData, 0, (int)lobSize);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nel metodo GetById(string id). Dettaglio: " + e.Message,
                        "E004", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    //err.userID = id;
                    _log.Error(err);
                }
                //ManagedException m = new ManagedException(e.Message, "E004", id, "GetById", e); 
                //ErrorLog error = new ErrorLog(m);
                //_log.Error(error);                
            }
            return msg;
        }

        /// <summary>
        ///  MODIFICATA GESTIONE FOLDER
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public System.Tuples.Tuple<Message, string, int,string> GetMessageById(string id)
        {
            System.Tuples.Tuple<Message,string, int,string> tuple = null;
            try
            {
                using (OracleCommand cmd = base.CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = "SELECT ID_MAIL, MAIL_FILE, MAIL_SERVER_ID, FLG_RATING, MAIL_FOLDER, STATUS_MAIL,FOLDERID,FOLDERTIPO FROM MAIL_INBOX WHERE ID_MAIL = :p_id_mail";
                    cmd.Parameters.Add("p_id_mail", id);

                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            tuple = DaoOracleDbHelper.MapToMessageTuple(r);
                        }
                    }
                }
                if (tuple == null)
                {
                    Message msg = GetById(id, null, "2");
                    if (msg != null)
                    {
                        tuple = new System.Tuples.Tuple<Message, string, int,string>();
                        tuple.Element1 = msg;
                        tuple.Element2 = msg.FolderId.ToString()+msg.FolderTipo;
                        tuple.Element3 = -1;
                        tuple.Element4 = "2";
                    }
                }
            }
            catch(Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nel metodo GetMessageById(string id). Dettaglio: " + e.Message,
                        "E005", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    //err.userID = id;
                    _log.Error(err);
                }
                //ManagedException m = new ManagedException(e.Message, "E005", id, "GetMessageById", e); 
                //ErrorLog error = new ErrorLog(m);
                //_log.Error(error);       
            }
            return tuple;
        }

        /// <summary>
        /// modificata gestione folder
        /// </summary>
        /// <param name="mailID"></param>
        /// <param name="mailAccount"></param>
        /// <param name="mailFolder"></param>
        /// <returns></returns>
        public Message GetById(string mailID, string mailAccount, string mailFolder)
        {
            Message msg = null;
            try
            {
                switch (mailFolder.Substring(0, 1))
                {
                    case "1":
                    case "3":
                        msg = GetInboxMessage(mailID, mailAccount);
                        break;
                    case "2":
                        msg = GetOutBoxMessage(mailID, mailAccount);
                        break;
                    default:
                        switch (mailFolder.Substring(1, 1))
                        {
                            case "I":
                                msg = GetInboxMessage(mailID, mailAccount);
                                break;
                            case "O":
                                msg = GetOutBoxMessage(mailID, mailAccount);
                                break;
                            default:
                                msg = GetInboxMessage(mailID, mailAccount);
                                if (msg != null)
                                {
                                    msg = GetOutBoxMessage(mailID, mailAccount);
                                }
                                break;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                //TASK: Allineamento log - Ciro
                if (!e.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nel metodo GetById(string mailID, string mailAccount, string mailFolder). Dettaglio: " + e.Message,
                        "E006", string.Empty, string.Empty, e.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    //err.userID = mailAccount;
                    _log.Error(err);
                }
                //ManagedException m = new ManagedException(e.Message, "E006", mailID, "GetById", e);
                //ErrorLog error = new ErrorLog(m);
                //_log.Error(error);
            }

            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comId"></param>
        /// <returns></returns>
        public Message GetOutBoxMessageByComId(string comId)
        {
            Message msg = null;
            using (OracleCommand oCmd0 = base.CurrentConnection.CreateCommand())
            {
                oCmd0.CommandText = selectOutboxMessage + " WHERE m.ref_id_com = :p_id_com";
                oCmd0.Parameters.Add("p_id_com", int.Parse(comId));

                using (OracleDataReader or = oCmd0.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (or.HasRows)
                    {
                        msg = new Message();
                        // recupero solo la prima riga
                        or.Read();
                        msg = DaoOracleDbHelper.MapOutboxToMailMessage(or);
                        msg.Uid = msg.Id.ToString();
                    }
                }
            }
            return msg;
        }

        public void Insert(Message entity)
        {
            throw new NotImplementedException();
        }

        public void Insert(MailUser user, Message m)
        {
            using (OracleCommand cmd = base.CurrentConnection.CreateCommand())
            {
                Context.StartTransaction(this.GetType());
                cmd.CommandText = insertMessage;               
                cmd.Parameters.AddRange(MapToOracleParams(user, m));
                cmd.BindByName = true;
                int rows = 0;
                try
                {
                    rows = cmd.ExecuteNonQuery();
                }
                catch (OracleException oEx)
                {
                    //Allineamento log - Ciro
                    ManagedException mEx = mEx = new ManagedException("Errore nell'inserimento su DB della mail con uid: " + m.Uid
                            + " della casella " + user.EmailAddress,
                            "ERR_INS_ML_001", string.Empty, string.Empty, oEx.InnerException);
                    mEx.addEnanchedInfosTag("DETTAGLIO", "Classe: MailMessageDaoOracleDb " + "Metodo: Insert(MailUser user, Message m) " + 
                        "Dettaglio: Salvataggio della mail su Oracle DB " +
                        "User Login: " + user.LoginId + " Mail UID: " + m.Uid);
                            //"MailMessageDaoOracleDb",
                            //"Insert(MailUser user, Message m)",
                            //"Salvataggio della mail su Oracle DB",
                            //user.LoginId,
                            //m.Uid,
                            //oEx);
                    ErrorLogInfo err;

                    if (oEx.Message.StartsWith("ORA-00001", StringComparison.InvariantCultureIgnoreCase))
                    {
                        mEx.CodiceEccezione = "WRN_INS_ML_001";
                        err = new ErrorLogInfo(mEx);
                        _log.Warn(err);
                    }
                    else
                    {
                        err = new ErrorLogInfo(mEx);
                        _log.Error(err);
                    }
                    Context.RollBackTransaction(this.GetType());
                    throw mEx;
                }
                catch (Exception ex)
                {
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException(ex.Message,
                            "ERR_COM_006", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
            
                        _log.Error(err);
                    }
                    //ErrorLogInfo error = new ErrorLogInfo();
                    //error.freeTextDetails = ex.Message;
                    //error.logCode = "ERR_COM_006";
                    //_log.Error(error);
                    Context.RollBackTransaction(this.GetType());
                    throw;
                }

                if (rows == 1)
                {
                    Int64 id = Convert.ToInt64(cmd.Parameters["pID_MAIL"].Value.ToString());
                    m.Id = (int)id;

                    cmd.CommandText = "UPDATE mail_inbox SET mail_file = :p_FILE WHERE id_mail = :p_ID_MAIL";
                    cmd.Parameters.Clear();
                    cmd.BindByName = true;
                    cmd.Parameters.Add("p_ID_MAIL", id);
                    OracleClob clob = new OracleClob(CurrentConnection);
                    char[] msgData = Encoding.GetEncoding("iso-8859-1").GetChars(m.OriginalData);
                    clob.BeginChunkWrite();
                    int BUFFER_SIZE = 1024, startOffset = 0, writeBytes = 0;
                    do
                    {
                        writeBytes = startOffset + BUFFER_SIZE > msgData.Length ? msgData.Length - startOffset : BUFFER_SIZE;
                        clob.Write(msgData, startOffset, writeBytes);
                        startOffset += writeBytes;
                    } while (startOffset < msgData.Length);
                    clob.EndChunkWrite();
                    
                    OracleParameter par = new OracleParameter("p_FILE", OracleDbType.Clob);
                    par.Value = clob;
                    cmd.Parameters.Add(par);

                    try
                    {
                        int risp = cmd.ExecuteNonQuery();
                        if (risp != 1)
                            throw new InvalidOperationException("Errore nel salvataggio del MIME in banca dati.");
                    }
                    catch
                    {
                        Context.RollBackTransaction(this.GetType());
                        throw;
                    }

                    try
                    {
                        MailStatus newStatus = MailStatus.SCARICATA;
                        if (String.IsNullOrEmpty(m.MessageId))
                            newStatus = MailStatus.SCARICATA_INCOMPLETA;
                        InsertFlussoInbox(id, MailStatus.UNKNOWN, newStatus, m.ReceivedDate, "SYSTEM");
                    }
                    catch
                    {
                        Context.RollBackTransaction(this.GetType());
                        throw;
                    }
                }
                else
                {
                    Context.RollBackTransaction(this.GetType());
                    throw new Exception("Messaggio non inserito in banca dati");
                }
                Context.EndTransaction(this.GetType());
            }
        }

        public void Update(Message entity)
        {
            throw new NotImplementedException();
        }

        public void Update(MailUser u, Message entity)
        {
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = cmdUpdateMessage;
                oCmd.BindByName = true;
                oCmd.Parameters.AddRange(MapToOracleUpdateParams(u, entity));
                try
                {
                    oCmd.ExecuteNonQuery();
                }
                catch
                {
                }
            }
        }

        public int UpdateRating(string idMail, string account, int rating)
        {
            if (String.IsNullOrEmpty(idMail) && String.IsNullOrEmpty(account) &&
                (rating < 0 || rating > 4))
            {
                throw new ArgumentException("Errore nei parametri");
            }

            int result = 0;

            using (OracleCommand ocmd = base.CurrentConnection.CreateCommand())
            {
                ocmd.CommandText = String.Format("UPDATE MAIL_INBOX SET FLG_RATING = {0} WHERE MAIL_SERVER_ID = '{1}' AND MAIL_ACCOUNT = '{2}'",
                    rating, idMail, account);

                base.Context.StartTransaction(this.GetType());
                result = ocmd.ExecuteNonQuery();

                if (result != 1)
                {
                    base.Context.RollBackTransaction(this.GetType());
                }
                else
                {
                    base.Context.EndTransaction(this.GetType());
                }
            }

            return result;
        }

        public int UpdateServerStatus(string account, string mailUid, MailStatusServer status)
        {
            int rows = 0;
            try
            {
                using (OracleCommand cmd = base.CurrentConnection.CreateCommand())
                {
                    cmd.CommandText = string.Format("UPDATE MAIL_INBOX SET STATUS_SERVER = '{0}' WHERE MAIL_SERVER_ID = '{1}' AND MAIL_ACCOUNT = '{2}'",
                        (int)status, mailUid, account);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            catch
            {
            }
            return rows;
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        public void Delete(string mailID, string mailAccount)
        {
            throw new NotImplementedException();
        }

        public void InsertFlussoInbox(Int64 id, MailStatus oldSt, MailStatus newSt, DateTime? dataOp, string uteOpe)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = insertFlusso;
                    oCmd.BindByName = true;
                    oCmd.Parameters.AddRange(MapToOracleParams(id, (oldSt == MailStatus.UNKNOWN) ? null : ((int)oldSt).ToString(), ((int)newSt).ToString(), dataOp, uteOpe));
                    oCmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #endregion

        #region Mapping

        private OracleParameter[] MapToOracleParams(MailUser us, Message m)
        {
            OracleParameter[] pars = new OracleParameter[20];
            pars[0] = new OracleParameter("pMAIL_SERVER_ID", OracleDbType.Varchar2, 100);
            pars[0].Value = m.Uid.Replace(',','§');
            pars[1] = new OracleParameter("pMAIL_ACCOUNT", OracleDbType.Varchar2, 150);
            pars[1].Value = us.EmailAddress;
            pars[2] = new OracleParameter("pMAIL_FROM", OracleDbType.Varchar2, 150);
            pars[2].Value = m.From.ToString();
            pars[3] = new OracleParameter("pMAIL_TO", OracleDbType.Varchar2, 1000);
            pars[3].Value = String.Join(";", m.To.Select(to => to.Email).ToArray());
            pars[4] = new OracleParameter("pMAIL_CC", OracleDbType.Varchar2, 1000);
            pars[4].Value = String.Join(";", m.Cc.Select(cc => cc.Email).ToArray());
            pars[5] = new OracleParameter("pMAIL_CCN", OracleDbType.Varchar2, 1000);
            pars[5].Value = String.Join(";", m.Bcc.Select(bcc => bcc.Email).ToArray());
            pars[6] = new OracleParameter("pMAIL_SUBJECT", OracleDbType.Varchar2, 300);
            pars[6].Value = m.Subject;
            pars[7] = new OracleParameter("pMAIL_TEXT", OracleDbType.Varchar2, 150);
            if (m.BodyHtml != null && !string.IsNullOrEmpty(m.BodyHtml.TextStripped))
            {
                pars[7].Value = m.BodyHtml.TextStripped;
            }
            else if (m.BodyText != null && !String.IsNullOrEmpty(m.BodyText.TextStripped))
            {
                pars[7].Value = m.BodyText.TextStripped;
            }
            pars[8] = new OracleParameter("pDATA_INVIO", OracleDbType.Date);
            pars[8].Value = m.ReceivedDate.ToLocalTime();
            pars[9] = new OracleParameter("pDATA_RICEZIONE", OracleDbType.Date);
            pars[9].Value = m.Date.ToLocalTime();
            pars[10] = new OracleParameter("pSTATUS_SERVER", OracleDbType.Varchar2, 1);
            pars[11] = new OracleParameter("pSTATUS_MAIL", OracleDbType.Varchar2, 1);
            if (String.IsNullOrEmpty(m.MessageId))
            {
                pars[10].Value = ((int)MailStatusServer.DA_NON_CANCELLARE).ToString();
                pars[11].Value = ((int)MailStatus.SCARICATA_INCOMPLETA).ToString();
            }
            else
            {
                pars[10].Value = ((int)MailStatusServer.PRESENTE).ToString();
                pars[11].Value = ((int)MailStatus.SCARICATA).ToString();
            }

            pars[12] = new OracleParameter("pFLG_INTEG_APPL", OracleDbType.Varchar2, 1);
            pars[12].Value = "0";
            pars[13] = new OracleParameter("pMAIL_FILE", OracleDbType.Clob);
            //pars[13].Value = System.Text.Encoding.GetEncoding("iso-8859-1").GetString(m.OriginalData);
            pars[13].Value = "Inserting... LOB size=" + m.OriginalData.Length;
            pars[14] = new OracleParameter("pFLG_ATTACHMENTS", OracleDbType.Varchar2);
            pars[14].Value = Convert.ToInt16(m.Attachments.Count > 0).ToString();
          //  pars[15] = new OracleParameter("pMAIL_FOLDER", OracleDbType.Varchar2, 2);
            pars[15] = new OracleParameter("pFOLDERID", OracleDbType.Decimal);        
            if (String.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]))
            {
                pars[15].Value = 1;
            }
            else
            {
                pars[15].Value = 3;
            }
            pars[16] = new OracleParameter("pFOLDERTIPO", OracleDbType.Varchar2, System.Data.ParameterDirection.Input);
            pars[16].Value = "I";
            pars[17] = new OracleParameter("pFOLLOWS", OracleDbType.Int64);
            if (!string.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]) &&
                !m.HeaderFields["X-Ricevuta"].Equals("posta-certificata") &&
                !String.IsNullOrEmpty(m.HeaderFields["X-Riferimento-Message-ID"]))
            {
                string idOldString = m.HeaderFields["X-Riferimento-Message-ID"];
                if (idOldString.StartsWith("<"))
                    idOldString = idOldString.Substring(1);
                if (idOldString.EndsWith(">"))
                    idOldString = idOldString.Substring(0, idOldString.Length - 1);
                string[] idOldStr = idOldString.Split('.');
                Int64 idOld = -1;
                if (idOldStr.Length > 0 && Int64.TryParse(idOldStr[0], out idOld))
                {
                    pars[17].Value = idOld;
                }
            }
            pars[18] = new OracleParameter("pMSGLENGTH", OracleDbType.Int64, System.Data.ParameterDirection.Input);
            pars[18].Value = m.OriginalData.Length;
            pars[19] = new OracleParameter("pID_MAIL", OracleDbType.Int64, System.Data.ParameterDirection.Output);       
            return pars;
        }

        private OracleParameter[] MapToOracleUpdateParams(MailUser u, Message m)
        {
            OracleParameter[] pars = new OracleParameter[17];
            pars[0] = new OracleParameter("p_MAIL_FROM", OracleDbType.Varchar2, 150);
            pars[0].Value = m.From.ToString();
            pars[1] = new OracleParameter("p_MAIL_TO", OracleDbType.Varchar2, 1000);
            pars[1].Value = String.Join(";", m.To.Select(to => to.Email).ToArray());
            pars[2] = new OracleParameter("p_MAIL_CC", OracleDbType.Varchar2, 1000);
            pars[2].Value = String.Join(";", m.Cc.Select(cc => cc.Email).ToArray());
            pars[3] = new OracleParameter("p_MAIL_CCN", OracleDbType.Varchar2, 1000);
            pars[3].Value = String.Join(";", m.Bcc.Select(bcc => bcc.Email).ToArray());
            pars[4] = new OracleParameter("p_MAIL_SUBJECT", OracleDbType.Varchar2, 300);
            pars[4].Value = m.Subject;
            pars[5] = new OracleParameter("p_MAIL_TEXT", OracleDbType.Varchar2, 150);
            if (m.BodyHtml != null && !string.IsNullOrEmpty(m.BodyHtml.TextStripped))
            {
                pars[5].Value = m.BodyHtml.TextStripped;
            }
            else if (m.BodyText != null && !String.IsNullOrEmpty(m.BodyText.TextStripped))
            {
                pars[5].Value = m.BodyText.TextStripped;
            }
            pars[6] = new OracleParameter("p_DATA_INVIO", OracleDbType.Date);
            pars[6].Value = m.ReceivedDate.ToLocalTime();
            pars[7] = new OracleParameter("p_DATA_RICEZIONE", OracleDbType.Date);
            pars[7].Value = m.Date.ToLocalTime();
            pars[8] = new OracleParameter("p_STATUS_SERVER", OracleDbType.Varchar2, 1);
            pars[9] = new OracleParameter("p_STATUS_MAIL", OracleDbType.Varchar2, 1);
            if (String.IsNullOrEmpty(m.MessageId))
            {
                pars[8].Value = ((int)MailStatusServer.DA_NON_CANCELLARE).ToString();
                pars[9].Value = ((int)MailStatus.SCARICATA_INCOMPLETA).ToString();
            }
            else
            {
                pars[8].Value = ((int)MailStatusServer.PRESENTE).ToString();
                pars[9].Value = ((int)MailStatus.SCARICATA).ToString();
            }

            pars[10] = new OracleParameter("p_MAIL_FILE", OracleDbType.Clob);
            pars[10].Value = System.Text.Encoding.GetEncoding("iso-8859-1").GetString(m.OriginalData);
            pars[11] = new OracleParameter("p_FLG_ATTACHMENTS", OracleDbType.Varchar2);
            pars[11].Value = Convert.ToInt16(m.Attachments.Count > 0).ToString();
          //  pars[12] = new OracleParameter("p_MAIL_FOLDER", OracleDbType.Varchar2, 2);
            pars[12] = new OracleParameter("p_FOLDERID", OracleDbType.Decimal, 1);
            if (String.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]))
            {
                pars[12].Value = 1;
            }
            else
            {
                pars[12].Value = 3;
            }
            pars[13] = new OracleParameter("p_FOLLOWS", OracleDbType.Int64);
            if (!string.IsNullOrEmpty(m.HeaderFields["X-Ricevuta"]) &&
                !m.HeaderFields["X-Ricevuta"].Equals("posta-certificata") &&
                !String.IsNullOrEmpty(m.HeaderFields["X-Riferimento-Message-ID"]))
            {
                string idOldString = m.HeaderFields["X-Riferimento-Message-ID"];
                if (idOldString.StartsWith("<"))
                    idOldString = idOldString.Substring(1);
                if (idOldString.EndsWith(">"))
                    idOldString = idOldString.Substring(0, idOldString.Length - 1);
                string[] idOldStr = idOldString.Split('.');
                Int64 idOld = -1;
                if (idOldStr.Length > 0 && Int64.TryParse(idOldStr[0], out idOld))
                {
                    pars[13].Value = idOld;
                }
            }
            pars[14] = new OracleParameter("p_MAIL_SERVER_ID", OracleDbType.Varchar2, 20);
            pars[14].Value = m.Uid;
            pars[15] = new OracleParameter("p_MAIL_ACCOUNT", OracleDbType.Varchar2, 150);
            pars[15].Value = u.EmailAddress;
            pars[16].Value = "I";
            return pars;
        }

        private OracleParameter[] MapToOracleParams(Int64 id, string oldStatus, string newStatus, DateTime? data, string ute)
        {
            OracleParameter[] pars = new OracleParameter[4];
            pars[0] = new OracleParameter("pREF_ID_MAIL", OracleDbType.Int64);
            pars[0].Value = id;
            pars[1] = new OracleParameter("pSTATUS_MAIL_OLD", OracleDbType.Varchar2, 1);
            if (!String.IsNullOrEmpty(oldStatus))
                pars[1].Value = oldStatus;
            pars[2] = new OracleParameter("pSTATUS_MAIL_NEW", OracleDbType.Varchar2, 1);
            pars[2].Value = newStatus;
            //pars[3] = new OracleParameter("pDATA_OPERAZIONE", OracleDbType.Date);
            //pars[3].Value = data ?? null;
            pars[3] = new OracleParameter("pUTE_OPE", OracleDbType.Varchar2, 50);
            pars[3].Value = ute;

            return pars;
        }
        #endregion

        #region IDao<Message,string> Membri di




        #endregion

        #region Private Method

        private Message GetOutBoxMessage(string mailID,string mailAccount)
        {
            Message msg = null;         
            using (OracleCommand oCmd0 = base.CurrentConnection.CreateCommand())
            {
                oCmd0.CommandText = selectOutboxMessage + " WHERE id_mail = " + int.Parse(mailID);
                using (OracleDataReader or = oCmd0.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (or.HasRows)
                    {
                        msg = new Message();
                        while (or.Read())
                        {
                            msg = DaoOracleDbHelper.MapOutboxToMailMessage(or);
                            msg.Uid = mailID;
                        }
                    }
                }
            }
            return msg;
        }

        private Message GetInboxMessage(string mailID, string mailAccount)
        {
            Message msg = null;          
            using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
            {
                oCmd.CommandText = "SELECT ID_MAIL, MAIL_FILE,FOLDERID,FOLDERTIPO FROM MAIL_INBOX WHERE MAIL_SERVER_ID = :p_server_id AND MAIL_ACCOUNT = :p_account";
                oCmd.BindByName = true;
                oCmd.Parameters.Add("p_server_id", mailID);
                oCmd.Parameters.Add("p_account", mailAccount);
                oCmd.InitialLOBFetchSize = BUFFER_SIZE;
                try
                {
                    using (OracleDataReader r = oCmd.ExecuteReader(System.Data.CommandBehavior.SequentialAccess))
                    {
                        if (r.HasRows)
                        {
                            msg = new Message();
                            while (r.Read())
                            {
                                if (!r.IsDBNull("ID_MAIL"))
                                {
                                    // msg.Id = idMail = (int)r.GetInt64(0);
                                    //  msgText = r.GetString(1);
                                    //  msg.Uid = mailID;
                                    long lobSize = r.GetChars(r.GetOrdinal("MAIL_FILE"), 0, null, 0, 0);
                                    char[] file = new char[lobSize];
                                    r.GetChars(r.GetOrdinal("MAIL_FILE"), 0, file, 0, (int)lobSize);
                                    msg.OriginalData = Encoding.UTF8.GetBytes(file);
                                    //  msg = Parser.ParseMessage(new string(file));
                                    msg.Uid = mailID;
                                    msg.Id = (int)r.GetInt64("ID_MAIL");
                                    msg.FolderId = r.GetDecimal("FOLDERID");
                                    msg.FolderTipo = r.GetString("FOLDERTIPO");
                                    msg.ParentFolder = "1";
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
            return msg;
        }
        #endregion
    }
}
