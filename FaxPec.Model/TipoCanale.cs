using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FaxPec.Model
{
    /// <summary>
    /// Enumerativo dei possibili tipi di canali.
    /// </summary>
    public enum TipoCanale
    {
        [Description("NON DEFINITO")]
        UNKNOWN = 0,
        FAX = 1,
        MAIL = 2,
        POSTA = 3,
        [Description("A MANO")]
        AMANO = 4,
    }
}
