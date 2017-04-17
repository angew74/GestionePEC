using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Messaging.MapperMessages;
using Com.Delta.Security;
using Com.Delta.Web;
using Com.Delta.Web.Cache;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace GestionePEC.pages.MailClient
{
    public partial class NewMail : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger("NewMailSender");

        private static readonly string regexMail = RegexUtils.EMAIL_REGEX.ToString();

        private string _comunicazione;

        public string Comunicazione
        {
            get
            {
                if (SessionManager<string>.exist(SessionKeys.TESTO_MAIL))
                {
                    string html = string.Empty;
                    if (HidHtml.Value != string.Empty)
                    {
                        html = HidHtml.Value;
                        SessionManager<string>.set(SessionKeys.TESTO_MAIL, html);
                    }
                    else
                    {
                        html = SessionManager<string>.get(SessionKeys.TESTO_MAIL);
                    }
                    try
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        string htmlView = html.Replace("'", "''");
                        HidHtml.Value = htmlView;
                        doc.LoadHtml(htmlView);
                        if (doc.ParseErrors.Count() == 0)
                        {
                            var n = (from node in doc.DocumentNode.Descendants()
                                     where node.Name.Equals("body", StringComparison.InvariantCultureIgnoreCase)
                                     select node.FirstChild).SingleOrDefault();
                            if (n != null)
                                html = n.InnerHtml;
                        }
                    }
                    catch
                    {
                    }
                    return html;
                }
                else
                {
                    return _comunicazione;
                }
            }
            set
            {
                string htmlView = value.Replace("'", "''");
                _comunicazione = htmlView;
            }
        }
            

        protected void Login_OnChangeStatus(object sender, EventArgs e)
        {

        }

        protected void ibRemoveAll_Click(object sender, ImageClickEventArgs e)
        {
            // rimuove l'allegato dalla sessione            
            // switch view
            ImageButton lb = sender as ImageButton;
            string id = lb.CommandArgument;
            Dictionary<string, DTOFileUploadResult> dict = new Dictionary<string, DTOFileUploadResult>();
            List<DTOFileUploadResult> list = new List<DTOFileUploadResult>();
            if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
            {
                dict = SessionManager<Dictionary<string, DTOFileUploadResult>>.get(SessionKeys.DTO_FILE);
                dict.Remove(id);
                SessionManager<Dictionary<string, DTOFileUploadResult>>.set(SessionKeys.DTO_FILE, dict);
                if (dict.Values.Count > 0) { list = (from obj in dict.Values.Where(p => p.errormessage != "Upload non riuscito.") select obj).ToList(); }
            }         

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            MailUser mailuser = WebMailClientManager.getAccount();
            if (!(WebMailClientManager.AccountIsValid()))
            {
                (this.Page as BasePage).info.AddMessage("Account non più valido. Ripetere la selezione della casella di posta", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
            }
            if (Page.IsPostBack)
            {
                string test = HidHtml.Value;
                if (!(string.IsNullOrEmpty(test)))
                {
                    SessionManager<string>.set(SessionKeys.TESTO_MAIL, test);
                }
            }
            else
            {
                SessionManager<string>.del(SessionKeys.TESTO_MAIL);
            }
        }

        protected void Login_OnNewMail(object sender, EventArgs e)
        {
            //Response.Redirect("~/pages/mail/NewMailSender.aspx");
            Redirect("~/pages/MailClient/NewMail.aspx", null);
        }

        protected void btnSend_OnClick(object sender, EventArgs e)
        {
            if (HeaderNew.DestinatarioSelected != null && HeaderNew.DestinatarioSelected.Length > 0)
            {
                if (HeaderNew.SottoTitoloSelected != string.Empty && HeaderNew.TitoloSelected != "-- Selezionare un titolo --" && HeaderNew.SottoTitoloSelected.Length > 0)
                {
                    try
                    {

                        SendMail.Model.ComunicazioniMapping.Comunicazioni comun = new SendMail.Model.ComunicazioniMapping.Comunicazioni();
                        // gestione allegati
                        List<ComAllegato> allegati = new List<ComAllegato>();
                        if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
                        {

                            Dictionary<string, DTOFileUploadResult> dictDto = SessionManager<Dictionary<string, DTOFileUploadResult>>.get(SessionKeys.DTO_FILE);
                            List<DTOFileUploadResult> dto = (List<DTOFileUploadResult>)dictDto.Values.ToList();
                            foreach (DTOFileUploadResult d in dto)
                            {
                                SendMail.Model.ComunicazioniMapping.ComAllegato allegato = new SendMail.Model.ComunicazioniMapping.ComAllegato();
                                allegato.AllegatoExt = d.Extension;
                                allegato.AllegatoFile = d.CustomData;
                                allegato.AllegatoName = d.FileName;
                                allegato.AllegatoTpu = "";
                                allegato.FlgInsProt = AllegatoProtocolloStatus.FALSE;
                                allegato.FlgProtToUpl = AllegatoProtocolloStatus.FALSE;
                                allegati.Add(allegato);
                            }

                        }

                        comun.ComAllegati = allegati;
                        string[] destinatari = HeaderNew.DestinatarioSelected.Split(';');                       
                        for (int i = 0; i < destinatari.Length; i++)
                        {
                            var match = Regex.Match(destinatari[i], regexMail, RegexOptions.IgnoreCase);
                            if (!match.Success)
                            {                              
                                ((BasePage)this.Page).info.AddMessage("Attenzione mail destinatario non valida ", LivelloMessaggio.ERROR);
                                return;
                            }
                        }
                        // gestione destinatari
                        ContactsApplicationMapping c = new ContactsApplicationMapping
                        {
                            Mail = HeaderNew.DestinatarioSelected,
                            IdSottotitolo = long.Parse(HeaderNew.SottoTitoloSelected),
                            IdTitolo = long.Parse(HeaderNew.TitoloSelected)
                        };
                        Collection<ContactsApplicationMapping> en = new Collection<ContactsApplicationMapping>();
                        en.Add(c);
                        if (HeaderNew.ConoscenzaSelected != string.Empty)
                        {
                            string[] conoscenze = HeaderNew.ConoscenzaSelected.Split(';');
                            for (int i = 0; i < conoscenze.Length; i++)
                            {
                                var match = Regex.Match(conoscenze[i], regexMail, RegexOptions.IgnoreCase);
                                if (!match.Success)
                                {                                
                                    ((BasePage)this.Page).info.AddMessage("Attenzione mail conoscenza non valida ", LivelloMessaggio.ERROR);
                                    return;
                                }
                            }
                            ContactsApplicationMapping c1 = new ContactsApplicationMapping
                            {
                                Mail = HeaderNew.ConoscenzaSelected,
                                IdSottotitolo = long.Parse(HeaderNew.SottoTitoloSelected),
                                IdTitolo = long.Parse(HeaderNew.TitoloSelected)
                            };
                            Collection<ContactsApplicationMapping> en1 = new Collection<ContactsApplicationMapping>();
                            en1.Add(c1);
                            comun.SetMailDestinatari(en1, AddresseeType.CC);
                        }
                        if (HeaderNew.BCCSelected != string.Empty)
                        {
                            string[] bcc = HeaderNew.BCCSelected.Split(';');
                            for (int i = 0; i < bcc.Length; i++)
                            {
                                var match = Regex.Match(bcc[i], regexMail, RegexOptions.IgnoreCase);
                                if (!match.Success)
                                {                                    
                                    ((BasePage)this.Page).info.AddMessage("Attenzione mail BCC non valida ", LivelloMessaggio.ERROR);
                                    return;
                                }
                            }

                            ContactsApplicationMapping c2 = new ContactsApplicationMapping
                            {
                                Mail = HeaderNew.BCCSelected,
                                IdSottotitolo = long.Parse(HeaderNew.SottoTitoloSelected),
                                IdTitolo = long.Parse(HeaderNew.TitoloSelected)
                            };
                            Collection<ContactsApplicationMapping> en2 = new Collection<ContactsApplicationMapping>();
                            en2.Add(c2);
                            comun.SetMailDestinatari(en2, AddresseeType.CCN);
                        }
                        comun.SetMailDestinatari(en, AddresseeType.TO);
                        // gestione body email
                        MailContent content = new MailContent();
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        string html = CacheManager<XmlDocument>.EmptyHtml;
                        HtmlAgilityPack.HtmlNode body = null;
                        if (string.IsNullOrEmpty(html) == false)
                        {
                            doc.LoadHtml(html);
                            doc.OptionAutoCloseOnEnd = true;
                            doc.OptionFixNestedTags = true;
                            body = doc.DocumentNode.Descendants()
                                                       .Where(n => n.Name.Equals("body", StringComparison.InvariantCultureIgnoreCase))
                                                       .FirstOrDefault();
                        }
                        var ele = new HtmlAgilityPack.HtmlNode(HtmlAgilityPack.HtmlNodeType.Element, doc, 0);
                        ele.Name = "div";
                        ele.InnerHtml = HidHtml.Value;
                        content.MailSubject = HeaderNew.ObjectSelected;
                        content.MailText = ele.OuterHtml;
                        // gestione sender 
                        MailUser mailuser = WebMailClientManager.getAccount();
                        content.MailSender = mailuser.EmailAddress;
                        c.IdTitolo = long.Parse(HeaderNew.TitoloSelected);
                        c.IdSottotitolo = long.Parse(HeaderNew.SottoTitoloSelected);
                        comun.UtenteInserimento = MySecurityProvider.CurrentPrincipal.Identity.Name;
                        comun.MailComunicazione = content;
                        // da scommentare
                        comun.FolderTipo = "O";
                        comun.FolderId = 2;
                        ComunicazioniService com = new ComunicazioniService();
                        com.InsertComunicazione(comun);
                        // ((baseLayoutUnisys.BasePage)this.Page).info.ClearMessage();
                        ((BasePage)this.Page).info.AddMessage("Email correttamente inviata", LivelloMessaggio.INFO);
                        btnSend.Visible = false;
                        SessionManager<string>.del("APP_CODE");
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() != typeof(ManagedException))
                        {                           
                            ManagedException mEx = new ManagedException("Errore invio nuova mail. Dettaglio: " + ex.Message +
                                "StackTrace: " + ((ex.StackTrace != null) ? ex.StackTrace.ToString() : " vuoto "),
                                "ERR317",
                                string.Empty,
                                string.Empty,
                                ex.InnerException);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            log.Error(err);
                        }                       
                        btnSend.Enabled = true;                       
                        ((BasePage)this.Page).info.AddMessage(/*error.logCode + */" - Errore durante l'inserimento della mail:" + ex.Message, LivelloMessaggio.ERROR);
                    }
                    return;
                }             
                ((BasePage)this.Page).info.AddMessage("Attenzione sottotitolo non selezionato ", LivelloMessaggio.ERROR);
                return;
            }
            else
            {                       
                ((BasePage)this.Page).info.AddMessage("Attenzione email destinatario non corretta ", LivelloMessaggio.ERROR);
                return;
            }
        }
    }
}