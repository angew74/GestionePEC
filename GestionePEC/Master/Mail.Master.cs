using GestionePEC.Extensions;
using GestionePEC.pages.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.Master
{
    public partial class Mail : MasterPage, IBaseMaster
    {
        string UserId = HttpContext.Current.User.Identity.Name;
        Com.Delta.Security.MyPrincipal _user = Com.Delta.Security.MySecurityProvider.CurrentPrincipal;

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Title
        {
            set
            {
                this.hfCenterTitle.Value = value;
            }
        }

        public ScriptManager ScriptManager
        {
            get { return ScriptManager; }
        }

        protected string PostBackStr;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Mail));

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.User == null || HttpContext.Current.User.Identity.IsAuthenticated == false)
            {
                this.FindControl("upMain").Visible = false;
            }
            PostBackStr = Page.ClientScript.GetPostBackEventReference(this, "MyCustomArgument");
            string eventArg = HttpContext.Current.Request["__EVENTARGUMENT"];
            if (eventArg == "ForzaLogoff")
            {
                ForzaLogoff();
            }
            string msg = ConfigurationManager.AppSettings["headerMessage"];           
            //end

            if (_user != null)
            {
                if (LoginView1 != null)
                {
                    hfUser.Value = _user.Identity.Name;
                    hfDipUser.Value = _user.MyIdentity.dipartimento;
                    (LoginView1.FindControl("lblCurrentUser") as Label).Text = hfUser.Value.ToString().ToUpper();
                    MailSiteMap.SiteMapProvider = "SiteMapMail";
                    MailSiteMap.DataBind();
                }
            }
            else
            {
                hfUser.Value = string.Empty;
                hfDipUser.Value = string.Empty;                       
            }

            string script = string.Empty;

            if (Page is IAccessPage)
            {
                script = "InitViewPort(true, false);";
            }
            else
            {
                if (Page is IHomePage)
                {
                    script = "InitViewPort(false, false);";
                    pnlCenter.CssClass = "homeBody";
                }
                else
                {
                    script = "InitViewPort(false, true);";
                }
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "script_InsMsgHeader", script, true);           
            if (IsPostBack)
                return;


            if (_user != null && _user.Identity != null && _user.Identity.IsAuthenticated)
            {
                string sSede = string.Empty;              
               
            }
            
        }

        public void ShowMessageList(string msg, Boolean show)
        {
            litErrore.Text = string.Empty;
            litMsgErrore.Text = msg;
            if (show)
            {
                pnlContainerMsg.Visible = show;
                litErrore.Text = "Messaggi";
            }
            else
                pnlContainerMsg.Visible = show;

            upMessaggi.Update();
        }


        protected void ForzaLogoff()
        {
            Response.Redirect(LogOffPath);
        }

        protected string LogOffPath
        {
            get
            {
                var contextWrapper = new HttpContextWrapper(HttpContext.Current);
                RequestContext requestContext;
                if (this.Page is BasePage)
                    requestContext = ((BasePage)this.Page).RequestContext;
                else
                    requestContext = new RequestContext(contextWrapper, new RouteData());
                var path = RouteTable.Routes.GetVirtualPath(requestContext, "LogOffPage", null);
                return path.VirtualPath;
            }
        }

        protected string LogOffFromBrowserPath
        {
            get
            {
                var contextWrapper = new HttpContextWrapper(HttpContext.Current);
                RequestContext requestContext;
                if (this.Page is BasePage)
                    requestContext = ((BasePage)this.Page).RequestContext;
                else
                    requestContext = new RequestContext(contextWrapper, new RouteData());
                var path = RouteTable.Routes.GetVirtualPath(requestContext, "LogOffFromBrowserPage", null);
                return path.VirtualPath;
            }
        }
    }
}