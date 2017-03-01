using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Base;
using SendMail.Business.Contracts;
using SendMail.Model;
using SendMail.DataContracts.Interfaces;

namespace SendMail.Business
{
    class SendersFoldersService : BaseSingletonService<SendersFoldersService>, ISendersFoldersService
    {

        #region ISendersFoldersService Membri di

        List<SendersFolders> ISendersFoldersService.GetFoldersNONAbilitati(string mail)
        {
            using (ISendersFoldersDao dao = this.getDaoContext().DaoImpl.SendersFoldersDao)
            {
                return dao.GetFoldersNONAbilitati(mail);
            }
        }

        List<SendersFolders> ISendersFoldersService.GetFoldersAbilitati(string mail)
        {
            using (ISendersFoldersDao dao = this.getDaoContext().DaoImpl.SendersFoldersDao)
            {
                return dao.GetFoldersAbilitati(mail);
            }
        }

        public int InsertAbilitazioneFolder(int idNome, int idSender, int system)
        {
            using (ISendersFoldersDao dao = this.getDaoContext().DaoImpl.SendersFoldersDao)
            {
                return dao.InsertAbilitazioneFolder(idNome, idSender, system);
            }
        }

        public void DeleteAbilitazioneFolder(int idNome, int idSender)
        {
            using (ISendersFoldersDao dao = this.getDaoContext().DaoImpl.SendersFoldersDao)
            {
                dao.DeleteAbilitazioneFolder(idNome, idSender);
            }
        }

        #endregion
    }
}
