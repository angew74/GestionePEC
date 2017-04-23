using SendMail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.BusinessEF.Contracts
{
   public interface IActionsFoldersService
    {
        void Insert(List<ActionFolder> af);
    }
}
