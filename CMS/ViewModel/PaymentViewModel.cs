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

        [Required(ErrorMessage = "Không được để trống")]
        [Remote("doesUserNameExist", "Account", ErrorMessage = "Tài khoản chưa được đăng ký. vui lòng liên hệ admin để được trợ giúp")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [RegularExpression("([0-9,]+)", ErrorMessage = "Vui lòng nhập số tiền bằng số")]
        public string Amount { get; set; }

        public int PaymentMethodId { get; set; }

        public string Note { get; set; }
    }

    //public class PaymentHistory
    //{
    //    public int Id { get; set; }
    //    public int CustomerId { get; set; }
    //}
}