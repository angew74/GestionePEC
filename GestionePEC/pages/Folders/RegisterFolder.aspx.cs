using ActiveUp.Net.Common.DeltaExt;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using GestionePEC.Extensions;
using SendMail.BusinessEF.Operations;
using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GestionePEC.pages.Folders
{
    public partial class RegisterFolder : BasePage
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RegisterFolder));
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSalva_Click(object sender, EventArgs e)
        {
            if(NomeFolder.Text.Trim() != string.Empty)
            {
                FoldersService foldersservice = new FoldersService();
                Folder f= foldersservice.GetByName(NomeFolder.Text.Trim());
                if(f != null && f.Id != 0)
                {
                    info.AddError("Folder già presente in archivio");
                    return;
                }
               List<decimal> ids = CreateFolders();
                CreateActionsForFolders(ids[0],ids[1], ids[2], ids[3], NomeFolder.Text.Trim(), ids[4].ToString());
            }
            else
            {
                info.AddError("Inserire nome folder");
                return;
            }
            info.AddInfo("Cartella inserita");
        }

        private List<decimal> CreateFolders()
        {
            List<decimal> ids = new List<decimal>();
            FoldersService foldersservice = new FoldersService();
            Folder folder = new Folder();
            folder.Nome = NomeFolder.Text.Trim();
            folder.TipoFolder = "I";
            foldersservice.Insert(folder);
            ids.Add(folder.Id);
            Folder folderOut = new Folder();
            folderOut.Nome = folder.Nome;
            folderOut.IdNome = folder.IdNome;
            folderOut.TipoFolder = "O";
            foldersservice.Insert(folderOut);
            ids.Add(folderOut.Id);
            Folder folderOutArch = new Folder();
            folderOutArch.Nome = folder.Nome;
            folderOutArch.IdNome = folder.IdNome;
            folderOutArch.TipoFolder = "D";
            foldersservice.Insert(folderOutArch);
            ids.Add(folderOutArch.Id);
            Folder folderInArch = new Folder();
            folderInArch.Nome = folder.Nome;
            folderInArch.IdNome = folder.IdNome;
            folderInArch.TipoFolder = "E";
            foldersservice.Insert(folderInArch);
            ids.Add(folderOutArch.Id);
            ids.Add(decimal.Parse(folder.IdNome));
            return ids;
        }

        private void CreateActionsForFolders(decimal idIn, decimal idOut, decimal idInArch, decimal idOutArch, string nome, string idNome)
        {
            List<ActiveUp.Net.Common.DeltaExt.Action> actions = new List<ActiveUp.Net.Common.DeltaExt.Action>();
            ActiveUp.Net.Common.DeltaExt.Action actionin = new ActiveUp.Net.Common.DeltaExt.Action();
            actionin.IdDestinazione = decimal.Parse(idNome);
            actionin.IdFolderDestinazione =(int) idIn;
            actionin.NomeAzione = "SPOSTA IN " + nome;
            actionin.TipoAzione = "SP";            
            actions.Add(actionin);
            ActiveUp.Net.Common.DeltaExt.Action actionout = new ActiveUp.Net.Common.DeltaExt.Action();
            actionout.IdDestinazione = decimal.Parse(idNome);
            actionout.IdFolderDestinazione = (int) idOut;
            actionout.NomeAzione = "SPOSTA IN "+ nome;
            actionout.TipoAzione = "SP";
            actions.Add(actionout);
            // archivio 
            ActiveUp.Net.Common.DeltaExt.Action actioninArch = new ActiveUp.Net.Common.DeltaExt.Action();
            actioninArch.IdDestinazione = decimal.Parse(idNome);
            actioninArch.IdFolderDestinazione = (int)idInArch;
            actioninArch.NomeAzione = "SPOSTA IN " + nome;
            actioninArch.TipoAzione = "SP";
            actions.Add(actioninArch);
            ActiveUp.Net.Common.DeltaExt.Action actionoutArch = new ActiveUp.Net.Common.DeltaExt.Action();
            actionoutArch.IdDestinazione = decimal.Parse(idNome);
            actionoutArch.IdFolderDestinazione = (int)idOutArch;
            actionoutArch.NomeAzione = "SPOSTA IN " + nome;
            actionoutArch.TipoAzione = "SP";
            actions.Add(actionoutArch);
            ActionsService actService = new ActionsService();
            try
            {
                actService.Insert(actions);
                List<ActionFolder> actionsFolders = new List<ActionFolder>();
                // inserimento azioni e collegamento con folders
                ActionFolder afInbox = new ActionFolder();
                afInbox.idAction = actionin.Id;
                afInbox.idFolder = 1;
                actionsFolders.Add(afInbox);
                ActionFolder afNewFolder = new ActionFolder();
                afNewFolder.idAction = 4;
                afNewFolder.idFolder = idIn;
                actionsFolders.Add(afNewFolder);
                ActionFolder afOutBox = new ActionFolder();
                afOutBox.idAction = actionout.Id;
                afOutBox.idFolder = 2;
                actionsFolders.Add(afOutBox);
                ActionFolder afNewFolderOut = new ActionFolder();
                afNewFolderOut.idAction = 6;
                afNewFolderOut.idFolder = idOut;
                actionsFolders.Add(afNewFolderOut);
                // ARCHIVI 
                ActionFolder afInboxArch = new ActionFolder();
                afInboxArch.idAction = actioninArch.Id;
                afInboxArch.idFolder = 5;
                actionsFolders.Add(afInboxArch);
                ActionFolder afNewFolderArch = new ActionFolder();
                afNewFolderArch.idAction = 4;
                afNewFolderArch.idFolder = idInArch;
                actionsFolders.Add(afNewFolderArch);
                ActionFolder afOutBoxArch = new ActionFolder();
                afOutBoxArch.idAction = actionoutArch.Id;
                afOutBoxArch.idFolder = 7;
                actionsFolders.Add(afOutBoxArch);
                ActionFolder afNewFolderOutArch = new ActionFolder();
                afNewFolderOutArch.idAction = 6;
                afNewFolderOutArch.idFolder = idOutArch;
                actionsFolders.Add(afNewFolderOutArch);
                ActionsFoldersService service = new ActionsFoldersService();
                service.Insert(actionsFolders);

            }
            catch(Exception ex)
            {
                ManagedException mEx = new ManagedException(ex.Message, "FOL_APP001", string.Empty, string.Empty, ex);
                ErrorLogInfo er = new ErrorLogInfo(mEx);
                log.Error(er);
                info.AddError("Attenzione azioni non inserite");
                return;
            }

        }
    }
}