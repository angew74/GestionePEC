//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SendMail.Data.SQLServerDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class COMUNICAZIONI_FLUSSO_PROT
    {
        public decimal REF_ID_COM { get; set; }
        public Nullable<decimal> STATO_OLD { get; set; }
        public decimal STATO_NEW { get; set; }
        public System.DateTime DATA_OPERAZIONE { get; set; }
        public string UTE_OPE { get; set; }
        public decimal ID_FLUSSO { get; set; }
        public System.Guid ROWID { get; set; }
    
        public virtual COMUNICAZIONI COMUNICAZIONI { get; set; }
    }
}
