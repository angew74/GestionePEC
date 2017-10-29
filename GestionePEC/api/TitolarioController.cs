using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Web.Cache;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GestionePEC.api
{
    public class TitolarioController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TitolarioController));
        [Authorize]
        [Route("api/TitolarioController/GetTitoli")]
        public HttpResponseMessage GetTitoli(int? page, int? start, int? limit)
        {
            TitolarioService<SendMail.Model.Titolo> ts = new TitolarioService<SendMail.Model.Titolo>();
            TitoliModel model = new TitoliModel();
            List<SendMail.Model.Titolo> list = new List<SendMail.Model.Titolo>();
            try
            {
                if (CacheManager<List<SendMail.Model.Titolo>>.exist(CacheKeys.ALL_TITLES))
                {
                    list = CacheManager<List<SendMail.Model.Titolo>>.get(CacheKeys.ALL_TITLES, VincoloType.BACKEND);
                }
                else
                {
                    list =(List<SendMail.Model.Titolo>) ts.GetAll(null);
                }

                model.success = "true";
                model.TitoliList = list.ToList();
                model.Totale = list.Count.ToString();
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ErrorLogInfo error = new ErrorLogInfo();
                    error.freeTextDetails = ex.Message;
                    error.logCode = "ERR_T001";
                    error.loggingAppCode = "PEC";
                    error.loggingTime = System.DateTime.Now;
                    error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                    log.Error(error);
                    model.message = ex.Message;
                    model.success = "false";
                }
                else
                {
                    model.message = ex.Message;
                    model.success = "false";
                }
                return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [Route("api/TitolarioController/GetSottoTitoli")]
        public HttpResponseMessage GetSottoTitoli(string idtitolo, int? page, int? start, int? limit)
        {
            TitolarioService<SendMail.Model.SottoTitolo> ts = new TitolarioService<SendMail.Model.SottoTitolo>();
            TitoliModel model = new TitoliModel();
            IList<SendMail.Model.SottoTitolo> sottotitoli = new List<SendMail.Model.SottoTitolo>();
            try
            {
                if (CacheManager<List<SendMail.Model.Titolo>>.exist(CacheKeys.ALL_SUBTITLES))
                {
                    sottotitoli = CacheManager<List<SendMail.Model.SottoTitolo>>.get(CacheKeys.ALL_SUBTITLES, VincoloType.BACKEND);
                }
                else
                {
                    sottotitoli = (List<SendMail.Model.SottoTitolo>)ts.FindByTitolo(int.Parse(idtitolo));
                }

                model.success = "true";
                model.SottoTitoliList = sottotitoli.ToList();
                model.Totale = sottotitoli.Count.ToString();
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ErrorLogInfo error = new ErrorLogInfo();
                    error.freeTextDetails = ex.Message;
                    error.logCode = "ERR_T002";
                    error.loggingAppCode = "PEC";
                    error.loggingTime = System.DateTime.Now;
                    error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                    log.Error(error);
                    model.message = ex.Message;
                    model.success = "false";
                }
                else
                {
                    model.message = ex.Message;
                    model.success = "false";
                }
                return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<TitoliModel>(HttpStatusCode.OK, model);
        }
    }
}
