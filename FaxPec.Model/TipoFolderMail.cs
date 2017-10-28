using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FaxPec.Model
{
    public enum TipoMailFolder
    {
        Tutte = 0,
        Ricevute = 1,
        Server_Comunications = 2,
        Inviate = 3,
        Problemi_di_consegna = 4,
        Cestino = 5
    }
}
