using System;
using System.Collections.Generic;
using System.Text;

namespace FaxPec.Model.QueryModel
{
    /// <summary>
    /// Classe helper del query model.
    /// </summary>
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
            DateTime dt1 = Model.ModelHelper.ParseDateTime(date1);
            DateTime dt2 = Model.ModelHelper.ParseDateTime(date2);

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
    }
}
