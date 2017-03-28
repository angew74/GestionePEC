using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Data.SQLServerDB;
using SendMail.Model.ComunicazioniMapping;

namespace SendMail.Data.SQLServerDB
{
    public interface IComAllegatoDao : IDao<ComAllegato, Int64>
    {
        void Update(IList<ComAllegato> entities);
    }
}
