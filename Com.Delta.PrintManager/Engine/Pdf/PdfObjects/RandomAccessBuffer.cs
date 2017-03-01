using System;
using System.IO;
using System.Text;
using System.Net;


namespace Com.Delta.PrintManager.Engine.Pdf
{

    internal class RandomAccessBuffer 
	{
        
        internal FileStream rf;
        internal String filename;
        internal byte[] arrayIn;
        internal int arrayInPtr;
        internal byte back;
        internal bool isBack = false;
        
        /** Holds value of property startOffset. */
        private int startOffset = 0;
        
        public RandomAccessBuffer(String filename) : this(filename, false) {
        }
        
        public RandomAccessBuffer(String filename, bool forceRead) {
            if (!File.Exists(filename)) {
                if (filename.StartsWith("file:/") || filename.StartsWith("http://") || filename.StartsWith("https://")) {
                    Stream isp = WebRequest.Create(new Uri(filename)).GetResponse().GetResponseStream();
                    try {
                        this.arrayIn = InputStreamToArray(isp);
                        return;
                    }
                    finally {
                        try {isp.Close();}catch{}
                    }
                }
                else {
					/*
                    Stream isp = BaseFont.GetResourceStream(filename);
                    if (isp == null)
                        throw new IOException(filename + " not found as file or resource.");
                    try {
                        this.arrayIn = InputStreamToArray(isp);
                        return;
                    }
                    finally {
                        try {isp.Close();}catch{}
                    }
					*/
                }
            }
            else if (forceRead) {
                Stream s = null;
                try {
                    s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    this.arrayIn = InputStreamToArray(s);
                }
                finally {
                    try{s.Close();}catch{}
                }
                return;
            }
            this.filename = filename;
            rf = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        }



        public RandomAccessBuffer(Stream isp) 
		{
            this.arrayIn = InputStreamToArray(isp);
        }
        
        public static byte[] InputStreamToArray(Stream isp) {
            byte[] b = new byte[8192];
            MemoryStream outp = new MemoryStream();
            while (true) {
                int read = isp.Read(b, 0, b.Length);
                if (read < 1)
                    break;
                outp.Write(b, 0, read);
            }
            return outp.ToArray();
        }

        public RandomAccessBuffer(byte[] arrayIn) {
            this.arrayIn = arrayIn;
        }
        
        public RandomAccessBuffer(RandomAccessBuffer file) 
		{
            filename = file.filename;
            arrayIn = file.arrayIn;
            startOffset = file.startOffset;
        }
        
        public void PushBack(byte b) {
            back = b;
            isBack = true;
        }
        
        public int Read() {
            if (isBack) {
                isBack = false;
                return back & 0xff;
            }
            if (arrayIn == null)
                return rf.ReadByte();
            else {
                if (arrayInPtr >= arrayIn.Length)
                    return -1;
                return arrayIn[arrayInPtr++] & 0xff;
            }
        }
        
        public int Read(byte[] b, int off, int len) {
            if (len == 0)
                return 0;
            int n = 0;
            if (isBack) {
                isBack = false;
                if (len == 1) {
                    b[off] = back;
                    return 1;
                }
                else {
                    n = 1;
                    b[off++] = back;
                    --len;
                }
            }
            if (arrayIn == null) {
                return rf.Read(b, off, len) + n;
            }
            else {
                if (arrayInPtr >= arrayIn.Length)
                    return -1;
                if (arrayInPtr + len > arrayIn.Length)
                    len = arrayIn.Length - arrayInPtr;
                Array.Copy(arrayIn, arrayInPtr, b, off, len);
                arrayInPtr += len;
                return len + n;
            }
        }
        
        public int Read(byte[] b) {
            return Read(b, 0, b.Length);
        }
        
        public void ReadFully(byte[] b) {
            ReadFully(b, 0, b.Length);
        }
        
        public void ReadFully(byte[] b, int off, int len) {
            int n = 0;
            do {
                int count = Read(b, off + n, len - n);
                if (count <= 0)
                    throw new EndOfStreamException();
                n += count;
            } while (n < len);
        }
        
        public long Skip(long n) {
            return SkipBytes((int)n);
        }
        
        public int SkipBytes(int n) {
            if (n <= 0) {
                return 0;
            }
            int adj = 0;
            if (isBack) {
                isBack = false;
                if (n == 1) {
                    return 1;
                }
                else {
                    --n;
                    adj = 1;
                }
            }
            int pos;
            int len;
            int newpos;
            
            pos = FilePointer;
            len = Length;
            newpos = pos + n;
            if (newpos > len) {
                newpos = len;
            }
            Seek(newpos);
            
            /* return the actual number of bytes skipped */
            return newpos - pos + adj;
        }
        
