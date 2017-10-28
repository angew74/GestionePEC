using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.WorkFlows
{
    /// <summary>
    /// Semplice enumerativo per gli esiti 'tipizzati' dei metodi esposti da IIstruzioneRichiestaWorkflow.
    /// </summary>
    /// <remarks>
    /// Va rivisto alla luce di una gestione più articolata dei workflows.
    /// </remarks>
    public enum IstruzioneRichiestaMessage
    {
        /// <summary>
        /// Operazione eseguita correttamente
        /// </summary>
        OK,
        /// <summary>
        /// Nominativo modificato rispetto al riferimento in Rubrica
        /// </summary>
        NOMINATIVO_MODIFICATO,
        /// <summary>
        /// Nominativo non presente in Rubrica
        /// </summary>
        NOMINATIVO_NONPRESENTE,

    }
}
