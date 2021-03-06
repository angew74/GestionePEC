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
    using System.Linq;
    using System.Xml.Serialization;
    using System.Xml.Schema;
    using SendMail.Model.RubricaMapping;
    using System.Collections.Generic;
    using ActiveUp.Net.Common.UnisysExt;
    using SendMail.Model;
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;

    public class RubricaContattiType : RubricaContatti, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        #region "Private Fields"
        private bool m_IsNull;
        private string m_ENTITA_REF;
        private RubricaEntitaType m_RUBR_ENTITA;
        private string m_TIPO_REF;
        private MappedAppsListType m_MAPPED_APPS;
        private string m_FAX;
        private string m_TELEFONO;
        private string m_FLG_IPA;
        private string m_IPA_DN;
        private long m_ID_CONTACT;
        private string m_SOURCE;
        private string m_MAIL;
        private short m_FLG_PEC;
        private long m_REF_ID_REFERRAL;
        private short m_AFF_IPA;
        private string m_NOTE;
        private string m_IPA_ID;
        private string m_CONTACT_REF;
        private string m_SRC;

        private bool m_ID_CONTACTIsNull;
        private bool m_FLG_PECIsNull;
        private bool m_REF_ID_REFERRALIsNull;
        private bool m_AFF_IPAIsNull;
        #endregion


        #region "C.tor"
        public RubricaContattiType()
        {
            // TODO : Add code to initialise the object
            this.m_ID_CONTACTIsNull = true;
            this.m_FLG_PECIsNull = true;
            this.m_REF_ID_REFERRALIsNull = true;
            this.m_AFF_IPAIsNull = true;
        }

        public RubricaContattiType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public RubricaContattiType(RubricaContatti r)
            : base(r)
        {
            if (r == null || r.IsValid == false) return;

            this.AFF_IPAIsNull = !(r.AffIPA.HasValue);
            this.ID_CONTACTIsNull = !(r.IdContact.HasValue);
            this.REF_ID_REFERRALIsNull = !(r.RefIdReferral.HasValue);

            this.m_FLG_IPA = Convert.ToInt32(r.IsIPA).ToString();
            this.m_FLG_PEC = Convert.ToInt16(r.IsPec);
            this.m_FLG_PECIsNull = false;
            this.m_TIPO_REF = r.TipoContatto.ToString();
            if (r.MappedAppsId == null)
            {
                this.m_MAPPED_APPS = MappedAppsListType.Null;
            }
            else
            {
                this.m_MAPPED_APPS = new MappedAppsListType();
                this.m_MAPPED_APPS.MappedApps = r.MappedAppsId.ToArray();
                this.m_MAPPED_APPS.StatusArray = new OracleTypes.OracleUdtStatus[r.MappedAppsId.Count];
                Array.ForEach(this.m_MAPPED_APPS.StatusArray, x => x = OracleTypes.OracleUdtStatus.NotNull);
            }
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

        public static RubricaContattiType Null
        {
            get
            {
                RubricaContattiType obj = new RubricaContattiType();
                obj.m_IsNull = true;
                return obj;
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

        [OracleTypes.OracleObjectMappingAttribute("ID_CONTACT")]
        public override long? IdContact
        {
            get
            {
                if (this.m_ID_CONTACTIsNull == true) return null;
                return this.m_ID_CONTACT;
            }
            set
            {
                this.m_ID_CONTACTIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_ID_CONTACT = (long)value;
            }
        }

        public bool ID_CONTACTIsNull
        {
            get
            {
                return this.m_ID_CONTACTIsNull;
            }
            set
            {
                this.m_ID_CONTACTIsNull = value;
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

        [OracleTypes.OracleObjectMappingAttribute("FLG_PEC")]
        public short FLG_PEC
        {
            get
            {
                return this.m_FLG_PEC;
            }
            set
            {
                this.m_FLG_PEC = value;
                this.m_FLG_PECIsNull = false;
                base.IsPec = Convert.ToBoolean(value);
            }
        }

        public bool FLG_PECIsNull
        {
            get
            {
                return this.m_FLG_PECIsNull;
            }
            set
            {
                this.m_FLG_PECIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("TIPO_REF")]
        public string TIPO_REF
        {
            get
            {
                return this.m_TIPO_REF;
            }
            set
            {
                this.m_TIPO_REF = value;

                AddresseeType tipoCont = AddresseeType.UNDEFINED;
                if (!String.IsNullOrEmpty(value))
                    if (Enum.GetNames(typeof(AddresseeType)).Contains(value, StringComparer.InvariantCultureIgnoreCase))
                        tipoCont = (AddresseeType)Enum.Parse(typeof(AddresseeType), value, true);
                base.TipoContatto = tipoCont;
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

        [OracleTypes.OracleObjectMappingAttribute("REF_ID_REFERRAL")]
        public override long? RefIdReferral
        {
            get
            {
                if (this.m_REF_ID_REFERRALIsNull == true) return null;
                return this.m_REF_ID_REFERRAL;
            }
            set
            {
                this.m_REF_ID_REFERRALIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_REF_ID_REFERRAL = (long)value;
            }
        }

        public bool REF_ID_REFERRALIsNull
        {
            get
            {
                return this.m_REF_ID_REFERRALIsNull;
            }
            set
            {
                this.m_REF_ID_REFERRALIsNull = value;
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

        [OracleTypes.OracleObjectMapping("SRC")]
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

        [OracleTypes.OracleObjectMappingAttribute("MAPPED_APPS")]
        public MappedAppsListType MAPPED_APPS
        {
            get
            {
                return this.m_MAPPED_APPS;
            }
            set
            {
                this.m_MAPPED_APPS = value;

                if (value.IsNull)
                {
                    base.MappedAppsId = null;
                }
                else
                {
                    base.MappedAppsId = new List<long>();
                    for (int i = 0; i < value.MappedApps.Length; i++)
                    {
                        if (value.StatusArray[i] == OracleTypes.OracleUdtStatus.NotNull)
                        {
                            base.MappedAppsId.Add(value.MappedApps[i]);
                        }
                    }
                }
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

        [OracleTypes.OracleObjectMappingAttribute("ENTITA_REF")]
        public string ENTITA_REF
        {
            get
            {
                return this.m_ENTITA_REF;
            }
            set
            {
                this.m_ENTITA_REF = value;
            }
        }

        public RubricaEntitaType RUBR_ENTITA
        {
            get { return this.m_RUBR_ENTITA; }
            set
            {
                this.m_RUBR_ENTITA = value;
                if (value.IsNull == false)
                {
                    base.Entita = (RubricaEntita)value;
                }
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("SOURCE")]
        public override string Source
        {
            get
            {
                return this.m_SOURCE;
            }
            set
            {
                this.m_SOURCE = value;
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

        #region "Public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FLG_IPA", this.FLG_IPA);
            if ((ID_CONTACTIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_CONTACT", this.IdContact);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FAX", this.Fax);
            if ((FLG_PECIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "FLG_PEC", this.FLG_PEC);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "TIPO_REF", this.TIPO_REF);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL", this.Mail);
            if ((REF_ID_REFERRALIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "REF_ID_REFERRAL", this.RefIdReferral);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "IPA_ID", this.IPAId);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "TELEFONO", this.Telefono);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "NOTE", this.Note);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAPPED_APPS", this.MAPPED_APPS);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "IPA_DN", this.IPAdn);
            if ((AFF_IPAIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "AFF_IPA", this.AffIPA);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "ENTITA_REF", this.ENTITA_REF);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "SOURCE", this.Source);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "CONTACT_REF", this.ContactRef);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "SRC", this.Src);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.FLG_IPA = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FLG_IPA")));
            this.ID_CONTACTIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_CONTACT");
            if ((ID_CONTACTIsNull == false))
            {
                this.IdContact = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_CONTACT")));
            }
            this.Fax = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FAX")));
            this.FLG_PECIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "FLG_PEC");
            if ((FLG_PECIsNull == false))
            {
                this.FLG_PEC = ((short)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FLG_PEC")));
            }
            this.TIPO_REF = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "TIPO_REF")));
            this.Mail = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL")));
            this.REF_ID_REFERRALIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "REF_ID_REFERRAL");
            if ((REF_ID_REFERRALIsNull == false))
            {
                this.RefIdReferral = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REF_ID_REFERRAL")));
            }
            this.IPAId = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "IPA_ID")));
            this.Telefono = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "TELEFONO")));
            this.Note = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "NOTE")));
            this.MAPPED_APPS = ((MappedAppsListType)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAPPED_APPS")));
            this.IPAdn = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "IPA_DN")));
            this.AFF_IPAIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "AFF_IPA");
            if ((AFF_IPAIsNull == false))
            {
                this.AffIPA = ((short)(OracleTypes.OracleUdt.GetValue(con, pUdt, "AFF_IPA")));
            }
            this.ENTITA_REF = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ENTITA_REF")));
            this.Source = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "SOURCE")));
            this.ContactRef = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CONTACT_REF")));
            this.Src = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "SRC")));
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

        public static RubricaContattiType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new RubricaContattiType();
        }

        public void SetEntita(RubricaEntitaType re)
        {
            this.Entita = re;
        }

        public void SetEntita(OracleClient.OracleConnection conn)
        {
            OracleTypes.OracleRef or = new OracleTypes.OracleRef(conn, this.m_ENTITA_REF);
            RubricaEntitaType re = (RubricaEntitaType)or.GetCustomObject(OracleTypes.OracleUdtFetchOption.Server);
            re.SetContatti(conn);

            Array.ForEach(re.RUBR_CONTATTI_LIST.RubricaContatti, x =>
            {
                if (x.IdContact == this.IdContact)
                    x = this;
            });
            this.RUBR_ENTITA = re;
        }
        #endregion
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("RUBR_CONTATTI_TYPE")]
    public class RubricaContattiTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            RubricaContattiType obj = new RubricaContattiType();
            return obj;
        }
    }
}
