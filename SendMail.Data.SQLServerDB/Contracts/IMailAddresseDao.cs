using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.Data.SQLServerDB
{
    public interface IMailAddresseDao : IDao<MailAddressee, Int64>
    {
    }
}
