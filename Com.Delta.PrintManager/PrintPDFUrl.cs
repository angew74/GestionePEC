using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using Com.Delta.PrintManager.Engine;
using System.Data;

namespace Com.Delta.PrintManager
{
    public class PrintPDFUrl : PrintPDF
    {
        private String tpuUrl;

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPDFUrl" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable e il TPU scaricato dall'Url specifico 
        /// </summary>
        public PrintPDFUrl(Hashtable parameters,
                           String tpuUrl, 
                           HttpResponse response)
        {
            this.parameters = parameters;
            this.tpuUrl = tpuUrl;
            this.response = response;
            this.reportDoc = new ReportDocument();
         }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPDFUrl" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable, le tabelle passate mediante una lista
        /// e il TPU scaricato dall'Url specifico
        /// </summary>
        public PrintPDFUrl(Hashtable parameters, 
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

        protected override void setXML()
        {
            this.reportDoc.setXML(this.tpuUrl);
        }
    }
}
