using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Unisys.Pdf.ManagedModules.ModuleParts
{
    /// <summary>
    /// Classe che espone i dati "più comuni" relativi alla richiesta.
    /// Dipendentemente dal tipo specifico di richiesta i dati possono
    /// essere presenti o meno.
    /// </summary>
    public class DatiComuniRichiesta
    {
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
        /// 
        /// </summary>
        public virtual string GiornoNascita { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string MeseNascita { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AnnoNascita { get; set; }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public virtual string Sesso { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string TestoLibero { get; set; }

        /// <summary>
        /// Proprietà in sola lettura che riepiloga i dati.
        /// </summary>
        public string FullText
        {
            get
            {
                string t = "";
                t += (!string.IsNullOrEmpty(Cognome)) ? Cognome + " " : "";
                t += (!string.IsNullOrEmpty(Nome)) ? Nome + " " : "";
                t += (!string.IsNullOrEmpty(CodiceFiscale)) ? CodiceFiscale + " " : "";
                t += (!string.IsNullOrEmpty(DataDiNascita)) ? DataDiNascita + " " : "";
                t += (!string.IsNullOrEmpty(TestoLibero)) ? TestoLibero : "";
                return t.Trim();
            }
        }

        /// <summary>
        /// Proprietà in sola lettura che indica se i dati sono empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (string.IsNullOrEmpty(Cognome) 
                            && string.IsNullOrEmpty(Nome)
                            && string.IsNullOrEmpty(CodiceFiscale)
                            && string.IsNullOrEmpty(DataDiNascita));
            }
        }
    }
}
