using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Com.Delta.Logging.Mail
{
    /// <summary>
    /// classe per i log di crabmail
    /// </summary>
    [Serializable]
    public class MailLogInfo : BaseLogInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string userID;
        /// <summary>
        /// 
        /// </summary>
        public string userMail;

        /// <summary>
        /// 
        /// </summary>
        public string mobjectID;

        /// <summary>
        /// Default constructor
        /// </summary>
        public MailLogInfo() { }
        /// <summary>
        /// Costruttore parametrico
        /// </summary>
        /// <param name="appCode">APP CODE</param>
        /// <param name="logCode">CODICE del log</param>
        /// <param name="userID">username dell'utente</param>
        /// <param name="userMail">The user mail.</param>
        /// <param name="details">dettagli del log</param>
        public MailLogInfo(string appCode, string logCode, string userID, string userMail, infotype details)
            : base(appCode, logCode, details.Serialize())
        {
            this.userID = userID;
            this.userMail = userMail;
        }
        /// <summary>
        /// 
        /// </summary>
        public MailLogInfo(string appCode, string logCode, string userID, string userMail, string details)
            : base(appCode, logCode, details)
        {
            this.userID = userID;
            this.userMail = userMail;         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appCode"></param>
        /// <param name="logCode"></param>
        /// <param name="userID"></param>
        /// <param name="userMail"></param>
        /// <param name="details"></param>
        /// <param name="objectid">id mail</param>
        public MailLogInfo(string appCode, string logCode, string userID, string userMail, string details, string objectid)
            : base(appCode, logCode, details)
        {
            this.userID = userID;
            this.userMail = userMail;
            this.mobjectID = objectid;
        }

        /// <summary>
        /// Conversione in stringa
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return string.Format("userID: {0}|mail: {1}|{2}|mobjectID:{3}", userID, userMail, base.ToString(), mobjectID);
        }
        /// <summary>
        /// Metodo di deserializzazione
        /// </summary>
        /// <param name="doc">XmlDocument</param>
        /// <returns>istanza della classe</returns>
        public static new MailLogInfo Deserialize(XmlDocument doc)
        {
            return (MailLogInfo)ConversionUtils.XmlToObject(doc, typeof(MailLogInfo));
        }
        /// <summary>
        /// Metodo di deserializzazione
        /// </summary>
        /// <param name="xDoc">XDocument da convertire in oggetto</param>
        /// <returns>istanza della classe</returns>
        public static MailLogInfo Deserialize(XDocument xDoc)
        {
            XmlSerializer s = new XmlSerializer(typeof(MailLogInfo));
            XmlReader rd = xDoc.CreateReader();
            return (MailLogInfo)s.Deserialize(rd);
        }
    }
}
