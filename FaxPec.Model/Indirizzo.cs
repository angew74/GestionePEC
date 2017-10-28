using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione degli indirizzi.
    /// </summary>
    public class Indirizzo : IDomainObject
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Stato { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Provincia { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Citta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Sedime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Via { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int Numero { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Lettera { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CAP { get; set; }

        /// <summary>
        /// C.tor
        /// </summary>
        public Indirizzo()
        {
            Init();
        }

        /// <summary>
        /// Inizializza i valori.
        /// </summary>
        private void Init()
        {
            Stato = string.Empty;
            Provincia = string.Empty;
            Citta = string.Empty;
            CAP = string.Empty;
            Sedime = string.Empty;
            Via = string.Empty;
            Lettera = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  bool IsValid
        {
            get
            {
                // verifica i dati minimi relativi all'indirizzo 
                if (string.IsNullOrEmpty(Stato))
                    return false;
                if (Stato.ToUpper() == "ITALIA" && string.IsNullOrEmpty(Provincia))
                    return false;
                if (string.IsNullOrEmpty(Citta))
                    return false;
                if (string.IsNullOrEmpty(CAP))
                    return false;              
                return true;                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  bool IsPersistent
        {
            get { return false; }
        }
    }
}
