using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class ReportTotalViewModel
    {
        public List<ReportTotalModel> ReportTotal { get; set; }
        public List<PaymentMethod> PaymentMethodList { get; set; }
        public List<DateTime> DateTime { get; set; }
        public DateTime DatePick { get; set; }
        public List<ReportDetailModel> ReportDetail { get; set; }
    }

    public class ReportTotalModel
    {
        public DateTime CreateDate { get; set; }
        public int PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public string Payment { get; set; }
    }

    public class ReportDetailModel
    {
        public DateTime CreateDate { get; set; }
        public decimal Amount { get; set; }
        public string Payment { get; set; }
        public string Note { get; set; }
        public string CustomerName { get; set; }
        public int DisplayOrder { get; set; }
    }
}