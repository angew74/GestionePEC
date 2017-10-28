using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione dei destinatari delle risposte. 
    /// </summary>
    public class Destinatario : IDomainObject
    {
        #region "Properties"

        /// <summary>
        /// Id di riferimento.
        /// </summary>
        public virtual decimal Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual TipoDestinatario Tipo { get; set; }

        /// <summary>
        /// Risposta di riferimento.
        /// </summary>
        public virtual Risposta Risposta { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Risposta.
        /// </summary>
        public decimal RispostaId { get; set; }
        
        /// <summary>
        /// Tipo di canale utilizzato.
        /// </summary>
        public virtual TipoCanale Canale { get; set; }

        /// <summary>
        /// Nominativo di riferimento.
        /// </summary>
        public virtual Nominativo Nominativo { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Nominativo.
        /// </summary>
        public decimal NominativoId { get; set; }

        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Destinatario()
        {
            Init();
        }

        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
            Nominativo = new Nominativo();
            Risposta = new Risposta();
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

        /// <summary>
        /// Verifica che l'oggetto sia "virtualmente" persistente.
        /// </summary>
        /// <returns></returns>
        public  bool IsPersistent
        {
            get { return Id != (default(int)); }
        } 

        #endregion
    }
}
