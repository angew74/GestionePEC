using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Logging.Mail;
using Com.Delta.Security;
using GestionePEC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace GestionePEC.api
{
    public class LoginServiceController : ApiController
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(LoginServiceController));
        private int CACHEEXPIRATION = int.Parse(ConfigurationManager.AppSettings["CACHEXPIRATION"]);


        [HttpGet]
        [Route("api/LoginServiceController/dologin/{username}/{password}")]
        [AllowAnonymous]
        public HttpResponseMessage DoLogin(string username, string password)
        {
            LoginModel loginModel = new LoginModel();
            try
            {
                string user = username.Trim().ToUpper();
                string pw = password.Trim();
                MyPrincipal upro = null;
                bool found = false;
                MyIdentity identity = null;
                if ((HttpContext.Current.Cache[user] != null))
                {
                    upro = (MyPrincipal)HttpContext.Current.Cache.Get(user);
                    identity = (MyIdentity)upro.Identity;
                    HttpContext.Current.User = upro;
                    found = true;
                }

                if (!found)
                {
                    try
                    {
                        upro = MySecurityProvider.BuildNewIdentity(user, "", pw, "Form").Result;
                    }
                    catch (System.Exception ex)
                    {
                        ErrorLogInfo error = new ErrorLogInfo();
                        error.freeTextDetails = ex.Message;
                        error.logCode = "ERR111";
                        error.loggingAppCode = "SCA";
                        error.loggingTime = System.DateTime.Now;
                        error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                        _log.Error(error);
                        loginModel.Error = ex.Message;
                        loginModel.success = "false";
                        return this.Request.CreateResponse<LoginModel>(HttpStatusCode.InternalServerError, loginModel);
                    }
                }

                //se l'utente ha fornito username e password corretta
                if (upro != null && (((MyIdentity)upro.Identity).checkIdentity(user, pw)))
                {
                    //se l'utente non era in cache carico il profilo utente
                    if (!found)
                    {
                        //upro = MySecurityProvider.BuildPrincipal(identity, "0");
                        HttpContext.Current.Cache.Add(user, upro, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(CACHEEXPIRATION), System.Web.Caching.CacheItemPriority.AboveNormal, null);
                    }
                    //a questo punto ho riunito le due strade

                    //controllo se l'utente è già loggato
                    //if (upro.isLoggedIn) errorlabel.Text = "Accesso impossibile.<br /><br /><b>ATTENZIONE: Account già in uso!!</b>";
                    if (false) { }
                    else
                    {

                        MailLogInfo logInfo = new MailLogInfo();
                        logInfo.logCode = "LON";
                        logInfo.loggingAppCode = "MAIL";
                        logInfo.loggingTime = System.DateTime.Now;
                        logInfo.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                        logInfo.userID = user;
                        logInfo.freeTextDetails = string.Empty;
                        _log.Info(logInfo);
                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(user, false, 15);
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
                        HttpContext.Current.Response.Cookies.Add(cookie);
                        //Response.Cookies.Add(cookie);
                        // upro.isLoggedIn = true;
                        HttpContext.Current.User = upro;
                        HttpContext.Current.Cache.Insert(user, upro, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(CACHEEXPIRATION), System.Web.Caching.CacheItemPriority.AboveNormal, null);
                        loginModel.success = "true";
                        loginModel.ResponseUrl = "pages/Common/Default.aspx";
                    }

                }
                // hanno provato ad inserie uno username giusto ma una password sbagliata(grave)
                else
                {
                    loginModel.success = "false";
                    loginModel.Error = "Attenzione! Credenziali di accesso errate";
                    return this.Request.CreateResponse<LoginModel>(HttpStatusCode.BadRequest, loginModel);
                }
            }
            catch (System.Exception e0)
            {
                loginModel.Error = e0.Message;
                loginModel.success = "false";
                return this.Request.CreateResponse<LoginModel>(HttpStatusCode.InternalServerError, loginModel);
            }

            return this.Request.CreateResponse<LoginModel>(HttpStatusCode.OK, loginModel);
        }
    }
}
