using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.QueryModel
{
    /// <summary>
    /// Classe per la gestione della paginazione. 
    /// </summary>
    public class Pager
    {
        /// <summary>
        /// dimensione del subset
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// indice del subset
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// C.tor
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        public Pager(int size, int index)
        {
            Size = size;
            Index = index;
        }
        
        /// <summary>
        /// Margine inferiore escluso del subset.
        /// </summary>
        public int LowerBound
        {
            get { return (Size * Index); }
        }

        /// <summary>
        /// Margine superiore incluso del subset.
        /// </summary>
        public int UpperBound
        {
            get { return (Size * (Index + 1)); }
        }
    }
}
