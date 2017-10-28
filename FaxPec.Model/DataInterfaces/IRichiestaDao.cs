using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using FaxPec.Model.QueryModel;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce le Richieste.
    /// </summary>
    public interface IRichiestaDao : IDao
    {
        void Insert(Richiesta item);

        Richiesta FindByKey(int key);

        Richiesta FindByKeyLazy(int key); //?PROXY?

        Richiesta FindByProtocollo(string protocollo);

        ICollection<Richiesta> FindByCriteria(Query query);

        ICollection<Richiesta> FindByCriteria(Query query, Pager pager, out int totalCount);
    }
}
