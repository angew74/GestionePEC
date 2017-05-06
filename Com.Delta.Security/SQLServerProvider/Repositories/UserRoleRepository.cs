using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet.Identity.SQLServerProvider.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Data.SqlClient;
    using SQLServerProvider;

    internal class UserRolesRepository
    {
        private readonly SQLServerDataContext _db;

        public UserRolesRepository(SQLServerDataContext database)
        {
            _db = database;
        }

        public int Insert(IdentityUser user, string roleId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return _db.ExecuteNonQuery(
                "INSERT INTO [FAXPEC].[FAXPEC].[userroles] (userid, roleid) values (@userid, @roleid)",
                new SqlParameter { ParameterName = "@userid", Value = user.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@roleid", Value = roleId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        //public int Delete(string userId)
        //{
        //    return _db.ExecuteScalarQuery<int>(
        //       "DELETE FROM userroles WHERE userid = :userid",
        //       new SqlParameter { ParameterName = ":userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        //}

        public int Delete(string userId, string roleId)
        {
            return _db.ExecuteNonQuery(
               "DELETE FROM [FAXPEC].[FAXPEC].[userroles] WHERE userid = @userid AND roleid = @roleid",
               new SqlParameter { ParameterName = "@userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
               new SqlParameter { ParameterName = "@roleid", Value = roleId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public int Delete(IdentityUser user, string roleId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (roleId.HasNoValue())
            {
                throw new ArgumentNullException("roleId");
            }

            return Delete(user.Id, roleId);
        }

        public IList<string> FindByUserId(string userId)
        {
            var result = _db.ExecuteQuery(
                "SELECT roles.name FROM  [FAXPEC].[FAXPEC].[userroles],  [FAXPEC].[FAXPEC].[roles] WHERE userroles.userid = @userid AND userroles.roleid = roles.id",
                new SqlParameter { ParameterName = "@userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            return result.Rows.Cast<DataRow>().Select(row => row["name"].ToString()).ToList();
        }
    }
}