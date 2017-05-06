using AspNet.Identity.SQLServerProvider;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using GestionePEC.Models;
using log4net;
using SendMail.BusinessEF;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using static GestionePEC.Models.UsersModel;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using SendMail.BusinessEF.MailFacedes;
using System.Net.Http.Formatting;
using Com.Delta.Security;

namespace GestionePEC.api
{
    public class UsersServiceController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger("UsersServiceController");

        [HttpGet]
        [Authorize]
        [Route("api/UsersServiceController/GetUsersAbilitati")]
        public HttpResponseMessage GetUsersAbilitati(int idSender)
        {
            BackendUserService bus = new BackendUserService();
            UsersMailModel model = new UsersMailModel();
            try
            {
                List<BackendUser> listaUtentiAbilitati = bus.GetDipendentiDipartimentoAbilitati(idSender);
                List<UserMail> list = new List<UserMail>();
                foreach(BackendUser b in listaUtentiAbilitati)
                {
                    UserMail u = new UserMail()
                    {
                        UserId =(int) b.UserId,
                        UserName = b.UserName
                    };
                    list.Add(u);
                }
                model.success = "true";
                model.UsersList = list.ToArray();
                model.Totale = list.Count.ToString();
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ErrorLogInfo error = new ErrorLogInfo();
                    error.freeTextDetails = ex.Message;
                    error.logCode = "ERR_U001";
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
                return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
        }

        [Authorize]
        [HttpGet]
        [Route("api/UsersServiceController/AbilitaUsers")]
        public HttpResponseMessage AbilitaUsers(string idemail, string itemselector)
        {

            FoldersSendersModel model = new FoldersSendersModel();
            BackendUserService bus = new BackendUserService();
            if (string.IsNullOrEmpty(idemail) && string.IsNullOrEmpty(itemselector))
            {
                model.success = "false";
                model.message = "Valori insufficienti nella richiesta";
                this.Request.CreateResponse<FoldersSendersModel>(HttpStatusCode.OK, model);
            }
            try
            {

                // model.FoldersList = listaCartelleNonAbilitate.ToArray();
                string[] users = itemselector.Split(';');
                var usersAbilitati = bus.GetDipendentiDipartimentoAbilitati(int.Parse(idemail)).Select(z => z.UserId).ToArray();
                var ff = Array.ConvertAll(usersAbilitati, x => x.ToString());
                var usersda = users.Except(ff);
                var usersa = ff.Except(users);
                foreach (string s in usersda)
                {
                   bus.InsertAbilitazioneEmail(Convert.ToInt32(s), Convert.ToInt32(idemail), 0);
                }
                foreach (string s in usersa)
                {                  
                    { bus.RemoveAbilitazioneEmail(Convert.ToInt32(s), Convert.ToInt32(idemail)); }
                }
                MailUser m = WebMailClientManager.getAccount();
                if (m != null)
                {
                    WebMailClientManager.AccountRemove();
                    MailServerConfigFacade facade = MailServerConfigFacade.GetInstance();
                    MailUser account = facade.GetUserByUserId(m.UserId);
                    MailServerFacade serverFacade = MailServerFacade.GetInstance(account);
                    account.Validated = true;
                    WebMailClientManager.SetAccount(account);
                }
                model.success = "true";
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Eccezione nellA ABILITAZIONE UTENTI", "ERR_U005", string.Empty, ex.Message, null);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = idemail.ToString();
                    log.Error(er);
                    model.success = "false";
                    model.message = mEx.Message;
                }
                else
                {
                    model.success = "false";
                    model.message = ex.Message;
                }
            }
            return this.Request.CreateResponse<FoldersSendersModel>(HttpStatusCode.OK, model);

        }

