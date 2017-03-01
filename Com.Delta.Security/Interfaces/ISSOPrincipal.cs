using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Delta.Security.Interfaces
{
    public interface ISSOPrincipal : System.Security.Principal.IPrincipal
    {
        TimeSpan ServerTimeOut { get; set; }
        DateTime ServerCreated { get; set; }
        DateTime ServerRenewed { get; set; }
        string ServerToken { get; set; }
    }
}
