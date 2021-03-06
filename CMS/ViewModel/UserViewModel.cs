﻿using CMS.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace CMS.ViewModel
{
    public class UserViewModel
    {
        public List<UserModel> UserList { get; set; }
        public int Totalpage { get; set; }
        public List<SelectListItem> ManagerList { get; set; }
        public int ManagerId { get; set; }
        public List<SelectListItem> PaymentStatus { get; set; }
        public int StatusId { get; set; }
        public CustomerDetail CustomerDetail { get; set; }
        public NewsCustomerActionModel ActionDetailUser { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [Remote("doesUserNameNotExist", "Account", ErrorMessage = "Tài khoản đã được đăng ký. vui lòng chọn tài khoản khác")]
        [RegularExpression("^[0-9]{10,11}$", ErrorMessage = "Vui lòng nhập số điện thoại (Số điện thoại bao gồm dãy số từ 10 đến 11 số.)")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu mới có độ dài tối tiểu 6 kí tự", MinimumLength = 6)]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [System.Web.Mvc.Compare("PassWord", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassWord { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public Boolean IsMember { get; set; }
        public Boolean IsRestore { get; set; }
        public string ManagerBy { get; set; }
        public List<Role_User> RoleUsers { get; set; }
        public List<RoleModel> ListRoles { get; set; }

        public Boolean Sex { get; set; }

        public List<SelectListItem> ManagerList { get; set; }

        public string RoleName { get; set; }

        public Boolean IsDelete { get; set; }
        public Boolean IsOnline { get; set; }
        public DateTime EndTimePayment { get; set; }
        public int ManagerId { get; set; }
        public string Notes { get; set; }
    }

    public class UserModelApi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [Remote("doesUserNameNotExist", "Account", ErrorMessage = "Tài khoản đã được đăng ký. vui lòng chọn tài khoản khác")]
        [RegularExpression("^[0-9]{10,11}$", ErrorMessage = "Vui lòng nhập số điện thoại (Số điện thoại bao gồm dãy số từ 10 đến 11 số.)")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(100, ErrorMessage = "Mật khẩu mới có độ dài tối tiểu 6 kí tự", MinimumLength = 6)]
        public string PassWord { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [System.Web.Mvc.Compare("PassWord", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassWord { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public Boolean IsMember { get; set; }
        public Boolean IsRestore { get; set; }
        public string ManagerBy { get; set; }
        public List<Role_User> RoleUsers { get; set; }
        public List<RoleModel> ListRoles { get; set; }

        public Boolean Sex { get; set; }

        public List<SelectListItem> ManagerList { get; set; }

        public string RoleName { get; set; }

        public Boolean IsDelete { get; set; }
        public Boolean IsOnline { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string TimeEndStr { get; set; }
        public int ManagerId { get; set; }
        public string Notes { get; set; }
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

    [DataContract]
    public class RecaptchaResult
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "error-codes")]
        public string[] ErrorCodes { get; set; }
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

    public class CustomerDetailApi
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ManagerBy { get; set; }
        public DateTime? TimeEnd { get; set; }
        public string TimeEndStr { get; set; }
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

    public class NewsCustomerActionModel
    {
        public int Id { get; set; }
        public int SumIscc { get; set; }
        public int SumIsReport { get; set; }
        public string UserName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}