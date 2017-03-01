using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Delta.Data;
using System.Data.SqlClient;

namespace Com.Delta.Data
{
    public class SqlServerDao<T, TI> where T : IDaoBaseSession<TI>
    {
        private SqlServerDaoSession<T,TI> context;

        public SqlServerDao(SqlServerDaoSession<T, TI> context)
        {
            this.context = context;
        }

        public SqlServerDaoSession<T, TI> Context
        {
            get { return context; }
        }
        // //////////////////////////// metodi di utility specifici della connessione ad oracle ///////////////////// //
        protected SqlDataAdapter prepareSelectAdapter()
        {
            SqlDataAdapter adapt = new SqlDataAdapter();
            SqlCommand comm = new SqlCommand();
            comm.Connection = context.CurrentConnection;
            adapt.SelectCommand = comm;
            return adapt;
        }

        protected SqlConnection CurrentConnection
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
