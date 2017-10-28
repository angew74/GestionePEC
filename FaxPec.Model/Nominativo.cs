using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe base per la gestione dei Nominativi.
    /// </summary>
    public class Nominativo : IDomainObject
    {
        #region "Private fields"

        private bool _ToRubrica = true;

        #endregion

        #region "Properties"

        /// <summary>
        /// Id di riferimento.
        /// </summary>
        public virtual decimal Id { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public virtual string Cognome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CodiceFiscale { get; set; }

        /// <summary>
        /// Tipo dell'ente o dell'azienda o della persona
        /// </summary>
        public virtual SendMail.Model.EntitaType TipoEntita { get; set; }

        /// <summary>
        /// Denominazione dell'azienda o dell'ente.
        /// </summary>
        public virtual string RagioneSociale { get; set; }

        /// <summary>
        /// Partita IVA dell'azienda.
        /// </summary>
        public virtual string PartitaIVA { get; set; }

        /// <summary>
        /// Ufficio specifico dell'ente.
        /// </summary>
        public virtual string Ufficio { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Mail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Fax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Telefono { get; set; }

        /// <summary>
        /// Indirizzo del nominativo.
        /// </summary>
        public virtual Indirizzo Recapito { get; set; }

        /// <summary>
        /// Indica se il nominativo è 'certificato' e quindi non modificabile.
        /// </summary>
        public virtual bool Certificato { get; set; }

        /// <summary>
        /// Indica se il nominativo è 'preinserito'.
        /// </summary>
        public virtual bool PreInserito { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Note { get; set; }

        /// <summary>
        /// Indica se il Nominativo deve avere un riferimento in Rubrica o meno.
        /// </summary>
        public bool ToRubrica 
        {
            get { return _ToRubrica; }
            set 
            { 
                _ToRubrica = value;
                if (value == false) Id = default(decimal);
            } 
        }

        /// <summary>
        /// Accessor di servizio per la formattazione in visualizzazione del nominativo.
        /// </summary>
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(RagioneSociale))
                    return RagioneSociale;
                else
                    return String.Format("{0} {1}", Nome, Cognome);
            }
        }
        
        #endregion

        #region "C.tor"

        /// <summary>
        /// C.tor
        /// </summary>
        public Nominativo()
        {
            Init();
        }

        public Nominativo(decimal Id, string Cognome,string Nome , string CodiceFiscale , string PartitaIVA, SendMail.Model.EntitaType TipoEntita, string RagioneSociale , string Ufficio , string Mail , string Fax , string Telefono, Indirizzo Indirizzo)
        {
            this.Cognome = Cognome;
            this.Nome = Nome;
            this.CodiceFiscale = CodiceFiscale;
            this.PartitaIVA = PartitaIVA;
            this.TipoEntita = TipoEntita;
            this.RagioneSociale = RagioneSociale;
            this.Ufficio = Ufficio;
            this.Mail = Mail;
            this.Fax = Fax ;
            this.Telefono = Telefono;
            this.Recapito = Indirizzo;
            this.Id = Id;
        }

        
        
        /// <summary>
        /// Inizializza le properties.
        /// </summary>
        private void Init()
        {
            Cognome = string.Empty;
            Nome = string.Empty;
            CodiceFiscale = string.Empty;
            PartitaIVA = string.Empty;
            TipoEntita = default(SendMail.Model.EntitaType);
            RagioneSociale = string.Empty;
            Ufficio = string.Empty;
            Mail = string.Empty;
            Fax = string.Empty;
            Telefono = string.Empty;
            Recapito = new Indirizzo();
            ToRubrica = true;
        } 
        
        #endregion

        #region "DomainObject members"

        /// <summary>
        /// Verifica la presenza dei dati minimi dell'oggetto.
        /// </summary>
        /// <returns></returns>
        public  bool IsValid
        {
            get
            {
                //todo.. RIVEDERE!
                //verifica sui dati 'personali'
                if ((string.IsNullOrEmpty(Cognome) || string.IsNullOrEmpty(Nome))
                    && string.IsNullOrEmpty(RagioneSociale))
                    return false;
                //COMMENTATO * NUOVA TIPOLOGIA 'A MANO'
                //verifica sui dati 'di contatto'
                //if (string.IsNullOrEmpty(Mail) &&
                //    string.IsNullOrEmpty(Fax) &&
                //    !Recapito.IsValid)
                //    return false;

                //tutto ok
                return true;
            }
        }

        /// <summary>
        /// Verifica che il Nominativo sia "virtualmente" persistente in Rubrica.
        /// </summary>
        /// <returns></returns>
        public  bool IsPersistent
        {
            get { return Id != (default(int)); }
        } 

        #endregion

        #region "Public methods"

        /// <summary>
        /// Restituisce le informazioni relative al canale di comunicazione passato.
        /// </summary>
        /// <param name="canale">tipo di canale richiesto</param>
        public string GetInfoCanale(TipoCanale canale)
        {
            string info = string.Empty;
            switch (canale)
            {
                case TipoCanale.UNKNOWN:
                    break;
                case TipoCanale.FAX:
                    info = String.Format("Fax: {0}", Fax);
                    break;
                case TipoCanale.MAIL:
                    info = String.Format("Mail: {0}", Mail);
                    break;
                case TipoCanale.POSTA:
                    info = "Posta";
                    break;
                case TipoCanale.AMANO:
                    info = "A Mano";
                    break;
                default:
                    break;
            }
            return info;
        }

        #endregion

    }
}
