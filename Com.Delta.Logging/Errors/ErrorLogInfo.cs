using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Com.Delta.Logging.Errors
{
    /// <summary>
    /// Classe di serializzazione e deserializzazione dell'XML da inserire nel logger queue per l'accodamento dei log
    /// di errore delle applicazioni
    /// </summary>
    [Serializable]
    public class ErrorLogInfo : BaseLogInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogInfo"/> class.
        /// </summary>
        public ErrorLogInfo() : base() 
        {
            if(System.Threading.Thread.CurrentPrincipal!=null&& System.Threading.Thread.CurrentPrincipal.Identity!=null)
            this.userID = System.Threading.Thread.CurrentPrincipal.Identity.Name;
        }

        /// <summary>
        /// Costruttore generico che estende il costruttore della classe astratta BaseLogInfo
        /// </summary>
        /// <param name="appCode">Codice applicazione</param>
        /// <param name="logCode">tipo di log</param>
        /// <param name="details">dettagli e informazioni aggiuntive sul log</param>
        /// <param name="userID">identificativo utente agente</param>
        /// <param name="userIP">ip dell'utente agente</param>
        /// <param name="objectID">id dell'utente attivo</param>
        /// <param name="objectGroupID">gruppo utente attivo</param>
        /// <param name="objectAppID">id dell'applicazione </param>
        /// <param name="objectParentcode">parentcode del gruppo</param>
        /// <param name="passiveobjectID">id dell'utente passivo</param>
        /// <param name="passiveobjectGroupID">id gruppo passivo</param>
        /// <param name="passiveparentcodeobjectID">parentcode passivo</param>
        /// <param name="passiveapplicationID">id dell'applicazione passivo</param>
        public ErrorLogInfo(string appCode, string logCode, string details, string userID, string userIP, string objectID, string objectGroupID, string objectAppID, string objectParentcode, string passiveobjectID, string passiveobjectGroupID, string passiveparentcodeobjectID, string passiveapplicationID)
            : base(appCode, logCode, details)
        {
            this.userID = userID;

            this.userIP = userIP;
            this.objectID = objectID;
            this.objectGroupID = objectGroupID;
            this.objectAppID = objectAppID;
            this.objectParentcode = objectParentcode;
            this.passiveobjectGroupID = passiveobjectGroupID;
            this.passiveobjectID = passiveobjectID;
            this.passiveparentcodeobjectID = passiveparentcodeobjectID;
            this.passiveapplicationID = passiveapplicationID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogInfo" /> class.
        /// </summary>
        /// <param name="appCode">The application code.</param>
        /// <param name="mex">The mex.</param>
        public ErrorLogInfo(string appCode, ManagedException mex)
            : base(appCode, mex.CodiceEccezione, mex.Message + " // " + mex.InnerExceptionMessage)
        {
            this.userID = mex.User;
            this.objectID = mex.Ticket;
            this.objectAppID = appCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogInfo"/> class.
        /// </summary>
        /// <param name="e">The e.</param>
        public ErrorLogInfo(ManagedException e)
            : base(e.CodiceEccezione, e.Message + " >> " + e.InnerExceptionMessage)
        {
            this.userID = e.User;
            this.module = e.Modulo;
            this.function = e.Funzione;
            this.action = e.Azione;
            //this.objectID = e.Ticket; //modifica Ciro - 08/01/2016
            this.enanchedInfos = e.EnanchedInfos;
        }
        /// <summary>
        /// ID Utente
        /// </summary>
        public string userID;
        /// <summary>
        /// IP Utente
        /// </summary>
        public string userIP;
        /// <summary>
        /// extended infos
        /// </summary>
        public string enanchedInfos;
        /// <summary>
        /// id oggetto
        /// </summary>
        public string objectID;
        /// <summary>
        /// id gruppo oggetto
        /// </summary>
        public string objectGroupID;
        /// <summary>
        /// id applicazione
        /// </summary>
        public string objectAppID;
        /// <summary>
        /// Parentcode 
        /// </summary>
        public string objectParentcode;
        /// <summary>
        /// id oggetto passivo 
        /// </summary>
        public string passiveobjectID;
        /// <summary>
        /// id gruppo oggetto passivo
        /// </summary>
        public string passiveobjectGroupID;
        /// <summary>
        /// parentcode oggetto passivo
        /// </summary>
        public string passiveparentcodeobjectID;
        /// <summary>
        /// applicazione oggetto passivo
        /// </summary>
        public string passiveapplicationID;
        /// <summary>
        /// 
        /// </summary>
        public string module;
        /// <summary>
        /// 
        /// </summary>
        public string function;
        /// <summary>
        /// 
        /// </summary>
        public string action;


        /// <summary>
        /// Override del metodo to string che scrive in esteso il file xml del log
        /// </summary>
        /// <returns><c>string</c>stringa dell'XML da accodare</returns>
        public override string ToString()
        {
            return base.ToString() + "|Module:"+module+"|function:"+function+"|action:"+action+"|uniqueLogID:" + uniqueLogID + "|UserID:" + userID + "|userIP:" + userIP + "|objectAppID:" + objectAppID + "|objectGroupID:" + objectGroupID + "|objectID:" + objectID + "|objectParentcode:" + objectParentcode + "|passiveobjectID:" + passiveobjectID + "|passiveobjectGroupID:" + passiveobjectGroupID + "|enanchedInfos:" + enanchedInfos;
        }

        /// <summary>
        ///  Metodo per la deserializzazione dell'XML document già accodato nel logger queue e la trasformazione 
        /// in una classe per la gestione delle informazioni da accodare in base dati
        /// </summary>
        /// <param name="doc">Xml Document da deserializzare</param>
        /// <returns><c>ErrorLogInfo</c> classe che espone le proprietà dell'oggetto ErrorLogInfo per la scrittura in banca dati</returns>
        public static new ErrorLogInfo Deserialize(XmlDocument doc)
        {
            return (ErrorLogInfo)ConversionUtils.XmlToObject(doc, typeof(ErrorLogInfo));
        }
    }
}
