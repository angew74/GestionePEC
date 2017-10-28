using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
{
    public class Nazioni : IDomainObject
    {
        #region IDomainObject Membri di

        public bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(CodiceISO) &&
                    !String.IsNullOrEmpty(DenominazioneISO);
            }
        }

        public bool IsPersistent
        {
            get { return StatoId > 0; }
        }

        #endregion

        #region Public Properties
        public int StatoId { get; set; }
        public string Denominazione { get; set; }
        public string CodiceIstat { get; set; }
        public string CodiceISO { get; set; }
        public string DenominazioneISO { get; set; }
        #endregion

        #region C.tor
        public Nazioni()
        {
            StatoId = -1;
        }
        #endregion
    }
}
