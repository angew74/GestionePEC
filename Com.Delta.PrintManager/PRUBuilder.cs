using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.Web;
using System.Collections.Specialized;

namespace Com.Delta.PrintManager
{
    public class PRUBuilder
    {
        internal String pru;
        protected String url;
        internal Hashtable parameters;
        internal List<DataTable> tableCollection;
        protected HttpResponse response;

        protected bool savePdfOnDisk = false;
        protected bool enableHtmlViewer = false;
        protected bool enablePrintDialog = false;
        protected bool forcePreview = false;
        protected string zoomPreview;
        protected int copie = 1;

        public bool SavePdfOnDisk
        {
            get { return this.savePdfOnDisk; }

            set
            {
                this.savePdfOnDisk = value;

                if (value)
                {
                    this.enableHtmlViewer = false;
                    this.enablePrintDialog = false;
                    this.forcePreview = false;
                    this.zoomPreview = String.Empty;
                }
            }
        }

        public bool EnableHtmlViewer
        {
            get { return this.enableHtmlViewer; }

            set
            {
                this.enableHtmlViewer = value;

                if (value)
                {
                    this.savePdfOnDisk = false;
                    this.enablePrintDialog = false;
                    this.forcePreview = false;
                    this.zoomPreview = String.Empty;
                }
            }
        }

        public bool EnablePrintDialog
        {
            get { return this.enablePrintDialog; }

            set
            {
                this.enablePrintDialog = value;

                if (value)
                {
                    this.savePdfOnDisk = false;
                    this.enableHtmlViewer = false;
                    this.forcePreview = false;
                    this.zoomPreview = String.Empty;
                }
            }
        }

        public bool ForcePreview
        {
            get { return this.forcePreview; }

            set
            {
                this.forcePreview = value;

                if (value)
                {
                    this.savePdfOnDisk = false;
                    this.enableHtmlViewer = false;
                    this.enablePrintDialog = false;
                }
            }
        }

        /// <summary>
        /// Utilizzare Uno dei seguenti Zoom per la Preview: 10% 25% 50% 75% 100% 150% 200% 300% 400% 500%
        /// </summary>
        public string ZoomPreview
        {
            get { return this.zoomPreview; }

            set
            {
                this.zoomPreview = value;

                if (!String.IsNullOrEmpty(value))
                {
                    this.savePdfOnDisk = false;
                    this.enableHtmlViewer = false;
                    this.enablePrintDialog = false;
                    this.forcePreview = true;
                }
            }
        }

