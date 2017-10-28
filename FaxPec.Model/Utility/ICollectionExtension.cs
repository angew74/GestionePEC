using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace FaxPec.Model
{
    ///// <summary>
    ///// Classe statica che estende i metodi dell'ICollection 
    ///// per la gestione degli indici delle Pratiche allegate alla Richiesta.
    ///// </summary>
    //public static class ICollectionExtension
    //{
    //    #region "Extension methods"

    //    /// <summary>
    //    /// Aggiunge una pratica in collezione impostandone l'indice.
    //    /// </summary>
    //    /// <param name="pCollection">collezione di pratiche</param>
    //    /// <param name="pToAdd">pratica da aggiungere</param>
    //    public static void AddPratica(this ICollection<Pratica> pCollection, Pratica pToAdd)
    //    {
    //        int ix = 0;
    //        if (pCollection.Count > 0)
    //        {
    //            ix = pCollection.Max(q => q.Indice) + 1;
    //        }
    //        pToAdd.Indice = ix;
    //        pCollection.Add(pToAdd);
    //    }

    //    /// <summary>
    //    /// Sostituisce una pratica in collezione con la sua versione aggiornata.
    //    /// </summary>
    //    /// <param name="pCollection">collezione di pratiche</param>
    //    /// <param name="pToReplace">pratica aggiornata</param>
    //    public static ICollection<Pratica> ReplacePratica(this ICollection<Pratica> pCollection, Pratica pToReplace)
    //    {
    //        pCollection.Remove(pCollection.Single(q => q.Indice == pToReplace.Indice));
    //        pCollection.Add(pToReplace);
    //        // ritorna la collezione ordinandola per indice
    //        return pCollection.OrderBy(o => o.Indice).ToList();
    //    }

    //    #endregion
    //}
}
