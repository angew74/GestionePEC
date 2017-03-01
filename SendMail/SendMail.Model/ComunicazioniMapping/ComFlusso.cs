using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveUp.Net.Common.DeltaExt;

namespace SendMail.Model.ComunicazioniMapping
{
    [Serializable]
    public class ComFlusso : IDomainObject
    {
        #region "Protected Fields"
        private MailStatus m_STATO_COMUNICAZIONE_NEW;

        private string m_UTE_OPE;

        private Nullable<long> m_REF_ID_COM;

        private string m_CANALE;

        private MailStatus m_STATO_COMUNICAZIONE_OLD;

        private Nullable<decimal> m_ID_FLUSSO;

        private Nullable<System.DateTime> m_DATA_OPERAZIONE;
        #endregion

        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return ((this.m_REF_ID_COM.HasValue) &&
                    this.m_STATO_COMUNICAZIONE_NEW != MailStatus.UNKNOWN);
            }
        }

        public bool IsPersistent
        {
            get { return (this.m_REF_ID_COM.HasValue && this.m_REF_ID_COM.Value > 0); }
        }
        #endregion

        #region "Public Properties"
        public MailStatus StatoComunicazioneNew
        {
            get
            {
                return m_STATO_COMUNICAZIONE_NEW;
            }
            set
            {
                this.m_STATO_COMUNICAZIONE_NEW = value;
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

        public TipoCanale Canale
        {
            get
            {
                TipoCanale canale = TipoCanale.UNKNOWN;
                if (!String.IsNullOrEmpty(this.m_CANALE) &&
                    Enum.GetNames(typeof(TipoCanale)).Contains(this.m_CANALE, StringComparer.InvariantCultureIgnoreCase))
                {
                    canale = (TipoCanale)Enum.Parse(typeof(TipoCanale), this.m_CANALE, true);
                }
                return canale;
            }
            set
            {
                this.m_CANALE = value.ToString();
            }
        }

        public MailStatus StatoComunicazioneOld
        {
            get
            {
                return m_STATO_COMUNICAZIONE_OLD;
            }
            set
            {
                this.m_STATO_COMUNICAZIONE_OLD = value;
            }
        }

        public Nullable<decimal> IdFlusso
        {
            get
            {
                return m_ID_FLUSSO;
            }
            set
            {
                this.m_ID_FLUSSO = value;
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
