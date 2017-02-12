using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CMS.Helper
{
    public class ConfigWeb
    {
        public static int PageSize = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
    }
}