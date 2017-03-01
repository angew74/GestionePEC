using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using log4net;
using System.Collections.Generic;
using System.Xml;
using Com.Delta.Logging.Errors;
using Com.Delta.Web.Cache.Types;
using System.IO;
using Com.Delta.Logging;

namespace Com.Delta.Web.Cache
{
    public enum VincoloType
    {
        BACKEND,   //l'item è vincolato al backend e viene ricaricaro automaticamente se nullo in cache
        FILESYSTEM,//L'item è vincolato al filesystem e viene ricaricao automaticamente se nullo in cahe
        NONE       //L'item è gestito in modo applicativo quindi è stat fatta una set esplicita
    }

    

    public class CacheManager<T> where T : new()
    {
        private static readonly ILog log = LogManager.GetLogger("CacheManager");


        public class RequiredBackEndDataEventArgs
        {
            public T data;
            public string Key;
        }

        private static readonly ILog _log = LogManager.GetLogger("CacheManager");

        public delegate void RequiredBackEndDataEventHandler(RequiredBackEndDataEventArgs key);
        public static event RequiredBackEndDataEventHandler RequireDataFromBackEnd;
        protected static void RequiredBackEndData(RequiredBackEndDataEventArgs e)
        {
            if (RequireDataFromBackEnd != null)
            {
                RequireDataFromBackEnd(e);
            }
        }

        public static void set(CacheKeys key, T data)
        {
            set(Convert.ToString(key), data);
        }

        public static void del(CacheKeys key)
        {
           del(Convert.ToString(key));
        }

        public static T get(CacheKeys key, VincoloType vincoloType)
        {
            return get(Convert.ToString(key), vincoloType);
        }

        public static T get(CacheKeys key, VincoloType vincoloType, System.Xml.Xsl.XsltSettings settings)
        {
            return get(Convert.ToString(key), vincoloType, settings);
        }

        public static bool exist(CacheKeys key)
        {
            return exist(Convert.ToString(key));
        }

        protected internal static void set(string key, T data)
        {
            if (exist(key)) HttpContext.Current.Cache[Convert.ToString(key)] = data;
            else HttpContext.Current.Cache.Insert(Convert.ToString(key), data);
        }

