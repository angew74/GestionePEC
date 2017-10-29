using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendMail.Model;

namespace GestionePEC.Models
{
    public class TitoliModel
    {
        public string Totale { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public List<SendMail.Model.Titolo> TitoliList { get; set; }
        public List<SottoTitolo> SottoTitoliList { get; internal set; }
    }
}