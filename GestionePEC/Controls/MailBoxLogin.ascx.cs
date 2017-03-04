using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using log4net;
using SendMail.Locator;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Controls
{

    public partial class MailBoxLogin : System.Web.UI.UserControl
    {
        public string CurrentMail = "Login";

        private static readonly ILog log = LogManager.GetLogger(typeof(MailBoxLogin));

        public event EventHandler ChangeStatus;
        protected virtual void OnStatusChanged()
        {
            if (ChangeStatus != null)
            {
                ChangeStatus(this, EventArgs.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddlManagedAccounts.DataBind();
                ddlServer.DataBind();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            calcVisibility();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            MailServer m = ServiceLocator.GetServiceFactory().getMailServerConfigFacade().LoadServerConfigById(decimal.Parse(ddlServer.SelectedValue));
            MailUser account = new MailUser(m);
            account.Password = txtPassword.Text;
            account.LoginId = txtName.Text;
            account.EmailAddress = txtName.Text;
            account.Dominus = hidDominus.Value;
            account.Casella = string.Empty;
            try
            {
                IList<MailUser> ctrlAccount = ServiceLocator.GetServiceFactory().getMailServerConfigFacade().GetUserByServerAndUsername(account.Id, account.LoginId);
                if (ctrlAccount != null)
                {
                    if (ctrlAccount.Where(acc => acc.LoginId.Equals(account.LoginId) && (acc.Id == m.Id) && acc.IsManaged).Count() != 0)
                    {
                        ((BasePage)this.Page).info.AddError("Questo account è gestito applicativamente. Impossibile effetuare il login da applicazione");
                        return;
                    }
                }

                SetAccount(account);
                OnStatusChanged();
            }
            catch (ManagedException bex)
            {
                if (bex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(bex.Message, "ERR_G044", string.Empty, string.Empty, bex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                }
                ((BasePage)this.Page).info.AddError("Connessione al mail server impossibile, controllare le credenziali");
            }
        }

        protected void lbtnLogOff_Click(object sender, EventArgs e)
        {
            WebMailClientManager.AccountRemove();
            ddlManagedAccounts.SelectedIndex = 0;
            ddlServer.SelectedIndex = 0;
            OnStatusChanged();
        }

        private void calcVisibility()
        {
            if (WebMailClientManager.AccountIsValid())
            {
                this.CurrentMail = WebMailClientManager.getAccount().Casella;
                IbLogOff.Visible = true;
                panLogin.Visible = false;
            }
            else
            {
                this.CurrentMail = "Login";
                IbLogOff.Visible = false;
                panLogin.Visible = true;
                ManagedAccount.Checked = true;
            }
        }

        protected void ddlManagedAccounts_DataBinding(object sender, EventArgs e)
        {
            string username = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
            List<MailUser> l = SessionManager<List<MailUser>>.get(SessionKeys.ACCOUNTS_LIST);
            if (!(l != null && l.Count != 0))
            {
                l = ServiceLocator.GetServiceFactory().getMailServerConfigFacade().GetManagedAccountByUser(username).ToList();
                if (l == null) l = new List<MailUser>();
                if (l.Where(x => x.UserId.Equals(-1)).Count() == 0)
                    l.Insert(0, new MailUser() { UserId = -1, EmailAddress = "" });
                SessionManager<List<MailUser>>.set(SessionKeys.ACCOUNTS_LIST,l);
            }
            DropDownList ddl = sender as DropDownList;
            ddl.DataSource = l;
            ddl.DataTextField = "EmailAddress";
            ddl.DataValueField = "UserId";
        }

        protected void ddlManagedAccounts_DataBound(object sender, EventArgs e)
        {
            //login automatico per unico account mail
            bool autoLogin = false;
            bool.TryParse(ConfigurationManager.AppSettings.Get("UseLoginAutomatico"), out autoLogin);
            if (autoLogin == true)
            {
                DropDownList ddl = sender as DropDownList;
                if (ddl.Items.Cast<ListItem>().Where(x => !x.Value.Equals("-1")).Count() == 1)
                {
                    ddl.Items[1].Selected = true;
                    ddlManagedAccounts_SelectedIndexChanged(ddl, EventArgs.Empty);
                }
            }
        }

        protected void ddlServer_DataBinding(object sender, EventArgs e)
        {
            IList<MailServer> l = ServiceLocator.GetServiceFactory().getMailServerConfigFacade().GetAll();
            if (l == null) l = new List<MailServer>();
            MailServer[] servers = (MailServer[])l.ToArray().Clone();

            if (servers.Where(x => x.Id.Equals(-1)).Count() == 0)
            {
                List<MailServer> lm = servers.ToList();
                lm.Insert(0, new MailServer() { Id = -1, DisplayName = "" });
                servers = lm.ToArray();
            }

            DropDownList ddl = (DropDownList)sender;
            ddl.DataSource = servers;
            ddl.DataTextField = "DisplayName";
            ddl.DataValueField = "Id";
        }

        protected void ddlManagedAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = sender as DropDownList;
            decimal userId = decimal.Parse(ddl.SelectedValue);
            if (userId != -1)
            {
                try
                {
                    MailUser account = ServiceLocator.GetServiceFactory().getMailServerConfigFacade().GetUserByUserId(userId);
                    SetAccount(account);
                    ddlManagedAccounts.SelectedIndex = 0;
                }
                catch
                {
                    ((BasePage)this.Page).info.AddError("Connessione al mail server impossibile, controllare le credenziali");
                }
            }
            else
            {
                WebMailClientManager.AccountRemove();
            }
            OnStatusChanged();
        }

        private static void SetAccount(MailUser account)
        {
            ServiceLocator.GetServiceFactory().getMailServerFacade(account);
            account.Validated = true;
            WebMailClientManager.SetAccount(account);
        }
    }
}