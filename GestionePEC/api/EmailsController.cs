using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Models;
using HtmlAgilityPack;
using log4net;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model;
using SendMail.Model.ComunicazioniMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace GestionePEC.api
{
    public class EmailsController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("EmailsController");
        [Authorize]
        [HttpGet]
        [Route("api/EmailsController/GetMails")]
        public HttpResponseMessage GetMails(int? page, int? start, int? limit, string sort, string dir)
        {
            BackendUser _bUser;
            BackendUserModel model = new BackendUserModel();
            try
            {
                int starti = start.HasValue ? Convert.ToInt32(start) : 1;
                int recordPagina = limit.HasValue ? Convert.ToInt32(limit) : 5;
                if (!(SessionManager<BackendUser>.exist(SessionKeys.BACKEND_USER)))
                {
                    BackendUserService buservice = new BackendUserService();
                    _bUser = (BackendUser)buservice.GetByUserName(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                    SessionManager<BackendUser>.set(SessionKeys.BACKEND_USER, _bUser);
                }
                else { _bUser = SessionManager<BackendUser>.get(SessionKeys.BACKEND_USER); }
                List<BackEndUserMailUserMapping> listMailSender = null;
                if (_bUser != null)
                {
                    listMailSender = _bUser.MappedMails;
                    if (listMailSender != null && listMailSender.Count > 0)
                    {
                        listMailSender = listMailSender.OrderBy(ms => ms.EmailAddress).ToList();
                        model.Totale = listMailSender.Count.ToString();
                        model.ListBackendUsers = listMailSender.Skip(starti).Take(recordPagina).ToList();
                    }
                }
                else
                {
                    model.Totale = "0";
                    model.success = "true";
                }

            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_E001";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                model.message = ex.Message;
                model.success = "false";
                return this.Request.CreateResponse<BackendUserModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<BackendUserModel>(HttpStatusCode.OK, model);

        }

        [Authorize]
        [HttpGet]
        [Route("api/EmailsController/GetMailSendersByUserMails")]
        public HttpResponseMessage GetMailSendersByUserMails(int? page, int? start, int? limit)
        {
            MailServerConfigFacade mailServerConfigFacade = MailServerConfigFacade.GetInstance();
            List<MailUser> users = new List<MailUser>();
            UsersMailModel model = new UsersMailModel();
            try
            {

                string username = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
                MailServerConfigFacade mailSCF = null;
                mailSCF = MailServerConfigFacade.GetInstance();
                users = SessionManager<List<MailUser>>.get(SessionKeys.ACCOUNTS_LIST);
                if (!(users != null && users.Count != 0))
                {
                    users = mailSCF.GetManagedAccountByUser(username).ToList();
                    if (users == null) users = new List<MailUser>();
                    if (users.Where(x => x.UserId.Equals(-1)).Count() == 0)
                        users.Insert(0, new MailUser() { UserId = -1, EmailAddress = "" });
                    SessionManager<List<MailUser>>.set(SessionKeys.ACCOUNTS_LIST, users);
                }
                model.MailUsers = users;
            }
            catch (ManagedException bex)
            {
                if (bex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(bex.Message, "ERR_E002", string.Empty, string.Empty, bex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    model.success = "false";
                    model.message = bex.Message;
                    return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
                }
            }
            return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [HttpGet]
        [Route("api/EmailsController/GetMail")]
        public HttpResponseMessage GetMail(int idmail)
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
            SessionManager<Dictionary<string, DTOFileUploadResult>>.del(SessionKeys.DTO_FILE);
            MailModel model = new MailModel();
            SessionManager<List<ViewAttachement>>.del(SessionKeys.ATTACHEMENTS_LIST);
            ViewMail v = new ViewMail();
            if (mailUser == null)
            {
                model.message = "Account non selezionato";
                model.success = "false";
                return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
            }
            try
            {
                Message msg = GetCurrentMessage();
                if (msg == null)
                {
                    model.message = "Messaggio non valido ripetere la selezione";
                    model.success = "false";
                    return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
                }
                v.Mail = mailUser.EmailAddress;
                if (selectEmail(msg.To).Contains(mailUser.EmailAddress))
                {
                    List<Address> l = msg.To.Where(x => !x.Email.Equals(mailUser.EmailAddress)).ToList();
                    v.DestinatarioA = string.Join(";", (from e in l
                                                        select e.Email).ToArray());
                    v.DestinatarioA +=";" + msg.From.Email;
                }
                else
                {
                    v.DestinatarioA = msg.From.Email;
                }
                v.DestinatarioABlank = false;
                if (selectEmail(msg.Cc).Contains(mailUser.EmailAddress))
                {
                    List<Address> lc = msg.Cc.Where(x => !x.Email.Equals(mailUser.EmailAddress)).ToList();
                    v.DestinatarioCC = string.Join(";", (from e in lc
                                                         select e.Email).ToArray());
                }
                v.Oggetto = String.Concat("Re:", msg.Subject);
                v.TestoMail = msg.BodyText.TextStripped;
                v.Allegati = new List<ViewAttachement>();
                if (msg.Attachments.Count > 0)
                {
                    foreach (MimePart a in msg.Attachments)
                    {
                        ViewAttachement va = new ViewAttachement();
                        va.NomeFile = a.Filename;
                        va.ContentiId = a.ContentId;
                        va.Dimensione = a.Size;
                        v.Allegati.Add(va);
                    }
                }
                model.Mail = new List<ViewMail>();
                model.Mail.Add(v);
                model.success = "true";
                model.Totale = "1";
                SessionManager<List<ViewAttachement>>.set(SessionKeys.ATTACHEMENTS_LIST, model.Mail.FirstOrDefault().Allegati);
            }
            catch (Exception bex)
            {
                if (bex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(bex.Message, "ERR_E005", string.Empty, string.Empty, bex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    model.success = "false";
                    model.message = bex.Message;
                    return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
                }

            }
            return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);

        }

        [Authorize]
        [HttpGet]
        [Route("api/EmailsController/GetAttachements")]
        public HttpResponseMessage GetAttachements(int? page, int? start, int? limit)
        {
            AttachementsModel model = new AttachementsModel();
            model.AttachementsList = new List<ViewAttachement>();
            model.success = "true";
            model.Totale = "0";
            if (SessionManager<List<ViewAttachement>>.exist(SessionKeys.ACCOUNTS_LIST))
            {
                List<ViewAttachement> list = SessionManager<List<ViewAttachement>>.get(SessionKeys.ATTACHEMENTS_LIST);
                model.AttachementsList = list;
                model.Totale = list.Count.ToString();
            }
            return this.Request.CreateResponse<AttachementsModel>(HttpStatusCode.OK, model);
        }

        [HttpPost]
        [Authorize]
        [Route("api/EmailsController/SendMailExt")]
        public HttpResponseMessage SendMailExt(FormDataCollection collection)
        {
            MailModel model = new MailModel();
            string bodyBag;
            try
            {
                Message msg;
                ComunicazioniService comunicazioniService = new ComunicazioniService();
                if (MailMessageComposer.CurrentSendMailExist())
                    msg = MailMessageComposer.CurrentSendMailGet();
                else
                    msg = new Message();

                msg.Subject = collection["Oggetto"];
                if (String.IsNullOrEmpty(collection["DestinatarioA"]) &&
                    String.IsNullOrEmpty(collection["DestinatarioCC"]) &&
                    String.IsNullOrEmpty(collection["DestinatarioBCC"]))
                {
                    model.message = "Inserire almeno un destinatario";
                    model.success = "false";
                    return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
                }
                msg.To.Clear();
                msg.Cc.Clear();
                msg.Bcc.Clear();
                this.addEmailsTo(collection["DestinatarioA"].Trim(), msg);
                if (!(string.IsNullOrEmpty(collection["DestinatarioCC"])))
                { this.addEmailsCc(collection["DestinatarioCC"], msg); }
                if (!(string.IsNullOrEmpty(collection["DestinatarioBCC"])))
                { this.addEmailCcn(collection["DestinatarioBCC"], msg); }
                msg.Date = System.DateTime.Now;
                //mantengo il vecchio testo perché in caso di ErrorEventArgs lo devo ripristinare
                bodyBag = msg.BodyHtml.Text;
                SendMail.Model.BodyChunk bb = new SendMail.Model.BodyChunk();
                string txt = collection["TestoMail"];
                string[] lst = txt.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string l in lst)
                {
                    bb.Line.Add(l);
                }
                //inserisco il testo libero in testa a quello preformattato
                HtmlNode newNode = null;
                if (bb.Line.Count != 0)
                    newNode = HtmlNode.CreateNode(bb.getAsHtml());
                HtmlDocument d = new HtmlDocument();
                if (newNode != null)
                {
                    if (d.DocumentNode.Descendants().Count() != 0)
                    {
                        HtmlNode root = d.DocumentNode.Descendants().SingleOrDefault(x => x.Name.Equals("body", StringComparison.InvariantCultureIgnoreCase));
                        if (root == null)
                        {
                            root = d.DocumentNode.Descendants().FirstOrDefault(x => x.NodeType == HtmlNodeType.Element);
                        }
                        if (root != null)
                        {
                            root.PrependChild(newNode);
                        }
                        else
                        {
                            d.DocumentNode.PrependChild(newNode);
                        }
                    }
                    else
                    {
                        d.DocumentNode.PrependChild(newNode);
                    }
                }
                msg.BodyHtml.Text = d.DocumentNode.InnerHtml;
                //se non sono inclusi gli allegati originali
                if (!(string.IsNullOrEmpty(collection["IncludiAllegati"])) && collection["IncludiAllegati"].ToUpper() == "FALSE")
                {
                    for (int i = 0; i < msg.Attachments.Count; i++)
                    {
                        //rimuovo gli allegati originali
                        if (msg.Attachments[i].ParentMessage != null)
                            msg.Attachments.RemoveAt(i);
                    }
                }
                foreach (MimePart mm in msg.Attachments)
                {
                    if (mm.BinaryContent == null || mm.BinaryContent.Length < 10)
                    {
                        if (!String.IsNullOrEmpty(mm.ContentId))
                        {
                            string idAttach = mm.ContentId.Trim(new char[] { '<', '>' });
                            long idAtt = -1;
                            if (long.TryParse(idAttach, out idAtt))
                            {
                                ComAllegato all = comunicazioniService
                                            .LoadAllegatoComunicazioneById(long.Parse(idAttach));
                                mm.BinaryContent = all.AllegatoFile;
                            }
                        }
                    }
                }
                msg.InReplyTo = msg.Id.ToString();
                ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = null;
                if (WebMailClientManager.AccountExist())
                {
                    mailUser = WebMailClientManager.getAccount();
                }

                if (mailUser != null)
                {
                    MailServerFacade f = MailServerFacade.GetInstance(mailUser);
                    msg.From = new Address(mailUser.Casella);
                    if (mailUser.IsManaged)
                    {
                        try
                        {
                            SendMail.Model.ComunicazioniMapping.Comunicazioni c =
                                new SendMail.Model.ComunicazioniMapping.Comunicazioni(
                                     SendMail.Model.TipoCanale.MAIL,
                                     "0",
                                     msg,
                                     HttpContext.Current.User.Identity.Name, 2, "O");
                            if (c.MailComunicazione.MailRefs != null && c.MailComunicazione.MailRefs.Count != 0)
                            {
                                c.RubricaEntitaUsed = (from cont in c.MailComunicazione.MailRefs
                                                       select new SendMail.Model.ComunicazioniMapping.RubrEntitaUsed
                                                       {
                                                           Mail = cont.MailDestinatario,
                                                           TipoContatto = cont.TipoRef
                                                       }).ToList();
                            }
                            if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
                            {
                                Dictionary<string, DTOFileUploadResult> dictDto = SessionManager<Dictionary<string, DTOFileUploadResult>>.get(SessionKeys.DTO_FILE);
                                List<DTOFileUploadResult> dto = (List<DTOFileUploadResult>)dictDto.Values.ToList();
                                c.ComAllegati = new List<ComAllegato>();
                                foreach (DTOFileUploadResult dd in dto)
                                {
                                    ComAllegato allegato = new SendMail.Model.ComunicazioniMapping.ComAllegato();
                                    allegato.AllegatoExt = dd.Extension;
                                    allegato.AllegatoFile = dd.CustomData;
                                    allegato.AllegatoName = dd.FileName;
                                    allegato.AllegatoTpu = "";
                                    allegato.FlgInsProt = AllegatoProtocolloStatus.FALSE;
                                    allegato.FlgProtToUpl = AllegatoProtocolloStatus.FALSE;
                                    c.ComAllegati.Add(allegato);
                                }

                            }
                            comunicazioniService.InsertComunicazione(c);
                        }
                        catch (Exception ex)
                        {
                            ManagedException mex = new ManagedException("Errore nel salvataggio della mail",
                                "MAIL_CMP_002", "", ex.StackTrace, ex);
                            ErrorLog err = new ErrorLog(mex);
                            log.Error(err);
                            model.message = "Errore nell'invio del messaggio";
                            model.success = "false";
                            return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
                        }
                    }
                    else
                    {
                        f.sendMail(msg);
                    }

                    model.message = "Email memorizzata correttamente";
                    model.success = "true";
                    return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
                }
                else
                {
                    model.message = "Account inesistente";
                    model.success = "false";
                    return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                    log.Error(new Com.Delta.Logging.Errors.ErrorLog(new ManagedException(ex.Message, "FAC_007", string.Empty, string.Empty, ex)));
               
                model.message = ex.Message;
                model.success = "false";
                return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
            }
            model.message = "Email memorizzata correttamente";
            model.success = "true";
            return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Metodo per il caricamento del messaggio corrente
        /// </summary>
        /// <returns></returns>
        private Message GetCurrentMessage()
        {
            if (MailMessageComposer.CurrentSendMailExist())
            {
                return MailMessageComposer.CurrentSendMailGet();
            }
            else
            {
                Message msg = GetOriginalMessage();
                if (msg == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        int idOri = msg.Id;
                        if (msg.OriginalData == null)
                        { msg = Parser.ParseMessage(msg.ToMimeString()); }
                        else
                        {
                            msg = Parser.ParseMessage(msg.OriginalData);
                        }
                        msg.Id = idOri;
                    }
                    catch (OutOfMemoryException oomEx)
                    {
                        ManagedException mEx = new ManagedException(
                            "Memoria insufficiente. Chiudere il popup e riprovare.",
                            "MAIL_CMP_001",
                            string.Empty,
                            string.Empty,
                            oomEx);
                        ErrorLog err = new ErrorLog(mEx);
                        log.Error(err);
                        throw;

                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() != typeof(ManagedException))
                        {
                            ManagedException mEx = new ManagedException(
                                "Errore nel recupero del messaggio originale",
                                "MAIL_CMP_002",
                                string.Empty,
                                string.Empty,
                                ex);
                            ErrorLog err = new ErrorLog(mEx);
                            log.Error(err);
                            throw;
                        }
                        throw;
                    }
                }
                MailMessageComposer mc = new MailMessageComposer();
                mc.message = msg;
                return msg;
            }
        }
        /// <summary>
        /// Metodo per il caricamento del messaggio originale
        /// </summary>
        /// <returns></returns>
        private Message GetOriginalMessage()
        {
            Message msg = WebMailClientManager.CurrentMailGet();
            if (msg == null)
            {
                return null;
            }

            if (!String.IsNullOrEmpty(msg.HeaderFields["x-trasporto"])
                && String.Equals(msg.HeaderFields["x-trasporto"], "posta-certificata", StringComparison.InvariantCultureIgnoreCase))
            {
                int id = msg.Id;
                msg = msg.SubMessages[0];
                msg.Id = id;
            }
            return msg;
        }

        /// <summary>
        /// Funzione di selezione delle email
        /// </summary>
        Func<AddressCollection, string[]> selectEmail = x =>
        {
            var m = x.Select(y => y.Email);
            if (m.Count() != 0)
                return m.ToArray();
            else return new string[0];
        };

        #region "Gestione Invio Messaggi"
        private void addEmailsTo(string mails, Message message)
        {
            mails = mails.Trim();
            mails.Replace(" ", "");
            string[] toArray = mails.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            for (int i = 0; i < toArray.Length; i++)
                if (toArray[i] != String.Empty)
                    if (Regex.IsMatch(toArray[i], pattern))
                    {
                        if (!selectEmail(message.To).Contains(toArray[i]))
                            message.To.Add(toArray[i]);
                    }
                    else
                    {
                        //Allineamento log - Ciro 
                        ManagedException mEx = new ManagedException("La mail " + toArray[i] + "non è una mail valida", "ERR_MF_001", string.Empty, string.Empty, null);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        er.objectID = mails;
                        log.Error(er);
                        throw mEx;
                    }
            //throw new ManagedException("La mail " + toArray[i] + "non è una mail valida", "ERR_MF_001", null, null, null, null, null, null);
        }

        private void addEmailsCc(string mails, Message message)
        {
            mails = mails.Trim();
            string[] ccArray = mails.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            for (int i = 0; i < ccArray.Length; i++)
                if (ccArray[i] != String.Empty)
                    if (Regex.IsMatch(ccArray[i], pattern))
                    {
                        if (!selectEmail(message.Cc).Contains(ccArray[i]))
                            message.Cc.Add(ccArray[i]);
                    }
                    else
                    {
                        ManagedException mEx = new ManagedException("La mail " + ccArray[i] + "non è una mail valida", "ERR_MF_002", string.Empty, string.Empty, null);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        er.objectID = mails;
                        log.Error(er);
                        throw mEx;
                    }
            //throw new ManagedException("La mail " + ccArray[i] + "non è una mail valida", "ERR_MF_002", null, null, null, null, null, null);
        }

        private void addEmailCcn(string mails, Message message)
        {
            mails = mails.Trim();
            string[] bccArray = mails.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            for (int i = 0; i < bccArray.Length; i++)
            {
                if (bccArray[i] != String.Empty)
                    if (Regex.IsMatch(bccArray[i], pattern))
                    {
                        if (!selectEmail(message.Bcc).Contains(bccArray[i]))
                            message.Bcc.Add(bccArray[i]);
                    }
                    else
                    {
                        ManagedException mEx = new ManagedException("ERR_MF_003", "ERR_MF_002", string.Empty, string.Empty, null);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        er.objectID = mails;
                        log.Error(er);
                        throw mEx;
                    }
            }
        }
        #endregion

    }
}
