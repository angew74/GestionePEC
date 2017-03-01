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
        protected bool REF_ID_COMIsNull;
        protected long REF_ID_COM;
        protected string REQ_PROT_TIPO;
        protected string REQ_COD_DOCUMENTO;
        protected string REQ_IS_ESISTENTE;
        protected string REQ_IS_RISERVATO;
        protected string REQ_SUBJECT;
        protected string RESP_PROT_TIPO;
        protected bool RESP_PROT_ANNOIsNull;
        protected short RESP_PROT_ANNO;
        protected string RESP_PROT_NUMERO;
        protected string PROT_IN_OUT;
        #endregion

        #region "Public Fields"
        /// <summary>
        /// 
        /// </summary>
        public virtual Nullable<Int64> RefIdCom
        {
            get
            {
                if (REF_ID_COMIsNull == true)
                    return null;
                else return REF_ID_COM;
            }
            set
            {
                if (value.HasValue == true)
                {
                    REF_ID_COMIsNull = false;
                    REF_ID_COM = value.Value;
                }
                else REF_ID_COMIsNull = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string RequestTipoProtocollo
        {
            get { return REQ_PROT_TIPO; }
            set { REQ_PROT_TIPO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string RequestCodiceDocumento
        {
            get { return REQ_COD_DOCUMENTO; }
            set { REQ_COD_DOCUMENTO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual int? RequestIsEsistente
        {
            get
            {
                int? reqIsEsistente = null;
                if (!String.IsNullOrEmpty(REQ_IS_ESISTENTE))
                {
                    int x = -1;
                    if (int.TryParse(REQ_IS_ESISTENTE, out x))
                        reqIsEsistente =x;
                }
                return reqIsEsistente;
            }
            set
            {
                REQ_IS_ESISTENTE = Convert.ToInt32(value).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual int? RequestIsRiservato
        {
            get
            {
                int? reqIsRiservato =null;
                if (!String.IsNullOrEmpty(REQ_IS_RISERVATO))
                {
                    int x = -1;
                    if (int.TryParse(REQ_IS_RISERVATO, out x))
                        reqIsRiservato = x;
                }
                return reqIsRiservato;
            }
            set
            {
                REQ_IS_RISERVATO = Convert.ToInt32(value).ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string RequestSubject
        {
            get { return REQ_SUBJECT; }
            set { REQ_SUBJECT = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ResponseProtocolloTipo
        {
            get { return RESP_PROT_TIPO; }
            set { RESP_PROT_TIPO = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual Nullable<Int16> ResponseProtocolloAnno
        {
            get
            {
                if (RESP_PROT_ANNOIsNull == true)
                    return null;
                else return RESP_PROT_ANNO;
            }
            set
            {
                if (value.HasValue == true)
                {
                    RESP_PROT_ANNOIsNull = false;
                    RESP_PROT_ANNO = value.Value;
                }
                else
                   RESP_PROT_ANNOIsNull = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ResponseProtocolloNumero
        {
            get { return RESP_PROT_NUMERO; }
            set { RESP_PROT_NUMERO = value; }
        }

        public virtual string ProtocolloInOut
        {
            get { return PROT_IN_OUT; }
            set { PROT_IN_OUT = value; }
        }

        #endregion

        #region "C.tor"
          #region "C.tor"
        public ComunicazioniProtocollo() { }

        public ComunicazioniProtocollo(ComunicazioniProtocollo c)
        {
            if (c == null) return;

            Type t = c.GetType();
            foreach (System.Reflection.PropertyInfo p in t.GetProperties())
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(c, null), null);
                }
            }
        }

        #endregion
        #endregion

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return !String.IsNullOrEmpty(RESP_PROT_NUMERO); }
        }

        public bool IsPersistent
        {
            get { return REF_ID_COM > 0; }
        }

        #endregion
    }
}
