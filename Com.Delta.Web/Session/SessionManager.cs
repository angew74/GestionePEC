using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections.Generic;

namespace Com.Delta.Web.Session
{
    public class SessionManager
    {
        public static void CleanAllSessions()
        {
            foreach (SessionKeys key in Enum.GetValues(typeof(SessionKeys)))
            {
                HttpContext.Current.Session.Remove(Convert.ToString(key));

            }

        }
    }

    public class SessionManager<T>
    {
        public static void set(SessionKeys key, T data)
        {
            if (exist(key)) HttpContext.Current.Session[Convert.ToString(key)] = data;
            else HttpContext.Current.Session.Add(Convert.ToString(key), data);
        }


        public static void set(string key, T data)
        {
            if (exist(key)) HttpContext.Current.Session[key] = data;
            else HttpContext.Current.Session.Add(key, data);
        }


        public static void del(SessionKeys key)
        {
            HttpContext.Current.Session.Remove(Convert.ToString(key));
        }


        public static void del(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }


        public static T get(SessionKeys key)
        {
            if (HttpContext.Current.Session[Convert.ToString(key)] != null)
                return (T)HttpContext.Current.Session[Convert.ToString(key)];
            else return default(T);
        }


        public static T get(string key)
        {
            if (HttpContext.Current.Session[key] != null)
                return (T)HttpContext.Current.Session[key];
            else return default(T);
        }


        public static bool exist(SessionKeys key)
        {
            bool bRet = false;
            if (HttpContext.Current.Session[Convert.ToString(key)] != null)
                bRet = true;
            return bRet;
        }

        
        public static bool exist(string key)
        {
            bool bRet = false;
            if (HttpContext.Current.Session[key] != null)
                bRet = true;
            return bRet;
        }


        public static void set_Codici(string codice,SessionKeys key)
        {
            HttpContext.Current.Session.Add(Convert.ToString(key),codice);
        }

        public static string get_Codici(SessionKeys key)
        {
            if(HttpContext.Current.Session[Convert.ToString(key)] != null)
                return HttpContext.Current.Session[Convert.ToString(key)].ToString();
            else
                return "";
        }

       
    }//end class
}
