using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using AspNet.Identity.SQLServerProvider.Repositories;
using log4net;
using Com.Delta.Logging;
using Com.Delta.Logging.Errors;
using AspNet.Identity.SQLServerProvider;

namespace AspNet.Identity.SQLServerProvider
{
   

    public class UserStore :
        IUserStore<IdentityUser>,
        IUserClaimStore<IdentityUser>,
        IUserLoginStore<IdentityUser>,
        IUserRoleStore<IdentityUser>,
        IUserPasswordStore<IdentityUser>
    {

        private static readonly ILog _log = LogManager.GetLogger("UserStore");
        private readonly UserRepository _userRepository;
        private readonly UserClaimsRepository _userClaimsRepository;

        public Task<List<IdentityUser>> GetAll()
        {
            var result = _userRepository.GetAll();
            return Task.FromResult(result);
        }

        private readonly UserLoginsRepository _userLoginsRepository;
        private readonly RoleRepository _roleRepository;
        private readonly UserRolesRepository _userRolesRepository;

        public UserStore()
            : this(new SQLServerDataContext())
        {
        }

        public UserStore(SQLServerDataContext database)
        {
            // TODO: Compare with EntityFramework provider.
            Database = database;

            _userRepository = new UserRepository(database);
            _roleRepository = new RoleRepository(database);
            _userRolesRepository = new UserRolesRepository(database);
            _userClaimsRepository = new UserClaimsRepository(database);
            _userLoginsRepository = new UserLoginsRepository(database);
        }

        public SQLServerDataContext Database { get; private set; }

        Task IUserStore<IdentityUser, string>.UpdateAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateAsync(IdentityUser user)
        {
            int val = 0;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            try
            {
                val = _userRepository.Update(user);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(ManagedException))
                {
                    throw;
                }
                else
                {
                    ManagedException m = new ManagedException("Errore nell'aggiornamento per username - " + user.UserName + " dettagli: " + ex.Message, "STO05", "UpdateAsync", string.Empty, null);
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
        public Task<string> CreateAsync(IdentityUser user)
        {
            int val = 0;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            try
            {
               val = _userRepository.Insert(user);
            }
            catch(Exception ex)
            {
                if(ex.GetType() == typeof(ManagedException))
                {
                    throw;
                }
                else
                {
                    ManagedException m = new ManagedException("Errore nell'inserimento per username - " + user.UserName + " dettagli: " + ex.Message, "STO01", "CreateAsync", string.Empty, null);
                    ErrorLogInfo error = new ErrorLogInfo(m);
                    error.loggingTime = System.DateTime.Now;
                    _log.Error(error);
                    throw (m);
                }
            }
            if (val > 0)
            { return Task.FromResult<string>("OK"); }
            else
            {return Task.FromResult<string>("KO");}
        }

        public Task<List<IdentityUser>> FindUsersByRole(string id)
        {
            var result = _userRepository.FindUsersByRole(id);
            return Task.FromResult(result);
        }

        public Task DeleteAsync(IdentityUser user)
        {
            if (user != null)
            {
                _userRepository.Delete(user);
            }

            return Task.FromResult<object>(null);
        }

        public Task<IdentityUser> FindByIdAsync(string userId)
        {
            if (userId.HasNoValue())
            {
                throw new ArgumentException("userId");
            }

            var result = _userRepository.GetUserById(userId);

            return Task.FromResult(result);
        }

        public Task<IdentityUser> FindByNameAsync(string userName)
        {
            if (userName.HasNoValue())
            {
                throw new ArgumentException("userName");
            }

            var result = _userRepository.GetUserByName(userName).SingleOrDefault();

            return Task.FromResult(result);
        }


        public Task AddClaimAsync(IdentityUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            _userClaimsRepository.Insert(claim, user.Id);

            return Task.FromResult<object>(null);
        }

        public Task<IList<Claim>> GetClaimsAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var claimsIdentity = _userClaimsRepository.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(claimsIdentity.Claims.ToList());
        }

        public Task RemoveClaimAsync(IdentityUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            _userClaimsRepository.Delete(user, claim);

            return Task.FromResult<object>(null);
        }

        public Task AddLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            _userLoginsRepository.Insert(user, login);

            return Task.FromResult<object>(null);
        }

        public Task<IdentityUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userId = _userLoginsRepository.FindUserIdByLogin(login);

            if (userId != null)
            {
                var user = _userRepository.GetUserById(userId);

                if (user != null)
                {
                    return Task.FromResult(user);
                }
            }

            return Task.FromResult<IdentityUser>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var userLogins = _userLoginsRepository.FindByUserId(user.Id);

            return Task.FromResult(userLogins);
        }

        public Task RemoveLoginAsync(IdentityUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            _userLoginsRepository.Delete(user, login);

            return Task.FromResult<object>(null);
        }

        public Task AddToRoleAsync(IdentityUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (role.HasNoValue())
            {
                throw new ArgumentNullException("role");
            }

            var roleId = _roleRepository.GetRoleId(role);

            if (roleId.HasValue())
            {
                _userRolesRepository.Insert(user, roleId);
            }

            return Task.FromResult<object>(null);
        }


        public Task<int> AddToRoleAsync(IdentityUser user, int role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
         var rest = _userRolesRepository.Insert(user, role.ToString());        
            return Task.FromResult<int>(rest);
        }


        public Task<IList<string>> GetRolesAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var roles = _userRolesRepository.FindByUserId(user.Id);

            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (role.HasNoValue())
            {
                throw new ArgumentNullException("role");
            }

            var roles = _userRolesRepository.FindByUserId(user.Id);

            return Task.FromResult(roles != null && roles.Contains(role));
        }

        public Task RemoveFromRoleAsync(IdentityUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (role.HasNoValue())
            {
                throw new ArgumentNullException("role");
            }

            var roleId = _roleRepository.GetRoleId(role);

            if (roleId.HasValue())
            {
                _userRolesRepository.Delete(user, roleId);
            }

            return Task.FromResult<object>(null);
        }

        public Task RemoveUserIdFromRoleIdAsync(int iduser, int idrole)
        {        
             _userRolesRepository.Delete(iduser.ToString(), idrole.ToString());        

            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var passwordHash = _userRepository.GetPasswordHash(user.Id);

            return Task.FromResult(passwordHash);
        }

        public Task<string> GetPasswordHashAsync(string UserName,ref int id)
        {
            if (UserName == null)
            {
                throw new ArgumentNullException("user");
            }

            var passwordHash = _userRepository.GetPasswordHashByUserName(UserName,ref id);

            return Task.FromResult(passwordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var hasPassword = _userRepository.GetPasswordHash(user.Id).HasValue();

            return Task.FromResult(hasPassword);
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PasswordHash = passwordHash;

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

        Task IUserStore<IdentityUser, string>.CreateAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }
      
    }
}