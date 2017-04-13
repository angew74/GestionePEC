using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using GestionePEC.Controls;
using GestionePEC.Extensions;
using SendMail.BusinessEF.MailFacedes;
using System;
using System.Configuration;

namespace GestionePEC.pages.MailClient
{
    public partial class MailSearch : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void MailResearch_Mail(RicercaMail.MailSelectedEventArgs e)
        {

            if (!e.UId.IsNullOrWhiteSpace())
            {
                if (!(WebMailClientManager.CurrentMailExist() && e.UId.Trim() == WebMailClientManager.CurrentMailGet().Uid))
                {
                    MailServerConfigFacade mailserveconfigfacade = MailServerConfigFacade.GetInstance();
                    MailUser account = mailserveconfigfacade.GetUserByUserId(decimal.Parse(e.CurrentAccount));                  
                    account.Validated = true;
                    WebMailClientManager.SetAccount(account);
                    int idim = 0;
                    WebMailClientManager.CurrentFolderSet(e.CurrentFolder);
                    WebMailClientManager.ParentFolderSet(e.ParentFolder);
                    if (e.Dimension != string.Empty) { idim = int.Parse(e.Dimension); }
                    if (idim < int.Parse(ConfigurationManager.AppSettings["MaxMemoryDimensionForMailViewer"]))
                    {
                        MailViewer1.Initialize(e.UId, e.CurrRating, e.CurrentFolder, e.ParentFolder);
                        pnlMail.Update();
                    }
                    else
                    {
                        Message msg = new Message();
                        msg.Uid = e.UId;
                        msg.Size = idim;
                        WebMailClientManager.CurrentMailSet(msg);
                        MailViewer1.Initialize(e.UId, e.CurrRating, WebMailClientManager.CurrentFolderGet(), WebMailClientManager.ParentFolderGet());
                        pnlMail.Update();
                    }
                }
            }
        }

        protected void MailViewer1_OnRequireAction(object sender, MailActionEventArgs action, string parentFolder)
        {
        }

        protected void MailResearch_Hide(object sender, EventArgs e)
        {
            WebMailClientManager.CurrentMailRemove();
            MailViewer1.Initialize(null, null, null,null);
            pnlMail.Update();
        }

        protected void mailTreeViewer_MailSelected(object sender, EventArgs e)
        {

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

        protected void MailViewer1_AccountInvalid(object sender, EventArgs e)
        {

        }

        protected void MailViewer1_OnRequireAction(object sender, MailActionEventArgs e)
        {
            switch (e.Action)
            {
                case MailActions.ACQUIRE:
                    throw new NotImplementedException();
                case MailActions.REPLY_ALL:
                    throw new NotImplementedException();
                case MailActions.FORWARD:
                case MailActions.REPLY_TO:
                case MailActions.RE_SEND:
                    ucSendMail.Visible = true;
                    if (WebMailClientManager.AccountIsValid())
                    {
                        ucSendMail.LoginVisibile(false);
                    }
                    ucSendMail.Initialize(e.Action);
                    break;
                case MailActions.SEND:
                    ucSendMail.Visible = true;
                    if (WebMailClientManager.AccountIsValid())
                    {
                        ucSendMail.LoginVisibile(false);
                    }
                    ucSendMail.Initialize(MailActions.SEND, false);
                    break;
            }
        }
    }
}