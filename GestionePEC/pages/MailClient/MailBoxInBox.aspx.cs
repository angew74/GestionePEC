using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using GestionePEC.Controls;
using GestionePEC.Extensions;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.MailClient
{
    public partial class MailBoxInBox : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MailBoxInBox));

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login_OnChangeStatus(object sender, EventArgs e)
        {
            Inbox1.Initialize();
            MailViewer1.Initialize(null, null, null, null);
            pnlMail.Update();
            pnlNav.Update();
        }

        protected void Inbox1_OnRowSelected(object sender, InBox.RowSelectedEventArgs e)
        {
            if (!WebMailClientManager.AccountIsValid())
                AccountInvalidated();
            if (!e.UId.IsNullOrWhiteSpace())
            {
                if (!(WebMailClientManager.CurrentMailExist() && e.UId.Trim() == WebMailClientManager.CurrentMailGet().Uid))
                {

                    // string folder = e.CurrentFolder.Parse(0);                    
                    //  MailFolder mailFolder = (MailFolder)folder;
                    WebMailClientManager.CurrentFolderSet(e.CurrentFolder);
                    WebMailClientManager.ParentFolderSet(e.ParentFolder);
                    if (e.Dimension < int.Parse(ConfigurationManager.AppSettings["MaxMemoryDimensionForMailViewer"]))
                    {
                        MailViewer1.Initialize(e.UId, e.CurrentRating, e.CurrentFolder, e.ParentFolder);
                    }
                    else
                    {
                        Message msg = new Message();
                        MailViewer1.hfUIDMailValue = msg.Uid = e.UId;
                        msg.Size = e.Dimension;
                        WebMailClientManager.CurrentMailSet(msg);
                        MailViewer1.Initialize(e.UId, e.CurrentRating, WebMailClientManager.CurrentFolderGet(), WebMailClientManager.ParentFolderGet());
                        pnlMail.Update();
                    }
                }
            }
        }

        protected void mailTreeViewer_MailSelected(object sender, EventArgs e)
        {
            if (!WebMailClientManager.AccountIsValid())
                AccountInvalidated();
            string idMail = MailViewer1.hfIdMailValue;
            if (!idMail.IsNullOrWhiteSpace())
            {
                long id = idMail.Parse(-1);
                if (id == -1)
                {
                    this.info.AddMessage("Impossibile caricare il messaggio", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    MailViewer1.Initialize();
                }
            }
        }

        protected void MailViewer1_AccountInvalid(object sender, EventArgs e)
        {
            AccountInvalidated();
        }


        // Testare bene todo modificare mailmove
        protected void MailViewer1_OnRequireAction(object sender, MailActionEventArgs action, string parentFolder)
        {
            switch (action.Action)
            {
                case MailActions.ACQUIRE:
                    if (WebMailClientManager.CurrentMailExist() && WebMailClientManager.AccountIsValid())
                    {
                        MailUser muser = WebMailClientManager.getAccount();
                        Message msg = WebMailClientManager.CurrentMailGet();
                        while (msg.Container != null)
                        {
                            msg = msg.Container;
                            WebMailClientManager.CurrentMailRemove();
                        }
                        if (!WebMailClientManager.CurrentMailExist())
                            WebMailClientManager.CurrentMailSet(msg);
                        if (!String.IsNullOrEmpty(msg.HeaderFields["x-trasporto"])
                            && msg.HeaderFields["x-trasporto"].Equals("posta-certificata", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string uid = msg.Uid;
                            int id = msg.Id;
                            msg = msg.SubMessages[0];
                            msg.Uid = uid;
                            msg.Id = id;
                        }
                        try
                        {
                            //TODO:ACQUISIOZNE MAIL INIT BEAKPONT - ALBERTO
                            if (msg.Uid == null)
                            { msg.Uid = MailViewer1.hfUIDMailValue; }
                            //Richiesta req = FaxPec.ServiceContracts.ServiceLocator.GetServiceFactory().RichiestaService.ProcessMail(muser, msg, parentFolder);
                            //if (req != null)
                            //{
                            //    List<string> uids = new List<string>();
                            //    uids.Add(msg.Uid);
                            //    string utente = Com.Delta.Anag.Security.MySecurityProvider.CurrentPrincipalName;
                            //    ServiceLocator.GetServiceFactory().getMailServerFacade(muser).MailMove(uids, MailStatus.LETTA, "1", utente, parentFolder);
                            //    FaxPec.Caching.Session.SessionManager<Richiesta>.set(FaxPec.Caching.Session.SessionKeys.FAXPEC_RICHIESTA, req);
                            //}
                        }
                        catch (ManagedException bex)
                        {
                            if (bex.GetType() != typeof(ManagedException))
                            {
                                ManagedException mEx = new ManagedException(bex.Message, "ERR_G043", string.Empty, string.Empty, bex);
                                ErrorLogInfo er = new ErrorLogInfo(mEx);
                                log.Error(er);
                            }
                            ((BasePage)Page).info.AddMessage(bex, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                            return;
                        }
                      //  Response.Redirect("~/pages/Istruz/IstruzioneRichiesta.aspx?m=m");
                    }
                    break;
                case MailActions.FORWARD:
                case MailActions.REPLY_TO:
                case MailActions.RE_SEND:
                    ucSendMail.Visible = true;
                    if (WebMailClientManager.AccountIsValid())
                    {
                        ucSendMail.LoginVisibile(false);
                    }
                    ucSendMail.Initialize(action.Action);
                    break;
                case MailActions.SEND:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ucSendMail_MessageInvalid(object sender, EventArgs e)
        {
            WebMailClientManager.CurrentMailRemove();
            MailViewer1.Initialize();
            (this.Page as BasePage).info.AddMessage("Mail non disponibile. Ripetere la scelta", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
        }

        #region Private methods

        private void AccountInvalidated()
        {
            WebMailClientManager.AccountRemove();
            WebMailClientManager.CurrentFolderRemove();
            WebMailClientManager.CurrentMailRemove();
            Inbox1.Initialize();
            pnlLogin.Update();
            pnlMail.Update();
            pnlNav.Update();
            this.info.AddMessage("Account non più valido. Ripetere la selezione della casella di posta", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
        }

        #endregion

    }
}