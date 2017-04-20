using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging.Errors;
using GestionePEC.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static GestionePEC.Models.UsersModel;

namespace GestionePEC.api
{
    public class UsersServiceController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("UsersServiceController");

        [HttpGet]
        [Authorize]
        [Route("api/UsersServiceController/GetUtenti")]
        public HttpResponseMessage GetUtenti(string username, string role,
        int? page, int? start, int? limit)
        {
            UsersModel m = new UsersModel();
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    UserStore userStore = new UserStore();
                    var useri = userStore.FindByNameAsync(username).Result;
                    var roleStore = new RoleStore();
                   var roles = roleStore.GetRolesByUserName(username).Result;
                    List<UserRoles> users = new List<UserRoles>();
                    if(roles.Count > 0)
                    {
                        foreach (string rolefind in roles)
                        {
                            UserRoles user = new UserRoles()
                            {
                                Id = useri.Id.ToString(),
                                UserName = useri.UserName,
                                Role = rolefind
                            };
                            users.Add(user);
                        }
                    }
                    m.UtentiList = users;
                    m.Totale = users.Count.ToString();
                    m.success = "true";
                }
                if (!string.IsNullOrEmpty(role))
                {
                    RoleStore roleStore = new RoleStore();
                    var identityrole = roleStore.FindByNameAsync(role).Result;
                    UserStore userStore = new UserStore();
                    List<IdentityUser> _users = userStore.FindUsersByRole(identityrole.Id).Result;
                    List<UserRoles> users = new List<UserRoles>();
                    if (_users.Count > 0)
                    {
                        foreach (IdentityUser s in _users)
                        {
                            UserRoles user = new UserRoles()
                            {
                                Id = s.Id,
                                UserName = s.UserName,
                                Role = role
                            };
                            users.Add(user);
                        }
                    }
                    m.UtentiList = users;
                    m.Totale = users.Count.ToString();
                    m.success = "true";
                }
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR6456";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                m.message = ex.Message;
                m.success = "false";
                return this.Request.CreateResponse<UsersModel>(HttpStatusCode.InternalServerError, m);
            }
            return this.Request.CreateResponse<UsersModel>(HttpStatusCode.OK, m);
        }

        [HttpGet]
        [Authorize]
        [Route("api/UsersServiceController/RemoveRole")]
        public HttpResponseMessage RemoveRole(string userid, string roleid)  
        {
            UsersModel m = new UsersModel();
            try
            {
                if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(roleid))
                {
                    UserStore userStore = new UserStore();
                    var useri = userStore.FindByIdAsync(userid).Result;
                    userStore.RemoveFromRoleAsync(useri, roleid);
                    m.message = "Ruolo rimosso";
                    m.success = "true";
                }
                else
                {
                    m.message = "utente o ruolo non presente";
                    m.success = "false";
                }
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR6456";
                error.loggingAppCode = "PEC";
                error.loggingTime = System.DateTime.Now;
                error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                log.Error(error);
                m.message = ex.Message;
                m.success = "false";
                return this.Request.CreateResponse<UsersModel>(HttpStatusCode.InternalServerError, m);
            }
            return this.Request.CreateResponse<UsersModel>(HttpStatusCode.OK, m);
        }
}