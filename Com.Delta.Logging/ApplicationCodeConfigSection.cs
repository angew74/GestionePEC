using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Com.Delta.Logging
{
    /// <summary>
    /// Classe che implementa il Configuration Section. Essa serve per consentire di inserire una ConfigSection personalizzata
    /// nei files di configurazione delle applicazioni. Le dll referenziate potranno andare a leggere in punto ben preciso del config il riferimento all'applicazione 
    /// che deve risultare come agente nel log.
    /// </summary>
    public class ApplicationCodeConfigSection : ConfigurationSection
    {
        /// <summary>
        /// Proprietà appcode che passa il valore numerico che identifica l'applicazione
        /// </summary>
        [ConfigurationProperty("appCode", IsRequired = true)]
        public String AppCode
        {
            get
            { return (String)this["appCode"]; }
            set
            { this["appCode"] = value; }
        }

        /// <summary>
        /// Proprietà appDescr che passa la descrizione che identifica l'applicazione
        /// </summary>
        [ConfigurationProperty("appDescr", IsRequired = true)]
        public String AppDescr
        {
            get
            { return (String)this["appDescr"]; }
            set
            { this["appDescr"] = value; }
        }
    }
}
