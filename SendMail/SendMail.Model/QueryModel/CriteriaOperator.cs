using System;
using System.Collections.Generic;
using System.Text;

namespace SendMail.Business.Data.QueryModel
{
    /// <summary>
    /// Specifica l'operatore di un criterio di interrogazione.
    /// </summary>
    public enum CriteriaOperator
    {
        /// <summary>
        /// An operator that represents an "equal" criterion.
        /// </summary>
        Equal,
        /// <summary>
        /// An operator that represents a "not equal" criterion.
        /// </summary>
        NotEqual,
        /// <summary>
        /// An operator that represents a "greater than" criterion.
        /// </summary>
        GreaterThan,
        /// <summary>
        /// An operator that represents a "lesser than" criterion.
        /// </summary>
        LesserThan,
        /// <summary>
        /// An operator that represents a "greater than or equal" criterion.
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// An operator that represents a "lesser than or equal" criterion.
        /// </summary>
        LesserThanOrEqual,
        /// <summary>
        /// An operator that represents a "like" criterion.
        /// </summary>
        Like,
        /// <summary>
        /// An operator that represents a "not like" criterion.
        /// </summary>
        NotLike,
        /// <summary>
        /// An operator that represents a "starts with" criterion.
        /// </summary>
        StartsWith,
        /// <summary>
        /// An operator that represents a "ends with" criterion.
        /// </summary>
        EndsWith,
        /// <summary>
        /// An operator that represents a "is null" criterion.
        /// </summary>
        IsNull,
        /// <summary>
        /// An operator that represents a "is not null" criterion.
        /// </summary>
        IsNotNull
    }
}
