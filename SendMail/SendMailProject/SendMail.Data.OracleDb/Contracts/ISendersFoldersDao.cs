using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.DataContracts.Interfaces
{
    public interface ISendersFoldersDao : IDao<SendersFolders, Int64>
    {

        List<SendersFolders> GetFoldersNONAbilitati(string mail);
        
        List<SendersFolders> GetFoldersAbilitati(string mail);

        int InsertAbilitazioneFolder(int idNome, int idSender, int system);

        void DeleteAbilitazioneFolder(int idNome, int idSender);

    }
}
