using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Business
{
    /// <summary>
    /// Semplice enumerativo per gli esiti 'tipizzati' dei metodi di scrittura del servizio IRichiestaService.
    /// </summary>
    /// <remarks>
    /// Va rivisto alla luce di una gestione più articolata dei workflows.
    /// </remarks>
    public enum RichiestaServiceResponse
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
        /// <summary>
        /// Errore sul Protocollo
        /// </summary>
        PROTOCOLLO_NON_ACCESSIBILE,
        /// <summary>
        /// Richiesta non valida
        /// </summary>
        RICHIESTA_NON_VALIDA,
        /// <summary>
        /// Richiesta valida
        /// </summary>
        RICHIESTA_VALIDA,
        /// <summary>
        /// default
        /// </summary>
        INIT = 0,
    }
}