        public int Copie
        {
            get { return this.copie; }
            set { this.copie = value; }
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PRUBuilder" che
        /// crea l'oggetto di stampa a partire dal PRU passato in input
        /// e il TPU scaricato dal un url specifico 
        /// </summary>
        public PRUBuilder(String pru, String tpuUrl)
        {
            this.pru = pru;
            this.url = tpuUrl;

            this.parameters = this.GetParametersFromXML();
            this.tableCollection = this.GetTablesFromXML();
            this.GetPrintValuesFromXML();

            //this.pru = this.BuildPRU();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PRUBuilder" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante l'oggetto "Hashtable" per argomento
        /// e il TPU scaricato dal un url specifico 
        /// </summary>
        public PRUBuilder(Hashtable parameters, String tpuUrl)
        {
            this.parameters = parameters;
            this.url = tpuUrl;
            //this.pru = this.BuildPRU();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PRUBuilder" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante l'oggetto "Hashtable" per argomento,
        /// le tabelle passate medainte l'oggetto "List<DataTable>"
        /// e il TPU scaricato dal un url specifico 
        /// </summary>
        public PRUBuilder(Hashtable parameters, List<DataTable> tableCollection, String tpuUrl)
        {
            this.parameters = parameters;
            this.tableCollection = tableCollection;
            this.url = tpuUrl;
            //this.pru = this.BuildPRU();
        }

        /// <summary>
        /// Configurazione del collegamento del TPU sul Repository Remoto
        /// utilizzando il Codice Applicazione e Codice Stampa
        /// </summary>
        public void ConfigureRemoteTpuRepository(String tpuHandlerServerUrl, String codApp, String codStampa)
        {
            this.url = tpuHandlerServerUrl + "?codApp=" + codApp + "&codStampa=" + codStampa;
        }

        ///// <summary>
        ///// Configurazione del collegamento del TPU sul Repository Remoto
        ///// utilizzando il Codice Applicazione e il Nome Stampa
        ///// </summary>
        //public void ConfigureRemoteTpuRepository(String tpuHandlerServerUrl, String codApp, String nomeStampa)
        //{
        //    this.url = tpuHandlerServerUrl + "?codApp=" + codApp + "&nomeStampa=" + nomeStampa;
        //}

        /// <summary>
        /// Configurazione del collegamento del TPU sul Repository Remoto
        /// utilizzando la Chiave di Ricerca Univoca 
        /// </summary>
        public void ConfigureRemoteTpuRepository(String tpuHandlerServerUrl, String chiave)
        {
            this.url = tpuHandlerServerUrl + "?chiave=" + chiave;
        }

        public void AddParameter(string tpuFieldName, string value)
        {
            if (this.parameters == null)
            {
                this.parameters = new Hashtable();
            }
            this.parameters.Add(tpuFieldName, value);
        }

        public void RemoveParameter(string tpuFieldName)
        {
            if (this.parameters != null)
            {
                this.parameters.Remove(tpuFieldName);
            }
        }

        public string GetPRU()
        {
            pru = this.BuildPRU();
            return this.pru;
        }

        public void DownloadPRU()
        {
            if (HttpContext.Current.Response != null)
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "xml/printdatadelta"; //mime-type
                HttpContext.Current.Response.AddHeader("content-disposition", "inline; filename=Stampa.pru");
                HttpContext.Current.Response.Write(GetPRU());
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
        }

        #region private

      

        private String BuildPRU()
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><params><tpu></tpu><pdf>0</pdf><html>0</html>" +
                        "<dialog>0</dialog><preview>0</preview><zoomPreview></zoomPreview><copie>1</copie></params>");

            XmlNode xn = dom.DocumentElement;

            if (parameters != null)
            {
                foreach (System.Collections.DictionaryEntry de in parameters)
                {
                    XmlElement xe = dom.CreateElement("p");

                    XmlAttribute xa = dom.CreateAttribute("name");
                    xa.Value = de.Key.ToString();

                    xe.SetAttributeNode(xa);
                    xe.InnerText = de.Value.ToString();

                    xn.AppendChild(xe);
                }
            }

            if (tableCollection != null)
            {
                foreach (DataTable table in tableCollection)
                {
                    XmlElement xet = dom.CreateElement("t");

                    foreach (DataRow row in table.Rows)
                    {
                        XmlElement xetable = dom.CreateElement(table.TableName);

                        foreach (DataColumn column in table.Columns)
                        {
                            XmlElement xeColumn = dom.CreateElement(column.ColumnName);
                            xeColumn.InnerText = row[column].ToString();
                            xetable.AppendChild(xeColumn);
                        }

                        xet.AppendChild(xetable);
                    }

                    xn.AppendChild(xet);
                }
            }

            //Customization

           
            XmlNodeList nl;

            if (!String.IsNullOrEmpty(this.url))
            {
                nl = dom.GetElementsByTagName("tpu");
                if (nl.Count != 0)
                {
                    XmlElement el = nl.Item(0) as XmlElement;

                    if (el.HasChildNodes)
                    {
                        el.FirstChild.Value = this.url;
                    }
                    else
                    {
                        XmlText testo = dom.CreateTextNode(this.url);
                        el.AppendChild(testo);
                    }
                }
            }

            nl = dom.GetElementsByTagName("pdf");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.savePdfOnDisk == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("html");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.enableHtmlViewer == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("dialog");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.enablePrintDialog == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("preview");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.forcePreview == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("zoomPreview");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;

                if (el.HasChildNodes)
                {
                    el.FirstChild.Value = this.zoomPreview;
                }
                else
                {
                    XmlText testo = dom.CreateTextNode(this.zoomPreview);
                    el.AppendChild(testo);
                }
            }

            nl = dom.GetElementsByTagName("copie");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;

                if (el.HasChildNodes)
                {
                    el.FirstChild.Value = this.copie.ToString();
                }
                else
                {
                    XmlText testo = dom.CreateTextNode(this.copie.ToString());
                    el.AppendChild(testo);
                }
            }

