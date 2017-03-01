using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Data.OracleDb.OracleObjects
{
    using System;
    using System.Xml.Serialization;
    using System.Xml.Schema;
    using OracleTypes = global::Oracle.DataAccess.Types;
    using OracleClient = global::Oracle.DataAccess.Client;

   public class ComProtocolloListType : OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        private bool m_IsNull;

        private ComProtocolloType[] m_COM_PROTOCOLLO_TYPE;

        public ComProtocolloListType()
        {
            // TODO : Add code to initialise the object
        }

        public ComProtocolloListType(string str)
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

        public static ComProtocolloListType Null
        {
            get
            {
                ComProtocolloListType obj = new ComProtocolloListType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleArrayMappingAttribute()]
        public virtual ComProtocolloType[] ComProtocolli
        {
            get
            {
                return this.m_COM_PROTOCOLLO_TYPE;
            }
            set
            {
                this.m_COM_PROTOCOLLO_TYPE = value;
            }
        }

        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, 0, this.m_COM_PROTOCOLLO_TYPE);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.m_COM_PROTOCOLLO_TYPE = ((ComProtocolloType[])(OracleTypes.OracleUdt.GetValue(con, pUdt, 0)));
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

        public static ComProtocolloListType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new ComProtocolloListType();
        }
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("COM_PROTOCOLLO_LIST_TYPE")]
    public class ComProtocolloListTypeFactory : OracleTypes.IOracleCustomTypeFactory, OracleTypes.IOracleArrayTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            ComProtocolloListType obj = new ComProtocolloListType();
            return obj;
        }

        public virtual System.Array CreateArray(int length)
        {
            ComProtocolloType[] collElem = new ComProtocolloType[length];
            return collElem;
        }

        public virtual System.Array CreateStatusArray(int length)
        {
            return null;
        }
    }
}
