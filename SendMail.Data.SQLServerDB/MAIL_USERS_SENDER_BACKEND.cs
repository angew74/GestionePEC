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
    
    public partial class MAIL_USERS_SENDER_BACKEND
    {
        public decimal REF_ID_USER { get; set; }
        public decimal REF_ID_SENDER { get; set; }
        public string ROLE { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
    
        public virtual MAIL_SENDERS MAIL_SENDERS { get; set; }
        public virtual MAIL_USERS_BACKEND MAIL_USERS_BACKEND { get; set; }
    }
}
