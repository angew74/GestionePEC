using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe DTO per la gestione dello scambio dati 
    /// con la procedura relativa al Protocollo.
    /// </summary>
    public class DTOProtocolloResponse
    {
        #region "Properties"

        /// <summary>
        /// Numero di protocollo.
        /// </summary>
        public virtual string Numero { get; set; }

        /// <summary>
        /// Data del protocollo.
        /// </summary>
        public virtual string Data { get; set; }
                
        /// <summary>
        /// Codice fiscale o partita iva del mittente.
        /// </summary>
        public virtual string Codice { get; set; }

        /// <summary>
        /// Descrizione del mittente.
        /// </summary>
        public virtual string Mittente { get; set; }

        #endregion
    }

    /// <summary>
    /// Classe DTO per la gestione dello scambio dati 
    /// con la procedura relativa al Protocollo.
    /// </summary>
    public class DTOProtocolloRequest
    {
        #region "Properties"

        public virtual string UserName { get; set; }
        public virtual string Dipartimento { get; set; }
        public virtual string AppCode { get; set; }
        public virtual string TipoProtocollazione { get; set; }
        public virtual string TestoOggetto { get; set; }
        public virtual bool SeRiservato { get; set; }
        public virtual string Nominativo { get; set; }
        public virtual bool SeInteressato { get; set; }
        public virtual string TipoPersona { get; set; }

        #endregion
    }
}
