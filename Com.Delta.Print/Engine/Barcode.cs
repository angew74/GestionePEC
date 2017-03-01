
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml;
using System.Text;

namespace Com.Delta.Print.Engine
{

    /// <summary>
    /// Report element for displaying barcodes.
    /// 
    /// Barcode content (data) is set through Text property and the 
    /// barcode type (barcode standard) is set using BarcodeType property.
    /// </summary>

    public sealed class Barcode : ICustomPaint
    {

        private string text = "012345";
        private bool showLabel = true;
        private Font labelFont = new Font("Courier New", 10);
        private BarcodeTypes barcodeType = BarcodeTypes.Code39;

        /// <summary>
        /// Enumeration of barcode styles for the Barcode element
        /// </summary>
        public enum BarcodeTypes { Codabar, Code39, EAN128UCC, EAN13, EAN8, I2of5, UPCA, UPCE };

        public Barcode(int x, int y, int width, int height, Section parent)
        {
            section = parent;
            theRegion = new Rectangle(x, y, width, height);
            this.Bounds = theRegion;
            name = "barcode0";
        }

        public Barcode(XmlNode node, Section parent)
        {
            section = parent;
            Init(node);
        }

        /// <summary>
        /// Gets the string representation of report element.
        /// </summary>
        public override string ToString()
        {
            return "Barcode [" + this.Name + "]";
        }

        /// <summary>
        /// Gets or sets the name of the report element.
        /// </summary>
        /// <remarks>This property is used in code when setting other properties programmaticly.</remarks>
        [Category("Data"), Description("The name of the chart. This property is used in code when setting other properties programmaticly.")]
        public override string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// Paints the barcode element.
        /// </summary>
        /// <param name="g">The Graphics object to draw</param>
        /// <remarks>Causes the barcode element to be painted to the drawing device.</remarks>
        public override void Paint(Graphics g)
        {

            switch (barcodeType)
            {
                case BarcodeTypes.Code39:
                    PaintCode39(g);
                    break;

                case BarcodeTypes.I2of5:
                    PaintI2of5(g);
                    break;

                case BarcodeTypes.UPCA:
                    PaintUPCA(g);
                    break;

                case BarcodeTypes.UPCE:
                    PaintUPCE(g);
                    break;

                case BarcodeTypes.Codabar:
                    PaintCodabar(g);
                    break;

                case BarcodeTypes.EAN13:
                    PaintEAN13(g);
                    break;

                case BarcodeTypes.EAN8:
                    PaintEAN8(g);
                    break;

                case BarcodeTypes.EAN128UCC:
                    PaintEAN128(g);
                    break;

            }

        }


        /// <summary>
        /// Clones the structure of the barcode element, including all properties.
        /// </summary>
        /// <returns><see cref="Com.Delta.Print.Engine.ChartBox">Com.Delta.Print.Engine.ChartBox</see></returns>
        public override object Clone()
        {
            Barcode tmp = new Barcode(0, 0, 0, 0, section);
            tmp.X = this.X;
            tmp.Y = this.Y;
            tmp.Width = this.Width;
            tmp.Height = this.Height;
            tmp.Layout = this.Layout;
            tmp.Name = "barcode" + tmp.GetHashCode().ToString();
            tmp.BarcodeType = this.BarcodeType;
            tmp.Text = this.Text;
            tmp.LabelFont = this.LabelFont;
            tmp.ShowLabel = this.ShowLabel;

            return tmp;
        }


        /// <summary>
        /// Gets or sets the barcode type.
        /// </summary>
        /// <remarks>Gets or sets the barcode type</remarks>
        [Category("Data"), DefaultValue(BarcodeTypes.Code39), Description("The type of the barcode.")]
        public BarcodeTypes BarcodeType
        {
            get { return barcodeType; }
            set { barcodeType = value; }

        }

        /// <summary>
        /// Gets or sets the text content of the barcode.
        /// </summary>
        /// <remarks>Gets or sets the text content of the barcode.</remarks>
        [Category("Data"), Description("The text content of the barcode.")]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets or sets the whether barcode label is displayed.
        /// </summary>
        /// <remarks>Gets or sets the whether barcode label is displayed.</remarks>
        [Category("Data"), DefaultValue(true), Description("Gets or sets the whether barcode label is displayed.")]
        public bool ShowLabel
        {
            get { return showLabel; }
            set { showLabel = value; }
        }

        /// <summary>
        /// Gets or sets the label font.
        /// </summary>
        /// <remarks>Gets or sets the label font.</remarks>
        [Category("Data"), DefaultValue(typeof(System.Drawing.Font), "Courier New, 10pt"), Description("Label font of barcode.")]
        public Font LabelFont
        {
            get { return labelFont; }
            set { labelFont = value; }
        }

        /// <summary>
        /// Chart image for printing and exporting purpose
        /// </summary>
        [Browsable(false)]
        public Bitmap Image
        {
            get
            {
                Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;

                Bitmap barcodeImage = new Bitmap(paintArea.Width, paintArea.Height);

                Graphics imageGraphics = Graphics.FromImage(barcodeImage);

                Matrix m = new Matrix();
                m.Translate(-paintArea.X, -paintArea.Y, MatrixOrder.Append);
                imageGraphics.Transform = m;

                this.Paint(imageGraphics);
                imageGraphics.Dispose();

                return barcodeImage;
            }
        }

        private void PaintException(Graphics g, Exception ex)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End
            
            // invalid text content for this type of barcode
            // draw error message in element area
            string errorMessage = "Text " + text + " can not be represented in " + this.barcodeType.ToString() + " standard.";

            if (ex.Message == "Not enough space.")
            {
                errorMessage = "Not enough space to display data.";
            }

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(errorMessage, displayFont, Brushes.Black, paintArea, sf);
        }

