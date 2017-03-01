using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
   public class ActionsFolders : IDomainObject
    {
        [DatabaseField("ID")]
        public virtual Int32 IdAction { get; set; }

        [DatabaseField("ID_NOME_DESTINAZIONE")]
        public virtual Int32 IdNomeDestinazione { get; set; }

        [DatabaseField("FOLDERID")]
        public virtual Int32 FolderIdDestinazione { get; set; }
       
        [DatabaseField("TIPO")]
        public virtual string Tipo { get; set; }

        #region IDomainObject Membri di

        public bool IsValid
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPersistent
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    
    }
}
