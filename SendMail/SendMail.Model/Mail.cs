using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Model.ComunicazioniMapping;

namespace SendMail.Model
{
    [Serializable()]
    public class Mail : IDomainObject
    {
        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return (IdMail > 0 && RefIdCom > 0 && Refs != null && !IsCanceled); }
        }

        public bool IsPersistent
        {
            get { return IdMail > 0; }
        }

        #endregion

        #region "C.tor"

        public Mail()
        {

        }

        public Mail(String mailSender, String subject, String mailText,
            DateTime insertDate, Nullable<DateTime> sendDate, String sendUser)
        {
            this.IdMail = 0;
            this.RefIdCom = 0;

            this.MailSender = mailSender;
            this.Subject = subject;
            this.MailText = mailText;
            this.InsertDate = insertDate;
            this.SendDate = sendDate;
            this.UtenteInvio = sendUser;
        }

        #endregion


        #region "Properties"

        [DatabaseField("ID_MAIL")]
        public virtual Int64 IdMail { get; set; }

        [DatabaseField("REF_ID_COM")]
        public virtual Int64 RefIdCom { get; set; }

        [DatabaseField("MAIL_SENDER")]
        public virtual String MailSender { get; set; }

        public virtual ICollection<MailRefs> Refs { get; set; }
        
        public virtual ICollection<ComAllegato> Attachments { get; set; }

        [DatabaseField("OGGETTO")]
        public virtual String Subject { get; set; }

        [DatabaseField("TESTO")]
        public virtual String MailText { get; set; }

        [DatabaseField("FLG_ANNULLAMENTO")]
        private String FlgCancelled { get; set; }

        public virtual Boolean IsCanceled
        {
            get
            {
                Boolean isCancelled = false;
                if (!String.IsNullOrEmpty(FlgCancelled))
                {
                    if (FlgCancelled.Equals("1") ||
                        FlgCancelled.Equals("S", StringComparison.InvariantCultureIgnoreCase) ||
                        FlgCancelled.Equals("T", StringComparison.InvariantCultureIgnoreCase))
                        isCancelled = true;
                }
                return isCancelled;
            }
            set
            {
                FlgCancelled = Convert.ToInt32(value).ToString();
            }
        }

        [DatabaseField("DATA_INSERIMENTO")]
        public virtual DateTime InsertDate { get; set; }

        [DatabaseField("DATA_INVIO")]
        public virtual Nullable<DateTime> SendDate { get; set; }

        [DatabaseField("UTE_INVIO")]
        public virtual String UtenteInvio { get; set; }

        #endregion


        #region "Methods"

        public void SetMailRefs(ICollection<MailRefs> mailRefs)
        {
            this.Refs = mailRefs;
        }

        public void SetMailRefs(ICollection<String> mailAddress)
        {
            this.Refs = mailAddress.Select(x => new MailRefs(x, AddresseeType.TO)).ToList();
        }

        #endregion


    }
}
