using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SendMail.Model
{
    [Serializable]
    [DataContract]
    public class Titolo : IDomainObject
    {
        public Titolo select()
        {
            return this;
        }

        public Titolo Save(Titolo e)
        {
            return e;
        }

        private decimal _Id;
        private string _CodiceProtocollo;
        private string _AppCode;
        private string _Titolo;
        private string _Note;
        private int _Deleted;

        public virtual bool Deleted
        {
            get { if (_Deleted == 0)return false; else return true; }
            set { if (value == true) _Deleted = 1; else _Deleted = 0;}
        }

        [DataMember]
        public virtual decimal Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public virtual string CodiceProtocollo
        {
            get { return _CodiceProtocollo; }
            set { _CodiceProtocollo = value; }
        }

        public virtual string AppCode
        {
            get { return _AppCode; }
            set { _AppCode = value; }
        }

        public virtual string Note
        {
            get { return _Note; }
            set { _Note = value; }
        }

        [DataMember]
        public virtual string Nome
        {
            get { return _Titolo; }
            set { _Titolo = value; }
        }
        
        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Titolo()
        {
            Init();
        }

        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
        }
        
        #endregion

        #region "DomainObject members"

        public bool IsEmpty
        {
            get { return Id == -1; }
        }

        public bool IsValid
        {
            get
            {
                //todo.. altre verifiche
                //tutto ok
                return true;
            }
        }

        public bool IsPersistent
        {
            get { return Id != (default(int)); }
        } 

        #endregion
    }
}
