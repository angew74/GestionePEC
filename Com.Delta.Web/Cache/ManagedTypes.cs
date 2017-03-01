using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Delta.Web.Cache.Types
{
    public class TpuFile: System.Xml.XmlDocument
    {
    }
    
    public class BinaryResource
    {
        public string name;
        public string extension;
        public byte[] File;
    }

    public class TpuBinaryResource : BinaryResource
    { 
    
    }

    public class RolesFile : System.Xml.XmlDocument
    { 
    }
}
