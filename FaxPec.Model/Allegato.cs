using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Com.Unisys.Pdf.ManagedModules;
using Com.Unisys.Pdf.ManagedModules.ModuleParts;

namespace FaxPec.Model
{
    /// <summary>
    /// Classe per la gestione dei file allegati a una richiesta.
    /// </summary>
    public class Allegato : IDomainObject
    {
        #region "Private Fields"

        /// <summary>
        /// Identificativo Richiesta di riferimento
        /// </summary>
        private decimal _RichiestaId;
        /// <summary>
        /// Identificativo Pratica di riferimento
        /// </summary>
        private decimal _PraticaId;

        #endregion

        #region "Properties"

        /// <summary>
        /// 
        /// </summary>
        public virtual decimal Id { get; set; }

        /// <summary>
        /// Richiesta di riferimento dell'Allegato.
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
                if (Richiesta != null)
                    _RichiestaId = Richiesta.Id;
                return _RichiestaId;
            }
            set { _RichiestaId = value; }
        }

        /// <summary>
        /// Pratica di riferimento dell'Allegato. 
        /// </summary>
        public virtual Pratica Pratica { get; set; }

        /// <summary>
        /// Accessor di servizio per l'oggetto collegato Pratica.
        /// </summary>
        public decimal PraticaId
        {
            //..
            get
            {
                if (Pratica != null)
                    if (Pratica.Id.HasValue)
                    { _PraticaId = Pratica.Id.GetValueOrDefault(); }
                return _PraticaId;
            }
            set { _PraticaId = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public byte[] FileBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Dati Xml contenuti nell'allegato (se codificato).
        /// </summary>
        ///  //COMMENTATO DA ALBERTO 08/10/2012
        //public string XmlData { get; set; }

        /// <summary>
        /// Codice Xml contenuto nell'allegato (se codificato).
        /// </summary>
        /// <remarks>
        /// equivale al sottotitolo.
        /// </remarks>
        ///  //COMMENTATO DA ALBERTO 08/10/2012
        //public string XmlCode { get; set; }

        /// <summary>
        /// Richiesta base contenuta nell'allegato (se codificato).
        /// </summary>
        public BaseRichiesta BaseRichiesta { get; set; }
        
        /// <summary>
        /// Numero di procedure contenute nell'allegato (se codificato).
        /// </summary>
        public virtual int NumeroProcedure { get; set; }

        /// <summary>
        /// Richiedente contenuto nell'allegato (se codificato).
        /// </summary>
        //COMMENTATO DA ALBERTO 08/10/2012
        //public virtual Richiedente Richiedente { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty
        {
            get
            {
                return (FileBytes == null
                        && string.IsNullOrEmpty(FileName));
                         //COMMENTATO DA ALBERTO 08/10/2012
                        //&& string.IsNullOrEmpty(XmlData)
                        //&& string.IsNullOrEmpty(XmlCode));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ///  //COMMENTATO DA ALBERTO 08/10/2012
        //public bool IsCodificato
        //{
        //    get
        //    {
        //        return (!string.IsNullOrEmpty(XmlData)
        //                && !string.IsNullOrEmpty(XmlCode));
        //    }
        //}

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
                //empty
                if (IsEmpty)
                    return true;
                //oppure dati minimi
                if (string.IsNullOrEmpty(FileName))
                    return false;
                if (FileBytes == null)
                    return false;
                if (FileBytes.Length == 0)
                    return false;

                return true;
            }
        }

        public bool IsPersistent
        {
            get { return (Id != (default(decimal))); }
        }

        #endregion
    }
}
