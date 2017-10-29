using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SendMail.Model
{
    public class BackendUser : IDomainObject
    {
        [DatabaseField("ID_USER")]
        public virtual Int64 UserId { get; set; }

        [DatabaseField("USER_NAME")]
        public virtual string UserName { get; set; }

        [DatabaseField("COGNOME")]
        public virtual string Cognome { get; set; }

        [DatabaseField("NOME")]
        public virtual string Nome { get; set; }

        [DatabaseField("DEPARTMENT")]
        public virtual Int64 Department { get; set; }

        [DatabaseField("MUNICIPIO")]
        public virtual string Municipio { get; set; }

        [DatabaseField("DOMAIN")]
        public virtual string Domain { get; set; }

        [DatabaseField("ROLE")]
        public virtual Int32 UserRole { get; set; }

        [DatabaseField("ROLE_MAIL")]
        public virtual Int32 RoleMail { get; set; }

        public virtual List<BackEndUserMailUserMapping> MappedMails { get; set; }

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return true; }
        }

        public bool IsPersistent
        {
            get { return true; }
        }

        public string CodiceFiscale { get; set; }

        #endregion
    }

    public class BackEndUserMailUserMapping : ActiveUp.Net.Mail.DeltaExt.MailUser, IDomainObject
    {
        public BackEndUserMailUserMapping(ActiveUp.Net.Mail.DeltaExt.MailUser mailAccount, int accessLevel)
        {
            this.Casella = mailAccount.Casella;
            this.DisplayName = mailAccount.DisplayName;
            this.Dominus = mailAccount.Dominus;
            this.EmailAddress = mailAccount.EmailAddress;
            this.Id = this.MailSenderId = mailAccount.Id;
            this.IncomingProtocol = mailAccount.IncomingProtocol;
            this.IncomingServer = mailAccount.IncomingServer;
            this.IsIncomeSecureConnection = mailAccount.IsIncomeSecureConnection;
            this.FlgManaged = mailAccount.FlgManaged;
            this.FlgManagedInsert = (mailAccount.FlgManaged > 0) ? true : false; 
            this.IsOutgoingSecureConnection = mailAccount.IsOutgoingSecureConnection;
            this.IsOutgoingWithAuthentication = mailAccount.IsOutgoingWithAuthentication;
            this.IsPec = mailAccount.IsPec;
            this.OutgoingServer = mailAccount.OutgoingServer;
            this.Password = mailAccount.Password;
            this.PortIncomingChecked = mailAccount.PortIncomingChecked;
            this.PortIncomingServer = mailAccount.PortIncomingServer;
            this.PortOutgoingChecked = mailAccount.PortOutgoingChecked;
            this.PortOutgoingServer = mailAccount.PortOutgoingServer;
            this.UserId = mailAccount.UserId;
            this.MailAccessLevel = accessLevel;
        }

        public BackEndUserMailUserMapping(decimal mailSenderId, int accessLevel)
        {
            this.Id = mailSenderId;
            this.MailAccessLevel = accessLevel;
        }

        public BackEndUserMailUserMapping()
        {

        }

        [DatabaseField("ID_SENDER")]
        public virtual decimal MailSenderId { get; set; }

        [DatabaseField("ROLE")]
        public virtual Int32 MailAccessLevel { get; set; }

        #region IDomainObject Membri di

        public bool IsValid
        {
            //get { throw new NotImplementedException(); }
            get { return true; }
        }

        public bool IsPersistent
        {
            // get { throw new NotImplementedException(); }
           get { return true; }
        }

        #endregion
    }

}
