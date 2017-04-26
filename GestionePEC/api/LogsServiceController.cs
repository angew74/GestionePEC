using ActiveUp.Net.Mail.DeltaExt;
using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging;
using Com.Delta.Logging.Context;
using Com.Delta.Logging.Errors;
using Com.Delta.Logging.Repository;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF.MailFacedes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using static GestionePEC.Models.MailModel;

namespace GestionePEC.api
{
    public class LogsServiceController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("LogsServiceController");
        [Authorize]
        [Route("api/LogsServiceController/getLogCodes")]
        public HttpResponseMessage getLogCodes()
        {
            LogCodesModel model = new LogCodesModel();
            try
            {
                LogCodesRepository logCon = new LogCodesRepository();
                var logscode = logCon.GetAll();
                if (logscode != null)
                {
                    model.ElencoLogCodes = logscode;
                    model.Totale = logscode.Count.ToString();
                }
                else
                {
                    model.Totale = "0";
                }
                model.success = "true";
            }
            catch (Exception ex)
            {
                model.success = "false";
                model.message = "Errore: " + ex.Message;
            }
            return this.Request.CreateResponse<LogCodesModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [Route("api/LogsServiceController/getAppCodes")]
        public HttpResponseMessage getAppCodes()
        {
            AppCodesModel model = new AppCodesModel();
            try
            {
                AppCodesRepository logApp = new AppCodesRepository();
                var appCodes= logApp.GetAll();
                if (appCodes != null)
                {
                    model.ElencoAppCodes = appCodes;
                    model.Totale = appCodes.Count.ToString();
                }
                else { model.Totale = "0"; }
                model.success = "true";
            }
            catch (Exception ex)
            {
                model.success = "false";
                model.message = "Errore: " + ex.Message;
            }
            return this.Request.CreateResponse<AppCodesModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [HttpGet]
        [Route("api/LogsServiceController/GetLogs")]
        public HttpResponseMessage GetLogs(string usermail, string user, string codiceapp, string codicelog, 
          string datainizio, string datafine,
          int? page, int? start, int? limit, string sort, string dir)
        {
            List<LOG_ACTIONS> list = new List<LOG_ACTIONS>();
            LogActionsModel m = new LogActionsModel();
            try
            {
                int starti = start.HasValue ? Convert.ToInt32(start) : 1;
                int recordPagina = limit.HasValue ? Convert.ToInt32(limit) : 10;
                DateTime d = DateTime.Now;
                DateTime dF = DateTime.Now;
                MailLogRepository mailLog = new MailLogRepository();
                if (!string.IsNullOrEmpty(datainizio))
                 { d = DateTime.Parse(datainizio); }
                else { d.AddMonths(-3); }
                if (!string.IsNullOrEmpty(datafine))
                { d = DateTime.Parse(datafine); }
                list = mailLog.GetVariables(user,codiceapp,codicelog, usermail,d,dF, starti, recordPagina);
                 m.LogsList = list;
                m.Totale = list.Count.ToString();
                m.success = "true";
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR611";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                m.message = ex.Message;
                m.success = "false";
                return this.Request.CreateResponse<LogActionsModel>(HttpStatusCode.OK, m);
            }
            return this.Request.CreateResponse<LogActionsModel>(HttpStatusCode.OK, m);

        }

        [Authorize]
        [HttpGet]
        [Route("api/LogsServiceController/getUsers")]
        public HttpResponseMessage getUsers()
        {
            List<IdentityUser> users = new List<IdentityUser>();
            UsersModel model = new UsersModel();
            UserStore store = new UserStore();
            try
            {
                users = store.GetAll().Result;
                if (users != null)
                {
                    users.Insert(0, new IdentityUser() { UserName = ""});
                    model.ListUtenti = users;
                    model.Totale = users.Count.ToString();
                }
                else
                {
                    model.Totale = "0";
                }
                model.success = "true";
            }
            catch (Exception ex)
            {
                model.success = "false";
                model.message = "Errore: " + ex.Message;
            }
            return this.Request.CreateResponse<UsersModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [HttpGet]
        [Route("api/LogsServiceController/getMails")]
        public HttpResponseMessage getMails()
        {           
            MailModel model = new MailModel();            
            try
            {
                string username = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
                MailServerConfigFacade mailSCF = null;
                mailSCF = MailServerConfigFacade.GetInstance();
                List<CasellaMail> c = new List<CasellaMail>();
                List<MailUser> l = SessionManager<List<MailUser>>.get(SessionKeys.ACCOUNTS_LIST);
                if (!(l != null && l.Count != 0))
                {
                    l = mailSCF.GetManagedAccountByUser(username).Distinct().ToList();                  
                   foreach (MailUser m in l)
                    {
                        CasellaMail casella = new CasellaMail()
                        {
                            emailAddress = m.Casella,
                            idMail = m.Id.ToString()
                        };
                        c.Add(casella);
                    }
                   
                }
                model.ElencoMails =c;
                model.Totale = c.Count.ToString();
            }
            catch (Exception ex)
            {
                model.success = "false";
                model.message = "Errore: " + ex.Message;
            }
            return this.Request.CreateResponse<MailModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [HttpGet]
        [Route("api/LogsServiceController/GetLogsError")]
        public HttpResponseMessage GetLogsError(string user, string codiceapp, string codicelog,
         string datainizio, string datafine,string details,
         int? page, int? start, int? limit, string sort, string dir)
        {
            List<LOG_APP_ERRORS> list = new List<LOG_APP_ERRORS>();
            LogErrorsModel m = new LogErrorsModel();
            try
            {
                int starti = start.HasValue ? Convert.ToInt32(start) : 1;
                int recordPagina = limit.HasValue ? Convert.ToInt32(limit) : 10;
                DateTime d = DateTime.Now;
                DateTime dF = DateTime.Now;
                ErrorMailLogRepository mailLog = new ErrorMailLogRepository();
                if (!string.IsNullOrEmpty(datainizio))
                { d = DateTime.Parse(datainizio); }
                else { d.AddMonths(-3); }
                if (!string.IsNullOrEmpty(datafine))
                { d = DateTime.Parse(datafine); }
                list = mailLog.GetVariables(user, codiceapp, codicelog, details, d, dF, starti, recordPagina);
                m.ErrorLogsList = list;
                m.Totale = list.Count.ToString();
                m.success = "true";

            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR611";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                m.message = ex.Message;
                m.success = "false";
                return this.Request.CreateResponse<LogErrorsModel>(HttpStatusCode.OK, m);
            }
            return this.Request.CreateResponse<LogErrorsModel>(HttpStatusCode.OK, m);

        }
    }
}