using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Business.Contracts;
using SendMail.Business.Base;
using SendMail.Data.Contracts;

namespace SendMail.Business
{
    public class ContactsBackendService : BaseSingletonService<ContactsBackendService>,
                                          IContactsBackendService
    {
        #region IContactsBackendService Membri di

        public SendMail.Model.ContactApplicationMapping.ContactsBackendMap FindByIdMap(int idMap)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                return dao.GetById(idMap);
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindAll()
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                return dao.GetAll();
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindPerEntita(int idEntita)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                return dao.GetPerEntita(idEntita);
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindPerTitoloEntita(int idTitolo, int IdEntita)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                return dao.GetPerTitoloEntita(idTitolo, IdEntita);
            }
        }

        public ICollection<SendMail.Model.ContactApplicationMapping.ContactsBackendMap> FindPerCanaleTitoloEntita(int idCanale, int idTitolo, int idEntita)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                return dao.GetPerCanaleTitoloEntita(idCanale, idTitolo, idEntita);
            }
        }

        public void CreateMapping(SendMail.Model.ContactApplicationMapping.ContactsBackendMap map)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                dao.Insert(map);
            }
        }

        public void UpdateMapping(SendMail.Model.ContactApplicationMapping.ContactsBackendMap map)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                dao.Update(map);
            }
        }

        public void DeleteMapping(int idMap)
        {
            using (IContactsBackendDao dao = getDaoContext().DaoImpl.ContactsBackendDao)
            {
                dao.Delete(idMap);
            }
        }

        #endregion
    }
}
