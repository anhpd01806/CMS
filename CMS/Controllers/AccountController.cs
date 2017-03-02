using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
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
                    if (!CheckUserLogin(int.Parse(username.Split(',')[1].Trim())))
                    {
                        TempData["Error"] = "Tài khoản đang sử dụng phần mềm ở một nơi khác. vui lòng thử lại sau 5 phút.";
                        return View(model);
                    }
                    // set seesion for notify
                    Session.Add("SS-USERID", username.Split(',')[1].Trim());
                    Session.Add("SS-FULLNAME", HttpUtility.UrlDecode(username.Split(',')[2].Trim()));
                    CheckAcceptedUser(int.Parse(username.Split(',')[1].Trim()), username.Split(',')[3].Trim());
                    Session["IS-NOTIFY"] = username.Split(',')[4].Trim();
                    bool isUser = (bool)Session["IS-USERS"];
                    GetNotifyUser(isUser, int.Parse(username.Split(',')[1].Trim()));

                    //update last login user
                    UpdateLastLoginUser(int.Parse(username.Split(',')[1].Trim()));
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
                        bool isNotify;
                        isNotify = user.IsNotify??true;
                        //check login user
                        if (!CheckUserLogin(user.Id))
                        {
                            TempData["Error"] = "Tài khoản đang sử dụng phần mềm. vui lòng thử lại sau 5 phút.";
                            return RedirectToAction("Login", "Account");
                        }
                        //update last login user
                        UpdateLastLoginUser(user.Id);

                        Session.Add("SS-USER", user);
                        Session.Add("SS-USERID", user.Id);

                        // kt khách hàng đã đăng ký gói cước chưa?
                        CheckAcceptedUser(user.Id, user.IsFree.ToString());
                        // set cookies for user
                        HttpCookie rememberCookie = new HttpCookie("rememberCookies");
                        rememberCookie.Value = model.RememberMe.ToString() + "," + user.Id + "," + HttpUtility.UrlEncode(user.FullName) + "," + user.IsFree +","+ isNotify;
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
                            Sex = true,
                            Phone = model.UserName,
                            Email = "",
                            IsDeleted = false,
                            IsMember = false,
                            IsFree = false
                        };
                        var userId = new UserBussiness().Insert(u);
                        //Insert to tbl notify
                        var notify = new Notify
                        {
                            UserName = "Guest",
                            Userid = userId,
                            SendFlag = true,
                            DateSend = DateTime.Now,
                            Title = "Yêu cầu tạo tài khoản mới",
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
                            TempData["Success"] = "Tài khoản đã được khởi tạo. Quản trị viên sẽ liên lạc với bạn ngay khi duyệt tài khoản.";
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
            List<Models.NoticeModel> lstNotify = new NotifyBussiness().GetNotify(IsUser, UserId);
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