        [HttpGet]
        [Authorize]
        [Route("api/UsersServiceController/GetAdminsAbilitati")]
        public HttpResponseMessage GetAdminsAbilitati(int idSender)
        {
            BackendUserService bus = new BackendUserService();
            UsersMailModel model = new UsersMailModel();
            try
            {
                List<BackendUser> listaUtentiAbilitati = bus.GetDipendentiDipartimentoAbilitati(idSender);
                var ListaAdmins = listaUtentiAbilitati.Where(x => x.RoleMail > 0);
                List<UserMail> list = new List<UserMail>();
                foreach (BackendUser b in ListaAdmins)
                {
                    UserMail u = new UserMail()
                    {
                        UserId = (int)b.UserId,
                        UserName = b.UserName
                    };
                    list.Add(u);
                }
                model.success = "true";
                model.UsersList = list.ToArray();
                model.Totale = list.Count.ToString();
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ErrorLogInfo error = new ErrorLogInfo();
                    error.freeTextDetails = ex.Message;
                    error.logCode = "ERR_U002";
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
                return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
        }


        [HttpGet]
        [Authorize]
        [Route("api/UsersServiceController/GetAllUsers")]
        public HttpResponseMessage GetAllUsers()
        {
            BackendUserService bus = new BackendUserService();
            UsersMailModel model = new UsersMailModel();
            try
            {
                List<BackendUser> listaUtenti = bus.GetAllUsers();
                List<UserMail> list = new List<UserMail>();
                foreach (BackendUser b in listaUtenti)
                {
                    UserMail u = new UserMail()
                    {
                        UserId = (int)b.UserId,
                        UserName = b.UserName
                    };
                    list.Add(u);
                }
                model.success = "true";
                model.UsersList = list.ToArray();
                model.Totale = list.Count.ToString();
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ErrorLogInfo error = new ErrorLogInfo();
                    error.freeTextDetails = ex.Message;
                    error.logCode = "ERR_U003";
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
                return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<UsersMailModel>(HttpStatusCode.OK, model);
        }

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
                    if (roles.Count > 0)
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
                    var identityrole = roleStore.FindByIdAsync(role).Result;
                    if (identityrole != null)
                    {
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
                                    Role = identityrole.Name
                                };
                                users.Add(user);
                            }
                        }
                        m.UtentiList = users;
                        m.Totale = users.Count.ToString();
                        m.success = "true";
                    }
                }
                else
                {
                    m.Totale = "0";
                    m.success = "true";
                }
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_U004";
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
        public HttpResponseMessage RemoveRole(string userid, string role)
        {
            UsersModel m = new UsersModel();
            try
            {
                if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(role))
                {
                    UserStore userStore = new UserStore();
                    var useri = userStore.FindByIdAsync(userid).Result;                  
                    userStore.RemoveFromRoleAsync(useri, role);                   
                    m.success = "true";
                }
                else
                {
                    m.message = "utente o ruolo non presente";
                    m.success = "true";
                }
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_U006";
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
        [Route("api/UsersServiceController/DeleteUser")]
        public HttpResponseMessage DeleteUser(string userid)
        {
            UsersModel m = new UsersModel();
            try
            {
                if (!string.IsNullOrEmpty(userid))
                {
                    UserStore userStore = new UserStore();
                    var useri = userStore.FindByIdAsync(userid).Result;
                    RoleStore roleStore = new RoleStore();
                    List<IdentityRole> roles = roleStore.GetRolesByUserId(int.Parse(useri.Id)).Result;
                    foreach (IdentityRole r in roles)
                    {
                        userStore.RemoveFromRoleAsync(useri, r.Id);
                    }
                    m.success = "true";
                }
                else
                {
                    m.message = "utente o ruolo non presente";
                    m.success = "true";
                }
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_U006";
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


        [HttpPost]
        [Authorize]
        [Route("api/UsersServiceController/RegisterUser")]
        public HttpResponseMessage RegisterUser(FormDataCollection formsValues)
        {
            var userStore = new UserStore();
            UsersModel model = new UsersModel();
            var userName = formsValues["UserName"];
            var password = formsValues["Password"];
            var cognome = formsValues["Cognome"];
            var nome = formsValues["Nome"];
            var role = formsValues["Role"];
            var codicefiscale = formsValues["CodiceFiscale"];
            var user = new IdentityUser() { UserName = userName.ToUpper() };
            user.PasswordHash = MySecurityProvider.PlainToSHA256(password);
            user.SecurityStamp = System.DateTime.Now.Ticks.ToString();
            try
            {
                string result = userStore.CreateAsync(user).Result;
                user.Id = userStore.FindByNameAsync(userName.ToUpper()).Result.Id;
                if (result == "OK")
                {
                    BackendUserService bus = new BackendUserService();
                    BackendUser userBackend = new BackendUser();
                    userBackend.Cognome = cognome.Trim().ToUpper();
                    userBackend.Nome = nome.Trim().ToUpper();
                    userBackend.UserName = userName.Trim().ToUpper();
                    userBackend.CodiceFiscale = codicefiscale.Trim().ToUpper();
                    userBackend.Domain = role.ToUpper();
                    bus.Save(userBackend);
                    model.success = "true";
                }
                else
                {
                    model.success = "false";
                    model.message = "Utente non creato";
                }
                var resultRole = (userStore.AddToRoleAsync(user, int.Parse(role.ToUpper()))).Result;
                if (resultRole != 1)
                {
                    model.success = "false";
                    model.message = string.Format("Utente {0} non aggiunto a ruolo {1} è stato correttamente creato!", user.UserName, role);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(ManagedException))
                {
                    ManagedException mEx = new ManagedException("Errore creazione utente. Dettaglio: " + ex.Message +
                        "StackTrace: " + ((ex.StackTrace != null) ? ex.StackTrace.ToString() : " vuoto "),
                        "ERR317",
                        string.Empty,
                        string.Empty,
                        ex.InnerException);
                    ErrorLogInfo err = new ErrorLogInfo(mEx);
                    log.Error(err);
                    model.success = "false";
                    model.message = string.Format("Utente {0} non correttamente creato", user.UserName);                   

                }
                else
                {
                    model.success = "false";
                    model.message = "Utene non creato";                   
                }
                return this.Request.CreateResponse<UsersModel>(HttpStatusCode.OK, model);
            }
            return this.Request.CreateResponse<UsersModel>(HttpStatusCode.OK, model);
        }

    }
}