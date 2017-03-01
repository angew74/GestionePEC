using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Delta.Data;

namespace Com.Delta.Data
{
    public class SqlServerDaoSession<T, TI> : Com.Delta.Data.IDaoBaseSession<TI> where T : IDaoBaseSession<TI>
    {
        //internal Oracle.DataAccess.Client.OracleConnection CurrentConnection;
        internal System.Data.SqlClient.SqlConnection CurrentConnection;

        protected TI Daos;

        private Type transRoot;

        protected SqlServerDaoSession()
        {
            Session_Init();
        }

        #region IDaoBaseSession<T> Members

        public void Session_Init()
        {
            if (this.CurrentConnection == null)
            {
                this.CurrentConnection = new System.Data.SqlClient.SqlConnection();
                this.CurrentConnection.ConnectionString = System.Configuration.ConfigurationSettings.AppSettings["OraConn"];
            }
        }
        public void Dispose()
        {
            if (CurrentConnection != null) CurrentConnection.Dispose();
        }
        public bool Session_isActive()
        {
            if (CurrentConnection == null || (CurrentConnection != null && CurrentConnection.State == System.Data.ConnectionState.Closed)) return false;
            else return true;
        }
        public void StartTransaction(Type requestor)
        {
            if (transRoot != null) throw new Exception("Transazione in corso");
            transRoot = requestor;
        }
        public bool TransactionModeOn
        {
            get { if (transRoot == null)return false; else return true; }
        }
        public void EndTransaction(Type requestor)
        {
            if (requestor != transRoot) throw new Exception("Solo la classe Root della transazione può terminarala");
            this.transRoot = null;
        }

        public void RollBackTransaction(Type requestor)
        {
            if (requestor != transRoot) throw new Exception("Solo la classe Root della transazione può terminarala");
            this.transRoot = null;
        }
        public Type TransactionRootElement
        {
            get
            {
                return transRoot;
            }
        }
        public TI DaoImpl
        {

            get { return Daos; }

        }

        #endregion
    }
}
