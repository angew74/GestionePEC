using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Collections;
using System.Web;

namespace Com.Delta.PrintManager
{
    public class FlushPRUBuilt : Flush
    {
        protected bool pdf = false;
        protected bool html = false;
        protected bool dialog = false;
        protected bool preview = false;
        protected string zoomPreview;
        protected int copie = 1;

        public bool Pdf
        {
            get { return this.pdf; }

            set
            {
                this.pdf = value;

                if (value)
                {
                    this.html = false;
                    this.dialog = false;
                    this.preview = false;
                    this.zoomPreview = String.Empty;
                }
            }
        }

        public bool Html
        {
            get { return this.html; }

            set
            {
                this.html = value;

                if (value)
                {
                    this.pdf = false;
                    this.dialog = false;
                    this.preview = false;
                    this.zoomPreview = String.Empty;
                }
            }
        }

        public bool Dialog
        {
            get { return this.dialog; }

            set
            {
                this.dialog = value;

                if (value)
                {
                    this.pdf = false;
                    this.html = false;
                    this.preview = false;
                    this.zoomPreview = String.Empty;
                }
            }
        }

        public bool Preview
        {
            get { return this.preview; }

            set
            {
                this.preview = value;

                if (value)
                {
                    this.pdf = false;
                    this.html = false;
                    this.dialog = false;
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
                    this.pdf = false;
                    this.html = false;
                    this.dialog = false;
                    this.preview = true;
                }
            }
        }

        public int Copie
        {
            get { return this.copie; }
            set { this.copie = value; }
        }


        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPRUBuilt" che
        /// crea l'oggetto di stampa a partire dal PRU passato in input
        /// e il TPU scaricato dal un url o dall' Handler specifico 
        /// </summary>
        public FlushPRUBuilt(String pru, String tpuUrl, HttpResponse response)
        {
            this.pru = pru;
            this.url = tpuUrl;
            this.response = response;
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPRUBuilt" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante l'oggetto "System.Collections.Hashtable" per argomento
        /// e il TPU scaricato dal un url specifico 
        /// </summary>
        public FlushPRUBuilt(String tpuUrl, Hashtable parameters, HttpResponse response)
        {
            this.parameters = parameters;
            this.url = tpuUrl;
            this.response = response;
            this.pru = this.BuildPRU();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPRUBuilt" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante l'oggetto "System.Collections.Hashtable" per argomento,
        /// le tabelle passate medainte l'oggetto "System.Collections.Generic.List"
        /// e il TPU scaricato dal un url o dall' Handler specifico 
        /// </summary>
        public FlushPRUBuilt(Hashtable parameters, List<DataTable> tableCollection, String tpuUrl, HttpResponse response)
        {
            this.parameters = parameters;
            this.tableCollection = tableCollection;
            this.url = tpuUrl;
            this.response = response;
            this.pru = this.BuildPRU();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPRUBuilt" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante l'oggetto "System.Collections.Hashtable" per argomento,
        /// le tabelle passate medainte l'oggetto "System.Collections.Generic.List"
        /// e il TPU scaricato dall'Handler specifico 
        /// con chiavi di ricerca il nome della stampa e dell'applicazione
        /// </summary>
        public FlushPRUBuilt(Hashtable parameters, String handlerUrl, HttpResponse response)
        {
            this.parameters = parameters;
            this.url = handlerUrl;
            this.response = response;
            this.pru = this.BuildPRU();
        }

        public void set_CodApp_Stampa_ForHandler(String codApp, String stampa)
        {
            this.url = url + "?codApp=" + codApp + "&stampa=" + stampa;
        }

        public void set_CodApp_Chiave_ForHandler(String codApp, String chiave)
        {
            this.url = url + "?codApp=" + codApp + "&chiave=" + chiave;
        }

        public override void Invoke()
        {
            this.pru = this.CustomizePRU();

            this.response.Clear();
            this.response.ContentType = "xml/printdatadelta"; //mime-type
            this.response.AddHeader("content-disposition", "inline; filename=Stampa.pru");
            this.response.Write(pru);
            this.response.Flush();
            this.response.Close();
        }

        protected String CustomizePRU()
        {
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(this.pru);

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
                el.FirstChild.Value = (this.pdf == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("html");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.html == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("dialog");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.dialog == true ? "1" : "0");
            }

            nl = dom.GetElementsByTagName("preview");
            if (nl.Count != 0)
            {
                XmlElement el = nl.Item(0) as XmlElement;
                el.FirstChild.Value = (this.preview == true ? "1" : "0");
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
        }

        protected String BuildPRU()
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

            return dom.InnerXml;
        }
    }
}
