using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce le pratiche.
    /// </summary>
    public interface IPraticaDao : IDao
    {
        void Insert(Pratica p);

        ICollection<Pratica> FindByRichiesta(int rkey);
    }
}
