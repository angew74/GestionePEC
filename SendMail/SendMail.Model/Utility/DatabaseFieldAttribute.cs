using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMail.Model
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DatabaseFieldAttribute : Attribute
    {
        public string FieldName { get; set; }

        public DatabaseFieldAttribute(string DBFieldName)
        {
            this.FieldName = DBFieldName;
        }
    }
}