        public void ReOpen() {
            if (filename != null && rf == null)
                rf = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            Seek(0);
        }
        
        protected void InsureOpen() {
            if (filename != null && rf == null) {
                ReOpen();
            }
        }
        
        public bool IsOpen() {
            return (filename == null || rf != null);
        }
        
        public void Close() {
            isBack = false;
            if (rf != null) {
                rf.Close();
                rf = null;
            }
        }
        
        public int Length {
            get {
                if (arrayIn == null) {
                    InsureOpen();
                    return (int)rf.Length - startOffset;
                }
                else
                    return arrayIn.Length - startOffset;
            }
        }
        
        public void Seek(int pos) {
            pos += startOffset;
            isBack = false;
            if (arrayIn == null) {
                InsureOpen();
                rf.Position = pos;
            }
            else
                arrayInPtr = pos;
        }
        
        public void Seek(long pos) {
            Seek((int)pos);
        }
        
        public int FilePointer {
            get {
                InsureOpen();
                int n = isBack ? 1 : 0;
                if (arrayIn == null) {
                    return (int)rf.Position - n - startOffset;
                }
                else
                    return arrayInPtr - n - startOffset;
            }
        }
        
        public bool ReadBoolean() {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return (ch != 0);
        }
        
        public byte ReadByte() {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return (byte)(ch);
        }
        
        public int ReadUnsignedByte() {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return ch;
        }
        
        public short ReadShort() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (short)((ch1 << 8) + ch2);
        }
        
        /**
        * Reads a signed 16-bit number from this stream in little-endian order.
        * The method reads two
        * bytes from this stream, starting at the current stream pointer.
        * If the two bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where each of the two values is
        * between <code>0</code> and <code>255</code>, inclusive, then the
        * result is equal to:
        * <blockquote><pre>
        *     (short)((b2 &lt;&lt; 8) | b1)
        * </pre></blockquote>
        * 
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream, interpreted as a signed
        *             16-bit number.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */
        public short ReadShortLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (short)((ch2 << 8) + (ch1 << 0));
        }
        
        public int ReadUnsignedShort() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (ch1 << 8) + ch2;
        }
        
        
        public int ReadUnsignedShortLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (ch2 << 8) + (ch1 << 0);
        }
        
        public char ReadChar() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (char)((ch1 << 8) + ch2);
        }
        
        
        public char ReadCharLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (char)((ch2 << 8) + (ch1 << 0));
        }
        
        public int ReadInt() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
        }
        
        
        public int ReadIntLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
        }
        
        
        public long ReadUnsignedInt() {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
        }
        
        public long ReadUnsignedIntLE() {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
        }
        
        public long ReadLong() {
            return ((long)(ReadInt()) << 32) + (ReadInt() & 0xFFFFFFFFL);
        }
        
        public long ReadLongLE() {
            int i1 = ReadIntLE();
            int i2 = ReadIntLE();
            return ((long)i2 << 32) + (i1 & 0xFFFFFFFFL);
        }
        
        public float ReadFloat() {
            int[] a = {ReadInt()};
            float[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 4);
            return b[0];
        }
        
        public float ReadFloatLE() {
            int[] a = {ReadIntLE()};
            float[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 4);
            return b[0];
        }
        
        public double ReadDouble() {
            long[] a = {ReadLong()};
            double[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 8);
            return b[0];
        }
        
        public double ReadDoubleLE() {
            long[] a = {ReadLongLE()};
            double[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 8);
            return b[0];
        }

    public String ReadLine() {
            StringBuilder input = new StringBuilder();
            int c = -1;
            bool eol = false;
            
            while (!eol) {
                switch (c = Read()) {
                    case -1:
                    case '\n':
                        eol = true;
                        break;
                    case '\r':
                        eol = true;
                        int cur = FilePointer;
                        if ((Read()) != '\n') {
                            Seek(cur);
                        }
                        break;
                    default:
                        input.Append((char)c);
                        break;
                }
            }
            
            if ((c == -1) && (input.Length == 0)) {
                return null;
            }
            return input.ToString();
        }
        
        public int StartOffset {
            get {
                return startOffset;
            }
            set {
                startOffset = value;
            }
        }
    }
}
