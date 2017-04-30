using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SendMail.Model;

namespace GestionePEC.Models
{
    public class FoldersSendersModel
    {
        internal string message;
        internal string success;

        public FolderType[] FoldersList { get; internal set; }
        public string Totale { get; internal set; }
    }
}