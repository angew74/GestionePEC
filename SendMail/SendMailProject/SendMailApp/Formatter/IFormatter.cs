using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMailApp.Formatter
{
    interface IFormatter
    {
        System.IO.MemoryStream formatData(System.Xml.XmlDocument rawData, IList<string> Layout);

    }
}
