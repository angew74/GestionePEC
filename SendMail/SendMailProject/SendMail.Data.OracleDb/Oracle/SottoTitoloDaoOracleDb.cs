using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Com.Delta.Logging;
using System.Data;
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
    public class SottoTitoloDaoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, ISottoTitoloDao
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(SottoTitoloDaoOracleDb));

        private const string insertStatement = "INSERT INTO COMUNICAZIONI_SOTTOTITOLI"
                                            + " (REF_ID_TITOLO, SOTTOTITOLO, ACTIVE, NOTE, COM_CODE, PROT_MANAGED, PROT_SUBCODE, PROT_PWD, PROT_TIPI_AMMESSI, PROT_LOAD_ALLEGATI, PROT_CODE)"
                                            + " VALUES"
                                            + " (:pREF_ID_TITOLO, :pSOTTOTITOLO, :pACTIVE, :pNOTE, :pPROT_CODE, :pPROT_MANAGED, :pPROT_SUBCODE, :pPROT_PWD, :pPROT_TIPI_AMMESSI, :pPROT_LOAD_ALLEGATI, :pPROT_CODE)"
                                            + " RETURNING ID_SOTTOTITOLO INTO :pID_SOTTOTITOLO";


        private const string updateStatement = "UPDATE COMUNICAZIONI_SOTTOTITOLI SET"
                                            + " REF_ID_TITOLO := pREF_ID_TITOLO"
                                            + ", SOTTOTITOLO = :pSOTTOTITOLO"
                                            + ", ACTIVE = :pACTIVE"
                                            + ", NOTE = :pNOTE"
                                            + ", COM_CODE = :pCOM_CODE"
                                            + ", PROT_MANAGED = :pPROT_MANAGED"
                                            + ", PROT_SUBCODE = :pPROT_SUBCODE"
                                            + ", PROT_PWD = :pPROT_PWD"
                                            + ", PROT_TIPI_AMMESSI = :pPROT_TIPI_AMMESSI"
                                            + ", PROT_LOAD_ALLEGATI = :pPROT_LOAD_ALLEGATI"
                                            + ", PROT_CODE = :pPROT_CODE"
                                            + " WHERE (ID_SOTTOTITOLO = :pID_SOTTOTITOLO)";

        private const string deleteStatement = "DELETE FROM COMUNICAZIONI_SOTTOTITOLI WHERE"
                                            + " ID_SOTTOTITOLO = :pID_SOTTOTITOLO";

        private const string deleteLogicStatement = "UPDATE COMUNICAZIONI_SOTTOTITOLI SET"
                                               + "ACTIVE=0 WHERE (ID_SOTTOTITOLO=:pID_SOTTOTITOLO)";

      
        /// <summary>
        /// C.tor - restituisce il dao implementato
        /// </summary>
        /// <param name="daoContext">DaoOracleDbFactory : OracleSessionManager</param>
        public SottoTitoloDaoOracleDb(OracleSessionManager daoContext) 
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

        /// <summary>
        /// Dispose della classe.
        /// </summary>
        public void Dispose()
        {
            if (base.Context.TransactionModeOn == false)
                base.CurrentConnection.Close();
        }

        #region "ISottoTitoloDao members"

        /// <summary>
        /// Restituisce l'intera collezione degli oggetti SottoTitolo.
        /// </summary>
        /// <returns></returns>
        public ICollection<SottoTitolo> GetAll()
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                // preparo il command
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_SOTTOTITOLI";
                // eseguo il command
                try
                {
                    return DaoOracleDbHelper<SottoTitolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToSottoTitolo);
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ 
                catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO,
                        "DAL_SOT_001", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO, "DAL_SOT_001", "", "", "", "", "", oex);
                }
            }
        }

        /// <summary>
        /// Restituisce la collezione di oggetti SottoTitolo
        /// collegati al Titolo passato.
        /// </summary>
        /// <param name="titoloKey">identificativo del Titolo</param>
        /// <returns>una collection di oggetti SottoTitolo</returns>
        public ICollection<SottoTitolo> FindByTitolo(int titoloKey)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                // preparo il command
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_SOTTOTITOLI WHERE REF_ID_TITOLO = :pREF_ID_TIT ORDER BY ID_SOTTOTITOLO";
                ocmd.Parameters.Add("pREF_ID_TIT", titoloKey);
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    return DaoOracleDbHelper<SottoTitolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToSottoTitolo);
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ 
                catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO,
                        "DAL_SOT_002", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
      
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO, "DAL_SOT_002", "", "", "", "", "", oex);
                }
            }
        }

        #endregion

        #region ISottoTitoloDao Membri di

      
        public SottoTitolo GetSottoTitoloById(decimal id)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_SOTTOTITOLI WHERE ID_SOTTOTITOLO = :pID";
                ocmd.Parameters.Add("pID", id);

                // eseguo il command
                try
                {
                    ICollection<SottoTitolo> find = DaoOracleDbHelper<SottoTitolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToSottoTitolo);
                    if (find.Count > 0)
                        return find.First();
                    else
                        return null;
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ 
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

        #endregion

        #region ISottoTitoloDao Membri di

        /// <summary>
        /// Restituisce la collezione di oggetti SottoTitolo
        /// collegati al Titolo passato.
        /// </summary>
        /// <param name="tkey">identificativo del Titolo</param>
        /// <returns>una collection di oggetti SottoTitolo</returns>          
        public ICollection<SottoTitolo> FindByTitolo(decimal tkey)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                // preparo il command
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_SOTTOTITOLI WHERE REF_ID_TITOLO = :pREF_ID_TIT ORDER BY ID_SOTTOTITOLO";
                ocmd.Parameters.Add("pREF_ID_TIT", tkey);
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    return DaoOracleDbHelper<SottoTitolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToSottoTitolo);
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ 
                catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO,
                        "DAL_SOT_002", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
              
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.SOTTOTITOLO_NON_RECUPERATO, "DAL_SOT_002", "", "", "", "", "", oex);
                }
            }
        }

        public SottoTitolo GetSottoTitoloByComCode(string comcode)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_SOTTOTITOLI WHERE COM_CODE = :pComCode";
                ocmd.Parameters.Add("pComCode", comcode);

                // eseguo il command
                try
                {
                    ICollection<SottoTitolo> find = DaoOracleDbHelper<SottoTitolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToSottoTitolo);
                    if (find.Count > 0)
                        return find.First();
                    else
                        return null;
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (OracleException oex)
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO,
                        "DAL_TIT_011", string.Empty,
                        string.Empty, oex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.TITOLO_NON_RECUPERATO, "DAL_TIT_011", "", "", "", "", "", oex);
                }
            }
        }

        public SottoTitolo GetById(decimal id)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                ocmd.Connection = base.CurrentConnection;
                ocmd.CommandText = "SELECT * FROM COMUNICAZIONI_SOTTOTITOLI WHERE ID_SOTTOTITOLO = :pID";
                ocmd.Parameters.Add("pID", id);

                // eseguo il command
                try
                {
                    ICollection<SottoTitolo> find = DaoOracleDbHelper<SottoTitolo>.ExecSelectCommand(ocmd, DaoOracleDbHelper.MapToSottoTitolo);
                    if (find.Count > 0)
                        return find.First();
                    else
                        return null;
                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (OracleException oex)
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

        public void Insert(SottoTitolo sottoTitolo)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {
                // preparo il command
                ocmd.CommandText = insertStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(sottoTitolo, true));
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    ocmd.ExecuteNonQuery();
                    //param out
                    int iNewID = default(int);
                    int.TryParse(ocmd.Parameters["pID_SOT"].Value.ToString(), out iNewID);
                    //todo.. MIGLIORARE
                    if (iNewID != default(int))
                    {
                        sottoTitolo.Id = iNewID;
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
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (OracleException oex)
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

        public void Update(SottoTitolo sottoTitolo)
        {
            using (OracleCommand ocmd = new OracleCommand())
            {

                ocmd.CommandText = updateStatement;
                ocmd.Parameters.AddRange(MapObjectToParams(sottoTitolo, false));
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    int rowAff = ocmd.ExecuteNonQuery();
                    
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

        public void Delete(decimal id)
        {
            OracleCommand ocmd = new OracleCommand();
            ocmd.CommandText = deleteStatement;
            ocmd.Parameters.Add("pID_SOT", id);
            ocmd.Connection = base.CurrentConnection;
            try
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
                ocmd.Parameters.Add("pID_SOT", id);
                ocmd.Connection = base.CurrentConnection;
                // eseguo il command
                try
                {
                    int rowAff = ocmd.ExecuteNonQuery();
                    //todo.. MIGLIORARE
                    if (rowAff != 1)
                    {
                        ManagedException mEx = new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA,
                                               "DAL_SOT_009", string.Empty,
                                               string.Empty, null);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        log.Error(mEx);
                        throw mEx;
                    }
                        //throw new ManagedException(DalExMessages.NESSUNA_RIGA_MODIFICATA, "DAL_SOT_009", "", "", "", "", "", null);

                }
                /*TODO: INSERIRE I LOG DELLE ECCEZIONI*/ catch (InvalidOperationException ex)
                {

                    //throw new DALException(DalExMessages.RUBRICA_NON_AGGIORNATA, ex);
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA,
                        "DAL_SOT_034", string.Empty,
                        string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
             
                    log.Error(mEx);
                    throw mEx;
                    //throw new ManagedException(DalExMessages.RUBRICA_NON_AGGIORNATA, "DAL_SOT_034", "", "", "", "", "", ex);
                }
            }
        }

      
        #endregion


        #region mapping

        private OracleParameter[] MapObjectToParams(SottoTitolo r, bool isInsert)
        {
            OracleParameter[] oparams = new OracleParameter[12];
            oparams[0] = new OracleParameter("pREF_ID_TITOLO", OracleDbType.Decimal, r.RefIdTitolo, ParameterDirection.Input);
            oparams[1] = new OracleParameter("pSOTTOTITOLO", OracleDbType.Varchar2, r.Nome, ParameterDirection.Input);
            oparams[2] = new OracleParameter("pACTIVE", OracleDbType.Decimal, Convert.ToDecimal(!r.Deleted), ParameterDirection.Input);
            oparams[3] = new OracleParameter("pNOTE", OracleDbType.Varchar2, r.Note, ParameterDirection.Input);
            oparams[4] = new OracleParameter("pCOM_CODE", OracleDbType.Varchar2, r.ComCode, ParameterDirection.Input);
            oparams[5] = new OracleParameter("pPROT_MANAGED", OracleDbType.Decimal, Convert.ToDecimal(r.UsaProtocollo), ParameterDirection.Input);
            oparams[6] = new OracleParameter("pPROT_SUBCODE", OracleDbType.Varchar2, r.ProtocolloSubCode, ParameterDirection.Input);
            oparams[7] = new OracleParameter("pPROT_PWD", OracleDbType.Varchar2, r.ProtocolloPassword, ParameterDirection.Input);
            oparams[8] = new OracleParameter("pPROT_TIPI_AMMESSI", OracleDbType.Varchar2, ParameterDirection.Input);
            oparams[8].Value = String.Join(";", r.TipiProcollo.Cast<string>().ToArray());
            oparams[9] = new OracleParameter("pPROT_LOAD_ALLEGATI", OracleDbType.Decimal, Convert.ToDecimal(r.ProtocolloLoadAllegati), ParameterDirection.Input);
            oparams[10] = new OracleParameter("pPROT_CODE", OracleDbType.Varchar2, r.ProtocolloCode, ParameterDirection.Input);
            
            if (isInsert)
                oparams[11] = new OracleParameter("pID_SOTTOTITOLO", OracleDbType.Decimal, r.Id, ParameterDirection.Output);
            else
                oparams[11] = new OracleParameter("pID_SOTTOTITOLO", OracleDbType.Decimal, r.Id, ParameterDirection.Input);

            return oparams;
        }

        #endregion

    }
}
