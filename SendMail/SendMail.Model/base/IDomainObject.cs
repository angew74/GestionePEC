using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    /// <summary>
    /// Interfaccia astratta di tutte le domain entities.
    /// </summary>
    public interface IDomainObject
    {
        /// <summary>
        /// se valido (verifica dati minimi)
        /// </summary>
        bool IsValid { get; }
        /// <summary>
        /// se nuovo
        /// </summary>
        bool IsPersistent { get; }
    }
}