        private void PaintUPCA(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            try
            {
                // check if text content is valid for this type of barcode

                if (text.Length != 12)
                    throw new Exception("Number of data elements must be 12.");

                for (int i = 0; i < text.Length; i++)
                {
                    if (!Char.IsDigit(text[i]))
                    {
                        throw new Exception("UPC-A standard supports only digit data elements.");
                    }
                }

                string quietZone = "000000000";
                string lead = "101";
                string separator = "01010";

                string[] leftDigits = new string[] { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
                string[] rightDigits = new string[] { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };


                string encodedString = string.Empty;

                encodedString += quietZone;
                encodedString += lead;

                string productType = text.Substring(0, 1);
                string manufacturerCode = text.Substring(1, 5);
                string productCode = text.Substring(6, 5);

                int checksum = 0;
                for (int i = 0; i < 11; i++)
                {
                    int digit = Convert.ToInt32(text[i].ToString());

                    if (digit % 2 == 0)
                        checksum += digit * 3;
                    else
                        checksum += digit;
                }

                checksum = checksum % 10;
                checksum = (10 - checksum) % 10;


                encodedString += leftDigits[Convert.ToInt32(productType)];

                for (int i = 0; i < manufacturerCode.Length; i++)
                {
                    encodedString += leftDigits[Convert.ToInt32(manufacturerCode[i].ToString())];
                }

                encodedString += separator;

                for (int i = 0; i < productCode.Length; i++)
                {
                    encodedString += rightDigits[Convert.ToInt32(productCode[i].ToString())];
                }

                encodedString += rightDigits[checksum];

                encodedString += lead;
                encodedString += quietZone;


                int encodedStringLength = encodedString.Length;

                double wideToNarrowRatio = 3;
                double numberOfLines = encodedStringLength;


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);

                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }

                float x = paintArea.X;
                for (int i = 0; i < encodedStringLength; i++)
                {
                    g.FillRectangle(encodedString[i] == '1' ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), x, paintArea.Y, weight, paintArea.Height - 1);
                    x += weight;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                if (showLabel)
                {
                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF productTypeBox = new RectangleF(paintArea.X, paintArea.Y + paintArea.Height - labelHeight, 9 * weight, labelHeight);
                        RectangleF checkSumBox = new RectangleF(paintArea.X + paintArea.Width - 9 * weight, paintArea.Y + paintArea.Height - labelHeight, 9 * weight, labelHeight);
                        RectangleF manufacturerCodeBox = new RectangleF(paintArea.X + 19 * weight, paintArea.Y + paintArea.Height - labelHeight, 35 * weight, labelHeight);
                        RectangleF productCodeBox = new RectangleF(paintArea.X + 59 * weight, paintArea.Y + paintArea.Height - labelHeight, 35 * weight, labelHeight);



                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Far;
                        g.DrawString(productType, displayFont, Brushes.Black, productTypeBox, sf);

                        g.FillRectangle(Brushes.White, manufacturerCodeBox);
                        g.DrawString(manufacturerCode, displayFont, Brushes.Black, manufacturerCodeBox, sf);

                        sf.Alignment = StringAlignment.Near;
                        g.FillRectangle(Brushes.White, productCodeBox);
                        g.DrawString(productCode, displayFont, Brushes.Black, productCodeBox, sf);

                        g.DrawString(checksum.ToString(), displayFont, Brushes.Black, checkSumBox, sf);
                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }

        private void PaintEAN13(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);

            try
            {
                // check if text content is valid for this type of barcode

                if (text.Length != 13 && text.Length != 12)
                    throw new Exception("Number of data elements must be 13.");

                for (int i = 0; i < text.Length; i++)
                {
                    if (!Char.IsDigit(text[i]))
                    {
                        throw new Exception("EAN-13 standard supports only digit data elements.");
                    }
                }

                string quietZone = "000000000";
                string lead = "101";
                string separator = "01010";

                string[] leftDigitsOdd = new string[] { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
                string[] leftDigitsEven = new string[] { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };


                string[] rightDigits = new string[] { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };

                string[] patterns = new string[] { "OOOOOO", "OOEOEE", "OOEEOE", "OOEEEO", "OEOOEE", "OEEOOE", "OEEEOO", "OEOEOE", "OEOEEO", "OEEOEO" };

                string encodedString = string.Empty;

                encodedString += quietZone;
                encodedString += lead;

                string firstCountryCode = text.Substring(0, 1);

                int checksum = 0;
                for (int i = text.Length; i >= 1; i--)
                {
                    int digit = Convert.ToInt32(text[i - 1].ToString());

                    if (i % 2 == 0)
                        checksum += digit * 3;
                    else
                        checksum += digit;
                }

                checksum = checksum % 10;
                checksum = (10 - checksum) % 10;


                string pattern = patterns[Convert.ToInt32(firstCountryCode)];

                for (int i = 1; i < 7; i++)
                {
                    if (pattern[i - 1] == 'E')
                        encodedString += leftDigitsEven[Convert.ToInt32(text[i].ToString())];
                    else
                        encodedString += leftDigitsOdd[Convert.ToInt32(text[i].ToString())];

                }

                encodedString += separator;

                for (int i = 7; i < text.Length; i++)
                {
                    encodedString += rightDigits[Convert.ToInt32(text[i].ToString())];
                }

                encodedString += rightDigits[checksum];

                encodedString += lead;
                encodedString += quietZone;


                int encodedStringLength = encodedString.Length;

                double numberOfLines = encodedStringLength;


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);

                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }

                float x = paintArea.X;
                for (int i = 0; i < encodedStringLength; i++)
                {
                    g.FillRectangle(encodedString[i] == '1' ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), x, paintArea.Y, weight, paintArea.Height - 1);
                    x += weight;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                if (showLabel)
                {

                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF countryCodeBox = new RectangleF(paintArea.X, paintArea.Y + paintArea.Height - labelHeight, 9 * weight, labelHeight);
                        RectangleF manufacturerCodeBox = new RectangleF(paintArea.X + 12 * weight, paintArea.Y + paintArea.Height - labelHeight, 42 * weight, labelHeight);
                        RectangleF productCodeBox = new RectangleF(paintArea.X + 59 * weight, paintArea.Y + paintArea.Height - labelHeight, 42 * weight, labelHeight);



                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Far;
                        g.DrawString(firstCountryCode, displayFont, Brushes.Black, countryCodeBox, sf);

                        sf.Alignment = StringAlignment.Center;
                        g.FillRectangle(Brushes.White, manufacturerCodeBox);
                        string manufacturerString = text.Substring(1, 6);
                        g.DrawString(manufacturerString, displayFont, Brushes.Black, manufacturerCodeBox, sf);


                        g.FillRectangle(Brushes.White, productCodeBox);
                        string productString = text.Substring(7) + checksum.ToString();
                        g.DrawString(productString, displayFont, Brushes.Black, productCodeBox, sf);
                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }

        private void PaintEAN8(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);

            try
            {
                // check if text content is valid for this type of barcode

                if (text.Length != 7)
                    throw new Exception("Number of data elements must be 7.");

                for (int i = 0; i < text.Length; i++)
                {
                    if (!Char.IsDigit(text[i]))
                    {
                        throw new Exception("EAN-8 standard supports only digit data elements.");
                    }
                }

                string quietZone = "000000000";
                string lead = "101";
                string separator = "01010";

                string[] leftDigitsOdd = new string[] { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
                string[] leftDigitsEven = new string[] { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };


                string[] rightDigits = new string[] { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };

                string[] patterns = new string[] { "OOOOOO", "OOEOEE", "OOEEOE", "OOEEEO", "OEOOEE", "OEEOOE", "OEEEOO", "OEOEOE", "OEOEEO", "OEEOEO" };

                string encodedString = string.Empty;

                encodedString += quietZone;
                encodedString += lead;

                string firstCountryCode = text.Substring(0, 1);

                int checksum = 0;
                for (int i = text.Length; i >= 1; i--)
                {
                    int digit = Convert.ToInt32(text[i - 1].ToString());

                    if (i % 2 == 0)
                        checksum += digit * 3;
                    else
                        checksum += digit;
                }

                checksum = checksum % 10;


                string pattern = patterns[Convert.ToInt32(firstCountryCode)];

                for (int i = 0; i < 4; i++)
                {
                    encodedString += leftDigitsOdd[Convert.ToInt32(text[i].ToString())];
                }

                encodedString += separator;

                for (int i = 4; i < text.Length; i++)
                {
                    encodedString += rightDigits[Convert.ToInt32(text[i].ToString())];
                }

                encodedString += rightDigits[checksum];

                encodedString += lead;
                encodedString += quietZone;


                int encodedStringLength = encodedString.Length;

                double numberOfLines = encodedStringLength;


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);

                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }

                float x = this.X;
                for (int i = 0; i < encodedStringLength; i++)
                {
                    g.FillRectangle(encodedString[i] == '1' ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), x, paintArea.Y, weight, paintArea.Height - 1);
                    x += weight;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                if (showLabel)
                {

                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF firstCodeBox = new RectangleF(paintArea.X + 12 * weight, paintArea.Y + paintArea.Height - labelHeight, 28 * weight, labelHeight);
                        RectangleF secondCodeBox = new RectangleF(paintArea.X + 45 * weight, paintArea.Y + paintArea.Height - labelHeight, 28 * weight, labelHeight);



                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;

                        g.FillRectangle(Brushes.White, firstCodeBox);
                        string firstString = text.Substring(0, 4);
                        g.DrawString(firstString, displayFont, Brushes.Black, firstCodeBox, sf);

                        g.FillRectangle(Brushes.White, secondCodeBox);
                        string secondString = text.Substring(4) + checksum.ToString();
                        g.DrawString(secondString, displayFont, Brushes.Black, secondCodeBox, sf);

                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }


        // Raffaele Russo - 28/04/2011 - Start - Metodo aggiunto per poter mandare in input una stringa all'oggetto barcode, proveniente dal pru
        /// <summary>
        /// Resolves parameter values during printing.
        /// </summary>
        private string ResolveParameterValues(string input)
        {
            string buffer = "";
            int pos = -1;
            int oldPos = 0;

            while ((pos = input.IndexOf("$P", oldPos)) != -1)
            {

                buffer += input.Substring(oldPos, pos - oldPos);
                if (input.Substring(pos + 2, 1).Equals("{") && input.IndexOf("}", pos + 2) != -1)
                {
                    string parameterName = input.Substring(pos + 3, input.IndexOf("}", pos + 2) - pos - 3).Trim();
                    int parPosition = buffer.Length;
                    buffer += this.section.GetParameterValue(parameterName);

                    oldPos = input.IndexOf("}", pos + 2) + 1;
                }
                else
                {
                    oldPos = pos + 2;
                }
            }

            buffer += input.Substring(oldPos);

            return buffer;
        }
        // Raffaele Russo - 28/04/2011 - End


        private void PaintEAN128(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            try
            {
                // Raffaele Russo - 28/04/2011 - Start
                string theText = section.Document.DesignMode ? text : this.ResolveParameterValues(text);
                string encText = EAN128.Check(theText);
                //string encText = EAN128.Check(text);
                // Raffaele Russo - 28/04/2011 - End

                //string bCode = EAN128.GetRawText128(text);				
                string bCode = EAN128.GetRawText128(encText);

                int len = bCode.Length;
                byte[] bars = EAN128.GetBars(bCode);


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);


                float barsWidth = 0;
                for (int i = 0; i < bars.Length; i++)
                {
                    barsWidth += bars[i];
                }

                float ratio = (float)(this.Width / barsWidth);

                if (ratio < 1)
                {
                    throw new Exception("Not enough space.");
                }

                float x = paintArea.X;
                for (int i = 0; i < bars.Length; i++)
                {
                    Brush brush = i % 2 == 0 ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White);
                    g.FillRectangle(brush, x, paintArea.Y, (int)(bars[i] * ratio), paintArea.Height - 1);
                    x += Convert.ToSingle(bars[i] * ratio);
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;


                if (showLabel)
                {
                    // Raffaele Russo - 28/04/2011 - Start
                    string label = EAN128.GetLabel(theText);
                    //string label = EAN128.GetLabel(text);
                    // Raffaele Russo - 28/04/2011 - End

                    float textWidth = g.MeasureString(label, displayFont).Width;
                    float labelStart = paintArea.X + (paintArea.Width - textWidth) / 2;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF labelBox = new RectangleF(labelStart, paintArea.Y + paintArea.Height - labelHeight, textWidth, labelHeight);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        g.FillRectangle(Brushes.White, labelBox);
                        g.DrawString(label, displayFont, Brushes.Black, labelBox, sf);
                    }

                }


            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }


