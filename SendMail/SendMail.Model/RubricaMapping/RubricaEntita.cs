using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.RubricaMapping
{
    [Serializable]
    public class RubricaEntita : IDomainObject
    {
        #region IDomainObject Membri di
        public bool IsValid
        {
            get { return IdPadre.HasValue; }
        }

        public bool IsPersistent
        {
            get { return IdReferral.Value > 0; }
        }
        #endregion

        #region "Properties"
        public virtual Nullable<Int64> IdReferral { get; set; }

        private RubricaEntita padre;
        public virtual RubricaEntita Padre
        {
            get 
            {
                if (padre != null) return padre;
                else{
                RubricaEntita p= new RubricaEntita();
                    p.IdReferral=this.IdPadre;
                    return p;
                } 
            }
            set 
            {
                if(value.IdReferral!=null && value.IdReferral!=this.IdPadre)
                value.IdReferral = this.IdPadre;
                this.padre = value;
            }
        }

        public virtual String Cognome { get; set; }

        public virtual Nullable<Int64> RefIdAddress { get; set; }

        public Boolean IsIPA { get; set; }

        public virtual String IPAdn { get; set; }

        public virtual String CodiceFiscale { get; set; }

        public virtual String Nome { get; set; }

        public virtual String RagioneSociale { get; set; }

        public virtual Nullable<Int64> IdPadre { get; set; }

        public virtual String Ufficio { get; set; }

        public virtual String PartitaIVA { get; set; }

        public virtual String Note { get; set; }

        public virtual String IPAId { get; set; }

        public EntitaType ReferralType { get; set; }

        public virtual Nullable<Int64> RefOrg { get; set; }

        public virtual String Src { get; set; }

        public virtual String DisambPre { get; set; }

        public virtual String DisambPost { get; set; }

        public virtual String SitoWeb { get; set; }

        public virtual Nullable<Int16> AffIPA { get; set; }

        public List<RubricaContatti> Contatti { get; set; }

        public RubricaAddress Address { get; set; }
        #endregion

        #region "c.tor"
        public RubricaEntita() { }

        public RubricaEntita(RubricaEntita ent)
        {
            if (ent == null) return;

            Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(ent, null), null);
                }
            }
        }
        #endregion

        #region "Public Methods"
        public RubricaEntita select()
        {
            return this;
        }

        public RubricaEntita save(RubricaEntita e)
        {
            return e;
        }
        #endregion

        #region "Clone"
        public RubricaEntita Clone()
        {
            RubricaEntita e = new RubricaEntita(this);
            List<RubricaContatti> l = new List<RubricaContatti>();
            if (this.Contatti != null)
            {
                foreach (RubricaContatti c in this.Contatti)
                {
                    l.Add(c.Clone(e));
                }
            }
            e.Contatti = l;
            if (this.Address != null)
                e.Address = this.Address.Clone();

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.IFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, e);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                e = (RubricaEntita)bf.Deserialize(ms);
            }

            return e;
        }
        #endregion
    }
}
