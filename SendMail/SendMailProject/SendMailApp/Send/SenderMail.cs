#define TEST
#region "using"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using log4net;

using Com.Delta.MetaBus.Base;

using SendMail.Contracts;
using SendMail.Business.Contracts;
using SendMail.Locator;
using SendMail.Model;
using SendMail.Business;
using System.Xml;
//using PrintDirectorTpu.Model;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using System.Xml.Linq;
using Com.Delta.MetaBus.Schemas.Envelope;
using Com.Delta.MetaBus.Schemas.Smtp;
using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.PrintDirector;
using SendMailApp.Formatter;
using System.IO;
using Com.Delta.Logging.CrabMail;
using SendMailApp.Properties;

#endregion

namespace SendMailApp
{
    public class SenderMail
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SenderMail));
        private const String APP_CODE = "SEN_MAIL";
        private const TipoCanale CANALE_INVIO = TipoCanale.MAIL;

        public static void Send(string mail, int maxNrMail)
        {
            Console.WriteLine("Invio mail");
            List<SendMail.Model.ComunicazioniMapping.Comunicazioni> listComunicazioni = null;
            Request ric = null;
            IComunicazioniService s = ServiceLocator.GetServiceFactory().ComunicazioniService;
            listComunicazioni = (List<SendMail.Model.ComunicazioniMapping.Comunicazioni>)s.LoadComunicazioniDaInviare(CANALE_INVIO, 0, maxNrMail, mail);
            if (listComunicazioni == null || listComunicazioni.Count == 0)
            {
                _log.Info(new CrabMailLogInfo(APP_CODE, "", "BATCH", mail,
                    "Nessuna comunicazione da inviare"));
                return;
            }
#if TEST
           // listComunicazioni = listComunicazioni.Where(c => c.IdComunicazione == 54373).ToList(); 
#endif
            Console.WriteLine("Trovate {0} comunicazioni", listComunicazioni.Count);
            _log.Info(new CrabMailLogInfo(APP_CODE, "", "BACTH", mail,
                string.Format("Trovate {0} comunicazioni", listComunicazioni.Count)));
            List<String> mails = null;
            var accs = from c in listComunicazioni
                       where c.MailComunicazione != null
                       let mc = c.MailComunicazione
                       where !String.IsNullOrEmpty(mc.MailSender)
                       select mc.MailSender;

            if (accs != null && accs.Count() != 0)
            {
                mails = accs.Distinct().ToList();
            }
            IList<MailUser> mUs = null;
            if (mails != null)
            {
                IMailAccountService accS = ServiceLocator.GetServiceFactory().MailAccountService;
                mUs = accS.GetUsersByMails(mails);
            }
            if (mUs == null)
            {
                //TASK: Allineamento log - Ciro
                ManagedException mEx = new ManagedException("Nessun account mappato per invio mail",
                    "SND_ERR_011",
                    string.Empty,
                    string.Empty,
                    null);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                //err.objectID = mail;
                _log.Error(err);
                //ManagedException mEx = new ManagedException("Nessun account mappato per invio mail", "SND_ERR_011", "", "", null);
                //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                //_log.Error(err);
                return;
            }

            foreach (SendMail.Model.ComunicazioniMapping.Comunicazioni comun in listComunicazioni)
            {
                Console.WriteLine("Id comunicazione: {0}", comun.IdComunicazione);
                SendMail.Model.ComunicazioniMapping.ComFlusso nuovoFlusso = null;
                IOrderedEnumerable<SendMail.Model.ComunicazioniMapping.ComFlusso> flusso = comun.ComFlussi[CANALE_INVIO].OrderBy(f => !f.DataOperazione.HasValue).ThenBy(f => f.DataOperazione);
                if (flusso.Last().StatoComunicazioneOld == MailStatus.SEND_AGAIN)
                {
                    nuovoFlusso = flusso.Last();
                }
                else
                {
                    nuovoFlusso =
                    new SendMail.Model.ComunicazioniMapping.ComFlusso()
                    {
                        Canale = CANALE_INVIO,
                        RefIdComunicazione = comun.IdComunicazione,
                        StatoComunicazioneOld = flusso.Last().StatoComunicazioneNew,
                        StatoComunicazioneNew = MailStatus.UNKNOWN,
                        UtenteOperazione = flusso.Last().UtenteOperazione
                    };
                    comun.ComFlussi[CANALE_INVIO].Add(nuovoFlusso);
                }

                if (!comun.IsValid)
                {
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.ERROR;
                    UpdateFlusso(comun);
                    continue;
                }

                MailUser us = mUs.SingleOrDefault(x => x.EmailAddress == comun.MailComunicazione.MailSender);
                if (us == null)
                {
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.CANCELLED;
                    UpdateFlusso(comun);
                    continue;
                }

                List<SendMail.Model.ComunicazioniMapping.ComAllegato> allegatiList =
                    (List<SendMail.Model.ComunicazioniMapping.ComAllegato>)comun.ComAllegati;
                try
                {
                    foreach (SendMail.Model.ComunicazioniMapping.ComAllegato allegato in allegatiList)
                    {
                        byte[] pdfAllegato = null;
                        try
                        {
                            switch (allegato.AllegatoExt)
                            {
                                case "PRU":
                                case "pru":
                                    System.IO.MemoryStream sPru = new System.IO.MemoryStream(allegato.AllegatoFile);
                                    XmlDocument pru = new XmlDocument();
                                    pru.Load(sPru);
                                    string tpuUri = System.IO.Path.Combine(ConfigurationManager.AppSettings.Get("pathFolderTpu"), allegato.AllegatoTpu);
                                    Com.Delta.PrintDirector.PDFBuilder b = new Com.Delta.PrintDirector.PDFBuilder(new PRUBuilder(pru.InnerXml, tpuUri));
                                    b.setTPU(tpuUri);

                                    System.IO.Stream str = null;
                                    byte[] ser = null;

                                    XDocument xTpu = XDocument.Load(tpuUri);
                                    //elementi picture
                                    var picturesTag = from nd in xTpu.Root.DescendantNodesAndSelf().OfType<XElement>()
                                                      where nd.Name.LocalName.Equals("pictureBox")
                                                      select nd;
                                    //estrae gli attributi delle picture
                                    var pictures = from nd in picturesTag
                                                   select new
                                                   {
                                                       Name = nd.Attribute("name").Value,
                                                       FileName = nd.Element("file").Attribute("value").Value,
                                                       Width = int.Parse(nd.Attribute("width").Value),
                                                       Heigth = int.Parse(nd.Attribute("height").Value)
                                                   };
                                    //aggiunge le picture alla stampa
                                    foreach (var pic in pictures)
                                    {
                                        String keyImg = System.IO.Path.Combine(ConfigurationManager.AppSettings.Get("pathFolderTpu"), pic.FileName);
                                        byte[] im = System.IO.File.ReadAllBytes(keyImg);

                                        System.IO.MemoryStream ms = new System.IO.MemoryStream(im);
                                        System.Drawing.Bitmap bb = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromStream(ms);
                                        b.AddImage(pic.Name, bb, true);
                                    }

                                    pdfAllegato = b.CreatePDF();
                                    allegato.AllegatoFile = pdfAllegato;
                                    allegato.AllegatoExt = "PDF";
                                    break;
                                case "XSL":
                                case "xsl":
                                    System.IO.MemoryStream sXML = new System.IO.MemoryStream(allegato.AllegatoFile);
                                    XmlDocument xml = new XmlDocument();
                                    xml.Load(sXML);
                                    string xslUri = System.IO.Path.Combine(ConfigurationManager.AppSettings.Get("pathFolderTpu"), allegato.AllegatoTpu);
                                    string[] separator = new string[1] { ".tpu" };
                                    string[] appo = xslUri.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                    XmlDocument xsl = new XmlDocument();
                                    xsl.Load(appo[0].Trim());
                                    // modificato per usare itextsharp
                                    PdfFormatterITEXT fo = FormatterProvider.formatDocumentitext("PDF");
                                    System.IO.MemoryStream mPdf = fo.formatData(xml, xsl);         
                                    // fine modifica
                                    IDictionary<string, string> d = new Dictionary<string, string>();
                                    d.Add("subject", "VERIFICA ABITAZIONE");
                                    d.Add("author", "ROMA CAPITALE");
                                    d.Add("creator", "Certificati Online");
                                    byte[] bdoc = fo.SetMetadati(mPdf, d);
                                    allegato.AllegatoFile = bdoc;
                                    allegato.AllegatoExt = "PDF";
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                //TASK: Allineamento log - Ciro
                                ManagedException mEx = new ManagedException("Errore nella generazione del pdf",
                                    "SEN_012",
                                    string.Empty,
                                    string.Empty,
                                    ex.InnerException);
                                mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                                    new XAttribute("IdComunicazione", comun.IdComunicazione),
                                    new XElement("Status", nuovoFlusso.StatoComunicazioneNew.ToString()),
                                    new XElement("IdMail",
                                        (comun.MailComunicazione.IdMail!=null) ? comun.MailComunicazione.IdMail.ToString():" vuoto ")).ToString());
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                                _log.Error(err); 
                                throw mEx;
                            }
                            else
                                throw;
                            //ManagedException mEx = new ManagedException(
                            //    "Errore nella generazione del pdf",
                            //    "SEN_012",
                            //    "",
                            //    new XElement("Mail",
                            //        new XAttribute("IdMail", comun.IdComunicazione),
                            //        new XElement("Status", nuovoFlusso.StatoComunicazioneNew.ToString())
                            //        ).ToString(),
                            //        ex);
                            //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                            //_log.Error(err);
                        }
                    }
                }
                catch
                {
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.ERROR;
                    UpdateFlusso(comun);
                    continue;
                }

                try
                {
                    s.UpdateAllegati(CANALE_INVIO, comun);
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                //TASK: Allineamento log - Ciro
                                ManagedException mEx = new ManagedException("Errore aggiornamento della mail",
                                    "SEN_002",
                                    string.Empty,
                                    string.Empty,
                                    ex.InnerException);
                                mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                                    new XAttribute("IdMail", comun.IdComunicazione),
                                    new XElement("Status", nuovoFlusso.StatoComunicazioneNew.ToString())).ToString());
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                                _log.Error(err); 
                            }
                            
                    //ManagedException mEx = new ManagedException(
                    //    "Errore aggiornamento della mail",
                    //    "SEN_002",
                    //    "Update Mail",
                    //    new XElement("Mail",
                    //            new XAttribute("IdMail", comun.IdComunicazione),
                    //            new XElement("Status", nuovoFlusso.StatoComunicazioneNew.ToString())
                    //            ).ToString(),
                    //    ex);
                    //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                    //_log.Error(err);
                }

                try
                {
                    ric = (Request)comun.ConvertTo(typeof(Request));
                    if (ric == null)
                    {
                        throw new ArgumentNullException("Richiesta non creata");
                    }
                    if (ric.SecurityContext == null)
                        ric.SecurityContext = new securityContext();
                    ric.SecurityContext.pUserName = us.LoginId;
                    ric.SecurityContext.pUserPassword = us.Password;

                    email email = (email)comun.ConvertTo(typeof(email));
                    if (null == email)
                    {
                        throw new ArgumentNullException("email non creata");
                    }
                    //carica le immagini
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(email.body);
                    var imgs = from img in htmlDoc.DocumentNode.Descendants("img")
                               where img.Attributes["src"].Value.StartsWith("file:///")
                               select img;
                    if (imgs.Count() > 0)
                    {
                        foreach (var img in imgs)
                        {
                            string rootPath = ConfigurationManager.AppSettings.Get("pathFolderTpu");
                            string pathImg = Path.Combine(rootPath, img.Attributes["src"].Value.Substring(8));
                            string ext = Path.GetExtension(pathImg).Substring(1);
                            byte[] imgBytes = File.ReadAllBytes(pathImg);
                            img.Attributes["src"].Value = "data:image/" + ext + ";base64,"
                                + Convert.ToBase64String(imgBytes);
                        }

                        using (Stream ms = new MemoryStream())
                        {
                            htmlDoc.Save(ms, new UTF8Encoding(false));
                            ms.Position = 0;
                            byte[] buffer = new byte[ms.Length];
                            ms.Read(buffer, 0, (int)ms.Length);
                            email.body = new UTF8Encoding(false).GetString(buffer);
                            s.UpdateMailBody(comun.MailComunicazione.IdMail.Value, email.body);
                        }
                    }
                    ric.SetFullBody(email);
                    attachments attaches = (attachments)comun.ConvertTo(typeof(attachments));
                    if (attaches != null)
                        ric.SetFullBody(attaches);
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                //TASK: Allineamento log - Ciro
                                ManagedException mEx = new ManagedException("Errore nella creazione della mail per il metabus",
                                    "SEN_003",
                                    string.Empty,
                                    string.Empty,
                                    ex.InnerException);
                                mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                                    new XAttribute("IdMail", comun.MailComunicazione.IdMail)).ToString());
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                                _log.Error(err);  
                            }
                    //ManagedException mEx = new ManagedException(
                    //    "Errore nella creazione della mail per il metabus",
                    //    "SEN_003",
                    //    "Creazione della mail per il metabus",
                    //    new XElement("Mail",
                    //            new XAttribute("IdMail", comun.IdComunicazione)
                    //            ).ToString(),
                    //    ex);
                    //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                    //_log.Error(err);

                    nuovoFlusso.StatoComunicazioneNew = MailStatus.ERROR;
                    UpdateFlusso(comun);
                    continue;
                }


                Response resp = null;
                try
                {
                    Com.Delta.MetaBus.Base.MetaBusProxy.MetaBus metabus = new Com.Delta.MetaBus.Base.MetaBusProxy.MetaBus();
                    metabus.Url = Settings.Default.Com_Delta_MetaBus_Base_MetBusProxy_MetaBus;
                    resp = metabus.SendSync(ric);
                    if (!string.IsNullOrEmpty(resp.Message.DescrizioneMessaggio) && resp.Message.DescrizioneMessaggio.ToLower().Contains("daily cos quota violation"))
                    {
                        ManagedException mEx = new ManagedException(String.Format("Errore: {0}", resp.Message.DescrizioneMessaggio),
                                "WRN_ERR_127",
                                string.Empty,
                                string.Empty,
                                null);
                        mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                            new XAttribute("Casella", comun.MailComunicazione.MailSender)).ToString());
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        _log.Warn(err);
                        Environment.Exit(0);
                    }
                    if (!string.IsNullOrEmpty(resp.Message.DescrizioneMessaggio) && resp.Message.DescrizioneMessaggio.ToLower().Contains("parameter 'address' cannot be an empty string"))
                    {
                        ManagedException mEx = new ManagedException(String.Format("Errore: {0}", resp.Message.DescrizioneMessaggio),
                                 "SND_ERR_127",
                                 string.Empty,
                                 string.Empty,
                                 null);
                        mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                            new XAttribute("IdMail", comun.IdComunicazione)).ToString());
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                        _log.Error(err);
                        nuovoFlusso.StatoComunicazioneNew = MailStatus.ERROR;
                        UpdateFlusso(comun);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(resp.Message.DescrizioneMessaggio) && resp.Message.DescrizioneMessaggio.ToLower().Contains("the specified string is not in the form required for an e-mail address."))
                    {
                        ManagedException mEx = new ManagedException(String.Format("Errore: {0}", resp.Message.DescrizioneMessaggio),
                                 "SND_ERR_128",
                                 string.Empty,
                                 string.Empty,
                                 null);
                        mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                            new XAttribute("IdMail", comun.IdComunicazione)).ToString());
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                        _log.Error(err);
                        nuovoFlusso.StatoComunicazioneNew = MailStatus.ERROR;
                        UpdateFlusso(comun);
                        continue;
                    }
                }                    
                catch (Exception ex)
                {
                    XElement details = new XElement("comunicazione",
                        new XAttribute("uniqueId", ric.RoutingInfo.UniqueId));
                    if (resp != null)
                    {
                        System.Xml.Serialization.XmlSerializer ser =
                            new System.Xml.Serialization.XmlSerializer(typeof(Response));
                        MemoryStream ms = new MemoryStream();
                        ser.Serialize(ms, resp);
                        ms.Seek(0, SeekOrigin.Begin);
                        details.Add(XElement.Load(XmlReader.Create(ms)));
                    }

                    if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
                                //TASK: Allineamento log - Ciro
                                ManagedException mEx = new ManagedException(String.Format("Errore: {0}", ex.Message),
                                    "SND_ERR_107",
                                    string.Empty,
                                    string.Empty,
                                    ex.InnerException);
                                    mEx.addEnanchedInfosTag("REQUEST", new XElement("Mail",
                                    new XAttribute("IdMail", comun.IdComunicazione)).ToString());
                                ErrorLogInfo err = new ErrorLogInfo(mEx);
                                err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                                _log.Error(err); 
                            }
                    //ErrorLogInfo err = new ErrorLogInfo(
                    //    APP_CODE, "SND_ERR_107",
                    //    "Message: " + ex.Message + " - Stack: " + ex.StackTrace +
                    //    " - Details: " +
                    //    details.ToString(SaveOptions.DisableFormatting),
                    //    mail, null, ric.RoutingInfo.UniqueId, null, null,
                    //    null, comun.IdComunicazione.Value.ToString(), null,
                    //    null, null);
                    //_log.Error(err);
                }

                if (resp != null && resp.Message.Codice.Equals("OK"))
                {
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.SENT;
                }
                else
                {
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.SEND_AGAIN;
                    System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Response));
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    ser.Serialize(ms, resp);
                    ms.Seek(0, System.IO.SeekOrigin.Begin);
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ms);


                    //TASK: Allineamento log - Ciro              
                    //System.Xml.XmlDocument doce = new System.Xml.XmlDocument();                   
                    //doce.LoadXml("<det></det>");
                    //XmlElement el = doce.CreateElement("d");
                    //XmlAttribute at = doce.CreateAttribute("n");
                    //at.Value = "DETTAGLIO";
                    //el.Attributes.Append(at);
                    //el.InnerXml = doc.InnerXml;
                    //doce.FirstChild.AppendChild(el);
                    string msg =  "Errore nella creazione della mail per il metabus. Mail: ";
                    msg += (mail == null) ? "" : mail;

                    ManagedException mEx = new ManagedException(msg,
                        "SND_ERR_100",
                         string.Empty,
                         string.Empty,
                        null);
                    mEx.addEnanchedInfosTag("REQUEST", (doc.InnerXml == null) ? "" : doc.InnerXml.Replace("<?xml version=\"1.0\"?>", ""));
                    mEx.addEnanchedInfosTag("ROUTING_INFO_ID", ric.RoutingInfo.UniqueId);
                    mEx.addEnanchedInfosTag("COMUNICAZIONE_ID", comun.IdComunicazione.Value.ToString());
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = (comun.UniqueId != null) ? comun.UniqueId : comun.IdComunicazione.ToString();
                    _log.Error(err);

                    //ErrorLogInfo err = new ErrorLogInfo(APP_CODE,
                    //    "SND_ERR_100", doc.InnerXml,
                    //    mail, null, ric.RoutingInfo.UniqueId, null, null, null,
                    //    comun.IdComunicazione.Value.ToString(), null, null, null);
                    //_log.Error(err);
                }
                UpdateFlusso(comun);
            }
        }

        private static XmlDocument SerializeRequest(Request ric, email email)
        {
            System.IO.MemoryStream str = null;
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(email));
            System.Xml.XmlDocument docRequestMail = new System.Xml.XmlDocument();
            System.Xml.XmlDocument docRequest = new System.Xml.XmlDocument();
            using (str = new System.IO.MemoryStream())
            {
                ser.Serialize(str, email);
                str.Position = 0;
                docRequestMail.Load(str);
            }

            ser = new System.Xml.Serialization.XmlSerializer(typeof(Request));
            using (str = new System.IO.MemoryStream())
            {
                ser.Serialize(str, ric);
                str.Position = 0;
                docRequest.Load(str);
            }

            System.Xml.XmlDocumentFragment frag = docRequest.CreateDocumentFragment();
            frag.InnerXml = docRequestMail.DocumentElement.OuterXml;

            System.Xml.XmlNode bodyNode = docRequest.GetElementsByTagName("Body").Item(0);
            bodyNode.AppendChild(frag);
            return docRequest;
        }

        private static void UpdateFlusso(SendMail.Model.ComunicazioniMapping.Comunicazioni c)
        {
            IComunicazioniService s = ServiceLocator.GetServiceFactory().ComunicazioniService;
            try
            {
                s.UpdateFlussoComunicazione(CANALE_INVIO, c);
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    //TASK: Allineamento log - Ciro
                    ManagedException mEx = new ManagedException("Errore nell'aggiornamento della mail",
                        "SEN_001",
                        string.Empty,
                        string.Empty,
                        ex.InnerException);
                        mEx.addEnanchedInfosTag("REQUEST", 
                            new XElement("Mail", new XAttribute("IdMail", c.MailComunicazione.IdMail),
                            new XElement("Status", c.ComFlussi[CANALE_INVIO].Last().StatoComunicazioneNew.ToString())).ToString());
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.objectID = (c.UniqueId != null) ? c.UniqueId : c.IdComunicazione.ToString();
                    _log.Error(err);
                }
                //ManagedException mEx = new ManagedException(
                //    "Errore nell'aggiornamento della mail",
                //    "SEN_001",
                //    "Update Mail",
                //    new XElement("Mail",
                //        new XAttribute("IdMail", c.IdComunicazione),
                //        new XElement("Status", c.ComFlussi[CANALE_INVIO].Last().StatoComunicazioneNew.ToString())
                //        ).ToString(),
                //    ex);
                //ErrorLogInfo err = new ErrorLogInfo(APP_CODE, mEx);
                //_log.Error(err);
            }
        }
    }
}
