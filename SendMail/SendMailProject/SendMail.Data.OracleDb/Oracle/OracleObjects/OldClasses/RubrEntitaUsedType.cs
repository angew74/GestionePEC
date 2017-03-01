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
    using SendMail.Model.ComunicazioniMapping;
    using SendMail.Model;
    using System.Linq;
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;

    public class RubrEntitaUsedType : RubrEntitaUsed, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        #region "Private Fields"
        private bool m_IsNull;

        private long m_ID_ENT_USED;

        private bool m_ID_ENT_USEDIsNull;

        private string m_SIGLA_PROV;

        private string m_COD_ISO_STATO;

        private long m_ID_REFERRAL;

        private bool m_ID_REFERRALIsNull;

        private string m_FAX;

        private string m_COMUNE;

        private string m_RAGIONE_SOCIALE;

        private string m_MAIL;

        private string m_COD_FIS;

        private string m_CIVICO;

        private string m_UFFICIO;

        private string m_P_IVA;

        private string m_TELEFONO;

        private string m_NOTE;

        private string m_COGNOME;

        private string m_INDIRIZZO;

        private string m_CAP;

        private string m_REFERRAL_TYPE;

        private string m_NOME;

        private string m_CONTACT_REF;
        #endregion

        #region "C.tor"
        public RubrEntitaUsedType()
        {
            // TODO : Add code to initialise the object
            this.m_ID_ENT_USEDIsNull = true;
            this.m_ID_REFERRALIsNull = true;
        }

        public RubrEntitaUsedType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public RubrEntitaUsedType(RubrEntitaUsed r)
            : base(r)
        {
            if (r == null || r.IsValid == false)
            {
                this.m_IsNull = true;
                return;
            }

            this.m_ID_ENT_USEDIsNull = !r.IdEntUsed.HasValue;
            this.m_ID_REFERRALIsNull = !r.IdReferral.HasValue;
            this.m_REFERRAL_TYPE = (r.ReferralType != EntitaType.UNKNOWN) ? r.ReferralType.ToString() : null;
        }
        #endregion

        #region "Public Properties
        public virtual bool IsNull
        {
            get
            {
                return this.m_IsNull;
            }
        }

        public static RubrEntitaUsedType Null
        {
            get
            {
                RubrEntitaUsedType obj = new RubrEntitaUsedType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("ID_ENT_USED")]
        public override long? IdEntUsed
        {
            get
            {
                if (this.m_ID_ENT_USEDIsNull == true) return null;
                return this.m_ID_ENT_USED;
            }
            set
            {
                this.m_ID_ENT_USEDIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_ID_ENT_USED = (long)value;
            }
        }

        public bool ID_ENT_USEDIsNull
        {
            get
            {
                return this.m_ID_ENT_USEDIsNull;
            }
            set
            {
                this.m_ID_ENT_USEDIsNull = value;
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

        [OracleTypes.OracleObjectMappingAttribute("COD_ISO_STATO")]
        public override string CodISOStato
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

        [OracleTypes.OracleObjectMappingAttribute("ID_REFERRAL")]
        public override long? IdReferral
        {
            get
            {
                if (this.m_ID_REFERRALIsNull == true) return null;
                return this.m_ID_REFERRAL;
            }
            set
            {
                this.m_ID_REFERRALIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_ID_REFERRAL = (long)value;
            }
        }

        public bool ID_REFERRALIsNull
        {
            get
            {
                return this.m_ID_REFERRALIsNull;
            }
            set
            {
                this.m_ID_REFERRALIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("FAX")]
        public override string Fax
        {
            get
            {
                return this.m_FAX;
            }
            set
            {
                this.m_FAX = value;
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

        [OracleTypes.OracleObjectMappingAttribute("RAGIONE_SOCIALE")]
        public override string RagioneSociale
        {
            get
            {
                return this.m_RAGIONE_SOCIALE;
            }
            set
            {
                this.m_RAGIONE_SOCIALE = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("MAIL")]
        public override string Mail
        {
            get
            {
                return this.m_MAIL;
            }
            set
            {
                this.m_MAIL = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("COD_FIS")]
        public override string CodiceFiscale
        {
            get
            {
                return this.m_COD_FIS;
            }
            set
            {
                this.m_COD_FIS = value;
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

        [OracleTypes.OracleObjectMappingAttribute("UFFICIO")]
        public override string Ufficio
        {
            get
            {
                return this.m_UFFICIO;
            }
            set
            {
                this.m_UFFICIO = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("P_IVA")]
        public override string PartitaIVA
        {
            get
            {
                return this.m_P_IVA;
            }
            set
            {
                this.m_P_IVA = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("TELEFONO")]
        public override string Telefono
        {
            get
            {
                return this.m_TELEFONO;
            }
            set
            {
                this.m_TELEFONO = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("NOTE")]
        public override string Note
        {
            get
            {
                return this.m_NOTE;
            }
            set
            {
                this.m_NOTE = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("COGNOME")]
        public override string Cognome
        {
            get
            {
                return this.m_COGNOME;
            }
            set
            {
                this.m_COGNOME = value;
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

        [OracleTypes.OracleObjectMappingAttribute("REFERRAL_TYPE")]
        public string REFERRAL_TYPE
        {
            get
            {
                return this.m_REFERRAL_TYPE;
            }
            set
            {
                this.m_REFERRAL_TYPE = value;

                EntitaType entType = EntitaType.UNKNOWN;
                if (!String.IsNullOrEmpty(value))
                    if (Enum.GetNames(typeof(EntitaType)).Contains(value, StringComparer.InvariantCultureIgnoreCase))
                        entType = (EntitaType)Enum.Parse(typeof(EntitaType), value);
                base.ReferralType = entType;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("NOME")]
        public override string Nome
        {
            get
            {
                return this.m_NOME;
            }
            set
            {
                this.m_NOME = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("CONTACT_REF")]
        public override string ContactRef
        {
            get
            {
                return this.m_CONTACT_REF;
            }
            set
            {
                this.m_CONTACT_REF = value;
            }
        }
        #endregion

        #region "public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            if ((ID_ENT_USEDIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_ENT_USED", this.IdEntUsed);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "SIGLA_PROV", this.SiglaProvincia);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COD_ISO_STATO", this.CodISOStato);
            if ((ID_REFERRALIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_REFERRAL", this.IdReferral);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FAX", this.Fax);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COMUNE", this.Comune);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RAGIONE_SOCIALE", this.RagioneSociale);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL", this.Mail);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COD_FIS", this.CodiceFiscale);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "CIVICO", this.Civico);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "UFFICIO", this.Ufficio);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "P_IVA", this.PartitaIVA);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "TELEFONO", this.Telefono);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "NOTE", this.Note);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COGNOME", this.Cognome);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "INDIRIZZO", this.Indirizzo);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "CAP", this.Cap);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REFERRAL_TYPE", this.REFERRAL_TYPE);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "NOME", this.Nome);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "CONTACT_REF", this.ContactRef);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.ID_ENT_USEDIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_ENT_USED");
            if ((ID_ENT_USEDIsNull == false))
            {
                this.IdEntUsed = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_ENT_USED")));
            }
            this.SiglaProvincia = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "SIGLA_PROV")));
            this.CodISOStato = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COD_ISO_STATO")));
            this.ID_REFERRALIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_REFERRAL");
            if ((ID_REFERRALIsNull == false))
            {
                this.IdReferral = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_REFERRAL")));
            }
            this.Fax = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FAX")));
            this.Comune = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COMUNE")));
            this.RagioneSociale = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RAGIONE_SOCIALE")));
            this.Mail = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL")));
            this.CodiceFiscale = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COD_FIS")));
            this.Civico = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CIVICO")));
            this.Ufficio = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "UFFICIO")));
            this.PartitaIVA = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "P_IVA")));
            this.Telefono = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "TELEFONO")));
            this.Note = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "NOTE")));
            this.Cognome = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COGNOME")));
            this.Indirizzo = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "INDIRIZZO")));
            this.Cap = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CAP")));
            this.REFERRAL_TYPE = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REFERRAL_TYPE")));
            this.Nome = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "NOME")));
            this.ContactRef = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CONTACT_REF")));
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

        public static RubrEntitaUsedType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new RubrEntitaUsedType();
        }
        #endregion
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("RUBR_ENTITA_USED_TYPE")]
    public class RubrEntitaUsedTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            RubrEntitaUsedType obj = new RubrEntitaUsedType();
            return obj;
        }
    }
}
