using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SendMail.Model.RubricaMapping
{
    [Serializable]
    public class RubricaAddress : IDomainObject
    {
        #region IDomainObject Membri di
        public bool IsValid
        {
            get { return IdAddress.HasValue; }
        }

        public bool IsPersistent
        {
            get { return IdAddress.Value > 0; }
        }
        #endregion

        #region "Properties"
        public virtual String Indirizzo { get; set; }

        public virtual Boolean IsIpa { get; set; }

        public virtual String Comune { get; set; }

        public virtual String Cap { get; set; }

        public virtual String Civico { get; set; }

        public virtual Nullable<Int64> IdAddress { get; set; }

        public virtual String CodIsoStato { get; set; }

        public virtual String SiglaProvincia { get; set; }

        public virtual Boolean IndirizzoNonStandard { get; set; }

        public virtual Nullable<Int16> AffIPA { get; set; }
        #endregion

        #region "C.tor"
        public RubricaAddress() { }

        public RubricaAddress(RubricaAddress ra)
        {
            if (ra == null) return;

            Type t = this.GetType();

            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(ra, null), null);
                }
            }
        }
        #endregion

        #region "Clone"
        public RubricaAddress Clone()
        {
            return new RubricaAddress(this);
        } 
        #endregion
    }
}
