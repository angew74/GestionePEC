using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using GestionePEC.Extensions;
using HtmlAgilityPack;
using log4net;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model.ComunicazioniMapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class MailComposer : System.Web.UI.UserControl
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(MailComposer));
        private const string CURRENT_MAIL_ACTION = "CURRENT_MAIL_ACTION";
        private const string ENABLE_ATTACHMENTS = "ENABLE_ATTACHMENTS";
        private const string ENABLE_NEW_ATTACHMENTS = "ENABLE_NEW_ATTACHMENTS";
        private const string SOTTOTITOLO = "SOTTOTITOLO";
        private const string DIVS = "DIVS";

        protected enum Divs
        {
            NewAttachments,
            Attachments
        }

        #region "Public Properties"
        public string SottoTitolo
        {
            get
            {
                return ((string)ViewState[SOTTOTITOLO] ?? "0");
            }
            set
            {
                ViewState[SOTTOTITOLO] = value;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableAttachments
        {
            get
            {
                object e = ViewState[ENABLE_ATTACHMENTS];
                if (e == null) return true;
                return (bool)e;
            }
            set
            {
                ViewState[ENABLE_ATTACHMENTS] = value;
            }
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableNewAttachments
        {
            get
            {
                object e = ViewState[ENABLE_NEW_ATTACHMENTS];
                if (e == null) return false;
                return (bool)e;
            }
            set
            {
                ViewState[ENABLE_NEW_ATTACHMENTS] = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MailActions CurrentAction
        {
            get
            {
                MailActions act = MailActions.NONE;
                try
                {
                    act = (MailActions)ViewState[CURRENT_MAIL_ACTION];
                }
                catch { }
                return act;
            }
            set
            {
                ViewState[CURRENT_MAIL_ACTION] = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Message CurrentMessage
        {
            get
            {
                Message msg = MailMessageComposer.CurrentSendMailGet();
                if (msg == null) msg = new Message();
                return msg;
            }
        }

        private const string MAIL_EDITABLE = "MAIL_EDITABLE";
        public bool MailEditabile
        {
            get
            {
                object _MailEditable = ViewState[MAIL_EDITABLE];
                if (_MailEditable == null)
                    return true;
                else return (bool)_MailEditable;
            }
            set
            {
                ViewState[MAIL_EDITABLE] = value;
            }
        }

        protected Dictionary<Divs, bool> DivsVisibility
        {
            get
            {
                object divs = ViewState[DIVS];
                if (divs == null)
                {
                    Dictionary<Divs, bool> dic = new Dictionary<Divs, bool>();
                    dic.Add(Divs.Attachments, false);
                    dic.Add(Divs.NewAttachments, false);
                    ViewState[DIVS] = dic;
                    return dic;
                }
                return (Dictionary<Divs, bool>)divs;
            }
        }
        #endregion

        #region "Delegates & events"

        /// <summary>
        /// Notifica l'azione di mail inviata.
        /// </summary>
        public event EventHandler MailSent;
        private void onMailSent()
        {
            if (MailSent != null)
                MailSent(this, EventArgs.Empty);
        }

        public event EventHandler AccountInvalid;
        private void onAccountInvalid()
        {
            if (AccountInvalid != null)
                AccountInvalid(this, EventArgs.Empty);
            this.Visible = false;
        }

        public event EventHandler MessageInvalid;
        private void onMessageInvalid()
        {
            if (MessageInvalid != null)
                MessageInvalid(this, EventArgs.Empty);
            this.Visible = false;
        }
        #endregion

        #region "Initialize"
        public void Initialize(MailActions action, String sottoTitolo)
        {
            ErrorLabel.Visible = false;
            Label5.Visible = false;
            this.SottoTitolo = sottoTitolo;
            CurrentAction = action;
            BodyTextBox.Text = string.Empty;
            if (MailMessageComposer.CurrentSendMailExist())
            {
                BodyTextBox.Text = MailMessageComposer.CurrentSendMailGet().BodyText.Text;
            }
            this.Visible = true;
            switch (action)
            {
                case MailActions.REPLY_ALL:
                    DivsVisibility[Divs.Attachments] = false;
                    DivsVisibility[Divs.NewAttachments] = true;
                    if (LoadMessageReplyAll() == false)
                        return;
                    break;
                case MailActions.REPLY_TO:
                    DivsVisibility[Divs.Attachments] = false;
                    DivsVisibility[Divs.NewAttachments] = true;
                    if (LoadMessageReply() == false)
                        return;
                    break;
                case MailActions.SEND:
                    DivsVisibility[Divs.Attachments] = true;
                    DivsVisibility[Divs.NewAttachments] = true;
                    if (LoadMessageSend() == false)
                        return;
                    break;
                case MailActions.FORWARD:
                    DivsVisibility[Divs.Attachments] = true;
                    DivsVisibility[Divs.NewAttachments] = true;
                    if (LoadMessageForward() == false)
                        return;
                    break;
                case MailActions.RE_SEND:
                    DivsVisibility[Divs.Attachments] = true;
                    DivsVisibility[Divs.NewAttachments] = false;
                    if (LoadMessageReSend() == false)
                        return;
                    break;
                default:
                    break;
            }
            pnlMail.DataBind();
        }

        public void Initialize(MailActions action, String sottoTitolo, bool mailEditabile)
        {
            this.MailEditabile = mailEditabile;
            Initialize(action, sottoTitolo);
        }
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            //per le immagini embedded
            if (Request.QueryString["cont"] != null)
            {
                string content = Request.QueryString["cont"];
                Response.Clear();
                Message msg = MailMessageComposer.CurrentSendMailGet();
                if (msg == null) return;
                foreach (MimePart m in msg.EmbeddedObjects)
                {
                    if (m.ContentId.Equals("<" + content + ">"))
                    {
                        Response.ContentType = m.ContentType.ToString();
                        Response.OutputStream.Write(m.BinaryContent, 0, m.BinaryContent.Length);
                        Response.Flush();
                        Response.End();
                        return;
                    }
                }

                foreach (MimePart m in msg.UnknownDispositionMimeParts)
                {
                    if (m.ContentId.Equals("<" + content + ">"))
                    {
                        Response.ContentType = m.ContentType.ToString();
                        Response.OutputStream.Write(m.BinaryContent, 0, m.BinaryContent.Length);
                        Response.Flush();
                        Response.End();
                        return;
                    }
                }

                Response.Flush();
                Response.End();
                return;
            }

            ToTextBox.ReadOnly = false;
            CCTextBox.ReadOnly = false;
            BCCTextBox.ReadOnly = false;
            SubjectTextBox.ReadOnly = false;
            BodyTextBox.ReadOnly = false;
        }
        #endregion

        #region "Private Methods"
        #region "Gestione Caricamento Messaggi"
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
        /// <summary>
        /// Metodo per il caricamento del messaggio di reply
        /// </summary>
        /// <returns></returns>
        private bool LoadMessageReply()
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
            if (mailUser == null)
            {
                onAccountInvalid();
                return false;
            }
            Message msg = GetCurrentMessage();
            if (msg == null)
            {
                onMessageInvalid();
                return false;
            }
            msg.To.Clear();
            msg.To.Add(msg.From);
            msg.From = new Address(mailUser.EmailAddress, mailUser.DisplayName);
            msg.ReplyTo = new Address();
            msg.Bcc.Clear();
            msg.Subject = String.Concat("Re:", msg.Subject);

            ToTextBox.ReadOnly = true;
            SubjectTextBox.ReadOnly = true;
            return true;
        }
        /// <summary>
        /// Metodo per il caricamento del messaggio di reply to all
        /// </summary>
        /// <returns></returns>
        private bool LoadMessageReplyAll()
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
            if (mailUser == null)
            {
                onAccountInvalid();
                return false;
            }
            Message msg = GetCurrentMessage();
            if (msg == null)
            {
                onMessageInvalid();
                return false;
            }
            if (selectEmail(msg.To).Contains(mailUser.EmailAddress))
            {
                msg.To = (AddressCollection)msg.To.Where(x => !x.Email.Equals(mailUser.EmailAddress)).ToList();
            }
            msg.To.Add(msg.From);
            if (selectEmail(msg.Cc).Contains(mailUser.EmailAddress))
            {
                msg.Cc = (AddressCollection)msg.Cc.Where(x => !x.Email.Equals(mailUser.EmailAddress)).ToList();
            }
            msg.Bcc.Clear();
            msg.From = new Address(mailUser.EmailAddress, mailUser.DisplayName);
            msg.ReplyTo = new Address();
            msg.Subject = String.Concat("Re:", msg.Subject);

            ToTextBox.ReadOnly = true;
            SubjectTextBox.ReadOnly = true;
            CCTextBox.ReadOnly = true;
            return true;
        }
        /// <summary>
        /// Metodo per il caricamento del messaggio di send
        /// </summary>
        /// <returns></returns>
        private bool LoadMessageReSend()
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
            if (mailUser == null)
            {
                onAccountInvalid();
                return false;
            }
            Message msg = GetCurrentMessage();
            if (msg == null)
            {
                onMessageInvalid();
                return false;
            }
            msg.From = new Address(mailUser.EmailAddress, mailUser.DisplayName);
            msg.ReplyTo = new Address();
            return true;
        }
        /// <summary>
        /// Metodo per il caricamento del messaggio di forward
        /// </summary>
        /// <returns></returns>
        private bool LoadMessageForward()
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
            if (mailUser == null)
            {
                onAccountInvalid();
                return false;
            }
            Message msg = GetCurrentMessage();
            if (msg == null)
            {
                onMessageInvalid();
                return false;
            }
            msg.From = new Address(mailUser.EmailAddress, mailUser.DisplayName);
            msg.To.Clear();
            msg.Cc.Clear();
            msg.Bcc.Clear();
            msg.Subject = String.Concat("Fw:", msg.Subject);
            return true;
        }
        /// <summary>
        /// Metodo per il caricamento del messaggio di send
        /// </summary>
        /// <returns></returns>
        private bool LoadMessageSend()
        {
            ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
            if (mailUser == null)
            {
                onAccountInvalid();
                return false;
            }
            Message msg = CreateNewMessage();
            if (msg == null)
            {
                onMessageInvalid();
                return false;
            }
            msg.From = new Address(mailUser.EmailAddress, mailUser.DisplayName);
            msg.ReplyTo = new Address();
            return true;
        }

        #region "Metodi comuni"
        /// <summary>
        /// Metodo per la creazione di un nuovo messaggio
        /// (Per la send)
        /// </summary>
        private Message CreateNewMessage()
        {
            //se esiste il messaggio corrente, riprendo lo stesso
            if (MailMessageComposer.CurrentSendMailExist())
                return MailMessageComposer.CurrentSendMailGet();
            //altrimenti costruisco un nuovo messaggio
            MailMessageComposer mmc = new MailMessageComposer();
            mmc.CurrentSendMail_NEW();
            return mmc.message;
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
                        _log.Error(err);
                        (this.Page as BasePage).info.AddMessage(mEx, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
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
                            _log.Error(err);
                            (this.Page as BasePage).info.AddMessage(mEx, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
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
        #endregion
        #endregion

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
                        _log.Error(er);
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
                        _log.Error(er);
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
                        _log.Error(er);
                        throw mEx;
                    }
                //throw new ManagedException("La mail " + bccArray[i] + "non è una mail valida", "ERR_MF_003", null, null, null, null, null, null);
            }
        }
        #endregion
        #endregion

        #region "Page Methods"
        string bodyBag;

        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Message msg;
                ComunicazioniService comunicazioniService = new ComunicazioniService();
                if (MailMessageComposer.CurrentSendMailExist())
                    msg = MailMessageComposer.CurrentSendMailGet();
                else
                    msg = new Message();

                msg.Subject = SubjectTextBox.Text;
                if (String.IsNullOrEmpty(ToTextBox.Text.Trim()) &&
                    String.IsNullOrEmpty(CCTextBox.Text.Trim()) &&
                    String.IsNullOrEmpty(BCCTextBox.Text.Trim()))
                {
                    ErrorLabel.Text = "Inserire almeno un destinatario";
                    ErrorLabel.Visible = true;
                    return;
                }
                msg.To.Clear();
                msg.Cc.Clear();
                msg.Bcc.Clear();
                this.addEmailsTo(ToTextBox.Text, msg);
                this.addEmailsCc(CCTextBox.Text, msg);
                this.addEmailCcn(BCCTextBox.Text, msg);
                msg.Date = System.DateTime.Now;
                //mantengo il vecchio testo perché in caso di ErrorEventArgs lo devo ripristinare
                bodyBag = msg.BodyHtml.Text;
                SendMail.Model.BodyChunk bb = new SendMail.Model.BodyChunk();
                string txt = BodyTextBox.Text;
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
                if (!String.IsNullOrEmpty(PreformattedBody.Text))
                {
                    d.LoadHtml(PreformattedBody.Text);
                }
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
                if (MailEditabile == true && cbIncludiAllegati.Checked == false)
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
                switch (CurrentAction)
                {
                    case MailActions.REPLY_TO:
                    case MailActions.REPLY_ALL:
                    case MailActions.RE_SEND:
                    case MailActions.FORWARD:
                        msg.InReplyTo = msg.Id.ToString();
                        break;
                }

                ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = null;
                if (WebMailClientManager.AccountExist())
                {
                    mailUser = WebMailClientManager.getAccount();
                }

                if (mailUser != null)
                {
                    MailServerFacade f = MailServerFacade.GetInstance(mailUser);

                    if (mailUser.IsManaged)
                    {
                        try
                        {
                            SendMail.Model.ComunicazioniMapping.Comunicazioni c =
                                new SendMail.Model.ComunicazioniMapping.Comunicazioni(
                                     SendMail.Model.TipoCanale.MAIL,
                                     this.SottoTitolo,
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

                            comunicazioniService.InsertComunicazione(c);
                        }
                        catch (Exception ex)
                        {
                            ManagedException mex = new ManagedException("Errore nel salvataggio della mail",
                                "MAIL_CMP_002", "", ex.StackTrace, ex);
                            ErrorLog err = new ErrorLog(mex);
                            _log.Error(err);
                            ErrorLabel.Text = "Errore nell'invio del messaggio";
                            ErrorLabel.Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        f.sendMail(msg);
                    }

                    (this.Page as BasePage).info.AddInfo("Il messaggio e' stato spedito correttamente");
                    MailMessageComposer.CurrentSendMailClear();
                    onMailSent();
                }
                else
                {
                    ((BasePage)this.Page).info.AddInfo("Account inesistente.");
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                    _log.Error(new Com.Delta.Logging.Errors.ErrorLog(new ManagedException(ex.Message, "FAC_007", string.Empty, string.Empty, ex)));
                //_log.Error(new Com.Delta.Logging.Errors.ErrorLog("FAC_007", ex, string.Empty, string.Empty, string.Empty));

                MailMessageComposer.CurrentSendMailGet().BodyHtml.Text = bodyBag;
                ErrorLabel.Visible = true;
                ErrorLabel.Text = ex.Message;
                return;
            }
            Label5.Visible = true;
        }

        protected void ibAddAttach_Click(object sender, ImageClickEventArgs e)
        {
            //if (FileUpload1.HasFile == false) return;
            //Message msg = GetCurrentMessage();
            //if (msg == null) return;
            //if (msg.Attachments == null)
            //{
            //    throw new InvalidDataException("La proprietà Attachments non può essere null");
            //}
            //if (!msg.Attachments.Contains(FileUpload1.FileName))
            //{
            //    msg.Attachments.Add(FileUpload1.FileBytes, FileUpload1.FileName);
            //}
            //rpAttach.DataSource = msg.Attachments;
            //rpAttach.DataBind();
        }

        protected void asyncFileUpload_UploadedFileError(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {

        }

        protected void asyncFileUpload_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            if (e.State == AjaxControlToolkit.AsyncFileUploadState.Success)
            {
                int fSize = 0;
                if (String.IsNullOrEmpty(e.FileSize) || !int.TryParse(e.FileSize, out fSize) || fSize == 0)
                {
                    return;
                }
                else
                {
                    string confSize = ConfigurationManager.AppSettings.Get("MaxAttachmentSizeINMB");
                    int maxSize = 1;
                    if (!String.IsNullOrEmpty(confSize))
                        int.TryParse(confSize, out maxSize);
                    if (fSize > (maxSize * Math.Pow(1024, 2)))
                    {
                        return;
                    }
                }

                Message msg = CurrentMessage;
                if (msg == null) return;
                if (msg.Attachments == null)
                {
                    throw new InvalidDataException("La proprietà Attachments non può essere null");
                }
                string fileName = Path.GetFileName(e.FileName);
                if (!msg.Attachments.Contains(fileName))
                {
                    HttpPostedFile postedFile = null;
                    if (asyncFileUpload.HasFile)
                        postedFile = asyncFileUpload.PostedFile;
                    if (postedFile == null)
                    {
                        (this.Page as BasePage).info.AddError("Attenzione. Il file non è stato allegato");
                        return;
                    }
                    byte[] fByte = new byte[fSize];
                    if (postedFile.InputStream.CanSeek)
                        postedFile.InputStream.Seek(0, SeekOrigin.Begin);
                    postedFile.InputStream.Read(fByte, 0, fSize);
                    msg.Attachments.Add(fByte, fileName);
                }
            }
        }

        protected void btnPostFileUpload_Click(object sender, EventArgs e)
        {
            this.rptNewAttach.DataBind();
        }

        protected void rpAttach_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "RemoveAttach":
                    Message msg = CurrentMessage;
                    if (msg.Attachments != null)
                    {
                        if (msg.Attachments.Contains((string)e.CommandArgument))
                        {
                            for (int i = 0; i < msg.Attachments.Count; i++)
                            {
                                if (msg.Attachments[i].Filename == (string)e.CommandArgument)
                                {
                                    msg.Attachments.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                    if (msg.Attachments.Count == 0)
                        cbIncludiAllegati.Checked = false;
                    (source as Repeater).DataBind();
                    break;
            }
        }

        protected void cbIncludiAllegati_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Visible == false) return;
            if (cb.Checked)
            {
                DivsVisibility[Divs.Attachments] = true;
                Message oriMex = GetOriginalMessage();
                if (oriMex != null)
                {
                    foreach (MimePart att in oriMex.Attachments)
                    {
                        if (!CurrentMessage.Attachments.Contains(att.Filename))
                            CurrentMessage.Attachments.Add(att.BinaryContent, att.Filename);
                    }
                }
            }
            else
                DivsVisibility[Divs.Attachments] = false;
            rpAttach.DataBind();
        }

        protected string ParseOriginalMessage()
        {
            Message msg = null;
            switch (CurrentAction)
            {
                case MailActions.SEND:
                case MailActions.RE_SEND:
                    msg = GetCurrentMessage();
                    break;
                default:
                    msg = GetOriginalMessage();
                    break;
            }

            if (msg == null) return null;

            StringWriter sw = new StringWriter();
            using (HtmlTextWriter w = new HtmlTextWriter(sw))
            {
                w.AddStyleAttribute(HtmlTextWriterStyle.Overflow, "auto");
                w.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "10px");
                w.AddStyleAttribute(HtmlTextWriterStyle.TextAlign, "left");
                w.RenderBeginTag(HtmlTextWriterTag.Div);
                w.WriteLine();
                w.Indent++;
                switch (CurrentAction)
                {
                    case MailActions.FORWARD:
                    case MailActions.REPLY_ALL:
                    case MailActions.REPLY_TO:
                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.WriteLine();
                        w.Indent++;
                        w.WriteLine("--- Messaggio Originale ---");
                        w.Indent--;
                        w.RenderEndTag();
                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.WriteLine();
                        w.Indent++;
                        w.WriteLine("Da: " + msg.From.Name);
                        w.Indent--;
                        w.RenderEndTag();
                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.WriteLine();
                        w.Indent++;
                        w.Write("A: ");
                        foreach (Address to in msg.To)
                        {
                            w.Write(to.Email + ";");
                        }
                        w.WriteLine();
                        w.Indent--;
                        w.RenderEndTag();
                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.WriteLine();
                        w.Indent++;
                        if (msg.Date != System.DateTime.MinValue)
                            w.WriteLine("Inviata il: " + msg.Date.ToString("dd/MM/yyyy HH:mm:ss"));
                        else
                            w.WriteLine("Inviata il: " + msg.DateString);
                        w.Indent--;
                        w.RenderEndTag();
                        w.RenderBeginTag(HtmlTextWriterTag.P);
                        w.WriteLine();
                        w.Indent++;
                        if (!String.IsNullOrEmpty(msg.Subject))
                            w.WriteLine("Oggetto: " + msg.Subject.Replace("<", "&lt;").Replace(">", "&gt;"));
                        w.Indent--;
                        w.RenderEndTag();
                        w.AddStyleAttribute(HtmlTextWriterStyle.MarginTop, "20px");
                        break;
                }
                w.RenderBeginTag(HtmlTextWriterTag.Div);
                w.WriteLine();
                w.Indent++;
                string body;
                if (msg.BodyHtml != null && !String.IsNullOrEmpty(msg.BodyHtml.Text))
                {
                    body = msg.BodyHtml.Text.Replace("cid:", Request.Url + "?cont=");
                    HtmlDocument d = new HtmlDocument();
                    d.LoadHtml(body);
                    HtmlNode root = d.DocumentNode.Descendants().SingleOrDefault(x => x.Name.Equals("body", StringComparison.InvariantCultureIgnoreCase));
                    if (root == null)
                        root = d.DocumentNode.Descendants().FirstOrDefault(x => x.NodeType == HtmlNodeType.Element);
                    if (root != null)
                        body = root.InnerHtml;
                }
                else
                    body = msg.BodyText.Text.Replace("cid:", Request.Url + "?cont=");
                w.WriteLine(body);
                w.Indent--;
                w.RenderEndTag();
                w.Indent--;
                w.RenderEndTag();
            }
            return sw.ToString();
        }

        protected bool IncludiAllegatiVisible()
        {
            Message msg = GetOriginalMessage();
            if (msg != null && msg.Attachments.Count != 0 && MailEditabile == true)
                return true;
            else
                return false;
        }
        #endregion
    }
}
