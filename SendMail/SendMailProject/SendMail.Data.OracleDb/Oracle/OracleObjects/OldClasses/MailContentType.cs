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
    using System.Linq;
    using System.Collections.Generic;
    using ActiveUp.Net.Common.UnisysExt;
    using OracleTypes = global::Oracle.DataAccess.Types;
    using OracleClient = global::Oracle.DataAccess.Client;

    public class MailContentType : MailContent, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {
        #region "Private Fields"
        private bool m_IsNull;

        private string m_MAIL_TEXT;

        private MailRefsListType m_MAIL_REFS;

        private string m_MAIL_SENDER;

        private string m_FLG_ANNULLAMENTO;

        private long m_ID_MAIL;

        private bool m_ID_MAILIsNull;

        private string m_MAIL_SUBJECT;

        private long m_REF_ID_COM;

        private bool m_REF_ID_COMIsNull;

        private string m_FLG_CUSTOM_REFS;

        private bool m_FOLLOWSIsNull;

        private long m_FOLLOWS;
        #endregion

        #region "C.tor"
        public MailContentType()
        {
            // TODO : Add code to initialise the object
            this.m_ID_MAILIsNull = true;
            this.m_REF_ID_COMIsNull = true;
            this.m_FOLLOWSIsNull = true;
        }

        public MailContentType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public MailContentType(MailContent m)
            : base(m)
        {
            if (m == null || m.IsValid == false)
            {
                this.m_IsNull = true;
                return;
            }

            this.m_ID_MAILIsNull = !m.IdMail.HasValue;
            this.m_FOLLOWSIsNull = !m.Follows.HasValue;
            this.m_REF_ID_COMIsNull = !m.RefIdComunicazione.HasValue;
            this.m_FLG_ANNULLAMENTO = Convert.ToInt32(m.IsCancelled).ToString();
            this.m_FLG_CUSTOM_REFS = Convert.ToInt32(m.HasCustomRefs).ToString();

            if (m.MailRefs == null)
            {
                this.m_MAIL_REFS = MailRefsListType.Null;
            }
            else
            {
                this.m_MAIL_REFS = new MailRefsListType();
                this.m_MAIL_REFS.MailRefs = m.MailRefs.Select(mr => (MailRefsType)mr).ToArray();
            }
        }
        #endregion

        #region "public Properties"
        public virtual bool IsNull
        {
            get
            {
                return this.m_IsNull;
            }
        }

        public static MailContentType Null
        {
            get
            {
                MailContentType obj = new MailContentType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("MAIL_TEXT")]
        public override string MailText
        {
            get
            {
                return this.m_MAIL_TEXT;
            }
            set
            {
                this.m_MAIL_TEXT = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("MAIL_REFS")]
        public MailRefsListType MAIL_REFS
        {
            get
            {
                return this.m_MAIL_REFS;
            }
            set
            {
                this.m_MAIL_REFS = value;
                if (this.m_MAIL_REFS.IsNull == false)
                {
                    base.MailRefs = this.m_MAIL_REFS.MailRefs.Cast<SendMail.Model.ComunicazioniMapping.MailRefs>().ToList();
                }
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("MAIL_SENDER")]
        public override string MailSender
        {
            get
            {
                return this.m_MAIL_SENDER;
            }
            set
            {
                this.m_MAIL_SENDER = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("FLG_ANNULLAMENTO")]
        public string FLG_ANNULLAMENTO
        {
            get
            {
                return this.m_FLG_ANNULLAMENTO;
            }
            set
            {
                this.m_FLG_ANNULLAMENTO = value;
                int isCancelled = 0;
                if (!String.IsNullOrEmpty(value) &&
                    int.TryParse(value, out isCancelled))
                {
                    base.IsCancelled = Convert.ToBoolean(isCancelled);
                }
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("ID_MAIL")]
        public override long? IdMail
        {
            get
            {
                if (this.m_ID_MAILIsNull == true)
                    return null;
                return this.m_ID_MAIL;
            }
            set
            {
                this.m_ID_MAILIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_ID_MAIL = (long)value;
            }
        }

        public bool ID_MAILIsNull
        {
            get
            {
                return this.m_ID_MAILIsNull;
            }
            set
            {
                this.m_ID_MAILIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("MAIL_SUBJECT")]
        public override string MailSubject
        {
            get
            {
                return this.m_MAIL_SUBJECT;
            }
            set
            {
                this.m_MAIL_SUBJECT = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("REF_ID_COM")]
        public override long? RefIdComunicazione
        {
            get
            {
                if (this.m_REF_ID_COMIsNull == true)
                    return null;
                return this.m_REF_ID_COM;
            }
            set
            {
                this.m_REF_ID_COMIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_REF_ID_COM = (long)value;
            }
        }

        public bool REF_ID_COMIsNull
        {
            get
            {
                return this.m_REF_ID_COMIsNull;
            }
            set
            {
                this.m_REF_ID_COMIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("FLG_CUSTOM_REFS")]
        public string FLG_CUSTOM_REFS
        {
            get
            {
                return this.m_FLG_CUSTOM_REFS;
            }
            set
            {
                this.m_FLG_CUSTOM_REFS = value;

                int hasCustomRefs = 0;
                if (!String.IsNullOrEmpty(value) &&
                    int.TryParse(value, out hasCustomRefs))
                {
                    base.HasCustomRefs = Convert.ToBoolean(hasCustomRefs);
                }
            }
        }

        public bool FOLLOWSIsNull
        {
            get
            {
                return this.m_FOLLOWSIsNull;
            }
            set
            {
                this.m_FOLLOWSIsNull = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("FOLLOWS")]
        public override long? Follows
        {
            get
            {
                if (this.m_FOLLOWSIsNull == true)
                    return null;
                return this.m_FOLLOWS;
            }
            set
            {
                this.m_FOLLOWSIsNull = !(value.HasValue);
                if (value.HasValue)
                    this.m_FOLLOWS = (long)value;
            }
        }
        #endregion

        #region "Public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL_TEXT", this.MailText);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL_REFS", this.MailRefs);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL_SENDER", this.MailSender);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FLG_ANNULLAMENTO", this.FLG_ANNULLAMENTO);
            if ((ID_MAILIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_MAIL", this.IdMail);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL_SUBJECT", this.MailSubject);
            if ((REF_ID_COMIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "REF_ID_COM", this.RefIdComunicazione);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "FLG_CUSTOM_REFS", this.FLG_CUSTOM_REFS);
            if ((FOLLOWSIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "FOLLOWS", this.Follows);
            }
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.MailText = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL_TEXT")));
            this.MAIL_REFS = ((MailRefsListType)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL_REFS")));
            this.MailSender = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL_SENDER")));
            this.FLG_ANNULLAMENTO = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FLG_ANNULLAMENTO")));
            this.ID_MAILIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_MAIL");
            if ((ID_MAILIsNull == false))
            {
                this.IdMail = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_MAIL")));
            }
            this.MailSubject = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL_SUBJECT")));
            this.REF_ID_COMIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "REF_ID_COM");
            if ((REF_ID_COMIsNull == false))
            {
                this.RefIdComunicazione = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REF_ID_COM")));
            }
            this.FLG_CUSTOM_REFS = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FLG_CUSTOM_REFS")));
            this.FOLLOWSIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "FOLLOWS");
            if ((FOLLOWSIsNull == false))
            {
                this.Follows = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "FOLLOWS")));
            }
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

        public static MailContentType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new MailContentType();
        }
        #endregion

        #region "Commentato"
        /*
         [OracleObjectMappingAttribute("MAIL_TEXT")]
        public string oMAIL_TEXT
        {
            get
            {
                return this.m_MAIL_TEXT;
            }
            set
            {
                this.m_MAIL_TEXT = value;
            }
        }

        [OracleObjectMappingAttribute("MAIL_REFS")]
        public MailRefsListType oMAIL_REFS
        {
            get
            {
                if (this.m_MAIL_REFS == null)
                    return MailRefsListType.Null;
                else
                {
                    MailRefsListType mr = new MailRefsListType();
                    if (this.m_MAIL_REFS.All(m => m is MailRefsType))
                    {
                        mr.MailRefs = this.m_MAIL_REFS.Cast<MailRefsType>().ToArray();
                    }
                    else
                    {
                        mr.MailRefs = this.m_MAIL_REFS
                                        .Select<MailRefs, MailRefsType>(a => new MailRefsType()
                                        {
                                            oMAIL_DESTINATARIO = a.MailDestinatario,
                                            oTIPO_REF = a.TipoRef.ToString()
                                        }).ToArray();
                    }
                    return mr;
                }
            }
            set
            {
                if (value.IsNull)
                    this.m_MAIL_REFS = null;
                else
                    this.m_MAIL_REFS = value.MailRefs.Cast<MailRefs>().ToList();
            }
        }

        [OracleObjectMappingAttribute("MAIL_SENDER")]
        public string oMAIL_SENDER
        {
            get
            {
                return this.m_MAIL_SENDER;
            }
            set
            {
                this.m_MAIL_SENDER = value;
            }
        }

        [OracleObjectMappingAttribute("FLG_ANNULLAMENTO")]
        public string oFLG_ANNULLAMENTO
        {
            get
            {
                return this.m_FLG_ANNULLAMENTO;
            }
            set
            {
                this.m_FLG_ANNULLAMENTO = value;
            }
        }

        [OracleObjectMappingAttribute("ID_MAIL")]
        public long oID_MAIL
        {
            get
            {
                return this.m_ID_MAIL;
            }
            set
            {
                this.m_ID_MAIL = value;
            }
        }

        public bool ID_MAILIsNull
        {
            get
            {
                return this.m_ID_MAILIsNull;
            }
            set
            {
                this.m_ID_MAILIsNull = value;
            }
        }

        [OracleObjectMappingAttribute("MAIL_SUBJECT")]
        public string oMAIL_SUBJECT
        {
            get
            {
                return this.m_MAIL_SUBJECT;
            }
            set
            {
                this.m_MAIL_SUBJECT = value;
            }
        }

        [OracleObjectMappingAttribute("REF_ID_COM")]
        public long oREF_ID_COM
        {
            get
            {
                return this.m_REF_ID_COM;
            }
            set
            {
                this.m_REF_ID_COM = value;
            }
        }

        public bool REF_ID_COMIsNull
        {
            get
            {
                return this.m_REF_ID_COMIsNull;
            }
            set
            {
                this.m_REF_ID_COMIsNull = value;
            }
        }

        [OracleObjectMappingAttribute("FLG_CUSTOM_REFS")]
        public string oFLG_CUSTOM_REFS
        {
            get
            {
                return this.m_FLG_CUSTOM_REFS;
            }
            set
            {
                this.m_FLG_CUSTOM_REFS = value;
            }
        }

        public bool FOLLOWSIsNull
        {
            get
            {
                return this.m_FOLLOWSIsNull;
            }
            set
            {
                this.m_FOLLOWSIsNull = value;
            }
        }

        [OracleObjectMappingAttribute("FOLLOWS")]
        public long oFOLLOWS
        {
            get
            {
                return this.m_FOLLOWS;
            }
            set
            {
                this.m_FOLLOWS = value;
            }
        }
         */
        #endregion
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("MAIL_CONTENT_TYPE")]
    public class MailContentTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            MailContentType obj = new MailContentType();
            return obj;
        }
    }

    public class MailRefsType : MailRefs, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {
        #region "Private Fields"
        private bool m_IsNull;

        private string m_TIPO_REF;

        private string m_MAIL_DESTINATARIO;
        #endregion

        #region "C.tor"
        public MailRefsType()
        {
            // TODO : Add code to initialise the object
        }

        public MailRefsType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public MailRefsType(MailRefs mr)
            : base(mr)
        {
            if (mr == null || mr.IsValid == false)
            {
                this.m_IsNull = true;
                return;
            }

            this.m_TIPO_REF = mr.TipoRef.ToString();
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

        public static MailRefsType Null
        {
            get
            {
                MailRefsType obj = new MailRefsType();
                obj.m_IsNull = true;
                return obj;
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

                AddresseeType tipoRef = AddresseeType.UNDEFINED;
                if (!String.IsNullOrEmpty(value))
                    if (Enum.GetNames(typeof(AddresseeType)).Contains(value, StringComparer.InvariantCultureIgnoreCase))
                        tipoRef = (AddresseeType)Enum.Parse(typeof(AddresseeType), value, true);

                base.TipoRef = tipoRef;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("MAIL_DESTINATARIO")]
        public override string MailDestinatario
        {
            get
            {
                return this.m_MAIL_DESTINATARIO;
            }
            set
            {
                this.m_MAIL_DESTINATARIO = value;
            }
        }
        #endregion

        #region "Public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, "TIPO_REF", this.TIPO_REF);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "MAIL_DESTINATARIO", this.MailDestinatario);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.TIPO_REF = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "TIPO_REF")));
            this.MailDestinatario = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "MAIL_DESTINATARIO")));
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

        public static MailRefsType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new MailRefsType();
        }
        #endregion
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("MAIL_REFS_TYPE")]
    public class MailRefsTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            MailRefsType obj = new MailRefsType();
            return obj;
        }
    }

    public class MailRefsListType : OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        private bool m_IsNull;

        private MailRefsType[] m_MAIL_REFS_TYPE;

        public MailRefsListType()
        {
            // TODO : Add code to initialise the object
        }

        public MailRefsListType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public virtual bool IsNull
        {
            get
            {
                return this.m_IsNull;
            }
        }

        public static MailRefsListType Null
        {
            get
            {
                MailRefsListType obj = new MailRefsListType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleArrayMappingAttribute()]
        public virtual MailRefsType[] MailRefs
        {
            get
            {
                return this.m_MAIL_REFS_TYPE;
            }
            set
            {
                this.m_MAIL_REFS_TYPE = value;
            }
        }

        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, 0, this.m_MAIL_REFS_TYPE);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.m_MAIL_REFS_TYPE = ((MailRefsType[])(OracleTypes.OracleUdt.GetValue(con, pUdt, 0)));
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

        public static MailRefsListType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new MailRefsListType();
        }
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("MAIL_REFS_LIST_TYPE")]
    public class MailRefsListTypeFactory : OracleTypes.IOracleCustomTypeFactory, OracleTypes.IOracleArrayTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            MailRefsListType obj = new MailRefsListType();
            return obj;
        }

        public virtual System.Array CreateArray(int length)
        {
            MailRefsType[] collElem = new MailRefsType[length];
            return collElem;
        }

        public virtual System.Array CreateStatusArray(int length)
        {
            return null;
        }
    }
}