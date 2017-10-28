using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione..
    /// </summary>
    public class Risposta : IDomainObject
    {
        #region "Private Fields"

        /// <summary>
        /// Identificativo Richiesta
        /// </summary>
        private decimal _RichiestaId;

        #endregion
        
        #region "Properties"

        /// <summary>
        /// 
        /// </summary>
        public virtual decimal Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Richiesta Richiesta { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Richiesta.
        /// </summary>
        public decimal RichiestaId 
        { 
            //..
            get
            {
                if (Richiesta != null)   //..
                    _RichiestaId = Richiesta.Id;
                return _RichiestaId;
            }
            set { _RichiestaId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool Mittente { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<Documento> Documenti { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string ProtocolloOut { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Oggetto { get; set; }

        /// <summary>
        /// .
        /// </summary>
        public virtual StatoPagamentoRisposta StatoPagamento { get; set; }

        /// <summary>
        /// Codice pagamento da usare come causale.
        /// </summary>
        public virtual string CodicePagamento { get; set; }

        /// <summary>
        /// Importo complessivo pagamento.
        /// </summary>
        public virtual decimal ImportoPagamento { get; set; }

        //private string testo;
        private Body testoObj = null;

        //public virtual string Testo 
        //{
        //    get
        //    {
        //        return testo;
        //    }
        //    set
        //    {
        //        testo = value;
        //        if(!string.IsNullOrEmpty(testo))
        //            TestoObj = new Body().BodySeserialize(testo);
        //    }
        //}

        /// <summary>
        /// Accessor stringa all'oggetto Body
        /// </summary>
        public virtual string Testo
        {
            get
            {
                return testoObj.BodyDeserialize(testoObj);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    testoObj = new Body().BodySeserialize(value);
            }
        }
        
        /// <summary>
        /// Accessor stringa all'oggetto Body
        /// </summary>
        public virtual Body TestoObj
        {
            get
            {
                return testoObj;
            }
            set
            {
                testoObj = value;
                //testo = testoObj.BodyDeserialize(testoObj);
            }
        }
                
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime DataProtocollo { get; set; }

        /// <summary>
        /// Getter per la gestione in lettura del Protocollo.
        /// </summary>
        public string ProtocolloUscitaRead
        {
            get
            {
                if (DataProtocollo == default(DateTime))
                    return ProtocolloOut;
                else
                    return String.Format("{0}/{1}", DataProtocollo.ToString("yyyy"), ProtocolloOut);
            }
        }

        /// <summary>
        /// Indica se la risposta è stata inviata (chiusa) o meno.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInviata { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime DataInvio { get; set; }

        /// <summary>
        /// Getter per la gestione in lettura della DataInvio.
        /// </summary>
        public string DataInvioRead
        {
            get
            {
                if (DataInvio == default(DateTime))
                    return string.Empty;
                else
                    return DataInvio.ToString("dd/MM/yyyy");
            }
        }

        #endregion

        #region "Properties calcolate"

        /// <summary>
        /// Verifica se la risposta è modificabile mediante l'eliminazione di chunk. 
        /// </summary>
        /// <returns></returns>
        public bool IsModificabile
        {
            get
            {
                return ((StatoPagamento == StatoPagamentoRisposta.GRATUITA
                            || StatoPagamento == StatoPagamentoRisposta.A_PAGAMENTO
                            || StatoPagamento == StatoPagamentoRisposta.RICHIEDI_PAGAMENTO) && !IsInviata);
            }
        }

        /// <summary>
        /// Indica se la risposta prevede pagamenti.
        /// </summary>
        /// <returns></returns>
        public bool HasPagamenti
        {
            get
            {
                return (ImportoPagamento > 0);
            }
        }
        
        /// <summary>
        /// Indica se tutte le pratiche sono state lavorate.
        /// </summary>
        public bool IsCompleta
        {
            get
            {
                return (Richiesta.Pratiche.Count(q => !q.IsCompleta) == 0);
            }
        }

        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Risposta()
        {
            Init(null);
        }

        /// <summary>
        /// C.tor
        /// </summary>
        public Risposta(Richiesta r)
        {
            Init(r);
        }

        private void Init(Richiesta req)
        {
            Richiesta = req;
            //RichiestaId = r.Id;
            Mittente = false;
            ProtocolloOut = string.Empty;
            Oggetto = string.Empty;
            CodicePagamento = string.Empty;
            ImportoPagamento = 0;
            TestoObj = new Body();
            DataProtocollo = default(DateTime);
            Documenti = new List<Documento>();
            StatoPagamento = default(StatoPagamentoRisposta);
            DataInvio = default(DateTime);
            IsInviata = false;
        }

        #endregion

        #region "DomainObject members"

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  bool IsValid
        {
            get
            {
                //* todo..
                // verifica che non ci siano chunk collegati a una pratica X
                // maggiori del numero di procedure prevista da quella stessa pratica
                foreach (Pratica p in Richiesta.Pratiche)
                {
                    if (TestoObj.Chunks.Count(q => q.PraticaId == p.Id.ToString()) > p.NumeroProcedure)
                    {
                        return false;
                    }
                }
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

        #region "Public methods"

        /// <summary>
        /// Rivaluta importo e stato pagamento.
        /// </summary>
        public void EvaluateImporto()
        {
            // ricalcola l'importo
            ImportoPagamento = TestoObj.ChunkImporti();
            if (ImportoPagamento > 0)
            {
                //o richiedi pagamento o a pagamento
                if (Richiesta.HasPagamentiCalcolabili)
                    StatoPagamento = StatoPagamentoRisposta.RICHIEDI_PAGAMENTO;
                else
                    StatoPagamento = StatoPagamentoRisposta.A_PAGAMENTO;
            }
            else
            {
                StatoPagamento = StatoPagamentoRisposta.GRATUITA;
            }
        }

        #endregion

    }
}
