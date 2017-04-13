using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet.Identity.SQLServerProvider.Repositories
{
    using SQLServerProvider;
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Security.Claims;


    internal class UserClaimsRepository
    {
        private readonly SQLServerDataContext _db;

        public UserClaimsRepository(SQLServerDataContext oracleContext)
        {
            _db = oracleContext;
        }

        public int Insert(Claim userClaim, string userId)
        {
            if (userClaim == null)
            {
                throw new ArgumentNullException("userClaim");
            }

            return _db.ExecuteNonQuery(
                "INSERT INTO userclaims (userid, claimtype, claimvalue) VALUES (@userId, @type, @value)",
                new SqlParameter { ParameterName = "@userId", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@type", Value = userClaim.Type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@value", Value = userClaim.Value, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        ////public int Delete(string userId)
        ////{
        ////    return _db.ExecuteScalarQuery<int>(
        ////        "DELETE FROM userclaims WHERE userid = :userid",
        ////        new SqlParameter { ParameterName = ":userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        ////}

        public int Delete(IdentityUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            return _db.ExecuteNonQuery(
                "DELETE FROM userclaims WHERE userid = @userid AND claimtype = @type AND claimvalue = @value",
                new SqlParameter { ParameterName = "@userid", Value = user.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@type", Value = claim.Type, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@value", Value = claim.Value, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public ClaimsIdentity FindByUserId(string userId)
        {
            var result = _db.ExecuteQuery(
                "SELECT * FROM userclaims WHERE userid = @userid",
                new SqlParameter { ParameterName = "@userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            var claims = new ClaimsIdentity();

            foreach (var row in result.Rows.Cast<DataRow>())
            {
                claims.AddClaim(new Claim(row["claimtype"].ToString(), row["claimvalue"].ToString()));
            }

            return claims;
        }
    }
}