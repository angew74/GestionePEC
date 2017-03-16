using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using Oracle.DataAccess.Client;
using System.Data;
using SendMail.Model;
using SendMail.Data.Utilities;
using SendMail.Model.Wrappers;
using ActiveUp.Net.Mail.DeltaExt;
using Oracle.DataAccess.Types;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMail.Data.OracleDb
{
    class BackEndCodeOracleDb: Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IBackEndCodeDao 
    {
        #region statements

        private const string insertStatement = "INSERT INTO RUBR_BACKEND"
                                            + " (ID_BACKEND, BACKEND_CODE, BACKEND_DESCR, CATEGORY)"
                                            + " VALUES"
                                            + " (RUBRICA_SEQ.nextval, :pID, :pCODE, :pDESCR, :pCAT)"
                                            + " RETURNING ID_BACKEND INTO :pID";

        private const string updateStatement = "UPDATE RUBR_BACKEND SET"
                                            + "  ID_BACKEND = :pID, BACKEND_CODE = :pCODE"
                                            + ", BACKEND_DESCR = :pDESCR, CATEGORY = :pCAT"
                                            + " WHERE (ID_BACKEND = :pID)";

        private const string deleteStatement = "DELETE FROM RUBR_BACKEND  WHERE"
                                            + " ID_BACKEND = :pID";

        private const string selectByID = "SELECT * FROM RUBR_BACKEND WHERE ID_BACKEND = :pId";

        private const string selectAll = "SELECT * FROM RUBR_BACKEND";

        #endregion

        private static readonly ILog log = LogManager.GetLogger(typeof(BackEndCodeOracleDb));

        public BackEndCodeOracleDb(OracleSessionManager daoContext) 
            : base(daoContext)
        {
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


        #region IDao<BackEndRefCode,long> Membri di

        public ICollection<SendMail.Model.BackEndRefCode> GetAll()
        {
            List<BackEndRefCode> entityList = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {

                    oCmd.CommandText = selectAll;
                    return DaoOracleDbHelper<BackEndRefCode>.ExecSelectCommand(oCmd, DaoOracleDbHelper.MapToBackEndRefCode);
  
                }
            }
                //TODO:Exception Handling
            catch (Exception e0)
            {
                //Allineamento log - Ciro
                if (e0.GetType() != typeof(ManagedException))
                {
                    ManagedException me = new ManagedException(e0.Message, "ORA_ERR001", string.Empty, string.Empty, e0);
                    ErrorLogInfo err = new ErrorLogInfo(me);
                    log.Error(err);
                }
                return null;
            }
        }

        public SendMail.Model.BackEndRefCode GetById(decimal id)
        {
            BackEndRefCode entity = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {

                    oCmd.CommandText = selectByID;
                    oCmd.Parameters.Add("pId", id);
                    ICollection<BackEndRefCode> find = DaoOracleDbHelper<BackEndRefCode>.ExecSelectCommand(oCmd, DaoOracleDbHelper.MapToBackEndRefCode);
                    if (find.Count > 0)
                        return find.First();
                    else return null;
   
                }
            }
            //TODO:Exception Handling
            catch (Exception e0)
            {
                //Allineamento log - Ciro
                if (e0.GetType() != typeof(ManagedException))
                {
                    ManagedException me = new ManagedException(e0.Message, "ORA_ERR002", string.Empty, string.Empty, e0);
                    ErrorLogInfo err = new ErrorLogInfo(me);
                    log.Error(err);
                }
                return null;
            }
        }

        public void Insert(SendMail.Model.BackEndRefCode entity)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder("INSERT INTO rubr_backend")
                        .Append(" (backend_code, backend_descr, category, descr_plus)")
                        .Append(" VALUES")
                        .Append(" (:p_code, :p_descr, :p_cat, :p_descrp)")
                        .Append(" RETURNING id_backend INTO :p_id");
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_code",
                        Size = 15,
                        Value = entity.Codice
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_descr",
                        Size = 100,
                        Value = entity.Descrizione
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.NVarchar2,
                        ParameterName = "p_cat",
                        Size = 40,
                        Value = entity.Categoria
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_descrp",
                        Size = 10,
                        Value = entity.DescrizionePlus
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Output,
                        OracleDbType = OracleDbType.Decimal,
                        ParameterName = "p_id",
                        Precision = 10,
                        Scale = 0
                    });
                    int risp = cmd.ExecuteNonQuery();
                    if (risp != 1)
                        throw new InvalidOperationException("Dato non inserito");
                    entity.Id = ((OracleDecimal)cmd.Parameters["p_id"].Value).Value;
                }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ERR_ORADB111",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                else throw;
            }
        }

        public void Update(SendMail.Model.BackEndRefCode entity)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder("UPDATE rubr_backend SET")
                        .Append(" backend_code = :p_code, backend_descr = :p_descr")
                        .Append(", category = :p_cat, descr_plus = :p_descrp")
                        .Append(" WHERE id_backend = :p_id");
                    cmd.CommandText = sb.ToString();
                    cmd.BindByName = true;
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_code",
                        Size = 15,
                        Value = entity.Codice
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_descr",
                        Size = 100,
                        Value = entity.Descrizione
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.NVarchar2,
                        ParameterName = "p_cat",
                        Size = 40,
                        Value = entity.Categoria
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_descrp",
                        Size = 10,
                        Value = entity.DescrizionePlus
                    });
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Decimal,
                        ParameterName = "p_id",
                        Precision = 10,
                        Scale = 0,
                        Value = entity.Id
                    });
                    int risp = cmd.ExecuteNonQuery();
                    if (risp != 1)
                        throw new InvalidOperationException("Oggetto non aggiornato");
                }
            }
            catch
            {
                throw;
            }
        }

        public void Delete(decimal id)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder("DELETE FROM rubr_backend  WHERE")
                        .Append(" id_backend = :p_id");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Decimal,
                        ParameterName = "p_id",
                        Precision = 10,
                        Scale = 0,
                        Value = id
                    });
                    int resp = cmd.ExecuteNonQuery();
                    if (resp != 1)
                        throw new InvalidOperationException("Oggetto non cancellato");
                }
            }
            catch (Exception ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORA_ERR006", string.Empty, string.Empty, ex);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    log.Error(err);
                    throw mEx;
                }
                else throw ex;
            }
        }

        #endregion

        #region IBackEndCodeDao Membri di

        public BackEndRefCode GetByCode(string backendCode)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder("SELECT id_backend")
                        .Append(", backend_code")
                        .Append(", backend_descr")
                        .Append(", category")
                        .Append(", descr_plus")
                        .Append(" FROM rubr_backend")
                        .Append(" WHERE backend_code = :p_backend_code");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_backend_code",
                        Size = 15,
                        Value = backendCode
                    });
                    return DaoOracleDbHelper<BackEndRefCode>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToBackEndRefCode).SingleOrDefault();
                }
            }
            catch
            {
                return null;
            }
        }

        public List<BackEndRefCode> GetByDescr(string backendDescr)
        {
            try
            {
                using (OracleCommand cmd = CurrentConnection.CreateCommand())
                {
                    StringBuilder sb = new StringBuilder("SELECT id_backend")
                        .Append(", backend_code")
                        .Append(", backend_descr")
                        .Append(", category")
                        .Append(", descr_plus")
                        .Append(" FROM rubr_backend")
                        .Append(" WHERE UPPER(backend_descr) LIKE :p_backend_descr");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.Add(new OracleParameter
                    {
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        ParameterName = "p_backend_descr",
                        Value = string.Format("%{0}%", backendDescr.ToUpper())
                    });
                    return (List<BackEndRefCode>)DaoOracleDbHelper<BackEndRefCode>.ExecSelectCommand(cmd,
                        DaoOracleDbHelper.MapToBackEndRefCode);
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region mapping

        private OracleParameter[] MapObjectToParams(BackEndRefCode r, bool isInsert)
        {
            OracleParameter[] oparams = new OracleParameter[4];

            oparams[0] = new OracleParameter("pCODE", OracleDbType.Varchar2, 15, r.Codice, ParameterDirection.Input);
            oparams[1] = new OracleParameter("pDESCR", OracleDbType.Varchar2, 100, r.Descrizione, ParameterDirection.Input);
            oparams[2] = new OracleParameter("pCAT", OracleDbType.Varchar2, 40, r.Categoria, ParameterDirection.Input);
            
            if (isInsert)
                oparams[3] = new OracleParameter("pID", OracleDbType.Decimal, r.Id, ParameterDirection.Output);
            else
                oparams[3] = new OracleParameter("pID", OracleDbType.Decimal, r.Id, ParameterDirection.Input);

            return oparams;
        }

        #endregion
    }
   
}
