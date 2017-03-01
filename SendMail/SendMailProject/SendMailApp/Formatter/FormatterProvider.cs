using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMailApp.Formatter
{
    class FormatterProvider
    {

        public static PdfFormatter formatDocument(String FormatName)
        {
            if (FormatName == "PDF") return PdfFormatter.Instance;
            else return null;

        }

        public static PdfFormatterITEXT formatDocumentitext(String FormatName)
        {
            if (FormatName == "PDF") return PdfFormatterITEXT.Instance;
            else return null;

        }
    }
}
