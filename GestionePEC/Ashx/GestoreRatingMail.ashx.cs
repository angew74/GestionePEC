using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace GestionePEC.Ashx
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GestoreRatingMail : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        ILog _log = LogManager.GetLogger(typeof(GestoreRatingMail));

        public void ProcessRequest(HttpContext context)
        {
            bool response = false;

            string idMail = context.Request["idMail"];
            string account = context.Request["account"];
            int rating = -1;

            if (!String.IsNullOrEmpty(idMail) && !String.IsNullOrEmpty(account))
            {
                int risp = 0;
                if (int.TryParse(context.Request["rating"], out rating) &&
                    (rating >= 0 && rating <= 4))
                {
                    try
                    {
                        risp = SendMail.Locator.ServiceLocator.GetServiceFactory().getMailServerFacade(WebMailClientManager.getAccount()).AssignMessageRating(idMail, rating);
                    }
                    catch (Exception e)
                    {
                        //TASK: Allineamento log - Ciro
                        if (e.GetType() != typeof(ManagedException))
                        {
                            ManagedException mEx = new ManagedException("Detagli errore: " + e.Message, "ERR_RAT", "", "", e);
                            ErrorLogInfo err = new ErrorLogInfo(mEx);
                            err.objectID = idMail;
                            _log.Error(err);
                        }
                        //ErrorLog err = new ErrorLog("ERR_RAT", e, "", "", "");
                        //_log.Error(err);
                    }
                }

                if (risp == 1) response = true;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(response.ToString().ToLower());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}