using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
{
    public class Comuni : IDomainObject
    {
        #region IDomainObject Membri di
        public bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(Denominazione) &&
                    Nazione != null;
            }
        }

        public bool IsPersistent
        {
            get { return ComuneId > 0; }
        }
        #endregion

        #region Public Properties
        public int ComuneId { get; set; }
        public string CodiceComune { get; set; }
        public string Denominazione { get; set; }
        public string CodiceComuneIstat { get; set; }
        public string SiglaProvincia { get; set; }
        public int? StatoId { get; set; }
        public Nazioni Nazione { get; set; }
        #endregion
    }
}