        private void PaintUPCE(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            try
            {
                // check if text content is valid for this type of barcode
                if (text.Length != 12 && text.Length != 8)
                    throw new Exception("Number of data elements must be 12 or 8.");

                for (int i = 0; i < text.Length; i++)
                {
                    if (!Char.IsDigit(text[i]))
                    {
                        throw new Exception("UPC-E standard supports only digit data elements.");
                    }
                }

                string quietZone = "000000000";
                string lead = "101";
                string separator = "01010";

                string[] leftDigitsOdd = new string[] { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
                string[] leftDigitsEven = new string[] { "0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111" };
                string[] rightDigits = new string[] { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };

                string[] pattern0 = new string[] { "EEEOOO", "EEOEOO", "EEOOEO", "EEOOOE", "EOEEOO", "EOOEEO", "EOOOEE", "EOEOEO", "EOEOOE", "EOOEOE" };
                string[] pattern1 = new string[] { "OOOEEE", "OOEOEE", "OOEEOE", "OOEEEO", "OEOOEE", "OEEOOE", "OEEEOO", "OEOEOE", "OEOEEO", "OEEOEO" };


                string encodedString = string.Empty;

                encodedString += quietZone;
                encodedString += lead;

                string productType = text.Substring(0, 1);
                string theCode = text.Substring(1, 6);

                int checksum = 0;

                if (text.Length == 8)
                {
                    checksum = Convert.ToInt32(text[7].ToString());
                }
                else
                {
                    for (int i = 0; i < 11; i++)
                    {
                        int digit = Convert.ToInt32(text[i].ToString());

                        if (digit % 2 == 0)
                            checksum += digit * 3;
                        else
                            checksum += digit;
                    }

                    checksum = checksum % 10;
                    checksum = (10 - checksum) % 10;
                }


                string pattern = string.Empty;
                if (productType == "0")
                    pattern = pattern0[checksum];
                else
                    pattern = pattern1[checksum];


                for (int i = 1; i < 7; i++)
                {
                    if (pattern[i - 1] == 'E')
                        encodedString += leftDigitsEven[Convert.ToInt32(text[i].ToString())];
                    else
                        encodedString += leftDigitsOdd[Convert.ToInt32(text[i].ToString())];
                }

                encodedString += separator;
                encodedString += "1";
                encodedString += quietZone;


                int encodedStringLength = encodedString.Length;
                double numberOfLines = encodedStringLength;


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);

                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }

                float x = paintArea.X;
                for (int i = 0; i < encodedStringLength; i++)
                {
                    g.FillRectangle(encodedString[i] == '1' ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), new RectangleF(x, paintArea.Y, weight, paintArea.Height - 1));
                    x += weight;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                if (showLabel)
                {
                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF productTypeBox = new RectangleF(paintArea.X, paintArea.Y + paintArea.Height - labelHeight, 9 * weight, labelHeight);
                        RectangleF checkSumBox = new RectangleF(paintArea.X + paintArea.Width - 9 * weight, paintArea.Y + paintArea.Height - labelHeight, 9 * weight, labelHeight);
                        RectangleF theCodeBox = new RectangleF(paintArea.X + 12 * weight, paintArea.Y + paintArea.Height - labelHeight, 42 * weight, labelHeight);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Far;
                        g.DrawString(productType, displayFont, Brushes.Black, productTypeBox, sf);

                        sf.Alignment = StringAlignment.Center;
                        g.FillRectangle(Brushes.White, theCodeBox);
                        g.DrawString(theCode, displayFont, Brushes.Black, theCodeBox, sf);

                        sf.Alignment = StringAlignment.Near;
                        g.DrawString(checksum.ToString(), displayFont, Brushes.Black, checkSumBox, sf);
                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }

        private void PaintI2of5(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            string[] i2of5 = { "00110", "10001", "01001", "11000", "00101", "10100", "01100", "00011", "10010", "01010" };

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            try
            {

                // check if text content is valid for this type of barcode
                if (text.Length % 2 != 0)
                    throw new Exception("Odd number of data elements.");

                for (int i = 0; i < text.Length; i++)
                {
                    if (!Char.IsDigit(text[i]))
                    {
                        throw new Exception("I2of5 standard supports only digit data elements.");
                    }
                }


                string startMark = "0000";
                string endMark = "100";

                string encodedString = string.Empty;

                encodedString += startMark;

                for (int i = 0; i < text.Length; i += 2)
                {
                    string segment = text.Substring(i, 2);

                    string first = i2of5[Convert.ToInt32(segment[0].ToString())];
                    string second = i2of5[Convert.ToInt32(segment[1].ToString())];

                    for (int j = 0; j < 5; j++)
                    {
                        encodedString += first[j];
                        encodedString += second[j];
                    }
                }

                encodedString += endMark;

                int encodedStringLength = encodedString.Length;

                double wideToNarrowRatio = 3;
                double numberOfLines = 0;

                for (int i = 0; i < encodedStringLength; i++)
                {
                    if (encodedString[i] == '1')
                        numberOfLines += wideToNarrowRatio;
                    else
                        numberOfLines += 1;
                }

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);

                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }


                float lineWidth = 0;
                float x = paintArea.X;

                float labelStart = 0;
                float labelEnd = 0;

                for (int i = 0; i < encodedStringLength; i++)
                {
                    if (encodedString[i] == '1')
                        lineWidth = (float)(wideToNarrowRatio * weight);
                    else
                        lineWidth = weight;

                    g.FillRectangle(i % 2 == 0 ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), x, paintArea.Y, (int)lineWidth, paintArea.Height - 1);

                    if (i == 4)
                        labelStart = x;
                    else if (i == encodedString.Length - 4)
                        labelEnd = x;

                    x += lineWidth;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                if (showLabel)
                {
                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF labelBox = new RectangleF(labelStart, paintArea.Y + paintArea.Height - labelHeight, labelEnd - labelStart, labelHeight);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        g.FillRectangle(Brushes.White, labelBox);
                        g.DrawString(text, displayFont, Brushes.Black, labelBox, sf);
                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }

        private void PaintCode39(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            try
            {
                // Raffaele Russo - 26/07/2013 - Start
                text = section.Document.DesignMode ? text : this.ResolveParameterValues(text);
                // Raffaele Russo - 26/07/2013 - End

                // check if text content is valid for this type of barcode
                for (int i = 0; i < text.Length; i++)
                {
                    if (alphabet39.IndexOf(text[i]) == -1 || text[i] == '*')
                    {
                        throw new Exception("Unsupported data elements.");
                    }
                }

                String fullText = '*' + text.ToUpper() + '*';
                int strLength = fullText.Length;

                String intercharacterGap = "0";
                String encodedString = "";

                for (int i = 0; i < strLength; i++)
                {
                    if (i > 0)
                    {
                        encodedString += intercharacterGap;
                    }

                    encodedString += coded39Char[alphabet39.IndexOf(fullText[i])];
                }

                int encodedStringLength = encodedString.Length;

                double wideToNarrowRatio = 3;
                double numberOfLines = 0;

                for (int i = 0; i < encodedStringLength; i++)
                {
                    if (encodedString[i] == '1')
                        numberOfLines += wideToNarrowRatio;
                    else
                        numberOfLines += 1;
                }

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);
                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }


                float lineWidth = 0;
                float x = paintArea.X;

                float labelStart = 0;
                float labelEnd = 0;

                for (int i = 0; i < encodedStringLength; i++)
                {
                    if (encodedString[i] == '1')
                        lineWidth = (float)(wideToNarrowRatio * weight);
                    else
                        lineWidth = weight;

                    g.FillRectangle(i % 2 == 0 ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), x, paintArea.Y, (int)lineWidth, paintArea.Height - 1);

                    if (i == 9)
                        labelStart = x;
                    else if (i == encodedString.Length - 9)
                        labelEnd = x;

                    x += lineWidth;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                if (showLabel)
                {
                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < this.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF labelBox = new RectangleF(labelStart, paintArea.Y + paintArea.Height - labelHeight, labelEnd - labelStart, labelHeight);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;

                        g.FillRectangle(Brushes.White, labelBox);
                        g.DrawString(text, displayFont, Brushes.Black, labelBox, sf);
                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }
        }

