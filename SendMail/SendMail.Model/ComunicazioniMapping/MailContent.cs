using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class MailContent : IDomainObject
    {
        #region IDomainObject Membri di

        public bool IsValid
        {
            get
            {
                return ((this.IdMail.HasValue) &&
                    (this.RefIdComunicazione.HasValue == false));
            }
        }

        public bool IsPersistent
        {
            get
            {
                return ((this.IdMail.HasValue) &&
                    (this.IdMail.Value > 0));
            }
        }

        #endregion

        #region "Public Properties"

        public virtual String MailText { get; set; }

        public virtual String MailSender { get; set; }

        public virtual Boolean IsCancelled { get; set; }

        public virtual Nullable<Int64> IdMail { get; set; }

        public virtual String MailSubject { get; set; }

        public virtual Nullable<Int64> RefIdComunicazione { get; set; }

        public virtual Boolean HasCustomRefs { get; set; }

        public virtual Nullable<Int64> Follows { get; set; }

        public virtual List<MailRefs> MailRefs { get; set; }

        #endregion

        #region "C.tor"
        public MailContent() { }

        public MailContent(MailContent mc)
        {
            if (mc == null) return;

            Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(mc, null), null);
                }
            }
        }

        #endregion
    }

    [Serializable]
    public class MailRefs : IDomainObject
    {

        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(MailDestinatario) &&
                    TipoRef != AddresseeType.UNDEFINED;
            }
        }

        public bool IsPersistent
        {
            get { return true; }
        }
        #endregion

        #region "public Properties"
        public virtual String MailDestinatario { get; set; }

        public virtual AddresseeType TipoRef { get; set; }
        #endregion

        #region "C.tor"
        public MailRefs() { }

        public MailRefs(MailRefs mr)
        {
            if (mr == null) return;

            this.MailDestinatario = mr.MailDestinatario;
            this.TipoRef = mr.TipoRef;
        }
        #endregion
    }
}
