using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Http;
using Com.Delta.Web.Cache;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using GestionePEC.Routing;
using log4net;
using Com.Delta.Security;
using SendMail.BusinessEF.MailFacedes;
using Com.Delta.Web.Session;
using GestionePEC.Models;

namespace GestionePEC
{
    public class Global : HttpApplication
    {

        static readonly ILog log = LogManager.GetLogger("Global");
        void Application_Start(object sender, EventArgs e)
        {
            // Codice eseguito all'avvio dell'applicazione
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //inizializza log4net
            log4net.Config.XmlConfigurator.Configure();    
            //inizializza il routing
            RegisterRoutes(RouteTable.Routes);
            MailLocalService mailLocalService = new MailLocalService();
            CacheManager<List<ActiveUp.Net.Common.DeltaExt.Action>>.set(CacheKeys.FOLDERS_ACTIONS, mailLocalService.GetFolderDestinationForAction());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = null;
            if (Server.GetLastError() != null)
                ex = Server.GetLastError();
                     
            if (ex != null)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    string msg = "Application_Error. Dettaglio: " + ex.Message;
                    msg += " -- \n Stack: " + ((ex.StackTrace != null) ? ex.StackTrace : " vuoto. ");
                    msg += " -- \n InnerException: " + ((ex.InnerException != null) ? ex.InnerException.Message : " vuoto. ");

                    ManagedException mEx = new ManagedException(msg,
                        "ERR999",
                        string.Empty,
                        string.Empty,
                        ex);
                    mEx.addEnanchedInfosTag("ERROR_MSG", msg);
                    ErrorLogInfo error = new ErrorLogInfo(mEx);
                    log.Error(error);
                }
            }            
            Server.ClearError();
            if(SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
           { SessionManager<Dictionary<string, DTOFileUploadResult>>.del(SessionKeys.DTO_FILE); }
            var contextWrapper = new HttpContextWrapper(HttpContext.Current);
            var virtualPath = RouteTable.Routes.GetVirtualPath(new RequestContext(contextWrapper, new RouteData()),
                "ErrorPage", null);
            if (!Response.IsRequestBeingRedirected)
            { Response.Redirect("~/pages/pubblica/ErrorPage.aspx"); }
            Context.Response.End();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if ((HttpContext.Current.User.Identity.IsAuthenticated))
                {
                    //MyPrincipal upro = (MyPrincipal)HttpRuntime.Cache.Get(System.Web.HttpContext.Current.User.Identity.Name);
                    //if ((HttpRuntime.Cache.Get(System.Web.HttpContext.Current.User.Identity.Name) == null))
                    //{

                    //    // distruggo l'account corrente 
                    //    System.Web.HttpContext.Current.User = null;

                    //    // distruggo il ticket di autenticazione 
                    //    System.Web.Security.FormsAuthentication.SignOut();

                    //    // abbandono la sessione 
                    //    if (((System.Web.HttpContext.Current.Session != null)))
                    //    {
                    //        System.Web.HttpContext.Current.Session.Abandon();
                    //    }
                    //    Response.Redirect(((Global)sender).Context.Request.ApplicationPath + "/logoff.aspx?ForceClose=2");
                    //}
                    //else
                    //{
                    FormsIdentity id = HttpContext.Current.User.Identity as FormsIdentity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    FormsAuthentication.RenewTicketIfOld(ticket);
                    HttpContext.Current.User = (MyPrincipal)System.Web.HttpContext.Current.Cache.Get(System.Web.HttpContext.Current.User.Identity.Name);
                    //}
                }
                else
                {
                    if (HttpContext.Current.User != null)
                    {
                        log.Debug(HttpContext.Current.User.Identity.Name);
                    }
                }
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
        protected void Application_End(object sender, EventArgs e)
        {
            LogManager.GetRepository().Shutdown();
        }
        protected void RegisterRoutes(RouteCollection routes)
        {
            routes.Add(new Route("{resource}.axd/{*pathinfo}",
                new StopRoutingHandler()));

            routes.Add("LoginPage",
                new Route("login",
                    new WebFormRouteHandler("~/login.aspx")
                )
            );
            routes.Add("LogOffPage",
                new Route("logoff",
                    new WebFormRouteHandler("~/LogOff.aspx")
                )
            );
            routes.Add("LogOffFromBrowserPage",
                new Route("logoff/browser",
                    new WebFormRouteHandler("~/LogOffFromBrowser.aspx")
                )
            );           
            //routes.Add("NewMailSenderPage",
            //    new Route("mail-sender",
            //        new WebFormRouteHandler("~/pages/mail/NewMailSender.aspx")
            //    )
            //);
            routes.Add("StatisticaPage",
               new Route("statistica",
                   new WebFormRouteHandler("~/pages/MailClient/MailBoxStat.aspx")
               )
           );
            //routes.Add("COECViewerPage",
            //    new Route("coec-viewer/{TipRic}/{stringaId}",
            //        new WebFormRouteHandler("~/pages/mail/COECViewer.aspx")
            //    )
            //);
            routes.Add("GestRubricaPage",
                new Route("gestione-rubrica/{ID}/{IDTITOLO}",
                    new WebFormRouteHandler("~/pages/Mail/GestRubrica.aspx")
                )
            );
            routes.Add("MailInboxPage",
                new Route("casella-mail",
                    new WebFormRouteHandler("~/pages/MailClient/MailBoxInBox.aspx")
                )
            );
            routes.Add("MailViewerId",
                new Route("mailviewerid/{IDMAIL}/{DIM}/{FOLDERID}/{RATING}/{PARENTFOLDER}/{ACCOUNTID}",
                    new WebFormRouteHandler("~/pages/MailClient/MailViewerId.aspx")
                )
            );
            routes.Add("MailSearch",
               new Route("ricerca-mail",
                   new WebFormRouteHandler("~/pages/MailClient/MailSearch.aspx")
               )
           );
            routes.Add("MailMove",
               new Route("movimentazioni-mail",
                   new WebFormRouteHandler("~/pages/Inbox/MailMove.aspx")
               )
           );
            routes.Add("MailAdminPage",
                new Route("gestione-mail",
                    new WebFormRouteHandler("~/pages/Inbox/MailAdmin.aspx")
                )
            );
            routes.Add("EntiResearchPage",
                new Route("gestione-contatti",
                    new WebFormRouteHandler("~/pages/Rubrica/EntiResearch.aspx")
                )
            );
            routes.Add("ErrorPage",
                new Route("error",
                    new WebFormRouteHandler("~/pages/pubblica/ErrorPage.aspx")
                )
            );
            routes.Add("MaintenancePage",
                new Route("maintenance",
                    new WebFormRouteHandler("~/Maintenance.aspx")
                )
            );
            //routes.Add("ManualeUtentePage",
            //    new Route("manuale-utente",
            //        new WebFormRouteHandler("~/pages/common/ManualeUtente.aspx")
            //    )
            //);
            routes.Add("Default",
                new Route("",
                    new WebFormRouteHandler("~/pages/common/Default.aspx")
                )
            );
        }
    }
}