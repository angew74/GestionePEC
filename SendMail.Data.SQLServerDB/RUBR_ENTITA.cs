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
    
    public partial class RUBR_ENTITA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RUBR_ENTITA()
        {
            this.COMUNICAZIONI_DESTINATARI = new HashSet<COMUNICAZIONI_DESTINATARI>();
            this.RUBR_ENTITA1 = new HashSet<RUBR_ENTITA>();
            this.RUBR_TAGS = new HashSet<RUBR_TAGS>();
        }
    
        public decimal ID_REFERRAL { get; set; }
        public Nullable<decimal> ID_PADRE { get; set; }
        public string REFERRAL_TYPE { get; set; }
        public string COGNOME { get; set; }
        public string NOME { get; set; }
        public string COD_FIS { get; set; }
        public string P_IVA { get; set; }
        public string RAGIONE_SOCIALE { get; set; }
        public string UFFICIO { get; set; }
        public string NOTE { get; set; }
        public Nullable<decimal> REF_ID_ADDRESS { get; set; }
        public string FLG_IPA { get; set; }
        public string IPA_DN { get; set; }
        public string IPA_ID { get; set; }
        public string DISAMB_PRE { get; set; }
        public string DISAMB_POST { get; set; }
        public Nullable<decimal> REF_ORG { get; set; }
        public string SITO_WEB { get; set; }
        public Nullable<decimal> AFF_IPA { get; set; }
        public System.Guid ROWID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMUNICAZIONI_DESTINATARI> COMUNICAZIONI_DESTINATARI { get; set; }
        public virtual RUBR_ADDRESS RUBR_ADDRESS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUBR_ENTITA> RUBR_ENTITA1 { get; set; }
        public virtual RUBR_ENTITA RUBR_ENTITA2 { get; set; }
        public virtual RUBR_REFERRAL_TYPE RUBR_REFERRAL_TYPE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RUBR_TAGS> RUBR_TAGS { get; set; }
    }
}