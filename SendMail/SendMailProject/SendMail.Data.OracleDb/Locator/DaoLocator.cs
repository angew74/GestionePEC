using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.DataContracts.Interfaces;
using System.Configuration;
using System.Reflection;
using Com.Delta.Data;

using SendMail.Data.OracleDb;

namespace SendMail.DataContracts
{
    /// <summary>
    ///  Classe per la localizzazione del provider DAO.
    /// </summary>
    public class DaoLocator
    {
        private static DaoAssemblyElement daoConfig;
        //private static IDaoBaseSession<ISessionModel> provider;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyPath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        //private static T InitializeFromPath<T>(string assemblyPath, string className)
        //{
        //    //Assembly assembly = Assembly.LoadFrom(assemblyPath);
        //    //return (T)assembly.CreateInstance(className);
        //    OracleSessionManager s = new OracleSessionManager();
        //    return (T)((IDaoBaseSession<ISessionModel>)s);
        //}

        private static T InitializeFromPath<T>(string assemblyPath, string className, string connName)
        {
            Assembly assembly = null;
            string classe = null;
            //if (assembly == null)
            //{
                string[] typeCompl = className.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (typeCompl.Length < 2)
                {
                    throw new System.Configuration.ConfigurationErrorsException("Errore nella file di configurazione; il type del DaoAssembly è incompleto");
                }

                classe = typeCompl[0];
                string tipo = typeCompl[1];

                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (assemblies != null && assemblies.Length != 0)
                {
                    assembly = (from ass in assemblies
                                where ass.FullName.Contains(tipo)
                                select ass).SingleOrDefault();
                }

                if (assembly == null)
                    assembly = Assembly.LoadFrom(assemblyPath.Trim());
            //}
            if (classe == null) throw new InvalidOperationException();
            return (T)assembly.CreateInstance(classe, true, BindingFlags.CreateInstance, null,
                new object[] { connName }, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        //private static T Initialize<T>(string assemblyName, string className)
        //{
        //    //Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
        //    //return (T)assembly.CreateInstance(className);
        //    OracleSessionManager s = new OracleSessionManager();
        //    return (T)((IDaoBaseSession<ISessionModel>)s);
        //}

        /// <summary>
        /// Carica Service e Dao Factory.
        /// </summary>
        /// <returns></returns>
        public static IDaoBaseSession<ISessionModel> GetDaoProvider()
        {
            if (daoConfig == null) throw new ConfigurationErrorsException("DaoProvider NOT Initialized");
            IDaoBaseSession<ISessionModel> provider = InitializeFromPath<IDaoBaseSession<ISessionModel>>(daoConfig.File, daoConfig.ClassName, daoConfig.ConnectionName); ;
            return provider;
        }

        public static void Initialize(DaoAssemblyElement daoAssembly)
        {
            //if (provider == null)
            //{
            //    provider = InitializeFromPath<IDaoBaseSession<ISessionModel>>(daoAssembly.File, daoAssembly.ClassName, daoAssembly.ConnectionName);
            //}
            //return provider;
            if (daoConfig == null)
            {
                daoConfig = daoAssembly;
            }
        }

        #region "CODICE COMMENTATO"

        //string daofTypeName = ConfigurationManager.AppSettings["DaoFactory"];
        //Type daofType = Type.GetType(daofTypeName);
        //provider = (ISession)Activator.CreateInstance(daofType);
        //provider = Initialize<ISession>(ConfigurationManager.AppSettings["DaoAssembly"], ConfigurationManager.AppSettings["DaoManager"]);

        #endregion
    }
}
