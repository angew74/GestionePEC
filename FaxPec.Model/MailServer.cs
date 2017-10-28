using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione ...
    /// </summary>
    public class MailServer : DomainObject
    {
        #region "Properties"

        /// <summary>
        /// Id di riferimento.
        /// </summary>
        public virtual decimal Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Nome { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual EndPoint In { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual EndPoint Out { get; set; }

        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public MailServer()
        {
            Init();
        }

        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
            Nome = string.Empty;
            In = new EndPoint();
            Out = new EndPoint();
        } 

        #endregion

        #region "DomainObject members"

        /// <summary>
        /// Verifica la presenza dei dati minimi dell'oggetto.
        /// </summary>
        /// <returns></returns>
        public override bool IsValid
        {
            get
            {
                //todo.. altre verifiche

                //tutto ok
                return true;
            }
        }

        /// <summary>
        /// Verifica che l'oggetto sia "virtualmente" persistente
        /// </summary>
        /// <returns></returns>
        public override bool IsPersistent
        {
            get { return Id != (default(int)); }
        }

        #endregion
    }
}
