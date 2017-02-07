using System.Web.Mvc;
namespace CMS.ViewModel
{
    using System.ComponentModel.DataAnnotations;
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Không được để trống")]
        [Remote("doesUserNameExist", "Account", ErrorMessage = "Tài khoản chưa được đăng ký. vui lòng liên hệ admin để được trợ giúp")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool RememberMe { get; set; }
    }
}