using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace Com.Delta.Logging.Errors
{
    /// <summary>
    /// Classe di serializzazione e deserializzazione dell'XML da inserire nel logger queue per l'accodamento dei log
    /// di errore delle applicazioni
    /// </summary>
    [Serializable]
    public class ErrorLog : BaseLogInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public ErrorLog() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appCode"></param>
        /// <param name="e"></param>
        public ErrorLog(string appCode, ManagedException e)
            : base(appCode, e.CodiceEccezione, e.Message+"//"+e.InnerExceptionMessage)
        {
            this._userID = e.User;
            this._modulo = e.Modulo;
            this._azione = e.Azione;
            this._funzione = e.Funzione;
            this._XmlAppInfo = e.EnanchedInfos;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        public ErrorLog(ManagedException e)
            : base( e.CodiceEccezione, e.Message + "//" + e.InnerExceptionMessage)
        {
            this._userID = e.User;
            this._modulo = e.Modulo;
            this._azione = e.Azione;
            this._funzione = e.Funzione;
            this._XmlAppInfo = e.EnanchedInfos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appCode"></param>
        /// <param name="logCode"></param>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="modulo"></param>
        /// <param name="funzione"></param>
        /// <param name="azione"></param>
        /// <param name="xmlInfo"></param>
        /// <param name="schema"></param>
        public ErrorLog(string appCode, string logCode, string message, string user, string modulo, string funzione, string azione, string xmlInfo,string schema)
            : base(appCode, logCode, message)
        {
            this._userID = user;
            this._modulo = modulo;
            this._azione = azione;
            this._funzione = funzione;
            this._XmlAppInfo = xmlInfo;
            this._Schema = schema;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logCode"></param>
        /// <param name="e"></param>
        /// <param name="azioneInfo"></param>
        /// <param name="xmlInfo"></param>
        /// <param name="schemaInfo"></param>
        public ErrorLog(string logCode,Exception e,string azioneInfo,string xmlInfo,string schemaInfo)
            : base(logCode, e.Message + ((e.InnerException!=null)?"//"+e.InnerException:string.Empty))
        {
            StackTrace s = new StackTrace();
            StackFrame f= s.GetFrame(1);
            this._userID = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            this._modulo = f.GetMethod().DeclaringType.ToString();
            this._azione = azioneInfo;
            this._funzione = f.GetMethod().Name;
            this._XmlAppInfo = xmlInfo;
            this._Schema = schemaInfo;
        }

   
        string _userID; 
        string _modulo;
        string _funzione; 
        string _azione;
        string _XmlAppInfo;
        string _Schema;


        /// <summary>
        /// Override del metodo to string che scrive in esteso il file xml del log
        /// </summary>
        /// <returns><c>string</c>stringa dell'XML da accodare</returns>
        public override string ToString()
        {
            return base.ToString() + "|uniqueLogID:" + uniqueLogID + "|UserID:" + _userID + "|modulo:" + _modulo + "|funzione:" + _funzione + "|azione" + _azione + "|XmlDetails:" + _XmlAppInfo;
        }
         
        /// <summary>
        ///  Metodo per la deserializzazione dell'XML document già accodato nel logger queue e la trasformazione 
        /// in una classe per la gestione delle informazioni da accodare in base dati
        /// </summary>
        /// <param name="doc">Xml Document da deserializzare</param>
        /// <returns><c>ErrorLogInfo</c> classe che espone le proprietà dell'oggetto ErrorLogInfo per la scrittura in banca dati</returns>
        public static new ErrorLog Deserialize(XmlDocument doc)
        {

            return (ErrorLog)ConversionUtils.XmlToObject(doc, typeof(ErrorLog));


        }



    }
}
