using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Unisys.Pdf.ManagedModules.ModuleParts;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione delle pratiche da evadere.
    /// </summary>
    public class Pratica : IDomainObject
    {
        public class SottoTitoloEventArgs : EventArgs
        {
            public decimal IdSottoTitolo { get; set; }
            public SottoTitolo SottoTitolo { get; set; }
        }

        public static event EventHandler<SottoTitoloEventArgs> onSetSottoTitoloId;

        #region "Private Fields"

        protected long m_ID_PRA;

        protected bool m_ID_PRAIsNull;

        #endregion

        #region "Properties"




        /// <summary>
        /// 
        /// </summary>
        public virtual Nullable<Int64> Id
        {
            get;
            set;
            //get
            //{
            //    if (this.m_ID_PRAIsNull)
            //        return null;
            //    else
            //        return this.m_ID_PRA;
            //}
            //set
            //{
            //    if (value.HasValue)
            //    {
            //        this.m_ID_PRAIsNull = false;
            //        this.m_ID_PRA = value.Value;
            //    }
            //    else
            //        this.m_ID_PRAIsNull = true;
            //}
        }

        /// <summary>
        /// Richiesta di riferimento della Pratica.
        /// </summary>
        /// <remarks>
        /// obbligatoria
        /// </remarks>
        public virtual Richiesta Richiesta { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Richiesta.
        /// </summary>
        public virtual Nullable<Int64> RichiestaId { get; set; }

        /// <summary>
        /// Accessor di servizio per mantenere traccia dell'indice 
        /// all'interno della collezione Pratiche della Richiesta.
        /// </summary>
        public virtual int Indice { get; set; }

        public virtual string Indirizzo { get; set; }

        public virtual string Nome { get; set; }

        public virtual string CodInd { get; set; }

        public virtual string CodiceFamiglia { get; set; }

        public virtual string Cognome { get; set; }
        public virtual string Sesso { get; set; }
        public virtual string CodFis { get; set; }
        public virtual string GiornoNascita { get; set; }
        public virtual string MeseNascita { get; set; }
        public virtual string AnnoNascita { get; set; }
        public virtual Allegato AllegatoAtt { get; set; }

        public string DataDiNascita
        {
            get
            {
                string ddn = String.Format("{0}/{1}/{2}",
                                            !string.IsNullOrEmpty(GiornoNascita) ? GiornoNascita : "00",
                                            !string.IsNullOrEmpty(MeseNascita) ? MeseNascita : "00",
                                            AnnoNascita);
                // salta solo se anno non presente
                return (ddn.Length != 10) ? string.Empty : ddn;
            }
        }

        public virtual string Note { get; set; }

        private SottoTitolo _SottoTitolo;
        public virtual SottoTitolo SottoTitolo
        {
            get
            {
                if (this._SottoTitolo == null)
                {
                    if (this.SottoTitoloId.HasValue == false)
                        _SottoTitolo = null;
                    else
                    {
                        _SottoTitolo = new SottoTitolo { Id = this.SottoTitoloId.Value };
                        if (onSetSottoTitoloId != null)
                        {
                            SottoTitoloEventArgs e = new SottoTitoloEventArgs { IdSottoTitolo = this.SottoTitoloId.Value };
                            onSetSottoTitoloId(this, e);
                            if (e.SottoTitolo != null)
                            {
                                _SottoTitolo = e.SottoTitolo;
                            }
                        }
                    }
                }
                return _SottoTitolo;
            }
            set
            {
                _SottoTitolo = value;
            }
        }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato SottoTitolo.
        /// </summary>
        public Nullable<Int64> SottoTitoloId
        {

            get
            {
                if (this.SottoTitolo != null)
                    return (long)this.SottoTitolo.Id;
                else
                    return null;
            }
        }

        public virtual int NumeroProcedure { get; set; }

        /// <summary>
        /// Restituisce il numero di procedure lavorate.
        /// </summary>
        public virtual int NumeroProcedureLavorate
        {
            get
            {
                int wrk = 0;
                if (Richiesta != null)
                {
                    if (Richiesta.Risposta != null && Richiesta.Risposta.Id != 0)
                        if (Id != null)
                        {
                            wrk = Richiesta.Risposta.TestoObj.ChunkPraticaCount(decimal.Parse(Id.ToString()));
                        }
                }
                return wrk;
            }
        }

        /// <summary>
        /// Indica se tutte le procedure della pratica sono state lavorate.
        /// </summary>
        public bool IsCompleta
        {
            get
            {
                return (NumeroProcedureLavorate == NumeroProcedure);
            }
        }

        /// <summary>
        /// Accessor di servizio per la visualizzazione 
        /// dello status di lavorazione della pratica.
        /// </summary>
        public string StatusProcedure
        {
            get
            {
                return String.Format("{0} su {1}", NumeroProcedureLavorate.ToString(), NumeroProcedure.ToString());
            }
        }

        /// <summary>
        /// Indica se la pratica è elaborabile automaticamente (allegato = PDF FORM).
        /// </summary>
        /// <returns></returns>
        public bool IsAutomatica
        {
            get
            {
                return false;
                //COMMENTATO DA ALBERTO 08/10/2012
                //if (Allegato.IsEmpty)
                //    return false;
                //else
                //    return (!string.IsNullOrEmpty(Allegato.XmlCode)); 
            }
        }

        /// <summary>
        /// Indica se la pratica prevede o può prevedere pagamenti.
        /// </summary>
        /// <returns></returns>
        public bool HasPagamenti
        {
            get
            {
                // todo.. rivedere strategia complessiva
                return (SottoTitolo.CodiceProcedura == "Mod.Cpri");
            }
        }

        #endregion

        #region "Properties 'oggetto'"

        /// <summary>
        /// Dati comuni della pratica richiesta.
        /// </summary>
        //public virtual DatiComuniRichiesta DatiRichiesta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Individuo
        {
            get
            {
                string cod = "";
                string nm = "";
                string cg = "";

                cod = CodFis;
                cg = Cognome;
                nm = Nome;

                return (!string.IsNullOrEmpty(cod)) ? cod + "-" + cg + " " + nm : cg + " " + nm;
            }
        }

        protected string individuopratica;

        public virtual string IndividuoPratica
        {
            get
            {
                if (GiornoNascita == string.Empty || GiornoNascita == null)
                { GiornoNascita = "00"; }
                if (MeseNascita == string.Empty || MeseNascita == null)
                { MeseNascita = "00"; }
                if (!string.IsNullOrEmpty(Cognome) &&
                    !string.IsNullOrEmpty(Nome))
                {
                    individuopratica = Sesso + "-" + Nome + "-" + Cognome + "-" + DataDiNascita;
                }
                else if (!string.IsNullOrEmpty(Cognome))
                {
                    individuopratica = Sesso + "-" + Cognome + "-" + DataDiNascita;
                }
                else if (!string.IsNullOrEmpty(Note))
                {
                    individuopratica = Note;
                }
                else { individuopratica = "NON INSERITO"; }
                return individuopratica;
            }

        }

        public string FullText
        {
            get
            {
                string t = "";
                t += (!string.IsNullOrEmpty(Cognome)) ? Cognome + " " : "";
                t += (!string.IsNullOrEmpty(Nome)) ? Nome + " " : "";
                t += (!string.IsNullOrEmpty(CodFis)) ? CodFis + " " : "";
                t += (!string.IsNullOrEmpty(DataDiNascita)) ? DataDiNascita + " " : "";
                t += (!string.IsNullOrEmpty(Note)) ? Note : "";
                return t.Trim();
            }
        }
        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Pratica()
        {
            Init();
        }

        public Pratica(Pratica p)
        {
            Init();
            if (p == null) return;

            Type t = this.GetType();
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
            {
                if (pi.CanWrite)
                    pi.SetValue(this, pi.GetValue(p, null), null);
            }
        }

        private void Init()
        {
            //..
            //Richiesta = new Richiesta();
            //Cmq. instanziato. Se empty significa che la pratica non ha allegato.
            //Allegato = new Allegato();
            SottoTitolo = new SottoTitolo();
            //DatiRichiesta = new DatiComuniRichiesta();
            Indice = -1;
            NumeroProcedure = 1;
        }

        #endregion

        #region "DomainObject members"

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get
            {
                //verifica i dati minimi
                //..
                //if (Richiesta == null && RichiestaId == default(int))
                //    return false;
                // if (m_REF_ID_SOT == default(decimal))
                //   return false;
                if (NumeroProcedure == default(int))
                    return false;
                //se ci sono i dati per fare una ricerca per CF
                else if (CodFis != string.Empty)
                    return true;
                else if (Indirizzo != string.Empty && Indirizzo != null)
                { return true; }
                else if (CodiceFamiglia != string.Empty && CodiceFamiglia != null)
                { return true; }
                //se ci sono i dati per fare una ricerca minima per dati anagrafici
                else if (ModelHelper.CheckDataComposta(GiornoNascita, MeseNascita, AnnoNascita) && Cognome != string.Empty && Sesso != string.Empty)
                    return true;
                //se ci sono solo le note
                else if (Note != string.Empty
                        && string.IsNullOrEmpty(GiornoNascita)
                        && string.IsNullOrEmpty(MeseNascita)
                        && string.IsNullOrEmpty(AnnoNascita)
                        && string.IsNullOrEmpty(Cognome)
                        && string.IsNullOrEmpty(Nome)
                        && string.IsNullOrEmpty(Sesso)
                        && string.IsNullOrEmpty(CodFis)
                        && string.IsNullOrEmpty(CodInd))
                    return true;
                else return false;
            }
        }

        /// <summary>
        /// Verifica che l'allegato sia "virtualmente" persistente.
        /// </summary>
        /// <returns></returns>
        public bool IsPersistent
        {
            //get { return (Id != (default(decimal))); }
            get { return (Id.HasValue && Id != default(decimal)); }
        }

        public bool IsEmpty
        {
            get
            {
                return (string.IsNullOrEmpty(Cognome)
                            && string.IsNullOrEmpty(Nome)
                            && string.IsNullOrEmpty(CodFis)
                            && string.IsNullOrEmpty(DataDiNascita)
                            && string.IsNullOrEmpty(Note));
            }
        }
        #endregion

        #region "Public Methods"
        public void SetSottoTitolo(decimal idSottotitolo)
        {
            SottoTitoloEventArgs e = new SottoTitoloEventArgs { IdSottoTitolo = idSottotitolo };
            if (onSetSottoTitoloId != null)
                onSetSottoTitoloId(this, e);
            if (e.SottoTitolo != null)
            {
                this.SottoTitolo = e.SottoTitolo;
            }
            else
                this.SottoTitolo = new SottoTitolo { Id = idSottotitolo };
        }
        #endregion
    }
}
