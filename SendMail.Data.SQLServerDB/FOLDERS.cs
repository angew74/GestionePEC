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
    
    public partial class FOLDERS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FOLDERS()
        {
            this.ACTIONS_FOLDERS = new HashSet<ACTIONS_FOLDERS>();
            this.MAIL_CONTENT = new HashSet<MAIL_CONTENT>();
            this.MAIL_INBOX = new HashSet<MAIL_INBOX>();
            this.FOLDERS_SENDERS = new HashSet<FOLDERS_SENDERS>();
            this.MAIL_INBOX1 = new HashSet<MAIL_INBOX>();
        }
    
        public decimal ID { get; set; }
        public string NOME { get; set; }
        public string TIPO { get; set; }
        public string SYSTEM { get; set; }
        public Nullable<double> IDNOME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACTIONS_FOLDERS> ACTIONS_FOLDERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIL_CONTENT> MAIL_CONTENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIL_INBOX> MAIL_INBOX { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FOLDERS_SENDERS> FOLDERS_SENDERS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAIL_INBOX> MAIL_INBOX1 { get; set; }
    }
}
