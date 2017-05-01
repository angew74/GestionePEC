using System.Collections.Generic;
using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.Model;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
  public  class SendersFoldersService : BaseSingletonService<SendersFoldersService>, ISendersFoldersService
    {

        #region ISendersFoldersService Membri di

       public List<SendersFolders> GetFoldersNONAbilitati(string mail)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return dao.GetFoldersNONAbilitati(mail);
            }
        }

        public List<SendersFolders> GetAll()
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return (List<SendersFolders>) dao.GetAll();
            }
        }

        public List<SendersFolders> GetFoldersAbilitati(string mail)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return dao.GetFoldersAbilitati(mail);
            }
        }

        public List<SendersFolders> GetFoldersAbilitatiByIdSender(int idsender)
        {
            using (SendersFoldersSQLDb dao = new SendersFoldersSQLDb())
            {
                return dao.GetFoldersAbilitatiByIdSender(idsender);
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
