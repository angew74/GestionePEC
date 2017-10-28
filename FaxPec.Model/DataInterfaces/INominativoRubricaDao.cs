using System;
using System.Collections.Generic;
using System.Text;
using FaxPec.Model;
using FaxPec.Model.QueryModel;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per l'oggetto DAO che gestisce i nominativi in rubrica.
    /// </summary>
    public interface INominativoRubricaDao : IDao
    {
        void Insert(NominativoRubrica item);

        void Update(NominativoRubrica item);

        NominativoRubrica FindByKey(int id);

        ICollection<NominativoRubrica> FindByCriteria(Query query);

        ICollection<NominativoRubrica> FindByCriteria(Query query, Pager pager, out int totalcount);
    }
}
