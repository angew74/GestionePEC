using AspNet.Identity.OracleProvider;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Delta.Security
{
    public class MySecurityProvider
    {
        private static readonly ILog _log = LogManager.GetLogger("MySecurityProvider");

        public static MyPrincipal CurrentPrincipal
        {
            get
            {
                if (Thread.CurrentPrincipal.GetType().Equals(typeof(MyPrincipal)))
                    return (MyPrincipal)Thread.CurrentPrincipal;
                else
                    return null;
            }
        }

        public static string CurrentPrincipalName
        {
            get { if (CurrentPrincipal != null && CurrentPrincipal.Identity != null) return CurrentPrincipal.Identity.Name; else return null; }
        }


        public static async Task<MyPrincipal> BuildNewIdentity(string username, string dipartimento, string password, string type)
        {
            UserStore store = new UserStore();
            MyPrincipal mp = null;
            //LoginResp r = store.Authenticate(username, dipartimento, password);
            string passwordHash= await store.GetPasswordHashAsync(username);
             string digitPasswordSHA = MySecurityProvider.PlainToSHA256(password);
            if (digitPasswordSHA == passwordHash)
            {
                MyIdentity id = new MyIdentity(username, dipartimento, password, type);
                mp = new MyPrincipal(id, null);
            }
            else
            {
                MyIdentity id = new MyIdentity(username, dipartimento);
                mp = new MyPrincipal(id, null);
            }
            return await Task.FromResult(mp);
        }


      
       
        public static String PlainToSHA256(String Stringa)
        {
            if (Stringa == null || String.IsNullOrEmpty(Stringa))
                return null;
            else
            {
                System.Security.Cryptography.SHA256 sha = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                System.Text.Encoding objEncoding = System.Text.Encoding.UTF8;
                byte[] pwHashed = sha.ComputeHash(objEncoding.GetBytes(Stringa));
                return System.Convert.ToBase64String(pwHashed);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="funzione1">Ruolo da testare 1°</param>
        /// <param name="funzione2">Ruolo da testare 2°</param>
        /// <param name="operatore">and / or se entrambe sono vere o una delle due</param>
        /// <param name="procedura">la procedura della pagina da controllare</param>
        /// <returns></returns>
        /// 

        public static bool CheckAccessRight(string accessRight)
        {
            return (CurrentPrincipal != null) && (CurrentPrincipal.IsInRole(accessRight));
       }

    }
}
