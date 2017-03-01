using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Data;
using System.Collections;
using Com.Delta.Print.Engine;
using System.IO;

namespace Com.Delta.Print
{
   public class HTMLBuilder
    {
        protected ReportDocument reportDoc;
        protected Hashtable parameters;
        protected List<DataTable> tableCollection;
        protected List<Picture> Pictures;
        private System.Xml.XmlDocument tpuXML;

        /// <summary>
        /// Classe associata alla singola immagini presente nel PDF
        /// </summary>
        public class Picture
        {
            private string name;
            private Bitmap image;
            private bool stretch;

            public Picture(string name, Bitmap image, bool stretch)
            {
                this.name = name;
                this.image = image;
                this.stretch = stretch;
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public Bitmap Image
            {
                get { return this.image; }
                set { this.image = value; }
            }

            public bool Stretch
            {
                get { return this.stretch; }
                set { this.stretch = value; }
            }
        }
        
         /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PDFBuilder" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable e le tabelle passate mediante una lista
        /// </summary>
        public HTMLBuilder(Hashtable parameters,
                          List<DataTable> tableCollection)
        {
            this.parameters = parameters;
            this.tableCollection = tableCollection;
            this.reportDoc = new ReportDocument();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PDFBuilder" che
        /// crea l'oggetto di stampa a partire dal PRU passato mediante l'oggetto PRUBuilder
        /// </summary>
        public HTMLBuilder(PRUBuilder pru)
        {
            this.tpuXML = new System.Xml.XmlDocument(); //CC
            this.parameters = pru.parameters;
            this.tableCollection = pru.tableCollection;
            this.reportDoc = new ReportDocument();
        }

        /// <summary>
        /// Creazione dell'array di byte associato al PDF
        /// </summary>
        public byte[] CreateHTMLByte()
        {
            byte[] byteHTML = null;

            if (this.parameters != null)
            {
                this.reportDoc = new ReportDocument();
                this.reportDoc.SetParameters(parameters);
                this.reportDoc.setXML(tpuXML, tpuXML.DocumentElement.Name);
                if (this.tableCollection != null)
                {
                    foreach (DataTable table in this.tableCollection)
                    {
                        reportDoc.AddData(table);                      
                    }
                }

                if (this.Pictures != null)
                {
                    foreach (Picture picture in this.Pictures)
                    {
                        reportDoc.AddPicture(picture.Name, picture.Image, picture.Stretch);
                    }
                }
               byteHTML =reportDoc.SerializeToHtmlStream();    
              
            }
            return byteHTML;
        }


        public string CreateHTMLString()
        {

            string HTML = null;
            if (this.parameters != null)
            {
                this.reportDoc = new ReportDocument();
                this.reportDoc.SetParameters(parameters);
                this.reportDoc.setXML(tpuXML, tpuXML.DocumentElement.Name);
                if (this.tableCollection != null)
                {
                    foreach (DataTable table in this.tableCollection)
                    {
                        reportDoc.AddData(table);
                    }
                }

                if (this.Pictures != null)
                {
                    foreach (Picture picture in this.Pictures)
                    {
                        reportDoc.AddPicture(picture.Name, picture.Image, picture.Stretch);
                    }
                }
               HTML = reportDoc.SerializeToHtmlString();
            }
            return HTML;
        }

        /// <summary>
        /// Si imposta il TPU a partire dal percorso fisico del file
        /// </summary>
        public void setTPU(string tpuPath)
        {
            this.reportDoc.setXML(tpuPath);
            tpuXML.Load(tpuPath);
        }

        /// <summary>
        /// Si imposta il TPU a partire da un array di byte letto dalla banca dati dei TPU.
        /// Per argomento vengono passati l'url dell'Handler per il collegamento alla banca dati
        /// e il codice dell'applicazione e della stampa che identificano univocamente un TPU
        /// </summary>
        public void setTPU(String tpuHandlerServerUrl, String codApp, String codStampa)
        {
            String tpuUrl = tpuHandlerServerUrl + "?codApp=" + codApp + "&codStampa=" + codStampa;
            String tpu = new StreamReader((new MemoryStream(this.getWebObjects(tpuUrl)))).ReadToEnd();

            tpuXML.LoadXml(tpu);

            this.reportDoc.setXML(tpuXML.OuterXml, tpuXML.DocumentElement.Name);

            /* Si verifica se nel tpu letto dalla banca dati sono presenti delle immagini,
             * in caso affermativo, si leggono le immagini corrispondenti dalla banca dati
             * e per ogni una si crea un oggetto 'Picture' */
            System.Xml.XmlNodeList xNodes = tpuXML.SelectNodes("stampa/sections/section/content/pictureBox");
            if (xNodes != null)
            {
                foreach (System.Xml.XmlNode xn in xNodes)
                {
                    System.Xml.XmlNode nodo = xn.SelectSingleNode("file");
                    if (!String.IsNullOrEmpty(nodo.Attributes["value"].Value))
                    {
                        int lio = tpuUrl.LastIndexOf('/');
                        byte[] byteImage = this.getWebObjects(tpuUrl.Substring(0, lio) + "/" + nodo.Attributes["value"].Value);
                        MemoryStream memoryStream = new MemoryStream(byteImage);
                        Bitmap image = (Bitmap)Image.FromStream(memoryStream);
                        this.AddImage(xn.Attributes["name"].Value, image, true);
                    }
                }
            }
        }

        /// <summary>
        /// Si imposta il TPU a partire da un array di byte
        /// </summary>
        public void setTPU(byte[] byteTPU)
        {
            //System.Xml.XmlDocument tpuXML = new System.Xml.XmlDocument();
            tpuXML.LoadXml(new StreamReader((new MemoryStream(byteTPU))).ReadToEnd());
            this.reportDoc.setXML(tpuXML.OuterXml, tpuXML.DocumentElement.Name);
        }

        /// <summary>
        /// Si imposta il TPU a partire da un oggetto XmlDocument
        /// </summary>
        public void setTPU(System.Xml.XmlDocument doc)
        {
            tpuXML = doc;
            this.reportDoc.setXML(doc, doc.DocumentElement.Name);
        }

        public void AddParameter(string tpuFieldName, string value)
        {
            if (this.parameters == null)
            {
                this.parameters = new Hashtable();
            }
            this.parameters.Add(tpuFieldName, value);
        }

        public void RemParameter(string tpuFieldName)
        {
            if (this.parameters != null)
            {
                if (this.parameters.ContainsKey(tpuFieldName))
                {
                    this.parameters.Remove(tpuFieldName);
                }
            }
        }

        public void AddImage(string tpuReferenceName, Bitmap image, bool stretch)
        {
            if (this.Pictures == null)
            {
                this.Pictures = new List<Picture>();
            }
            this.Pictures.Add(new Picture(tpuReferenceName, image, stretch));
        }

        protected byte[] getWebObjects(string url)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            byte[] byteFile = null;

            try
            {
                byteFile = client.DownloadData(url);
            }
            catch (System.Net.WebException wex)
            {
                throw wex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                client.Dispose();
            }

            return byteFile;
        }

   
   }
}
