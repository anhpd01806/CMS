﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class UserViewModel
    {
        public List<UserModel> UserList { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [Remote("doesUserNameExist", "User", ErrorMessage = "Tài khoản đã được đăng ký. vui lòng chọn tài khoản khác")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu mới có độ dài tối tiểu 6 kí tự", MinimumLength = 6)]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [System.Web.Mvc.Compare("PassWord", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassWord { get; set; }

        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng định dạng email")]
        public string Email { get; set; }
        public Boolean IsMember { get; set; }
        public string ManagerBy { get; set; }

        public List<RoleModel> ListRoles { get; set; }

        public Boolean Sex { get; set; }

        public List<SelectListItem> ManagerList { get; set; }

        public string RoleName { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [StringLength(100, ErrorMessage = "Mật khẩu mới có độ dài tối tiểu 6 kí tự", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu xác nhận không được để trống.")]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Mật khẩu mới và mật khẩu xác nhận không trùng khớp.")]
        public string ConfirmPassword { get; set; }

    }
}