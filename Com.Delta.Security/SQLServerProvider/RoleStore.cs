using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNet.Identity.SQLServerProvider
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Repositories;
    using SQLServerProvider;

    public class RoleStore : IRoleStore<IdentityRole>
    {
        private readonly RoleRepository _roleRepository;

        public RoleStore()
            : this(new SQLServerDataContext())
        {
        }

        public RoleStore(SQLServerDataContext oracleContext)
        {
            Database = oracleContext;

            _roleRepository = new RoleRepository(oracleContext);
        }

        public SQLServerDataContext Database { get; private set; }

        public Task CreateAsync(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Delete(role.Id);

            return Task.FromResult<object>(null);
        }

        public Task<IdentityRole> FindByIdAsync(string roleId)
        {
            var result = _roleRepository.GetRoleById(roleId);

            return Task.FromResult(result);
        }

        public Task<IdentityRole> FindByNameAsync(string roleName)
        {
            var result = _roleRepository.GetRoleByName(roleName);

            return Task.FromResult(result);
        }

        public Task UpdateAsync(IdentityRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Update(role);

            return Task.FromResult<object>(null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Database != null)
                {
                    Database.Dispose();
                    Database = null;
                }
            }
        }
    }
}