            return dom.InnerXml;


            return dom.InnerXml;
        }

        private Hashtable GetParametersFromXML()
        {
            Hashtable ht = new Hashtable();

            XmlDocument XMLData = new XmlDocument();
            XMLData.LoadXml(this.pru);

            // Estrae i valori dei parametri
            System.Xml.XmlNodeList nl = XMLData.SelectNodes("params/p");
            foreach (System.Xml.XmlNode xn in nl)
            {
                ht.Add(xn.Attributes["name"].Value, xn.InnerText);
            }

            return ht;
        }

        private List<DataTable> GetTablesFromXML()
        {
            List<DataTable> dtList = new List<DataTable>();
            StringCollection sCollection = new StringCollection();

            XmlDocument XMLData = new XmlDocument();
            XMLData.LoadXml(this.pru);

            // Estrae le tabelle
            System.Xml.XmlNodeList nl = XMLData.SelectNodes("params/t");
            foreach (System.Xml.XmlNode xn in nl)
            {
                sCollection.Add(xn.OuterXml);
            }

            DataSet ds = new DataSet();
            System.IO.MemoryStream memStream;
            System.IO.StreamWriter sWriter;

            foreach (string tabella in sCollection)
            {
                ds.Tables.Clear();
                memStream = new System.IO.MemoryStream();
                sWriter = new System.IO.StreamWriter(memStream);
                sWriter.Write(tabella);

                sWriter.Flush();
                memStream.Position = 0;

                ds.ReadXml(memStream);

                if (ds.Tables.Count > 0)
                {
                    dtList.Add(ds.Tables[0]);
                }

                sWriter.Dispose();
                memStream.Dispose();
            }

            return dtList;
        }

        private void GetPrintValuesFromXML()
        {
            XmlDocument XMLData = new XmlDocument();
            XMLData.LoadXml(this.pru);

            this.SavePdfOnDisk = isNodeSet(XMLData, "params/pdf", "1");
            this.EnableHtmlViewer = isNodeSet(XMLData, "params/html", "1");
            this.EnablePrintDialog = isNodeSet(XMLData, "params/dialog", "1");
            this.ForcePreview = isNodeSet(XMLData, "params/preview", "1");

            System.Xml.XmlNode xnode = XMLData.SelectSingleNode("params/copie");
            Int16 rit = 0;
            if (xnode != null)
                if (System.Int16.TryParse(xnode.InnerText, out rit))
                    this.Copie = rit;

            System.Xml.XmlNode xnodeZoomPreview = XMLData.SelectSingleNode("params/zoomPreview");
            if (xnodeZoomPreview != null)
            {
                if (xnodeZoomPreview.InnerText.Contains("10%") ||
                    xnodeZoomPreview.InnerText.Contains("25%") ||
                    xnodeZoomPreview.InnerText.Contains("50%") ||
                    xnodeZoomPreview.InnerText.Contains("75%") ||
                    xnodeZoomPreview.InnerText.Contains("100%") ||
                    xnodeZoomPreview.InnerText.Contains("150%") ||
                    xnodeZoomPreview.InnerText.Contains("200%") ||
                    xnodeZoomPreview.InnerText.Contains("300%") ||
                    xnodeZoomPreview.InnerText.Contains("400%") ||
                    xnodeZoomPreview.InnerText.Contains("500%"))
                {
                    this.ZoomPreview = xnodeZoomPreview.InnerText;
                }
            }
        }

        private bool isNodeSet(System.Xml.XmlDocument XMLData, string NodePath, string ValueToLookFor)
        {
            System.Xml.XmlNode xn = XMLData.SelectSingleNode(NodePath);
            if (xn != null)
            {
                if (xn.InnerText == ValueToLookFor)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
