using CMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class PaymentViewModel
    {
        public List<PaymentMethod> PaymentMethodList { get; set; }

        public List<SelectListItem> PayMethodList { get; set; }

        public List<SelectListItem> PackageList { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [Remote("doesUserNameExist", "Account", ErrorMessage = "Tài khoản chưa được đăng ký. vui lòng liên hệ admin để được trợ giúp")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [RegularExpression("(-|)([0-9,]+)", ErrorMessage = "Vui lòng nhập số tiền bằng số")]
        public string Amount { get; set; }

        public int PaymentMethodId { get; set; }

        public string Note { get; set; }
        public long Payment { get; set; }
    }

    public class PaymentHisApi
    {
        public int TotalPage { get; set; }
        public List<PaymentHistoryModel> PaymentHisList { get; set; }
    }
    public class PaymentHistoryModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DateString { get; set; }
        public string Amount { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
    }
}