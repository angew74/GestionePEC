using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    public class Rubrica : IDomainObject
    {
        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public bool IsPersistent
        {
            get { return false; }
        }
        #endregion

        #region "Properties"

        [DatabaseField("ID_RUB")]
        public virtual decimal IdRubrica { get; set; }

        [DatabaseField("COGNOME")]
        public virtual String Cognome { get; set; }

        [DatabaseField("NOME")]
        public virtual String Nome { get; set; }

        [DatabaseField("CODFIS")]
        public virtual String Codice_Fiscale { get; set; }

        [DatabaseField("PIVA")]
        public virtual String Partita_Iva { get; set; }

        [DatabaseField("RAGIONESOCIALE")]
        public virtual String Ragione_Sociale { get; set; }

        [DatabaseField("UFFICIO")]
        public virtual String Ufficio { get; set; }

        [DatabaseField("MAIL")]
        public virtual String Mail { get; set; }

        [DatabaseField("FAX")]
        public virtual String Fax { get; set; }

        [DatabaseField("TELEFONO")]
        public virtual String Telefono { get; set; }

        [DatabaseField("STATO")]
        public virtual String Stato { get; set; }

        [DatabaseField("PROVINCIA")]
        public virtual String Provincia { get; set; }

        [DatabaseField("CITTA")]
        public virtual String Citta { get; set; }

        [DatabaseField("SEDIME")]
        public virtual String Sedime { get; set; }

        [DatabaseField("VIA")]
        public virtual String Via { get; set; }

        [DatabaseField("NUMERO")]
        public virtual String Numero { get; set; }

        [DatabaseField("LETTERA")]
        public virtual String Lettera { get; set; }

        [DatabaseField("ID_TITOLO")]
        public virtual Int64 IdTitolo { get; set; }

        [DatabaseField("CAP")]
        public virtual String Cap { get; set; }

        [DatabaseField("CERTIFIED")]
        public virtual Int32 Certificato { get; set; }

        [DatabaseField("NOTE")]
        public virtual String Note { get; set; }

        [DatabaseField("FLG_IPA")]
        public virtual String Flag_IPA { get; set; }

        #endregion
    }
}
