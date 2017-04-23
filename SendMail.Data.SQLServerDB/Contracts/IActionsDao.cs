using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB.Contracts
{
   public interface IActionsDao : IDao<ActiveUp.Net.Common.DeltaExt.Action, Int64>
    {
    }
}
