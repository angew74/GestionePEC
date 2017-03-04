using ActiveUp.Net.Common.DeltaExt;
using ActiveUp.Net.Mail.DeltaExt;
using Com.Delta.Mail.MailMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace GestionePEC.Asmx
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FoldersService : IHttpHandler, IRequiresSessionState
    {
        public class FoldersResponse
        {
            public string message { get; set; }
            public int total { get; set; }
            public string success { get; set; }
            public List<ActiveUp.Net.Common.DeltaExt.Action> ActionsList { get; set; }
        }


        [WebGet(UriTemplate = "folders/folder/{idFolder}",
        RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        FoldersResponse GetActions(string idFolder)
        {
            return createJsonActions(idFolder);
        }

        public void ProcessRequest(HttpContext context)
        {
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private FoldersResponse createJsonActions(string folder)
        {

            FoldersResponse folderResponse = new FoldersResponse();
            MailUser m = WebMailClientManager.getAccount();
            if (m == null)
            {
                folderResponse.message = "Sessione scaduta";
                folderResponse.success = "false";
                return folderResponse;
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
            return folderResponse;
        }
    }
}
