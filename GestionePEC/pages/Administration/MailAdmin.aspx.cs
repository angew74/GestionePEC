using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using log4net;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.Administration
{
    public partial class MailAdmin : BasePage
    {
        private static readonly ILog log = LogManager.GetLogger("MailAdmin");

        #region ViewState

        public Decimal IdSender_ViewState
        {
            get
            {
                return (Decimal)(this.ViewState["idSender"]);
            }
            set
            {
                this.ViewState["idSender"] = value;
            }
        }

        public Decimal IdServer_ViewState
        {
            get
            {
                return (decimal)(this.ViewState["idServer"]);
            }
            set
            {
                this.ViewState["idServer"] = value;
            }
        }
       

        public int RoleBackendUser_ViewState
        {
            get
            {
                return (int)(this.ViewState["roleBackendUser"]);
            }
            set
            {
                this.ViewState["roleBackendUser"] = value;
            }
        }

        public Decimal UserIdBackendUser_ViewState
        {
            get
            {
                return (Decimal)(this.ViewState["userIdBackendUser"]);
            }
            set
            {
                this.ViewState["userIdBackendUser"] = value;
            }
        }

        #endregion

        #region "Properties"

        internal string mailRichiedente = String.Empty;

        public List<MailServer> MailServers
        {
            get
            {
                List<MailServer> s = new List<MailServer>();
                try
                {
                    MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                    s = facade.GetAll()
                        as List<MailServer>;
                }
                catch { }
                if (s.SingleOrDefault(x => x.Id == 0) == null)
                    s.Insert(0, new MailServer() { Id = 0, DisplayName = "" });
                return s;
            }
        }

        private BackendUser _bUser;
        public BackendUser bUser
        {
            get
            {
                return _bUser;
            }
        }
        #endregion

        #region Metodi Privati

        private void GetRoleBackEndUser()
        {
            this.RoleBackendUser_ViewState = (from map in bUser.MappedMails
                                              where map.UserId == this.IdSender_ViewState
                                              select map.MailAccessLevel).SingleOrDefault();
        }

        private bool IsValidEmailDesc(string email)
        {
            string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            return Regex.IsMatch(email, MatchEmailPattern);
        }

        #endregion

        #region ElencoEmailsShared

        private void popolaGridElencoEmailsShared()
        {
            List<BackEndUserMailUserMapping> listMailSender = null;
            if (bUser != null)
            {
                listMailSender = bUser.MappedMails;
                if (listMailSender != null && listMailSender.Count > 0)
                {
                    listMailSender = listMailSender.OrderBy(ms => ms.EmailAddress).ToList();
                }
            }
            gvElencoEmailsShared.DataSource = listMailSender;
            gvElencoEmailsShared.DataBind();
        }

        protected void gvElencoEmailsShared_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            List<BackEndUserMailUserMapping> listMailSender = bUser.MappedMails;
            if (listMailSender != null)
                listMailSender = listMailSender.OrderBy(ms => ms.EmailAddress).ToList();
            try
            {
                gvElencoEmailsShared.DataSource = listMailSender;
                gvElencoEmailsShared.PageIndex = e.NewPageIndex;
                gvElencoEmailsShared.DataBind();
            }
            catch (Com.Delta.Logging.ManagedException me)
            {
                info.AddMessage(me.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
            catch (Exception ex)
            {

                ManagedException mEx = new ManagedException("Errore nella paginazione della griglia", "CM006",
                    string.Empty, string.Empty, ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                err.loggingAppCode = "WEB_MAIL";
                err.objectID = this.Context.Session.SessionID;
                if (MySecurityProvider.CurrentPrincipal != null && MySecurityProvider.CurrentPrincipal.MyIdentity != null)
                    err.userID = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
                log.Error(err);
                info.AddMessage(mEx);
            }
        }

        protected void gvElencoEmailsShared_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (bUser == null)
            {
                info.AddMessage("Utente sconosciuto. Disconnettersi e riconnetersi", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.INFO);
                return;
            }
            switch (e.CommandName)
            {
                case "GestioneEmail":
                    pnlGestioneEmail.Visible = true;
                    pnlDettaglioEmailServer.Visible = false;                    
                    this.IdSender_ViewState = Decimal.Parse(e.CommandArgument.ToString());
                    if (bUser.UserRole == 0)
                        fvEmail.ChangeMode(FormViewMode.ReadOnly);
                    else
                    {
                        var currMail = bUser.MappedMails.SingleOrDefault(x => x.UserId == this.IdSender_ViewState);
                        if (currMail != null && currMail.MailAccessLevel > 0)
                            fvEmail.ChangeMode(FormViewMode.Edit);
                        else
                            fvEmail.ChangeMode(FormViewMode.ReadOnly);
                    }
                    fvEmail.DataBind();
                    break;

                case "GestioneServer":
                    this.IdServer_ViewState = decimal.Parse(e.CommandArgument.ToString());
                    pnlDettaglioEmailServer.Visible = true;
                    pnlGestioneEmail.Visible = false;                   
                    if (bUser.UserRole == 2)
                        fvServer.ChangeMode(FormViewMode.Edit);
                    else
                        fvServer.ChangeMode(FormViewMode.ReadOnly);
                    fvServer.DataBind();
                    break;   
            }
        }

        protected void btnInserimentoServer_OnClick(object sender, EventArgs e)
        {
            pnlDettaglioEmailServer.Visible = true;          
            pnlGestioneEmail.Visible = false;           
            this.IdServer_ViewState = decimal.MinusOne;
            fvServer.ChangeMode(FormViewMode.Insert);
        }

        protected void btnInserimentoEmail_OnClick(object sender, EventArgs e)
        {
            pnlGestioneEmail.Visible = true;           
            pnlDettaglioEmailServer.Visible = false;          
            this.IdSender_ViewState = decimal.MinusOne;
            fvEmail.ChangeMode(FormViewMode.Insert);
        }

        #endregion

        #region GestioneEmail

        protected void odsMailConfig_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            if (this.IdSender_ViewState != decimal.MinusOne)
                try
                {
                    MailServerConfigFacade serverFacade = MailServerConfigFacade.GetInstance();
                    e.ObjectInstance = serverFacade.GetUserByUserId(this.IdSender_ViewState);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {

                        ManagedException mEx = new ManagedException("Errore nel caricamento della configurazione della mail", "CM007",
                            string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.loggingAppCode = "WEB_MAIL";
                        err.objectID = this.Context.Session.SessionID;
                        log.Error(err);

                        info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    }
                    else
                    {
                        info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    }
                }
            else
                e.ObjectInstance = new MailUser();
        }

        protected void odsMailConfig_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            try
            {
                MailAccountService mailAccountService = new MailAccountService();
                MailUser mu = e.InputParameters[0] as MailUser;
                if (!IsValidEmailDesc(mu.EmailAddress))
                {
                    e.Cancel = true;
                    info.AddMessage("Errore nel formato della mail", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    return;
                }
                mu.FlgManaged = (mu.FlgManagedInsert == true) ? 1 : 0;
                mailAccountService.Update(mu);
                info.AddMessage("Operazione effettauata", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.OK);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Errore nel caricamento della configurazione della mail", "CM008",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;
                    log.Error(err);

                    info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
            e.Cancel = true;
        }

        protected void odsMailConfig_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            MailUser mu = e.InputParameters[0] as MailUser;
            MailAccountService mailAccountService = new MailAccountService();
            mu.IdResponsabile =decimal.Parse(MySecurityProvider.CurrentPrincipal.MyIdentity.Id);
            if (!IsValidEmailDesc(mu.EmailAddress))
            {
                e.Cancel = true;
                info.AddMessage("Errore nel formato della mail", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                return;
            }
            try
            {
                mu.FlgManaged = (mu.FlgManagedInsert == true) ? 1 : 0;
                string serverId = (fvEmail.FindControl("ddlListaServers") as DropDownList).SelectedValue;
                int serverid = 0;
                int.TryParse(serverId,out serverid);
                if (mailAccountService.GetUserByServerAndUsername(serverid, mu.EmailAddress.ToLower()).FirstOrDefault().Id > 0)
                {
                    info.AddMessage("Attenzione email già esistente", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    e.Cancel = true;
                    return;
                }
                else
                {
                    mailAccountService.Insert(mu);
                    this.IdSender_ViewState = mu.UserId;
                    BackendUserService buservice = new BackendUserService();
                    _bUser = (BackendUser)buservice.GetByUserName(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                    popolaGridElencoEmailsShared();
                    SessionManager<BackendUser>.set(SessionKeys.BACKEND_USER, _bUser);
                    info.AddMessage("Operazione effettuata", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.OK);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {

                    ManagedException mEx = new ManagedException("Errore nell'inserimento della nuova configurazione mail", "CM009",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;
                    log.Error(err);

                    info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
            e.Cancel = true;
        }

        protected void fvEmail_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            if (bUser != null && bUser.UserRole != 0)
            {
                if (e.NewMode == FormViewMode.ReadOnly)
                {
                    if (this.IdSender_ViewState != decimal.MinusOne)
                        e.NewMode = FormViewMode.Edit;
                    else
                        e.Cancel = true;
                }
            }
        }

        protected void fvEmail_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Cancel":
                    pnlGestioneEmail.Visible = false;
                    break;
                default:
                    break;
            }
        }

        protected void fvEmail_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            foreach (DictionaryEntry v in e.Values)
            {
                if (String.IsNullOrEmpty(v.Value.ToString()))
                    e.Values[v.Key] = null;
            }
        }

        protected void fvEmail_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            foreach (DictionaryEntry v in e.NewValues)
            {
                if (String.IsNullOrEmpty(v.Value.ToString()))
                    e.NewValues[v.Key] = null;
            }
        }

        #endregion

        #region "Servers"
        protected void odsServerConfig_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            if (this.IdServer_ViewState != decimal.MinusOne)
            {
                try
                {
                    MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                    e.ObjectInstance = facade.LoadServerConfigById(this.IdServer_ViewState);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {

                        ManagedException mEx = new ManagedException("Errore nel caricamento della configurazione del server", "CM020",
                            string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.loggingAppCode = "WEB_MAIL";
                        err.objectID = this.Context.Session.SessionID;
                        log.Error(err);

                        info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    }
                    else
                    {
                        info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                    }
                }
            }
            else
                e.ObjectInstance = new MailServer();
        }

        protected void odsServerConfig_Updating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            MailServer ms = e.InputParameters[0] as MailServer;
            try
            {
                MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                facade.updateServerConfig(ms);
                info.AddMessage("Operazione effettuata", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.OK);
            }
            catch (ManagedException mEx)
            {
                info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
            catch (Exception ex)
            {

                ManagedException mEx = new ManagedException("Errore nel caricamento della configurazione del server", "CM021",
                    string.Empty, string.Empty, ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                err.loggingAppCode = "WEB_MAIL";
                err.objectID = this.Context.Session.SessionID;
                if (MySecurityProvider.CurrentPrincipal != null && MySecurityProvider.CurrentPrincipal.MyIdentity != null)
                    err.userID = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
                log.Error(err);
                info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
            e.Cancel = true;
        }

        protected void odsServerConfig_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
        {
            MailServer ms = e.InputParameters[0] as MailServer;
            try
            {
                MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                facade.insertServerConfig(ms);
                this.IdServer_ViewState = ms.Id;
                info.AddMessage("Operazione effettuata", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.OK);
            }
            catch (ManagedException mEx)
            {
                info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
            catch (Exception ex)
            {

                ManagedException mEx = new ManagedException("Errore nell'inserimento del nuovo server", "CM010",
                    string.Empty, string.Empty, ex.InnerException);
                ErrorLogInfo err = new ErrorLogInfo(mEx);
                err.loggingAppCode = "WEB_MAIL";
                err.objectID = this.Context.Session.SessionID;
                if (MySecurityProvider.CurrentPrincipal != null && MySecurityProvider.CurrentPrincipal.MyIdentity != null)
                    err.userID = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
                log.Error(err);
                info.AddMessage(mEx.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
            }
        }

        protected void fvServer_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            if (e.NewMode == FormViewMode.ReadOnly)
            {
                if (bUser != null && bUser.UserRole == 1)
                {
                    if (this.IdServer_ViewState != decimal.MinusOne)
                        e.NewMode = FormViewMode.Edit;
                    else
                        e.Cancel = true;
                }
            }
        }

        protected void fvServer_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            foreach (DictionaryEntry de in e.Values)
            {
                if (de.Value.ToString() == string.Empty)
                {
                    e.Values[de.Key] = null;
                }
            }
        }

        protected void fvServer_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            foreach (DictionaryEntry de in e.NewValues)
            {
                if (de.Value.ToString() == string.Empty)
                {
                    e.NewValues[de.Key] = null;
                }
            }
        }

        protected void fvServer_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "annulla":
                    pnlDettaglioEmailServer.Visible = false;
                    break;
                default:
                    break;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MySecurityProvider.CurrentPrincipal == null)
            {
                Redirect("LogOffPage", null);
                //Response.Redirect("~/LogOff.aspx");
            }

            try
            {
                if (!(SessionManager<BackendUser>.exist(SessionKeys.BACKEND_USER)))
                {
                    BackendUserService buservice = new BackendUserService();
                    _bUser = (BackendUser)buservice.GetByUserName(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                    SessionManager<BackendUser>.set(SessionKeys.BACKEND_USER, _bUser);
                }
                else { _bUser = SessionManager<BackendUser>.get(SessionKeys.BACKEND_USER); }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {

                    ManagedException mEx = new ManagedException("Errore nel caricamento dei dati utente", "CM001",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;
                    log.Error(err);
                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }

            if (!IsPostBack)
            {
                if (bUser != null)
                {
                    this.UserIdBackendUser_ViewState = bUser.UserId;                    
                    popolaGridElencoEmailsShared();
                }
                else
                {
                    //Response.Redirect("~/LogOff.aspx");
                    Redirect("LogOffPage", null);
                }
            }
        }
    }
}