using System;
using System.Collections.Generic;
using System.Text;
using SendMail.Data.SQLServerDB;
using SendMail.Model;

namespace SendMail.Data.SQLServerDB
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i titoli.
    /// </summary>
    public interface ITitoloDao : IDao<Titolo,decimal>
    {
        ICollection<Titolo> GetSottotitoliByIdPadre(decimal titoloKey);
        void DeleteLogic(decimal id);
    }
}
