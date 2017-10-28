using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce le risposte.
    /// </summary>
    public interface IRispostaDao
    {
        void Insert(Risposta item);

        ICollection<Risposta> FindByRichiesta(int richiestaKey);
    }
}
