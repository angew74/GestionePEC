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
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;

    [Serializable]
    public class RubricaContattiListType : OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        private bool m_IsNull;

        private RubricaContattiType[] m_RUBR_CONTATTI_TYPE;

        public RubricaContattiListType()
        {
            // TODO : Add code to initialise the object
        }

        public RubricaContattiListType(string str)
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

        public static RubricaContattiListType Null
        {
            get
            {
                RubricaContattiListType obj = new RubricaContattiListType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleArrayMappingAttribute()]
        public virtual RubricaContattiType[] RubricaContatti
        {
            get
            {
                return this.m_RUBR_CONTATTI_TYPE;
            }
            set
            {
                this.m_RUBR_CONTATTI_TYPE = value;
            }
        }

        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, 0, this.m_RUBR_CONTATTI_TYPE);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.m_RUBR_CONTATTI_TYPE = ((RubricaContattiType[])(OracleTypes.OracleUdt.GetValue(con, pUdt, 0)));
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

        public static RubricaContattiListType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new RubricaContattiListType();
        }
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("RUBR_CONTATTI_LIST_TYPE")]
    public class RubricaContattiListTypeFactory : OracleTypes.IOracleCustomTypeFactory, OracleTypes.IOracleArrayTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            RubricaContattiListType obj = new RubricaContattiListType();
            return obj;
        }

        public virtual System.Array CreateArray(int length)
        {
            RubricaContattiType[] collElem = new RubricaContattiType[length];
            return collElem;
        }

        public virtual System.Array CreateStatusArray(int length)
        {
            return null;
        }
    }
}
