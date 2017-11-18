using Com.Delta.Logging.Errors;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace GestionePEC.api
{
    public class StatServiceController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("StatServiceController");
        [Authorize]
        [Route("api/StatServiceController/getStatByUserMail")]
        public HttpResponseMessage getStatByUserMail(string utente, string mail, 
          string dt, string df,
          int? page, int? start, int? limit, string sort, string dir)
        {
            StatModel model = new StatModel();
            try
            {
                
                BackendUserService bus = new BackendUserService();
                DateTime datainzio = DateTime.Parse(dt);
                DateTime datafine = DateTime.Parse(df);
                int starti = start.HasValue ? Convert.ToInt32(start) : 1;
                int recordPagina = limit.HasValue ? Convert.ToInt32(limit) : 10;
                int tot = 0;
                List<UserResultItem> list = bus.GetStatsInBox(mail, utente, datainzio, datafine, starti, recordPagina, ref tot);
                model.Totale= tot.ToString();
                model.success = "true";
                model.ElencoStat = list;
                return this.Request.CreateResponse<StatModel>(HttpStatusCode.OK, model);

            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message + " " + "Parameteri: " + utente + " " + mail + " " + dt + " " + df;
                error.logCode = "ERR_S01";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                model.message = ex.Message;
                model.success = "false";
                return this.Request.CreateResponse<StatModel>(HttpStatusCode.OK, model);

            }
        }
    }
}