using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model;

namespace SendMail.Data.SQLServerDB
{
    public interface IMailRefsDao : IDao<MailRefs, Int64>
    {
        ICollection<MailRefs> GetMailRefsOfAMail(Int64 idMail);
       
    }
}
