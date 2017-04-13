using System;
using System.Collections.Generic;
using System.Text;

namespace SendMail.Business.Data.QueryModel
{
    /// <summary>
    /// Classe per la gestione della paginazione. 
    /// </summary>
    public class Pager
    {
        /// <summary>
        /// dimensione del subset
        /// </summary>
        public int Da { get; set; }

        /// <summary>
        /// dimensione del subset
        /// </summary>
        public int Per { get; set; }

        
        /// <summary>
        /// dimensione del subset
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// indice del subset
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// C.tor OBSOLETO
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        //public Pager(int size, int index)
        //{
        //    Size = size;
        //    Index = index;
        //}

        /// <summary>
        /// C.tor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        public Pager(int da, int per)
        {
            Da = da;
            Per = per;
        }

        /// <summary>
        /// Margine inferiore incluso del subset.
        /// </summary>
        public int LowerBound
        {
            //get { return (Size * Index); }
            get { return Da; }
        }

        /// <summary>
        /// Margine superiore incluso del subset.
        /// </summary>
        public int UpperBound
        {
            //get { return (Size * (Index + 1)); }
            get { return (Da + Per - 1); }
        }
    }
}
