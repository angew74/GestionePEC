using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class ComFlussoProtocollo : IDomainObject
    {
        #region "Private Fields"
        private ProtocolloStatus m_STATO_NEW;

        private string m_UTE_OPE;

        private Nullable<long> m_REF_ID_COM;

        private ProtocolloStatus m_STATO_OLD;

        private Nullable<System.DateTime> m_DATA_OPERAZIONE;
        #endregion

        #region IDomainObject Membri di
        public bool IsValid
        {
            get { return ((m_REF_ID_COM.HasValue) && (m_STATO_OLD != ProtocolloStatus.UNKNOWN)); }
        }

        public bool IsPersistent
        {
            get { return (m_REF_ID_COM.HasValue && m_REF_ID_COM.Value > 0); }
        }
        #endregion

        #region "Public Properties"
        public ProtocolloStatus StatoNew
        {
            get
            {
                return m_STATO_NEW;
            }
            set
            {
                this.m_STATO_NEW = value;
            }
        }

        public String UtenteOperazione
        {
            get { return this.m_UTE_OPE; }
            set { this.m_UTE_OPE = value; }
        }

        public Nullable<Int64> RefIdComunicazione
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

        public ProtocolloStatus StatoOld
        {
            get
            {
                return m_STATO_OLD;
            }
            set
            {
                this.m_STATO_OLD = value;
            }
        }

        public Nullable<DateTime> DataOperazione
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
        #endregion
    }
}
