using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class ComAllegato : IDomainObject
    {

        #region IDomainObject Membri di

        public bool IsValid
        {
            get
            {
                return (!String.IsNullOrEmpty(this.AllegatoExt) &&
                    (this.AllegatoFile != null && this.AllegatoFile.Length > 0) &&
                    !String.IsNullOrEmpty(this.AllegatoName));
            }
        }

        public bool IsPersistent
        {
            get { return (this.IdAllegato.HasValue && this.IdAllegato.Value > 0); }
        }

        #endregion

        #region "Properties"
        public virtual Nullable<Int64> IdAllegato { get; set; }

        public virtual String AllegatoName { get; set; }

        public virtual Byte[] AllegatoFile { get; set; }

        public virtual String AllegatoExt { get; set; }

        public virtual Nullable<Int64> RefIdCom { get; set; }

        public virtual AllegatoProtocolloStatus FlgProtToUpl { get; set; }

        public virtual String AllegatoTpu { get; set; }

        public virtual AllegatoProtocolloStatus FlgInsProt { get; set; }

        public virtual int T_Progr { get; set; }
        #endregion

        #region "C.tor"
        public ComAllegato() { }

        public ComAllegato(ComAllegato c)
        {
            if (c == null) return;

            Type t = c.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(c, null), null);
                }
            }
        }

        #endregion
    }
}
