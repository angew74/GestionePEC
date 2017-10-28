using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace FaxPec.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SottoTitolo : Titolo
    {
        public SottoTitolo()
        {
            Init();
        }

        public SottoTitolo(Titolo padre)
        {
            Init();
            base.Id = padre.Id;
            base.Nome = padre.Nome;
            base.Note = padre.Note;
            base.Ufficio = padre.Ufficio;
        }

        public SottoTitolo(decimal ID_SOT,decimal	REF_ID_TIT,string	SOTTOTITOLO,string	UFFICIO,string	CODICE_PROCEDURA,string	NOTE,int	DELETED,string	URL_PROCEDURA)
        {
            this.Id=ID_SOT;
            this.RefIdTitolo=REF_ID_TIT;
            this.Nome=SOTTOTITOLO;
            this.Ufficio=UFFICIO;
            this.CodiceProcedura=CODICE_PROCEDURA;
            this.Note=NOTE;
            this.UrlProcedura=URL_PROCEDURA;
        }

        public SottoTitolo selectSottoTitolo()
        {
            return this;
        }

        public SottoTitolo saveSottoTitolo(SottoTitolo e)
        {
            return e;
        }


        private decimal _RefIdTitolo;
        private string _CodiceProcedura;
        private string _UrlProcedura;
        private Titolo _Titolo;

        public string CodiceProcedura
        {
            get { return _CodiceProcedura; }
            set { _CodiceProcedura = value; }
        }

        [ScriptIgnore]
        public string UrlProcedura
        {
            get { return _UrlProcedura; }
            set { _UrlProcedura = value; }
        }

        [ScriptIgnore]
        public Titolo Titolo
        {
            get { if (_Titolo == null) _Titolo = new Titolo(); return _Titolo; }
            set { _Titolo = value; }
        }

        [ScriptIgnore]
        public decimal RefIdTitolo
        {
            get { return Titolo.Id; }
            set { if (_Titolo == null) _Titolo = new Titolo(); _Titolo.Id = value; }
        }

        /// <summary>
        /// TOKILL!
        /// </summary>
        [ScriptIgnore]
        public Titolo TitoloRiferimento { get { return new Titolo(); } }

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        

        private void Init()
        {
        }

        #endregion

        #region "DomainObject members"

        [ScriptIgnore]
        public bool IsEmpty
        {
            get { return Id == -1; }
        }

        [ScriptIgnore]
        public  bool IsValid
        {
            get
            {
                //todo.. altre verifiche

                //tutto ok
                return true;
            }
        }

        [ScriptIgnore]
        public  bool IsPersistent
        {
            get { return Id != (default(int)); }
        }

        #endregion



    }
}

