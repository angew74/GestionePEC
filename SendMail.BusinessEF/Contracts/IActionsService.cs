using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.BusinessEF.Contracts
{
   public interface IActionsService
    {
        void Insert(ActiveUp.Net.Common.DeltaExt.Action action);
        void Delete(long id);

        void Insert(List<ActiveUp.Net.Common.DeltaExt.Action> actions);
        void Update(ActiveUp.Net.Common.DeltaExt.Action action);
        ActiveUp.Net.Common.DeltaExt.Action GetById(long id);

        ICollection<ActiveUp.Net.Common.DeltaExt.Action> GetAll();
    }
}
