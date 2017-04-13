using System.Data;
using System.Linq;
using System.Collections.Generic;
using Com.Delta.Web.Cache;
using ActiveUp.Net.Common.DeltaExt;
using iTextSharp.text;
using ActiveUp.Net.Mail.DeltaExt;
using System.IO;
using iTextSharp.text.pdf;
using System.Configuration;
using SendMail.Model.Wrappers;
using SendMail.BusinessEF.MailFacedes;

namespace GestionePEC.Extensions
{
    public class Helpers
    {
        public static DataTable StampaStatisticaExcel(List<UserResultItem> list, string account,
            string dati, string dataf)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("Utente", typeof(string));
            dt.Columns.Add("Totale", typeof(decimal));
            for (int i = 0; i < list.Count; i++)
            {
                dt.Rows.Add(list[i].Account, list[i].User, list[i].Operazioni);
            }
            return dt;
        }
        public static byte[] StampaStatisticaITEXT(List<UserResultItem> list, string account, string datai, string dataf)
        {
            // step 1: creation of a document-object
            Document document = new Document(PageSize.A4.Rotate(), 10, 10, 70, 15);
            MemoryStream ms = new MemoryStream();
            // step 2: we create a writer that listens to the document
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(ConfigurationManager.AppSettings["Image"]);
            MyPageEventHandler e = new MyPageEventHandler()
            {
                ImageHeader = imageHeader,
                Titolo = "N. righe : " + list.Count.ToString() + " - Email: " + account + " intervallo date : " + datai + " - " + dataf
            };
            writer.PageEvent = e;
            //set some header stuff
            document.AddTitle("Statistica Lavorazioni");
            document.AddSubject("Statistica Lavorazioni");
            document.AddCreator("Roma Capitale");
            document.AddAuthor("n.r.");

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            CollectionToPDFStat(ref document, list);

            // step 5: we close the document
            document.Close();
            return ms.ToArray();
        }
        public static byte[] StampaEmailAttoITEXT(List<MailHeaderExtended> dt, string account, string cartella, string datai, string dataf, string parentFolder, string accountid)
        {

            // step 1: creation of a document-object
            Document document = new Document(PageSize.A4.Rotate(), 10, 10, 70, 15);
            MemoryStream ms = new MemoryStream();
            // step 2: we create a writer that listens to the document
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            iTextSharp.text.Image imageHeader = iTextSharp.text.Image.GetInstance(ConfigurationManager.AppSettings["Image"]);
            imageHeader.ScaleToFit(400, 80);
            MyPageEventHandler e = new MyPageEventHandler()
            {
                ImageHeader = imageHeader,
                Titolo = "N. righe : " + dt.Count.ToString() + " - Email: " + account + " Cartella: " + cartella + " intervallo date : " + datai + " - " + dataf
            };
            writer.PageEvent = e;
            //set some header stuff
            document.AddTitle("Elenco Email");
            document.AddSubject("Elenco Email");
            document.AddCreator("Roma Capitale");
            document.AddAuthor("n.r.");

            // step 3: we open the document
            document.Open();

            // step 4: we add content to the document
            CollectionToPDFTableAtti(ref document, dt, parentFolder, accountid);

            // step 5: we close the document
            document.Close();
            return ms.ToArray();

        }


        private static void CollectionToPDFStat(ref Document document, List<UserResultItem> list)
        {
            int cols = 3;
            PdfPTable pdfTable = new PdfPTable(cols);
            pdfTable.DefaultCell.Padding = 2;
            pdfTable.WidthPercentage = 100; // percentage
            pdfTable.DefaultCell.BorderWidth = 2;
            pdfTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            int rows = list.Count;
            pdfTable.AddCell(FormatHeaderPhrase("CASELLA"));
            pdfTable.AddCell(FormatHeaderPhrase("UTENTE"));
            pdfTable.AddCell(FormatHeaderPhrase("TOTALE"));
            pdfTable.HeaderRows = 1;  // this is the end of the table header
            pdfTable.DefaultCell.BorderWidth = 1;
            foreach (UserResultItem row in list)
            {

                pdfTable.AddCell(FormatPhraseEvento(row.Account));
                pdfTable.AddCell(FormatPhraseEvento(row.User));
                pdfTable.AddCell(FormatPhraseEvento(row.Operazioni));
            }
            document.Add(pdfTable);
        }

