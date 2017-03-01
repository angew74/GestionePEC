using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections;
using System.Data;
using Com.Delta.PrintManager.Engine;

namespace Com.Delta.PrintManager
{
    public class PrintPDFByte : PrintPDF
    {
        private byte[] byteTPU;

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPDFByte" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable e il TPU passato tramite un array di byte
        /// </summary>
        public PrintPDFByte(Hashtable parameters, 
                            byte[] byteTPU,
                            HttpResponse response)
        {
            this.parameters = parameters;
            this.response = response;
            this.byteTPU = byteTPU;
            this.reportDoc = new ReportDocument();
        }

        /// <summary>
        /// Inizializza una Nuova Istanza della Classe "PrintPDFByte" che
        /// crea l'oggetto di stampa a partire dai parametri passati 
        /// mediante un Hashtable, le tabelle passate mediante una lista
        /// e il TPU passato tramite un array di byte
        /// </summary>
        public PrintPDFByte(Hashtable parameters,
                            List<DataTable> tableCollection,
                            byte[] byteTPU,
                            HttpResponse response)
        {
            this.parameters = parameters;
            this.tableCollection = tableCollection;
            this.response = response;
            this.reportDoc = new ReportDocument();
        }

        protected override void setXML()
        {
            this.reportDoc.setXML((new System.Text.UTF8Encoding()).GetString(byteTPU));
        }
    }
}
