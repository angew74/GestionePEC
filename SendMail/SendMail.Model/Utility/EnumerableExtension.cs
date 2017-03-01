using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SendMail.Model.Utility
{
    public static class EnumerableExtension
    {
        public static string GetDecodedType(this Enum enumeration)
        {
            string value = enumeration.ToString();
            Type type = enumeration.GetType();
            DescriptionAttribute[] attributes = (DescriptionAttribute[])type.GetField(value).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0 && attributes != null) ? attributes[0].Description : value.ToString();
        }
    }
}
