using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Oracle.DataAccess.Client;
using Com.Delta.Logging;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using SendMail.Data.Utilities;
using SendMail.Model.RubricaMapping;
using SendMail.Model.Wrappers;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.OracleDb
{
    public class RubricaOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IRubricaDao
    {
      private static readonly ILog log = LogManager.GetLogger("RubricaOracleDb");
        #region "Private members"

        private const string cmdSelectIdRubrica = "SELECT * from RUBRICA WHERE " +
          " ID_RUB = :RUBID  ";
  

        #endregion

        #region "Class Methods"


      public RubricaOracleDb(OracleSessionManager daoContext)
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
        private OracleSessionManager context;
        #endregion
        #region IDao<Rubrica> Membri di 
       
        public Rubrica GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(Rubrica dao)
        {
            throw new NotImplementedException();
        }

        public void Delete(long Id)
        {
            throw new NotImplementedException();
        }

        public void Update(Rubrica dao)
        {
            throw new NotImplementedException();
        }

        public int Save(Rubrica dao)
        {
            throw new NotImplementedException();
        }

        public ICollection<Rubrica> GetAll()
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

        #region IRuricaDao Membri di

        public Rubrica GetRubricaItem(Int64 idrub)
        {
            Rubrica dao = new Rubrica();

            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {

                    oCmd.CommandText = cmdSelectIdRubrica;
                    oCmd.Parameters.Add("RUBID", idrub);
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        dao = (Rubrica)DataMapper.FillObject(r, typeof(Rubrica));
                    }

                }
            }
            //catch (OracleException ex)
            //{
            //    Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
            //    error.freeTextDetails = "Errore nel caricamento dal database di un elemento della rubrica Data Layer E035 Dettagli Errore: " + ex.Message;
            //    error.logCode = "ERR_035";              
            //    error.passiveparentcodeobjectID = string.Empty;
            //    error.passiveobjectGroupID = string.Empty;
            //    error.passiveobjectID = string.Empty;
            //    error.passiveapplicationID = string.Empty;
            //    log.Error(error);
            //    throw new ManagedException(ex.Message, "E035", "Delta.CdR.INAData.OracleImpl", "GetRubricaItem", "Caricamento dal database di un elemento della rubrica Data Layer", string.Empty, "Eccezione non Gestita", ex);
            //}
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Errore nel caricamento dal database di un elemento della rubrica Data Layer E035 Dettagli Errore: " + ex.Message,
                        "ERR_035", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
            
                    //err.objectID = string.Empty;
                    //err.userID = System.DateTime.Now.Date.ToString() + " - " + System.DateTime.Now.Ticks.ToString();
                    log.Error(err);
                    throw mEx;
                }
                else
                    throw ex;
                //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //error.freeTextDetails = "Errore nel caricamento dal database di un elemento della rubrica Data Layer E035 Dettagli Errore: " + ex.Message;
                //error.logCode = "ERR_035";              
                //error.passiveparentcodeobjectID = string.Empty;
                //error.passiveobjectGroupID = string.Empty;
                //error.passiveobjectID = string.Empty;
                //error.passiveapplicationID = string.Empty;
                //log.Error(error);
                //throw new ManagedException(ex.Message, "E035", "Delta.CdR.INAData.OracleImpl", "GetRubricaItem", "Caricamento dal database di un elemento della rubrica Data Layer", string.Empty, "Eccezione non Gestita", ex);
            }

            return dao;
        }

        public ICollection<Rubrica> GetRubricaItems(string[] codici)
        {
            ICollection<Rubrica> list = null;
            for (int i = 0; i < codici.Length; i++)
            {
                try
                {
                    Rubrica dao = new Rubrica();
                    string idrub = codici[i];
                    using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                    {

                        oCmd.CommandText = PrepareQueryStatement(codici); 
                        using (OracleDataReader r = oCmd.ExecuteReader())
                        {
                           
                             list = (DaoOracleDbHelper<Rubrica>.ExecSelectCommand(oCmd, DaoOracleDbHelper.MapToRubrica) as ICollection<Rubrica>);
                            
                        }

                    } 
                }
                //catch (OracleException ex)
                //{
                //    Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //    error.freeTextDetails = "Errore nel caricamento dal database della rubrica Data Layer E034 Dettagli Errore: " + ex.Message;
                //    error.logCode = "ERR_034";                  
                //    error.passiveparentcodeobjectID = string.Empty;
                //    error.passiveobjectGroupID = string.Empty;
                //    error.passiveobjectID = string.Empty;
                //    error.passiveapplicationID = string.Empty;
                //    log.Error(error);
                //    throw new ManagedException(ex.Message, "E034", "Delta.CdR.INAData.OracleImpl", "GetRubricaItems", "Caricamento degli elementi della rubrica Data Layer", string.Empty, "Eccezione non Gestita", ex);
                //}
                catch (Exception ex)
                {
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento dal database degli elementi della rubrica Data Layer E034 Dettagli Errore: " + ex.Message,
                            "ERR_034", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        
                        //err.objectID = string.Empty;
                        //err.userID = System.DateTime.Now.Date.ToString() + " - " + System.DateTime.Now.Ticks.ToString();
                        log.Error(err);
                        throw mEx;
                    }
                    else
                        throw ex;
                    //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                    //error.freeTextDetails = "Errore nel caricamento dal database degli elementi della rubrica Data Layer E034 Dettagli Errore: " + ex.Message;
                    //error.logCode = "ERR_034";                
                    //error.passiveparentcodeobjectID = string.Empty;
                    //error.passiveobjectGroupID = string.Empty;
                    //error.passiveobjectID = string.Empty;
                    //error.passiveapplicationID = string.Empty;
                    //log.Error(error);
                    //throw new ManagedException(ex.Message, "E034", "Delta.CdR.INAData.OracleImpl", "GetRubricaItems", "Caricamento dal database degli elementi della rubrica Data Layer", string.Empty, "Eccezione non Gestita", ex);
                }
            }
            return list;
        }

        public Dictionary<string, SimpleTreeItem> LoadTree(Int64? startNode, SendMail.Model.IndexedCatalogs catalog, int? levels)
        {
            Dictionary<string, SimpleTreeItem> list = null;
                StringBuilder query = new StringBuilder();
                try
                {
                    switch (catalog)
                    {
                        case IndexedCatalogs.RUBR:
                            query.Append("select ID_REFERRAL AS VALUE, ");
                                query.Append("CASE WHEN REFERRAL_TYPE in ('PA','AZ_PRI','AZ_CP','GRP') then RAGIONE_SOCIALE else UFFICIO end as TEXT, ");
                                query.Append("REFERRAL_TYPE AS SUBTYPE, ");
                                
                                query.Append("'RUBR' AS SOURCE, ");
                                query.Append("ID_PADRE AS PADRE ");
                                query.Append("from rubr_entita ");
                                query.Append("where REFERRAL_TYPE in ('PA','PA_SUB','PA_UFF','AZ_PRI','AZ_CP','AZ_UFF','GRP') ");
                                if (levels.HasValue) query.Append("and level <=:pLVL ");
                                if (startNode.HasValue) query.Append("start with ID_REFERRAL = :pID ");
                                else query.Append("start with ID_REFERRAL = 0 ");
                                query.Append("connect by NOCYCLE prior ID_REFERRAL = ID_PADRE ");
                        break;
                        case IndexedCatalogs.IPA:
                            query.Append("select ID_RUB AS VALUE, ");
                            query.Append("'IPA' AS SOURCE, ");
                            
                            query.Append("CASE WHEN ( substr(DN,1,2)= 'c=' or substr(DN,1,2) = 'o=') then RAGIONESOCIALE ");
                            query.Append("else UFFICIO end AS TEXT, ");
                            query.Append("CASE WHEN substr(DN,1,2)= 'c=' then 'GRP' ");
                            query.Append("WHEN substr(DN,1,2)= 'o=' then 'PA' ");
                            query.Append("else 'PA_UFF' end AS SUBTYPE, ");
                            query.Append("ID_PADRE AS PADRE ");
                            query.Append("from IPA ");
                            if (levels.HasValue) query.Append("where level <=:pLVL ");
                            if (startNode.HasValue) query.Append("start with ID_RUB = :pID ");
                            else query.Append("start with ID_RUB = 1 ");
                            query.Append("connect by NOCYCLE prior ID_RUB = ID_PADRE");

                        break;
                    }
                    
                    using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                    {
                        oCmd.CommandText = query.ToString();
                        oCmd.BindByName = true;
                        if (startNode.HasValue) oCmd.Parameters.Add("pID", startNode.Value);
                        if (levels.HasValue) oCmd.Parameters.Add("pLVL", levels.Value);
                        using (OracleDataReader r = oCmd.ExecuteReader())
                            if (r.HasRows)
                            {
                                list = new Dictionary<string, SimpleTreeItem>();
                                bool skip = false;
                                while (r.Read())
                                {
                                    if (!skip)
                                    {
                                        SimpleTreeItem it = DaoOracleDbHelper.MapToSimpleTreeItem(r);
                                        list.Add(it.ExtendedValue, it);
                                    }
                                }
                            }
                    }
                }
                //catch (OracleException ex)
                //{
                //    Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                //    error.freeTextDetails = "Errore nel caricamento dell'albero Data Layer E033 Dettagli Errore: " + ex.Message;
                //    error.logCode = "ERR_033";                   
                //    error.passiveparentcodeobjectID = string.Empty;
                //    error.passiveobjectGroupID = string.Empty;
                //    error.passiveobjectID = string.Empty;
                //    error.passiveapplicationID = string.Empty;
                //    log.Error(error);
                //    throw new ManagedException(ex.Message, "E033", "Delta.CdR.INAData.OracleImpl", "LoadTree", "Caricamento dell'albero ", string.Empty, "Eccezione non Gestita", ex);
                //}
                catch (Exception ex)
                {
                    //TASK: Allineamento log - Ciro
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento dal database dell'albero Data Layer E035 Dettagli Errore: " + ex.Message,
                            "ERR_035", string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
              
                        log.Error(err);
                        throw mEx;
                    }
                    else
                        throw ex;
                    //Com.Delta.Logging.Errors.ErrorLogInfo error = new Com.Delta.Logging.Errors.ErrorLogInfo();
                    //error.freeTextDetails = "Errore nel caricamento dal database dell'albero Data Layer E034 Dettagli Errore: " + ex.Message;
                    //error.logCode = "ERR_034";                  
                    //error.passiveparentcodeobjectID = string.Empty;
                    //error.passiveobjectGroupID = string.Empty;
                    //error.passiveobjectID = string.Empty;
                    //error.passiveapplicationID = string.Empty;
                    //log.Error(error);
                    //throw new ManagedException(ex.Message, "E034", "Delta.CdR.INAData.OracleImpl", "LoadTree", "Caricamento dal database dell'albero Data Layer", string.Empty, "Eccezione non Gestita", ex);
                }
            return list;
        }

        private string PrepareQueryStatement(string[] codici)
        {
            StringBuilder query = new StringBuilder();
            query.Append(" SELECT * from RUBRICA WHERE  ");
            query.Append("ID_RUB = " + Int64.Parse(codici[0]));
            for(int i=1;i<codici.Length;i++)
            {
                query.Append(" or ID_RUB = " + Int64.Parse(codici[i]));
           }
            return query.ToString();
        }

            
        #endregion


    }
}
