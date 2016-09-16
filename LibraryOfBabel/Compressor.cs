using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryOfBabel
{
    public class Compressor
    {
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        public static void performanceCheck()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string compress = "StringStringStringStringStringStringStringStringStringStringStringStringStringString";
            Console.WriteLine("Before compression: " + compress.Length);
            Console.WriteLine(compress);
            string after = ByteArrayToString(Zip(compress));
            Console.WriteLine("After compression: " + after.Length);
            Console.WriteLine(after);
            double percent = (100 - ((double)after.Length / (double)compress.Length * 100));
            Console.WriteLine("Compression effectiveness: " + percent + "%");
            string unzip = Unzip(StringToByteArray(after));
            Console.WriteLine(unzip == compress ? "Unzip success!" : "Unzip mismatch!");
            sw.Stop();
            Console.WriteLine("Time elapsed: " + sw.Elapsed);
        }
    }
}
