using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SendMailApp.Formatter
{
    public class XmlUty
    {
        public static string XsltToString(System.Xml.XmlDocument source, System.Xml.XmlDocument docXslt)
        {

            System.IO.MemoryStream xsltStream = new System.IO.MemoryStream();
            System.IO.MemoryStream inStream = new System.IO.MemoryStream();
            System.IO.MemoryStream outStream = new System.IO.MemoryStream();


            inStream.Position = 0;
            source.Save(inStream);
            inStream.Position = 0;
            XmlReader rr = XmlReader.Create(inStream);


            xsltStream.Position = 0;
            docXslt.Save(xsltStream);
            xsltStream.Position = 0;
            XmlReader xsltReader = XmlReader.Create(xsltStream);
            System.Xml.Xsl.XslCompiledTransform t = new System.Xml.Xsl.XslCompiledTransform();
            t.Load(xsltReader);


            t.Transform(rr, null, outStream);

            outStream.Seek(0, System.IO.SeekOrigin.Begin);
            System.IO.StreamReader outReader = new System.IO.StreamReader(outStream);
            return outReader.ReadToEnd();

        }

    
    }
}
