using ActiveUp.Net.Common.DeltaExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.BusinessEF.Contracts
{
   public interface IFoldersService
    {
        void Insert(ActiveUp.Net.Common.DeltaExt.Folder action);
        void Delete(long id);
       
        void Update(ActiveUp.Net.Common.DeltaExt.Folder action);
        Folder GetById(long id);

        ICollection<ActiveUp.Net.Common.DeltaExt.Folder> GetAll();
    }
}