        protected internal static void set(string key, T data, string path)
        {
            if (exist(key)) HttpContext.Current.Cache[Convert.ToString(key)] = data;
            else HttpContext.Current.Cache.Insert(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            
        }

        protected internal static void del(string key)
        {
            HttpContext.Current.Cache.Remove(Convert.ToString(key));
        }

        protected internal static T get(string key, VincoloType vincoloType)
        {
            //string ky = Convert.ToString(key);
            bool exsist = false;

            if (HttpContext.Current.Cache[key] != null)
                return (T)HttpContext.Current.Cache[key];

            switch (vincoloType)
            {
                case VincoloType.NONE:
                    if (exsist)
                        return (T)HttpContext.Current.Cache[key];
                    else
                        return default(T);
                    

                case VincoloType.FILESYSTEM:
                    if (typeof(T) == typeof(System.Xml.XmlDocument) || typeof(T).BaseType == typeof(System.Xml.XmlDocument))
                    {
                        T data = default(T);
                        data = getXml(key.ToString(),typeof(T));
                        return data;
                    }
                    if (typeof(T) == typeof(System.Xml.Xsl.XslCompiledTransform))
                    {
                        T data = default(T);
                        data = getXslt(key.ToString());
                        return data;
                    }
                    if ((typeof(T)).BaseType == typeof(System.Data.DataSet) ||
                        typeof(T) == typeof(System.Data.DataSet))
                    {
                        return getDatasetFromFileSystem(key.ToString());
                    }
                    if ((typeof(T)).BaseType == typeof(BinaryResource) ||
                        typeof(T) == typeof(BinaryResource))
                    {
                        return getBinary(key.ToString(), typeof(T));
                    }
                    break;

                case VincoloType.BACKEND:
                    return getClassFromBackEnd(key.ToString());
                    

                default:
                    throw new Exception("CACHEMANAGER:Tipo di sorgente non valido:");
            }

            return default(T);
        }

        protected internal static T get(string key, VincoloType vincoloType, System.Xml.Xsl.XsltSettings settings)
        {
            string ky = Convert.ToString(key);
            bool exsist = false;

            if (HttpContext.Current.Cache[ky] != null)
                return (T)HttpContext.Current.Cache[ky];

            switch (vincoloType)
            {
                case VincoloType.NONE:
                    if (exsist)
                        return (T)HttpContext.Current.Cache[ky];
                    else
                        return default(T);
                    

                case VincoloType.FILESYSTEM:
                    if (typeof(T) == typeof(System.Xml.XmlDocument))
                    {
                    }
                    if (typeof(T) == typeof(System.Xml.Xsl.XslCompiledTransform))
                    {
                        T data = default(T);
                        data = getXslt(key.ToString(), settings);
                        return data;
                    }
                    if ((typeof(T)).BaseType == typeof(System.Data.DataSet) ||
                        typeof(T) == typeof(System.Data.DataSet))
                    {
                        return getDatasetFromFileSystem(key.ToString());
                    }
                    break;

                case VincoloType.BACKEND:
                    return getClassFromBackEnd(key.ToString());
                    
                default:
                    throw new Exception("CACHEMANAGER:Tipo di sorgente non valido:");
            }

            return default(T);
        }

        protected internal static bool exist(string key)
        {
            bool bRet = false;
            if (HttpContext.Current.Cache[Convert.ToString(key)] != null)
                bRet = true;
            return bRet;
        }

        protected static T getBinary(string Name, Type t)
        {
            T data = new T();
            string key = Name;
            
            if (HttpContext.Current.Cache[key] == null)
            {
                try
                {
                    string path = getResourcePath(key, typeof(T).Name + "s");
                    byte[] b = File.ReadAllBytes(path);
                    (data as BinaryResource).File = b;
                    (data as BinaryResource).name = System.IO.Path.GetFileName(path);
                    (data as BinaryResource).extension = System.IO.Path.GetExtension(path);
                    HttpContext.Current.Cache.Add(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                catch (System.Exception errore)
                {
                    ManagedException ex = new ManagedException("Impossibile caricare il file Binario.", "CACHE_01", "", "", errore);
                    ErrorLog error = new ErrorLog(ex);
                    log.Error(error);
                    throw ex;
                }
            }
            return (T)HttpContext.Current.Cache[key];
        }


        protected static T getXml(string XmlName, Type t)
        {
            T data = new T();
            string key = XmlName;
            if (HttpContext.Current.Cache[key] == null)
            {
                try
                {
                    string path = "";
                    if (typeof(T) == typeof(System.Xml.XmlDocument))
                        path = getResourcePath(key + ".xml", "XML");
                    else
                        path = getResourcePath(key, typeof(T).Name + "s");
                    (data as System.Xml.XmlDocument).Load(path);
                    HttpContext.Current.Cache.Add(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                catch (System.Exception errore)
                {
                    ManagedException ex = new ManagedException("Impossibile caricare il documento XML.", "CACHE_02", "", "", errore);
                    ErrorLog error = new ErrorLog(ex);
                    log.Error(error);
                    throw ex;
                }
            }
            return (T)HttpContext.Current.Cache[key];
        }

        protected static T getXslt(string XsltName)
        {
            T data = new T();
            string key = XsltName;
            if (HttpContext.Current.Cache[key] == null)
            {
                try
                {
                    string path = getResourcePath(key + ".xsl", "XSLT");
                    (data as System.Xml.Xsl.XslCompiledTransform).Load(path);
                    HttpContext.Current.Cache.Add(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                catch (System.Exception errore)
                {
                    ErrorLog error = new ErrorLog();
                    error.freeTextDetails = errore.Message;
                    error.logCode = "CACHE_03";
                    //error.loggingAppCode = "WEB";
                    error.loggingTime = System.DateTime.Now;
                    error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                    log.Error(error);
                    throw new Exception("Impossibile caricare il foglio XSLT.", errore);
                }
            }
            return (T)HttpContext.Current.Cache[key];
        }

        protected static T getXslt(string XsltName, System.Xml.Xsl.XsltSettings settings)
        {
            T data = new T();
            string key = XsltName;
            if (HttpContext.Current.Cache[key] == null)
            {
                try
                {
                    string path = getResourcePath(key + ".xsl", "XSLT");
                    (data as System.Xml.Xsl.XslCompiledTransform).Load(path, settings, new XmlUrlResolver());
                    HttpContext.Current.Cache.Add(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                catch (System.Exception errore)
                {
                    ErrorLog error = new ErrorLog();
                    error.freeTextDetails = errore.Message;
                    error.logCode = "CACHE_04";
                    //error.loggingAppCode = "WEB";
                    error.loggingTime = System.DateTime.Now;
                    error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                    log.Error(error);
                    throw new Exception("Impossibile caricare il foglio XSLT.", errore);
                }
            }
            return (T)HttpContext.Current.Cache[key];
        }

        protected static T getDatasetFromFileSystem(string datasetName)
        {
            T data = new T();
            string key = datasetName;
            if (HttpContext.Current.Cache[key] == null)
            {
                try
                {
                    string path = getResourcePath(key + ".xml", "XML");
                    (data as DataSet).ReadXml(path);
                    HttpContext.Current.Cache.Add(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                catch (System.Exception errore)
                {
                    ErrorLog error = new ErrorLog();
                    error.freeTextDetails = errore.Message;
                    //error.logCode = "CACHE_05";                    
                    error.loggingTime = System.DateTime.Now;
                    error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                    log.Error(error);
                    throw new Exception("Impossibile caricare il Dataset", errore);
                }
            }
            else data = (T)HttpContext.Current.Cache[key];
            return data;
        }

        protected static T getClassFromBackEnd(string key)
        {
            T data = new T();
            if (HttpContext.Current.Cache[key] == null)
            {
                try
                {
                    RequiredBackEndDataEventArgs e0= new RequiredBackEndDataEventArgs();
                    e0.Key=key;
                    RequiredBackEndData(e0);
                    if (e0.data != null)
                        HttpContext.Current.Cache.Add(key, e0.data, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                catch (System.Exception errore)
                {
                    ErrorLog error = new ErrorLog();
                    error.freeTextDetails = errore.Message;
                    error.logCode = "CACHE_06";
                    //error.loggingAppCode = "WEB";
                    error.loggingTime = System.DateTime.Now;
                    error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                    log.Error(error);
                    throw new Exception("Impossibile caricare la classe dal backend per la chiave:"+key, errore);
                }
            }
            return (T)HttpContext.Current.Cache[key];
            //return data;
        }



        /// <summary>
        /// Carica il dizionario da un file xml con strutture ripetute chiave valore.
        /// La conversione del valore da stringa a T funziona per i tipi base.
        /// In caso di tipi personalizzati ridefinire la conversione...
        /// </summary>
        /// <param name="dictionaryName"></param>
        /// <returns></returns>
        protected static Dictionary<string, T> getDictionary(string key, VincoloType vincoloType)
        {
            if (!(typeof(T).IsPrimitive))
                throw new Exception("Non ancora sviluppato per tipi non primitivi");

            bool exsist = false;

            if (HttpContext.Current.Cache[key] != null)
                return (Dictionary<string, T>)HttpContext.Current.Cache[key];

            switch (vincoloType)
            {
                case VincoloType.NONE:
                    if (exsist)
                        return (Dictionary<string, T>)HttpContext.Current.Cache[key];
                    else
                        return new Dictionary<string, T>();
                    

                case VincoloType.FILESYSTEM:

                    Dictionary<string, T> data = new Dictionary<string, T>();

                    try
                    {
                        string path = getResourcePath(key + ".xml", "XML");
                        XmlDocument doc = new XmlDocument();
                        doc.Load(path);
                         foreach (XmlElement e in doc.DocumentElement.ChildNodes)
                        {
                            data.Add(e.SelectSingleNode("key").InnerText,
                                (T)Convert.ChangeType(e.SelectSingleNode("value").InnerText, typeof(T)));
                        }
                        HttpContext.Current.Cache.Add(key, data, new System.Web.Caching.CacheDependency(path), System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                    }
                    catch (System.Exception errore)
                    {
                        ErrorLog error = new ErrorLog();
                        error.freeTextDetails = errore.Message;
                        error.logCode = "CACHE_07";
                        //error.loggingAppCode = "WEB";
                        error.loggingTime = System.DateTime.Now;
                        error.uniqueLogID = System.DateTime.Now.Ticks.ToString();
                        log.Error(error);
                        throw new Exception("Impossibile caricare il Dizionario", errore);
                    }
                    return data;
                   

                case VincoloType.BACKEND:
                    throw new Exception("Non ancora sviluppato");
                  

                default:
                    throw new Exception("CACHEMANAGER:Tipo di sorgente non valido:");
            }

        }

        public static string getResourcePath(string resourceName, string Tipo)
        {
            CacheConfig section = (CacheConfig)ConfigurationManager.GetSection("CacheConfig");
            string where = null, relativepath = string.Empty;

            if (section != null)
            {
                for (int i = 0; i < section.FolderItems.Count; i++)
                    if (section.FolderItems[i].FolderType.Equals(Tipo))
                    {
                        relativepath = HttpContext.Current.Server.MapPath(section.FolderItems[i].Path);
                        where = System.IO.Path.Combine(relativepath, resourceName);
                    }
            }
            if (where == null)
                throw new Exception("Impossibile individuare il percorso di caricamento dei file XSLT: controllare il file di configurazione");
            else
                if (!System.IO.File.Exists(where))
                {
                    string[] ss=System.IO.Directory.GetFiles(relativepath, resourceName + ".*");
                    if (ss != null && ss.Length == 1)
                        return ss[0];
                    if (ss == null || ss.Length == 0) throw new Exception("Il file [" + where + "] non è stato individuato");
                    if (ss != null && ss.Length > 0) throw new Exception("Il file [" + where + "] ha corrispondenze multiple sul file system");
                }
            return where;
        }

    }
}
