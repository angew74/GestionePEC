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
    
    public partial class RUBR_BACKEND
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUBR_BACKEND()
        {
            this.RUBR_CONTATTI_BACKEND = new HashSet<RUBR_CONTATTI_BACKEND>();
        }
    
        public decimal ID_BACKEND { get; set; }
        public string BACKEND_CODE { get; set; }
        public string BACKEND_DESCR { get; set; }
        public string CATEGORY { get; set; }
        public string DESCR_PLUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUBR_CONTATTI_BACKEND> RUBR_CONTATTI_BACKEND { get; set; }
    }
}
