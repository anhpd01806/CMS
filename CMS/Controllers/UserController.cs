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
            //int totalpage = 0;
            //model.UserList = GetUserList(ref totalpage, 1, 1,"");
            model.Totalpage = new UserBussiness().GetAllUser().Count;
            return View(model);
        }

        [HttpPost]
        public JsonResult LoadData(string search, string pageIndex)
        {
            try
            {
                int totalpage = 0;
                UserViewModel model = new UserViewModel();
                model.UserList = GetUserList(ref totalpage, int.Parse(pageIndex), 20, search);
                var content = RenderPartialViewToString("~/Views/User/UserDetail.cshtml", model.UserList);
                model.Totalpage = totalpage;
                return Json(new
                {
                    TotalPage = model.Totalpage,
                    Content = content
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    TotalPage = 0
                }, JsonRequestBehavior.AllowGet);
            }
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
                    IsMember = true,
                    IsFree = true
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
                        TempData["Error"] = ex;
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
                if (userId != 1)
                {
                    new UserBussiness().Delete(userId);
                    TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
                }else TempData["Error"] = "Bạn không thể xóa người dùng này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
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
                var rs = new UserBussiness().ChangePassword(userId, model.NewPassword, model.OldPassword);
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

        public ActionResult UserInformation()
        {
            var userId = int.Parse(Session["SS-USERID"].ToString());

            UserModel model = new UserModel();
            var user = new UserBussiness().GetUserById(userId);
            model.UserName = user.UserName;
            model.FullName = user.FullName;
            model.Sex = user.Sex ?? false;
            model.Phone = user.Phone;
            model.Email = user.Email;
            model.Id = user.Id;
            return View(model);
        }

        [HttpPost]
        public ActionResult UserInformation(UserModel model)
        {
            new UserBussiness().UpdateProfile(model);
            TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
            return RedirectToAction("UserInformation", "User");
        }

        #region Private funtion
        private string GetNameRole(List<Role> role, List<Role_User> roleUser, int userId)
        {
            var rs = (from r in role
                      join ru in roleUser on r.Id equals ru.RoleId
                      where ru.UserId == userId
                      select r.Name).ToArray();
            return String.Join(", ", rs);
        }

        private List<UserModel> GetUserList(ref int pageTotal, int pageIndex, int pageSize, string search)
        {
            var allUser = new UserBussiness().GetAllUser().Where(x => x.UserName.Contains(search)).ToList();
            var allRoles = new RoleBussiness().GetRoles();
            var allRolesUser = new RoleUserBussiness().GetAllRoleUser();
            pageTotal = (int)Math.Ceiling((double)allUser.Count / (double)pageSize);
            return (from a in allUser
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
                    }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }
        #endregion
    }
}