using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.ContactApplicationMapping;

namespace SendMail.BusinessEF.Contracts
{
    public interface IContactsBackendService
    {
        ContactsBackendMap FindByIdMap(int idMap);
        ICollection<ContactsBackendMap> FindAll();
        ICollection<ContactsBackendMap> FindPerEntita(int idEntita);
        ICollection<ContactsBackendMap> FindPerTitoloEntita(int idTitolo, int IdEntita);       
        ICollection<ContactsBackendMap> FindPerCanaleTitoloEntita(int idCanale, int idTitolo, int idEntita);

        void CreateMapping(ContactsBackendMap map);
        void UpdateMapping(ContactsBackendMap map);
        void DeleteMapping(int idMap);
    }
}
