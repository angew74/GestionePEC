using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FaxPec.Model
{
    /// <summary>
    /// Enumerativo dei possibili stati della richiesta.
    /// </summary>
    public enum StatoRichiesta
    {
        [Description("In inserimento")]
        [Searchable(false)] 
        UNKNOWN = 0,
        [Description("Non lavorata")]
        NON_LAVORATA = 1,
        [Description("Parzialmente lavorata")] 
        PARZ_LAVORATA = 2,
        [Description("Completa")]   //Lavorata ?
        COMPLETA = 3,
        [Description("Lavorata")]   //Chiusa ?
        LAVORATA = 4,
    }
}
