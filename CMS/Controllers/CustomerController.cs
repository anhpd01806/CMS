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
    public class CustomerController : BaseAuthedController
    {
        // GET: Customer
        public ActionResult Index()
        {
            UserViewModel model = new UserViewModel();
            //int totalpage = 0;
            var managerList = new UserBussiness().GetManagerUser();
            managerList.Add(new SelectListItem { Text = "Tất cả", Value = "-1" });
            model.ManagerList = managerList.ToList();
            model.ManagerId = int.Parse(Session["SS-USERID"].ToString());
            model.Totalpage = (int)Math.Ceiling((double)new UserBussiness().GetCustomerUser(model.ManagerId).Count / (double)20);
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(UserModel model)
        {
            new UserBussiness().UpdateCustomer(model); 
            return RedirectToAction("Index","Notice");
        }

        [HttpPost]
        public JsonResult LoadData(string search, string pageIndex,string managerId)
        {
            try
            {
                int totalpage = 0;
                int manId = int.Parse(managerId);
                UserViewModel model = new UserViewModel();
                model.UserList = GetCustomerList(ref totalpage, int.Parse(pageIndex), 20, search, manId);
                var content = RenderPartialViewToString("~/Views/Customer/CustomerDetail.cshtml", model.UserList);
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
                    Phone = model.UserName,
                    Email = "",
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

        public ActionResult Edit(int id)
        {
            UserModel model = new UserModel();

            //get user by userId
            var user = new UserBussiness().GetUserById(id);
            model.Id = user.Id;
            model.UserName = user.UserName;
            model.FullName = user.FullName;
            model.IsMember = user.IsMember ?? false;
            model.IsRestore = user.IsDeleted ?? false;
            model.ManagerBy = user.ManagerBy + "";
            model.RoleUsers = new RoleBussiness().GetListByUserId(id);

            //get dropdownlist and list role
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
                    if (model.RoleUsers.FirstOrDefault(r => r.RoleId == roleView.Id) != null)
                    {
                        roleView.IsChecked = true;
                    }
                    lstRoles.Add(roleView);
                }
                model.ListRoles = lstRoles;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UserModel model, string[] selectRoles)
        {
            try
            {
                //update ismember and delete
                new UserBussiness().Update(model);

                //delte all role in role_users
                new RoleBussiness().DeleteRoleUserByUserId(model.Id);

                //insert role to role_user
                List<Role_User> lstRoleUser = new List<Role_User>();
                if (selectRoles != null)
                {
                    lstRoleUser.AddRange(selectRoles.Select(roleId => new Role_User
                    {
                        UserId = model.Id,
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
                        return RedirectToAction("Index", "User");
                    }
                }

                TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
            }
            catch (Exception)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
                return RedirectToAction("Index", "User");
            }

            return RedirectToAction("Index", "User");
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
                }
                else TempData["Error"] = "Bạn không thể xóa người dùng này.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex;
            }

            return RedirectToAction("Index", "User");
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

        public ActionResult DetailInformation()
        {
            return View();
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

        //get all khách hàng
        private List<UserModel> GetCustomerList(ref int pageTotal, int pageIndex, int pageSize, string search,int managerId)
        {
            var admin = new UserBussiness().GetAdminUser().ToList();
            var allUser = new UserBussiness().GetCustomerUser(managerId).Where(x => x.UserName.Contains(search)).ToList();
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
                        IsDelete = a.IsDeleted ?? false,
                        IsMember = a.IsMember ?? false,
                        ManagerBy = a.ManagerBy != null ? admin.Where(x => x.Id == a.ManagerBy).Select(x => x.FullName).FirstOrDefault() : "",
                        RoleName = GetNameRole(allRoles, allRolesUser, a.Id),
                        IsOnline = CheckCustomerOnline(a.Id)
                    }).OrderBy(x => x.IsMember).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        private Boolean CheckCustomerOnline(int userId)
        {
            var currentApp = System.Web.HttpContext.Current.Application["usr_" + userId];

            if (currentApp != null)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}