        string alphabet39 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

        string[] coded39Char = 
		{
			/* 0 */ "000110100", 
			/* 1 */ "100100001", 
			/* 2 */ "001100001", 
			/* 3 */ "101100000",
			/* 4 */ "000110001", 
			/* 5 */ "100110000", 
			/* 6 */ "001110000", 
			/* 7 */ "000100101",
			/* 8 */ "100100100", 
			/* 9 */ "001100100", 
			/* A */ "100001001", 
			/* B */ "001001001",
			/* C */ "101001000", 
			/* D */ "000011001", 
			/* E */ "100011000", 
			/* F */ "001011000",
			/* G */ "000001101", 
			/* H */ "100001100", 
			/* I */ "001001100", 
			/* J */ "000011100",
			/* K */ "100000011", 
			/* L */ "001000011", 
			/* M */ "101000010", 
			/* N */ "000010011",
			/* O */ "100010010", 
			/* P */ "001010010", 
			/* Q */ "000000111", 
			/* R */ "100000110",
			/* S */ "001000110", 
			/* T */ "000010110", 
			/* U */ "110000001", 
			/* V */ "011000001",
			/* W */ "111000000", 
			/* X */ "010010001", 
			/* Y */ "110010000", 
			/* Z */ "011010000",
			/* - */ "010000101", 
			/* . */ "110000100", 
			/*' '*/ "011000100",
			/* $ */ "010101000",
			/* / */ "010100010", 
			/* + */ "010001010", 
			/* % */ "000101010", 
			/* * */ "010010100" 
		};

