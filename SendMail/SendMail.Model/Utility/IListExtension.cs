using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace SendMail.Model
{
    /// <summary>
    /// Classe statica che estende i metodi dell'IList 
    /// per la gestione degli indici delle Pratiche allegate alla Richiesta.
    /// </summary>
    public static class IListExtension
    {
        #region "Extension methods"

        /// <summary>
        /// Inserisce una pratica in collezione gestendone l'indice.
        /// </summary>
        /// <param name="pList">collezione di pratiche</param>
        /// <param name="p">pratica da inserire</param>
        public static void InsertEntity<T>(this IList<T> pList, T p)
        {

            //TODO  riscrivere come gneric quando lorenzo ci spiega l'uso 
            //if (p.Indice == -1)
            //    pList.Add(p);
            //else if (pList.ElementAtOrDefault(p.Indice) != null)
            //{
            //    pList.RemoveAt(p.Indice);
            //    pList.Insert(p.Indice, p);
            //}
            ////todo.. exception?
        }

        #endregion
    }
}
