using System;
using System.Collections.Generic;
using System.Text;
using SendMail.DataContracts.Interfaces;
using SendMail.Model;

namespace SendMail.DataContracts.Interfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i sottotitoli.
    /// </summary>
    public interface ISottoTitoloDao : IDao<SottoTitolo,decimal>
    {
       
        ICollection<SottoTitolo> FindByTitolo(decimal tkey);
        void DeleteLogic(decimal id);
        SottoTitolo GetSottoTitoloByComCode(string comcode);
       
    }
}
