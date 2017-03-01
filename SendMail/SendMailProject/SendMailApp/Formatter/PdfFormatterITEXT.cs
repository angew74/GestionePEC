using Com.Unisys.Logging.Errors;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Xsl;

namespace SendMailApp.Formatter
{
    public class PdfFormatterITEXT
    {
        static readonly PdfFormatterITEXT instance = new PdfFormatterITEXT();
        private static readonly ILog _log = LogManager.GetLogger(typeof(PdfFormatter));

        static PdfFormatterITEXT()
        {

        }
        protected string NestingPoint;
        protected System.Xml.XmlNamespaceManager nsmgr;

        public static PdfFormatterITEXT Instance
        {
            get
            {
                return instance;
            }
        }

        public System.IO.MemoryStream formatData(System.Xml.XmlDocument rawData, System.Xml.XmlDocument xslt)
        {
            byte[] bPDF = null;
            try
            {
                MemoryStream ms = new MemoryStream();              
                System.Web.UI.HtmlControls.HtmlGenericControl gc = new HtmlGenericControl();
                System.IO.MemoryStream inputStream = new System.IO.MemoryStream();
                System.IO.MemoryStream outoputStream = new System.IO.MemoryStream();
                string head = "";
                string foot = "";
                XslCompiledTransform xs = new XslCompiledTransform();
                xs.Load(xslt);
                inputStream.Position = 0;
                rawData.Save(inputStream);
                inputStream.Position = 0;
                System.Xml.XmlTextReader xTextReader = new XmlTextReader(inputStream);
                xs.Transform(rawData, null, outoputStream);
                outoputStream.Seek(0, System.IO.SeekOrigin.Begin);
                System.IO.StreamReader streamReader = new System.IO.StreamReader(outoputStream);
                gc.TagName = "div";
                string output = streamReader.ReadToEnd();               
                // Pulisco e compatto l'output
                string whattostrip = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<xsl:stylesheet version=\"2.0\" ";
                output = output.Replace(whattostrip, String.Empty);
                whattostrip = "<html xmlns=\"http://tempuri.org/DefinizioneResponse.xsd\">";
                output = output.Replace(whattostrip, String.Empty);
                whattostrip = "dd/mm/yyyy";
                output = output.Replace(whattostrip, System.DateTime.Now.ToString("dd/MM/yyyy"));                
                gc.InnerHtml = head + output + foot;
                gc.Dispose();
                streamReader.Close();
                streamReader.Dispose();               
                StringReader txtReader = new StringReader(gc.InnerHtml);
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                MemoryStream m = new MemoryStream();               
                System.Drawing.Image imageDisco = System.Drawing.Image.FromFile(ConfigurationManager.AppSettings["ImageRoma"]);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageDisco, BaseColor.WHITE);
                image.ScaleToFit(336f, 86f);               
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();
                pdfDoc.Add(image);
                htmlparser.Parse(txtReader);
                pdfDoc.Close();
                bPDF = ms.ToArray();

            }

            catch (Exception ex)
            {
                ErrorLogInfo error = new ErrorLogInfo();
                error.freeTextDetails = ex.Message;
                error.logCode = "ERR_PDF_001";
            }
            return StreamUty.byteArray2MemoryStream(bPDF);
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