        private static void CollectionToPDFTableAtti(ref Document document, List<MailHeaderExtended> beans, string parentFolder, string accountid)
        {

            int cols = 7;
            PdfPTable pdfTable = new PdfPTable(cols);
            pdfTable.DefaultCell.Padding = 2;
            pdfTable.WidthPercentage = 100; // percentage
            pdfTable.DefaultCell.BorderWidth = 2;
            pdfTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            int rows = beans.Count;
            float[] columnWidths = new float[] { 20f, 15f, 40f, 40f, 40f, 10f, 10f };
            pdfTable.SetWidths(columnWidths);
            pdfTable.AddCell(FormatHeaderPhrase("MAIL"));
            pdfTable.AddCell(FormatHeaderPhrase("UTENTE"));
            pdfTable.AddCell(FormatHeaderPhrase("MITTENTE"));
            pdfTable.AddCell(FormatHeaderPhrase("DESTINATARIO"));
            pdfTable.AddCell(FormatHeaderPhrase("OGGETTO"));
            pdfTable.AddCell(FormatHeaderPhrase("DATA MAIL"));
            pdfTable.AddCell(FormatHeaderPhrase("CARTELLA"));
            pdfTable.HeaderRows = 1;  // this is the end of the table header
            pdfTable.DefaultCell.BorderWidth = 1;
            foreach (MailHeaderExtended row in beans)
            {

                string mail = string.Concat(ConfigurationManager.AppSettings["HOST"], "/", row.UniqueId, "/", row.Dimensione, "/", row.FolderId, "/", row.MailRating, "/", parentFolder, "/", accountid);
                Font link = FontFactory.GetFont(FontFactory.TIMES, 8, Font.UNDERLINE, BaseColor.RED);
                Anchor anchor = new Anchor("VISUALIZZA MAIL", link);
                anchor.Reference = mail;
                PdfPCell cell = new PdfPCell(anchor);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfTable.AddCell(cell);
                pdfTable.AddCell(FormatPhraseEvento(row.Utente));
                pdfTable.AddCell(FormatPhraseEvento(row.From));
                pdfTable.AddCell(FormatPhraseEvento(row.To));
                pdfTable.AddCell(FormatPhraseEvento(row.Subject));
                pdfTable.AddCell(FormatPhraseEvento(row.Date.ToString("dd/MM/yyyy")));
                pdfTable.AddCell(FormatPhraseEvento(row.NomeFolder.ToString()));
            }
            document.Add(pdfTable);

        }


        #region Stampa Metodi utili stampe EMAILS

        private static Phrase FormatHeaderPhrase(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLDITALIC, 8, iTextSharp.text.Font.ITALIC, BaseColor.BLUE));
        }

        private static Phrase FormatPhrase(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES, 5));
        }
        private static Phrase FormatPhraseEvento(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES, 8));
        }

        private static Phrase FormatPhraseBold(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLD, 5, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        }

        private static Phrase FormatPhraseBoldEvento(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        }

        private static Phrase FormatPageHeaderPhrase(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
        }

        private class MyPageEventHandler : PdfPageEventHelper
        {
            public iTextSharp.text.Image ImageHeader { get; set; }

            public string Titolo { get; set; }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // cell height 
                float cellHeight = document.TopMargin;
                // PDF document size      
                iTextSharp.text.Rectangle page = document.PageSize;

                // create two column table
                PdfPTable head = new PdfPTable(2);
                head.TotalWidth = page.Width;

                // add image; PdfPCell() overload sizes image to fit cell
                PdfPCell c = new PdfPCell(ImageHeader, true);
                // c.HorizontalAlignment = Element.ALIGN_RIGHT;
                c.HorizontalAlignment = Element.ALIGN_LEFT;
                c.FixedHeight = cellHeight;
                c.Border = PdfPCell.NO_BORDER;
                head.AddCell(c);

                // add the header text
                c = new PdfPCell(new Phrase(
                    Titolo,
                  new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 9)
                ));
                c.Border = PdfPCell.NO_BORDER;
                c.VerticalAlignment = Element.ALIGN_BOTTOM;
                c.FixedHeight = cellHeight;
                head.AddCell(c);
                Paragraph footer = new Paragraph("Elaborata il : " + System.DateTime.Now.ToString("dd-MM-yyyy HH:mm"), FontFactory.GetFont(FontFactory.TIMES, 7, iTextSharp.text.Font.NORMAL));
                footer.Alignment = Element.ALIGN_RIGHT;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 350;
                footerTbl.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell cell = new PdfPCell(footer);
                cell.Border = 0;
                cell.PaddingLeft = 10;

                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 550, 30, writer.DirectContent);
                // since the table header is implemented using a PdfPTable, we call
                // WriteSelectedRows(), which requires absolute positions!
                head.WriteSelectedRows(
                  0, -1,  // first/last row; -1 flags all write all rows
                  0,      // left offset
                          // ** bottom** yPos of the table
                  page.Height - cellHeight + head.TotalHeight,
                  writer.DirectContent
                );
            }
        }

        #endregion

        public static string GetTipo(string folderid)
        {
            string tipo = string.Empty;
            MailLocalService mailLocalService = new MailLocalService();
            string azione = string.Empty;
            List<Folder> getFolders = CacheManager<List<Folder>>.get(CacheKeys.FOLDERS, VincoloType.NONE);
            if (getFolders == null)
            {
                getFolders = mailLocalService.getAllFolders();
            }
            int id = 0;
            int.TryParse(folderid, out id);
            var idnome = (from f in getFolders
                          where f.Id == id
                          select f.IdNome).FirstOrDefault();
            if (idnome == "3")
            { return "R"; }
            var t = (from a in getFolders
                     where a.Id == id 
                     select a.TipoFolder);            
            tipo = t.FirstOrDefault();
            return tipo;

        }
        public static string GetAzione(string ActionId)
        {
            string azione = string.Empty;
            MailLocalService localService = new MailLocalService();
            List<ActiveUp.Net.Common.DeltaExt.Action> getActions = localService.GetFolderDestinationForAction();
            int id = 0;
            int.TryParse(ActionId, out id);
            var t = (from a in getActions
                     where a.Id == id
                     select a.NomeAzione);
            azione = t.First();
            return azione;
        }
        public static bool CanDo(string parentFolder, string mailAction, List<string> mailIds)
        {
            bool ret = false;
            if ((parentFolder == "O" || parentFolder == "D" || mailAction != "0" || parentFolder == "AO" || parentFolder == "I") && mailIds.Count > 0 && mailIds[0] != string.Empty)
            { return true; }
            else
            { return false; }
        }
    }
}
