using SendMail.Business.Data.QueryModel;

namespace System.Collections.Generic
{
    /// <summary>
    /// Classe statica che estende i metodi di caricamento dei Criterion.
    /// </summary>
    public static class ICollectionCriterionExtension
    {
        /// <summary>
        /// Delegate per le funzioni 'custom' di verifica degli input null value.
        /// </summary>
        /// <param name="input">input value da verificare</param>
        public delegate bool IsNullValueDel<T>(T input);

        /// <summary>
        /// Aggiunge un Criterion string solo se il valore di input è diverso da 'null'.
        /// </summary>
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
        /// Aggiunge un Criterion generic solo se il valore di input 
        /// viene verificato come diverso da 'null' tramite la funzione di verifica 'base'.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="coper">tipo di operatore</param>
        /// <param name="inputValue">valore di input</param>
        public static void Add<T>(this ICollection<Criterion> list, string propName, CriteriaOperator coper, T inputValue)
        {
            if (!IsNullValue(inputValue))
            {
                list.Add(new Criterion(propName, coper, inputValue));
            }
        }

        /// <summary>
        /// Aggiunge un Criterion generic solo se il valore di input 
        /// viene verificato come diverso da 'null' tramite la funzione di verifica 'custom' passata a parametro.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="coper">tipo di operatore</param>
        /// <param name="inputValue">valore di input</param>
        /// <param name="func">funzione per la valutazione degli input 'null'</param>
        public static void Add<T>(this ICollection<Criterion> list, string propName, CriteriaOperator coper, T inputValue, IsNullValueDel<T> func)
        {
            if (!func.Invoke(inputValue))
            {
                list.Add(new Criterion(propName, coper, inputValue));
            }
        }

        /// <summary>
        /// Aggiunge un Criterion string di uguaglianza che compara anche valori 'null'.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="inputValue">valore di input</param>
        public static void AddEqual(this ICollection<Criterion> list, string propName, string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                list.Add(new Criterion(propName, CriteriaOperator.IsNull, inputValue));
            else
                list.Add(new Criterion(propName, CriteriaOperator.Equal, inputValue));
        }

        /// <summary>
        /// Aggiunge un Criterion generic di uguaglianza che compara anche valori 'null' 
        /// tramite la funzione di verifica 'base'.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="inputValue">valore di input</param>
        public static void AddEqual<T>(this ICollection<Criterion> list, string propName, T inputValue)
        {
            if (IsNullValue(inputValue))
                list.Add(new Criterion(propName, CriteriaOperator.IsNull, inputValue));
            else
                list.Add(new Criterion(propName, CriteriaOperator.Equal, inputValue));
        }

        /// <summary>
        /// Aggiunge un Criterion generic di uguaglianza che compara anche valori 'null' 
        /// tramite la funzione di verifica 'custom' passata a parametro.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="inputValue">valore di input</param>
        /// <param name="func">funzione per la valutazione degli input 'null'</param>
        public static void AddEqual<T>(this ICollection<Criterion> list, string propName, T inputValue, IsNullValueDel<T> func)
        {
            if (func.Invoke(inputValue))
                list.Add(new Criterion(propName, CriteriaOperator.IsNull, inputValue));
            else
                list.Add(new Criterion(propName, CriteriaOperator.Equal, inputValue));
        }

        /// <summary>
        /// Aggiunge un Criterion string di disuguaglianza che compara anche il valore 'null'.
        /// </summary>
        /// <param name="propName">property name</param>
        /// <param name="inputValue">valore di input</param>
        public static void AddNotEqual(this ICollection<Criterion> list, string propName, string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                list.Add(new Criterion(propName, CriteriaOperator.IsNotNull, inputValue));
            else
                list.Add(new Criterion(propName, CriteriaOperator.NotEqual, inputValue));
        }

        /// <summary>
        /// Aggiunge un Criterion generic di disuguaglianza che compara anche valori 'null' 
        /// tramite la funzione di verifica 'base'.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="inputValue">valore di input</param>
        public static void AddNotEqual<T>(this ICollection<Criterion> list, string propName, T inputValue)
        {
            if (IsNullValue(inputValue))
                list.Add(new Criterion(propName, CriteriaOperator.IsNotNull, inputValue));
            else
                list.Add(new Criterion(propName, CriteriaOperator.NotEqual, inputValue));
        }

        /// <summary>
        /// Aggiunge un Criterion generic di disuguaglianza che compara anche valori 'null' 
        /// tramite la funzione di verifica 'custom' passata a parametro.
        /// </summary>
        /// <typeparam name="T">tipo della property</typeparam>
        /// <param name="propName">property name</param>
        /// <param name="inputValue">valore di input</param>
        /// <param name="func">funzione per la valutazione degli input 'null'</param>
        public static void AddNotEqual<T>(this ICollection<Criterion> list, string propName, T inputValue, IsNullValueDel<T> func)
        {
            if (func.Invoke(inputValue))
                list.Add(new Criterion(propName, CriteriaOperator.IsNotNull, inputValue));
            else
                list.Add(new Criterion(propName, CriteriaOperator.NotEqual, inputValue));
        }

        /// <summary>
        /// Effettua il parsing di un criterio di ricerca basato su un range di date
        /// aggiungendolo all'oggetto Query soltanto se valorizzato.
        /// </summary>
        /// <param name="propName">property name</param>
        /// <param name="lowBound">input limite inferiore</param>
        /// <param name="upBound">input limite superiore</param>
        public static void AddDateRange(this ICollection<Criterion> list, string propName, string lowBound, string upBound)
        {
            DateTime dt1 = QueryHelper.ParseDateTime(lowBound);
            DateTime dt2 = QueryHelper.ParseDateTime(upBound);

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

        /// <summary>
        /// Metodo base di verifica degli input 'null'.
        /// </summary>
        /// <typeparam name="T">tipo di input</typeparam>
        /// <param name="inputValue">valore di input</param>
        /// <remarks>
        /// non utilizzabile con le struct 'custom' (restituisce sempre true)
        /// </remarks>
        /// <returns></returns>
        private static bool IsNullValue<T>(T inputValue)
        {
            T temp = default(T);
            if (temp == null)   //reference type
            {
                return (inputValue == null);
            }
            else   //value type
            {
                return (temp.ToString() == inputValue.ToString());
            }
        }

    }
}
