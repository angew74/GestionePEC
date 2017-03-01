using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.Business.Contracts
{
    public interface ISendersFoldersService
    {
        List<SendersFolders> GetFoldersNONAbilitati(string mail);
        List<SendersFolders> GetFoldersAbilitati(string mail);
        int InsertAbilitazioneFolder(int idNome, int idSender, int system);
        void DeleteAbilitazioneFolder(int idNome, int idSender);
    }
}
