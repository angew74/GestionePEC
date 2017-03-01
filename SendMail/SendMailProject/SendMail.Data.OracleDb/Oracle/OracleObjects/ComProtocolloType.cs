

namespace SendMail.Data.OracleDb.OracleObjects
{
    using System;
    using System.Xml.Serialization;
    using System.Xml.Schema;
    using SendMail.Model.ComunicazioniMapping;
    using SendMail.Model;
    using OracleTypes = global::Oracle.DataAccess.Types;
    using OracleClient = global::Oracle.DataAccess.Client;

   public class ComProtocolloType : ComunicazioniProtocollo, OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {
          #region "Private Fields"
        private bool m_IsNull;
        private  bool m_REF_ID_COMIsNull;
        private long m_REF_ID_COM;
        private string m_REQ_PROT_TIPO;
        private string m_REQ_COD_DOCUMENTO;
        private string m_REQ_IS_ESISTENTE;
        private string m_REQ_IS_RISERVATO;
        private string m_REQ_SUBJECT;
        private string m_RESP_PROT_TIPO;
        private bool m_RESP_PROT_ANNOIsNull;
        private short m_RESP_PROT_ANNO;
        private string m_RESP_PROT_NUMERO;
        private string m_PROT_IN_OUT;
        #endregion

        #region "C.tor"
        public ComProtocolloType()
        {
            // TODO : Add code to initialise the object          
            this.m_REF_ID_COMIsNull = true;
        }

        public ComProtocolloType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }

