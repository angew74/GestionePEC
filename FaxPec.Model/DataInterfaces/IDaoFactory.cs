using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.DataInterfaces
{
    /// <summary>
    /// Interfaccia per il recupero degli oggetti DAO.
    /// </summary>
    public interface IDaoFactory
    {
        IRichiestaDao GetRichiestaDao();
        INominativoRubricaDao GetNominativoRubricaDao();
        IPraticaDao GetPraticaDao();
    }
}
