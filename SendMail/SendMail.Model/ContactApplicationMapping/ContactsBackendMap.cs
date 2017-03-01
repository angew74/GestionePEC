using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Model.RubricaMapping;

namespace SendMail.Model.ContactApplicationMapping
{
    public class ContactsBackendMap : IDomainObject
    {
        #region IDomainObject Membri di

        public bool IsValid
        {
            get
            {
                return (Canale != TipoCanale.UNKNOWN &&
                        Backend != null &&
                        Titolo != null);
            }
        }

        public bool IsPersistent
        {
            get { return Id >= 0; }
        }

        #endregion

        #region Properties
        public int Id { get; set; }
        public TipoCanale Canale { get; set; }
        public BackEndRefCode Backend { get; set; }      
        public RubricaContatti Contatto { get; set; }
        public Titolo Titolo { get; set; }
        #endregion
    }
}
