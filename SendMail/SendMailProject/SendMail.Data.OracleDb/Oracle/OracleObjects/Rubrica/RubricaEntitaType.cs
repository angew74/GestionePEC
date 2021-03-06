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
    using System.Collections.Generic;
    using System.Linq;
    using SendMail.Model;
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;

    public class RubricaEntitaType : RubricaEntita, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        #region "Private Fields"
        private bool m_IsNull;
        private RubricaContattiRefListType m_RUBR_CONTATTI_REFS;
        private RubricaContattiListType m_RUBR_CONTATTI_LIST;
        private string m_DISAMB_PRE;
        private string m_FLG_IPA;
        private long m_ID_REFERRAL;
        private string m_NOME;
        private string m_RAGIONE_SOCIALE;
        private string m_SITO_WEB;
        private string m_COD_FIS;
        private RubricaAddressType m_RUBR_ADDRESS;
        private string m_UFFICIO;
        private string m_P_IVA;
        private long m_REF_ORG;
        private string m_IPA_ID;
        private string m_NOTE;
        private string m_COGNOME;
        private string m_IPA_DN;
        private short m_AFF_IPA;
        private long m_ID_PADRE;
        private string m_SRC;
        private long m_REF_ID_ADDRESS;

        private bool m_ID_REFERRALIsNull;
        private bool m_REF_ORGIsNull;
        private bool m_AFF_IPAIsNull;
        private bool m_ID_PADREIsNull;
        private bool m_REF_ID_ADDRESSIsNull;

        private string m_REFERRAL_TYPE;

        private string m_DISAMB_POST;
        #endregion

        #region "C.tor"
        public RubricaEntitaType()
        {
            // TODO : Add code to initialise the object
            this.m_ID_REFERRALIsNull = true;
            this.m_REF_ORGIsNull = true;
            this.m_AFF_IPAIsNull = true;
            this.m_ID_PADREIsNull = true;
            this.m_REF_ID_ADDRESSIsNull = true;
        }

        public RubricaEntitaType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public RubricaEntitaType(RubricaEntita re)
            : base(re)
        {
            if (re == null || re.IsValid == false)
            {
                this.m_IsNull = true;
                return;
            }

            this.m_AFF_IPAIsNull = !re.AffIPA.HasValue;
            this.m_ID_PADREIsNull = !re.IdPadre.HasValue;
            this.m_ID_REFERRALIsNull = !re.IdReferral.HasValue;
            this.m_REF_ID_ADDRESSIsNull = !re.RefIdAddress.HasValue;
            this.m_REF_ORGIsNull = !re.RefOrg.HasValue;

            this.m_FLG_IPA = Convert.ToInt32(re.IsIPA).ToString();
            if (re.Address is RubricaAddressType)
            {
                this.m_RUBR_ADDRESS = re.Address as RubricaAddressType;
            }
            else
            {
                this.m_RUBR_ADDRESS = new RubricaAddressType(re.Address);
            }

            RubricaContattiListType rubrContList;
            if (re.Contatti == null)
            {
                rubrContList = RubricaContattiListType.Null;
            }
            else
            {
                rubrContList = new RubricaContattiListType();
                rubrContList.RubricaContatti = re.Contatti.Select(c =>
                {
                    if (c is RubricaContattiType)
                    {
                        return (c as RubricaContattiType);
                    }
                    else
                    {
                        return new RubricaContattiType(c);
                    }
                }).ToArray();
            }

            this.m_REFERRAL_TYPE = re.ReferralType.ToString();
        }
        #endregion

        #region "Public Properties"
        public virtual bool IsNull
        {
            get
            {
                return this.m_IsNull;
            }
        }

        public static RubricaEntitaType Null
        {
            get
            {
                RubricaEntitaType obj = new RubricaEntitaType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("DISAMB_PRE")]
        public override string DisambPre
        {
            get
            {
                return this.m_DISAMB_PRE;
            }
            set
            {
                this.m_DISAMB_PRE = value;
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
                        base.IsIPA = Convert.ToBoolean(isIpa);
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
                if(value.HasValue)
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

        [OracleTypes.OracleObjectMappingAttribute("SITO_WEB")]
        public override string SitoWeb
        {
            get
            {
                return this.m_SITO_WEB;
            }
            set
            {
                this.m_SITO_WEB = value;
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

        [OracleTypes.OracleObjectMappingAttribute("RUBR_ADDRESS")]
        public RubricaAddressType RUBR_ADDRESS
        {
            get
            {
                return this.m_RUBR_ADDRESS;
            }
            set
            {
                this.m_RUBR_ADDRESS = value;

                if (value.IsNull == false)
                    base.Address = value;
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

        [OracleTypes.OracleObjectMappingAttribute("REF_ORG")]
        public override long? RefOrg
        {
            get
            {
                if (this.m_REF_ORGIsNull == true) return null;
                return this.m_REF_ORG;
            }
            set
            {
                this.m_REF_ORGIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_REF_ORG = (long)value;
            }
        }

        public bool REF_ORGIsNull
        {
            get
            {
                return this.m_REF_ORGIsNull;
            }
            set
            {
                this.m_REF_ORGIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("RUBR_CONTATTI_REFS")]
        public RubricaContattiRefListType RUBR_CONTATTI_REFS
        {
            get
            {
                return this.m_RUBR_CONTATTI_REFS;
            }
            set
            {
                this.m_RUBR_CONTATTI_REFS = value;
            }
        }

        public RubricaContattiListType RUBR_CONTATTI_LIST
        {
            get
            {
                return this.m_RUBR_CONTATTI_LIST;
            }
            set
            {
                this.m_RUBR_CONTATTI_LIST = value;
                if (value.IsNull == false)
                {
                    base.Contatti = value.RubricaContatti.Cast<RubricaContatti>().ToList();
                }
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("IPA_ID")]
        public override string IPAId
        {
            get
            {
                return this.m_IPA_ID;
            }
            set
            {
                this.m_IPA_ID = value;
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

        [OracleTypes.OracleObjectMappingAttribute("IPA_DN")]
        public override string IPAdn
        {
            get
            {
                return this.m_IPA_DN;
            }
            set
            {
                this.m_IPA_DN = value;
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

        [OracleTypes.OracleObjectMappingAttribute("ID_PADRE")]
        public override long? IdPadre
        {
            get
            {
                if (this.m_ID_PADREIsNull == true) return null;
                return this.m_ID_PADRE;
            }
            set
            {
                this.m_ID_PADREIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_ID_PADRE = (long)value;
            }
        }

        public bool ID_PADREIsNull
        {
            get
            {
                return this.m_ID_PADREIsNull;
            }
            set
            {
                this.m_ID_PADREIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("SRC")]
        public override string Src
        {
            get
            {
                return this.m_SRC;
            }
            set
            {
                this.m_SRC = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("REF_ID_ADDRESS")]
        public override long? RefIdAddress
        {
            get
            {
                if (this.m_REF_ID_ADDRESSIsNull == true) return null;
                return this.m_REF_ID_ADDRESS;
            }
            set
            {
                this.m_REF_ID_ADDRESSIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_REF_ID_ADDRESS = (long)value;
            }
        }

        public bool REF_ID_ADDRESSIsNull
        {
            get
            {
                return this.m_REF_ID_ADDRESSIsNull;
            }
            set
            {
                this.m_REF_ID_ADDRESSIsNull = value;
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

                if (!string.IsNullOrEmpty(value))
                    if (Enum.GetNames(typeof(EntitaType)).Contains(value, StringComparer.InvariantCultureIgnoreCase))
                        base.ReferralType = (EntitaType)Enum.Parse(typeof(EntitaType), value, true);
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("DISAMB_POST")]
        public override string DisambPost
        {
            get
            {
                return this.m_DISAMB_POST;
            }
            set
            {
                this.m_DISAMB_POST = value;
            }
        }
        #endregion

        #region "Public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, "DISAMB_PRE", this.DisambPre);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FLG_IPA", this.FLG_IPA);
            if ((ID_REFERRALIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_REFERRAL", this.IdReferral);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "NOME", this.Nome);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RAGIONE_SOCIALE", this.RagioneSociale);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "SITO_WEB", this.SitoWeb);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COD_FIS", this.CodiceFiscale);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RUBR_ADDRESS", this.RUBR_ADDRESS);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "UFFICIO", this.Ufficio);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "P_IVA", this.PartitaIVA);
            if ((REF_ORGIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "REF_ORG", this.RefOrg);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RUBR_CONTATTI_REFS", this.RUBR_CONTATTI_REFS);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "IPA_ID", this.IPAId);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "NOTE", this.Note);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "COGNOME", this.Cognome);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "IPA_DN", this.IPAdn);
            if ((AFF_IPAIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "AFF_IPA", this.AffIPA);
            }
            if ((ID_PADREIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_PADRE", this.IdPadre);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "SRC", this.Src);
            if ((REF_ID_ADDRESSIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "REF_ID_ADDRESS", this.RefIdAddress);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REFERRAL_TYPE", this.REFERRAL_TYPE);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "DISAMB_POST", this.DisambPost);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.DisambPre = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "DISAMB_PRE")));
            this.FLG_IPA = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FLG_IPA")));
            this.ID_REFERRALIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_REFERRAL");
            if ((ID_REFERRALIsNull == false))
            {
                this.IdReferral = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_REFERRAL")));
            }
            this.Nome = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "NOME")));
            this.RagioneSociale = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RAGIONE_SOCIALE")));
            this.SitoWeb = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "SITO_WEB")));
            this.CodiceFiscale = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COD_FIS")));
            this.RUBR_ADDRESS = ((RubricaAddressType)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RUBR_ADDRESS")));
            this.Ufficio = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "UFFICIO")));
            this.PartitaIVA = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "P_IVA")));
            this.REF_ORGIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "REF_ORG");
            if ((REF_ORGIsNull == false))
            {
                this.RefOrg = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REF_ORG")));
            }
            this.RUBR_CONTATTI_REFS = ((RubricaContattiRefListType)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RUBR_CONTATTI_REFS")));
            this.IPAId = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "IPA_ID")));
            this.Note = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "NOTE")));
            this.Cognome = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "COGNOME")));
            this.IPAdn = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "IPA_DN")));
            this.AFF_IPAIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "AFF_IPA");
            if ((AFF_IPAIsNull == false))
            {
                this.AffIPA = ((short)(OracleTypes.OracleUdt.GetValue(con, pUdt, "AFF_IPA")));
            }
            this.ID_PADREIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_PADRE");
            if ((ID_PADREIsNull == false))
            {
                this.IdPadre = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_PADRE")));
            }
            this.Src = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "SRC")));
            this.REF_ID_ADDRESSIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "REF_ID_ADDRESS");
            if ((REF_ID_ADDRESSIsNull == false))
            {
                this.RefIdAddress = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REF_ID_ADDRESS")));
            }
            this.REFERRAL_TYPE = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REFERRAL_TYPE")));
            this.DisambPost = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "DISAMB_POST")));
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

        public static RubricaEntitaType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new RubricaEntitaType();
        }

        public virtual void SetContatti(OracleClient.OracleConnection conn)
        {
            if (this.RUBR_CONTATTI_REFS.IsNull == true) return;

            List<RubricaContattiType> l = new List<RubricaContattiType>();
            foreach (string s in this.RUBR_CONTATTI_REFS.ListContattiRef)
            {
                OracleTypes.OracleRef r = new OracleTypes.OracleRef(conn, s);
                RubricaContattiType c = (RubricaContattiType)r.GetCustomObject(OracleTypes.OracleUdtFetchOption.Server);
                c.SetEntita(this);
                l.Add(c);
            }

            RubricaContattiListType rubrContList;
            if (l.Count == 0)
                rubrContList = RubricaContattiListType.Null;
            else
            {
                rubrContList = new RubricaContattiListType();
                rubrContList.RubricaContatti = l.ToArray();
            }
            this.RUBR_CONTATTI_LIST = rubrContList;
        }
        #endregion
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("RUBR_ENTITA_TYPE")]
    public class RubricaEntitaTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            RubricaEntitaType obj = new RubricaEntitaType();
            return obj;
        }
    }
}
