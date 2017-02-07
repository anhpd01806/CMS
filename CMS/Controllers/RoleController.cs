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
    public class RoleController : BaseAuthedController
    {

        /// <summary>
        /// Get role
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            RoleViewModel model = new RoleViewModel();
            model.RoleList = new RoleBussiness().GetRoles();
            return View(model);
        }

        /// <summary>
        /// Create role
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Save create role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(RoleViewModel role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var _role = new Role
                    {
                        Name = role.Name,
                    };
                    new RoleBussiness().Insert(_role);
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Create", "Role");
        }

        /// <summary>
        /// Edit role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            try
            {
                Role role = new RoleBussiness().GetById(id);

                if (role == null)
                {
                    return HttpNotFound();
                }
                RoleViewModel roleViewModel = new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name,
                };
                return View("Edit", roleViewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại";
                return RedirectToAction("Index", "Role");
            }

        }

        /// <summary>
        /// Save edit role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(RoleViewModel role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rolenew = new Role
                    {
                        Name = role.Name,
                        Id = role.Id,
                    };
                    new RoleBussiness().Update(rolenew);
                    TempData["Success"] = "Cập nhật thông tin nhóm quyền thành công.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return Redirect("~/Role/Index");
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            try
            {

                if (id != null)
                {
                    var roleinfo = new RoleBussiness().GetById(id);
                    new RoleBussiness().Delete(roleinfo);
                    TempData["Success"] = "Xóa thông tin nhóm quyền thành công.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return Redirect("~/Role/Index");
        }
    }
}