        private void PaintCodabar(Graphics g)
        {
            float sizeInPixels = this.PointsToPixels(labelFont.SizeInPoints);
            Font displayFont = new Font(labelFont.FontFamily, sizeInPixels * g.PageScale, labelFont.Style, GraphicsUnit.Pixel);

            string alphabet = "0123456789-$:/.+ABCD";
            string[] codes = new string[] { "0000011", "0000110", "0001001", "1100000", "0010010", "1000010", "0100001", "0100100", "0110000", "1001000", "0001100", "0011000", "1000101", "1010001", "1010100", "0010101", "0011010", "0101001", "0001011", "0001110" };

            // Raffaele Russo - 12/12/2011 - Start
            Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : new Rectangle(Bounds.X - rBounds.X, Bounds.Y - rBounds.Y, Bounds.Width, Bounds.Height);
            // Rectangle paintArea = new Rectangle(theRegion.X - rBounds.X, theRegion.Y - rBounds.Y, theRegion.Width, theRegion.Height);
            // Rectangle paintArea = this.Section.Document.DesignMode ? theRegion : Bounds;
            // Raffaele Russo - 12/12/2011 - End

            try
            {

                // check if text content is valid for this type of barcode
                for (int i = 0; i < text.Length; i++)
                {
                    if (alphabet.IndexOf(text[i]) == -1)
                    {
                        throw new Exception("Unsupported data elements.");
                    }
                }

                string intercharacterGap = "0";
                string encodedString = string.Empty;

                encodedString += codes[alphabet.IndexOf('A')];

                for (int i = 0; i < text.Length; i++)
                {
                    if (i > 0)
                        encodedString += intercharacterGap;

                    encodedString += codes[alphabet.IndexOf(text[i])];
                }

                encodedString += codes[alphabet.IndexOf('B')];

                int encodedStringLength = encodedString.Length;

                double wideToNarrowRatio = 3;
                double numberOfLines = 0;

                for (int i = 0; i < encodedStringLength; i++)
                {
                    if (encodedString[i] == '1')
                        numberOfLines += wideToNarrowRatio;
                    else
                        numberOfLines += 1;
                }

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle((paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), paintArea);

                float weight = (float)(this.Width / numberOfLines);
                if (weight < 1)
                {
                    throw new Exception("Not enough space.");
                }


                float lineWidth = 0;
                float x = paintArea.X;

                float labelStart = 0;
                float labelEnd = 0;

                for (int i = 0; i < encodedStringLength; i++)
                {
                    if (encodedString[i] == '1')
                        lineWidth = (float)(wideToNarrowRatio * weight);
                    else
                        lineWidth = weight;

                    g.FillRectangle(i % 2 == 0 ? Brushes.Black : (paintTransparent ? new SolidBrush(ShiftColor(Color.White, 128)) : Brushes.White), x, paintArea.Y, lineWidth, paintArea.Height - 1);

                    if (i == 7)
                        labelStart = x;
                    else if (i == encodedString.Length - 7)
                        labelEnd = x;

                    x += lineWidth;
                }


                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;


                if (showLabel)
                {
                    float textWidth = g.MeasureString(text, displayFont).Width;

                    if (textWidth < paintArea.Width)
                    {
                        int labelHeight = displayFont.Height;

                        RectangleF labelBox = new RectangleF(labelStart, paintArea.Y + paintArea.Height - labelHeight, labelEnd - labelStart, labelHeight);

                        StringFormat sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;

                        g.FillRectangle(Brushes.White, labelBox);
                        g.DrawString(text, displayFont, Brushes.Black, labelBox, sf);
                    }

                }
            }
            catch (Exception ex)
            {
                PaintException(g, ex);
            }

        }






