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
    
    public partial class MAIL_INBOX_FLUSSO
    {
        public decimal REF_ID_MAIL { get; set; }
        public string STATUS_MAIL_OLD { get; set; }
        public string STATUS_MAIL_NEW { get; set; }
        public System.DateTime DATA_OPERAZIONE { get; set; }
        public string UTE_OPE { get; set; }
        public System.Guid ROWID { get; set; }
    }
}