using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using SendMail.DataContracts.Interfaces;
using SendMail.DataContracts;
using Com.Delta.Data;


namespace SendMail.Business.Base
{
    /// <summary>
    /// Classe astratta base di tutte le classi Business.
    /// riporta tutti i riferimenti a layers diversi da questo e caricati dinamicamente dal sistema
    /// </summary>
    public abstract class BusinessBase
    {
        public virtual IDaoBaseSession<ISessionModel> getDaoContext()
        {
            return DaoLocator.GetDaoProvider();
        }
    }
}
