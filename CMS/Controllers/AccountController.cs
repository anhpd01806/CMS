using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.Models;
using CMS.ViewModel;
using Elmah;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static CMS.Common.Common;

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
                if (Session["SS-USERID"] != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                AccountViewModel model = new AccountViewModel();

                string username = "false";
                HttpCookie reCookie = Request.Cookies["rememberCookies"];
                if (reCookie != null) { username = Server.HtmlEncode(reCookie.Value); }
                if (username.Split(',')[0].Trim().ToLower() == "true")
                {
                    AddUserLogin(int.Parse(username.Split(',')[1].Trim()));

                    Session.Add("SS-USERID", username.Split(',')[1].Trim());
                    Session.Add("SS-FULLNAME", HttpUtility.UrlDecode(username.Split(',')[2].Trim()));
                    CheckAcceptedUser(int.Parse(username.Split(',')[1].Trim()), username.Split(',')[3].Trim());
                    Session["IS-NOTIFY"] = bool.Parse(username.Split(',')[4].Trim());
                    bool isUser = (bool)Session["IS-USERS"];
                    GetNotifyUser(isUser, int.Parse(username.Split(',')[1].Trim()));

                    //update last login user
                    UpdateLastLoginUser(int.Parse(username.Split(',')[1].Trim()));
                    return RedirectToAction("Index", "Home");
                }
                // set seesion for notify
                if (Session["OtherLogin"] != null)
                {
                    TempData["Error"] = Session["OtherLogin"].ToString();
                    Session["OtherLogin"] = null;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(ex));
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
        //[ValidateAntiForgeryToken]
        public ActionResult Login(AccountViewModel model)
        {
            try
            {
                if (Session["SS-USERID"] != null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (ModelState.IsValid)
                {
                    var user = db.Users.FirstOrDefault(x => x.UserName.Equals(model.UserName) && x.Password.Equals(Helpers.md5(model.UserName.Trim() + "ozo" + model.Password.Trim()))
                                                        && x.IsDeleted == false && x.IsMember == true);
                    if (user != null)
                    {
                        bool isNotify;
                        isNotify = user.IsNotify ?? true;

                        //add login information
                        AddUserLogin(user.Id);

                        //update last login user
                        UpdateLastLoginUser(user.Id);

                        Session.Add("SS-USER", user);
                        Session.Add("SS-USERID", user.Id);

                        // kt khách hàng đã đăng ký gói cước chưa?
                        CheckAcceptedUser(user.Id, user.IsFree.ToString());
                        // set cookies for user
                        HttpCookie rememberCookie = new HttpCookie("rememberCookies");
                        rememberCookie.Value = model.RememberMe.ToString() + "," + user.Id + "," + HttpUtility.UrlEncode(user.FullName) + "," + user.IsFree + "," + isNotify;
                        rememberCookie.Expires = DateTime.Now.AddDays(3);
                        Response.Cookies.Add(rememberCookie);
                        // set sesssion ative notify
                        Session["IS-NOTIFY"] = isNotify;

                        bool isUser = (bool)Session["IS-USERS"];
                        GetNotifyUser(isUser, user.Id);
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

        private void AddUserLogin(int userId)
        {
            string info = GetLocalIPAddress();
            // gán vào session hiện tại
            Session["TokenInfoLogin"] = md5(info);
            var currentApp = (List<LoginInfomation>)System.Web.HttpContext.Current.Application["LoginInfomation"];
            if (currentApp != null)
            {
                // update lại private key khi đăng nhập tại 1 nơi khác
                var check = currentApp.FirstOrDefault(x => x.UserId == userId);
                if (check != null) check.PrivateKey = md5(info);
                else
                {
                    currentApp.Add(new LoginInfomation
                    {
                        UserId = userId,
                        PrivateKey = md5(info)
                    });
                }
            }
            else
            {
                List<LoginInfomation> login = new List<LoginInfomation>();
                login.Add(new LoginInfomation
                {
                    UserId = userId,
                    PrivateKey = md5(info)
                });
                System.Web.HttpContext.Current.Application["LoginInfomation"] = login;
            }
        }

        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = "";
            if (host != null) ipAddress = host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
            // get name of machine
            string nameMachine = Environment.MachineName;
            // get web browther
            var browser = Request.Browser;
            string browerInfo = browser.Type + browser.Version + browser.Type;

            return ipAddress + "|" + nameMachine + "|" + browerInfo;
        }
        /// <summary>
        /// Đăng xuất
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult Logout()
        {

            // kiểm tra xem tài khoản có đăng nhập ở thiết bị khác hay không
            var currentApp = (List<LoginInfomation>)System.Web.HttpContext.Current.Application["LoginInfomation"];
            var tokenLogin = currentApp.FirstOrDefault(x => x.UserId == int.Parse(Session["SS-USERID"].ToString()));
            if (tokenLogin != null) tokenLogin.PrivateKey = "";
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
                string secret = ConfigWeb.Captcha_SecretUser_Key;
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
                            CreatedOn = DateTime.Now,
                            Password = Helpers.md5(model.UserName.Trim() + "ozo" + model.PassWord.Trim()),
                            Sex = true,
                            Phone = model.UserName,
                            Email = "",
                            IsDeleted = false,
                            IsMember = true,
                            ManagerBy = 1,
                            IsFree = false
                        };
                        var userId = new UserBussiness().Insert(u);
                        //Insert to tbl notify
                        var notify = new Notify
                        {
                            UserName = "Khách hàng ",
                            Userid = userId,
                            SendFlag = true,
                            DateSend = DateTime.Now,
                            Title = "Đăng ký mới",
                            Accepted = false,
                            ViewFlag = false,
                            Type = 1
                        };
                        var notifyId = new NotifyBussiness().Insert(notify);
                        //set iduser vừa insert vào
                        notify.Id = notifyId;
                        //gan notify vao viewbag
                        TempData["Notify"] = notify;

                        var roleUser = new Role_User
                        {
                            UserId = userId,
                            RoleId = 2 // roleId = 2: tài khoản khách hàng
                        };
                        // Insert User to Roles
                        try
                        {
                            new RoleUserBussiness().Insert(roleUser);
                            TempData["Success"] = "Tài khoản của bạn đã được đăng ký thành công vui lòng liên hệ 0982667700 để được hướng dẫn xem tin chính chủ";
                        }
                        catch (Exception)
                        {
                            TempData["Error"] = Messages_Contants.ERROR_COMMON;

                        }
                    }
                    else TempData["Error"] = "Vui lòng xác nhận captcha";
                }
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        #region json
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
        #endregion

        #region Private
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
        //check tai khoan con tien su dung
        private void CheckAcceptedUser(int userId, string isFree)
        {
            if (isFree.ToLower().Trim() == "true")
            {
                Session["USER-ACCEPTED"] = true;
                Session["IS-USERS"] = false;
            }
            else
            {
                Session["USER-ACCEPTED"] = db.PaymentAccepteds.Any(x => x.UserId == userId && DateTime.Now <= x.EndDate);
                Session["IS-USERS"] = true;
            }
        }

        private void GetNotifyUser(bool IsUser, int UserId)
        {
            List<NoticeModel> lstNotify = new NotifyBussiness().GetNotify(IsUser, UserId);
            //nếu tk đăng nhập sắp hết tiền thì thông báo 
            var check = db.PaymentAccepteds.Where(x => x.UserId == UserId && x.EndDate >= DateTime.Now.AddDays(-1) && x.EndDate <= DateTime.Now).Any();
            if (check)
            {
                NoticeModel notyfi = new NoticeModel();
                notyfi.Type = 4;
                notyfi.Id = 0;
                notyfi.Title = "Tài khoản của bạn sắp hết hạn sử dụng , Vui lòng đăng ký mới gói cước để sử dụng dịch vụ.";
                notyfi.DateSend = DateTime.Now;
                notyfi.UserName = "Admin";
                lstNotify.Add(notyfi);
            }
            Session["NotityUser"] = lstNotify;
        }

        private Boolean CheckUserIsUser(int userId)
        {
            var rs = (from r in db.Roles
                      join ru in db.Role_Users
                      on r.Id equals ru.RoleId
                      join u in db.Users
                      on ru.UserId equals u.Id
                      where r.Id == 2 && u.Id == userId
                      select r).Any();
            return rs;
        }

        private void UpdateLastLoginUser(int userId)
        {
            new UserBussiness().UpdateLastLogin(userId);
        }
        #endregion
    }
}