        public ComProtocolloType(ComunicazioniProtocollo a)
            : base(a)
        {
            if (a == null || a.IsValid == false)
            {
                this.m_IsNull = true;
                return;
            }
         
            this.m_REF_ID_COMIsNull = !a.RefIdCom.HasValue;

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

        public static ComProtocolloType Null
        {
            get
            {
                ComProtocolloType obj = new ComProtocolloType();
                obj.m_IsNull = true;
                return obj;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        [OracleTypes.OracleObjectMappingAttribute("REF_ID_COM")]
        public override Nullable<Int64> RefIdCom
        {
            get
            {
                if (m_REF_ID_COMIsNull == true)
                    return null;
                else return m_REF_ID_COM;
            }
            set
            {
                if (value.HasValue == true)
                {
                    m_REF_ID_COMIsNull = false;
                    m_REF_ID_COM = value.Value;
                }
                else m_REF_ID_COMIsNull = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [OracleTypes.OracleObjectMappingAttribute("REQ_PROT_TIPO")]
        public override string RequestTipoProtocollo
        {
            get { return m_REQ_PROT_TIPO; }
            set { m_REQ_PROT_TIPO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
          [OracleTypes.OracleObjectMappingAttribute("REQ_COD_DOCUMENTO")]
        public override string RequestCodiceDocumento
        {
            get { return m_REQ_COD_DOCUMENTO; }
            set { m_REQ_COD_DOCUMENTO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
         [OracleTypes.OracleObjectMappingAttribute("REQ_IS_ESISTENTE")]
        public override int? RequestIsEsistente
        {
            get
            {
                int? reqIsEsistente = null;
                if (!String.IsNullOrEmpty(m_REQ_IS_ESISTENTE))
                {
                    int x = -1;
                    if (int.TryParse(m_REQ_IS_ESISTENTE, out x))
                        reqIsEsistente = x;
                }
                return reqIsEsistente;
            }
            set
            {
                m_REQ_IS_ESISTENTE = Convert.ToInt32(value).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [OracleTypes.OracleObjectMappingAttribute("REQ_IS_RISERVATO")]
        public override int? RequestIsRiservato
        {
            get
            {
                int? reqIsRiservato = null;
                if (!String.IsNullOrEmpty(m_REQ_IS_RISERVATO))
                {
                    int x = -1;
                    if (int.TryParse(m_REQ_IS_RISERVATO, out x))
                        reqIsRiservato = x;
                }
                return reqIsRiservato;
            }
            set
            {
                m_REQ_IS_RISERVATO = Convert.ToInt32(value).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [OracleTypes.OracleObjectMappingAttribute("REQ_SUBJECT")]
        public override string RequestSubject
        {
            get { return m_REQ_SUBJECT; }
            set { m_REQ_SUBJECT = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [OracleTypes.OracleObjectMappingAttribute("RESP_PROT_TIPO")]
        public override string ResponseProtocolloTipo
        {
            get { return m_RESP_PROT_TIPO; }
            set { m_RESP_PROT_TIPO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        [OracleTypes.OracleObjectMappingAttribute("RESP_PROT_ANNO")]
        public override Nullable<Int16> ResponseProtocolloAnno
        {
            get
            {
                if (m_RESP_PROT_ANNOIsNull == true)
                    return null;
                else return m_RESP_PROT_ANNO;
            }
            set
            {
                if (value.HasValue == true)
                {
                    m_RESP_PROT_ANNOIsNull = false;
                    m_RESP_PROT_ANNO = value.Value;
                }
                else
                    m_RESP_PROT_ANNOIsNull = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [OracleTypes.OracleObjectMappingAttribute("RESP_PROT_NUMERO")]
        public override string ResponseProtocolloNumero
        {
            get { return m_RESP_PROT_NUMERO; }
            set { m_RESP_PROT_NUMERO = value; }
        }

         [OracleTypes.OracleObjectMappingAttribute("PROT_IN_OUT")]
        public override string ProtocolloInOut
        {
            get { return m_PROT_IN_OUT; }
            set { m_PROT_IN_OUT = value; }
        }

        #endregion

        #region "Public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            if ((m_REF_ID_COMIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "REF_ID_COM", this.RefIdCom);
            }
            OracleTypes.OracleUdt.SetValue(con, pUdt, "PROT_IN_OUT", this.ProtocolloInOut);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RESP_PROT_NUMERO", this.ResponseProtocolloNumero);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RESP_PROT_ANNO", this.ResponseProtocolloAnno);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "RESP_PROT_TIPO", this.ResponseProtocolloTipo);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REQ_SUBJECT", this.RequestSubject);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REQ_IS_RISERVATO", this.RequestIsRiservato);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REQ_IS_ESISTENTE", this.RequestIsEsistente);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REQ_COD_DOCUMENTO", this.RequestCodiceDocumento);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "REQ_PROT_TIPO", this.RequestTipoProtocollo);           

        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {       
          
            this.m_REF_ID_COMIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "REF_ID_COM");
            if ((m_REF_ID_COMIsNull == false))
            {
                this.RefIdCom = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REF_ID_COM")));
            }
            this.ResponseProtocolloTipo = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RESP_PROT_TIPO")));
            this.RequestSubject = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REQ_SUBJECT")));
            this.RequestIsRiservato = ((int?)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REQ_IS_RISERVATO")));
            this.RequestIsEsistente = ((int?)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REQ_IS_ESISTENTE")));
            this.RequestCodiceDocumento = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REQ_COD_DOCUMENTO")));
            this.RequestTipoProtocollo = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REQ_PROT_TIPO")));
            this.ProtocolloInOut = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "PROT_IN_OUT")));
            this.ResponseProtocolloNumero = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RESP_PROT_NUMERO")));
            this.ResponseProtocolloAnno = ((byte)(OracleTypes.OracleUdt.GetValue(con, pUdt, "RESP_PROT_ANNO")));
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

        public static ComProtocolloType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new ComProtocolloType();
        }
        #endregion   

      
    }

   [OracleTypes.OracleCustomTypeMappingAttribute("COM_PROTOCOLLO_TYPE")]
   public class ComProtocolloTypeFactory : OracleTypes.IOracleCustomTypeFactory
   {

       public virtual OracleTypes.IOracleCustomType CreateObject()
       {
           ComProtocolloType obj = new ComProtocolloType();
           return obj;
       }
   }
}
