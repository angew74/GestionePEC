using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Com.Delta.Logging;
using System.Globalization;
using Com.Delta.Logging.Um;

namespace Com.Delta.Logging
{
    /// <summary>
    /// Classe di utilità per effettuare la conversione degli oggetti utili nella scrittura
    /// e lettura di log sulle code
    /// </summary>
    public class ConversionUtils
    {
        /// <summary>
        ///  Metodo che converte una stringa in un array di Byte
        /// </summary>
        /// <param name="pXmlString">stringa da convertire</param>
        /// <returns><c>Byte[]</c> array di bytes di ritorno</returns>
        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {

            UTF8Encoding encoding = new UTF8Encoding();

            Byte[] byteArray = encoding.GetBytes(pXmlString);

            return byteArray;

        }
        
        /// <summary>
        /// Metodo che converte un array di Byte[] in una stringa
        /// </summary>
        /// <param name="characters">Array di Byte da convertire</param>
        /// <returns><c>String</c> stringa ottenuta dall'array di Byte</returns>
        public static String UTF8ByteArrayToString(Byte[] characters)
        {

            UTF8Encoding encoding = new UTF8Encoding();

            String constructedString = encoding.GetString(characters);

            return (constructedString);

        }

        /// <summary>
        /// Metodo che trasforma un oggetto generico in un XmlDocument utile nelle classi di log per la 
        /// serializzazione dei log 
        /// </summary>
        /// <param name="obj">Oggetto generico con la classe da serializzare</param>
        /// <param name="extraTypes">Array dei tipi matrici necessari ad effettuare la serializzazione</param>
        /// <returns><c>XmlDocument</c> documento Xml frutto della trasformazione</returns>
        public static XmlDocument ObjectToXml(object obj,System.Type[] extraTypes)
        {
            String XmlizedString = null;
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer xs=null;
            if (extraTypes==null) xs = new System.Xml.Serialization.XmlSerializer(obj.GetType());//, extraTypes);
            else xs = new System.Xml.Serialization.XmlSerializer(obj.GetType(), extraTypes);
            
            System.Xml.XmlTextWriter xmlTextWriter = new System.Xml.XmlTextWriter(memoryStream, System.Text.Encoding.UTF8);
            xs.Serialize(xmlTextWriter, obj);

            memoryStream = (System.IO.MemoryStream)xmlTextWriter.BaseStream;

            XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(XmlizedString.Substring(1));
            return doc;
        }
        
        /// <summary>
        /// Metodo che trasforma un XmlDocument in un oggetto generico utile nelle classi di log per la 
        /// deserializzazione del XmlDocument letto nella coda
        /// </summary>
        /// <param name="doc">XmlDocument da trasformare</param>
        /// <param name="returnType">Tipo della classe da restituire al termine della deserializzazione</param>
        /// <returns></returns>
        public static object XmlToObject(System.Xml.XmlDocument doc,System.Type returnType) 
        { 
            XmlSerializer xs = new XmlSerializer ( returnType );
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(ConversionUtils.StringToUTF8ByteArray(doc.OuterXml));
            System.Xml.XmlTextWriter xmlTextWriter = new System.Xml.XmlTextWriter(memoryStream, System.Text.Encoding.UTF8);
            return xs.Deserialize ( memoryStream );
        }

        /// <summary>
        /// Strings to object.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <returns></returns>
        public static object StringToObject(string s, System.Type returnType)
        {

            UmLogInfo i = new UmLogInfo();
            if (returnType == typeof(UmLogInfo))
            {
                
                string[] ss = s.Split('|');                
                string[] data = ss[1].Split(':'); 
                string d = data[1].Replace('.', ':');               
                DateTime dt = Convert.ToDateTime(d);
                i.loggingTime = dt;
                i.loggingAppCode = ss[0].Split(':')[1];
                i.uniqueLogID = ss[2].Split(':')[1];
                i.logCode = ss[3].Split(':')[1];
                i.freeTextDetails = ss[4].Split(':')[2];
                i.userID = ss[5].Split(':')[1];
                i.userIP = ss[6].Split(':')[1];
                i.objectAppID = ss[7].Split(':')[1];
                i.objectGroupID = ss[8].Split(':')[1];
                i.objectID = ss[9].Split(':')[1];
                i.objectParentcode = ss[10].Split(':')[1];
                i.passiveobjectID = ss[11].Split(':')[1];
                i.passiveobjectGroupID = ss[12].Split(':')[1].Replace("\r\n","");
            }
            return i;
            
        }   
 
    }
}
