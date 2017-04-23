using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendMail.Model;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF.Operations
{
    public class ActionsFoldersService : BaseSingletonService<ActionsFoldersService>, IActionsFoldersService
    {
        public void Insert(List<ActionFolder> af)
        {
            using (ActionsFoldersSQLDb dao = new ActionsFoldersSQLDb())
            {
                dao.Insert(af);
            }
        }
    }

}
