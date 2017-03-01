using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SendMail.Model;

namespace SendMail.Business.Contracts
{
    public interface IContactsMappingService
    {
        ICollection<ContactsApplicationMapping> FindByDatiGenerici(ContactsApplicationMapping contact);
        ICollection<ContactsApplicationMapping> FindByDatiGenerici(ICollection<ContactsApplicationMapping> contacts);
        ICollection<ContactsApplicationMapping> FindByBackendCodeAndCodComunicazione(IEnumerable<string> codes, string codCom);
        ICollection<ContactsApplicationMapping> FindByIdContact(long idContact);
        void SetContattoAsDefault(long idTitolo, long idEntita, long idContatto);
    }
}
