using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;

namespace Com.Delta.Logging
{
    /// <summary>
    /// Classe che estende la classe generica di gestione dell'eccezione
    /// con proprietà e informazioni utili per il tracciamento dell'eccezione
    /// </summary>
    public class ManagedException : System.Exception
    {
        /// <summary>
        /// The codice eccezione
        /// </summary>
        public string CodiceEccezione;
        //public List<string> ActionPath;
        /// <summary>
        /// The modulo
        /// </summary>
        public string Modulo;
        /// <summary>
        /// The funzione
        /// </summary>
        public string Funzione;
        /// <summary>
        /// The azione
        /// </summary>
        public string Azione;
        /// <summary>
        /// The user
        /// </summary>
        public string User;
        /// <summary>
        /// The enanched infos
        /// </summary>
        public string EnanchedInfos;
        /// <summary>
        /// The ticket
        /// </summary>
        public string Ticket;
        /// <summary>
        /// The inner exception message
        /// </summary>
        public string InnerExceptionMessage;

        /// <summary>
        /// Costruttore esteso per la gestione dell'eccezione
        /// </summary>
        /// <param name="message">messaggio di errore</param>
        /// <param name="codiceEccezione">codice eccezione</param>
        /// <param name="modulo">modulo all'interno del quale si è verificata l'eccezione</param>
        /// <param name="funzione">funzione all'interno della quale si è scatenata l'eccezione</param>
        /// <param name="azioneInfo">azione legata all'eccezione</param>
        /// <param name="utente">utente agente al momento dell'eccezione</param>
        /// <param name="enanchedInfo">informazioni accessorie relative all'eccezione</param>
        /// <param name="InnerException">Eccezione interna, ove presente da cui è scaturita l'eccezione</param>
        [Obsolete]
        public ManagedException(string message,
                                string codiceEccezione,
                                string modulo,
                                string funzione,
                                string azioneInfo,
                                string utente,
                                string enanchedInfo,
                                Exception InnerException)
            : base(message, InnerException)
        {
            //ActionPath = new List<string>();
            //AddPath(modulo, funzione, azione);

            Modulo = modulo;
            Funzione = funzione;
            Azione = azioneInfo;
            CodiceEccezione = codiceEccezione;
            User = utente;
            EnanchedInfos = enanchedInfo;
            if (InnerException != null) InnerExceptionMessage = InnerException.Message;
            Random r = new Random(DateTime.Now.Millisecond);
            this.Ticket = DateTime.Now + "-" + r.Next();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="codiceEccezione">The codice eccezione.</param>
        /// <param name="azioneInfo">The azione information.</param>
        /// <param name="enanchedInfo">The enanched information.</param>
        /// <param name="InnerException">The inner exception.</param>
        public ManagedException(string message,
                                string codiceEccezione,
                                string azioneInfo,
                                string enanchedInfo,
                                Exception InnerException)
            : base(message, InnerException)
        {
            StackTrace s = new StackTrace();
            StackFrame f = s.GetFrame(1);

            Modulo = f.GetMethod().DeclaringType.ToString();
            Funzione = f.GetMethod().Name;
            Azione = azioneInfo;
            CodiceEccezione = codiceEccezione;
            User = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            if (!string.IsNullOrEmpty(enanchedInfo))
                EnanchedInfos = "<det>" + enanchedInfo + "</det>";
            if (InnerException != null) InnerExceptionMessage = InnerException.Message;
            Random r = new Random(DateTime.Now.Millisecond);
            this.Ticket = DateTime.Now + "-" + r.Next();
        }

        public void addEnanchedInfosTag(string name, string innerXml)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            if (EnanchedInfos != null)
                doc.LoadXml(EnanchedInfos);
            else
                doc.LoadXml("<det></det>");
            XmlElement el = doc.CreateElement("d");
            XmlAttribute at = doc.CreateAttribute("n");
            at.Value = name;
            el.Attributes.Append(at);
            string parsingText = innerXml.Replace(".", "").Replace("/","").Replace(";","").Replace("\\","");
            try
            {
                el.InnerXml = parsingText;
            }
            catch (Exception ex)
            {
                el.InnerXml = "Errore non gestito";
            }
            finally
            {
                doc.FirstChild.AppendChild(el);
                EnanchedInfos = doc.DocumentElement.OuterXml;
            }
        }

        public XmlDocument getXmlDetails()
        {
            if (!string.IsNullOrEmpty(EnanchedInfos))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(EnanchedInfos);
                return doc;
            }
            else return null;
        }

        public string getXmlDetail(string name)
        {
            if (!string.IsNullOrEmpty(EnanchedInfos))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(EnanchedInfos);
                XmlNodeList l = doc.GetElementsByTagName("d");
                foreach (XmlNode n in l)
                {
                    if (n.Attributes["n"] != null && n.Attributes["n"].Value == name)
                        return n.InnerXml;
                }
                return null;
            }
            else return null;
        }
    }
}
