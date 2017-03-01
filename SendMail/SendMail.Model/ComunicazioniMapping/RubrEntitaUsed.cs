using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class RubrEntitaUsed : IDomainObject
    {
        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return (this.IdEntUsed.HasValue && 
                    (!String.IsNullOrEmpty(this.Mail) ||
                    !String.IsNullOrEmpty(this.Fax) ||
                    !String.IsNullOrEmpty(this.Telefono)));
            }
        }

        public bool IsPersistent
        {
            get { return (this.IdEntUsed.HasValue && this.IdEntUsed.Value > 0); }
        }
        #endregion

        #region "Public Properties"
        public virtual Nullable<Int64> IdEntUsed { get; set; }

        public virtual String SiglaProvincia { get; set; }

        public virtual String CodISOStato { get; set; }

        public virtual Nullable<Int64> IdReferral { get; set; }

        public virtual String Fax { get; set; }

        public virtual String Comune { get; set; }

        public virtual String RagioneSociale { get; set; }

        public virtual String Mail { get; set; }

        public virtual String CodiceFiscale { get; set; }

        public virtual String Civico { get; set; }

        public virtual String Ufficio { get; set; }

        public virtual String PartitaIVA { get; set; }

        public virtual String Telefono { get; set; }

        public virtual String Note { get; set; }

        public virtual String Cognome { get; set; }

        public virtual String Indirizzo { get; set; }

        public virtual String Cap { get; set; }

        public virtual EntitaType ReferralType { get; set; }

        public virtual String Nome { get; set; }

        public virtual String ContactRef { get; set; }

        public virtual AddresseeType TipoContatto { get; set; }

        public virtual Boolean IsDefault { get; set; }
        #endregion

        #region "C.tor"
        public RubrEntitaUsed() { }

        public RubrEntitaUsed(RubrEntitaUsed reu)
        {
            if (reu == null) return;

            Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(reu, null), null);
                }
            }
        }
        #endregion
    }
}
