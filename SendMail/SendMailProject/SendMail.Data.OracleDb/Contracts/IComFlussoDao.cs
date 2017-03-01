using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using SendMail.Model.ComunicazioniMapping;

namespace SendMail.DataContracts.Interfaces
{
    public interface IComFlussoDao : IDao<ComFlusso, Int64>
    {
    }
}
