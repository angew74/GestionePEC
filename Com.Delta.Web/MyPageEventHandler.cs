using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Delta.Web
{
    public class MyPageEventHandler : PdfPageEventHelper
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
            int pageN = writer.PageNumber;
            // add image; PdfPCell() overload sizes image to fit cell
            PdfPCell c = new PdfPCell(ImageHeader);
            c.HorizontalAlignment = Element.ALIGN_RIGHT;
            //  c.FixedHeight = cellHeight;
            String text = "Pagina " + writer.PageNumber;
            c.Border = PdfPCell.NO_BORDER;
            head.AddCell(c);

            // add the header text
            c = new PdfPCell(new Phrase(
                Titolo + " " +
              DateTime.Now.ToString("dd-MM-yyyy HH:mm"),
              new iTextSharp.text.Font(iTextSharp.text.Font.TIMES_ROMAN, 9)
            ));
            c.Border = PdfPCell.NO_BORDER;
            c.VerticalAlignment = Element.ALIGN_BOTTOM;
            c.FixedHeight = cellHeight;
            head.AddCell(c);
            PdfContentByte cb = writer.DirectContent;
            cb.BeginText();
            BaseFont bf = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            cb = writer.DirectContent; ;
            cb.SetFontAndSize(bf, 7);
            cb.SetTextMatrix(document.PageSize.GetRight(180), document.PageSize.GetBottom(30));
            cb.ShowText(text);
            cb.EndText();
            PdfTemplate footerTemplate;
            footerTemplate = cb.CreateTemplate(50, 50);
            float len = bf.GetWidthPoint(text, 12);
            cb.AddTemplate(footerTemplate, document.PageSize.GetRight(180) + len, document.PageSize.GetBottom(30));

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
}
