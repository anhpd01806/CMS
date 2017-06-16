using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CMS.Common
{
    public class APIConfig
    {
        public static string PrivateKey = ConfigurationManager.AppSettings["private_key"].ToString();
    }
}