using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Com.Delta.Logging.Um;
using System.Configuration;

namespace Com.Delta.Logging
{


    /// <summary>
    /// Classe astratta che tutte le classi per la scrittura dei log relativi alle applicazioni, 
    /// devono implementare. La classe presenta due costruttori e due metodi fondamentali
    /// uno per la serializzazione della classe stessa, e uno per la deserializzazione.
    /// </summary>
    [Serializable]
    public abstract class BaseLogInfo : ICloneable
    {
        /// <summary>
        /// The cache log
        /// </summary>
        public static bool CacheLog = false;
        /// <summary>
        /// Costruttore di default
        /// </summary>
        public BaseLogInfo()
        {
            this.loggingTime = System.DateTime.Now;
            this.uniqueLogID = PlainToSHA1(Guid.NewGuid().ToString());
            if (ConfigurationManager.GetSection("ApplicationCode") != null)
                this.loggingAppCode = ((ApplicationCodeConfigSection)ConfigurationManager.GetSection("ApplicationCode")).AppCode;
        }

        /// <summary>
        /// Costruttore che prende in input il codice applicazione, il codice evento del log 
        /// e i dettagli del log
        /// </summary>
        /// <param name="logAppCode">Codice applicazione del log</param>
        /// <param name="logCode">codice del log</param>
        /// <param name="details">dettagli del log</param>
        public BaseLogInfo(string logAppCode, string logCode, string details)
        {
            this.loggingTime = System.DateTime.Now;
            this.loggingAppCode = logAppCode;
            this.logCode = logCode;
            this.freeTextDetails = details;
            this.uniqueLogID = PlainToSHA1(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Costruttore che consente alle applicazioni chiamanti di chiamare il log senza
        /// passare in input il codice applicazione che viene letto direttamente dal file di 
        /// configurazione
        /// </summary>
        /// <param name="logCode">codice del log</param>
        /// <param name="details">dettagli del log</param>
        public BaseLogInfo(string logCode, string details)
        {
            this.loggingTime = System.DateTime.Now;
            if (ConfigurationManager.GetSection("ApplicationCode") != null)
                this.loggingAppCode = ((ApplicationCodeConfigSection)ConfigurationManager.GetSection("ApplicationCode")).AppCode;
            this.logCode = logCode;
            this.freeTextDetails = details;
            this.uniqueLogID = PlainToSHA1(Guid.NewGuid().ToString());

        }

        /// <summary>
        /// Data del log
        /// </summary>
        public System.DateTime loggingTime;
        /// <summary>
        /// Codice dell'applicazione legata al log evento
        /// </summary>
        public string loggingAppCode;
        /// <summary>
        /// Codice del log
        /// </summary>
        public string logCode;
        /// <summary>
        /// Messaggio associato alla coda
        /// </summary>
        public string freeTextDetails;
        /// <summary>
        /// Logid univoco
        /// </summary>
        public string uniqueLogID;

        /// <summary>
        /// Metodo che trasforma la stringa in una stringa crittografata
        /// </summary>
        /// <param name="Stringa">stringa da crittografare</param>
        /// <returns><c>string</c> stringa crittografata</returns>
        private static String PlainToSHA1(string Stringa)
        {
            System.Security.Cryptography.SHA1CryptoServiceProvider sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            System.Text.Encoding objEncoding = System.Text.Encoding.UTF8;
            byte[] pwHashed = sha.ComputeHash(objEncoding.GetBytes(Stringa));
            return System.Convert.ToBase64String(pwHashed);
        }

        /// <summary>
        /// Trasforma l'oggetto log in una stringa posizionale
        /// </summary>
        /// <returns><c>string</c> stringa con il log scritto in maniera estesa e posizionale</returns>
        public override string ToString()
        {
            return "LoggingApp:" + loggingAppCode + "|LogTime:" + loggingTime.ToString("dd/MM/yyyy HH:mm:ss") + "|UniqueID:" + uniqueLogID + "|logCode:" + logCode + "|Details:" + freeTextDetails;
        }



        /// <summary>
        /// Metodo che effettua la serializzazione in un XmlDocument dell'oggetto della classe di log
        /// </summary>
        /// <returns><c>XmlDocument</c> XmlDocument serializzato con l'oggetto generico del log</returns>
        public XmlDocument Serialize()
        {
            return ConversionUtils.ObjectToXml(this, null);
        }
        /// <summary>
        /// Deserializzazione del Xml Document letto dalla coda 
        /// </summary>
        /// <param name="doc">Xml Document da Deserializzare</param>
        /// <returns>Classe tipizzata di Tipo UmLogInfo</returns>
        public static BaseLogInfo Deserialize(XmlDocument doc)
        {
            return (BaseLogInfo)ConversionUtils.XmlToObject(doc, typeof(BaseLogInfo));
        }

        #region ICloneable Members

        /// <summary>
        /// Metodo che clona l'oggetto di log
        /// </summary>
        /// <returns><c>object</c> Oggetto generico clonato dalla classe</returns>
        public object Clone()
        {
            //First we create an instance of this specific type.
            object newObject = Activator.CreateInstance(this.GetType());

            //We get the array of fields for the new type instance.
            FieldInfo[] fields = newObject.GetType().GetFields();

            int i = 0;

            foreach (FieldInfo fi in this.GetType().GetFields())
            {
                //We query if the fiels support the ICloneable interface.
                Type ICloneType = fi.FieldType.
                            GetInterface("ICloneable", true);

                if (ICloneType != null)
                {
                    //Getting the ICloneable interface from the object.
                    if (fi.GetValue(this) != null)
                    {
                        ICloneable IClone = (ICloneable)fi.GetValue(this);

                        //We use the clone method to set the new value to the field.
                        fields[i].SetValue(newObject, IClone.Clone());
                    }
                }
                else
                {
                    // If the field doesn't support the ICloneable 
                    // interface then just set it.
                    fields[i].SetValue(newObject, fi.GetValue(this));
                }

                //Now we check if the object support the 
                //IEnumerable interface, so if it does
                //we need to enumerate all its items and check if 
                //they support the ICloneable interface.
                Type IEnumerableType = fi.FieldType.GetInterface
                                ("IEnumerable", true);
                if (IEnumerableType != null)
                {
                    //Get the IEnumerable interface from the field.
                    IEnumerable IEnum = (IEnumerable)fi.GetValue(this);

                    //This version support the IList and the 
                    //IDictionary interfaces to iterate on collections.
                    Type IListType = fields[i].FieldType.GetInterface
                                        ("IList", true);
                    Type IDicType = fields[i].FieldType.GetInterface
                                        ("IDictionary", true);

                    int j = 0;
                    if (IListType != null)
                    {
                        //Getting the IList interface.
                        IList list = (IList)fields[i].GetValue(newObject);

                        foreach (object obj in IEnum)
                        {
                            //Checking to see if the current item 
                            //support the ICloneable interface.
                            ICloneType = obj.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                //If it does support the ICloneable interface, 
                                //we use it to set the clone of
                                //the object in the list.
                                ICloneable clone = (ICloneable)obj;

                                list[j] = clone.Clone();
                            }

                            //NOTE: If the item in the list is not 
                            //support the ICloneable interface then in the 
                            //cloned list this item will be the same 
                            //item as in the original list
                            //(as long as this type is a reference type).

                            j++;
                        }
                    }
                    else if (IDicType != null)
                    {
                        //Getting the dictionary interface.
                        IDictionary dic = (IDictionary)fields[i].
                                            GetValue(newObject);
                        j = 0;

                        foreach (DictionaryEntry de in IEnum)
                        {
                            //Checking to see if the item 
                            //support the ICloneable interface.
                            ICloneType = de.Value.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                ICloneable clone = (ICloneable)de.Value;

                                dic[de.Key] = clone.Clone();
                            }
                            j++;
                        }
                    }
                }
                i++;
            }
            return newObject;
        }

        #endregion

    }
}