        internal class BarcodeData
        {
            internal string BarcodeText = "";
            internal Barcode.BarcodeTypes BarcodeType = Barcode.BarcodeTypes.Codabar;

            internal BarcodeData(Barcode.BarcodeTypes type, string barcodeData)
            {
                this.BarcodeType = type;
                this.BarcodeText = barcodeData;
            }
        }

        internal class EAN128
        {

            private static Hashtable ais = new Hashtable();


            /** The stop bars.
        */
            private static readonly byte[] BARS_STOP = { 2, 3, 3, 1, 1, 1, 2 };
            /** The charset code change.
            */
            public const char CODE_AB_TO_C = (char)99;
            /** The charset code change.
            */
            public const char CODE_AC_TO_B = (char)100;
            /** The charset code change.
            */
            public const char CODE_BC_TO_A = (char)101;

            /** The code for UCC/EAN-128.
            */
            public const char FNC1_INDEX = (char)102;
            /** The start code.
            */
            public const char START_A = (char)103;
            /** The start code.
            */
            public const char START_B = (char)104;
            /** The start code.
            */
            public const char START_C = (char)105;

            public const char FNC1 = '\u00ca';
            public const char DEL = '\u00c3';
            public const char FNC3 = '\u00c4';
            public const char FNC2 = '\u00c5';
            public const char SHIFT = '\u00c6';
            public const char CODE_C = '\u00c7';
            public const char CODE_A = '\u00c8';
            public const char FNC4 = '\u00c8';
            public const char STARTA = '\u00cb';
            public const char STARTB = '\u00cc';
            public const char STARTC = '\u00cd';


            private static readonly byte[][] BARS = 
		{
			new byte[] {2, 1, 2, 2, 2, 2},
			new byte[] {2, 2, 2, 1, 2, 2},
			new byte[] {2, 2, 2, 2, 2, 1},
			new byte[] {1, 2, 1, 2, 2, 3},
			new byte[] {1, 2, 1, 3, 2, 2},
			new byte[] {1, 3, 1, 2, 2, 2},
			new byte[] {1, 2, 2, 2, 1, 3},
			new byte[] {1, 2, 2, 3, 1, 2},
			new byte[] {1, 3, 2, 2, 1, 2},
			new byte[] {2, 2, 1, 2, 1, 3},
			new byte[] {2, 2, 1, 3, 1, 2},
			new byte[] {2, 3, 1, 2, 1, 2},
			new byte[] {1, 1, 2, 2, 3, 2},
			new byte[] {1, 2, 2, 1, 3, 2},
			new byte[] {1, 2, 2, 2, 3, 1},
			new byte[] {1, 1, 3, 2, 2, 2},
			new byte[] {1, 2, 3, 1, 2, 2},
			new byte[] {1, 2, 3, 2, 2, 1},
			new byte[] {2, 2, 3, 2, 1, 1},
			new byte[] {2, 2, 1, 1, 3, 2},
			new byte[] {2, 2, 1, 2, 3, 1},
			new byte[] {2, 1, 3, 2, 1, 2},
			new byte[] {2, 2, 3, 1, 1, 2},
			new byte[] {3, 1, 2, 1, 3, 1},
			new byte[] {3, 1, 1, 2, 2, 2},
			new byte[] {3, 2, 1, 1, 2, 2},
			new byte[] {3, 2, 1, 2, 2, 1},
			new byte[] {3, 1, 2, 2, 1, 2},
			new byte[] {3, 2, 2, 1, 1, 2},
			new byte[] {3, 2, 2, 2, 1, 1},
			new byte[] {2, 1, 2, 1, 2, 3},
			new byte[] {2, 1, 2, 3, 2, 1},
			new byte[] {2, 3, 2, 1, 2, 1},
			new byte[] {1, 1, 1, 3, 2, 3},
			new byte[] {1, 3, 1, 1, 2, 3},
			new byte[] {1, 3, 1, 3, 2, 1},
			new byte[] {1, 1, 2, 3, 1, 3},
			new byte[] {1, 3, 2, 1, 1, 3},
			new byte[] {1, 3, 2, 3, 1, 1},
			new byte[] {2, 1, 1, 3, 1, 3},
			new byte[] {2, 3, 1, 1, 1, 3},
			new byte[] {2, 3, 1, 3, 1, 1},
			new byte[] {1, 1, 2, 1, 3, 3},
			new byte[] {1, 1, 2, 3, 3, 1},
			new byte[] {1, 3, 2, 1, 3, 1},
			new byte[] {1, 1, 3, 1, 2, 3},
			new byte[] {1, 1, 3, 3, 2, 1},
			new byte[] {1, 3, 3, 1, 2, 1},
			new byte[] {3, 1, 3, 1, 2, 1},
			new byte[] {2, 1, 1, 3, 3, 1},
			new byte[] {2, 3, 1, 1, 3, 1},
			new byte[] {2, 1, 3, 1, 1, 3},
			new byte[] {2, 1, 3, 3, 1, 1},
			new byte[] {2, 1, 3, 1, 3, 1},
			new byte[] {3, 1, 1, 1, 2, 3},
			new byte[] {3, 1, 1, 3, 2, 1},
			new byte[] {3, 3, 1, 1, 2, 1},
			new byte[] {3, 1, 2, 1, 1, 3},
			new byte[] {3, 1, 2, 3, 1, 1},
			new byte[] {3, 3, 2, 1, 1, 1},
			new byte[] {3, 1, 4, 1, 1, 1},
			new byte[] {2, 2, 1, 4, 1, 1},
			new byte[] {4, 3, 1, 1, 1, 1},
			new byte[] {1, 1, 1, 2, 2, 4},
			new byte[] {1, 1, 1, 4, 2, 2},
			new byte[] {1, 2, 1, 1, 2, 4},
			new byte[] {1, 2, 1, 4, 2, 1},
			new byte[] {1, 4, 1, 1, 2, 2},
			new byte[] {1, 4, 1, 2, 2, 1},
			new byte[] {1, 1, 2, 2, 1, 4},
			new byte[] {1, 1, 2, 4, 1, 2},
			new byte[] {1, 2, 2, 1, 1, 4},
			new byte[] {1, 2, 2, 4, 1, 1},
			new byte[] {1, 4, 2, 1, 1, 2},
			new byte[] {1, 4, 2, 2, 1, 1},
			new byte[] {2, 4, 1, 2, 1, 1},
			new byte[] {2, 2, 1, 1, 1, 4},
			new byte[] {4, 1, 3, 1, 1, 1},
			new byte[] {2, 4, 1, 1, 1, 2},
			new byte[] {1, 3, 4, 1, 1, 1},
			new byte[] {1, 1, 1, 2, 4, 2},
			new byte[] {1, 2, 1, 1, 4, 2},
			new byte[] {1, 2, 1, 2, 4, 1},
			new byte[] {1, 1, 4, 2, 1, 2},
			new byte[] {1, 2, 4, 1, 1, 2},
			new byte[] {1, 2, 4, 2, 1, 1},
			new byte[] {4, 1, 1, 2, 1, 2},
			new byte[] {4, 2, 1, 1, 1, 2},
			new byte[] {4, 2, 1, 2, 1, 1},
			new byte[] {2, 1, 2, 1, 4, 1},
			new byte[] {2, 1, 4, 1, 2, 1},
			new byte[] {4, 1, 2, 1, 2, 1},
			new byte[] {1, 1, 1, 1, 4, 3},
			new byte[] {1, 1, 1, 3, 4, 1},
			new byte[] {1, 3, 1, 1, 4, 1},
			new byte[] {1, 1, 4, 1, 1, 3},
			new byte[] {1, 1, 4, 3, 1, 1},
			new byte[] {4, 1, 1, 1, 1, 3},
			new byte[] {4, 1, 1, 3, 1, 1},
			new byte[] {1, 1, 3, 1, 4, 1},
			new byte[] {1, 1, 4, 1, 3, 1},
			new byte[] {3, 1, 1, 1, 4, 1},
			new byte[] {4, 1, 1, 1, 3, 1},
			new byte[] {2, 1, 1, 4, 1, 2},
			new byte[] {2, 1, 1, 2, 1, 4},
			new byte[] {2, 1, 1, 2, 3, 2}
		};

