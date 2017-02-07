using CMS.Bussiness;
using CMS.Data;
using CMS.Helper;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBackendPlus.Controllers;

namespace CMS.Controllers
{
    public class UserController : BaseAuthedController
    {
        // GET: User
        public ActionResult Index()
        {
            UserViewModel model = new UserViewModel();
            var allUser = new UserBussiness().GetAllUser();
            var allRoles = new RoleBussiness().GetRoles();
            var allRolesUser = new RoleUserBussiness().GetAllRoleUser();
            model.UserList = (from a in allUser
                              select new UserModel
                              {
                                  Id = a.Id,
                                  FullName = a.FullName,
                                  UserName = a.UserName,
                                  Phone = a.Phone,
                                  Email = a.Email,
                                  IsMember = a.IsMember ?? false,
                                  ManagerBy = a.ManagerBy != null ? allUser.Where(x => x.Id == a.ManagerBy).Select(x => x.FullName).FirstOrDefault() : "",
                                  RoleName = GetNameRole(allRoles, allRolesUser, a.Id)
                              }).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            UserModel model = new UserModel();
            var allRoles = new RoleBussiness().GetRoles();
            model.ManagerList = new UserBussiness().GetManagerUser();
            if (allRoles != null)
            {
                List<RoleModel> lstRoles = new List<RoleModel>();
                foreach (var role in allRoles)
                {
                    RoleModel roleView = new RoleModel
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                    lstRoles.Add(roleView);
                }
                model.ListRoles = lstRoles;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(UserModel model, string[] selectRoles)
        {
            if (ModelState.IsValid)
            {
                var u = new User
                {
                    UserName = model.UserName,
                    FullName = model.FullName,
                    Password = Helpers.md5(model.UserName.Trim() + "ozo" + model.PassWord.Trim()),
                    Sex = model.Sex,
                    Phone = model.Phone,
                    Email = model.Email,
                    ManagerBy = int.Parse(model.ManagerBy),
                    IsDeleted = false,
                    IsMember = true
                };

                int userId = 0;
                try
                {
                    userId = new UserBussiness().Insert(u);
                }
                catch (Exception)
                {
                    TempData["Error"] = Messages_Contants.ERROR_COMMON;
                    return RedirectToAction("Create", "User");
                }

                List<Role_User> lstRoleUser = new List<Role_User>();
                if (selectRoles != null)
                {
                    lstRoleUser.AddRange(selectRoles.Select(roleId => new Role_User
                    {
                        UserId = userId,
                        RoleId = Convert.ToInt32(roleId)
                    }));
                    // Insert User to Roles
                    try
                    {
                        foreach (var item in lstRoleUser)
                        {
                            new RoleUserBussiness().Insert(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = Messages_Contants.ERROR_COMMON;
                        return RedirectToAction("Create", "User");
                    }
                }
                TempData["Success"] = Messages_Contants.SUCCESS_INSERT;

                return RedirectToAction("Create", "User");

            }
            return View(model);
        }

        public ActionResult ResertPassword(string id)
        {

            TempData["Success"] = Messages_Contants.SUCCESS_RESETPASSWORD;
            return RedirectToAction("Index", "User");
        }

        public ActionResult RemoveUser(string id)
        {
            try
            {
                int userId = int.Parse(id);
                new UserBussiness().Delete(userId);
                TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_DELETE;
            }

            return RedirectToAction("Index", "User");
        }

        public ActionResult ChangePassword(int? id)
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (model.NewPassword == model.OldPassword)
            {
                TempData["Error"] = "Mật khẩu cũ và mật khẩu mới không được trùng nhau";
            }
            else
            {
                var userId = int.Parse(Session["SS-USERID"].ToString());
                var rs = new UserBussiness().ChangePassword(userId, model.NewPassword,model.OldPassword);
                if (rs == true)
                {
                    HttpCookie rememberCookies = new HttpCookie("rememberCookies");
                    rememberCookies.Expires = DateTime.Now.AddDays(-1d);
                    Response.Cookies.Add(rememberCookies);
                    Session.Abandon();
                    return RedirectToAction("Login", "Account");
                }
                else TempData["Error"] = "Mật khẩu cũ không đúng";
            }
            return View(new ChangePasswordViewModel());
        }
        private string GetNameRole(List<Role> role, List<Role_User> roleUser, int userId)
        {
            var rs = (from r in role
                      join ru in roleUser on r.Id equals ru.RoleId
                      where ru.UserId == userId
                      select r.Name).ToArray();
            return String.Join(", ", rs);
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
            CmsDataDataContext db = new CmsDataDataContext();
            var user = db.Users.FirstOrDefault(x => x.UserName.Equals(UserName));
            if (user == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}