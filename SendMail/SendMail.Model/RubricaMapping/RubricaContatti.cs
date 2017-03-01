using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Model.RubricaMapping
{
    [Serializable]
    public class RubricaContatti : IDomainObject
    {
        public RubricaContatti select()
        {
            return this;
        }

        public RubricaContatti save(RubricaContatti e)
        {
            return e;
        }

        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return ((!String.IsNullOrEmpty(this.Mail) ||
                    !String.IsNullOrEmpty(this.Fax) ||
                    !String.IsNullOrEmpty(this.Telefono))
                    && this.Entita != null);
            }
        }

        public bool IsPersistent
        {
            get { return (IdContact.HasValue && IdContact.Value > 0); }
        }
        #endregion

        #region "Properties"
        public virtual Nullable<Int64> IdContact { get; set; }

        public virtual String Mail { get; set; }

        public virtual String Fax { get; set; }

        public virtual String Telefono { get; set; }

        public virtual Nullable<Int64> RefIdReferral { get; set; }

        public virtual String Source { get; set; }

        public Boolean IsIPA { get; set; }

        public virtual String IPAdn { get; set; }

        public virtual String IPAId { get; set; }

        public virtual String Note { get; set; }

        public virtual String ContactRef { get; set; }

        public List<Int64> MappedAppsId { get; set; }

        public AddresseeType TipoContatto { get; set; }

        public virtual Nullable<Int16> AffIPA { get; set; }

        public Boolean IsPec { get; set; }

        public virtual String Src { get; set; }

        //usati dal frontend per il binding dei dati//
        public virtual string T_MappedAppName { get; set; }
        public virtual bool T_isMappedAppDefault { get; set; }
        public virtual long T_MappedAppID { get; set; }
        //PS.: che schifo!!!

        public RubricaEntita Entita { get; set; }

        private string t_RagioneSociale;
        public string T_RagioneSociale
        {
            get
            {
                if (String.IsNullOrEmpty(t_RagioneSociale))
                {
                    if (Entita != null)
                        t_RagioneSociale = Entita.RagioneSociale;
                }
                return t_RagioneSociale;
            }
            set
            {
                t_RagioneSociale = value;
            }
        }

        private string t_Ufficio;
        public string T_Ufficio
        {
            get
            {
                if (String.IsNullOrEmpty(t_Ufficio))
                {
                    if (Entita != null)
                        t_Ufficio = Entita.Ufficio;
                }
                return t_Ufficio;
            }
            set
            {
                t_Ufficio = value;
            }
        }
        #endregion

        #region "C.tor"
        public RubricaContatti() { }

        public RubricaContatti(RubricaContatti rc)
        {
            if (rc == null) return;

            Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(rc, null), null);
                }
            }
        }
        #endregion

        #region Public Methods
        public void MapTitoliContatto(Titolo titolo)
        {
            this.T_MappedAppID = (long)(titolo.Id);
            this.T_MappedAppName = titolo.Nome;

            if (this.MappedAppsId.Contains((long)titolo.Id))
            {
                this.T_isMappedAppDefault = true;
            }
            else
            {
                this.T_isMappedAppDefault = false;
            }
        }
        #endregion

        #region "Clone"
        public RubricaContatti Clone()
        {
            RubricaEntita e = this.Entita.Clone();
            return e.Contatti.SingleOrDefault(x => x.IdContact == this.IdContact);
        }

        public RubricaContatti Clone(RubricaEntita e)
        {
            RubricaContatti c = new RubricaContatti(this);
            c.Entita = e;
            return c;
        } 
        #endregion
    }
}
