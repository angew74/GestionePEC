using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Security;
using Com.Delta.Web.Session;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using SendMail.BusinessEF.MailFacedes;
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
                error.logCode = "ERR_E001";
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

        [Authorize]
        [HttpGet]
        [Route("api/EmailsController/GetMailSendersByUserMails")]
        public HttpResponseMessage GetMailSendersByUserMails(int? page, int? start, int? limit)
        {
            MailServerConfigFacade mailServerConfigFacade = MailServerConfigFacade.GetInstance();
            List<MailUser> users = new List<MailUser>();
            UsersMailModel model = new UsersMailModel();
            try
            {

                string username = MySecurityProvider.CurrentPrincipal.MyIdentity.UserName;
                MailServerConfigFacade mailSCF = null;
                mailSCF = MailServerConfigFacade.GetInstance();
                users = SessionManager<List<MailUser>>.get(SessionKeys.ACCOUNTS_LIST);
                if (!(users != null && users.Count != 0))
                {
                    users = mailSCF.GetManagedAccountByUser(username).ToList();
                    if (users == null) users = new List<MailUser>();
                    if (users.Where(x => x.UserId.Equals(-1)).Count() == 0)
                        users.Insert(0, new MailUser() { UserId = -1, EmailAddress = "" });
                    SessionManager<List<MailUser>>.set(SessionKeys.ACCOUNTS_LIST, users);
                }
                model.MailUsers = users;
            }
            catch (ManagedException bex)
            {
                if (bex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException(bex.Message, "ERR_E002", string.Empty, string.Empty, bex);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    log.Error(er);
                    model.success = "false";
                    model.message = bex.Message;
                    return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
                }
            }
            return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
        }

    }
}
