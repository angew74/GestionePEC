using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.OracleDb;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.RubricaMapping;
using Oracle.DataAccess.Client;
using SendMail.Data.Utilities;

namespace SendMailApp.OracleCore.Oracle.OrderedQuery
{
    internal static class OrderedTOracleDB
    {
        #region Query

        private const string QUERY = "SELECT * FROM (SELECT ROWNUM rn, t.* FROM ({0}) t WHERE ROWNUM <= {1}) WHERE rn >= {2}";

        #endregion

        #region Internal methods

        internal static string GetOrderedQuery(string queryBase, int da, int per)
        {
            int finoA = da + per - 1;
            return String.Format(QUERY, queryBase, finoA, da);
        }

        #endregion
    }
}
