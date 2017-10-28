using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FaxPec.Model
{
    /// <summary>
    /// Enumerativo dei possibili stati pagamento della risposta.
    /// </summary>
    public enum StatoPagamentoRisposta
    {
        [Description("In inserimento")]
        [Searchable(false)] 
        UNKNOWN = 0,
        [Description("Gratuita")]
        [Searchable(false)] 
        GRATUITA = 1,
        [Description("A pagamento")]
        [Searchable(false)] 
        A_PAGAMENTO = 2,
        [Description("Richiedi pagamento")]
        RICHIEDI_PAGAMENTO = 3,
        [Description("In attesa di pagamento")]
        ATTESA_PAGAMENTO = 4,
        [Description("Pagata")]
        PAGATA = 5,
    }
}
