using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.API.Models
{
    public class CustomerDetailModel
    {
    }

    public class CustomerDetail
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ManagerBy { get; set; }
        public string TimeEnd { get; set; }
        public string Amount { get; set; }
        public DateTime LastLogin { get; set; }
        public string CashPayment { get; set; }
        public string CardPayment { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string Notes { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PaymentTotal
    {
        public int PaymentMethodId { get; set; }
        public string TypePayment { get; set; }
        public string AmoutPayment { get; set; }
    }
}