using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.Facades;
using Com.Delta.Mail.MailMessage;
using Com.Delta.PrintManager;
using Com.Delta.Web.Cache;
using GestionePEC.Extensions;
using HtmlAgilityPack;
using iTextSharp.text;
using log4net;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model.ComunicazioniMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace GestionePEC.Controls
{
    public sealed class MailActionEventArgs : EventArgs
    {
        public MailActions Action { get; set; }
    }

    public partial class MailViewer : System.Web.UI.UserControl
    {

        private static readonly ILog _log = LogManager.GetLogger(typeof(MailViewer));

        public delegate void ActionHandler(object sender, MailActionEventArgs action, string folder);

        public event ActionHandler RequireAction;
        private void onActionRequired(object sender, MailActionEventArgs e, string parentFolder)
        {
            if (RequireAction != null)
            {
                RequireAction(sender, e, parentFolder);
            }
        }

        public event EventHandler MailSelected;
        private void onMailSelected()
        {
            if (MailSelected != null)
                MailSelected(this, EventArgs.Empty);
        }



        protected void butMailViewer_Click(object sender, EventArgs e)
        {
            onMailSelected();
        }

        public event EventHandler AccountInvalid;
        private void onAccountInvalid()
        {
            if (AccountInvalid != null)
                AccountInvalid(this, EventArgs.Empty);
            this.Visible = false;
        }

        #region "Public properties"

        public Boolean EnableReplyAll { get; set; }
        public Boolean EnableReplyTo { get; set; }
        public Boolean EnableForward { get; set; }
        public Boolean EnableRating { get; set; }
        public Boolean EnableAcquire { get; set; }
        public Boolean EnableMailTree { get; set; }

        protected Dictionary<HeaderButtons, bool> ButtonVisible
        {
            get { return ViewState[BUTTONS_HEADER] as Dictionary<HeaderButtons, bool>; }
        }

        public string hfIdMailClientID
        {
            get { return hfIdMail.ClientID; }
        }
        public string hfIdMailValue
        {
            get { return hfIdMail.Value; }
        }
        public string hfUIDMailClientID
        {
            get { return hfUIDMail.ClientID; }
        }
        public string hfAccountClienID
        {
            get { return hfAccount.ClientID; }
        }
        public string hfCurrFolderClientID
        {
            get { return hfCurrFolder.ClientID; }
        }
        public string hfUIDMailValue
        {
            get { return hfUIDMail.Value; }
            set { hfUIDMail.Value = value; }
        }
        public string hfParFolderValue
        {
            get { return hfParFolder.Value; }
        }

        public string hfCurrFolderValue
        {
            get { return hfCurrFolder.Value; }
        }

        public string ibShowMailTreeClientID
        {
            get { return ibShowMailTree.ClientID; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Message CurrentMessage { get; set; }

        #endregion
        private const string BUTTONS_HEADER = "button_headers";
        protected enum HeaderButtons
        {
            ReplyAll,
            ReplyTo,
            Forward,
            Rating,
            MailTree,
            Acquire,
            ReSend
        }

        public void Initialize(string uid, string currRating, string currFolder, string parentFolder)
        {
            Message mmsg = null;
            if (WebMailClientManager.AccountIsValid() == false ||
                (!String.IsNullOrEmpty(hfAccount.Value) &&
                WebMailClientManager.getAccount().EmailAddress.Equals(hfAccount.Value) == false))
            {
                onAccountInvalid();
                return;
            }
            if (!String.IsNullOrEmpty(uid))
            {
                ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
                this.hfAccount.Value = mailUser.EmailAddress;

                if (String.IsNullOrEmpty(currFolder)) currFolder = "1I";
                string fold = currFolder;

                this.hfCurrFolder.Value = fold.ToString();
                string folder = fold;
                //WebMailClientManager.CurrentFolderSet(folder);

                this.rating.InnerHtml = currRating;
                if (!String.IsNullOrEmpty(currRating))
                    this.rating.Style[HtmlTextWriterStyle.Width] = (int.Parse(currRating) * 20) + "px";
                SetHeaderButtonsVisibility(folder, parentFolder);
                MailServerFacade f = MailServerFacade.GetInstance(mailUser);              
                switch (parentFolder)
                {
                    case "I":
                        f.MailBoxName = "inbox";
                        break;
                    case "O":
                    case "AO":
                        f.MailBoxName = "outbox";
                        break;
                }
                mmsg = f.getMessage(uid, false);
                hfUIDMail.Value = uid;
                WebMailClientManager.CurrentMailSet(mmsg);
                this.Visible = (mmsg != null);
                if (mmsg == null)
                {
                    ((BasePage)this.Page).info.AddMessage("Messaggio non più disponibile (" + uid + ").\r\n\r\nFare 'refresh' prima di continuare. ", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                this.Visible = true;
            }
            else
                this.Visible = false;
        }

        public void Initialize()
        {
            System.Tuples.Tuple<Message, string, int, string> tuple;
            MailLocalService mailLocalService = new MailLocalService();
            string idMail = hfIdMail.Value;
            if (WebMailClientManager.AccountIsValid() == false)
            {
                onAccountInvalid();
                return;
            }
            if (!String.IsNullOrEmpty(idMail))
            {
                ActiveUp.Net.Mail.DeltaExt.MailUser mailUser = WebMailClientManager.getAccount();
                this.hfAccount.Value = mailUser.EmailAddress;

                long id = -1;
                if (!long.TryParse(idMail, out id))
                {
                    (this.Page as BasePage).info.AddInfo("Impossibile caricare il messaggio");
                    return;
                }

                tuple = mailLocalService.GetMessageById(id.ToString());
                this.Visible = (tuple != null && tuple.Element1 != null);
                if (tuple == null || tuple.Element1 == null)
                {
                    ((BasePage)this.Page).info.AddMessage("Messaggio non piu' disponibile (" + id + ").\r\n\r\nFare 'refresh' prima di continuare. ", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }

                WebMailClientManager.CurrentMailSet(tuple.Element1);
                hfCurrFolder.Value = tuple.Element2.ToString();
                WebMailClientManager.CurrentFolderSet(tuple.Element2);
                WebMailClientManager.ParentFolderSet(tuple.Element4);
                hfParFolder.Value = tuple.Element4.ToString();
                SetHeaderButtonsVisibility(tuple.Element2, tuple.Element4);
                this.rating.InnerHtml = tuple.Element3.ToString();
                this.rating.Style[HtmlTextWriterStyle.Width] = (tuple.Element3 * 20) + "px";
            }
            else
                this.Visible = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            butMailViewer.Style.Add(HtmlTextWriterStyle.Display, "none");
            if (this.IsPostBack == false)
            {
                Dictionary<HeaderButtons, bool> buttonVisible = new Dictionary<HeaderButtons, bool>();
                buttonVisible.Add(HeaderButtons.Acquire, false);
                buttonVisible.Add(HeaderButtons.Forward, false);
                buttonVisible.Add(HeaderButtons.MailTree, false);
                buttonVisible.Add(HeaderButtons.Rating, false);
                buttonVisible.Add(HeaderButtons.ReplyAll, false);
                buttonVisible.Add(HeaderButtons.ReplyTo, false);
                buttonVisible.Add(HeaderButtons.ReSend, false);
                this.ViewState.Add(BUTTONS_HEADER, buttonVisible);
                if (Request.QueryString["cont"] == null)
                {
                    if (WebMailClientManager.CurrentFolderExists())
                        WebMailClientManager.CurrentFolderRemove();
                    if (WebMailClientManager.CurrentMailExist())
                        WebMailClientManager.CurrentMailRemove();
                }
            }

            if (Request.QueryString["cont"] != null)
            {
                string content = Request.QueryString["cont"];
                Response.Clear();

                foreach (MimePart m in WebMailClientManager.CurrentMailGet().EmbeddedObjects)
                {
                    if (m.ContentId.Equals("<" + content + ">"))
                    {
                        Response.ContentType = m.ContentType.ToString();
                        if (Response.IsClientConnected)
                            Response.OutputStream.Write(m.BinaryContent, 0, m.BinaryContent.Length);
                        //  Context.Response.Flush();
                        Context.Response.End();
                        return;
                    }
                }

                foreach (MimePart m in WebMailClientManager.CurrentMailGet().UnknownDispositionMimeParts)
                {
                    if (m.ContentId.Equals("<" + content + ">"))
                    {
                        Response.ContentType = m.ContentType.ToString();
                        if (Response.IsClientConnected)
                            Response.OutputStream.Write(m.BinaryContent, 0, m.BinaryContent.Length);
                        //   Context.Response.Flush();
                        Context.Response.End();
                        return;
                    }
                }

                return;
            }
            //se non ho tutti i valori necessari a richiedere un mail message settati il controllo non è visibile
            if (WebMailClientManager.CurrentMailExist())
            {
                this.Visible = true;
                string currFolder = WebMailClientManager.CurrentFolderGet();
                string parFolder = WebMailClientManager.ParentFolderGet();
                SetHeaderButtonsVisibility(currFolder, parFolder);
            }
            else
                this.Visible = false;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (WebMailClientManager.CurrentMailExist())
            {
                Message msg = WebMailClientManager.CurrentMailGet();

                if ((WebMailClientManager.CurrentFolderGet() == "2") || (WebMailClientManager.ParentFolderGet() == "O") || WebMailClientManager.ParentFolderGet() == "AO" || (WebMailClientManager.CurrentFolderGet() == "6") || (msg != null && msg.Size < long.Parse(ConfigurationManager.AppSettings["MaxMemoryDimensionForMailViewer"])))
                {
                    LoadMessage(msg);
                    hfIdMail.Value = msg.Id.ToString();

                    ReplyButton.Visible = ForwardButton.Visible = ibReSend.Visible = lbStampaPdf.Visible = true;
                    if (msg.Container != null)
                        ButtonVisible[HeaderButtons.Rating] = false;

                    string currFolder = WebMailClientManager.CurrentFolderGet();
                    string parentFolder = WebMailClientManager.ParentFolderGet();
                    if (String.IsNullOrEmpty(hfCurrFolder.Value))
                        hfCurrFolder.Value = currFolder.ToString();
                    if (String.IsNullOrEmpty(hfParFolder.Value))
                        hfParFolder.Value = parentFolder.ToString();
                    if (hfCurrFolder.Value == "2" || hfCurrFolder.Value == "3" || hfCurrFolder.Value == "9" || hfCurrFolder.Value == "7" || hfParFolder.Value == "O" || hfParFolder.Value == "AO" || hfCurrFolder.Value == "6")
                    { ibShowMailTree.Visible = true; }
                    else { ibShowMailTree.Visible = false; }
                }
                else
                {
                    try
                    {
                        int id = msg.Id;
                        string uid = msg.Uid;
                        ActiveUp.Net.Mail.Header header = Parser.ParseHeader(msg.OriginalData);

                        System.Reflection.PropertyInfo[] t = typeof(ActiveUp.Net.Mail.Header).GetProperties(); //tutte le proprietà header

                        foreach (System.Reflection.PropertyInfo pi in
                            typeof(ActiveUp.Net.Mail.Header).GetProperties())
                        {
                            try
                            {
                                if (pi.CanRead)
                                {
                                    System.Reflection.PropertyInfo pm = (from p in typeof(Message).GetProperties()
                                                                         where p.Name == pi.Name
                                                                         select p).SingleOrDefault();
                                    if (pm.Name != "ConfirmRead" && pm.Name != "Xref")
                                    {
                                        if (pm != null && pm.CanWrite)
                                        {
                                            pm.SetValue(msg, pi.GetValue(header, null), null);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {                               
                                if (ex.GetType() != typeof(ManagedException))
                                {
                                    ManagedException mEx = new ManagedException("Errore nella lettura delle proprietà dei messaggi. Dettaglio: "
                                        + ex.Message,
                                        "ERR_MV001",
                                        string.Empty,
                                        string.Empty,
                                        ex.InnerException);
                                    mEx.addEnanchedInfosTag("DETAILS", new XElement("info",
                                        new XElement("MAIL_INBOX.ID_MAIL", id.ToString()),
                                        new XElement("MAIL_INBOX.UID", uid.ToString()),
                                        new XElement("user_msg", "Errore nella lettura delle proprietà dei messaggi. Dettaglio: " + ex.Message),
                                        new XElement("exception",
                                            new XElement("message", ex.Message),
                                            new XElement("source", ex.Source),
                                            new XElement("stack", ex.StackTrace),
                                            new XElement("innerException", ex.InnerException))).ToString(SaveOptions.DisableFormatting));
                                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                                    er.objectID = (id != null) ? id.ToString() : "-";
                                    _log.Error(er);
                                    throw mEx;
                                }
                                else throw ex;
                            }
                        }                       
                        msg.Id = id;
                        hfIdMail.Value = id.ToString();
                        hfUIDMail.Value = msg.Uid = uid;                      
                        lbDownload.Visible = true;
                        this.CurrentMessage = msg;                       
                        ReplyButton.Visible = ForwardButton.Visible = lnbSMBack.Visible = ibReSend.Visible = ibShowMailTree.Visible = lbStampaPdf.Visible = false;
                        MailContent.Visible = PnlAttachment.Visible = PnlInnerMail.Visible = PnlEmbeddedElements.Visible = false;
                        return;
                    }
                    catch (Exception ex)
                    {                       
                        if (ex.GetType() != typeof(ManagedException))
                        {
                            ManagedException mEx = new ManagedException("Errore lettura degli header >> " + ex.Message,
                                "ERR987",
                                string.Empty,
                                string.Empty,
                                ex.InnerException);
                            mEx.addEnanchedInfosTag("DETAILS", new XElement("info",
                                new XElement("user_msg", "Errore lettura degli header. Dettaglio: " + ex.Message),
                                new XElement("exception",
                                    new XElement("message", ex.Message),
                                    new XElement("source", ex.Source),
                                    new XElement("stack", ex.StackTrace),
                                    new XElement("innerException", ex.InnerException))).ToString(SaveOptions.DisableFormatting));
                            ErrorLogInfo er = new ErrorLogInfo(mEx);
                            er.objectID = (msg != null) ? msg.Id.ToString() : " - ";
                            _log.Error(er);
                        }
                        (this.Page as BasePage).info.AddMessage("Impossibile elaborare la email riprovare in un secondo momento", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                        return;
                    }

                }               
            }
            this.DataBind();
        }

        #region Methods

        protected void lbDownload_Click(object sender, EventArgs e)
        {
            //  Message message = WebMailClientManager.CurrentMailGet();
            MailServerFacade facade = MailServerFacade.GetInstance(WebMailClientManager.getAccount());           
            Message message = facade.getMessage(hfUIDMail.Value, false);
            if (message == null)
            {
                (this.Page as BasePage).info.AddError("Errore nella lettura del messaggio. Ricaricare la pagina");
                return;
            }
            message.Uid = hfUIDMail.Value;
            Response.Clear();
            Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            Response.AddHeader("content-disposition", "attachment; filename=MAIL_" + message.Uid + ".eml");
            if (Response.IsClientConnected)
                Response.BinaryWrite(message.OriginalData);
            Context.Response.Flush();
            Context.Response.Clear();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();          
            Context.Response.End();


        }

        protected void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MimePart attach = e.Item.DataItem as MimePart;
            ComunicazioniService comService = new ComunicazioniService();
            if (attach.BinaryContent == null || attach.BinaryContent.Length == 0 && !String.IsNullOrEmpty(attach.Filename))
            {
                if (attach.ContentId != null)
                {
                    string idAttach = attach.ContentId.Trim(new char[] { '<', '>' });
                    long idAtt = -1;
                    if (long.TryParse(idAttach, out idAtt))
                    {
                        ComAllegato all = comService
                                    .LoadAllegatoComunicazioneById(long.Parse(idAttach));
                        attach.BinaryContent = all.AllegatoFile;
                    }
                }
            }
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string fileName = (string)e.CommandArgument;
            ComunicazioniService comService = new ComunicazioniService();
            Response.Clear();
            Message msg = WebMailClientManager.CurrentMailGet();
            for (int i = 0; i < msg.Attachments.Count; i++)
            {
                if (WebMailClientManager.CurrentMailGet().Attachments[i].Filename == fileName)
                {
                    //if (hfCurrFolder.Value.Equals(((int)MailFolder.Ricevute).ToString())
                    //    || hfCurrFolder.Value.Equals(((int)MailFolder.RicevutePEC).ToString())
                    //    || hfCurrFolder.Value.Equals(((int)MailFolder.Cestino).ToString()))
                    if ((hfCurrFolder.Value.Equals("1") || hfCurrFolder.Value.Equals("3")) ||
                        (hfParFolder.Value.Equals("1")))
                    {
                        if (Response.IsClientConnected)
                            if (msg.Attachments[i].BinaryContent.Length > 0)
                            {
                                Response.BinaryWrite(msg.Attachments[i].BinaryContent);
                            }
                    }
                    else if (msg.Attachments[i].BinaryContent != null &&
                        msg.Attachments[i].BinaryContent.Length > 0)
                    {
                        if (Response.IsClientConnected)
                            Response.BinaryWrite(msg.Attachments[i].BinaryContent);
                    }
                    else
                    {
                        string idAttach = msg.Attachments[i].ContentId.Trim(new char[] { '<', '>' });
                        ComAllegato all = comService
                            .LoadAllegatoComunicazioneById(long.Parse(idAttach));

                        if (all.AllegatoExt.Equals("TPU", StringComparison.InvariantCultureIgnoreCase))
                        {
                            byte[] pdf = comService
                                .GetPdfTpuStampeBUS(all.AllegatoTpu, all.AllegatoFile, "");
                            if (Response.IsClientConnected)
                                Response.BinaryWrite(pdf);
                        }
                        else
                        {
                            if (Response.IsClientConnected)
                                Response.BinaryWrite(all.AllegatoFile);
                        }
                    }
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                }
            }
            // Context.Response.Flush();
            Context.Response.End();
            //Context.ApplicationInstance.CompleteRequest();
        }

        protected void Repeater2_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string fileName = (string)e.CommandArgument;
            Response.Clear();
            Message msg = WebMailClientManager.CurrentMailGet();
            IEnumerable<MimePart> obj = (from emb in msg.EmbeddedObjects.Cast<MimePart>()
                                         where emb.Filename == fileName
                                         select emb).Union(
                      from unk in msg.UnknownDispositionMimeParts.Cast<MimePart>()
                      where unk.Filename == fileName
                      select unk);

            Response.ContentType = "application/octet-stream";
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            System.Threading.Thread.Sleep(10000);

            if (Response.IsClientConnected && obj.Count<MimePart>() > 0 && obj.First().BinaryContent != null && obj.First().BinaryContent.Length > 0)

                Response.BinaryWrite(obj.First().BinaryContent);

            //  Context.Response.Flush();
            Context.Response.End();
        }

        protected void rptSubMessage_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            Message msg = (Message)WebMailClientManager.CurrentMailGet().SubMessages[e.Item.ItemIndex];
            msg.Id = WebMailClientManager.CurrentMailGet().Id;
            WebMailClientManager.CurrentMailSet(msg);
            ButtonVisible[HeaderButtons.Rating] = false;
        }

        protected void SubMessage_Back_Click(object sender, EventArgs e)
        {
            if (!WebMailClientManager.CurrentMailIsRoot())
                WebMailClientManager.CurrentMailSet((Message)WebMailClientManager.CurrentMailGet().Container);
            ButtonVisible[HeaderButtons.Rating] = true;
        }

        protected void Action_WorkOn(object sender, CommandEventArgs e)
        {
            Message currMsg = WebMailClientManager.CurrentMailGet();
            if (currMsg != null)
            {
                if (((WebMailClientManager.getAccount() != null) && (WebMailClientManager.getAccount().EmailAddress != hfAccount.Value)) ||
                 (currMsg.Id.ToString() != hfIdMail.Value))
                {
                    (this.Page as BasePage).info.AddMessage("Risultano aperte altre finestre. Ripetere la selezione", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    this.Visible = false;
                    return;
                }
            }

            MailActions act;
            switch (e.CommandName)
            {
                case "Acquire":
                    act = MailActions.ACQUIRE;
                    break;
                case "Forward":
                    act = MailActions.FORWARD;
                    break;
                case "Reply_All":
                    act = MailActions.REPLY_ALL;
                    break;
                case "Reply_To":
                    act = MailActions.REPLY_TO;
                    break;
                case "Re_Send":
                    act = MailActions.RE_SEND;
                    break;
                default:
                    throw new FormatException("Caso non implementato");
            }

            onActionRequired(sender, new MailActionEventArgs() { Action = act }, hfParFolder.Value);
        }

        protected void rptSubMessage_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void lbStampaPdf_Click(object sender, EventArgs e)
        {
            Message currMsg = WebMailClientManager.CurrentMailGet();
            string fromMessage = currMsg.From.Email;
            string toMessage = string.Empty;
            string CCMessage = string.Empty;
            foreach (Address t in currMsg.To)
            {
                toMessage += " " + t.Email;
            }
            foreach (Address c in currMsg.Cc)
            {
                CCMessage += " " + c.Email;
            }
            string subjectMessage = currMsg.Subject;
            string data = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Font white = new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD,iTextSharp.text.BaseColor.BLACK);
            Font white10 = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            Chunk c2 = new Chunk(Environment.NewLine, white);
            Chunk c1 = new Chunk("Mittente: " + fromMessage, white);
            Chunk c3 = new Chunk("Destinatario: " + toMessage, white);
            Chunk c4 = new Chunk("CC: " + CCMessage, white);
            Chunk c5 = new Chunk("Oggetto: " + subjectMessage, white);
            Chunk c6 = new Chunk("Data stampa: " + data, white10);
            Paragraph p1 = new Paragraph();
            p1.Add(c2);
            p1.Add(c6);
            p1.Add(c2);
            p1.Add(c2);
            p1.Add(c1);
            p1.Add(c2);
            p1.Add(c3);
            p1.Add(c2);
            p1.Add(c4);
            p1.Add(c2);
            p1.Add(c5);
            p1.Add(c2);
            System.Drawing.Image img = System.Drawing.Image.FromFile(ConfigurationManager.AppSettings["LOGO"]);
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(img, iTextSharp.text.BaseColor.WHITE);
            image.ScaleToFit(300f, 47f);
            string body = MailContent.InnerHtml;
            if (String.IsNullOrEmpty(body))
                (this.Page as BasePage).info.AddInfo("Non c'è niente da stampare");
            iTextSharp.text.Document d = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            MemoryStream ms = new MemoryStream();
            iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(d, ms);
           // writer.I = 12.5f;
            d.Open();
            d.Add(image);
            d.Add(p1);
            d.Add(c2);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(body);
            if (htmlDoc.ParseErrors.Count() != 0)
            {
                Message m = new Message();
                m.BodyHtml.Text = body;
                body = m.BodyHtml.TextStripped;
                iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.NORMAL);
                font.Color = iTextSharp.text.BaseColor.BLACK;
                iTextSharp.text.Chunk c = new iTextSharp.text.Chunk(body, font);
                iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
                p.Add(c);
                d.Add(p);
                writer.CloseStream = false;
            }
            else
            {
                if (htmlDoc.DocumentNode.Descendants().FirstOrDefault(x => x.NodeType == HtmlNodeType.Element) == null)
                {
                    HtmlNode n = htmlDoc.CreateElement("div");
                    n.InnerHtml = htmlDoc.DocumentNode.InnerHtml;
                    body = n.OuterHtml;
                }

                var nds = htmlDoc.DocumentNode.Descendants().Where(x => (x.Attributes["style"] != null && x.Attributes["style"].Value.Contains("font-size")));
                foreach (var nd in nds)
                {
                    string v = nd.Attributes["style"].Value;
                    int idx = v.IndexOf("font-size:") + 10;
                    int idxLast = v.IndexOf(";", idx);
                    if (idxLast == -1) idxLast = v.Length;
                    string val = v.Substring(idx, idxLast - idx).Trim();
                    switch (val.ToLower())
                    {
                        case "large":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1.2em");
                            break;
                        case "larger":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1.5em");
                            break;
                        case "medium":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1em");
                            break;
                        case "small":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.8em");
                            break;
                        case "smaller":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.7em");
                            break;
                        case "x-large":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1.8em");
                            break;
                        case "x-small":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.5em");
                            break;
                        case "xx-large":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "2em");
                            break;
                        case "xx-small":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.4em");
                            break;
                    }
                    nd.Attributes["style"].Value = v;
                }
                body = htmlDoc.DocumentNode.OuterHtml;

                try
                {

                    //  iTextSharp.tool.xml.XMLWorkerHelper wh = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();
                    //  wh.ParseXHtml(writer, d, new StringReader(body));    
                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(body);
                    HtmlAgilityPack.HtmlNode n = html.DocumentNode;
                    if (n == null)
                    {
                        n = html.DocumentNode.SelectSingleNode("//div");
                    }
                    iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.NORMAL);
                    font.Color = iTextSharp.text.BaseColor.BLACK;
                    iTextSharp.text.Chunk c = new iTextSharp.text.Chunk(n.InnerText.Replace("&nbsp;", " "), font);
                    iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
                    p.Add(c);
                    d.Add(p);
                    //iTextSharp.text.html.simpleparser.HTMLWorker w = new iTextSharp.text.html.simpleparser.HTMLWorker(d);
                    //w.StartDocument();
                    //w.Parse(new StringReader(body));
                    //w.EndDocument();
                    //w.Close();
                }
                catch
                {
                    HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
                    html.LoadHtml(body);
                    HtmlAgilityPack.HtmlNode n = html.DocumentNode.SelectSingleNode("//body");
                    if (n == null)
                    {
                        n = html.DocumentNode.SelectSingleNode("//div");
                    }
                    iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 11f, iTextSharp.text.Font.NORMAL);
                    font.Color = iTextSharp.text.BaseColor.BLACK;
                    iTextSharp.text.Chunk c = new iTextSharp.text.Chunk(n.InnerText.Replace("&nbsp;", " "), font);
                    iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
                    p.Add(c);
                    d.Add(p);
                }
                writer.CloseStream = false;
            }
            bool pdfCreato = false;
            try
            {
                d.Close();
                ms.Seek(0, SeekOrigin.Begin);
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=Mail.pdf");
                Response.ContentType = "application/pdf";
                Response.OutputStream.Write(ms.ToArray(), 0, (int)ms.Length);
                ms.Close();
                pdfCreato = true;
            }
            catch (Exception ex)
            {
                pdfCreato = false;
                ms.Close();
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Impossibile produrre il file pdf. " + ex.Message,
                        "ERR_G048",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    _log.Error(er);
                }
                (this.Page as BasePage).info.AddMessage("Impossibile produrre il file pdf", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
            if (pdfCreato)
                Response.End();
        }

        protected void lbStampaPDU_Click(object sender, EventArgs e)
        {
            string body = MailContent.InnerHtml;
            string strHostName = System.Net.Dns.GetHostName();
            string url = "http://" + strHostName + ResolveUrl("~/config/stampe/tpu/StampaMail.tpu");
            PRUBuilder p = new PRUBuilder(new Hashtable(), url);
            if (String.IsNullOrEmpty(body))
                (this.Page as BasePage).info.AddInfo("Non c'è niente da stampare");
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(body);
            if (htmlDoc.ParseErrors.Count() != 0)
            {

                Message m = new Message();
                m.BodyHtml.Text = body;
                body = m.BodyHtml.TextStripped;
            }
            else
            {
                if (htmlDoc.DocumentNode.Descendants().FirstOrDefault(x => x.NodeType == HtmlNodeType.Element) == null)
                {
                    HtmlNode n = htmlDoc.CreateElement("div");
                    n.InnerHtml = htmlDoc.DocumentNode.InnerHtml;
                    body = n.OuterHtml;
                }

                var nds = htmlDoc.DocumentNode.Descendants().Where(x => (x.Attributes["style"] != null && x.Attributes["style"].Value.Contains("font-size")));
                foreach (var nd in nds)
                {
                    string v = nd.Attributes["style"].Value;
                    int idx = v.IndexOf("font-size:") + 10;
                    int idxLast = v.IndexOf(";", idx);
                    string val = string.Empty;
                    if (idxLast > -1)
                    {
                        val = v.Substring(idx, idxLast - idx).Trim();
                    }
                    else { val = v.Substring(idx).Trim(); }
                    switch (val.ToLower())
                    {
                        case "large":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1.2em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "larger":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1.5em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "medium":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "small":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.8em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "smaller":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.7em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "x-large":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "1.8em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "x-small":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.5em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "xx-large":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "2em");
                            nd.Attributes["style"].Value = v;
                            break;
                        case "xx-small":
                            v = v.Remove(idx, idxLast - idx);
                            v = v.Insert(idx, "0.4em");
                            nd.Attributes["style"].Value = v;
                            break;
                    }
                }
                body = htmlDoc.DocumentNode.OuterHtml;

            }
            try
            {
                p.Copie = 1;
                p.ForcePreview = true;
                p.AddParameter("HTML", body);
            }
            catch (Exception ex)
            {
                //ErrorLogInfo errorlog = new ErrorLogInfo("", "ERR_325", ex.Message, "", "", "", "", "", "", "", "", "", "");
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Impossibile produrre la stampa. " + ex.Message,
                        "ERR_325",
                        string.Empty,
                        string.Empty,
                        ex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    _log.Error(er);
                }
                (this.Page as BasePage).info.AddMessage("Impossibile produrre la stampa", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                return;
            }
            p.DownloadPRU();

        }


        #endregion

        #region "Private Methods"
        private void SetHeaderButtonsVisibility(string folder, string parentFolder)
        {
            Dictionary<HeaderButtons, bool> buttonVisible =
                this.ViewState[BUTTONS_HEADER] as Dictionary<HeaderButtons, bool>;
            if (buttonVisible == null) return;
            string tipo = Helpers.GetTipo(folder);
            switch (parentFolder)
            {
                case "I":
                    switch (tipo)
                    {
                        case "I":
                        case "A":
                            buttonVisible[HeaderButtons.Rating] = true;
                            foreach (HeaderButtons but in buttonVisible.Keys.ToList())
                                if (but != HeaderButtons.ReSend)
                                    buttonVisible[but] = true;
                                else buttonVisible[but] = false;
                            buttonVisible[HeaderButtons.Rating] = true;
                            break;
                        case "C":
                            foreach (HeaderButtons but in buttonVisible.Keys.ToList())
                                buttonVisible[but] = false;
                            buttonVisible[HeaderButtons.MailTree] = true;
                            buttonVisible[HeaderButtons.Rating] = true;
                            break;
                        case "R":
                            foreach (HeaderButtons but in buttonVisible.Keys.ToList())
                                buttonVisible[but] = false;
                            buttonVisible[HeaderButtons.MailTree] = true;
                            break;
                    }
                    break;
                case "O":
                case "OA":
                case "AO":
                    buttonVisible[HeaderButtons.Acquire] = true;
                    buttonVisible[HeaderButtons.Forward] = true;
                    buttonVisible[HeaderButtons.MailTree] = true;
                    buttonVisible[HeaderButtons.Rating] = false;
                    buttonVisible[HeaderButtons.ReplyAll] = false;
                    buttonVisible[HeaderButtons.ReplyTo] = false;
                    buttonVisible[HeaderButtons.ReSend] = true;
                    break;
                    //case MailFolder.Problemi_di_consegna:
                    //    foreach (HeaderButtons but in buttonVisible.Keys.ToList())
                    //        buttonVisible[but] = false;
                    //    buttonVisible[HeaderButtons.MailTree] = true;
                    //    break; 

            }
        }

        private void LoadMessage(Message msg)
        {
            this.Visible = true;
            int id = 0;
            string uid = null;
            //il messaggio viene dalla INBOX            
            id = msg.Id;
            uid = msg.Uid;
            if (msg.OriginalData != null)
            {

                try
                {
                    lbDownload.Visible = false;
                    msg = Parser.ParseMessage(msg.OriginalData);
                    msg.Id = id;
                    msg.Uid = uid;
                    WebMailClientManager.CurrentMailSet(msg);
                }
                catch (OutOfMemoryException oom)
                {
                    msg.Id = id;
                    msg.Uid = uid;
                    WebMailClientManager.CurrentMailSet(msg);
                    lbDownload.Visible = true;
                    ManagedException mEx = new ManagedException(oom.Message,
                        "ERR_G049",
                        string.Empty,
                        string.Empty,
                        oom);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    _log.Error(er);
                    throw mEx;

                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        ManagedException mEx = new ManagedException("Errore nel caricamento dell'email : " + ex.Message,
                            "ERR977",
                            string.Empty,
                            string.Empty,
                            ex);
                        ErrorLogInfo er = new ErrorLogInfo(mEx);
                        _log.Error(er);
                    }
                 
                    (this.Page as BasePage).info.AddMessage("Impossibile elaborare la email riprovare in un secondo momento", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }
            }

            this.CurrentMessage = msg;
            MailContent.Visible = true;
            if (string.IsNullOrEmpty(msg.BodyHtml.Text))
            {

                MailContent.InnerHtml = msg.BodyText.Text.Replace("cid:", Request.Url + "?cont=");
            }
            else { MailContent.InnerHtml = msg.BodyHtml.Text.Replace("cid:", Request.Url + "?cont="); }

            Repeater1.DataSource = msg.Attachments;
            Repeater1.DataBind();

            List<MimePart> objs = new List<MimePart>();
            foreach (MimePart p in msg.EmbeddedObjects)
            {
                if (!String.IsNullOrEmpty(p.Filename))
                    objs.Add(p);
            }
            foreach (MimePart p in msg.UnknownDispositionMimeParts)
            {
                if (!string.IsNullOrEmpty(p.Filename))
                    objs.Add(p);
            }

            Repeater2.DataSource = objs;
            Repeater2.DataBind();

            rptSubMessage.DataSource = msg.SubMessages;
            rptSubMessage.DataBind();

            PnlEmbeddedElements.Visible = (msg.EmbeddedObjects != null && msg.EmbeddedObjects.Count > 0) ||
                (msg.UnknownDispositionMimeParts != null && msg.UnknownDispositionMimeParts.Count > 0);
            if (msg.Container != null)
            {
                lnbSMBack.Visible = true;
            }
            else
            {
                lnbSMBack.Visible = false;
            }
            PnlAttachment.Visible = !(msg.Attachments == null || msg.Attachments.Count == 0);
            PnlInnerMail.Visible = !(msg.SubMessages == null || msg.SubMessages.Count == 0);

            hfIdMail.Value = msg.Id.ToString();
            if (msg.Uid != null)
            { hfUIDMail.Value = msg.Uid; }
        }
        #endregion

        #region "CODICE COMMENTATO"

        //protected void Action_WorkOn(object sender, EventArgs e)
        //{
        //Facade                      facade;
        //Message                     msg;
        ////LR//facade                      = Facade.GetInstance();
        //facade                      = Facade.GetInstance( (AccountSettings)Session[ Constants.SESSIONKEY_PO_SETTINGS ] );
        //msg                         = facade.CurrentMailShow;
        //for ( int i = 0; i < msg.Attachments.Count; i++ )
        //    Attachment_DownloadTo( msg, i, Path.GetTempPath() );
        //// msg.MessageId 
        //}

        #endregion

    }
}