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
    
    public partial class COMUNICAZIONI_FLUSSO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMUNICAZIONI_FLUSSO()
        {
            this.MAIL_CONTENT = new HashSet<MAIL_CONTENT>();
            this.MAIL_CONTENT1 = new HashSet<MAIL_CONTENT>();
        }
    
        public decimal REF_ID_COM { get; set; }
        public string STATO_COMUNICAZIONE_OLD { get; set; }
        public string STATO_COMUNICAZIONE_NEW { get; set; }
        public System.DateTime DATA_OPERAZIONE { get; set; }
        public string UTE_OPE { get; set; }
        public string CANALE { get; set; }
        public double ID_FLUSSO { get; set; }
        public System.Guid ROWID { get; set; }
    
        public virtual COMUNICAZIONI COMUNICAZIONI { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIL_CONTENT> MAIL_CONTENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIL_CONTENT> MAIL_CONTENT1 { get; set; }
    }
}
