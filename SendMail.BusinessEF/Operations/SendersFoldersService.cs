using System.Collections.Generic;
using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
    class SendersFoldersService : BaseSingletonService<SendersFoldersService>, ISendersFoldersService
    {

        #region ISendersFoldersService Membri di

        List<SendersFolders> ISendersFoldersService.GetFoldersNONAbilitati(string mail)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return dao.GetFoldersNONAbilitati(mail);
            }
        }

        List<SendersFolders> ISendersFoldersService.GetFoldersAbilitati(string mail)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return dao.GetFoldersAbilitati(mail);
            }
        }

        public int InsertAbilitazioneFolder(int idNome, int idSender, int system)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return dao.InsertAbilitazioneFolder(idNome, idSender, system);
            }
        }

        public void DeleteAbilitazioneFolder(int idNome, int idSender)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                dao.DeleteAbilitazioneFolder(idNome, idSender);
            }
        }

        #endregion
    }
}
