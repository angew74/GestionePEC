using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Delta.Logging.Context;

namespace GestionePEC.Models
{
    public class AppCodesModel
    {
        internal string message;
        internal string success;

        public List<LOG_APP_CODES> ElencoAppCodes { get; internal set; }
        public object Totale { get; internal set; }
    }
}