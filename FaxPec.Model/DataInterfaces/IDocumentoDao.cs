using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i documenti.
    /// </summary>
    public interface IDocumentoDao : IDao
    {
        void Insert(Documento item);

        ICollection<Documento> FindByRichiesta(int richiestaKey);

        ICollection<Documento> FindByRisposta(int rispostaKey);

        ICollection<Documento> FindByPratica(int praticaKey);
    }
}
