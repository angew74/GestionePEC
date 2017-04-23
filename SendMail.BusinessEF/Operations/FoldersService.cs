using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF.Operations
{
    public class FoldersService : BaseSingletonService<FoldersService>, IFoldersService
    {
        public void Delete(long id)
        {
            using (FolderSQLDb dao = new FolderSQLDb())
            {
                dao.Delete(id);
            }
        }

        public ICollection<ActiveUp.Net.Common.DeltaExt.Folder> GetAll()
        {
            using (FolderSQLDb dao = new FolderSQLDb())
            {
               return dao.GetAll();
            }
        }

        public Folder GetById(long id)
        {
            using (FolderSQLDb dao = new FolderSQLDb())
            {
                return dao.GetById(id);
            }
        }

        public Folder GetByName(string name)
        {
            using (FolderSQLDb dao = new FolderSQLDb())
            {
                return dao.GetFolderByName(name);
            }
        }

        public void Insert(ActiveUp.Net.Common.DeltaExt.Folder folder)
        {

            using (FolderSQLDb dao = new FolderSQLDb())
            {
                dao.Insert(folder);
            }
        }

        public void Update(ActiveUp.Net.Common.DeltaExt.Folder folder)
        {

            using (FolderSQLDb dao = new FolderSQLDb())
            {
                dao.Update(folder);
            }
        }
    }
}
