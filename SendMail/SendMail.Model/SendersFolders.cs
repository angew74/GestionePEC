using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SendMail.Model
{
    public class SendersFolders : IDomainObject
    {

        //[DatabaseField("ID_FS")]
        //public virtual Int32 IdFolderSender { get; set; }

        [DatabaseField("ID")]
        public virtual Int32 IdFolder { get; set; }

        [DatabaseField("ID_SENDER")]
        public virtual Int32 IdSender { get; set; }

        [DatabaseField("NOME")]
        public virtual string Nome { get; set; }

        [DatabaseField("MAIL")]
        public virtual string Mail { get; set; }

        [DatabaseField("TIPO")]
        public virtual string Tipo { get; set; }

        [DatabaseField("SYSTEM")]
        public virtual Int16 System { get; set; }

        [DatabaseField("IDNOME")]
        public virtual Int16 IdNome { get; set; }

        //public virtual List<BackEndUserMailUserMapping> MappedMails { get; set; }

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { return true; }
        }

        public bool IsPersistent
        {
            get { return true; }
        }

        #endregion
    }
}
