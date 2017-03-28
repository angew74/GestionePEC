using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.SQLServerDB;
using SendMail.Model.ContactApplicationMapping;

namespace SendMail.Data.Contracts
{
    public interface IContactsBackendDao : IDao<ContactsBackendMap, int>
    {
        ICollection<ContactsBackendMap> GetPerEntita(int idEntita);
        ICollection<ContactsBackendMap> GetPerTitoloEntita(int idTitolo, int idEntita);
        ICollection<ContactsBackendMap> GetPerCanaleTitoloEntita(int idCanale, int idTitolo, int idEntita);
    }
}
