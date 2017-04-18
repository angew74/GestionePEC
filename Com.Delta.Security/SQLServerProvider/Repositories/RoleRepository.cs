using AspNet.Identity.SQLServerProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AspNet.Identity.SQLServerProvider.Repositories
{


    internal class RoleRepository
    {
        private readonly SQLServerDataContext _db;

        public RoleRepository(SQLServerDataContext oracleContext)
        {
            _db = oracleContext;
        }

        public int Insert(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            return _db.ExecuteNonQuery(
                "INSERT INTO roles (id, name) VALUES (@id, :name)",
                new SqlParameter { ParameterName = "@id", Value = role.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@name", Value = role.Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public int Update(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            return _db.ExecuteNonQuery(
                "UPDATE roles SET name = :name WHERE id = :id",
                new SqlParameter { ParameterName = "@name", Value = role.Name, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input },
                new SqlParameter { ParameterName = "@id", Value = role.Id, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public int Delete(string roleId)
        {
            return _db.ExecuteNonQuery(
                "DELETE FROM roles WHERE id = @id",
                new SqlParameter { ParameterName = "@id", Value = roleId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        internal List<IdentityRole> GetAll()
        {
            List<IdentityRole> listRole = null;
            var roles = _db.ExecuteQuery(@"Select ID, NAME FROM [FAXPEC].[FAXPEC].[ROLES]");
            if (roles.Rows.Count > 0)
            {
                listRole = new List<IdentityRole>();
                foreach (DataRow r in roles.Rows)
                {
                    IdentityRole role = new IdentityRole()
                    {
                        Id = r["ID"].ToString(),
                        Name = r["NAME"].ToString()
                    };
                    listRole.Add(role);
                }
            }
            return listRole;
        }

        public string GetRoleName(string roleId)
        {
            return _db.ExecuteScalarQuery<string>(
                "SELECT name FROM roles WHERE id = @id",
                new SqlParameter { ParameterName = "@id", Value = roleId, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public string GetRoleId(string roleName)
        {
            return _db.ExecuteScalarQuery<string>(
                "SELECT id FROM roles WHERE name = @name",
                new SqlParameter { ParameterName = "@name", Value = roleName, SqlDbType = SqlDbType.VarChar, Direction = ParameterDirection.Input });
        }

        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);

            return roleName != null ? new IdentityRole(roleName, roleId) : null;
        }

        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);

            return roleId != null ? new IdentityRole(roleName, roleId) : null;
        }
    }
}