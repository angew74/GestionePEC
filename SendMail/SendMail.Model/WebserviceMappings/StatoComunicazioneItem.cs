using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model.WebserviceMappings
{
    public class StatoComunicazioneItem
    {
        public string Id { get; set; }
        public string DataInserimento { get; set; }
        public string StatoInvio { get; set; }
        public string SottoTitolo { get; set; }
        public string SottoTitoloDescr { get; set; }
    }
}
