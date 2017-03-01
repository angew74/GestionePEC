using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    public class BackEndRefCode : IDomainObject
    {
        public BackEndRefCode select()
        {
            return this;
        }

        public BackEndRefCode Save(BackEndRefCode e)
        {
            return e;
        }

        private decimal _Id;
        private string _Codice;
        private string _Descrizione;
        private string _Categoria;

        public decimal Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Codice
        {
            get { return _Codice; }
            set { _Codice = value; }
        }

        public string Descrizione
        {
            get { return _Descrizione; }
            set { _Descrizione = value; }
        }

        public string Categoria
        {
            get { return _Categoria; }
            set { _Categoria = value; }
        }

        public string DescrizionePlus { get; set; }

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public BackEndRefCode()
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
