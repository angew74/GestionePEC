using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging.Errors;
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
    public class RolesServiceController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RolesServiceController));

        [HttpGet]
        [Authorize]
        [Route("api/RolesServiceController/GetAllRoles")]
        public HttpResponseMessage GetAllRoles()
        {
            RolesModel m = new RolesModel();
            try
            {

                var roleStore = new RoleStore();
                var roles = roleStore.GetAll().Result;
                m.RolesList = roles;
                m.Totale = roles.Count.ToString();
                m.success = "true";
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_R001";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                m.message = ex.Message;
                m.success = "false";
                return this.Request.CreateResponse<RolesModel>(HttpStatusCode.InternalServerError, m);
            }
            return this.Request.CreateResponse<RolesModel>(HttpStatusCode.OK, m);
        }
        
}
}
