using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using Com.Delta.Mail.MailMessage;
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
using System.Net.Http.Formatting;
using System.Web.Http;

namespace GestionePEC.api
{

    public class FoldersResponse
    {
        public string message { get; set; }
        public int total { get; set; }
        public string success { get; set; }
        public List<ActiveUp.Net.Common.DeltaExt.Action> ActionsList { get; set; }
    }


    public class FolderController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FolderController));
        [Authorize]
        [Route("api/FolderController/GetActions")]
        public HttpResponseMessage GetActions(string idFolder,string page, string start,string limit)
        {
            return createJsonActions(idFolder);
        }

        private HttpResponseMessage createJsonActions(string folder)
        {

            FoldersResponse folderResponse = new FoldersResponse();
            MailUser m = WebMailClientManager.getAccount();
            if (m == null)
            {
                folderResponse.message = "Sessione scaduta";
                folderResponse.success = "false";
                return  this.Request.CreateResponse<FoldersResponse>(HttpStatusCode.OK, folderResponse); 
            }
            bool managed = m.IsManaged;
            if (managed)
            {
                List<Folder> Folders = m.Folders;
                int idfolder = 0;
                int.TryParse(folder, out idfolder);
                if (Folders != null && Folders.Count > 0 && idfolder > 0)
                {
                    var results = (from f in Folders
                                   where f.Id == idfolder
                                   select f.Azioni).ToList();
                    folderResponse.ActionsList = results[0];
                    //List<ActiveUp.Net.Common.DeltaExt.Action> actions =(List<ActiveUp.Net.Common.DeltaExt.Action>) Folders.Where(f => f.TipoFolder.ToUpper() == tipoFolder && f.Id == idfolder).ToList().First().Azioni;
                    //folderResponse.data = new Actions(actions);  
                    folderResponse.success = "true";
                    folderResponse.total = folderResponse.ActionsList.Count;
                }
                else
                {
                    folderResponse.message = "Nessuna azione";
                    folderResponse.success = "false";
                }
            }
            return this.Request.CreateResponse<FoldersResponse>(HttpStatusCode.OK, folderResponse);
        }

        [Authorize]
        [Route("api/FolderController/GetFoldersNonAbilitate")]
        public HttpResponseMessage GetFoldersNonAbilitate(string email)
        {
            SendersFoldersService sfs = new SendersFoldersService();
            FoldersSendersModel model = new FoldersSendersModel();
            try
            {
                List<SendersFolders> listaCartelleNonAbilitate = sfs.GetFoldersNONAbilitati(email);
               // model.FoldersList = listaCartelleNonAbilitate.ToArray();
                model.Totale = listaCartelleNonAbilitate.Count.ToString();
                model.success = "true";
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Eccezione nella lettura delle cartelle", "ERR_F005", string.Empty, ex.Message, null);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = email;
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

        [Authorize]
        [HttpPost]
        [Route("api/FolderController/AbilitaFolders/{email}")]
        public HttpResponseMessage AbilitaFolders(int idemail, FormDataCollection collection)
        {
           
            FoldersSendersModel model = new FoldersSendersModel();
            try
            {

                // model.FoldersList = listaCartelleNonAbilitate.ToArray();
                SendersFoldersService sfs = new SendersFoldersService();
               // sfs.InsertAbilitazioneFolder(Convert.ToInt32(listaIdStr[0]), Convert.ToInt32(listaIdStr[1]), 0);
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
                    ManagedException mEx = new ManagedException("Eccezione nella lettura delle cartelle", "ERR_F005", string.Empty, ex.Message, null);
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

        [Authorize]
        [Route("api/FolderController/GetFoldersAbilitate")]
        public HttpResponseMessage GetFoldersAbilitate(string email)
        {
            SendersFoldersService sfs = new SendersFoldersService();
            List<SendersFolders> listaCartelleAbilitate = new List<SendersFolders>();
            List<FolderType> types = new List<FolderType>();
            FoldersSendersModel model = new FoldersSendersModel();
            try
            {
                listaCartelleAbilitate = sfs.GetFoldersAbilitati(email);
                foreach (SendersFolders f in listaCartelleAbilitate)
                {
                    FolderType type = new FolderType()
                    {
                        IdNome = f.IdNome,
                        Nome = f.Nome
                    };
                    types.Add(type);
                }
                model.FoldersList = types.ToArray();
                model.Totale = types.Count.ToString();
                model.success = "true";
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Eccezione nella lettura delle cartelle", "ERR_F006", string.Empty, ex.Message, null);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = email;
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

        [Authorize]
        [Route("api/FolderController/GetAllFolders")]
        public HttpResponseMessage GetAllFolders()
        {
            SendersFoldersService sfs = new SendersFoldersService();
            FoldersSendersModel model = new FoldersSendersModel();
            List<SendersFolders> listaCartelle = new List<SendersFolders>();
            List<FolderType> types = new List<FolderType>();
            try
            {
                 listaCartelle = sfs.GetAll();
                 foreach(SendersFolders f in listaCartelle)
                {
                    FolderType type = new FolderType()
                    {
                        IdNome = f.IdNome,
                        Nome = f.Nome
                    };
                    types.Add(type);
                }
                 model.FoldersList = types.ToArray();
                 model.Totale = types.Count.ToString();
                 model.success = "true";
            }
            catch (Exception ex)
            {
                if (!ex.GetType().Equals(typeof(ManagedException)))
                {
                    ManagedException mEx = new ManagedException("Eccezione nella lettura delle cartelle", "ERR_F006", string.Empty, ex.Message, null);
                    ErrorLogInfo er = new ErrorLogInfo(mEx);
                    er.objectID = null;
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
    }
}