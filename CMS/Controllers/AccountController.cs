using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Linq;
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
                string username = "false";
                HttpCookie reCookie = Request.Cookies["rememberCookies"];
                if (reCookie != null) { username = Server.HtmlEncode(reCookie.Value); }
                if (username.Split(',')[0].Trim().ToLower() == "true")
                {
                    Session.Add("SS-USERID", username.Split(',')[1].Trim());
                    Session.Add("SS-FULLNAME", HttpUtility.UrlDecode(username.Split(',')[2].Trim()));
                    return RedirectToAction("Index", "Home");
                }
                AccountViewModel model = new AccountViewModel();
                return View(model);
            }
            catch (Exception ex)
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
                        // set cookies for user
                        HttpCookie rememberCookie = new HttpCookie("rememberCookies");
                        rememberCookie.Value = model.RememberMe.ToString() + "," + user.Id + "," + HttpUtility.UrlEncode(user.FullName);
                        rememberCookie.Expires = DateTime.Now.AddDays(3);
                        Response.Cookies.Add(rememberCookie);

                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("CredentialError", "Mật khẩu không đúng hoặc bạn chưa có quyền đăng nhập. vui lòng liên hệ sđt xxx.xxx");
                    return View("Login");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
                return View("Login");
            }
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
            catch (Exception ex)
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
    }
}