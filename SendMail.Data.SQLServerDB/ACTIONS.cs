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
    
    public partial class ACTIONS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ACTIONS()
        {
            this.ACTIONS_FOLDERS = new HashSet<ACTIONS_FOLDERS>();
        }
    
        public double ID { get; set; }
        public string NOME_AZIONE { get; set; }
        public Nullable<double> ID_NOME_DESTINAZIONE { get; set; }
        public string TIPO_DESTINAZIONE { get; set; }
        public string TIPO_AZIONE { get; set; }
        public string NUOVO_STATUS { get; set; }
        public Nullable<double> ID_FOLDER_DESTINAZIONE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ACTIONS_FOLDERS> ACTIONS_FOLDERS { get; set; }
    }
}
