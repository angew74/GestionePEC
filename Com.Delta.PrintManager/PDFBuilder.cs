using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using Com.Delta.PrintManager.Engine;
using System.Data;
using System.Drawing;
using System.IO;

namespace Com.Delta.PrintManager
{
    public class PDFBuilder
    {
        protected ReportDocument reportDoc;
        protected Hashtable parameters;
        protected List<DataTable> tableCollection;
        protected List<Picture> Pictures;

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
        public PDFBuilder(Hashtable parameters,
                          List<DataTable> tableCollection)
        {
            this.parameters = parameters;
            this.tableCollection = tableCollection;
            this.reportDoc = new ReportDocument();
        }

        public PDFBuilder(PRUBuilder pru)
        {
            this.parameters = pru.parameters;
            this.tableCollection = pru.tableCollection;
            this.reportDoc = new ReportDocument();
        }

        public byte[] CreatePDF()
        {
            byte[] bytePDF = null;

            if (this.parameters != null)
            {
                this.reportDoc.SetParameters(parameters);

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

                bytePDF = reportDoc.SerializeToPdfStream();

                //this.response.ContentType = "application/pdf";
                //this.response.OutputStream.Write(bytePDF, 0, bytePDF.Length);
                //this.response.End();

                //this.reportDoc.Dispose();
            }

            return bytePDF;
        }


        public void setTPU(string tpuPath)
        {
            this.reportDoc.setXML(tpuPath);
        }

        public void setTPU(String tpuHandlerServerUrl, String codApp, String codStampa)
        {
            String tpuUrl = tpuHandlerServerUrl + "?codApp=" + codApp + "&codStampa=" + codStampa;
            String tpu = new StreamReader((new MemoryStream(this.getWebObjects(tpuUrl)))).ReadToEnd();

            System.Xml.XmlDocument tpuXML = new System.Xml.XmlDocument();
            tpuXML.LoadXml(tpu);

            this.reportDoc.setXML(tpuXML.OuterXml, tpuXML.DocumentElement.Name);

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

        public void setTPU(byte[] byteTPU)
        {
            System.Xml.XmlDocument tpuXML = new System.Xml.XmlDocument();
            tpuXML.LoadXml(new StreamReader((new MemoryStream(byteTPU))).ReadToEnd());
            this.reportDoc.setXML(tpuXML.OuterXml, tpuXML.DocumentElement.Name);
        }

        public void setTPU(System.Xml.XmlDocument doc)
        {
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
              if(this.parameters.ContainsKey(tpuFieldName))
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
