using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Com.Delta.Logging;
using System.Data;
using System.Collections;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using SendMail.Data.Utilities;
using SendMailApp.OracleCore.Contracts;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMail.Data.OracleDb
{
    /// <summary>
    /// 
    /// </summary>
    public class TitoloDaoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, ITitoloDao
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(TitoloDaoOracleDb));

        #region statements

        private const string insertStatement= "INSERT INTO COMUNICAZIONI_TITOLI"
                                            + " (ID_TITOLO, APP_CODE, PROT_CODE, TITOLO, NOTE)"
                                            + " VALUES"
                                            + " (RUBRICA_SEQ.nextval, :pAPPCODE, :pPROT_CODE, :pTITOLO, :pNOTE)"
                                            + " RETURNING ID_TITOLO INTO :pID_TITOLO";
        
        private const string updateStatement= "UPDATE COMUNICAZIONI_TITOLI SET"
                                            + "  APP_CODE = :pAPPCODE, PROT_CODE = :pPROT_CODE"
                                            + ", TITOLO = :pTITOLO, NOTE = :pNOTE"
                                            + " WHERE (ID_TITOLO = :pID_TITOLO)";
        
        private const string deleteStatement= "DELETE FROM COMUNICAZIONI_TITOLI WHERE"
                                            + " ID_TITOLO = :pID_TITOLO";

        private const string deleteLogicStatement = "UPDATE COMUNICAZIONI_TITOLI SET"
                                               + "ACTIVE=0 WHERE (ID_TITOLO=:pID_TITOLO)";

        #endregion

        public TitoloDaoOracleDb(OracleSessionManager daoContext) 
            : base(daoContext)
        {
            //todo.. rivedere
            //apro la cn se non è già aperta.
            if (!base.Context.Session_isActive())
            {
                base.Context.Session_Init();
                base.CurrentConnection.Open();
            }
        }

        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #region "ITitoloDao members"

        public Titolo GetById(decimal id)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_TITOLI WHERE ID_TITOLO = :pID";
                ocmd.Parameters.Add("pID", id);

                // eseguo il command
                try
                {
                    ICollection<Titolo> find = DaoOracleDbHelper<Titolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToTitolo);
                    if (find.Count > 0)
                        return find.First();
                    else
                        return null;
                }
                catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_010", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_010", "", "", "", "", "", oex);
                }
            }
        }

        public ICollection<Titolo> GetAll()
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                // preparo il command
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_TITOLI ORDER BY ID_TITOLO";
                // eseguo il command
                try
                {
                    return DaoOracleDbHelper<Titolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToTitolo);
                }
                catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_001", "", "", "", "", "", oex);
                }
            }
        }

        public ICollection<Titolo> GetSottotitoliByIdPadre(decimal titoloKey)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT ID_SOTTOTITOLO AS ID_TITOLO, COM_CODE AS APP_CODE, SOTTOTITOLO AS TITOLO , PROT_CODE, ACTIVE FROM COMUNICAZIONI_SOTTOTITOLI WHERE REF_ID_TITOLO = :pREF_ID_TIT ORDER BY SOTTOTITOLO";
                ocmd.Parameters.Add("pREF_ID_TIT", titoloKey);
                List<string> mappingList= new List<string>(){"ID_TITOLO","TITOLO","PROT_CODE","DELETED","APP_CODE"};
                

                // eseguo il command
                try
                {
                    return DaoOracleDbHelper<Titolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToTitolo, mappingList);
                }
                catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
            
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_001", "", "", "", "", "", oex);
                }
            }
        }


        //Inserisce un nuovo titolo, automaticamente viene inserito anche il sottotiolo di default
        public void Insert(Titolo titolo)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                SottoTitolo s1 = new SottoTitolo(titolo);
                s1.Id = 0;
                s1.RefIdTitolo = titolo.Id;

                ocmd.CommandText = insertStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(titolo, true));
                ocmd.Connection = base.CurrentConnection;
                
                try
                {
                    ocmd.ExecuteNonQuery();
                    
                    //param out
                    Int64 iNewID = default(Int64);
                    Int64.TryParse(ocmd.Parameters["pID_TIT"].Value.ToString(), out iNewID);
                    s1.RefIdTitolo = iNewID;

                    Context.DaoImpl.SottoTitoloDao.Insert(s1);
                    
                    //todo.. MIGLIORARE
                    if (iNewID != default(int))
                    {
                        titolo.Id = iNewID;
                        
                    }
                    else
                        throw new Exception(DalExMessages.ID_NON_RESTITUITO);
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (InvalidOperationException ioex)
                {
                    //throw new DALException(DalExMessages.RUBRICA_NON_INSERITA, ioex);

                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA,
                        "DAL_RUB_002", string.Empty,
                        string.Empty, ioex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;

                    //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_002", "", "", "", "", "", ioex);
               
                }
                catch (OracleException oex)
                {
                    //throw new DALException(DalExMessages.RUBRICA_NON_INSERITA, oex);

                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_INSERITA,
                        "DAL_RUB_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
       
                    log.Error(mEx);
                    throw mEx;

                    //throw new ManagedException(DalExMessages.RUBRICA_NON_INSERITA, "DAL_RUB_001", "", "", "", "", "", oex);
               
                }
            }
        }

        public void Update(Titolo titolo)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
              
                ocmd.CommandText = updateStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(titolo, false));
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    int rowAff = ocmd.ExecuteNonQuery();
                    //todo.. MIGLIORARE
                    if (rowAff != 1)
                        //throw new DALException(DalExMessages.NESSUNA_RIGA_MODIFICATA);
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                    }
                        //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);
                    
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (InvalidOperationException ex)
                {

                    //throw new DALException(DalExMessages.RUBRICA_NON_AGGIORNATA, ex);

                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                        "DAL_UNIQUE_CODE", string.Empty,
                        string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
    
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                }
            }
        }

        //1-se i titoli hanno referenze da 'RICHIESTE' la cancellazione deve essere logica
        //2-se i tioli hanno refrenze a sottotiotli prima devono essere cancellati i sottotitoli
        //3- se però tutti i sottotitoli sono cancellati logicamente allora si può procedere con la cancellazione logica
        public void Delete(decimal id)
        {
            List<Titolo> st= (List<Titolo>)this.GetSottotitoliByIdPadre(id);
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandText = deleteStatement;
            ocmd.Parameters.Add("pID_TIT", id);
            ocmd.Connection = base.CurrentConnection;

            int sottoTitoli=st.Count;
            int sottoTitoliAttivi = 0;
            decimal lastActive=-1;
            
            try
            {
                if(st.Count>1)
                {
                    IEnumerator<Titolo> en =st.GetEnumerator();
                    en.Reset();
                    while(en.MoveNext())
                    {
                        if(!en.Current.Deleted)
                        {
                            sottoTitoliAttivi++;
                            lastActive=en.Current.Id;
                        }
                    }
                    if (sottoTitoliAttivi > 1)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException("ATTENZIONE!!! Cancellare prima i sottotitoli associati questo titolo",
                            "DAL_TIT_0015", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
       
                        log.Error(mEx);
                        throw mEx;
                    }
                        //throw new ManagedException("ATTENZIONE!!! Cancellare prima i sottotitoli associati questo titolo", "DAL_TIT_0015","", "", "", "", "", null);
                    else if (sottoTitoliAttivi == 1)
                    {
                        this.DeleteLogic(id);
                        Context.DaoImpl.SottoTitoloDao.DeleteLogic(lastActive);
                    }
                }
                else if(st.Count ==1)
                {
                    //cancello fisicamente i record del titolo e del sottotitolo di default
                    //TODO:occorre aggiungere la parte relativa al controllo sulle richieste associate
                    //se ci sono la cancellazione torna ad essere logica
                    Context.DaoImpl.SottoTitoloDao.Delete(st[0].Id);
                    ocmd.ExecuteNonQuery();
                }   
                if(st.Count==0)
                {
                    if(ocmd.ExecuteNonQuery() !=1)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);

                        log.Error(mEx);
                        throw mEx;
                    }
                    //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);
                }
            }
            /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                                "DAL_UNIQUE_CODE", string.Empty,
                                string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                }
                else throw;
                //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
            }
        }
        
        public void DeleteLogic(decimal id)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.CommandText = deleteLogicStatement;
                ocmd.Parameters.Add("pID_TIT", id);
                ocmd.Connection = base.CurrentConnection;
                try
                {
                    int rowAff = ocmd.ExecuteNonQuery();
                    if (rowAff != 1)
                    {
                        //TASK: Allineamento log - Ciro
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                            "DAL_TIT_009", string.Empty,
                            string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
          
                        log.Error(mEx);
                        throw mEx;
                    }
                        //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_TIT_009", "", "", "", "", "", null);
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (InvalidOperationException ex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                        "DAL_UNIQUE_CODE", string.Empty,
                        string.Empty, null);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    if (System.Threading.Thread.CurrentContext.ContextID != null)
                        err.objectID = System.Threading.Thread.CurrentContext.ContextID.ToString();
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_UNIQUE_CODE", "", "", "", "", "", ex);
                }
            }
        }

        #endregion

        #region mapping

        private OracleParameter[] MapObjectToParams(Titolo r, bool isInsert)
        {
            OracleParameter[] oparams = new OracleParameter[4];

            oparams[0] = new OracleParameter("pAPPCODE", OracleDbType.Varchar2, 200, r.AppCode, ParameterDirection.Input);
            oparams[1] = new OracleParameter("pPROT_CODE", OracleDbType.Varchar2, 100, r.CodiceProtocollo, ParameterDirection.Input);
            oparams[2] = new OracleParameter("pDESCR", OracleDbType.Varchar2, 200, r.Note, ParameterDirection.Input);
            if (isInsert)
                oparams[3] = new OracleParameter("pID_TIT", OracleDbType.Decimal, r.Id, ParameterDirection.Output);
            else
                oparams[3] = new OracleParameter("pID_TIT", OracleDbType.Decimal, r.Id, ParameterDirection.Input);

            return oparams;
        }

        #endregion

  
    }
}
