using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNet.Identity;
using AspNet.Identity.SQLServerProvider;
using System.Data.SqlClient;

namespace AspNet.Identity.SQLServerProvider.Repositories
{
     internal class UserLoginsRepository
    {
        private readonly SQLServerDataContext _db;

        public UserLoginsRepository(SQLServerDataContext oracleContext)
        {
            _db = oracleContext;
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "login", Justification = "Needless.")]
        public int Insert(IdentityUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            return _db.ExecuteNonQuery(
                "INSERT INTO userlogins (userid, loginprovider, providerkey) VALUES (@userid, @loginprovider, @providerkey)",
                new SqlParameter { ParameterName = "@userid", Value = user.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@loginprovider", Value = login.LoginProvider, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@providerkey", Value = login.ProviderKey, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        ////public int Delete(string userId)
        ////{
        ////    return _db.ExecuteScalarQuery<int>(
        ////       "DELETE FROM userlogins WHERE userid = :userid",
        ////       new SqlParameter { ParameterName = ":userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        ////}

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "login", Justification = "Needless.")]
        public int Delete(IdentityUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            return _db.ExecuteNonQuery(
                "DELETE FROM userlogins WHERE userid = @userid AND loginprovider = @loginprovider AND providerkey = @providerkey",
                new SqlParameter { ParameterName = "@userid", Value = user.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@loginprovider", Value = login.LoginProvider, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@providerkey", Value = login.ProviderKey, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "Needless.")]
        public string FindUserIdByLogin(UserLoginInfo userLogin)
        {
            if (userLogin == null)
            {
                throw new ArgumentNullException("userLogin");
            }

            return _db.ExecuteScalarQuery<string>(
               "SELECT userid FROM userlogins WHERE loginprovider = @loginprovider AND providerkey = @providerkey",
               new SqlParameter { ParameterName = ":loginprovider", Value = userLogin.LoginProvider, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
               new SqlParameter { ParameterName = ":providerkey", Value = userLogin.ProviderKey, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public IList<UserLoginInfo> FindByUserId(string userId)
        {
            var result = _db.ExecuteQuery(
               "SELECT * FROM [FAXPEC].[FAXPEC].[userlogins] WHERE userid = @userid",
               new SqlParameter { ParameterName = "@userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            return result.Rows.Cast<DataRow>().Select(row => new UserLoginInfo(row["loginprovider"].ToString(), row["providerkey"].ToString())).ToList();
        }
    }
}