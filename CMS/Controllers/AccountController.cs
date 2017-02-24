﻿using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CMS.Controllers
{
    public class AccountController : Controller
    {
        CmsDataDataContext db = new CmsDataDataContext();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Login()
        {
            try
            {
                AccountViewModel model = new AccountViewModel();

                string username = "false";
                HttpCookie reCookie = Request.Cookies["rememberCookies"];
                if (reCookie != null) { username = Server.HtmlEncode(reCookie.Value); }
                if (username.Split(',')[0].Trim().ToLower() == "true")
                {
                    if (!CheckUserLogin(int.Parse(username.Split(',')[1].Trim())))
                    {
                        TempData["Error"] = "Tài khoản đang sử dụng phần mềm. vui lòng thử lại sau 5 phút.";
                        return View(model);
                    }
                    Session.Add("SS-USERID", username.Split(',')[1].Trim());
                    Session.Add("SS-FULLNAME", HttpUtility.UrlDecode(username.Split(',')[2].Trim()));
                    CheckAcceptedUser(int.Parse(username.Split(',')[1].Trim()), username.Split(',')[3].Trim());
                    return RedirectToAction("Index", "Home");
                }

                return View(model);
            }
            catch (Exception)
            {
                return View();
            }

        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = db.Users.FirstOrDefault(x => x.UserName.Equals(model.UserName) && x.Password.Equals(Helpers.md5(model.UserName.Trim() + "ozo" + model.Password.Trim()))
                                                        && x.IsDeleted == false && x.IsMember == true);
                    if (user != null)
                    {
                        Session.Add("SS-USER", user);
                        Session.Add("SS-USERID", user.Id);
                        CheckAcceptedUser(user.Id, user.IsFree.ToString());
                        // set cookies for user
                        HttpCookie rememberCookie = new HttpCookie("rememberCookies");
                        rememberCookie.Value = model.RememberMe.ToString() + "," + user.Id + "," + HttpUtility.UrlEncode(user.FullName) + "," + user.IsFree;
                        rememberCookie.Expires = DateTime.Now.AddDays(3);
                        Response.Cookies.Add(rememberCookie);

                        //check login user
                        if (!CheckUserLogin(user.Id))
                        {
                            TempData["Error"] = "Tài khoản đang sử dụng phần mềm. vui lòng thử lại sau.";
                            return RedirectToAction("Login", "Account");
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("CredentialError", "Mật khẩu không đúng hoặc bạn chưa có quyền đăng nhập. vui lòng liên hệ sđt " + Information.HOT_PHONE_NUMBER);
                    return View("Login");
                }
                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
                return View("Login");
            }
        }

        private Boolean CheckUserLogin(int userId)
        {
            var currentApp = System.Web.HttpContext.Current.Application["usr_" + userId];

            if (currentApp != null)
            {
                if (System.Web.HttpContext.Current.Application["usr_" + userId].Equals("true"))
                {
                    return false;
                }
            }
            //storing session to login at sametime
            System.Web.HttpContext.Current.Application["usr_" + userId] = "true";

            return true;
        }
        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Logout()
        {
            HttpCookie rememberCookies = new HttpCookie("rememberCookies");
            rememberCookies.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(rememberCookies);
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Create()
        {
            UserModel model = new UserModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                //check captcha
                const string verifyUrl = "https://www.google.com/recaptcha/api/siteverify";
                string secret = ConfigWeb.Captcha_Secret_Key;
                var response = Request["g-recaptcha-response"];
                var remoteIp = Request.ServerVariables["REMOTE_ADDR"];

                var myParameters = String.Format("secret={0}&response={1}&remoteip={2}", secret, response, remoteIp);

                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    var json = wc.UploadString(verifyUrl, myParameters);
                    var js = new DataContractJsonSerializer(typeof(RecaptchaResult));
                    var ms = new MemoryStream(Encoding.ASCII.GetBytes(json));
                    var result = js.ReadObject(ms) as RecaptchaResult;
                    if (result != null && result.Success) // SUCCESS!!!
                    {
                        var u = new User
                        {
                            UserName = model.UserName,
                            FullName = model.FullName,
                            Password = Helpers.md5(model.UserName.Trim() + "ozo" + model.PassWord.Trim()),
                            Sex = model.Sex,
                            Phone = model.Phone,
                            Email = model.Email,
                            IsDeleted = false,
                            IsMember = false,
                            IsFree = false
                        };
                        var userId = new UserBussiness().Insert(u);
                        var roleUser = new Role_User
                        {
                            UserId = userId,
                            RoleId = 2 // roleId = 2: tài khoản khách hàng
                        };
                        // Insert User to Roles
                        try
                        {
                            new RoleUserBussiness().Insert(roleUser);
                            TempData["Success"] = "Tài khoản đã được khởi tạo. Quản trị viên sẽ liên lạc với bạn ngay khi duyệt tài khoản.";
                        }
                        catch (Exception)
                        {
                            TempData["Error"] = Messages_Contants.ERROR_COMMON;

                        }
                    }
                }
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        #region Check user exist
        [AllowAnonymous]
        //[HttpPost]
        public JsonResult doesUserNameExist(string UserName)
        {
            bool ifUserExists = false;
            try
            {
                ifUserExists = IsUserExists(UserName) ? false : true;
                return Json(!ifUserExists, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        //[HttpPost]
        public JsonResult doesUserNameNotExist(string UserName)
        {
            bool ifUserExists = false;
            try
            {
                ifUserExists = IsUserExists(UserName) ? true : false;
                return Json(!ifUserExists, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        private bool IsUserExists(string UserName)
        {
            var user = db.Users.FirstOrDefault(x => x.UserName.Equals(UserName));
            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        //check tai khoan con tien su dung
        private void CheckAcceptedUser(int userId, string isFree)
        {
            if (isFree.ToLower().Trim() == "true") Session["USER-ACCEPTED"] = true;
            else Session["USER-ACCEPTED"] = db.PaymentAccepteds.Any(x => x.UserId == userId && x.StartDate.Date >= DateTime.Now.Date && DateTime.Now.Date <= x.EndDate.Date);
        }
    }
}