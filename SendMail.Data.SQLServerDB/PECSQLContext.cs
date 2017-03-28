using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMail.Data.SQLServerDB
{
    public class PECSQLContext :DbContext
    {
        public PECSQLContext() : base("name=FAXPECContext") { }
               
    }
}
