using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using SendMail.Data.Utilities;

namespace SendMail.Data.OracleDb
{
    public class MailAddresseeOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IMailAddresseDao
    {
        #region "Private command strings"

        private const string cmdSelectMailAddressee = "SELECT * FROM v_rubrica_completa WHERE id_rub = :IdRubrica";

        #endregion

        #region "Class Methods"

        
        public MailAddresseeOracleDb(OracleSessionManager daoContext)
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

        #region IDao<MailAddressee,long> Membri di

        public ICollection<MailAddressee> GetAll()
        {
            throw new NotImplementedException();
        }

       

        public MailAddressee GetById(long id)
        {
            MailAddressee mailAddressee = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectMailAddressee;
                    oCmd.Parameters.Add("IdRubrica", id);
                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            mailAddressee = (MailAddressee)DataMapper.FillObject(r, typeof(MailAddressee));
                        }
                    }
                }
            }
            catch
            { }
            return mailAddressee;
        }

        public void Insert(MailAddressee entity)
        {
            throw new NotImplementedException();
        }

        public int Save(MailAddressee entity)
        {
            throw new NotImplementedException();
        }

        public void Update(MailAddressee entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
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
    }
}
