using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.SQLServerDB;
using SendMail.Model;

namespace SendMail.Data.SQLServerDB
{
    public interface IBackEndCodeDao : IDao<BackEndRefCode, decimal>
    {
        BackEndRefCode GetByCode(String backendCode);
        List<BackEndRefCode> GetByDescr(String backendDescr);
    }
}
