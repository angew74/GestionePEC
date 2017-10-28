using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
{
    public class UtenteFAXPEC : IDomainObject
    {
        #region "Properties"

        public virtual string UserName { get; set; }

        public virtual string NomeCognome { get; set; }

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
            get { return UserName != (default(string)); }
        } 

        #endregion
    }
    
}
