using System;
using System.Collections.Generic;
using System.Text;
using FaxPec.Model.QueryModel;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i destinatari.
    /// </summary>
    public interface IDestinatarioDao : IDao
    {
        void Insert(Destinatario item);

        void Update(Destinatario item);

        Destinatario FindByKey(int key);

        ICollection<Destinatario> FindByCriteria(Query query);

        ICollection<Destinatario> FindByCriteria(Query query, Pager pager);

    }
}
