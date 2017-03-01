using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Com.Delta.Logging.Um
{ 
    /// <summary>
    /// Classe di serializzazione e deserializzazione dell'XML da inserire nel logger queue per l'accodamento dei log
    /// dello UserManager
    /// </summary>
    [Serializable]
    public class UmLogInfo : BaseLogInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UmLogInfo"/> class.
        /// </summary>
        public UmLogInfo() : base() { }
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
        public UmLogInfo(string appCode, string logCode, string details, string userID, string userIP, string objectID, string objectGroupID, string objectAppID, string objectParentcode, string passiveobjectID, string passiveobjectGroupID, string passiveparentcodeobjectID, string passiveapplicationID)
            : base(appCode, logCode, details)
        {
            this.userID = userID;

            this.userIP=userIP;
            this.objectID = objectID;
            this.objectGroupID = objectGroupID;
            this.objectAppID= objectAppID;
            this.objectParentcode = objectParentcode;
            this.passiveobjectGroupID = passiveobjectGroupID;
            this.passiveobjectID = passiveobjectID;
            this.passiveparentcodeobjectID = passiveparentcodeobjectID;
            this.passiveapplicationID = passiveapplicationID;
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
        /// Override del metodo to string che scrive in esteso il file xml del log
        /// </summary>
        /// <returns><c>string</c> stringa dell'XML da accodare</returns>
        public override string ToString()
        {
            return base.ToString() + "uniqueLogID:" + uniqueLogID + "|UserID:" + userID + "|userIP:" + userIP + "|objectAppID:" + objectAppID + "|objectGroupID:" + objectGroupID + "|objectID:" + objectID + "|objectParentcode:" + objectParentcode + "|passiveobjectID:" + passiveobjectID + "|passiveobjectGroupID:" + passiveobjectGroupID;
        }
        /// <summary>
        ///  Metodo per la deserializzazione dell'XML document già accodato nel logger queue e la trasformazione 
        /// in una classe per la gestione delle informazioni da accodare in base dati
        /// </summary>
        /// <param name="doc">Xml Document da deserializzare</param>
        /// <returns><c>UmLogInfo</c>classe che espone le proprietà dell'oggetto UMLogInfo per la scrittura in banca dati</returns>
        public static new UmLogInfo  Deserialize(XmlDocument doc)
        {

            return (UmLogInfo)ConversionUtils.XmlToObject(doc, typeof(UmLogInfo));


        }

         
        
    }
}
