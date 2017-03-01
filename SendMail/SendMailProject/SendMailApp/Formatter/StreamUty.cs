using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMailApp.Formatter
{
    public class StreamUty
    {
        public static void sbyte2File(sbyte[] input, string path)
        {
            int intLength = input.Length;
            System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            System.IO.BinaryWriter binWriter = new System.IO.BinaryWriter(fs);
            for (int i = 0; i < intLength; i++)
            {
                binWriter.Write(input[i]);
            }
            fs.Close();
            binWriter.Close();
        }

        public static string mStream2String(System.IO.MemoryStream input)
        {
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter wr = new System.IO.StringWriter(sb);
            byte[] x = input.ToArray();
            System.Text.UTF8Encoding enc = new UTF8Encoding();
            return enc.GetString(x);
        }

        public static System.IO.MemoryStream sbyteArray2MemoryStream(sbyte[] input)
        {

            int intLength = input.Length;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.BinaryWriter binWriter = new System.IO.BinaryWriter(ms);
            for (int i = 0; i < intLength; i++)
            {
                binWriter.Write(input[i]);
            }
            return ms;
        }

        public static System.IO.MemoryStream byteArray2MemoryStream(byte[] input)
        {

            int intLength = input.Length;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.BinaryWriter binWriter = new System.IO.BinaryWriter(ms);
            for (int i = 0; i < intLength; i++)
            {
                binWriter.Write(input[i]);
            }
            return ms;
        }

        private byte[] sbyteToByte(sbyte[] input)
        {
            byte[] output = new byte[input.Length];
            System.Buffer.BlockCopy(input, 0, output, 0, input.Length);
            return output;
        }
    }
}
