using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SendMail.Model
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SottoTitolo : Titolo
    {
        public SottoTitolo()
        {
            Init();
        }

        public SottoTitolo(Titolo padre)
        {
            Init();
            //base.Id = padre.Id;
            //base.AppCode = padre.AppCode;
            //base.Nome = padre.Nome;
            //base.Note = padre.Note;
            //base.CodiceProtocollo = padre.CodiceProtocollo;
            //base.Deleted = padre.Deleted;
            this.Titolo = padre;
        }

        public SottoTitolo selectSottoTitolo()
        {
            return this;
        }

        public SottoTitolo saveSottoTitolo(SottoTitolo e)
        {
            return e;
        }

        #region "Private Fields"
        private Titolo _Titolo;
        private string _ComCode;
        private bool _Deleted;
        private decimal _Id;
        private string _Note;
        private string _SottoTitolo;
        private bool _UsaProtocollo;
        private string _ProtocolloSubCode;
        private string _ProtocolloPassword;
        private List<ProtocolloTypes> _TipiProcollo;
        private bool _ProtocolloLoadAllegati;
        private string _ProtocolloCode;
        #endregion

        #region "Public Properties"
        public new bool Deleted
        {
            get { return _Deleted; }
            set { _Deleted = value; }
        }

        [DataMember]
        public new decimal Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public new string CodiceProtocollo
        {
            get
            {
                if (Titolo == null) return null;
                return Titolo.CodiceProtocollo;
            }
        }

        public new string AppCode
        {
            get
            {
                if (Titolo == null) return null;
                return Titolo.AppCode;
            }
        }

        public new string Note
        {
            get { return _Note; }
            set { _Note = value; }
        }
        [DataMember]
        public new string Nome
        {
            get { return _SottoTitolo; }
            set { _SottoTitolo = value; }
        }

        public string ComCode
        {
            get { return _ComCode; }
            set { _ComCode = value; }
        }

        public Titolo Titolo
        {
            get { if (_Titolo == null) _Titolo = new Titolo(); return _Titolo; }
            set { _Titolo = value; }
        }

        public decimal RefIdTitolo
        {
            get { return Titolo.Id; }
            set { if (_Titolo == null) _Titolo = new Titolo(); _Titolo.Id = value; }
        }

        public bool UsaProtocollo
        {
            get { return _UsaProtocollo; }
            set { _UsaProtocollo = value; }
        }

        public string ProtocolloSubCode
        {
            get { return _ProtocolloSubCode; }
            set { _ProtocolloSubCode = value; }
        }

        public string ProtocolloPassword
        {
            get { return _ProtocolloPassword; }
            set { _ProtocolloPassword = value; }
        }

        public List<ProtocolloTypes> TipiProcollo
        {
            get { return _TipiProcollo; }
            set { _TipiProcollo = value; }
        }

        public bool ProtocolloLoadAllegati
        {
            get { return _ProtocolloLoadAllegati; }
            set { _ProtocolloLoadAllegati = value; }
        }

        public string ProtocolloCode
        {
            get { return _ProtocolloCode; }
            set { _ProtocolloCode = value; }
        }
        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
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

