using System;
using System.Collections.Generic;
using System.Text;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;

namespace SendMail.DataContracts.Interfaces
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
