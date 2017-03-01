using ActiveUp.Net.Common.DeltaExt;
using Com.Delta.Mail.MailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class NewMail : System.Web.UI.UserControl
    {
        #region "Delegates & events"

        /// <summary>
        /// Notifica l'azione di mail inviata.
        /// </summary>
        public event EventHandler MailSent;
        private void onMailSent(object sender, EventArgs e)
        {
            if (MailSent != null)
                MailSent(sender, e);
        }
        /// <summary>
        /// Notifica l'evento di account invalido
        /// </summary>
        public event EventHandler AccountInvalid;
        private void onAccountInvalid(object sender, EventArgs e)
        {
            if (AccountInvalid != null)
                AccountInvalid(sender, e);
        }
        /// <summary>
        /// Notifica l'evento di messaggio invalido
        /// </summary>
        public event EventHandler MessageInvalid;
        private void onMessageInvalid(object sender, EventArgs e)
        {
            if (MessageInvalid != null)
                MessageInvalid(sender, e);
        }

        #endregion

        private const string MAIL_ACTION = "MAIL_ACTION";
        private const string SOTTOTITOLO = "SOTTOTITOLO";
        private const string MAIL_EDITABILE = "MAIL_EDITABILE";

        public MailActions SendType
        {
            get
            {
                MailActions act = MailActions.NONE;
                object o = ViewState[MAIL_ACTION];
                if (o != null)
                {
                    act = (MailActions)o;
                }
                return act;
            }
            set { ViewState[MAIL_ACTION] = value; }
        }
        public string SottoTitolo
        {
            get { return (string)ViewState[SOTTOTITOLO]; }
            set { ViewState[SOTTOTITOLO] = value; }
        }
        public bool MailEditabile
        {
            get
            {
                object _MailEditabile = ViewState[MAIL_EDITABILE];
                if (_MailEditabile == null)
                    return false;
                return (bool)_MailEditabile;
            }
            set { ViewState[MAIL_EDITABILE] = value; }
        }
        public Boolean LoginVisible
        {
            set { this.Login.Visible = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (WebMailClientManager.AccountIsValid())
                {
                    MailComposer1.Initialize(SendType, this.SottoTitolo);
                }
            }
        }

        protected void Login_OnChangeStatus(object sender, EventArgs e)
        {
            MailComposer1.Initialize(SendType, this.SottoTitolo);
        }

        public void Initialize(MailActions sendType)
        {
            this.SendType = sendType;
            MailComposer1.Initialize(sendType, this.SottoTitolo);
        }

        public void Initialize(MailActions sendType, bool mailEditabile)
        {
            this.SendType = sendType;
            this.MailEditabile = mailEditabile;
            MailComposer1.Initialize(sendType, this.SottoTitolo, this.MailEditabile);
        }


        /// <summary>
        /// Propaga la notifica di mail inviata.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MailComposer1_OnMailSent(object sender, EventArgs e)
        {
            onMailSent(sender, e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MailComposer1_AccountInvalid(object sender, EventArgs e)
        {
            WebMailClientManager.AccountRemove();
            this.Login.Visible = true;
            onAccountInvalid(sender, e);
        }

        protected void MailComposer1_MessageInvalid(object sender, EventArgs e)
        {
            onMessageInvalid(sender, e);
        }
    }
}