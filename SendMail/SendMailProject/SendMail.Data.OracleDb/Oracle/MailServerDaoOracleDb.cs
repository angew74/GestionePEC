using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Delta.Data;
using Oracle.DataAccess.Client;
using System.Data;
using Com.Delta.Logging;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.DataContracts.Interfaces;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.Utilities;
using SendMailApp.OracleCore.Contracts;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMail.Data.OracleDb
{
    public class MailServerDaoOracleDb: Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IMailServerDao
    {
        #region private
        private static readonly ILog log = LogManager.GetLogger(typeof(MailServerDaoOracleDb));

        private OracleSessionManager context;

        // Raffaele Russo - 31/08/2012 - Start
        //private const string insertStatement = "INSERT INTO MAILSERVERS "
        //                 +"(ID_SVR, NOME, INDIRIZZO_IN, PROTOCOLLO_IN, PORTA_IN, SSL_IN, INDIRIZZO_OUT, PORTA_OUT, SSL_OUT, AUTH_OUT)"
        //                 +"VALUES (RUBRICA_SEQ.nextval, :pNOME, :pINDIRIZZO_IN, :pPROTOCOLLO_IN, :pPORTA_IN, :pSSL_IN, :pINDIRIZZO_OUT, :pPORTA_OUT, :pSSL_OUT, :pAUTH_OUT)"
        //                 +" RETURNING ID_TIT INTO :pID_SVR";

        private const string insertStatement = "INSERT INTO MAILSERVERS "
                         + "(ID_SVR, NOME, INDIRIZZO_IN, PROTOCOLLO_IN, PORTA_IN, SSL_IN, INDIRIZZO_OUT, PORTA_OUT, SSL_OUT, AUTH_OUT, DOMINUS, FLG_ISPEC)"
                         + "VALUES (MAIL_SERVER_SEQ.nextval, :pNOME, :pINDIRIZZO_IN, :pPROTOCOLLO_IN, :pPORTA_IN, :pSSL_IN, :pINDIRIZZO_OUT, :pPORTA_OUT, :pSSL_OUT, :pAUTH_OUT, :pDOMINUS, :FLG_ISPEC)"
                         + " RETURNING ID_SVR INTO :pID_SVR";
        // Raffaele Russo - 31/08/2012 - End

        // Raffaele Russo - 31/08/2012 - Start
        //private const string updateStatement = "UPDATE MAILSERVERS "
        //                                +"SET ID_SVR = :pID_SVR, NOME = :pNOME, INDIRIZZO_IN = :pINDIRIZZO_IN, PROTOCOLLO_IN = :pPROTOCOLLO_IN, PORTA_IN = :pPORTA_IN, SSL_IN = :pSSL_IN,"
        //                                +" INDIRIZZO_OUT = :pINDIRIZZO_OUT, PORTA_OUT = :pPORTA_OUT, SSL_OUT = :pSSL_OUT, AUTH_OUT = :pAUTH_OUT" 
        //                                +" WHERE (ID_SVR = :pID_SVR)";

        private const string updateStatement = "UPDATE MAILSERVERS "
                                        + "SET NOME = :pNOME, INDIRIZZO_IN = :pINDIRIZZO_IN, PROTOCOLLO_IN = :pPROTOCOLLO_IN, PORTA_IN = :pPORTA_IN, SSL_IN = :pSSL_IN,"
                                        + " INDIRIZZO_OUT = :pINDIRIZZO_OUT, PORTA_OUT = :pPORTA_OUT, SSL_OUT = :pSSL_OUT, AUTH_OUT = :pAUTH_OUT, DOMINUS = :pDOMINUS, FLG_ISPEC = :FLG_ISPEC" 
                                        + " WHERE (ID_SVR = :pID_SVR)";
        // Raffaele Russo - 31/08/2012 - End


        private const string deleteStatement = "DELETE FROM MAILSERVERS WHERE ID_SVR = :pID_SVR";
        #endregion
        public MailServerDaoOracleDb(OracleSessionManager daoContext) 
            : base(daoContext)
        {
            context = daoContext;
            //todo.. RIVEDERE
            //apro la cn se non è già aperta.
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

        public ICollection<MailServer> GetAll()
        {
           using (OracleCommand ocmd = new OracleCommand())
            {
                // preparo il command
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM MAILSERVERS";
                // eseguo il command
                try
                {
                    return DaoOracleDbHelper<MailServer>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToMailServer);
                }
                catch (OracleException oex)
                {
                    //Allineamento log - Ciro
                    if (oex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO + oex.Message,
                                        "DAL_TIT_001",
                                        string.Empty,
                                        string.Empty,
                                        oex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_001", "", "", "", "", "", oex);
                }
            }
        }

        public MailServer GetById(decimal id)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM MAILSERVERS WHERE ID_SVR = :pID_SVR";
                ocmd.Parameters.Add("pID_SVR", id);

                // eseguo il command
                try
                {
                    ICollection<MailServer> find = DaoOracleDbHelper<MailServer>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToMailServer);
                    if (find.Count > 0)
                        return find.First();
                    else
                        return null;
                }
                catch (OracleException oex)
                {
                    //Allineamento log - Ciro
                    if (oex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO + oex.Message,
                                        "DAL_TIT_010",
                                        string.Empty,
                                        string.Empty,
                                        oex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_010", "", "", "", "", "", oex);
                }
            }
        }

        public void Insert(MailServer entity)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {

                ocmd.CommandText = insertStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(entity, true));
                ocmd.Connection = base.CurrentConnection;

                try
                {
                    ocmd.ExecuteNonQuery();

                    //param out
                    decimal iNewID = default(decimal);
                    decimal.TryParse(ocmd.Parameters["pID_SVR"].Value.ToString(), out iNewID);

                    //todo.. MIGLIORARE
                    if (iNewID != default(int))
                    {
                        entity.Id = iNewID;
                    }
                    else
                        throw new Exception(DalExMessages.ID_NON_RESTITUITO);
                }
                catch (InvalidOperationException ioex)
                {
                    //Allineamento log - Ciro
                    if (ioex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA + ioex.Message,
                                        "DAL_RUB_002",
                                        string.Empty,
                                        string.Empty,
                                        ioex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_002", "", "", "", "", "", ioex);

                }
                catch (OracleException oex)
                {
                    //Allineamento log - Ciro
                    if (oex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA + oex.Message,
                                        "DAL_RUB_001",
                                        string.Empty,
                                        string.Empty,
                                        oex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                    else throw;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_001", "", "", "", "", "", oex);

                }
            }
        }

        public void Update(MailServer entity)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {

                ocmd.CommandText = updateStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(entity, false));
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    int rowAff = ocmd.ExecuteNonQuery();
                    //todo.. MIGLIORARE
                    if (rowAff != 1)
                    {
                        //Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009",
                            string.Empty,
                            string.Empty,
                            null);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        log.Error(er);
                        throw mEx;
                    }
                        //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);

                }
                catch (InvalidOperationException ex)
                {

                    //Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA + " " + ex.Message,
                        "DAL_UNIQUE_CODE",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                }
            }
        }

        public void Delete(decimal id)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandText = deleteStatement;
            ocmd.Parameters.Add("pID_SVR", id);
            ocmd.Connection = base.CurrentConnection;
            try
            {
                ocmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA + " " + ex.Message,
                                "DAL_UNIQUE_CODE_1",
                                string.Empty,
                                string.Empty,
                                ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                else throw;
                //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
            }
        }

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #region mapping

        private OracleParameter[] MapObjectToParams(MailServer r, bool isInsert)
        {
            OracleParameter[] oparams = new OracleParameter[12];

            oparams[0] = new OracleParameter("pNOME", OracleDbType.Varchar2, 0,r.DisplayName, ParameterDirection.Input);
            oparams[1] = new OracleParameter("pINDIRIZZO_IN", OracleDbType.Varchar2, 0, r.IncomingServer, ParameterDirection.Input);
            oparams[2] = new OracleParameter("pPROTOCOLLO_IN", OracleDbType.Varchar2, 0, r.IncomingProtocol, ParameterDirection.Input);
            oparams[3] = new OracleParameter("pPORTA_IN", OracleDbType.Varchar2, 0, r.PortIncomingServer, ParameterDirection.Input);
            oparams[4] = new OracleParameter("pSSL_IN", OracleDbType.Varchar2, 0, r.IsIncomeSecureConnection, ParameterDirection.Input);
            oparams[5] = new OracleParameter("pINDIRIZZO_OUT", OracleDbType.Varchar2, 0, r.OutgoingServer, ParameterDirection.Input);
            oparams[6] = new OracleParameter("pPORTA_OUT", OracleDbType.Varchar2, 0, r.PortOutgoingServer, ParameterDirection.Input);
            oparams[7] = new OracleParameter("pSSL_OUT", OracleDbType.Varchar2, 0, r.IsOutgoingSecureConnection, ParameterDirection.Input);
            oparams[8] = new OracleParameter("pAUTH_OUT", OracleDbType.Varchar2, 0, r.IsOutgoingWithAuthentication, ParameterDirection.Input);
            oparams[9] = new OracleParameter("pDOMINUS", OracleDbType.Varchar2, 0, r.Dominus, ParameterDirection.Input);

            if (r.IsPec)
            {
                oparams[10] = new OracleParameter("pFLG_ISPEC", OracleDbType.Varchar2, 0, "1", ParameterDirection.Input);
            }
            else if (!r.IsPec)
            {
                oparams[10] = new OracleParameter("pFLG_ISPEC", OracleDbType.Varchar2, 0, "0", ParameterDirection.Input);
            }
            
            if (isInsert)
                oparams[11] = new OracleParameter("pID_SVR", OracleDbType.Decimal, 0, r.Id, ParameterDirection.Output);
           
            else
                oparams[11] = new OracleParameter("pID_SVR", OracleDbType.Decimal, 0, r.Id,ParameterDirection.Input);

            return oparams;
        }

        #endregion

    }
}
