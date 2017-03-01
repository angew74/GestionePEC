using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Web;
using System.Net;
using Com.Delta.PrintManager.Engine;
using System.IO;

namespace Com.Delta.PrintManager
{
    public abstract class PrintPDF : Print
    {
        protected HttpResponse response;

        public override byte[] Invoke()
        {
            byte[] bytePDF = null;

            this.setXML();

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

                bytePDF = reportDoc.SerializeToPdfStream();

                //this.response.ContentType = "application/pdf";
                //this.response.OutputStream.Write(bytePDF, 0, bytePDF.Length);
                //this.response.End();

                //this.reportDoc.Dispose();
            }

            return bytePDF;
        }

        // Scarico gli oggetto dal Web server
        protected byte[] getWebObjects(string tpuUrl)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            byte[] byteTPU = null;

            try
            {
                byteTPU = client.DownloadData(tpuUrl);
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

            return byteTPU;
        }

        protected abstract void setXML();
    }
}
