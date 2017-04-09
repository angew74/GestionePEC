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
    
    public partial class RUBR_CONTATTI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUBR_CONTATTI()
        {
            this.RUBR_CONTATTI_BACKEND = new HashSet<RUBR_CONTATTI_BACKEND>();
        }
    
        public decimal ID_CONTACT { get; set; }
        public string MAIL { get; set; }
        public string FAX { get; set; }
        public string TELEFONO { get; set; }
        public decimal REF_ID_REFERRAL { get; set; }
        public string SOURCE { get; set; }
        public string FLG_IPA { get; set; }
        public string IPA_DN { get; set; }
        public string IPA_ID { get; set; }
        public string NOTE { get; set; }
        public string CONTACT_REF { get; set; }
        public Nullable<decimal> AFF_IPA { get; set; }
        public decimal FLG_PEC { get; set; }
        public string MAIL_DOMAIN { get; set; }
        public string REF_PROT { get; set; }
    
        public virtual RUBR_ENTITA RUBR_ENTITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUBR_CONTATTI_BACKEND> RUBR_CONTATTI_BACKEND { get; set; }
    }
}
