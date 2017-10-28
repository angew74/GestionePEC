using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model
{
    /// <summary>
    /// 
    /// </summary>
    public struct EndPoint
    {
        /// <summary>
        /// 
        /// </summary>
        public string Indirizzo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Protocollo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Porta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SSL { get; set; }
    }
}
