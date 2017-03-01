using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SendMail.Contracts;
using SendMail.Business;
using SendMail.DataContracts.Interfaces;
using SendMail.DataContracts;
using Com.Delta.Data;
using System.Reflection;
using System.Configuration;

namespace SendMail.Locator
{
    public class ServiceLocator
    {
        private static SendMail.Contracts.IServiceFactory provider;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyPath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private static T InitializeFromPath<T>(string assemblyPath, string className, object[] args)
        {
            //Assembly assembly = Assembly.LoadFrom(assemblyPath);
            //return (T)assembly.CreateInstance(className, false, BindingFlags.CreateInstance, null, args, System.Globalization.CultureInfo.InvariantCulture, null);
            //return (T)assembly.CreateInstance(className);
            ServiceFactory s = new ServiceFactory();
            return (T)((SendMail.Contracts.IServiceFactory)s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private static T Initialize<T>(string assemblyName, string className)
        {
            //Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            //return (T)assembly.CreateInstance(className);
            ServiceFactory s = new ServiceFactory();
            return (T) ((SendMail.Contracts.IServiceFactory)s);
        }

        private static T InitializeFromPath<T>(ServiceAssemblyElement cfgElement)
        {
            Assembly assembly = null;

            string[] typeCompl = cfgElement.ClassName.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (typeCompl.Length < 2)
            {
                throw new System.Configuration.ConfigurationErrorsException("Errore nella file di configurazione; il type del DaoAssembly è incompleto");
            }

            string classe = typeCompl[0];
            string tipo = typeCompl[1];

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblies != null && assemblies.Length != 0)
            {
                assembly = (from ass in assemblies
                            where ass.FullName.Contains(tipo)
                            select ass).SingleOrDefault();
            }

            if (assembly == null)
                assembly = Assembly.LoadFrom(cfgElement.File.Trim());

            return (T)assembly.CreateInstance(classe);
        }
        

        /*
         * CARICA LA FACTORY SERVICE DI UN ALTRO ASSEMBLY (Business)
         * A SUA VOLTA LA FACTORY CARICA CIASCUN SERVICE
         * RICERCANDO E INIETTANDO IL DAO (non serve factory?)
        */

        /// <summary>
        /// Carica Service e Dao Factory.
        /// </summary>
        /// <returns></returns>
        public static IServiceFactory GetServiceFactory()
        {
            if (provider == null)
            {
                ServiceConfigurationSection cfg = (ServiceConfigurationSection)ConfigurationManager.GetSection("ServiceConfigurationSectionGroup/SendMail");
                if (cfg != null)
                {
                    //IDao<ISessionModel> daoProvider = DaoLocator.Initialize(cfg.DaoAssembly);
                    DaoLocator.Initialize(cfg.DaoAssembly);
                    provider = InitializeFromPath<IServiceFactory>(cfg.ServiceAssembly);
                }
            }
            return provider;
        }

        #region "CODICE COMMENTATO"

        //string servicefTypeName = ConfigurationManager.AppSettings["ServiceFactory"];
        //Type servicefType = Type.GetType(servicefTypeName);
        //provider = (IServiceFactory)Activator.CreateInstance(servicefType, daoProvider);
        //provider = (IServiceFactory)Activator.CreateInstance(servicefType);

        /// <summary>
        /// Carica Service e Dao Factory.
        /// </summary>
        /// <returns></returns>
        //public static SendMail.Contracts.IServiceFactory GetServiceFactory()
        //{
        //    if (provider == null)
        //    {
        //        IDaoBaseSession<ISessionModel> daoProvider = DaoLocator.GetDaoProvider();
        //        //provider = InitializeFromPath<SendMail.Contracts.IServiceFactory>(ConfigurationManager.AppSettings["ServiceAssemblyPath"], ConfigurationManager.AppSettings["ServiceFactory"], new object[] { daoProvider });
        //        provider = InitializeFromPath<SendMail.Contracts.IServiceFactory>("xx","xx",null);


        //    }
        //    return provider;
        //}
        #endregion

    }
}
