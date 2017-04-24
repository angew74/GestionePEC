using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Delta.Logging.Context;

namespace GestionePEC.Models
{
    public class LogCodesModel
    {
        internal string message;
        internal string success;

        public List<LOG_LOG_CODES> ElencoLogCodes { get; internal set; }
        public int Totale { get; internal set; }
    }
}