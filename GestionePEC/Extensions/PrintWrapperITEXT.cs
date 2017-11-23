using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using SendMail.Model.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestionePEC.Extensions
{
    public class PrintWrapperITEXT
    {
        static readonly ILog log = LogManager.GetLogger(typeof(PrintWrapperITEXT));

        iTextSharp.text.Font GlobalFont = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 12, iTextSharp.text.BaseColor.BLACK);
        iTextSharp.text.Font GlobalFontBold = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 12, iTextSharp.text.BaseColor.BLACK);

        internal static byte[] StampaStat(List<UserResultItem> dt)
        {
            System.IO.MemoryStream m = new System.IO.MemoryStream();
            Document doc = new Document(PageSize.A4, 10, 10, 10, 10);
            try
            {

                PdfWriter writer = PdfWriter.GetInstance(doc, m);
                doc.AddTitle("Stampa Statistica Utente");
                doc.AddSubject("Statistica lavorazioni per Utente");
                doc.AddCreator("deltasi.it");
                doc.AddAuthor("N.R.");
                doc.Open();
                PdfPTable table = new PdfPTable(3);
                table.DefaultCell.BorderWidth = 1;
                //table.WidthPercentage = 100;             
                PdfPCell cell = new PdfPCell(new Phrase("Utenti con Lavorazioni         - Totale righe: " + dt.Count.ToString() + " - Data Elaborazione : " + DateTime.Now.ToString("dd-MM-yyyy HH:mm"), FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK)));
                cell.Colspan = 3;
                table.AddCell(cell);
                table.AddCell(FormatHeaderPhrase("Casella Mail"));
                table.AddCell(FormatHeaderPhrase("Utente"));
                table.AddCell(FormatHeaderPhrase("Totale"));
                foreach (UserResultItem c in dt)
                {
                    table.AddCell(FormatPhraseEvento(c.User));
                    table.AddCell(FormatPhraseEvento(c.Account));
                    table.AddCell(FormatPhraseEvento(c.Operazioni));
                }
                doc.Add(table);
                doc.Close();

            }
            catch (Exception ex)
            {

            }
            return m.ToArray();
        }

        private static Phrase FormatHeaderPhrase(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLDITALIC, 8, iTextSharp.text.Font.ITALIC, iTextSharp.text.BaseColor.BLUE));
        }

        private static Phrase FormatHeaderPhraseEvento(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLDITALIC, 9, iTextSharp.text.Font.ITALIC, iTextSharp.text.BaseColor.BLUE));
        }

        private static Phrase FormatPhrase(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES, 8));
        }
        private static Phrase FormatPhraseEvento(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES, 11));
        }

        private static Phrase FormatPhraseBold(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
        }

        private static Phrase FormatPhraseBoldEvento(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES_BOLD, 11, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
        }

        private static Phrase FormatPageHeaderPhrase(string value)
        {
            return new Phrase(value, FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK));
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
                c.HorizontalAlignment = Element.ALIGN_RIGHT;
                c.FixedHeight = cellHeight;
                c.Border = PdfPCell.NO_BORDER;
                head.AddCell(c);

                // add the header text
                c = new PdfPCell(new Phrase(
                    Titolo + " " +
                  DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
                  new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 9)
                ));
                c.Border = PdfPCell.NO_BORDER;
                c.VerticalAlignment = Element.ALIGN_BOTTOM;
                c.FixedHeight = cellHeight;
                head.AddCell(c);
                head.WriteSelectedRows(
                  0, -1,  // first/last row; -1 flags all write all rows
                  0,      // left offset
                          // ** bottom** yPos of the table
                  page.Height - cellHeight + head.TotalHeight,
                  writer.DirectContent
                );
            }
        }
    }
}