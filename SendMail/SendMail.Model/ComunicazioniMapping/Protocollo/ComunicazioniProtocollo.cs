using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class ComunicazioniProtocollo : IDomainObject
    {
        #region "Private Fields"
        protected bool m_REF_ID_COMIsNull;
        protected long m_REF_ID_COM;
        protected string m_REQ_PROT_TIPO;
        protected string m_REQ_COD_DOCUMENTO;
        protected string m_REQ_IS_ESISTENTE;
        protected string m_REQ_IS_RISERVATO;
        protected string m_REQ_SUBJECT;
        protected string m_RESP_PROT_TIPO;
        protected bool m_RESP_PROT_ANNOIsNull;
        protected short m_RESP_PROT_ANNO;
        protected string m_RESP_PROT_NUMERO;
        #endregion

        #region "Public Fields"
        /// <summary>
        /// 
        /// </summary>
        public virtual Nullable<Int64> RefIdCom
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
        public virtual string RequestTipoProtocollo
        {
            get { return m_REQ_PROT_TIPO; }
            set { m_REQ_PROT_TIPO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string RequestCodiceDocumento
        {
            get { return m_REQ_COD_DOCUMENTO; }
            set { m_REQ_COD_DOCUMENTO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool RequestIsEsistente
        {
            get
            {
                bool reqIsEsistente = false;
                if (!String.IsNullOrEmpty(m_REQ_IS_ESISTENTE))
                {
                    int x = -1;
                    if (int.TryParse(m_REQ_IS_ESISTENTE, out x))
                        reqIsEsistente = Convert.ToBoolean(x);
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
        public virtual bool RequestIsRiservato
        {
            get
            {
                bool reqIsRiservato = false;
                if (!String.IsNullOrEmpty(m_REQ_IS_RISERVATO))
                {
                    int x = -1;
                    if (int.TryParse(m_REQ_IS_RISERVATO, out x))
                        reqIsRiservato = Convert.ToBoolean(x);
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
        public virtual string RequestSubject
        {
            get { return m_REQ_SUBJECT; }
            set { m_REQ_SUBJECT = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ResponseProtocolloTipo
        {
            get { return m_RESP_PROT_TIPO; }
            set { m_RESP_PROT_TIPO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual Nullable<Int16> ResponseProtocolloAnno
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
        public virtual string ResponseProtocolloNumero
        {
            get { return m_RESP_PROT_NUMERO; }
            set { m_RESP_PROT_NUMERO = value; }
        }
        #endregion

        #region "C.tor"
        public ComunicazioniProtocollo() { }
        #endregion

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return !String.IsNullOrEmpty(m_REQ_PROT_TIPO); }
        }

        public bool IsPersistent
        {
            get { return m_REF_ID_COM > 0; }
        }

        #endregion
    }
}
