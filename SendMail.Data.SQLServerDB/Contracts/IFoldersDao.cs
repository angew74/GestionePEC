
using ActiveUp.Net.Common.DeltaExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Contracts
{
   public interface IFoldersDao : IDao<Folder, Int64>
    {
    }
}
