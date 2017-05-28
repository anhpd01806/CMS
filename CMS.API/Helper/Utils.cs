using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CMS.API.Helper
{
    public class Utils
    {
        private static readonly string[] VietnameseSigns = new string[]
        {

        "aAeEoOuUiIdDyY",

        "áàạảãâấầậẩẫăắằặẳẵ",

        "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

        "éèẹẻẽêếềệểễ",

        "ÉÈẸẺẼÊẾỀỆỂỄ",

        "óòọỏõôốồộổỗơớờợởỡ",

        "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

        "úùụủũưứừựửữ",

        "ÚÙỤỦŨƯỨỪỰỬỮ",

        "íìịỉĩ",

        "ÍÌỊỈĨ",

        "đ",

        "Đ",

        "ýỳỵỷỹ",

        "ÝỲỴỶỸ"

        };

        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            return str;

        }

        public static string RemoveHtml(string source)
        {
            return Regex.Replace(Regex.Replace(source, "<.*?>", string.Empty), @"\s+", " ");
        }

        public static string FormatPrice(decimal? price)
        {
            if (price.HasValue && price != 0)
            {
                string result = price.Value.ToString("#,#");
                if (result.Contains(","))
                    result = result.Replace(',', '.');
                return result;
            }
            return "0";
        }

        public static string ConvertPrice(string price)
        {
            try
            {
                StringBuilder PriceStr = new StringBuilder();
                if (price.Length > 9)
                {
                    PriceStr.Append(price.Substring(0, price.Length - 9) + " tỷ ");
                    double milionPrice = int.Parse(price.Substring(price.Length - 9));
                    if (milionPrice > 0) PriceStr.Append(string.Format("{0:n0}", Math.Round((milionPrice / 1000000), 1)) + " triệu");
                }
                else
                {
                    double milionPrice = int.Parse(price);
                    PriceStr.Append(string.Format("{0:n1}", Math.Round((milionPrice / 1000000), 1)) + " triệu");
                }
                return PriceStr.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}