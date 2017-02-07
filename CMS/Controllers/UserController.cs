using CMS.Bussiness;
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
            model.UserList = (from a in allUser
                              select new UserModel
                              {
                                  Id = a.Id,
                                  FullName = a.FullName,
                                  UserName = a.UserName,
                                  Phone = a.Phone,
                                  Email = a.Email,
                                  IsMember = a.IsMember ?? false,
                                  ManagerBy = a.ManagerBy != null ? allUser.Where(x=>x.Id == a.ManagerBy).Select(x=>x.FullName).FirstOrDefault() : ""
                              }).ToList();

            return View(model);
        }
    }
}