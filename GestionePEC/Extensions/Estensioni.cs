using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Extensions
{
    public static class Estensioni
    {
        public static bool Contains(this string s, string[] values)
        {
            foreach (string v in values)
            {
                if (s.Contains(v))
                    return true;
            }

            return false;
        }

        public static bool EndsWith(this string s, string[] values)
        {
            return s.EndsWith(values, StringComparison.Ordinal);
        }

        public static bool EndsWith(this string s, string[] values, StringComparison comparisonType)
        {
            foreach (string v in values)
            {
                if (s.EndsWith(v, comparisonType))
                    return true;
            }
            return false;
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            if (s != null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!char.IsWhiteSpace(s[i]))
                        return false;
                }
            }
            return true;
        }

        public static int Parse(this string s, int @default)
        {
            if (s.IsNullOrWhiteSpace())
                return @default;
            int number;
            if (int.TryParse(s, out number))
                return number;
            return @default;
        }

        public static string GetAbsoluteUrl(this string url)
        {
            if (HttpContext.Current == null)
                return url;
            else if (VirtualPathUtility.IsAbsolute(url))
            {
                return HttpContext.Current.Request.Url.Scheme + "://"
                    + HttpContext.Current.Request.Url.Authority
                    + HttpContext.Current.Request.ApplicationPath
                    + url;
            }
            else
            {
                return HttpContext.Current.Request.Url.Scheme + "://"
                    + HttpContext.Current.Request.Url.Authority
                    + VirtualPathUtility.ToAbsolute(url);
            }
        }
    }
}