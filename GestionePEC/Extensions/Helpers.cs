using System.Data;
using System.Linq;
using System.Collections.Generic;
using Com.Delta.Web.Cache;
using SendMail.Locator;
using ActiveUp.Net.Common.DeltaExt;

namespace GestionePEC.Extensions
{
    public class Helpers
    {

        public static string GetTipo(string folderid)
        {
            string tipo = string.Empty;
            string azione = string.Empty;
            List<Folder> getFolders = CacheManager<List<Folder>>.get(CacheKeys.FOLDERS, VincoloType.NONE);
            if (getFolders == null)
            {
                getFolders = ServiceLocator.GetServiceFactory().MailLocalService.getAllFolders();
            }
            int id = 0;
            int.TryParse(folderid, out id);
            var idnome = (from f in getFolders
                          where f.Id == id
                          select f.IdNome).FirstOrDefault();
            if (idnome == "3")
            { return "R"; }
            var t = (from a in getFolders
                     where a.Id == id 
                     select a.TipoFolder);            
            tipo = t.FirstOrDefault();
            return tipo;

        }
        public static string GetAzione(string ActionId)
        {
            string azione = string.Empty;
            List<ActiveUp.Net.Common.DeltaExt.Action> getActions = ServiceLocator.GetServiceFactory().MailLocalService.GetFolderDestinationForAction();
            int id = 0;
            int.TryParse(ActionId, out id);
            var t = (from a in getActions
                     where a.Id == id
                     select a.NomeAzione);
            azione = t.First();
            return azione;
        }
        public static bool CanDo(string parentFolder, string mailAction, List<string> mailIds)
        {
            bool ret = false;
            if ((parentFolder == "O" || parentFolder == "D" || mailAction != "0" || parentFolder == "AO" || parentFolder == "I") && mailIds.Count > 0 && mailIds[0] != string.Empty)
            { return true; }
            else
            { return false; }
        }
    }
}
