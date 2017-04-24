﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Com.Delta.Logging.Context;

namespace GestionePEC.Models
{
    public class LogErrorsModel
    {
        internal string message;
        internal string success;

        public List<LOG_APP_ERRORS> ErrorLogsList { get; internal set; }
        public string Totale { get; internal set; }
    }
}