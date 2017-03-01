using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Web;
using Com.Delta.PrintManager.Engine;
using System.Drawing;

namespace Com.Delta.PrintManager
{
    public class PrintPDFHandler : PrintPDF
    {
        private String tpuUrl;

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPDF" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable, e il TPU scaricato dall'Handler specifico 
        /// </summary>
        public PrintPDFHandler(Hashtable parameters,
                               String tpuUrl,
                               HttpResponse response)
        {
            this.parameters = parameters;
            this.tpuUrl = tpuUrl;
            this.response = response;
            this.reportDoc = new ReportDocument();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPDF" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable, le tabelle passate mediante una lista
        /// e il TPU scaricato dall'Handler specifico 
        /// </summary>
        public PrintPDFHandler(Hashtable parameters,
                               List<DataTable> tableCollection,
                               String tpuUrl,
                               HttpResponse response)
        {
            this.parameters = parameters;
            this.tableCollection = tableCollection;
            this.tpuUrl = tpuUrl;
            this.response = response;
            this.reportDoc = new ReportDocument();
        }

        public void set_CodApp_Stampa(String codApp, String stampa)
        {
            this.tpuUrl = tpuUrl + "?codApp=" + codApp + "&stampa=" + stampa;
        }

        public void set_CodApp_Chiave(String codApp, String chiave)
        {
            this.tpuUrl = tpuUrl + "?codApp=" + codApp + "&chiave=" + chiave;
        }

        protected override void setXML()
        {
            String tpu = (new System.Text.UTF8Encoding()).GetString(this.getWebObjects(tpuUrl));

            this.reportDoc.setXML(tpu);
            System.Xml.XmlDocument tpuXML = new System.Xml.XmlDocument();

            tpuXML.Load(tpu);
            System.Xml.XmlNodeList xNodes = tpuXML.SelectNodes("stampa/sections/section/content/pictureBox");
            if (xNodes != null)
            {
                foreach (System.Xml.XmlNode xn in xNodes)
                {
                    System.Xml.XmlNode nodo = xn.SelectSingleNode("file");
                    if (!String.IsNullOrEmpty(nodo.Attributes["value"].Value))
                    {
                        int lio = tpu.LastIndexOf('/');
                        Bitmap image = new Bitmap((new System.Text.UTF8Encoding()).GetString(this.getWebObjects(tpuUrl.Substring(0, lio) + nodo.Attributes["value"].Value)));
                        this.reportDoc.AddPicture(nodo.Attributes["value"].Value, image);
                    }
                }
            }
        }
    }
}
