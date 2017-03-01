using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.OracleDb;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.Wrappers;
using Oracle.DataAccess.Client;
using SendMail.Data.Utilities;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;

namespace SendMailApp.OracleCore.Oracle
{
    public class ReferralTypeDaoOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IReferralTypeDao
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReferralTypeDaoOracleDb));
        #region "C.tor"

        public ReferralTypeDaoOracleDb(OracleSessionManager daoContext)
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

        #region IDao<BaseResultItem,string> Membri di

        public ICollection<SendMail.Model.Wrappers.BaseResultItem> GetAll()
        {
            List<BaseResultItem> res = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = "SELECT * FROM RUBR_REFERRAL_TYPE";

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            res = new List<BaseResultItem>();

                            while (r.Read())
                            {
                                res.Add(DaoOracleDbHelper.MapReferralTypeToResultItem(r));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message,
                        "ERR_REFF001",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    throw mEx;
                }
                else throw;
                
            }

            return res;
        }

        public SendMail.Model.Wrappers.BaseResultItem GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Insert(SendMail.Model.Wrappers.BaseResultItem entity)
        {
            throw new NotImplementedException();
        }

        public void Update(SendMail.Model.Wrappers.BaseResultItem entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(string id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (!base.Context.TransactionModeOn)
            {
                base.CurrentConnection.Close();
                base.Context.Dispose();
            }
        }

        #endregion
    }
}
