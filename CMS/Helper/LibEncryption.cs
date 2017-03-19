using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;
namespace CMS.Helper
{
    public sealed class libAES
    {
        private static readonly libAES _instance = new libAES();

        /// <summary>
        /// prevent instantiation from other classes
        /// </summary>
        private libAES() { }

        /// <summary>
        /// get instance of the class
        /// </summary>
        /// <returns></returns>
        public static libAES Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// Decrypt string using AES 128
        /// </summary>
        /// <param name="cipheredtext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public String Decrypt(String cipheredtext, String key)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes(key);
            byte[] cipheredData = Convert.FromBase64String(cipheredtext);

            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;

            //16 ascii characters for IV
            byte[] IVbytes = new byte[16];

            ICryptoTransform decryptor = aes.CreateDecryptor(keybytes, IVbytes);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(cipheredData);
            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            StreamReader creader = new StreamReader(cs, Encoding.UTF8);

            String Base64 = creader.ReadToEnd();

            ms.Close();
            cs.Close();

            return Encoding.UTF8.GetString(Convert.FromBase64String(Base64));
        }

        /// <summary>
        /// Encrypt string using AES 128
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public String Encrypt(String plaintext, String key)
        {
            byte[] keybytes = Encoding.UTF8.GetBytes(key);

            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            byte[] IVbytes = new byte[16];

            ICryptoTransform encryptor = aes.CreateEncryptor(keybytes, IVbytes);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

            byte[] plainBytes = Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)));

            cs.Write(plainBytes, 0, plainBytes.Length);

            cs.FlushFinalBlock();

            byte[] cipherBytes = ms.ToArray();

            ms.Close();
            cs.Close();

            return Convert.ToBase64String(cipherBytes, 0, cipherBytes.Length);
        }
    }
}
