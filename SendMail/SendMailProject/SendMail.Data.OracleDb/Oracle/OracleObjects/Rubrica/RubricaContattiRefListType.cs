namespace SendMail.Data.OracleDb.OracleObjects
{
    using System;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Xml.Schema;
    using SendMail.Model.RubricaMapping;
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;

    public class RubricaContattiRefListType : OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        private bool m_IsNull;

        //private OracleRef[] m_LIST_CONTATTI;
        private String[] m_LIST_CONTATTI_REF;

        public RubricaContattiRefListType()
        {

        }

        public RubricaContattiRefListType(string str)
        {

        }

        #region INullable Membri di

        public virtual bool IsNull
        {
            get { return this.m_IsNull; }
        }

        #endregion

        public static RubricaContattiRefListType Null
        {
            get
            {
                RubricaContattiRefListType obj = new RubricaContattiRefListType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleArrayMappingAttribute()]
        public virtual String[] ListContattiRef
        {
            get { return this.m_LIST_CONTATTI_REF; }
            set { this.m_LIST_CONTATTI_REF = value; }
        }

        #region IOracleCustomType Membri di

        public virtual void FromCustomObject(OracleClient.OracleConnection con, IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, 0, this.m_LIST_CONTATTI_REF);
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, IntPtr pUdt)
        {
            this.m_LIST_CONTATTI_REF = ((String[])(OracleTypes.OracleUdt.GetValue(con, pUdt, 0)));
        }

        #endregion

        #region IXmlSerializable Membri di

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
        }

        #endregion

        public override string ToString()
        {
            return "";
        }

        public static RubricaContattiRefListType Parse(string str)
        {
            return new RubricaContattiRefListType();
        }
    }

    [OracleTypes.OracleCustomTypeMappingAttribute("RUBR_CONTATTI_REF_LIST_TYPE")]
    public class RubricaContattiRefListTypeFactory : OracleTypes.IOracleCustomTypeFactory, OracleTypes.IOracleArrayTypeFactory
    {
        #region IOracleCustomTypeFactory Membri di

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            RubricaContattiRefListType obj = new RubricaContattiRefListType();
            return obj;
        }

        #endregion

        #region IOracleArrayTypeFactory Membri di

        public Array CreateArray(int numElems)
        {
            //OracleRef[] collElem = new OracleRef[numElems];
            String[] collElem = new string[numElems];
            return collElem;
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }

        #endregion
    }
}
