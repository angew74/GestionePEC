using System;
using System.Collections.Generic;
using System.Text;
using ActiveUp.Net.Mail.DeltaExt;
using SendMail.Data.SQLServerDB;

namespace SendMail.Data.Contracts.Mail
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i mail servers.
    /// </summary>
    public interface IMailServerDao : IDao<MailServer, decimal>
    {
    }
}
