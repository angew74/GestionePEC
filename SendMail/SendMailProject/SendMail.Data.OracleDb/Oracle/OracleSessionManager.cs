using SendMail.DataContracts.Interfaces;
using SendMail.Model;
using System.Configuration;
using SendMailApp.OracleCore.Oracle;
using log4net;
using SendMail.Data.Contracts.Mail;
using SendMail.Data.Oracle;

namespace SendMail.Data.OracleDb
{
    /// <summary>
    /// SessionManager.
    /// </summary>
    public class OracleSessionManager : Com.Delta.Data.Oracle10.OracleDaoSession<OracleSessionManager, ISessionModel>, ISessionModel
    {
        //private readonly string connString = ConfigurationManager.ConnectionStrings["MailConnection"].ConnectionString;
        private static string connName = "MailConnection";
        /// <summary>
        /// C.tor
        /// </summary>
        public OracleSessionManager()
            : base(connName)
        {
            base.Daos = this;
        }

        public OracleSessionManager(string conn)
            : base(conn)
        {
            base.Daos = this;
        }

        #region ISessionModel Membri di

        //public IAllegatoDao AllegatoDao { get { return new AllegatoOracleDb(this); } }

        public IMailDao MailDao { get { return new MailOracleDb(this); } }

        public IMailAddresseDao MailAddresseeDao { get { return new MailAddresseeOracleDb(this); } }

        public IBackendUserDao BackendUserDao { get { return new BackendUserOracleDb(this); } }

        public IMailRefsDao MailRefsDao { get { return new MailRefsOracleDb(this); } }

        public IRubricaDao RubricaDao { get { return new RubricaOracleDb(this); } }

        public ITitoloDao TitoloDao { get { return new TitoloDaoOracleDb(this); } }

        public ISottoTitoloDao SottoTitoloDao { get { return new SottoTitoloDaoOracleDb(this); } }

        //public INominativoRubricaDao NominativoRubricaDao { get { return new RubricaDaoOracleDb(this); } }

        public IBackEndCodeDao BackEndCodeDao { get { return new BackEndCodeOracleDb(this); } }

        public IContactsApplicationMappingDao ContactsApplicationMappingDao { get { return new ContactsApplicationMappingOracleDb(this); } }

        public IContattoDao ContattoDao { get { return new ContattoOracleDb(this); } }

        public IRubricaEntitaDao RubricaEntitaDao { get { return new RubricaEntitaOracleDb(this); } }

        public IComunicazioniDao ComunicazioniDao { get { return new ComunicazioniOracleDb(this); } }

        public IComFlussoDao ComFlussoDao { get { return new ComFlussoOracleDb(this); } }

        public IComAllegatoDao ComAllegatoDao { get { return new ComAllegatoOracleDb(this); } }

        public IMailServerDao MailServerDao { get { return new MailServerDaoOracleDb(this); } }

        public IMailAccountDao MailAccountDao { get { return new MailAccountDaoOracleDb(this); } }

        public IMailMessageDao MailMessageDao { get { return new MailMessageDaoOracleDb(this); } }

        public IMailHeaderDao MailHeaderDao { get { return new MailHeaderDaoOracleDb(this); } }

        public IReferralTypeDao ReferralTypeDao { get { return new ReferralTypeDaoOracleDb(this); } }

        public SendMail.Data.Contracts.IContactsBackendDao ContactsBackendDao { get { return new ContactsBackendOracleDb(this); } }

        public ISendersFoldersDao SendersFoldersDao { get { return new SendersFoldersOracleDb(this); } }

        #endregion
    }
}

