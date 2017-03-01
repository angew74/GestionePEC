using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Com.Delta.Data.QueryModel
{
    /// <summary>
    /// Classe per la gestione delle query 'astratte'.
    /// </summary>
    public sealed class Query
    {
        private List<Criterion> criteria = new List<Criterion>();
        private QueryOperator @operator;
        private IList<Query> subQueries = new List<Query>();
        private IList<OrderClause> orderClauses = new List<OrderClause>();

        public IList<Criterion> Criteria
        {
            get
            {
                return criteria;
            }
        }

        public QueryOperator Operator
        {
            get
            {
                return @operator;
            }
            set
            {
                @operator = value;
            }
        }

        public IList<Query> SubQueries
        {
            get
            {
                return subQueries;
            }
        }

        public IList<OrderClause> OrderClauses
        {
            get
            {
                return orderClauses;
            }
        }

        /// <summary>
        /// Verifica che sia stata costruita una query formalmente valida.
        /// </summary>
        public bool IsValidQuery
        {
            get
            {
                return Criteria.Count > 0;
            }
        }

    }

    //*******************************************************

    public interface IClause
    {

    }

    public class SingleClause : Criterion, IClause
    {

    }

    public class CombinedClause : IClause
    {
        public QueryOperator ClauseType { get; set; }
        public IEnumerable<IClause> SubClauses { get; set; }
    }

    public sealed class QueryCmp
    {
        public IClause WhereClause { get; set; }
        public IEnumerable<Query> SubQuery { get; set; }
    }
}