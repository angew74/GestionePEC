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
    using ActiveUp.Net.Common.DeltaExt;
    using SendMail.Model;
    using System.Linq;
    using OracleClient = global::Oracle.DataAccess.Client;
    using OracleTypes = global::Oracle.DataAccess.Types;

    public class ComFlussoType : OracleTypes.INullable, OracleTypes.IOracleCustomType, IXmlSerializable
    {

        private bool m_IsNull;

        private string m_STATO_COMUNICAZIONE_NEW;

        private string m_UTE_OPE;

        private long m_REF_ID_COM;

        private bool m_REF_ID_COMIsNull;

        private bool m_ID_FLUSSOIsNull;

        private string m_CANALE;

        private string m_STATO_COMUNICAZIONE_OLD;

        private System.DateTime m_DATA_OPERAZIONE;

        private bool m_DATA_OPERAZIONEIsNull;

        private decimal m_ID_FLUSSO;

        #region "C.tor"
        public ComFlussoType()
        {
            // TODO : Add code to initialise the object
            this.m_REF_ID_COMIsNull = true;
            this.m_DATA_OPERAZIONEIsNull = true;
        }

        public ComFlussoType(string str)
        {
            // TODO : Add code to initialise the object based on the given string 
        }
        #endregion

        public virtual bool IsNull
        {
            get
            {
                return this.m_IsNull;
            }
        }

        public static ComFlussoType Null
        {
            get
            {
                ComFlussoType obj = new ComFlussoType();
                obj.m_IsNull = true;
                return obj;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("STATO_COMUNICAZIONE_NEW")]
        public string STATO_COMUNICAZIONE_NEW
        {
            get
            {
                if (this.m_STATO_COMUNICAZIONE_NEW == "-1")
                    return null;

                return this.m_STATO_COMUNICAZIONE_NEW;
            }
            set
            {
                this.m_STATO_COMUNICAZIONE_NEW = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("UTE_OPE")]
        public string UTE_OPE
        {
            get
            {
                return this.m_UTE_OPE;
            }
            set
            {
                this.m_UTE_OPE = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("ID_FLUSSO")]
        public decimal ID_FLUSSO
        {
            get
            {
                return this.m_ID_FLUSSO;
            }
            set
            {
                this.m_ID_FLUSSO = value;
                this.m_ID_FLUSSOIsNull = false;
            }
        }


        [OracleTypes.OracleObjectMappingAttribute("REF_ID_COM")]
        public long REF_ID_COM
        {
            get
            {
                return this.m_REF_ID_COM;
            }
            set
            {
                this.m_REF_ID_COM = value;
                this.m_REF_ID_COMIsNull = false;
            }
        }

        public bool ID_FLUSSOIsNull
        {
            get
            {
                return this.m_ID_FLUSSOIsNull;
            }
            set
            {
                this.m_ID_FLUSSOIsNull = value;
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

        [OracleTypes.OracleObjectMappingAttribute("CANALE")]
        public string CANALE
        {
            get
            {
                return this.m_CANALE;
            }
            set
            {
                this.m_CANALE = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("STATO_COMUNICAZIONE_OLD")]
        public string STATO_COMUNICAZIONE_OLD
        {
            get
            {
                if (this.m_STATO_COMUNICAZIONE_OLD == "-1")
                    return null;

                return this.m_STATO_COMUNICAZIONE_OLD;
            }
            set
            {
                this.m_STATO_COMUNICAZIONE_OLD = value;
            }
        }

        [OracleTypes.OracleObjectMappingAttribute("DATA_OPERAZIONE")]
        public System.DateTime DATA_OPERAZIONE
        {
            get
            {
                return this.m_DATA_OPERAZIONE;
            }
            set
            {
                this.m_DATA_OPERAZIONE = value;
            }
        }

        public bool DATA_OPERAZIONEIsNull
        {
            get
            {
                return this.m_DATA_OPERAZIONEIsNull;
            }
            set
            {
                this.m_DATA_OPERAZIONEIsNull = value;
            }
        }

        #region "Public Methods"
        public virtual void FromCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            OracleTypes.OracleUdt.SetValue(con, pUdt, "STATO_COMUNICAZIONE_NEW", this.STATO_COMUNICAZIONE_NEW);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "UTE_OPE", this.UTE_OPE);
            if ((REF_ID_COMIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "REF_ID_COM", this.REF_ID_COM);
            }
            if ((ID_FLUSSOIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "ID_FLUSSO", this.ID_FLUSSO);
            }

            OracleTypes.OracleUdt.SetValue(con, pUdt, "CANALE", this.CANALE);
            OracleTypes.OracleUdt.SetValue(con, pUdt, "STATO_COMUNICAZIONE_OLD", this.STATO_COMUNICAZIONE_OLD);
            if ((DATA_OPERAZIONEIsNull == false))
            {
                OracleTypes.OracleUdt.SetValue(con, pUdt, "DATA_OPERAZIONE", this.DATA_OPERAZIONE);
            }
        }

        public virtual void ToCustomObject(OracleClient.OracleConnection con, System.IntPtr pUdt)
        {
            this.STATO_COMUNICAZIONE_NEW = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "STATO_COMUNICAZIONE_NEW")));
            this.UTE_OPE = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "UTE_OPE")));
            this.REF_ID_COMIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "REF_ID_COM");
            if ((REF_ID_COMIsNull == false))
            {
                this.REF_ID_COM = ((long)(OracleTypes.OracleUdt.GetValue(con, pUdt, "REF_ID_COM")));
            }
            this.ID_FLUSSOIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "ID_FLUSSO");
            if ((ID_FLUSSOIsNull == false))
            {
                this.ID_FLUSSO = ((decimal)(OracleTypes.OracleUdt.GetValue(con, pUdt, "ID_FLUSSO")));
            }
            this.CANALE = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "CANALE")));
            this.STATO_COMUNICAZIONE_OLD = ((string)(OracleTypes.OracleUdt.GetValue(con, pUdt, "STATO_COMUNICAZIONE_OLD")));
            this.DATA_OPERAZIONEIsNull = OracleTypes.OracleUdt.IsDBNull(con, pUdt, "DATA_OPERAZIONE");
            if ((DATA_OPERAZIONEIsNull == false))
            {
                this.DATA_OPERAZIONE = ((System.DateTime)(OracleTypes.OracleUdt.GetValue(con, pUdt, "DATA_OPERAZIONE")));
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

        public static ComFlussoType Parse(string str)
        {
            // TODO : Add code needed to parse the string and get the object represented by the string
            return new ComFlussoType();
        }
        #endregion

        #region "Public Conversion Operators"

        public static explicit operator ComFlusso(ComFlussoType c)
        {
            ComFlusso cf;
            if (c.m_IsNull == true)
            {
                cf = null;
                return cf;
            }

            cf = new ComFlusso();
            cf.Canale = TipoCanale.UNKNOWN;
            if (!String.IsNullOrEmpty(c.m_CANALE))
                if (Enum.GetNames(typeof(TipoCanale)).Contains(c.m_CANALE, StringComparer.InvariantCultureIgnoreCase))
                    cf.Canale = (TipoCanale)Enum.Parse(typeof(TipoCanale), c.m_CANALE);

            cf.DataOperazione = null;
            if (c.m_DATA_OPERAZIONEIsNull == false)
                cf.DataOperazione = c.m_DATA_OPERAZIONE;

            cf.RefIdComunicazione = null;
            if (c.m_REF_ID_COMIsNull == false)
                cf.RefIdComunicazione = c.m_REF_ID_COM;
            cf.IdFlusso = null;
            if (c.m_ID_FLUSSOIsNull == false)
            { cf.IdFlusso = c.m_ID_FLUSSO; }
            cf.StatoComunicazioneNew = MailStatus.UNKNOWN;
            if (!String.IsNullOrEmpty(c.m_STATO_COMUNICAZIONE_NEW))
            {
                int i = -1;
                if (int.TryParse(c.m_STATO_COMUNICAZIONE_NEW, out i))
                    if (Enum.IsDefined(typeof(MailStatus), i))
                        cf.StatoComunicazioneNew = (MailStatus)i;
            }

            cf.StatoComunicazioneOld = MailStatus.UNKNOWN;
            if (!String.IsNullOrEmpty(c.m_STATO_COMUNICAZIONE_OLD))
            {
                int i = -1;
                if (int.TryParse(c.m_STATO_COMUNICAZIONE_OLD, out i))
                    if (Enum.IsDefined(typeof(MailStatus), i))
                        cf.StatoComunicazioneOld = (MailStatus)i;
            }

            cf.UtenteOperazione = c.m_UTE_OPE;

            return cf;
        }

        public static explicit operator ComFlussoType(ComFlusso c)
        {
            ComFlussoType ct = new ComFlussoType();
            if (c == null)
            {
                ct.m_IsNull = true;
                return ct;
            }

            if (c.Canale == TipoCanale.UNKNOWN)
                ct.m_CANALE = null;
            else
                ct.m_CANALE = c.Canale.ToString();

            ct.m_DATA_OPERAZIONEIsNull = !c.DataOperazione.HasValue;
            if (ct.m_DATA_OPERAZIONEIsNull == false)
                ct.m_DATA_OPERAZIONE = c.DataOperazione.Value;

            ct.m_REF_ID_COMIsNull = !c.RefIdComunicazione.HasValue;
            if (ct.m_REF_ID_COMIsNull == false)
                ct.m_REF_ID_COM = c.RefIdComunicazione.Value;

            if (c.StatoComunicazioneNew == MailStatus.UNKNOWN)
                ct.m_STATO_COMUNICAZIONE_NEW = null;
            else
                ct.m_STATO_COMUNICAZIONE_NEW = c.StatoComunicazioneNew.ToString();

            if (c.StatoComunicazioneOld == MailStatus.UNKNOWN)
                ct.m_STATO_COMUNICAZIONE_OLD = null;
            else
                ct.m_STATO_COMUNICAZIONE_OLD = c.StatoComunicazioneOld.ToString();

            ct.m_UTE_OPE = c.UtenteOperazione;
            return ct;
        }

        public static explicit operator ComFlussoProtocollo(ComFlussoType cft)
        {
            ComFlussoProtocollo cfp;
            if (cft.m_IsNull == true)
            {
                cfp = null;
                return cfp;
            }

            cfp = new ComFlussoProtocollo();
            
            if (cft.m_REF_ID_COMIsNull == true)
                cfp.RefIdComunicazione = null;
            else
                cfp.RefIdComunicazione = cft.m_REF_ID_COM;

            cfp.StatoOld = ProtocolloStatus.UNKNOWN;
            if (!String.IsNullOrEmpty(cft.m_STATO_COMUNICAZIONE_OLD))
            {
                int i = -1;
                if (int.TryParse(cft.m_STATO_COMUNICAZIONE_OLD, out i))
                    if (Enum.IsDefined(typeof(ProtocolloStatus), i))
                        cfp.StatoOld = (ProtocolloStatus)i;
            }

            cfp.StatoNew = ProtocolloStatus.UNKNOWN;
            if (!String.IsNullOrEmpty(cft.m_STATO_COMUNICAZIONE_NEW))
            {
                int i = -1;
                if (int.TryParse(cft.m_STATO_COMUNICAZIONE_NEW, out i))
                    if (Enum.IsDefined(typeof(ProtocolloStatus), i))
                        cfp.StatoOld = (ProtocolloStatus)i;
            }

            if (cft.m_DATA_OPERAZIONEIsNull == true)
                cfp.DataOperazione = null;
            else
                cfp.DataOperazione = cft.m_DATA_OPERAZIONE;

            cfp.UtenteOperazione = cft.m_UTE_OPE;

            return cfp;
        }

        public static explicit operator ComFlussoType(ComFlussoProtocollo cfp)
        {
            ComFlussoType cft = new ComFlussoType();
            if (cfp == null)
            {
                cft.m_IsNull = true;
                return cft;
            }

            cft.m_CANALE = null;

            cft.m_DATA_OPERAZIONEIsNull = !cfp.DataOperazione.HasValue;
            if(cft.m_DATA_OPERAZIONEIsNull==false)
                cft.m_DATA_OPERAZIONE = cfp.DataOperazione.Value;

            cft.m_REF_ID_COMIsNull = !cfp.RefIdComunicazione.HasValue;
            if (cft.m_REF_ID_COMIsNull == false)
                cft.m_REF_ID_COM = cfp.RefIdComunicazione.Value;

            if (cfp.StatoNew == ProtocolloStatus.UNKNOWN)
                cft.m_STATO_COMUNICAZIONE_NEW = null;
            else
                cft.m_STATO_COMUNICAZIONE_NEW = cfp.StatoNew.ToString();

            if (cfp.StatoOld == ProtocolloStatus.UNKNOWN)
                cft.m_STATO_COMUNICAZIONE_OLD = null;
            else
                cft.m_STATO_COMUNICAZIONE_OLD = cfp.StatoOld.ToString();

            cft.m_UTE_OPE = cfp.UtenteOperazione;
            
            return cft;
        }

        #endregion
    }

    // Factory to create an object for the above class
    [OracleTypes.OracleCustomTypeMappingAttribute("COM_FLUSSO_TYPE")]
    public class ComFlussoTypeFactory : OracleTypes.IOracleCustomTypeFactory
    {

        public virtual OracleTypes.IOracleCustomType CreateObject()
        {
            ComFlussoType obj = new ComFlussoType();
            return obj;
        }
    }
}