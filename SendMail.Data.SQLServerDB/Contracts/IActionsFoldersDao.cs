using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Contracts
{
   public interface IActionsFoldersDao : IDao<ActionFolder, Int64>
    {
    }
}
