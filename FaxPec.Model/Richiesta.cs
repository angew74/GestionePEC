using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione delle Richieste.
    /// </summary>
    public class Richiesta : IDomainObject
    {
        public class TitoloEventArgs : EventArgs
        {
            public decimal IdTitolo { get; set; }
            public Titolo Titolo { get; set; }
        }

        public static event EventHandler<TitoloEventArgs> onSetTitolo;

        #region "Private Fields"

        /// <summary>
        /// Identificativo Titolo collegato
        /// </summary>
        private decimal _TitoloId;

        #endregion

        #region "Properties"

        /// <summary>
        /// Id di riferimento.
        /// </summary>
        public virtual decimal Id { get; set; }

        /// <summary>
        /// Tipo di canale utilizzato per la richiesta.
        /// </summary>
        public virtual TipoCanale Canale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime DataInserimento { get; set; }

        /// <summary>
        /// Getter per la gestione in lettura della DataInserimento.
        /// </summary>
        public string DataInserimentoRead
        {
            get
            {
                if (DataInserimento == default(DateTime))
                    return string.Empty;
                else
                    return DataInserimento.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime DataProtocollo { get; set; }

        /// <summary>
        /// Getter per la gestione in lettura del Protocollo.
        /// </summary>
        public string ProtocolloIngressoRead
        {
            get
            {
                if (DataProtocollo == default(DateTime))
                    return ProtocolloIngresso;
                else
                    return String.Format("{0}/{1}", DataProtocollo.ToString("yyyy"), ProtocolloIngresso);
            }
        }

        /// <summary>
        /// Protocollo della richiesta.
        /// </summary>
        public virtual string ProtocolloIngresso { get; set; }

        /// <summary>
        /// Eventuale protocollo del mittente.
        /// </summary>
        public virtual string ProtocolloMittente { get; set; }

        /// <summary>
        /// Oggetto della richiesta.
        /// </summary>
        public virtual string Oggetto { get; set; }

        /// <summary>
        /// Eventuale testo della richiesta.
        /// </summary>
        public virtual string Testo { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public virtual string MailID { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public virtual string MailAccount { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public virtual StatoRichiesta Stato { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public virtual Titolo Titolo { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Titolo.
        /// </summary>
        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Titolo.
        /// </summary>
        public decimal TitoloId
        {
            //..
            get
            {
                if (Titolo != null && Titolo.IsPersistent)   //..
                    _TitoloId = Titolo.Id;
                else _TitoloId = default(decimal);
                return _TitoloId;
            }
            //set { _TitoloId = value; }
        }

        /// <summary>
        /// Risposta collegata.
        /// </summary>
        public virtual Risposta Risposta { get; set; }

        /// <summary>
        /// Nominativo di riferimento.
        /// </summary>
        /// <remarks>
        /// non serve accessor perché Nominativo è un oggetto 'aggregato'
        /// e non collegato.
        /// </remarks>
        public virtual Nominativo Richiedente { get; set; }

        /// <summary>
        /// MailServer di riferimento (non obbligatorio).
        /// </summary>
        //public virtual ActiveUp.Net.Mail.UnisysExt.MailServer MailServer { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato MailServer.
        /// </summary>
        public int? MailServerId { get; set; }


        IList<Pratica> listPratica;
        /// <summary>
        /// 
        /// </summary>
        public virtual IList<Pratica> Pratiche
        {
            get
            {
                return listPratica;
            }
            set
            {
                listPratica = value;
            }
        }


        string rawHTML = string.Empty;
        public string RawHTML
        {
            get
            {
                if (listPratica != null)
                {
                    foreach (Pratica p in listPratica)
                    {
                        rawHTML += p.IndividuoPratica + "<br />";
                    }
                }
                return rawHTML;
            }

        }


        /// <summary>
        /// Accessor per il calcolo del numero complessivo di procedure richieste.
        /// </summary>
        public int NumeroProcedure
        {
            get
            {
                if (Pratiche != null)
                    return Pratiche.Sum(q => q.NumeroProcedure);
                else
                    return 0;
            }
        }

        /// <summary>
        /// Accessor per la visualizzazione dello stato di lavorazione delle procedure.
        /// </summary>
        /// <remarks>
        /// Probabilmente da rivedere in caso di risposte 1..n.
        /// </remarks>
        public string StatusProcedure
        {
            get
            {
                int wrk = (Risposta != null) ? Risposta.TestoObj.ChunkPraticheCount() : 0;
                return String.Format("{0} su {1}", wrk.ToString(), NumeroProcedure.ToString());
            }
        }

        /// <summary>
        /// Accessor per il calcolo del numero complessivo di allegati.
        /// </summary>
        //public int NumeroAllegati
        //{
        //    get
        //    {
        //        if (Pratiche != null && Pratiche.Count() > 0)
        //            return Pratiche.Sum(q => q.NumeroAllegati);
        //        else
        //            return 0;
        //    }
        //}

        /// <summary>
        /// Verifica che la richiesta coinvolga la lavorazione
        /// di una determinata procedura.
        /// </summary>
        /// <param name="urlProcedura">url della procedura</param>
        /// <returns></returns>
        public bool ContainsProcedura(string urlProcedura)
        {
            return (Pratiche.Where(q => urlProcedura.Contains(q.SottoTitolo.UrlProcedura)).Count() > 0);
        }

        /// <summary>
        /// Verifica che la richiesta coinvolga la lavorazione
        /// di una determinata procedura.
        /// </summary>
        /// <param name="urlProcedura">url della procedura</param>
        /// <returns></returns>
        public bool ContainsProcedura(decimal idSottoTitolo)
        {
            return (Pratiche.Where(q => idSottoTitolo == (q.SottoTitolo.Id)).Count() > 0);
        }

        /// <summary>
        /// Indica se la richiesta deve essere protocollata.
        /// </summary>
        /// <returns></returns>
        public bool ToBeCertified { get; set; }

        /// <summary>
        /// Verifica che la richiesta contenga procedure a pagamento.
        /// </summary>
        /// <returns></returns>
        public bool HasPagamentiCalcolabili
        {
            get
            {
                // non ha pratiche incomplete che possono prevedere pagamenti
                return (Pratiche.Where(q => q.HasPagamenti && !q.IsCompleta).Count() == 0);
            }
        }

        /// <summary>
        /// Verifica che tutte le pratiche siano state lavorate.
        /// </summary>
        public bool IsCompleta
        {
            get
            {
                if (Pratiche.Count == 0) return false;
                return (Pratiche.Where(q => !q.IsCompleta).Count() == 0);
            }
        }

        /// <summary>
        /// Verifica che abbia almeno una procedura lavorata.
        /// </summary>
        /// <remarks>
        /// Funziona anche con rapporto pratica procedure 1..n, 
        /// attualmente non previsto.
        /// </remarks>
        public bool HasProceduraLavorata
        {
            get
            {
                return (Pratiche.Where(q => q.NumeroProcedureLavorate > 0).Count() > 0);
            }
        }

        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Richiesta()
        {
            Init();
        }

        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
            Titolo = new Titolo();
            Richiedente = new Nominativo();
            //MailServerRiferimento = new MailServer(); //può essere null
            //MailServerRefId = null;
            Pratiche = new List<Pratica>();
            Canale = TipoCanale.UNKNOWN;
            DataInserimento = default(DateTime);
            DataProtocollo = default(DateTime);
            ProtocolloIngresso = string.Empty;
            ProtocolloMittente = string.Empty;
            Oggetto = string.Empty;
            Testo = string.Empty;
            MailID = string.Empty;
            MailAccount = string.Empty;
            Stato = default(StatoRichiesta);
            Risposta = new Risposta(this);
        }

        #endregion

        #region "DomainObject members"

        /// <summary>
        /// Verifica la validità dell'oggetto.
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            /*
             * Oggetto, 
             * Almeno una Pratica, 
             * Canale coerente, 
             * Richiedente formalmente valido.
            */
            get
            {
                //* todo..
                //verifica i dati minimi
                if (string.IsNullOrEmpty(Oggetto))
                    return false;
                if (TitoloId == default(decimal))
                    return false;
                //if (!(NumeroPratiche > 0))    //CALC.
                //    return false;
                //.. verifica su pratiche 
                //con l'introduzione della funzionalità di aggiunta pratiche automatico la richiesta deve essere valida anche con 0 pratiche
                //if (!(Pratiche.Count() > 0))
                //    return false;
                foreach (Pratica item in Pratiche)
                {
                    if (!item.IsValid)
                        return false;
                }
                if (!CanaleIsValid)
                    return false;
                if (!Richiedente.IsValid)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Verifica la combinazione di canale e info di contatto.
        /// </summary>
        /// <returns></returns>
        private bool CanaleIsValid
        {
            get
            {
                if (Canale == default(TipoCanale))
                    return false;
                if (Canale == TipoCanale.MAIL && string.IsNullOrEmpty(Richiedente.Mail))
                    return false;
                if (Canale == TipoCanale.FAX && string.IsNullOrEmpty(Richiedente.Fax))
                    return false;
                if (Canale == TipoCanale.POSTA && !Richiedente.Recapito.IsValid)
                    return false;
                //if (Canale == TipoCanale.A_MANO)
                //    return false;

                return true;
            }
        }

        /// <summary>
        /// Verifica che l'oggetto sia "virtualmente" persistente.
        /// </summary>
        /// <returns></returns>
        public bool IsPersistent
        {
            get { return Id != (default(int)); }
        }

        #endregion

        #region "Public Methods"

        public Pratica GetPraticaByKey(decimal pratKey)
        {
            return Pratiche.Where(q => q.Id == pratKey).FirstOrDefault();
        }

        public void SetTitolo(decimal titoloId)
        {
            TitoloEventArgs e = new TitoloEventArgs { IdTitolo = titoloId };
            if (onSetTitolo != null)
            {
                onSetTitolo(this, e);
            }
            if (e.Titolo != null)
                this.Titolo = e.Titolo;
            else
                this.Titolo = new Titolo { Id = titoloId };
        }

        #endregion
    }
}
