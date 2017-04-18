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
    using log4net;
    using Com.Delta.Logging.Errors;
    using Com.Delta.Logging;
    using System.Data.SqlClient;
    using SQLServerProvider;
    using System.Threading.Tasks;

    internal class UserRepository
    {
        private static readonly ILog _log = LogManager.GetLogger("UserRepository");
        private readonly SQLServerDataContext _db;

        public UserRepository(SQLServerDataContext sqlServerContext)
        {
            _db = sqlServerContext;
        }

        public int Insert(IdentityUser user)
        {
            int val = 0;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            try
            {
                val = _db.ExecuteNonQuery(
                    "INSERT INTO [FAXPEC].[FAXPEC].[USERS] ([username], [passwordhash], [securitystamp]) VALUES (@name, @passwordhash, @securitystamp)",
                  //  new SqlParameter { ParameterName = "@id", Value = user.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@name", Value = user.UserName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@passwordhash", Value = user.PasswordHash, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                    new SqlParameter { ParameterName = "@securitystamp", Value = user.SecurityStamp, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
           
            }
            catch(Exception ex)
            {
                        
                ManagedException m = new ManagedException("Errore nell'inserimento per username - " + user.UserName + " dettagli: " + ex.Message, "REP01","INSERT",string.Empty,null);
                ErrorLogInfo error = new ErrorLogInfo(m);
                error.loggingTime = System.DateTime.Now;                
                _log.Error(error);
                throw (m);
            }

            return val;
        }

        internal List<IdentityUser> GetAll()
        {
            List<IdentityUser> listUsers = null;
            var users = _db.ExecuteQuery(@"Select ID, USERNAME,PASSWORDHASH,SECURITYSTAMP FROM [FAXPEC].[FAXPEC].[USERS]");
            if (users.Rows.Count > 0)
            {
                listUsers = new List<IdentityUser>();
                foreach (DataRow r in users.Rows)
                {
                    IdentityUser user = new IdentityUser()
                    {
                        Id = r["ID"].ToString(),
                        UserName = r["USERNAME"].ToString(),
                        PasswordHash = r["PASSWORDHASH"].ToString(),
                        SecurityStamp = r["SECURITYSTAMP"].ToString()
                    };
                    listUsers.Add(user);
                }
            }
            return listUsers;
        }

        public int Update(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return _db.ExecuteNonQuery(
                "UPDATE users SET username = @userName, passwordhash = @passwordhash, securitystamp = @securitystamp WHERE id = @userid",
                new SqlParameter { ParameterName = "@username", Value = user.UserName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@passwordhash", Value = user.PasswordHash, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@securitystamp", Value = user.SecurityStamp, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@userid", Value = user.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public int Delete(string userId)
        {
            return _db.ExecuteNonQuery(
                "DELETE FROM users WHERE id = @userid",
                new SqlParameter { ParameterName = "@userid", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public int Delete(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Delete(user.Id);
        }

        ////public string GetUserName(string userId)
        ////{
        ////    return _db.ExecuteScalarQuery<string>(
        ////        "SELECT name FROM users WHERE id = :id",
        ////        new SqlParameter { ParameterName = ":id", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        ////}

        ////public string GetUserId(string userName)
        ////{
        ////    return _db.ExecuteScalarQuery<string>(
        ////       "SELECT id FROM users WHERE username = :name",
        ////       new SqlParameter { ParameterName = ":name", Value = userName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        ////}

        public IdentityUser GetUserById(string userId)
        {
            var result = _db.ExecuteQuery(
              "SELECT * FROM  [FAXPEC].[FAXPEC].[users] WHERE id = @id",
              new SqlParameter { ParameterName = "@id", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            var row = result.Rows.Cast<DataRow>().SingleOrDefault();

            if (row != null)
            {
                return new IdentityUser
                {
                    Id = row["id"].ToString(),
                    UserName = row["username"].ToString(),
                    PasswordHash = row["passwordhash"].ToString().HasValue() ? row["passwordhash"].ToString() : null,
                    SecurityStamp = row["securitystamp"].ToString().HasValue() ? row["securitystamp"].ToString() : null
                };
            }

            return null;
        }

        public ICollection<IdentityUser> GetUserByName(string userName)
        {
            var result = _db.ExecuteQuery(
                "SELECT * FROM  [FAXPEC].[FAXPEC].[users] WHERE username = @name",
                new SqlParameter { ParameterName = "@name", Value = userName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            return result.Rows.Cast<DataRow>().Select(
                r => new IdentityUser
                {
                    Id = r["id"].ToString(),
                    UserName = r["username"].ToString(),
                    PasswordHash = r["passwordhash"].ToString().HasValue() ? r["passwordhash"].ToString() : null,
                    SecurityStamp = r["securitystamp"].ToString().HasValue() ? r["securitystamp"].ToString() : null
                }).ToList();
        }

        public string GetPasswordHash(string userId)
        {
            var passwordHash = _db.ExecuteScalarQuery<string>(
                "SELECT passwordhash FROM [FAXPEC].[FAXPEC].[users] WHERE id = @id",
                new SqlParameter { ParameterName = "@id", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });

            return passwordHash.HasValue() ? passwordHash : null;
        }
        public string GetPasswordHashByUserName(string userName,ref int id)
        {
            var passwordHash = string.Empty;
               var result = _db.ExecuteQuery(
                "SELECT passwordhash,id FROM [FAXPEC].[FAXPEC].[users] WHERE username = @userName",
                new SqlParameter { ParameterName = "@userName", Value = userName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
            foreach (var row in result.Rows.Cast<DataRow>())
            {
                passwordHash = row["passwordhash"].ToString();
                int.TryParse(row["id"].ToString(),out id);
            }
            return passwordHash.HasValue() ? passwordHash : null;
        }


        ////public int SetPasswordHash(string userId, string passwordHash)
        ////{
        ////    return _db.ExecuteScalarQuery<int>(
        ////        "UPDATE users SET passwordhash = :passwordhash WHERE id = :id",
        ////        new SqlParameter { ParameterName = ":passwordhash", Value = passwordHash, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
        ////        new SqlParameter { ParameterName = ":id", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        ////}

        ////public string GetSecurityStamp(string userId)
        ////{
        ////    return _db.ExecuteScalarQuery<string>(
        ////        "SELECT securitystamp FROM users WHERE id = :id",
        ////        new SqlParameter { ParameterName = ":id", Value = userId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        ////}
    }
}