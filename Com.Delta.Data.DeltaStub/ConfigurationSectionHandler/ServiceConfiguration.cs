using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Delta.Data
{
    public class BaseAssemblyElement : System.Configuration.ConfigurationElement
    {
        [System.Configuration.ConfigurationProperty("path", IsRequired = true)]
        //[System.Configuration.StringValidator(InvalidCharacters = "~|\"£$%&/()=?'^[]{}*°#§,;:", MinLength = 5)]
        public String File
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
        [System.Configuration.ConfigurationProperty("type", IsRequired = true)]
        //[System.Configuration.StringValidator(InvalidCharacters = "~|\\!\"£$%%&/()=?'^{}[]*+°#§;,:", MinLength = 1)]
        public String ClassName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }

    public sealed class ServiceAssemblyElement : BaseAssemblyElement
    {
    }

    public sealed class DaoAssemblyElement : BaseAssemblyElement
    {
        [System.Configuration.ConfigurationProperty("connectionName", IsRequired = false)]
        public String ConnectionName
        {
            get { return (string)this["connectionName"]; }
            set { this["connectionName"] = value; }
        }
    }

    public class ServiceConfigurationSection : System.Configuration.ConfigurationSection
    {
        public ServiceConfigurationSection() { }

        [System.Configuration.ConfigurationProperty("ServiceAssembly", IsRequired = true)]
        public ServiceAssemblyElement ServiceAssembly
        {
            get { return (ServiceAssemblyElement)this["ServiceAssembly"]; }
            set { this["ServiceAssembly"] = value; }
        }
        [System.Configuration.ConfigurationProperty("DaoAssembly", IsRequired = true)]
        public DaoAssemblyElement DaoAssembly
        {
            get { return (DaoAssemblyElement)this["DaoAssembly"]; }
            set { this["DaoAssembly"] = value; }
        }
    }
}
