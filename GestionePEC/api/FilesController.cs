using Com.Delta.Web.Session;
using GestionePEC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GestionePEC.api
{
    public class FilesController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("FilesController");
        [Authorize]
        [HttpGet]
        [Route("api/FilesController/RemoveFile")]
        public HttpResponseMessage RemoveFile(string filename)
        {
            GenericResponseModel model = new GenericResponseModel();
            Dictionary<string, DTOFileUploadResult> dict = new Dictionary<string, DTOFileUploadResult>();
            if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
            {
                dict = SessionManager<Dictionary<string, DTOFileUploadResult>>.get(SessionKeys.DTO_FILE);
            }
            dict.Remove(filename);
            SessionManager<Dictionary<string, DTOFileUploadResult>>.set(SessionKeys.DTO_FILE, dict);
            model.success = "true";
            return this.Request.CreateResponse<GenericResponseModel>(HttpStatusCode.OK, model);
        }
        [Authorize]
        [HttpGet]
        [Route("api/FilesController/RemoveAllFiles")]
        public HttpResponseMessage RemoveAllFiles()
        {
            GenericResponseModel model = new GenericResponseModel();
            Dictionary<string, DTOFileUploadResult> dict = new Dictionary<string, DTOFileUploadResult>();
            if (SessionManager<Dictionary<string, DTOFileUploadResult>>.exist(SessionKeys.DTO_FILE))
            { SessionManager<Dictionary<string, DTOFileUploadResult>>.del(SessionKeys.DTO_FILE); }
            model.success = "true";
            return this.Request.CreateResponse<GenericResponseModel>(HttpStatusCode.OK, model);
        }
    }
}