using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
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
        /// se in banca dati
        /// </summary>
        bool IsPersistent { get; }
    }
}
