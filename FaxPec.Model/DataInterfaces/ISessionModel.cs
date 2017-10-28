using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FaxPec.Model.DataInterfaces;

namespace FaxPec.Model.DataInterfaces
{
    public interface ISessionModel
    {
        FaxPec.Model.DataInterfaces.INominativoRubricaDao NominativoRubricaDao { get; }
        FaxPec.Model.DataInterfaces.IRichiestaDao RichiestaDao { get; }
        FaxPec.Model.DataInterfaces.IPraticaDao PraticaDao { get; }
    }
}