            static EAN128()
            {
                ais[0] = 20;
                ais[1] = 16;
                ais[2] = 16;
                ais[10] = -1;
                ais[11] = 9;
                ais[12] = 8;
                ais[13] = 8;
                ais[15] = 8;
                ais[17] = 8;
                ais[20] = 4;
                ais[21] = -1;
                ais[22] = -1;
                ais[23] = -1;
                ais[240] = -1;
                ais[241] = -1;
                ais[250] = -1;
                ais[251] = -1;
                ais[252] = -1;
                ais[30] = -1;
                for (int k = 3100; k < 3700; ++k)
                    ais[k] = 10;
                ais[37] = -1;
                for (int k = 3900; k < 3940; ++k)
                    ais[k] = -1;
                ais[400] = -1;
                ais[401] = -1;
                ais[402] = 20;
                ais[403] = -1;
                for (int k = 410; k < 416; ++k)
                    ais[k] = 16;
                ais[420] = -1;
                ais[421] = -1;
                ais[422] = 6;
                ais[423] = -1;
                ais[424] = 6;
                ais[425] = 6;
                ais[426] = 6;
                ais[7001] = 17;
                ais[7002] = -1;
                for (int k = 7030; k < 7040; ++k)
                    ais[k] = -1;
                ais[8001] = 18;
                ais[8002] = -1;
                ais[8003] = -1;
                ais[8004] = -1;
                ais[8005] = 10;
                ais[8006] = 22;
                ais[8007] = -1;
                ais[8008] = -1;
                ais[8018] = 22;
                ais[8020] = -1;
                ais[8100] = 10;
                ais[8101] = 14;
                ais[8102] = 6;
                for (int k = 90; k < 100; ++k)
                    ais[k] = -1;
            }


            /** Converts the human readable text to the characters needed to
        * create a barcode. Some optimization is done to get the shortest code.
        * @param text the text to convert
        * @param ucc <CODE>true</CODE> if it is an UCC/EAN-128. In this case
        * the character FNC1 is added
        * @return the code ready to be fed to GetBarsCode128Raw()
        */
            public static string GetRawText128(string text)
            {
                String outs = "";
                int tLen = text.Length;
                if (tLen == 0)
                {
                    outs += START_B;
                    outs += FNC1_INDEX;
                    return outs;
                }

                int c = 0;
                for (int k = 0; k < tLen; ++k)
                {
                    c = text[k];
                    if (c > 127 && c != FNC1)
                        throw new ArgumentException("There are illegal characters for barcode 128 in '" + text + "'.");
                }

                c = text[0];
                char currentCode = START_B;
                int index = 0;
                if (IsNextDigits(text, index, 2))
                {
                    currentCode = START_C;
                    outs += currentCode;
                    outs += FNC1_INDEX;

                    String out2 = GetPackedRawDigits(text, index, 2);
                    index += (int)out2[0];
                    outs += out2.Substring(1);
                }
                else if (c < ' ')
                {
                    currentCode = START_A;
                    outs += currentCode;
                    outs += FNC1_INDEX;
                    outs += (char)(c + 64);
                    ++index;
                }
                else
                {
                    outs += currentCode;
                    outs += FNC1_INDEX;

                    if (c == FNC1)
                        outs += FNC1_INDEX;
                    else
                        outs += (char)(c - ' ');
                    ++index;
                }

                while (index < tLen)
                {
                    switch (currentCode)
                    {
                        case START_A:
                            {
                                if (IsNextDigits(text, index, 4))
                                {
                                    currentCode = START_C;
                                    outs += CODE_AB_TO_C;
                                    String out2 = GetPackedRawDigits(text, index, 4);
                                    index += (int)out2[0];
                                    outs += out2.Substring(1);
                                }
                                else
                                {
                                    c = text[index++];
                                    if (c == FNC1)
                                        outs += FNC1_INDEX;
                                    else if (c > '_')
                                    {
                                        currentCode = START_B;
                                        outs += CODE_AC_TO_B;
                                        outs += (char)(c - ' ');
                                    }
                                    else if (c < ' ')
                                        outs += (char)(c + 64);
                                    else
                                        outs += (char)(c - ' ');
                                }
                            }
                            break;
                        case START_B:
                            {
                                if (IsNextDigits(text, index, 4))
                                {
                                    currentCode = START_C;
                                    outs += CODE_AB_TO_C;
                                    String out2 = GetPackedRawDigits(text, index, 4);
                                    index += (int)out2[0];
                                    outs += out2.Substring(1);
                                }
                                else
                                {
                                    c = text[index++];
                                    if (c == FNC1)
                                        outs += FNC1_INDEX;
                                    else if (c < ' ')
                                    {
                                        currentCode = START_A;
                                        outs += CODE_BC_TO_A;
                                        outs += (char)(c + 64);
                                    }
                                    else
                                    {
                                        outs += (char)(c - ' ');
                                    }
                                }
                            }
                            break;
                        case START_C:
                            {
                                if (IsNextDigits(text, index, 2))
                                {
                                    String out2 = GetPackedRawDigits(text, index, 2);
                                    index += (int)out2[0];
                                    outs += out2.Substring(1);
                                }
                                else
                                {
                                    c = text[index++];
                                    if (c == FNC1)
                                        outs += FNC1_INDEX;
                                    else if (c < ' ')
                                    {
                                        currentCode = START_A;
                                        outs += CODE_BC_TO_A;
                                        outs += (char)(c + 64);
                                    }
                                    else
                                    {
                                        currentCode = START_B;
                                        outs += CODE_AC_TO_B;
                                        outs += (char)(c - ' ');
                                    }
                                }
                            }
                            break;
                    }
                }
                return outs;
            }



