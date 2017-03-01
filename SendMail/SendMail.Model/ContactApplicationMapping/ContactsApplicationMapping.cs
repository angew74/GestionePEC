using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    public class ContactsApplicationMapping : IDomainObject
    {

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return true; }
        }

        public bool IsPersistent
        {
            get { return true; }
        }

        #endregion

        #region "Properties"

        [DatabaseField("ID_MAP")]
        public Int64 IdMap { get; set; }

        [DatabaseField("ID_TITOLO")]
        public Int64 IdTitolo { get; set; }

        [DatabaseField("APP_CODE")]
        public String AppCode { get; set; }

        [DatabaseField("TITOLO")]
        public String Titolo { get; set; }

        [DatabaseField("TITOLO_PROT_CODE")]
        public String TitoloCode { get; set; }

        [DatabaseField("TITOLO_ACTIVE")]
        private UInt16 TitoloActive { get; set; }

        public Boolean IsTitoloActive
        {
            get
            {
                return TitoloActive.Equals(1);
            }
            set
            {
                TitoloActive = Convert.ToUInt16(value);
            }
        }

        [DatabaseField("ID_SOTTOTITOLO")]
        public Int64 IdSottotitolo { get; set; }

        [DatabaseField("SOTTOTITOLO")]
        public String Sottotitolo { get; set; }

        [DatabaseField("SOTTOTITOLO_PROT_CODE")]
        public String SottotitoloCode { get; set; }

        [DatabaseField("SOTTOTITOLO_ACTIVE")]
        private UInt16 SottotitoloActive { get; set; }

        public Boolean IsSottotitoloActive
        {
            get
            {
                return Convert.ToBoolean(SottotitoloActive);
            }
            set
            {
                SottotitoloActive = Convert.ToUInt16(value);
            }
        }

        [DatabaseField("COM_CODE")]
        public String ComCode { get; set; }

        [DatabaseField("ID_CONTACT")]
        public Int64 IdContact { get; set; }

        [DatabaseField("MAIL")]
        public String Mail { get; set; }

        [DatabaseField("FAX")]
        public String Fax { get; set; }

        [DatabaseField("TELEFONO")]
        public String Telefono { get; set; }

        [DatabaseField("REF_ID_REFERRAL")]
        public Int64 RefIdReferral { get; set; }

        [DatabaseField("FLG_PEC")]
        private UInt16 FlgPec { get; set; }

        public Boolean IsPec
        {
            get { return FlgPec.Equals(1); }
            set { FlgPec = Convert.ToUInt16(value); }
        }

        [DatabaseField("REF_ORG")]
        public Int64 RefOrg { get; set; }

        [DatabaseField("ID_CANALE")]
        public Int64 IdCanale { get; set; }

        [DatabaseField("CODICE")]
        public String Codice { get; set; }

        [DatabaseField("ID_BACKEND")]
        public Int64 IdBackend { get; set; }

        [DatabaseField("BACKEND_CODE")]
        public String BackendCode { get; set; }

        [DatabaseField("BACKEND_DESCR")]
        public String BackendDescr { get; set; }

        [DatabaseField("CATEGORY")]
        public String Category { get; set; }

        [DatabaseField("DESCR_PLUS")]
        public String DescrPlus { get; set; }

        #endregion
    }
}