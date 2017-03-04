using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApacheFop;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using Com.Delta.Logging.Errors;

namespace SendMailApp.Formatter
{
    public sealed class PdfFormatter
    {
        static readonly PdfFormatter instance = new PdfFormatter();
        private static readonly ILog _log = LogManager.GetLogger(typeof(PdfFormatter));

        static PdfFormatter()
        {

        }
        protected string NestingPoint;
        protected System.Xml.XmlNamespaceManager nsmgr;

        PdfFormatter()
        {
            NestingPoint = "fo:flow[@flow-name='xsl-region-body']/fo:block";
            nsmgr = new System.Xml.XmlNamespaceManager(new System.Xml.NameTable());
            nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            nsmgr.AddNamespace("fo", "http://www.w3.org/1999/XSL/Format");
        }

        public static PdfFormatter Instance
        {
            get
            {
                return instance;
            }
        }

        public System.IO.MemoryStream formatData(System.Xml.XmlDocument rawData, System.Xml.XmlDocument xslt)
        {
            string step = XmlUty.XsltToString(rawData, xslt);
            Engine en = new Engine();
            sbyte[] pdf = null;
            try
            {
                pdf = en.Run(step);
            }
            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_PDF_001";
            }
            return StreamUty.sbyteArray2MemoryStream(pdf);
        }

        public byte[] SetMetadati(System.IO.MemoryStream inputPdf, IDictionary<string, string> md)
        {
            inputPdf.Position = 0;
            iTextSharp.text.pdf.PdfReader prd = new iTextSharp.text.pdf.PdfReader(inputPdf);
            iTextSharp.text.Rectangle psize = prd.GetPageSize(1);
            Document doc = new Document(psize, 50, 50, 50, 70);
            System.IO.MemoryStream output = new System.IO.MemoryStream();
            output.Position = 0;

            iTextSharp.text.pdf.PdfWriter wr = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, output);
            doc.AddAuthor(md["author"]);
            doc.AddCreator(md["creator"]);
            doc.AddSubject(md["subject"]);
            wr.SetTagged();

            doc.Open();
            /*
            EndPage pageEvent = new EndPage();
            pageEvent.CIU = md["ciu"];
            pageEvent.IDUfficio = md["id_ufficio"];
            pageEvent.DocumentDate = md["data_emissione"];
 
            wr.PageEvent = pageEvent;
             */
            //wr.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
            wr.SetPdfVersion(PdfWriter.PDF_VERSION_1_6);
            PdfContentByte content = wr.DirectContent;

            for (int i = 1; i <= prd.NumberOfPages; i++)
            {
                doc.NewPage();
                PdfImportedPage pg = wr.GetImportedPage(prd, i);
                content.AddTemplate(pg, 0, 0);
            }
            doc.Close();
            return output.ToArray();
        }



    }
}
