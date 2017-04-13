using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SendMail.Model;
using SendMail.Model.ContactApplicationMapping;
using SendMail.Business.Data.QueryModel;

namespace SendMail.Data.SQLServerDB
{
    public interface IContactsApplicationMappingDao : IDao<ContactsApplicationMapping, Int64>
    {
        ICollection<ContactsApplicationMapping> GetContactsByCriteria(QueryCmpNew q);

        ICollection<ContactsApplicationMapping> FindByBackendCodeAndCodComunicazione(IEnumerable<string> codes, string codCom);
        void setDefaultContact(long idTitolo, long refOrg, long idContatto);

        ICollection<ContactsApplicationMapping> GetContactsByIdContact(long idContact);
    }
}
