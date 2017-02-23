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

        public static string DayPackage = ConfigurationManager.AppSettings["DayPackage"];

        public static double MinPayment = Convert.ToDouble(ConfigurationManager.AppSettings["MinPayment"]);

        public static string MonthPackage = ConfigurationManager.AppSettings["MonthPackage"];
    }

    public enum NewsStatus
    {
        IsSave = 1,
        IsRead = 2,
        IsDelete = 3
    }

    public enum CmsRole
    {
        Administrator = 1,
        Customer = 2
    }
}