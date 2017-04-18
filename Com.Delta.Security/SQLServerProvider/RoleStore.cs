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
    using Com.Delta.Logging;
    using Com.Delta.Logging.Errors;
    using log4net;

    public class RoleStore : IRoleStore<IdentityRole>
    {
        private static readonly ILog _log = LogManager.GetLogger("RoleStore");
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

        public Task<string> CreateAsync(IdentityRole role)
        {
            int val = 0;
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            try
            {

              val =_roleRepository.Insert(role);
            }
            catch(Exception ex)
            {
                if (ex.GetType() == typeof(ManagedException))
                {
                    throw;
                }
                else
                {
                    ManagedException m = new ManagedException("Errore nell'inserimento per ruolo - " + role.Name + " dettagli: " + ex.Message, "STO01", "CreateAsync", string.Empty, null);
                    ErrorLogInfo error = new ErrorLogInfo(m);
                    error.loggingTime = System.DateTime.Now;
                    _log.Error(error);
                    throw (m);
                }
            }

            if (val > 0)
            { return Task.FromResult<string>("OK"); }
            else
            { return Task.FromResult<string>("KO"); }
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

        public Task<List<IdentityRole>> GetAll()
        {
            var result = _roleRepository.GetAll();
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

        Task IRoleStore<IdentityRole, string>.CreateAsync(IdentityRole user)
        {
            throw new NotImplementedException();
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