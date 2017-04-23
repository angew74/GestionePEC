using SendMail.BusinessEF.Base;
using SendMail.BusinessEF.Contracts;
using SendMail.BusinessEF.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveUp.Net.Common.DeltaExt;
using SendMail.Data.SQLServerDB.Repository;

namespace SendMail.BusinessEF.Operations
{
    public class ActionsService : BaseSingletonService<ActionsService>, IActionsService
    {
        public void Delete(long id)
        {
            using (ActionsSQLDb dao = new ActionsSQLDb())
            {
                dao.Delete(id);
            }
        }

        public ICollection<ActiveUp.Net.Common.DeltaExt.Action> GetAll()
        {
            using (ActionsSQLDb dao = new ActionsSQLDb())
            {
               return dao.GetAll();
            }
        }

        public ActiveUp.Net.Common.DeltaExt.Action GetById(long id)
        {
            using (ActionsSQLDb dao = new ActionsSQLDb())
            {
              return dao.GetById(id);
            }
        }

        public void Insert(List<ActiveUp.Net.Common.DeltaExt.Action> actions)
        {
            using (ActionsSQLDb dao = new ActionsSQLDb())
            {
                dao.Insert(actions);
            }
        }

        public void Insert(ActiveUp.Net.Common.DeltaExt.Action action)
        {
            using (ActionsSQLDb dao = new ActionsSQLDb())
            {
                dao.Insert(action);
            }
        }

        public void Update(ActiveUp.Net.Common.DeltaExt.Action action)
        {
            using (ActionsSQLDb dao = new ActionsSQLDb())
            {
                dao.Update(action);
            }
        }
    }
}
