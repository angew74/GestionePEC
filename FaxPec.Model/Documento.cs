using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione dei documenti... 
    /// </summary>
    public  class Documento : IDomainObject
    {
        #region "Private Fields"

        /// <summary>
        /// Identificativo Pratica
        /// </summary>
        private long _PraticaId;
        /// <summary>
        /// Identificativo Risposta
        /// </summary>
        private decimal _RispostaId;

        #endregion
        
        #region "Properties"
        /// <summary>
        /// 
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Richiesta.
        /// </summary>
        public decimal RichiestaId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Risposta Risposta { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Risposta.
        /// </summary>
        public decimal RispostaId
        { 
            get
            {
                if (Risposta != null)   //..
                    _RispostaId = Risposta.Id;
                return _RispostaId;
            }
            set { _RispostaId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Pratica Pratica { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Pratica.
        /// </summary>
        public Nullable<Int64> PraticaId
        { 
            get
            {
                if (Pratica != null)   //..
                    _PraticaId = (long)Pratica.Id;
                return _PraticaId;
            }
            set { _PraticaId =(long) value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime DataLavorazione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual byte[] Attachment { get; set; } 
        
        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Documento()
        {
            Init();
        }

        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
            Pratica = new Pratica();
            Risposta = new Risposta();
            DataLavorazione = default(DateTime);
            this.Id = Guid.NewGuid().ToString();
            //Random r = new Random(System.DateTime.Now.Millisecond + System.DateTime.Now.Second);
            //this.Id = r.Next();
            _IsPersistent = false;
        } 

        #endregion

        #region "DomainObject members"

        /// <summary>
        /// Verifica la presenza dei dati minimi dell'oggetto.
        /// </summary>
        /// <returns></returns>
        public  bool IsValid
        {
            get
            {
                //todo.. altre verifiche

                //tutto ok
                return true;
            }
        }

        private bool _IsPersistent;

        /// <summary>
        /// Verifica che l'oggetto sia "virtualmente" persistente.
        /// </summary>
        /// <returns></returns>
        public  bool IsPersistent
        {
            get { return _IsPersistent; }
            set { _IsPersistent = value; }
        } 

        #endregion
    }
}
