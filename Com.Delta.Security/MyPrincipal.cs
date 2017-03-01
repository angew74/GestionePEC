using AspNet.Identity.OracleProvider;
using Com.Delta.Security.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Delta.Security
{
    public class MyPrincipal : ISSOPrincipal
    {

        #region Controls Properties
        private MyIdentity _identity;

        private string sede;
        public string Sede
        {
            get { return sede; }
            set { sede = value; }
        }

        public MyIdentity MyIdentity
        {
            get { return _identity; }
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        IIdentity IPrincipal.Identity
        {
            get { return _identity; }
        }

        private IList<string> _roles;
        public string UserIp;

        public DateTime ServerCreated
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ServerRenewed
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public TimeSpan ServerTimeOut
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string ServerToken
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
        public MyPrincipal(MyIdentity user)
        {
            _roles = new List<string>();           

            if (user == null)
                throw new Exception("Profilo non autenticato");


            _identity = user;


        }

        public MyPrincipal(MyIdentity user, string connIP)
        {
            _roles = new List<string>();
            if (user == null)
                throw new Exception("Profilo non autenticato");

            try
            {
                _identity = user;

                try
                {
                    UserStore store = new UserStore();
                  _roles = store.GetRolesAsync(user).Result;                 
                    
                }
                catch (Exception e0)
                {                 
                     throw new Exception("Username e/o Dipartimento e/o Password potrebbero essere errati");
                }

                _identity.NomeCognome = "";      
                sede = "";               
            }
            catch (Exception ex)
            {               
                throw new Exception("Profilo non corretto - " + ex.Message);
            }
        }

        public Task<bool> IsInRole(MyIdentity identity, string role)
        {
            UserStore store = new UserStore();
            return store.IsInRoleAsync(identity, role);
        }

        public Task<IList<string>> getRoles(MyIdentity user)
        {
            UserStore store = new UserStore();
            return store.GetRolesAsync(user);
        }

        public bool IsInRole(string role)
        {
            MyPrincipal principal = null;
            if (Thread.CurrentPrincipal.GetType().Equals(typeof(MyPrincipal)))
                principal = (MyPrincipal)Thread.CurrentPrincipal;
            UserStore store = new UserStore();
           Task<IList<string>> roles = store.GetRolesAsync(principal.MyIdentity);
            for (int i = 0; i < roles.Result.Count;i++)
            {
                if(roles.Result[i].ToUpper()== role.ToUpper())
                {
                    return true;
                }
            }
            return false;
        }
    }
}