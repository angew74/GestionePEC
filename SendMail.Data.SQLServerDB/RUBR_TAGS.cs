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
    
    public partial class RUBR_TAGS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUBR_TAGS()
        {
            this.RUBR_ENTITA = new HashSet<RUBR_ENTITA>();
            this.RUBR_REFERRAL_TYPE = new HashSet<RUBR_REFERRAL_TYPE>();
        }
    
        public decimal ID_TAG { get; set; }
        public string TAG { get; set; }
        public System.Guid ROWID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUBR_ENTITA> RUBR_ENTITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUBR_REFERRAL_TYPE> RUBR_REFERRAL_TYPE { get; set; }
    }
}
