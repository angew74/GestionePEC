using Com.Delta.Data;


namespace SendMail.BusinessEFEF.Base
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
