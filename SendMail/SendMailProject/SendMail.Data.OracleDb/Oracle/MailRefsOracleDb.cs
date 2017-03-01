using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using SendMail.Data.Utilities;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using log4net;

namespace SendMail.Data.OracleDb
{
    public class MailRefsOracleDb : Com.Delta.Data.Oracle10.OracleDao<OracleSessionManager, ISessionModel>, IMailRefsDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailRefsOracleDb));

        #region "Private command strings"
        private const string tabColumns = "ID_REF, REF_ID_MAIL, MAIL_DESTINATARIO, TIPO_REF";
        private const string cmdSelectMailRefsByIdMail = "SELECT " + tabColumns + " FROM mail_refs_new WHERE id_mail = :IdMail";
        private const string cmdInsertMailRefs = "INSERT INTO mail_refs_new (REF_ID_MAIL, MAIL_DESTINATARIO, TIPO_REF)" +
            " VALUES (:MAILID, :DEST, :TIPO) ";
        #endregion

        #region "Class Methods"


        public MailRefsOracleDb(OracleSessionManager daoContext)
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

        #region IMailRefs Membri di

        public ICollection<MailRefs> GetMailRefsOfAMail(long idMail)
        {
            List<MailRefs> mailRefsList = null;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdSelectMailRefsByIdMail;
                    oCmd.Parameters.Add("IdMail", idMail);

                    using (OracleDataReader r = oCmd.ExecuteReader())
                    {
                        if (r.HasRows)
                        {
                            mailRefsList = new List<MailRefs>();
                            while (r.Read())
                            {
                                mailRefsList.Add((MailRefs)DataMapper.FillObject(r, typeof(MailRefs)));
                            }
                        }
                    }
                }
            }
            catch
            {
                mailRefsList = null;
            }
            return mailRefsList;
        }

        #endregion

        #region IDao<MailRefs,long> Membri di

        public ICollection<MailRefs> GetAll()
        {
            throw new NotImplementedException();
        }

        public MailRefs GetById(long id)
        {
            throw new NotImplementedException();
        }

        public void Insert(MailRefs entity)
        {
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdInsertMailRefs;
                    oCmd.Parameters.Add("MAILID", entity.RefIdMail);
                    oCmd.Parameters.Add("DEST", entity.AddresseeMail);
                    oCmd.Parameters.Add("TIPO", entity.AddresseeClass.ToString());
                    oCmd.BindByName = true;
                    oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_INS001", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = entity.IdRef.ToString();
                    log.Error(er);
                    throw mEx;
                }
                else
                    throw ex;
            }
        }

        public void Update(MailRefs entity)
        {
            throw new NotImplementedException();
        }

        public int Save(MailRefs entity)
        {
            int ins = 0;
            try
            {
                using (OracleCommand oCmd = base.CurrentConnection.CreateCommand())
                {
                    oCmd.CommandText = cmdInsertMailRefs;
                    oCmd.Parameters.Add("MAILID", entity.RefIdMail);
                    oCmd.Parameters.Add("DEST", entity.AddresseeMail);
                    oCmd.Parameters.Add("TIPO", entity.AddresseeClass.ToString());
                    oCmd.BindByName = true;
                    ins= oCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //Allineamento log - Ciro
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(ex.Message, "ERR_INS002", string.Empty, string.Empty, ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = entity.IdRef.ToString();
                    log.Error(er);
                    throw mEx;
                }
                else
                    throw ex;
            }
            return ins;
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
