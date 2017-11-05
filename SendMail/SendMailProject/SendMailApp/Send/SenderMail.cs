#define TEST
#region "using"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using log4net;
using SendMail.Model;
using System.Xml;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using System.Xml.Linq;
using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using SendMailApp.Formatter;
using System.IO;
using Com.Delta.Logging.Mail;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using System.Net.Mail;
using SendMail.Model.ComunicazioniMapping;
using System.Net;

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
            MailMessage email = new MailMessage();
            List<SendMail.Model.ComunicazioniMapping.Comunicazioni> listComunicazioni = null;
            ComunicazioniService service = new ComunicazioniService();
            listComunicazioni = (List<SendMail.Model.ComunicazioniMapping.Comunicazioni>)service.LoadComunicazioniDaInviare(CANALE_INVIO, 0, maxNrMail, mail);
            if (listComunicazioni == null || listComunicazioni.Count == 0)
            {
                _log.Info(new MailLogInfo(APP_CODE, "", "BATCH", mail,
                    "Nessuna comunicazione da inviare"));
                return;
            }
#if TEST
           // listComunicazioni = listComunicazioni.Where(c => c.IdComunicazione == 54373).ToList(); 
#endif
            Console.WriteLine("Trovate {0} comunicazioni", listComunicazioni.Count);
            _log.Info(new MailLogInfo(APP_CODE, "", "BACTH", mail,
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
                MailAccountService accS = new MailAccountService(); 
                mUs = accS.GetUsersByMails(mails);
            }
            if (mUs == null)
            {
                ManagedException mEx = new ManagedException("Nessun account mappato per invio mail",
                    "SND_ERR_011",
                    string.Empty,
                    string.Empty,
                    null);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                _log.Error(err);
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
                        try
                        {
                            switch (allegato.AllegatoExt)
                            {
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
                    service.UpdateAllegati(CANALE_INVIO, comun);
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
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
                  
                }
                try
                {

                    email = ComunicazioniExtensionMethods.ConvertToEmail(comun);
                    if (null == email)
                    {
                        throw new ArgumentNullException("email non creata");
                    }
                    //carica le immagini
                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(email.Body);
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
                            email.Body = new UTF8Encoding(false).GetString(buffer);
                            service.UpdateMailBody(comun.MailComunicazione.IdMail.Value, email.Body);
                        }
                    }
                    foreach (ComAllegato t in comun.ComAllegati)
                    { email.Attachments.Add(ComunicazioniExtensionMethods.ConvertToAttachment(t)); }
                   
                }
                catch (Exception ex)
                {
                    if (!ex.GetType().Equals(typeof(ManagedException)))
                            {
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

                    nuovoFlusso.StatoComunicazioneNew = MailStatus.ERROR;
                    UpdateFlusso(comun);
                    continue;
                }
                try
                {
                    
                    MailUser user= MailServerConfigFacade.GetInstance().GetManagedUserByAccount(email.From.Address);
                    System.Net.Mail.SmtpClient smtpMail = new System.Net.Mail.SmtpClient();
                    smtpMail = new System.Net.Mail.SmtpClient(user.OutgoingServer, user.PortOutgoingServer);
                    smtpMail.EnableSsl = user.IsOutgoingSecureConnection;
                    smtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpMail.UseDefaultCredentials = false;
                    if (user.LoginId != null && user.Password != null && user.LoginId.Length > 0)
                    {
                        smtpMail.Credentials = new NetworkCredential(user.LoginId, user.Password);
                    }
                    smtpMail.Send(email);
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.SENT;
                    UpdateFlusso(comun);
                }
                catch (Exception ex)
                {
                    XElement details = new XElement("comunicazione",
                        new XAttribute("uniqueId", comun.UniqueId));
                    if (ex != null)
                    {
                        System.Xml.Serialization.XmlSerializer ser =
                            new System.Xml.Serialization.XmlSerializer(typeof(Exception));
                        MemoryStream ms = new MemoryStream();
                        ser.Serialize(ms, ex);
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
                    nuovoFlusso.StatoComunicazioneNew = MailStatus.SEND_AGAIN;
                    UpdateFlusso(comun);
                }
                
                    
             
                
                
            }
        }

     

        private static void UpdateFlusso(SendMail.Model.ComunicazioniMapping.Comunicazioni c)
        {
            ComunicazioniService s = new ComunicazioniService();
            try
            {
                s.UpdateFlussoComunicazione(CANALE_INVIO, c);
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                { 
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
            }
        }
    }
}
