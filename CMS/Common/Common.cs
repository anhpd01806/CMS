using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Specialized;

namespace CMS.Common
{
    public class Common
    {
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }

        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }

        public static string GenSign(string str, string key)
        {
            var encoding = new UTF8Encoding();
            var keyByte = encoding.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] dataToSign = Encoding.UTF8.GetBytes(str);
                var signature = hmacsha256.ComputeHash(dataToSign);
                return BitConverter.ToString(signature).Replace("-", "").ToLower();
            }
        }

        public static string Sort(NameValueCollection array)
        {
            var str = "";
            foreach (var item in array.AllKeys)
            {
                str += string.Format("&{0}={1}", item, UpperCaseUrlEncode(array[item].ToString()));
            }
            return str.Remove(0, 1);
        }

        public static string UpperCaseUrlEncode(string s)
        {
            char[] temp = s.ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }

        public class LoginInfomation
        {
            public int UserId { get; set; }
            public string PrivateKey { get; set; }
        }
    }
}