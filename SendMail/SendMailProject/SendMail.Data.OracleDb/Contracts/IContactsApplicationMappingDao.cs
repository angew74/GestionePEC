using System;
using System.Collections.Generic;

using SendMail.Model;
using SendMail.Business.Data.QueryModel;
using Com.Delta.Data.QueryModel;

namespace SendMail.DataContracts.Interfaces
{
    public interface IContactsApplicationMappingDao : IDao<ContactsApplicationMapping, Int64>
    {
        ICollection<ContactsApplicationMapping> GetContactsByCriteria(QueryCmp q);

        ICollection<ContactsApplicationMapping> FindByBackendCodeAndCodComunicazione(IEnumerable<string> codes, string codCom);
        void setDefaultContact(long idTitolo, long refOrg, long idContatto);

        ICollection<ContactsApplicationMapping> GetContactsByIdContact(long idContact);
    }
}
