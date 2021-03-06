﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:2.0.50727.3634
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
    public class MappedAppsListType : OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        private bool m_IsNull;

        private long[] m_MAPPED_APPS_LIST_TYPE;

        private OracleTypes.OracleUdtStatus[] m_statusArray;

        public MappedAppsListType()
        {
            // TODO : Add code to initialise the object
        }

        public MappedAppsListType(string str)
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

        public static MappedAppsListType Null
        {
            get
            {
                MappedAppsListType obj = new MappedAppsListType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleArrayMappingAttribute()]
        public virtual long[] MappedApps
        {
            get
            {
                return this.m_MAPPED_APPS_LIST_TYPE;
            }
            set
            {
                this.m_MAPPED_APPS_LIST_TYPE = value;
            }
        }

        public virtual OracleTypes.OracleUdtStatus[] StatusArray
        {
            get
            {
                return this.m_statusArray;
            }
            set
            {
                this.m_statusArray = value;
            }
        }

        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            object objectStatusArray = ((object)(m_statusArray));
            OracleTypes.OracleUdt.SetValue(con, pUdt, 0, this.m_MAPPED_APPS_LIST_TYPE, objectStatusArray);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            object objectStatusArray = null;
            this.m_MAPPED_APPS_LIST_TYPE = ((long[])(OracleTypes.OracleUdt.GetValue(con, pUdt, 0, out objectStatusArray)));
            this.m_statusArray = ((OracleTypes.OracleUdtStatus[])(objectStatusArray));
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

        public static MappedAppsListType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new MappedAppsListType();
        }
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("MAPPED_APPS_LIST_TYPE")]
    public class MappedAppsListTypeFactory : OracleTypes.IOracleCustomTypeFactory, OracleTypes.IOracleArrayTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            MappedAppsListType obj = new MappedAppsListType();
            return obj;
        }

        public virtual System.Array CreateArray(int length)
        {
            Int64[] collElem = new Int64[length];
            return collElem;
        }

        public virtual System.Array CreateStatusArray(int length)
        {
            OracleTypes.OracleUdtStatus[] udtStatus = new OracleTypes.OracleUdtStatus[length];
            return udtStatus;
        }
    }
}
