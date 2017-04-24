using Com.Delta.Logging;
using Com.Delta.Logging.Context;
using Com.Delta.Logging.Errors;
using Com.Delta.Logging.Repository;
using GestionePEC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

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
                model.ElencoLogCodes = logCon.GetAll();
                model.Totale = model.ElencoLogCodes.Count;
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
                model.ElencoAppCodes = logApp.GetAll();
                model.Totale = model.ElencoAppCodes.Count;
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