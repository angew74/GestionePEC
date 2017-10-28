using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.QueryModel
{
    /// <summary>
    /// Classe statica che estende i metodi di caricamento dei Criterion.
    /// </summary>
    public static class ICollectionCriterionExtension
    {
        /// <summary>
        /// Delegate per la funzione di parsing inputvalue/data.
        /// </summary>
        /// <param name="input">input value da convertire</param>
        public delegate T ParseInputDel<T>(string input);

        /// <summary>
        /// Aggiunge un criterion stringa solo se correttamente valorizzato.
        /// </summary>
        /// <param name="list">icollection estesa</param>
        /// <param name="propName">property name</param>
        /// <param name="coper">tipo di operatore</param>
        /// <param name="inputValue">valore di input</param>
        public static void Add(this ICollection<Criterion> list, string propName, CriteriaOperator coper, string inputValue)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                list.Add(new Criterion(propName, coper, inputValue));
            }
        }

        /// <summary>
        /// Aggiunge un criterion solo se correttamente valorizzato.
        /// </summary>
        /// <typeparam name="T">tipo di dato</typeparam>
        /// <param name="list">icollection estesa</param>
        /// <param name="propName">property name</param>
        /// <param name="coper">tipo di operatore</param>
        /// <param name="inputValue">valore di input</param>
        /// <param name="func">funzione di conversione</param>
        public static void Add<T>(this ICollection<Criterion> list, string propName, CriteriaOperator coper, string inputValue, ParseInputDel<T> func)
        {
            if (!string.IsNullOrEmpty(inputValue))
            {
                list.Add(new Criterion(propName, coper, func.Invoke(inputValue)));
            }
        }

        /// <summary>
        /// Effettua il parsing di un criterio di ricerca basato su un range di date
        /// aggiungendolo all'oggetto Query soltanto se valorizzato.
        /// </summary>
        /// <param name="list">icollection estesa</param>
        /// <param name="propName">property name</param>
        /// <param name="lowBound">input limite inferiore</param>
        /// <param name="upBound">input limite superiore</param>
        public static void AddDateRange(this ICollection<Criterion> list, string propName, string lowBound, string upBound)
        {
            DateTime dt1 = ModelHelper.ParseDateTime(lowBound);
            DateTime dt2 = ModelHelper.ParseDateTime(upBound);

            if (dt1 != default(DateTime))
            {
                if (dt2 != default(DateTime))
                {
                    list.Add(new Criterion(propName, CriteriaOperator.GreaterThanOrEqual, dt1));
                    list.Add(new Criterion(propName, CriteriaOperator.LesserThanOrEqual, dt2.AddHours(23).AddMinutes(59).AddSeconds(59)));
                }
                else
                {
                    list.Add(new Criterion(propName, CriteriaOperator.GreaterThanOrEqual, dt1));
                    list.Add(new Criterion(propName, CriteriaOperator.LesserThanOrEqual, dt1.AddHours(23).AddMinutes(59).AddSeconds(59)));
                }
            }
        }
    }
}
