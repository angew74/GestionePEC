using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    }
}