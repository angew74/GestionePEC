using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i mail servers.
    /// </summary>
    public interface IMailServerDao
    {
        ICollection<MailServer> GetAll();
    }
}
