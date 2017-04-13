using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Extensions;
using log4net;
using SendMail.BusinessEF;
using SendMail.BusinessEF.Contracts;
using SendMail.BusinessEF.MailFacedes;
using SendMail.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.MailClient
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

        public String Dipartimento_ViewState
        {
            get
            {
                return (this.ViewState["dipartimento"]).ToString();
            }
            set
            {
                this.ViewState["dipartimento"] = value;
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
                    if (bUser.UserRole != 2)
                    {
                        this.Dipartimento_ViewState = MySecurityProvider.CurrentPrincipal.MyIdentity.dipartimento;
                    }
                    popolaGridElencoEmailsShared();
                }
                else
                {
                    //Response.Redirect("~/LogOff.aspx");
                    Redirect("LogOffPage", null);
                }
            }
        }

        private void PopolaDDLDipartimenti()
        {
            List<BackendUser> deps = new List<BackendUser>();
            BackendUserService bus = new BackendUserService();
            try
            {
                deps = (List<BackendUser>)bus.GetDipartimentiByAdmin(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                if (deps == null || deps.Count == 0)
                {
                    deps = (List<BackendUser>)bus.GetAllDipartimentiByMailAdmin(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                }
                ddlListaDipartimenti.DataSource = deps;
                ddlListaDipartimenti.DataBind();
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {                 
                    ManagedException mEx = new ManagedException("Errore nel caricamento dei dipatimenti", "CM002",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                 
                    log.Error(err);
                    throw mEx;
                }
                else
                {
                    throw ex;
                }
            }
        }

        private void popolaListaDipendentiNONAbilitati()
        {
            if (!String.IsNullOrEmpty(this.Dipartimento_ViewState) && this.IdSender_ViewState != Decimal.MinusOne)
            {
                try
                {
                    BackendUserService bus = new BackendUserService();
                    List<BackendUser> listaDipendentiNONAbilitati = bus.GetDipendentiDipartimentoNONAbilitati(this.Dipartimento_ViewState, this.IdSender_ViewState);
                    pnlElencoUtenti.Visible = true;

                    if (listaDipendentiNONAbilitati != null)
                    {
                        lvDipendentiNONAbilitati.DataSource = listaDipendentiNONAbilitati.Distinct().OrderBy(k => k.UserName);
                    }
                    else
                        lvDipendentiNONAbilitati.DataSource = null;
                    lvDipendentiNONAbilitati.DataBind();
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {                       
                        ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti non abilitati", "CM003",
                            string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.loggingAppCode = "WEB_MAIL";
                        err.objectID = this.Context.Session.SessionID;                      
                        log.Error(err);
                        throw mEx;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        private void popolaListaDipendentiAbilitati()
        {
            if (this.IdSender_ViewState != Decimal.MinusOne)
            {
                try
                {
                    BackendUserService bus = new BackendUserService();
                    List<BackendUser> listaDipendentiAbilitati = bus.GetDipendentiDipartimentoAbilitati(this.IdSender_ViewState);
                    if (listaDipendentiAbilitati != null)
                    {
                        lvDipendentiAbilitati.DataSource = listaDipendentiAbilitati.OrderBy(x => x.UserName);
                        lvDipendentiAbilitati.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {                     
                        ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti abilitati", "CM0014",
                            string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.loggingAppCode = "WEB_MAIL";
                        err.objectID = this.Context.Session.SessionID;                       
                        log.Error(err);
                       
                        throw mEx;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        private void popolaListaAmministratori()
        {
            if (this.IdSender_ViewState != Decimal.MinusOne)
            {
                try
                {
                    BackendUserService bus = new BackendUserService();
                    List<BackendUser> listaDipendentiAbilitati = bus.GetDipendentiDipartimentoAbilitati(this.IdSender_ViewState);
                    if (listaDipendentiAbilitati != null)
                    {
                        lvUtentiAdmin.DataSource = listaDipendentiAbilitati.Where(x => x.RoleMail == 1);
                        lvUtentiAdmin.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {                       
                        ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti abilitati", "CM015",
                            string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.loggingAppCode = "WEB_MAIL";
                        err.objectID = this.Context.Session.SessionID;                       
                        log.Error(err);   
                        throw mEx;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        private void popolaListaUtentiAbilitati()
        {
            if (this.IdSender_ViewState != Decimal.MinusOne)
            {
                try
                {
                    BackendUserService bus = new BackendUserService();
                    List<BackendUser> listaDipendentiAbilitati = bus.GetDipendentiDipartimentoAbilitati(this.IdSender_ViewState);
                    if (listaDipendentiAbilitati != null)
                    {
                        lvUtenti.DataSource = listaDipendentiAbilitati.Where(x => x.RoleMail == 0);
                        lvUtenti.DataBind();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ManagedException))
                    {
                        
                        ManagedException mEx = new ManagedException("Errore nel caricamento degli utenti abilitati", "CM004",
                            string.Empty, string.Empty, ex.InnerException);
                        ErrorLogInfo err = new ErrorLogInfo(mEx);
                        err.loggingAppCode = "WEB_MAIL";
                        err.objectID = this.Context.Session.SessionID;                       
                        log.Error(err);     
                        throw mEx;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
        }

        private void GetRoleBackEndUser()
        {
            this.RoleBackendUser_ViewState = (from map in bUser.MappedMails
                                              where map.UserId == this.IdSender_ViewState
                                              select map.MailAccessLevel).SingleOrDefault();
        }

        protected void ddlListaDipartimenti_itemSelected(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ddlListaDipartimenti.SelectedValue))
            {
                this.Dipartimento_ViewState = ddlListaDipartimenti.SelectedValue;

                if (this.IdSender_ViewState != Decimal.MinValue)
                {
                    try
                    {
                        popolaListaDipendentiNONAbilitati();
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() != typeof(ManagedException))
                        {
                            
                            ManagedException mEx = new ManagedException("Errore nel popolamento della lista dei dipendenti non abilitati", "CM005",
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
                }
            }
        }

        protected void lvDipendentiAbilitati_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;
                BackendUser bUser = (BackendUser)dataItem.DataItem;
                (e.Item.FindControl("lb_ROLE") as HiddenField).Value = bUser.MappedMails[0].MailAccessLevel.ToString();
            }
        }

        protected void lvUtentiAdmin_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;
                BackendUser bUser = (BackendUser)dataItem.DataItem;
                (e.Item.FindControl("lb_ROLE_ADMIN") as HiddenField).Value = bUser.MappedMails[0].MailAccessLevel.ToString();
            }
        }

        private bool IsValidEmailDesc(string email)
        {
            string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                                     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                                     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                                     + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            return Regex.IsMatch(email, MatchEmailPattern);
        }

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
                    pnlElencoUtenti.Visible = false;
                    pnlAdmin.Visible = false;
                    pnlGestioneFolders.Visible = false;
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
                    pnlAdmin.Visible = false;
                    pnlElencoUtenti.Visible = false;
                    pnlGestioneFolders.Visible = false;
                    if (bUser.UserRole == 2)
                        fvServer.ChangeMode(FormViewMode.Edit);
                    else
                        fvServer.ChangeMode(FormViewMode.ReadOnly);
                    fvServer.DataBind();
                    break;

                case "GestioneAssociazione":
                    pnlGestioneEmail.Visible = false;
                    pnlDettaglioEmailServer.Visible = false;
                    pnlAdmin.Visible = false;
                    pnlElencoUtenti.Visible = true;
                    pnlGestioneFolders.Visible = false;
                    this.PopolaDDLDipartimenti();
                    // string AdminDepartment = ConfigurationManager.AppSettings.Get("AdministrationDepartment");
                    // if (AdminDepartment.Contains(this.Dipartimento_ViewState))
                    //  {
                    if (string.IsNullOrEmpty(MySecurityProvider.CurrentPrincipal.MyIdentity.dipartimento))
                    {
                        this.Dipartimento_ViewState = "152";
                    }else { this.Dipartimento_ViewState = MySecurityProvider.CurrentPrincipal.MyIdentity.dipartimento; }
                    
                    ddlListaDipartimenti.Items.FindByValue(this.Dipartimento_ViewState).Selected = true;
                    lblDepartment.Visible = false;
                    // }
                    // else
                    // {
                    //   ddlListaDipartimenti.Visible = false;
                    //  lblDepartment.Visible = true;
                    //  lblDepartment.Text = this.Dipartimento_ViewState;
                    // }
                    this.IdSender_ViewState = Decimal.Parse(e.CommandArgument.ToString());
                    this.GetRoleBackEndUser();
                    this.popolaListaDipendentiAbilitati();
                    this.popolaListaDipendentiNONAbilitati();
                    break;

                case "GestioneFolders":
                    pnlGestioneEmail.Visible = false;
                    pnlDettaglioEmailServer.Visible = false;
                    pnlElencoUtenti.Visible = false;
                    pnlAdmin.Visible = false;
                    pnlGestioneFolders.Visible = true;
                    if (!e.CommandArgument.ToString().Equals(""))
                        Session.Add("MailRichiedente", e.CommandArgument.ToString());
                    popolaListaFoldersNonAbilitate(e.CommandArgument.ToString());
                    popolaListaFoldersAbilitate(e.CommandArgument.ToString());
                    break;
                case "GestioneAssociazioneAdmin":
                    pnlGestioneEmail.Visible = false;
                    pnlDettaglioEmailServer.Visible = false;
                    pnlElencoUtenti.Visible = false;
                    pnlAdmin.Visible = true;
                    pnlGestioneFolders.Visible = false;
                    this.IdSender_ViewState = Decimal.Parse(e.CommandArgument.ToString());
                    this.GetRoleBackEndUser();
                    this.popolaListaUtentiAbilitati();
                    this.popolaListaAmministratori();
                    break;

            }
        }

        protected void btnInserimentoServer_OnClick(object sender, EventArgs e)
        {
            pnlDettaglioEmailServer.Visible = true;
            pnlElencoUtenti.Visible = false;
            pnlGestioneEmail.Visible = false;
            pnlGestioneFolders.Visible = false;
            this.IdServer_ViewState = decimal.MinusOne;
            fvServer.ChangeMode(FormViewMode.Insert);
        }

        protected void btnInserimentoEmail_OnClick(object sender, EventArgs e)
        {
            pnlGestioneEmail.Visible = true;
            pnlElencoUtenti.Visible = false;
            pnlDettaglioEmailServer.Visible = false;
            pnlGestioneFolders.Visible = false;
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
            if (!IsValidEmailDesc(mu.EmailAddress))
            {
                e.Cancel = true;
                info.AddMessage("Errore nel formato della mail", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                return;
            }
            try
            {
                mailAccountService.Insert(mu);
                this.IdSender_ViewState = mu.UserId;
                BackendUserService buservice = new BackendUserService();
                _bUser = (BackendUser)buservice.GetByUserName(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                popolaGridElencoEmailsShared();
                info.AddMessage("Operazione effettuata", Com.Delta.Messaging.MapperMessages.LivelloMessaggio.OK);
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

        #region "Utenti"
        protected void btnAbilita_Click(object sender, EventArgs e)
        {
            bool isItemChecked = false;

            try
            {
                foreach (ListViewItem lvi in lvDipendentiNONAbilitati.Items)
                {
                    if ((lvi.FindControl("checkBoxUtenteNONAbilitati") as CheckBox).Checked)
                    {
                        isItemChecked = true;
                        decimal userId = Decimal.Parse((lvi.FindControl("lb_ID_UTENTE") as HiddenField).Value);
                        BackendUserService buservice = new BackendUserService();
                        buservice.InsertAbilitazioneEmail(userId, this.IdSender_ViewState, 0);
                    }
                }

                if (isItemChecked)
                {
                    popolaListaDipendentiNONAbilitati();
                    popolaListaDipendentiAbilitati();
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {                   
                    ManagedException mEx = new ManagedException(" Errore " + ex.Message, "CM023",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                 
                    log.Error(err);                  

                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(((ManagedException)ex).Azione, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
        }

        protected void btnDisabilita_Click(object sender, EventArgs e)
        {
            bool isItemChecked = false;

            try
            {
                foreach (ListViewItem lvi in lvDipendentiAbilitati.Items)
                {
                    if ((lvi.FindControl("checkBoxUtenteAbilitati") as CheckBox).Checked)
                    {
                        isItemChecked = true;
                        decimal userId = Decimal.Parse((lvi.FindControl("lb_ID_UTENTE") as HiddenField).Value);
                        int role = Int16.Parse((lvi.FindControl("lb_ROLE") as HiddenField).Value);

                        if (userId != this.UserIdBackendUser_ViewState && role <= this.RoleBackendUser_ViewState)
                        {
                            BackendUserService buservice = new BackendUserService();
                            buservice.RemoveAbilitazioneEmail(userId, this.IdSender_ViewState);
                        }
                    }
                }

                if (isItemChecked)
                {
                    try
                    {
                        popolaListaDipendentiNONAbilitati();
                    }
                    catch (Exception) { }

                    popolaListaDipendentiAbilitati();
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                  
                    ManagedException mEx = new ManagedException(" Errore " + ex.Message, "CM024",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                    
                    log.Error(err);  
                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(((ManagedException)ex).Azione, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
        }
        #endregion

        #region Amministratori

        protected void btnAbilitaAdmin_Click(object sender, EventArgs e)
        {
            bool isItemChecked = false;

            try
            {
                foreach (ListViewItem lvi in lvUtenti.Items)
                {
                    if ((lvi.FindControl("checkBoxUtenteAdminNONAbilitati") as CheckBox).Checked)
                    {
                        isItemChecked = true;
                        decimal userId = Decimal.Parse((lvi.FindControl("lb_ID_UTENTE_ADMIN") as HiddenField).Value);
                        BackendUserService buservice = new BackendUserService();
                        buservice.UpdateAbilitazioneEmail(userId, this.IdSender_ViewState, 1);
                    }
                }

                if (isItemChecked)
                {
                    popolaListaAmministratori();
                    popolaListaUtentiAbilitati();
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                   
                    ManagedException mEx = new ManagedException(" Errore " + ex.Message, "CM015",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                    
                    log.Error(err);                   
                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(((ManagedException)ex).Azione, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
        }

        protected void btnDisabilitaAdmin_Click(object sender, EventArgs e)
        {
            bool isItemChecked = false;

            try
            {
                foreach (ListViewItem lvi in lvUtentiAdmin.Items)
                {
                    if ((lvi.FindControl("checkBoxUtentiAdmin") as CheckBox).Checked)
                    {
                        isItemChecked = true;
                        decimal userId = Decimal.Parse((lvi.FindControl("lb_ID_UTENTE_ADMIN") as HiddenField).Value);
                        int role = Int16.Parse((lvi.FindControl("lb_ROLE_ADMIN") as HiddenField).Value);

                        if (userId != this.UserIdBackendUser_ViewState && role <= this.RoleBackendUser_ViewState)
                        {
                            BackendUserService buservice = new BackendUserService();
                            buservice.UpdateAbilitazioneEmail(userId, this.IdSender_ViewState, 0);
                        }
                    }
                }

                if (isItemChecked)
                {
                    try
                    {
                        popolaListaAmministratori();
                        popolaListaUtentiAbilitati();
                    }
                    catch (Exception) { }

                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                  
                    ManagedException mEx = new ManagedException(" Errore " + ex.Message, "CM016",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                   
                    log.Error(err);                    

                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(((ManagedException)ex).Azione, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
        }

        #endregion

        #region Cartelle

        protected void btnAbilitaFolder_Click(object sender, EventArgs e)
        {
            bool isItemChecked = false;

            try
            {
                foreach (ListViewItem lvi in lvCartelleNonAbilitate.Items) 
                {
                    if ((lvi.FindControl("checkBoxCartelleNonAbilitate") as CheckBox).Checked)
                    {
                        isItemChecked = true;
                        string[] listaIdStr = (lvi.FindControl("lb_NOME_CARTELLA") as HiddenField).Value.Split(';');
                        SendersFoldersService sfs = new SendersFoldersService();
                        sfs.InsertAbilitazioneFolder(Convert.ToInt32(listaIdStr[0]), Convert.ToInt32(listaIdStr[1]), Convert.ToInt32(listaIdStr[2]));
                        MailUser m = WebMailClientManager.getAccount();
                        if (m != null)
                        {
                            WebMailClientManager.AccountRemove();
                            MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                            MailUser account = facade.GetUserByUserId(m.UserId);
                            MailServerFacade serverFacade = MailServerFacade.GetInstance(account); 
                            account.Validated = true;
                            WebMailClientManager.SetAccount(account);
                        }
                    }
                }

                if (isItemChecked)
                {
                    popolaListaFoldersNonAbilitate(Convert.ToString(Session["MailRichiedente"]));
                    popolaListaFoldersAbilitate(Convert.ToString(Session["MailRichiedente"]));
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {                   
                    ManagedException mEx = new ManagedException(" Errore " + ex.Message, "CM017",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                  
                    log.Error(err);                   

                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(((ManagedException)ex).Azione, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }
        }

        protected void btnDisabilitaFolder_Click(object sender, EventArgs e)
        {
            bool isItemChecked = false;

            try
            {
                foreach (ListViewItem lvi in lvCartelleAbilitate.Items)
                {
                    if ((lvi.FindControl("checkBoxCartelleAbilitate") as CheckBox).Checked)
                    {
                        isItemChecked = true;
                        string[] listaIdStr = (lvi.FindControl("lb_NOME_CARTELLA_ABILITATA") as HiddenField).Value.Split(';');
                        SendersFoldersService sfs = new SendersFoldersService();
                        sfs.DeleteAbilitazioneFolder(Convert.ToInt32(listaIdStr[0]), Convert.ToInt32(listaIdStr[1]));
                        MailUser m = WebMailClientManager.getAccount();
                        if (m != null)
                        {
                            WebMailClientManager.AccountRemove();
                            MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                            MailUser account = facade.GetUserByUserId(m.UserId);                            
                            MailServerFacade serverFacade = MailServerFacade.GetInstance(account);
                            account.Validated = true;
                            WebMailClientManager.SetAccount(account);
                        }
                    }
                }

                if (isItemChecked)
                {
                    popolaListaFoldersNonAbilitate(Convert.ToString(Session["MailRichiedente"]));
                    popolaListaFoldersAbilitate(Convert.ToString(Session["MailRichiedente"]));
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {                   
                    ManagedException mEx = new ManagedException(" Errore " + ex.Message, "CM018",
                        string.Empty, string.Empty, ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    err.loggingAppCode = "WEB_MAIL";
                    err.objectID = this.Context.Session.SessionID;                    
                    log.Error(err);
                    

                    info.AddMessage(ex.Message, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
                else
                {
                    info.AddMessage(((ManagedException)ex).Azione, Com.Delta.Messaging.MapperMessages.LivelloMessaggio.ERROR);
                }
            }

        }

        public void popolaListaFoldersNonAbilitate(string eMail)
        {
            SendersFoldersService sfs = new SendersFoldersService();
            List<SendersFolders> listaCartelleNonAbilitate = sfs.GetFoldersNONAbilitati(eMail);
            lvCartelleNonAbilitate.DataSource = listaCartelleNonAbilitate;
            lvCartelleNonAbilitate.DataBind();

        }

        public void popolaListaFoldersAbilitate(string eMail)
        {
            SendersFoldersService sfs = new SendersFoldersService();
            List<SendersFolders> listaCartelleAbilitate = sfs.GetFoldersAbilitati(eMail);
            lvCartelleAbilitate.DataSource = listaCartelleAbilitate;
            lvCartelleAbilitate.DataBind();
        }

        protected void lvCartelleAbilitate_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem dataItem = (ListViewDataItem)e.Item;
                SendersFolders sFolder = (SendersFolders)dataItem.DataItem;
                if (sFolder.System == 1)
                {
                    (e.Item.FindControl("checkBoxCartelleAbilitate") as CheckBox).Enabled = false;
                }
                else
                {
                    (e.Item.FindControl("checkBoxCartelleAbilitate") as CheckBox).Enabled = true;
                }
            }
        }

        #endregion
    }
}

