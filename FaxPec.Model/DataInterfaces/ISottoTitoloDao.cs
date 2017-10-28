using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i sottotitoli.
    /// </summary>
    public interface ISottoTitoloDao
    {
        ICollection<SottoTitolo> GetAll();
    }
}
