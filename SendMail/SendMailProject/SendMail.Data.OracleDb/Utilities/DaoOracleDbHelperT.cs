using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net;
using Oracle.DataAccess.Client;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMail.Data.Utilities
{
    /// <summary>
    /// Classe helper statica tipizzata per lo strato dao.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class DaoOracleDbHelper<T>
    {
        private static readonly ILog _log = LogManager.GetLogger("DaoOracleDbHelper");
        /// <summary>
        /// Delegate per la funzione di mapping dbrecord/object.
        /// </summary>
        /// <param name="dr">dbrecord da mappare</param>
        internal delegate T MapToObjectDel(IDataRecord dr);
        internal delegate T MapToObjectColDel(IDataRecord dr, List<string> columns);

        /// <summary>
        /// Esegue un comando select restituendo la collezione di oggetti corrispondenti.
        /// </summary>
        /// <param name="ocmd">comando select da eseguire</param>
        /// <param name="func">funzione mapping dbrecord/object da invocare</param>
        /// <param name="columns">lista delle colonne da mappare</param>
        /// <returns>una collezione di oggetti T</returns>
        internal static ICollection<T> ExecSelectCommand(OracleCommand ocmd, MapToObjectColDel func, List<string> columns)
        {
            try
            {
                using (OracleDataReader ordr = ocmd.ExecuteReader())
                {
                    ICollection<T> result = new List<T>();
                    if (ordr.HasRows)
                    {
                        while (ordr.Read())
                        {
                            result.Add(func.Invoke(ordr, columns));
                        }
                    }
                    return result;
                }
            }
            catch (OracleException ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORAUNI-001", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);

                    _log.Error(err);
                    throw mEx;
                }
                else
                    throw ex;
                //ManagedException mex = new ManagedException(ex.Message, "ORAUNI-001", "ExecSelectCommand", ex.Source, ex.InnerException);
                //ErrorLog errorlog = new ErrorLog(mex);
                //_log.Error(errorlog);
                //throw mex;
            }
        }

        /// <summary>
        /// Esegue un comando select restituendo la collezione di oggetti corrispondenti.
        /// </summary>
        /// <param name="ocmd">comando select da eseguire</param>
        /// <param name="func">funzione mapping dbrecord/object da invocare</param>
        /// <param name="columns">lista delle colonne da mappare</param>
        /// <returns>una collezione di oggetti T</returns>
        internal static IList<T> ExecSelectCommandList(OracleCommand ocmd, MapToObjectColDel func, List<string> columns)
        {

            IList<T> result = new List<T>();
            try
            {
                using (OracleDataReader ordr = ocmd.ExecuteReader())
                {
                    if (ordr.HasRows)
                    {
                        while (ordr.Read())
                        {
                            result.Add(func.Invoke(ordr, columns));
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORAUNI-002", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
     
                    _log.Error(err);
                    throw mEx;
                }
                else
                    throw ex;
                //ManagedException mex = new ManagedException(ex.Message, "ORAUNI-002", "ExecSelectCommandList", ex.Source, ex.InnerException);
                //ErrorLog errorlog = new ErrorLog(mex);
                //_log.Error(errorlog);
                //throw mex;
            }
            return result;
        }

        /// <summary>
        /// Esegue un comando select restituendo la collezione di oggetti corrispondenti.
        /// </summary>
        /// <param name="ocmd">comando select da eseguire</param>
        /// <param name="func">funzione mapping dbrecord/object da invocare</param>
        /// <returns>una collezione di oggetti T</returns>
        internal static ICollection<T> ExecSelectCommand(OracleCommand ocmd, MapToObjectDel func)
        {
            try
            {
                using (OracleDataReader ordr = ocmd.ExecuteReader())
                {
                    ICollection<T> result = new List<T>();
                    if (ordr.HasRows)
                    {
                        while (ordr.Read())
                        {
                            result.Add(func.Invoke(ordr));
                        }
                    }
                    return result;
                }
            }
            catch (OracleException ex)
            {
                //TASK: Allineamento log - Ciro
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ORAUNI-003", string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
           
                    _log.Error(err);
                    throw mEx;
                }
                else
                    throw ex;
                //ManagedException mex = new ManagedException(ex.Message, "ORAUNI-003", "ExecSelectCommand", ex.Source, ex.InnerException);
                //ErrorLog errorlog = new ErrorLog(mex);
                //_log.Error(errorlog);
                //throw mex;
            }
        }

    }
}
