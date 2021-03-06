//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:2.0.50727.3625
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SendMail.Data.OracleDb.OracleObjects
{
    using System;
    using System.Xml.Serialization;
    using System.Xml.Schema;
    using SendMail.Model.RubricaMapping;
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;
    
    [Serializable]
    public class RubricaAddressType : RubricaAddress, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        #region "Private Fields"
        private bool m_IsNull;
        private bool m_AFF_IPAIsNull;
        private bool m_ID_ADDRESSIsNull;
        private string m_INDIRIZZO_NON_STANDARD;
        private string m_INDIRIZZO;
        private string m_FLG_IPA;
        private string m_COMUNE;
        private string m_CAP;
        private string m_CIVICO;
        private short m_AFF_IPA;
        private long m_ID_ADDRESS;
        private string m_COD_ISO_STATO;
        private string m_SIGLA_PROV;
        #endregion

        #region "C.tor"
        public RubricaAddressType()
        {
            // TODO : Add code to initialise the object
            this.m_AFF_IPAIsNull = true;
            this.m_ID_ADDRESSIsNull = true;
        }

        public RubricaAddressType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public RubricaAddressType(RubricaAddress r)
            : base(r)
        {
            if (r == null || r.IsValid == false)
            {
                this.m_IsNull = true;
                return;
            }

            this.m_AFF_IPAIsNull = !r.AffIPA.HasValue;
            this.m_ID_ADDRESSIsNull = !r.IdAddress.HasValue;
            this.m_FLG_IPA = Convert.ToInt32(r.IsIpa).ToString();
            this.m_INDIRIZZO_NON_STANDARD = Convert.ToInt32(r.IndirizzoNonStandard).ToString();
        }
        #endregion

        #region "Public Fields"
        public virtual bool IsNull
        {
            get
            {
                return this.m_IsNull;
            }
        }

        public static RubricaAddressType Null
        {
            get
            {
                RubricaAddressType obj = new RubricaAddressType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("INDIRIZZO_NON_STANDARD")]
        public string INDIRIZZO_NON_STANDARD
        {
            get
            {
                return this.m_INDIRIZZO_NON_STANDARD;
            }
            set
            {
                this.m_INDIRIZZO_NON_STANDARD = value;

                int indNonStand = 0;
                if (!String.IsNullOrEmpty(value))
                    if (int.TryParse(value, out indNonStand))
                        base.IndirizzoNonStandard = Convert.ToBoolean(indNonStand);
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("INDIRIZZO")]
        public override string Indirizzo
        {
            get
            {
                return this.m_INDIRIZZO;
            }
            set
            {
                this.m_INDIRIZZO = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("FLG_IPA")]
        public string FLG_IPA
        {
            get
            {
                return this.m_FLG_IPA;
            }
            set
            {
                this.m_FLG_IPA = value;

                int isIpa = 0;
                if (!String.IsNullOrEmpty(value))
                    if (int.TryParse(value, out isIpa))
                        base.IsIpa = Convert.ToBoolean(isIpa);
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("COMUNE")]
        public override string Comune
        {
            get
            {
                return this.m_COMUNE;
            }
            set
            {
                this.m_COMUNE = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("CAP")]
        public override string Cap
        {
            get
            {
                return this.m_CAP;
            }
            set
            {
                this.m_CAP = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("CIVICO")]
        public override string Civico
        {
            get
            {
                return this.m_CIVICO;
            }
            set
            {
                this.m_CIVICO = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("AFF_IPA")]
        public override short? AffIPA
        {
            get
            {
                if (this.m_AFF_IPAIsNull == true) return null;
                return this.m_AFF_IPA;
            }
            set
            {
                this.m_AFF_IPAIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_AFF_IPA = (short)value;
            }
        }

        public bool AFF_IPAIsNull
        {
            get
            {
                return this.m_AFF_IPAIsNull;
            }
            set
            {
                this.m_AFF_IPAIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("ID_ADDRESS")]
        public override long? IdAddress
        {
            get
            {
                if (this.m_ID_ADDRESSIsNull == true) return null;
                return this.m_ID_ADDRESS;
            }
            set
            {
                this.m_ID_ADDRESSIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_ID_ADDRESS = (long)value;
            }
        }

        public bool ID_ADDRESSIsNull
        {
            get
            {
                return this.m_ID_ADDRESSIsNull;
            }
            set
            {
                this.m_ID_ADDRESSIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("COD_ISO_STATO")]
        public override string CodIsoStato
        {
            get
            {
                return this.m_COD_ISO_STATO;
            }
            set
            {
                this.m_COD_ISO_STATO = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("SIGLA_PROV")]
        public override string SiglaProvincia
        {
            get
            {
                return this.m_SIGLA_PROV;
            }
            set
            {
                this.m_SIGLA_PROV = value;
            }
        }
        #endregion

        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, "INDIRIZZO_NON_STANDARD", this.INDIRIZZO_NON_STANDARD);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "INDIRIZZO", this.Indirizzo);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FLG_IPA", this.FLG_IPA);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COMUNE", this.Comune);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "CAP", this.Cap);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "CIVICO", this.Civico);
            if ((AFF_IPAIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "AFF_IPA", this.AffIPA);
            }
            if ((ID_ADDRESSIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_ADDRESS", this.IdAddress);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COD_ISO_STATO", this.CodIsoStato);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "SIGLA_PROV", this.SiglaProvincia);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.INDIRIZZO_NON_STANDARD = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "INDIRIZZO_NON_STANDARD")));
            this.Indirizzo = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "INDIRIZZO")));
            this.FLG_IPA = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FLG_IPA")));
            this.Comune = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COMUNE")));
            this.Cap = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CAP")));
            this.Civico = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CIVICO")));
            this.AFF_IPAIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "AFF_IPA");
            if ((AFF_IPAIsNull == false))
            {
                this.AffIPA = ((short)(OracleTypes.OracleUdt.GetValue(con, pUdt, "AFF_IPA")));
            }
            this.ID_ADDRESSIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_ADDRESS");
            if ((ID_ADDRESSIsNull == false))
            {
                this.IdAddress = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_ADDRESS")));
            }
            this.CodIsoStato = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COD_ISO_STATO")));
            this.SiglaProvincia = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "SIGLA_PROV")));
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            // TODO : Read Serialized Xml Data
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            // TODO : Serialize object to xml data
        }

        public virtual XmlSchema GetSchema()
        {
            // TODO : Implement GetSchema
            return null;
        }

        public override string ToString()
        {
            // TODO : Return a string that represents the current object
            return "";
        }

        public static RubricaAddressType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new RubricaAddressType();
        }
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("RUBR_ADDRESS_TYPE")]
    public class RubricaAddressTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            RubricaAddressType obj = new RubricaAddressType();
            return obj;
        }
    }
}
