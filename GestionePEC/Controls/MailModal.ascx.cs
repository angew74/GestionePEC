using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{
    public partial class MailModal : System.Web.UI.UserControl
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
        /// Notifica l'errore del messaggio da caricare.
        /// </summary>
        public event EventHandler MessageInvalid;
        private void onMessageInvalid(object sender, EventArgs e)
        {
            if (MessageInvalid != null)
                MessageInvalid(sender, e);
        }

        /// <summary>
        /// Notifica la chiusura del PopUp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public event EventHandler MailClose;
        private void onMailClose(object sender, EventArgs e)
        {
            if (MailClose != null)
                MailClose(sender, e);
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Visualizza il PopUp.
        /// </summary>
        public void ShowPopUp()
        {
            PopUpExt.Show();
        }

        /// <summary>
        /// Nasconde il PopUp.
        /// </summary>
        public void HidePopUp()
        {
            PopUpExt.Hide();
        }

        public string SottoTitolo
        {
            get { return SendMailMessage.SottoTitolo; }
            set { SendMailMessage.SottoTitolo = value; }
        }
        /// <summary>
        /// Chiude il PopUp Mail.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPopUpCancel_Click(object sender, EventArgs e)
        {
            Com.Delta.Mail.MailMessage.WebMailClientManager.CurrentSendMailClear();
            if (this.MailClose != null)
            { onMailClose(sender, e); }
            HidePopUp();
        }

        /// <summary>
        /// Propaga la notifica di mail inviata ricevuta da SendMail.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SendMailMessage_OnMailSent(object sender, EventArgs e)
        {
            onMailSent(sender, e);
            HidePopUp();
        }

        protected void SendMailMessage_AccountInvalid(object sender, EventArgs e)
        {
            upnlMailPopUp.Update();
        }

        protected void SendMailMessage_MessageInvalid(object sender, EventArgs e)
        {
            onMessageInvalid(sender, e);
            HidePopUp();
        }

        public void Initialize(ActiveUp.Net.Common.DeltaExt.MailActions action)
        {
            ShowPopUp();
            SendMailMessage.Initialize(action);
        }

        public void Initialize(ActiveUp.Net.Common.DeltaExt.MailActions action, bool mailEditabile)
        {
            ShowPopUp();
            SendMailMessage.Initialize(action, mailEditabile);
        }

        public void LoginVisibile(bool visible)
        {
            SendMailMessage.LoginVisible = visible;
        }
    }
}