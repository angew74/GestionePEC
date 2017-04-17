using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Models
{
    public class DTOFileUploadResult
    {
        public bool success { get; set; }
        public string FileName { get; set; }
        public string errormessage { get; set; }
        public string message { get; set; }
        public byte[] CustomData { get; set; }


        /// <summary>
        /// Ctor.
        /// </summary>
        public DTOFileUploadResult()
        {
            success = false;
            FileName = "";
            errormessage = "";
            message = "";
            CustomData = null;
            PageCount = string.Empty;
            Extension = string.Empty;

        }

        public string PageCount { get; set; }
        public string Extension { get; internal set; }
    }
}