            private static bool IsNextDigits(string text, int textIndex, int numDigits)
            {
                int len = text.Length;
                while (textIndex < len && numDigits > 0)
                {
                    if (text[textIndex] == FNC1)
                    {
                        ++textIndex;
                        continue;
                    }
                    int n = Math.Min(2, numDigits);
                    if (textIndex + n > len)
                        return false;
                    while (n-- > 0)
                    {
                        char c = text[textIndex++];
                        if (c < '0' || c > '9')
                            return false;
                        --numDigits;
                    }
                }
                return numDigits == 0;
            }



            private static String GetPackedRawDigits(String text, int textIndex, int numDigits)
            {
                String outs = "";
                int start = textIndex;
                while (numDigits > 0)
                {
                    if (text[textIndex] == FNC1)
                    {
                        outs += FNC1_INDEX;
                        ++textIndex;
                        continue;
                    }
                    numDigits -= 2;
                    int c1 = text[textIndex++] - '0';
                    int c2 = text[textIndex++] - '0';
                    outs += (char)(c1 * 10 + c2);
                }
                string packed = (char)(textIndex - start) + outs;

                return packed;
            }


            /** Generates the bars. The input has the actual barcodes, not
            * the human readable text.
            * @param text the barcode
            * @return the bars
            */
            public static byte[] GetBars(string text)
            {
                int k;
                int idx = text.IndexOf('\uffff');
                if (idx >= 0)
                    text = text.Substring(0, idx);

                int chk = text[0];
                for (k = 1; k < text.Length; ++k)
                    chk += k * text[k];
                chk = chk % 103;

                text += (char)chk;


                byte[] bars = new byte[(text.Length + 1) * 6 + 7];
                for (k = 0; k < text.Length; ++k)
                    Array.Copy(BARS[text[k]], 0, bars, k * 6, 6);
                Array.Copy(BARS_STOP, 0, bars, k * 6, 7);

                return bars;
            }

            public static String GetLabel(String code)
            {
                StringBuilder buf = new StringBuilder();
                String fnc1 = FNC1.ToString();
                try
                {
                    while (true)
                    {
                        if (code.StartsWith(fnc1))
                        {
                            code = code.Substring(1);
                            continue;
                        }

                        int n = 0;
                        int idlen = 0;
                        for (int k = 2; k < 5; ++k)
                        {
                            if (code.Length < k)
                                break;

                            string appIdText = code.Substring(0, k);
                            int appId = Int32.Parse(appIdText);

                            if (ais.ContainsKey(appId))
                            {
                                idlen = k;
                                n = (int)ais[appId];
                                break;
                            }

                        }

                        if (idlen == 0)
                            break;

                        buf.Append('(').Append(code.Substring(0, idlen)).Append(')');
                        code = code.Substring(idlen);
                        if (n > 0)
                        {
                            n -= idlen;
                            if (code.Length <= n)
                                break;
                            buf.Append(RemoveFNC1(code.Substring(0, n)));
                            code = code.Substring(n);
                        }
                        else
                        {
                            int idx = code.IndexOf(FNC1);
                            if (idx < 0)
                                break;
                            buf.Append(code.Substring(0, idx));
                            code = code.Substring(idx + 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //empty
                }
                buf.Append(RemoveFNC1(code));
                return buf.ToString();
            }


            private static String RemoveFNC1(String code)
            {
                int len = code.Length;
                StringBuilder buf = new StringBuilder(len);
                for (int k = 0; k < len; ++k)
                {
                    char c = code[k];
                    if (c >= 32 && c <= 126)
                        buf.Append(c);
                }
                return buf.ToString();
            }

            public static string Check(string code)
            {
                if (code.StartsWith("("))
                {
                    int idx = 0;
                    String ret = "";
                    while (idx >= 0)
                    {
                        int end = code.IndexOf(')', idx);
                        if (end < 0)
                            throw new ArgumentException("Badly formed UCC string: " + code);

                        String sai = code.Substring(idx + 1, end - (idx + 1));
                        if (sai.Length < 2)
                            throw new ArgumentException("AI too short: (" + sai + ")");
                        int ai = int.Parse(sai);
                        int len = (int)ais[ai];
                        if (len == 0)
                            throw new ArgumentException("AI not found: (" + sai + ")");

                        sai = ai.ToString();
                        if (sai.Length == 1)
                            sai = "0" + sai;
                        idx = code.IndexOf('(', end);
                        int next = (idx < 0 ? code.Length : idx);
                        ret += sai + code.Substring(end + 1, next - (end + 1));
                        if (len < 0)
                        {
                            if (idx >= 0)
                                ret += FNC1;
                        }
                        else if (next - end - 1 + sai.Length != len)
                            throw new ArgumentException("Invalid AI length: (" + sai + ")");
                    }
                    return ret;
                }
                else
                {
                    return code;
                }

            }
        }
    }
}
