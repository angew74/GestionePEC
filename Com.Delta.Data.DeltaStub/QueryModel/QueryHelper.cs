using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Delta.Data.QueryModel
{
    public class QueryHelper
    {
        #region "Criteria"

        /// <summary>
        /// Effettua il parsing di un criterio di ricerca basato su un range di date
        /// aggiungendolo all'oggetto Query soltanto se valorizzato.
        /// </summary>
        /// <param name="q">oggetto Query</param>
        /// <param name="propName">property name</param>
        /// <param name="date1">data inizio range</param>
        /// <param name="date2">eventuale data fine range</param>
        public static void ParseRangeDateCriteria(Query q, string propName, string date1, string date2)
        {
            DateTime dt1 = QueryHelper.ParseDateTime(date1);
            DateTime dt2 = QueryHelper.ParseDateTime(date2);

            if (dt1 != default(DateTime))
            {
                if (dt2 != default(DateTime))
                {
                    q.Criteria.Add(new Criterion(propName, CriteriaOperator.GreaterThanOrEqual, dt1));
                    q.Criteria.Add(new Criterion(propName, CriteriaOperator.LesserThanOrEqual, dt2.AddHours(23).AddMinutes(59).AddSeconds(59)));
                }
                else
                {
                    q.Criteria.Add(new Criterion(propName, CriteriaOperator.GreaterThanOrEqual, dt1));
                    q.Criteria.Add(new Criterion(propName, CriteriaOperator.LesserThanOrEqual, dt1.AddHours(23).AddMinutes(59).AddSeconds(59)));
                }
            }
        }

        #endregion

        #region "Parsing"

        /*
         * COPIATO DA MODELHELPER.
         * ANDREBBE SPOSTATO IN UNO STRATO 'FRAMEWORK' ACCESSIBILE DA TUTTO IL SISTEMA.
        */

        /// <summary>
        /// Effettua il parse di un valore enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value)    //where T : Enum
        {
            T result = default(T);
            if (Enum.IsDefined(typeof(T), value))
            {
                result = (T)(Enum.Parse(typeof(T), value));
            }
            return result;
        }

        /// <summary>
        ///  Effettua il parse di un valore int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ParseInt(string value)
        {
            int iOut = default(int);
            int.TryParse(value, out iOut);
            return iOut;
        }

        /// <summary>
        ///  Effettua il parse di un valore datetime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string value)
        {
            DateTime dtOut = default(DateTime);
            DateTime.TryParse(value, out dtOut);
            return dtOut;
        }
        #endregion
    }
}
