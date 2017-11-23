using Com.Delta.Logging.Errors;
using GestionePEC.Extensions;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        
        [Authorize]
        [Route("api/StatServiceController/getStampaByUserMail")]
        public HttpResponseMessage getStampaByUserMail(string utente, string mail,
        string dt, string df)
        {
            StatModel model = new StatModel();
            try
            {

                BackendUserService bus = new BackendUserService();
                DateTime datainzio = DateTime.Parse(dt);
                DateTime datafine = DateTime.Parse(df);
                int starti = 1;
                int recordPagina = 100;
                int tot = 0;
                List<UserResultItem> list = bus.GetStatsInBox(mail, utente, datainzio, datafine, starti, recordPagina, ref tot);
                if (list.Count > 0)
                {
                    byte[] response = null;
                    response = PrintWrapperITEXT.StampaStat(list);
                    MemoryStream output = new MemoryStream();
                    output.Write(response, 0, response.Length);
                    output.Position = 0;
                    HttpResponseMessage result = this.Request.CreateResponse(HttpStatusCode.OK);
                    result.Content = new StreamContent(output);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = "StatisticheLavorazioni.pdf";
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentLength = output.Length;
                    return result;
                }
                else
                {
                    model.Totale = "0";
                    model.success = "false";
                    model.message = "Nessuna ritrovamento per la stampa";
                    return this.Request.CreateResponse<StatModel>(HttpStatusCode.OK, model);
                }

            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message + " " + "Parameteri: " + utente + " " + mail + " " + dt + " " + df;
                error.logCode = "ERR_S02";
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