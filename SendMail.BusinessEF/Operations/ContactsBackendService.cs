using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.BusinessEF.Contracts;
using SendMail.BusinessEF.Base;
using SendMail.Data.Contracts;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF
{
    public class ContactsBackendService : BaseSingletonService<ContactsBackendService>,
                                          IContactsBackendService
    {
        #region IContactsBackendService Membri di

        public SendMail.Model.ContactApplicationMapping.ContactsBackendMap FindByIdMap(int idMap)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                return dao.GetById(idMap);
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindAll()
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                return dao.GetAll();
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindPerEntita(int idEntita)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                return dao.GetPerEntita(idEntita);
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindPerTitoloEntita(int idTitolo, int IdEntita)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                return dao.GetPerTitoloEntita(idTitolo, IdEntita);
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindPerCanaleTitoloEntita(int idCanale, int idTitolo, int idEntita)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                return dao.GetPerCanaleTitoloEntita(idCanale, idTitolo, idEntita);
            }
        }

        public void CreateMapping(SendMail.Model.ContactApplicationMapping.ContactsBackendMap map)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                dao.Insert(map);
            }
        }

        public void UpdateMapping(SendMail.Model.ContactApplicationMapping.ContactsBackendMap map)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                dao.Update(map);
            }
        }

        public void DeleteMapping(int idMap)
        {
            using (ContactsBackEndSQLDb dao = new ContactsBackEndSQLDb())
            {
                dao.Delete(idMap);
            }
        }

        #endregion
    }
}
