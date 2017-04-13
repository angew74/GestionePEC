using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    namespace AspNet.Identity.SQLServerProvider
    {
        internal static class Extensions
        {
            public static bool HasValue(this string value)
            {
                return !value.HasNoValue();
            }

            public static bool HasNoValue(this string value)
            {
                return string.IsNullOrWhiteSpace(value);
            }
        }
    }
