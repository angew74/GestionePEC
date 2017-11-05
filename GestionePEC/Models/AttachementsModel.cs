using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class AttachementsModel
    {

        public List<ViewAttachement> AttachementsList { get; set; }

        public string message { get; set; }
        public string success { get; set; }
        public string Totale { get; set; }

    }
}