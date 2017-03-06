using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace CMS.Helper
{
    public class Utils
    {
        public static string RemoveHtml(string source)
        {
            return System.Text.RegularExpressions.Regex.Replace(Regex.Replace(source, "<.*?>", string.Empty), @"\s+", " ");
        }
    }
}