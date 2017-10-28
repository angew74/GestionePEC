using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace FaxPec.Model
{
    /// <summary>
    /// 
    /// </summary>
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
        private string _Nome;
        private string _Ufficio;
        private string _Note;
        private Int32 _Deleted;

        public decimal Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Nome
        {
            get { return _Nome; }
            set { _Nome = value; }
        }
        
        [ScriptIgnore]
        public string Ufficio
        {
            get { return _Ufficio; }
            set { _Ufficio = value; }
        }

        [ScriptIgnore]
        public string Note
        {
            get { return _Note; }
            set { _Note = value; }
        }

        [ScriptIgnore]
        public Int32 Deleted
        {
            get { return _Deleted; }
            set { _Deleted = value; }
        }
        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Titolo()
        {
            Init();
        }

        public Titolo(decimal ID_TIT, string TITOLO, string UFFICIO, string NOTE, int DELETED)
        {
            Init();
            this.Id = ID_TIT;
            this.Nome = TITOLO;
            this.Ufficio = UFFICIO;
            this.Note = NOTE;
            this.Deleted = DELETED;
        }

        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
        }
        
        #endregion

        [ScriptIgnore]
        public bool IsGeneric
        {
            // todo.. un po' troppo su pietra..
            get
            {
                return (Nome == "RICHIESTA MULTIPLA");
            }
        }

        #region "DomainObject members"

        [ScriptIgnore]
        public bool IsEmpty
        {
            get { return Id == -1; }
        }

        [ScriptIgnore]
        public bool IsValid
        {
            get
            {
                //todo.. altre verifiche

                //tutto ok
                return true;
            }
        }

        [ScriptIgnore]
        public bool IsPersistent
        {
            get { return Id != (default(int)); }
        } 

        #endregion

       

    }
}
