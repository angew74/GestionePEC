using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;

namespace Com.Delta.Data.Oracle10
{
    public class OracleDao<T, TI> where T : IDaoBaseSession<TI>
    {
        private OracleDaoSession<T,TI> context;
        public OracleDao(OracleDaoSession<T,TI> context)
        {
            this.context = context;
        }
        public OracleDaoSession<T,TI> Context
        {
            get { return context; }
        }
        // //////////////////////////// metodi di utility specifici della connessione ad oracle ///////////////////// //
        protected OracleDataAdapter prepareSelectAdapter()
        {
            OracleDataAdapter adapt = new OracleDataAdapter();
            OracleCommand comm = new OracleCommand();
            comm.Connection = context.CurrentConnection;
            adapt.SelectCommand = comm;
            return adapt;
        }
        protected OracleConnection CurrentConnection
        {
            get { return context.CurrentConnection; }
        }       
        protected string BuildLikeStatement(IList<string> limiti)
        {
            if (limiti != null && limiti.Count > 0)
            {
                string likexpr = "AND(";
                foreach (string s in limiti)
                {
                    likexpr = likexpr + " \"ROLES\".PARENTCODE LIKE '" + s + "%' OR ";
                }
                return likexpr.Substring(0, likexpr.Length -3) + ")";
            }
            else return "";

        }
    }
}
