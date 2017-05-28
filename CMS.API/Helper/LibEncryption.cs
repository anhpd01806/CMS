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

        public string EncryptText(string input, string password)
        {

            AesManaged tdes = new AesManaged();
            tdes.Key = Encoding.UTF8.GetBytes(password);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] plain = Encoding.UTF8.GetBytes(input);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            String encryptedText = Convert.ToBase64String(cipher);
            return encryptedText;
        }

    }
}
