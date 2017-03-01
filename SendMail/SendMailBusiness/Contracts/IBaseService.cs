using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Business.Contracts
{
    public interface IBaseService : IDisposable
    {
        //start external session
        void SessionStart(Type requestor);
        //end external session
        void SessionEnd(Type requestor);
    }
}
