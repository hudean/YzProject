using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YzGraduationProject.Common
{
    /// <summary>
    /// 加密帮助类
    /// </summary>
    public static class Encrypt
    {
        /// <summary>
        /// md5加密(推荐)
        /// </summary>
        /// <param name="content">要加密的内容</param>
        /// <param name="isUpper">是否大写，默认小写</param>
        /// <param name="is16">是否是16位，默认32位</param>
        /// <returns></returns>
        public static string Md5Encrypt(string content, bool isUpper = false, bool is16 = false)
        {
            //MD5 md5 = new MD5CryptoServiceProvider();
            using (var md5 = MD5.Create())
            {
                //获取密文字节数组
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
                //转换成字符串，32位
                string md5Str = BitConverter.ToString(result);
                //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
                md5Str = md5Str.Replace("-", "");
                md5Str = isUpper ? md5Str : md5Str.ToLower();
                return is16 ? md5Str.Substring(8, 16) : md5Str;
            }
        }

        /// <summary>
        /// 32位的MD5加密 即后转16进制小写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Hash(string input)
        {
            try
            {
                MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
                byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            catch { return null; }
        }

        public static string Md5Decrypt(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            //转换成字符串，32位
            string strResult = BitConverter.ToString(bytResult);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            strResult = strResult.Replace("-", "");
            return strResult.ToUpper();
        }



        #region AES base64加解密算法

        /// <summary>
        /// AES base64 加密算法
        /// Key 为16位
        /// </summary>
        /// <param name="Data">需要加密的字符串</param>
        /// <param name="Key">Key 为16位 密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns></returns>
        public static string RST_AesEncrypt_Base64(string Data, string Key)
        {
            if (string.IsNullOrEmpty(Data))
            {
                return null;
            }
            if (string.IsNullOrEmpty(Key))
            {
                return null;
            }
            var Vector = Key.Substring(0, 16);
            Byte[] plainBytes = Encoding.UTF8.GetBytes(Data);
            Byte[] bKey = new Byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] Cryptograph = null; // 加密后的密文
            Rijndael Aes = Rijndael.Create();
            //add
            Aes.Mode = CipherMode.CBC;//兼任其他语言的des
            Aes.BlockSize = 128;
            Aes.Padding = PaddingMode.PKCS7;
            //add end
            try
            {
                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(plainBytes, 0, plainBytes.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }
            return Convert.ToBase64String(Cryptograph);
        }

        /// <summary>
        /// AES base64 解密算法
        /// Key为16位
        /// </summary>
        /// <param name="Data">需要解密的字符串</param>
        /// <param name="Key">Key为16位 密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns></returns>
        public static string RST_AesDecrypt_Base64(string Data, string Key)
        {
            try
            {
                if (string.IsNullOrEmpty(Data))
                {
                    return null;
                }
                if (string.IsNullOrEmpty(Key))
                {
                    return null;
                }
                //hhb，为了解决空格加号等无效字符串
                if (Data.Contains(" "))
                {
                    Data = Data.Replace(' ', '+');
                }
                var Vector = Key.Substring(0, 16);
                Byte[] encryptedBytes = Convert.FromBase64String(Data);
                Byte[] bKey = new Byte[32];
                Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
                Byte[] bVector = new Byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
                Byte[] original = null; // 解密后的明文
                Rijndael Aes = Rijndael.Create();
                //add
                Aes.Mode = CipherMode.CBC;//兼任其他语言的des
                Aes.BlockSize = 128;
                Aes.Padding = PaddingMode.PKCS7;
                //add end
                try
                {
                    // 开辟一块内存流，存储密文
                    using (MemoryStream Memory = new MemoryStream(encryptedBytes))
                    {
                        //把内存流对象包装成加密流对象
                        using (CryptoStream Decryptor = new CryptoStream(Memory,
                        Aes.CreateDecryptor(bKey, bVector),
                        CryptoStreamMode.Read))
                        {
                            // 明文存储区
                            using (MemoryStream originalMemory = new MemoryStream())
                            {
                                Byte[] Buffer = new Byte[1024];
                                Int32 readBytes = 0;
                                while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                                {
                                    originalMemory.Write(Buffer, 0, readBytes);
                                }
                                original = originalMemory.ToArray();
                            }
                        }
                    }
                }
                catch
                {
                    original = null;
                }
                return Encoding.UTF8.GetString(original);
            }
            catch { return null; }
        }

        #endregion AES base64加解密算法

        #region Des加解密算法

        /// <summary>
        /// DES加密算法
        /// sKey为8位或16位
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>
        public static string DesEncrypt(string pToEncrypt, string sKey)
        {
            try
            {
                if (string.IsNullOrEmpty(pToEncrypt))
                {
                    return null;
                }
                StringBuilder ret = new StringBuilder();
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                    des.Key = Encoding.UTF8.GetBytes(sKey);
                    des.IV = Encoding.UTF8.GetBytes(sKey);
                    //des.Mode = CipherMode.CBC;//兼任其他语言的des
                    //des.BlockSize = 64;
                    //des.KeySize = 64;
                    //des.Padding = PaddingMode.PKCS7;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();

                            foreach (byte b in ms.ToArray())
                            {
                                ret.AppendFormat("{0:X2}", b);
                            }
                            return ret.ToString();
                            //return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// DES解密算法
        /// sKey为8位或16位
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="sKey">密钥</param>
        /// <returns></returns>
        public static string DesDecrypt(string pToDecrypt, string sKey)
        {
            try
            {
                if (string.IsNullOrEmpty(pToDecrypt))
                    return null;
                string str = "";
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                    for (int x = 0; x < pToDecrypt.Length / 2; x++)
                    {
                        int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                        inputByteArray[x] = (byte)i;
                    }
                    des.Key = Encoding.UTF8.GetBytes(sKey);
                    des.IV = Encoding.UTF8.GetBytes(sKey);
                    //des.Mode = CipherMode.CBC;//兼任其他语言的des
                    //des.Padding = PaddingMode.PKCS7;
                    //des.BlockSize = 64;
                    //des.KeySize = 64;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();

                            str = Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
                return str;
            }
            catch { return null; }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DesEncrypt(string password)
        {
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
            byte[] key = Encoding.UTF8.GetBytes("@qwe");//定义字节数组用来存储密钥
            byte[] data = Encoding.UTF8.GetBytes(password);//定义字节数组，用来存储加密对象
            MemoryStream MStream = new MemoryStream();
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);
            CStream.FlushFinalBlock();//释放解秘流
            string encryptPwd = Convert.ToBase64String(MStream.ToArray());
            return encryptPwd;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptStr"></param>
        /// <returns></returns>
        public static string DesDecrypt(string decryptStr)
        {
            //实例化加密对象
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();
            byte[] key = Encoding.UTF8.GetBytes("@qwe");//定义字节数组用来存储密钥
            byte[] data = Convert.FromBase64String(decryptStr);//定义字节数组，用来存储加密对象
            //实例化内存对象
            MemoryStream MStream = new MemoryStream();
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);
            CStream.FlushFinalBlock();//释放解秘流
            string decrypttPwd = Encoding.UTF8.GetString(MStream.ToArray());
            return decrypttPwd;
        }

        #endregion Des加解密算法

        #region 系统WebApi接口DES base64加解密算法

        /// <summary>
        /// C# DES加密方法 返回base64字符串
        /// Mode=CBC,PaddingMode=PKCS7,BlockSize=KeySize=64,Iv=Key长度
        /// </summary>
        /// <param name="encryptedValue">要加密的字符串</param>
        /// <param name="key">密钥 为8位</param>
        /// <returns>加密后的字符串</returns>
        public static string DESEncrypt_Base64(string originalValue, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(originalValue))
                    return "";
                using (DESCryptoServiceProvider sa = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(key) })
                {
                    using (ICryptoTransform ct = sa.CreateEncryptor())
                    {
                        byte[] by = Encoding.UTF8.GetBytes(originalValue);
                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                            {
                                cs.Write(by, 0, by.Length);
                                cs.FlushFinalBlock();
                            }
                            return Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
            }
            catch { }
            return "";
        }

        /// <summary>
        /// C# DES解密方法返回UTF-8格式的字符串
        /// Mode=CBC,PaddingMode=PKCS7,BlockSize=KeySize=64,Iv=Key长度
        /// </summary>
        /// <param name="encryptedValue">待解密的字符串</param>
        /// <param name="key">密钥 为8位</param>
        /// <returns>解密后的字符串</returns>
        public static string DESDecrypt_Base64(string encryptedValue, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(encryptedValue))
                    return null;

                //hhb，为了解决空格加号等无效字符串
                if (encryptedValue.Contains(" "))
                {
                    encryptedValue = encryptedValue.Replace(' ', '+');
                }
                using (DESCryptoServiceProvider sa = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(key) })
                {
                    using (ICryptoTransform ct = sa.CreateDecryptor())
                    {
                        byte[] byt = Convert.FromBase64String(encryptedValue);

                        using (var ms = new MemoryStream())
                        {
                            using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                            {
                                cs.Write(byt, 0, byt.Length);
                                cs.FlushFinalBlock();
                            }
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return "";
        }

        #endregion 系统WebApi接口DES base64加解密算法
    }
}
