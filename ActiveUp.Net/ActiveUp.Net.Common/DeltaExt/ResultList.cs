using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveUp.Net.Mail.DeltaExt
{
    /// <summary>
    /// Classe wrapper per la gestione delle collezioni paginate.
    /// </summary>
    /// <typeparam name="T">tipo degli elementi in collezione</typeparam>
    public class ResultList<T>
    {
        /// <summary>
        /// Ordinale del primo elemento restituito.
        /// </summary>
        public virtual int Da { get; set; }
        /// <summary>
        /// Numero di elementi restituiti.
        /// </summary>
        public virtual int Per { get; set; }
        /// <summary>
        /// Numero totale degli elementi trovati (non restituiti).
        /// </summary>
        public virtual int Totale { get; set; }
        /// <summary>
        /// Collezione degli elementi restituiti.
        /// </summary>
        public virtual ICollection<T> List { get; set; }

        /// <summary>
        /// C.tor
        /// </summary>
        public ResultList()
        {
            this.List = new List<T>();
        }

        /// <summary>
        /// C.tor
        /// </summary>
        /// <param name="da"></param>
        /// <param name="per"></param>
        /// <param name="list"></param>
        /// <param name="tot"></param>
        public ResultList(int da, int per, ICollection<T> list, int tot)
        {
            Da = da;
            Per = per;
            Totale = tot;
            List = list;
        }
    }
}
