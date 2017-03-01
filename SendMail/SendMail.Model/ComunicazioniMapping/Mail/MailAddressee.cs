using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    [Serializable()]
    public class MailAddressee : IDomainObject
    {

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return (IdRubrica > 0 && !String.IsNullOrEmpty(MailAddress)); }
        }

        public bool IsPersistent
        {
            get { return false; }
        }

        #endregion

        #region "Properties"

        [DatabaseField("ID_RUB")]
        public Int64 IdRubrica { get; set; }

        [DatabaseField("MAIL")]
        public virtual String MailAddress { get; set; }

        #endregion
    }
}
