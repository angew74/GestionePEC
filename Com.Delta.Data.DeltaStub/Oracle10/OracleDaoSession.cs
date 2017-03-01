using System;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess;
using log4net;

namespace Com.Delta.Data.Oracle10
{
    public class OracleDaoSession<T, TI> : Com.Delta.Data.IDaoBaseSession<TI> where T : IDaoBaseSession<TI>
    {
        private static readonly ILog log = LogManager.GetLogger("OracleDaoSession");

        internal Oracle.DataAccess.Client.OracleConnection CurrentConnection;

        protected TI Daos;
        protected Oracle.DataAccess.Client.OracleTransaction currentTransaction;
        private Type transRoot;

        private String _ConnectionName;
        internal String ConnectionName
        {
            get
            {
                if (String.IsNullOrEmpty(_ConnectionName))
                {
                    throw new System.Configuration.ConfigurationErrorsException("Errore nella configurazione del DaoAssembly");
                }
                return _ConnectionName;
            }
        }

        internal int SessionNumber { get; set; }

        protected OracleDaoSession()
        {
            Session_Init();
        }

        protected OracleDaoSession(string connName)
        {
            this._ConnectionName = connName;
            Session_Init();
        }

        #region IDaoBaseSession<T> Members

        public void Session_Init()
        {
            if (this.CurrentConnection == null)
            {
                var connStringSettings = System.Configuration.ConfigurationManager.ConnectionStrings[ConnectionName];
                if (connStringSettings == null)
                    throw new System.Configuration.ConfigurationErrorsException("Errore nel file di configurazione; manca la stringa di connessione " + ConnectionName);
                this.CurrentConnection = new Oracle.DataAccess.Client.OracleConnection();
                this.CurrentConnection.ConnectionString = connStringSettings.ConnectionString;
            }
        }

        public bool Session_isActive()
        {
            if (CurrentConnection == null || (CurrentConnection != null && CurrentConnection.State == System.Data.ConnectionState.Closed)) return false;
            else return true;
        }

        public void StartTransaction(Type requestor)
        {
            if (transRoot != null)
                throw new Exception("Transazione in corso");
            if (this.Session_isActive() == false)
                this.CurrentConnection.Open();
            currentTransaction = this.CurrentConnection.BeginTransaction();
            transRoot = requestor;
        }

        public bool TransactionModeOn
        {
            get { if (transRoot == null) return false; else return true; }
        }

        public void EndTransaction(Type requestor)
        {
            if (requestor != transRoot) throw new Exception("Solo la classe Root della transazione può terminarala");
            this.currentTransaction.Commit();
            this.CurrentConnection.Close();
            this.transRoot = null;
        }

        public void RollBackTransaction(Type requestor)
        {
            if (requestor != transRoot) throw new Exception("Solo la classe Root della transazione può terminarala");
            this.currentTransaction.Rollback();
            this.CurrentConnection.Close();
            this.transRoot = null;
        }

        public virtual Type TransactionRootElement
        {
            get
            {
                return transRoot;
            }
        }

        public virtual TI DaoImpl
        {
            get { return Daos; }

        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (CurrentConnection != null)
            {
                if (CurrentConnection.State != System.Data.ConnectionState.Closed)
                {
                    try
                    {
                        CurrentConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        log.Error("Eccezione nella chiusura della connessione dettagli: " + ex.Message);
                    }
                }

                CurrentConnection.Dispose();
                CurrentConnection = null;
            }
        }

        #endregion
    }
}
