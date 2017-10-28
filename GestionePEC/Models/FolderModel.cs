using ActiveUp.Net.Common.DeltaExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class FolderModel
    {
        public string Totale { get; set; }
        public string success { get; set; }
        public string message { get; set; }

        public List<Folder> ListFolders { get; set; }
    }
}