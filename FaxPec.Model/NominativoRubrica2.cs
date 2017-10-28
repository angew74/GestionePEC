using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione dei Nominativi in Rubrica.
    /// </summary>
    //public class NominativoRubrica2 : NominativoRubrica
    //{
    //    #region "Properties"

    //    /// <summary>
    //    /// Id di riferimento.
    //    /// </summary>
    //    public virtual int Id { get; set; }

    //    /// <summary>
    //    /// Indica se il nominativo in rubrica è 'certificato' 
    //    /// e pertanto non modificabile.
    //    /// </summary>
    //    public virtual bool IsCertified { get; set; }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public virtual string Note { get; set; }

    //    #endregion

    //    #region "C.tor"

    //    /// <summary>
    //    /// C.tor
    //    /// </summary>
    //    public NominativoRubrica2()
    //        : base()
    //    {
    //        Init();
    //    }

    //    /// <summary>
    //    /// Inizializza le properties.
    //    /// </summary>
    //    private void Init()
    //    {
    //        IsCertified = false;
    //        Note = string.Empty;
    //    }

    //    #endregion

    //    #region "DomainObject members"

    //    /// <summary>
    //    /// Verifica che il nominativo in rubrica sia "virtualmente" persistente
    //    /// </summary>
    //    /// <returns></returns>
    //    public override bool IsPersistent
    //    {
    //        get { return Id != (default(int)); }
    //    }

    //    #endregion
    //}
}
