using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Common
{
    public class Crypto
    {
        public string Base64Code(string inputText)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);
            return Convert.ToBase64String(inputBytes);
        }
        public string Base64Decode(string inputText)
        {
            byte[] inputBytes = Convert.FromBase64String(inputText);
            return Encoding.UTF8.GetString(inputBytes);

        }
        public static string Hash(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            var m5 = MD5.Create();
            byte[] inputBye;
            byte[] outputBye;
            inputBye = Encoding.UTF8.GetBytes(text);
            outputBye = m5.ComputeHash(inputBye);
            return Convert.ToBase64String(outputBye);
        }
        public static string MD5Sign(string text, Encoding code)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            var m5 = MD5.Create();
            byte[] inputBye;
            byte[] outputBye;
            inputBye = code.GetBytes(text);
            outputBye = m5.ComputeHash(inputBye);
            return BitConverter.ToString(outputBye).Replace("-", "");
        }
        public static string MD5Sign(string text)
        {
            return MD5Sign(text, Encoding.UTF8);
        }
        public static byte[] Compress(string content)
        {
            return Compress(content, Encoding.UTF8);
        }
        public static byte[] Compress(string content, Encoding encoding)
        {
            if (string.IsNullOrEmpty(content)) return new byte[0];
            var bts = encoding.GetBytes(content);
            using (var ms = new MemoryStream())
            {
                using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(bts, 0, bts.Length);
                    zip.Close();
                    return ms.ToArray();
                }
            }
        }
        public static string UnCompress(byte[] content)
        {
            return UnCompress(content, Encoding.UTF8);
        }
        public static string UnCompress(byte[] content, Encoding encoding)
        {
            if (content == null) return "";
            using (var ms = new MemoryStream(content))
            {
                using (var zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    using (var outBuffer = new MemoryStream())
                    {
                        byte[] byts = new byte[1024];
                        int len = 0;
                        while (true)
                        {
                            len = zip.Read(byts, 0, byts.Length);
                            if (len <= 0) break;
                            outBuffer.Write(byts, 0, len);
                        }
                        outBuffer.Close();
                        return encoding.GetString(outBuffer.ToArray());
                    }
                }
            }
        }
    }
}
