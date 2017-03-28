using System;
using System.Collections.Generic;
using System.Text;
using SendMail.Data.SQLServerDB;
using SendMail.Model;

namespace SendMail.Data.SQLServerDB
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
