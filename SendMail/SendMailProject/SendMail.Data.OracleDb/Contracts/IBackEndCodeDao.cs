using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;

namespace SendMail.DataContracts.Interfaces
{
    public interface IBackEndCodeDao : IDao<BackEndRefCode, decimal>
    {
        BackEndRefCode GetByCode(String backendCode);
        List<BackEndRefCode> GetByDescr(String backendDescr);
    }
}
