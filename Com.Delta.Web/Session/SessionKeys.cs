using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Com.Delta.Web.Session
{
	/// <summary>
	/// 
	/// </summary>
	public enum SessionKeys
	{
		

		/// <summary>
		/// Type: System.String
		/// Info: Chiave del tema dell'applicazione
		/// </summary>
		THEME_SELEZIONATO,

		/// <summary>
		/// Type:(System.String)
		/// Info: Chiave che serve a memorizzare le info sulla stampa come:
		/// a) Xml di risposta da Mapper per la stampa dei documenti associati alla pratica.
		/// b)Elenco degli indirizzi delle circoscrizioni
		/// </summary>
		DATI_XML_STAMPA,

		

        DTO_FILE,


      

        /// <summary>
        /// Type:(BackendUser)
        /// Info: Utente MAIL_USER_BACKEND
        /// </summary>
        MAIL_USER_BACKEND,       

        TESTO_MAIL,

        TESTO_MAIL_ELE,

        TESTO_MAIL_NEW,
        ACCOUNTS_LIST,
        BACKEND_USER,
        FOLDERS,
        UTENTIBACKEND,
        TITOLI,
        TITOLARIO,
        ATTACHEMENTS_LIST
    }
}