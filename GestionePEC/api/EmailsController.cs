using Com.Delta.Logging.Errors;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GestionePEC.api
{
    public class EmailsController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("EmailsController");
        [Authorize]
        [HttpGet]
        [Route("api/EmailsController/GetMails")]
        public HttpResponseMessage GetMails(int? page, int? start, int? limit, string sort, string dir)
        {
            BackendUser _bUser;
            BackendUserModel model = new BackendUserModel();
            try
            {
                int starti = start.HasValue ? Convert.ToInt32(start) : 1;
                int recordPagina = limit.HasValue ? Convert.ToInt32(limit) : 5;
                if (!(SessionManager<BackendUser>.exist(SessionKeys.BACKEND_USER)))
                {
                    BackendUserService buservice = new BackendUserService();
                    _bUser = (BackendUser)buservice.GetByUserName(MySecurityProvider.CurrentPrincipal.MyIdentity.UserName);
                    SessionManager<BackendUser>.set(SessionKeys.BACKEND_USER, _bUser);
                }
                else { _bUser = SessionManager<BackendUser>.get(SessionKeys.BACKEND_USER); }
                List<BackEndUserMailUserMapping> listMailSender = null;
                if (_bUser != null)
                {
                    listMailSender = _bUser.MappedMails;
                    if (listMailSender != null && listMailSender.Count > 0)
                    {
                        listMailSender = listMailSender.OrderBy(ms => ms.EmailAddress).ToList();
                        model.Totale = listMailSender.Count.ToString();
                        model.ListBackendUsers = listMailSender.Skip(starti).Take(recordPagina).ToList();
                    }
                }
                else
                {
                    model.Totale = "0";
                    model.success = "true";
                }

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
                model.message = ex.Message;
                model.success = "false";
                return this.Request.CreateResponse<BackendUserModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<BackendUserModel>(HttpStatusCode.OK, model);

        }
    }
}
