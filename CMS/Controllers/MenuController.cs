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
    public class MenuController : BaseAuthedController
    {
        /// <summary>
        /// Get menu
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            MenuOperationViewModel model = new MenuOperationViewModel();
            model.MenuList = new CacheBussiness().CacheAllMenus();
            model.OperationList = new CacheBussiness().CacheOperation();
            return View(model);
        }

        /// <summary>
        /// Create menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Create(int? id)
        {
            BindMenuParent(id);

            return View();
        }

        /// <summary>
        /// Insert menu
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(MenuViewModel menu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var u = new Menus()
                    {
                        Name = menu.Name,
                        Orders = menu.Orders,
                        CssClass = menu.CssClass ?? "",
                        ParentId = menu.ParentId,
                        IsActive = true
                    };
                    new MenuBussiness().Insert(u);

                    //update cache when menu changed
                    new CacheBussiness().UpdateCacheAllMenu();
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                    return RedirectToAction("Create", "Menu");

                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return View();
        }

        /// <summary>
        /// Create operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateOperation(int? id)
        {
            MenuOperationViewModel model = new MenuOperationViewModel();
            model.SelectListItem = new MenuBussiness().GetSelectChildMenus().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            model.Operation = new OperationViewModel { MenuId = id ?? 0 };
            return View(model);
        }

        /// <summary>
        /// Insert operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOperation(MenuOperationViewModel model)
        {
            try
            {
                var id = model.Operation.MenuId;
                if (ModelState.IsValid)
                {
                    var op = new Operation
                    {
                        Orders = model.Operation.Order,
                        Name = model.Operation.Name,
                        Controller = model.Operation.Controller,
                        Action = model.Operation.Action,
                        MenuId = model.Operation.MenuId
                    };

                    new OperationBussiness().Insert(op);

                    // update cache when operation changed
                    new CacheBussiness().UpdateCacheOperation();
                    TempData["Success"] = Messages_Contants.SUCCESS_INSERT;
                    ModelState.Clear();
                    model = new MenuOperationViewModel();
                    model.SelectListItem = new MenuBussiness().GetSelectChildMenus().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    model.Operation = new OperationViewModel { MenuId = id};
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return View(model);
        }

        /// <summary>
        /// Edit Menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            var menu = new MenuBussiness().FindById(id);
            var menuViewModel = new MenuViewModel();
            if (menu == null) return View("Edit", menuViewModel);
            BindMenuParent(menu.ParentId);
            menuViewModel = new MenuViewModel
            {
                Id = menu.Id,
                Name = menu.Name,
                Orders = menu.Orders ?? 0,
                CssClass = menu.CssClass,
                ParentId = menu.ParentId
            };
            return View("Edit", menuViewModel);
        }

        /// <summary>
        /// Save edit
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(MenuViewModel menu)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var u = new Menus()
                    {
                        Id = menu.Id,
                        Name = menu.Name,
                        Orders = menu.Orders,
                        CssClass = menu.CssClass,
                        ParentId = menu.ParentId
                    };
                    new MenuBussiness().Update(u);

                    //update cache when menu changed
                    new CacheBussiness().UpdateCacheAllMenu();
                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                    return RedirectToAction("Index", "Menu");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return View("Index");
        }

        /// <summary>
        /// Edit operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditOperation(string id)
        {

            int operationId = Convert.ToInt32(id);
            Operation operation = new OperationBussiness().GetById(operationId);
            if (operation != null)
            {
                OperationViewModel operationViewModel = new OperationViewModel
                {
                    Id = operationId,
                    Name = operation.Name,
                    Action = operation.Action,
                    Controller = operation.Controller,
                    MenuId = operation.MenuId ?? 0,
                    Order = operation.Orders ?? 0,
                };
                ViewBag.Menus = new MenuBussiness().GetSelectChildMenus().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                return View("EditOperation", operationViewModel);
            }
            else
            {
                return View("Index");
            }
        }

        /// <summary>
        /// Save edit operation
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditOperation(OperationViewModel operation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var op = new Operation
                    {
                        Id = operation.Id,
                        Orders = operation.Order,
                        Name = operation.Name,
                        Controller = operation.Controller,
                        Action = operation.Action,
                        MenuId = operation.MenuId
                    };
                    new OperationBussiness().Update(op);

                    //update cache when opeation changed
                    new CacheBussiness().UpdateCacheOperation();
                    TempData["Success"] = Messages_Contants.SUCCESS_UPDATE;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Index", "Menu");
        }

        /// <summary>
        /// Permission
        /// </summary>
        /// <returns></returns>
        public ActionResult Permission()
        {
            PermissionViewModel permission = new PermissionViewModel
            {
                Roles = new RoleBussiness().GetRoles().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList(),
                MenuChilds = new MenuBussiness().GetChildMenus(),
                Operations = new OperationBussiness().GetOperations()
            };

            if (permission.Roles != null)
            {
                int roleId = Convert.ToInt32(permission.Roles.ElementAt(0).Value);
                permission.RoleId = permission.Roles.ElementAt(0).Value;
                permission.RoleAccess = new RoleAccessBussiness().GetOperationInRole(roleId);
            }

            permission.MenuView = new List<MenuViewModel>();
            if (permission.MenuChilds != null)
            {
                foreach (var menuChild in permission.MenuChilds)
                {
                    MenuViewModel menuViewModel = new MenuViewModel
                    {
                        Name = menuChild.Name,
                        CssClass = menuChild.CssClass,
                        Id = menuChild.Id,
                        Orders = menuChild.Orders ?? 0,
                        ParentId = menuChild.ParentId,
                        Operation = new List<OperationViewModel>()
                    };
                    if (permission.Operations != null)
                    {
                        foreach (var operation in permission.Operations)
                        {
                            if (operation.MenuId == menuChild.Id)
                            {
                                OperationViewModel operationModel = new OperationViewModel
                                {
                                    Id = operation.Id,
                                    MenuId = operation.MenuId ?? 0,
                                    Name = operation.Name,
                                    Controller = operation.Controller,
                                    Action = operation.Action,
                                };
                                if (permission.RoleAccess != null)
                                {
                                    foreach (var roleAccess in permission.RoleAccess)
                                    {
                                        if (roleAccess.OperationId == operation.Id)
                                        {
                                            operationModel.IsSelected = true;
                                        }
                                    }
                                }
                                menuViewModel.Operation.Add(operationModel);
                            }
                        }
                    }
                    permission.MenuView.Add(menuViewModel);
                }
            }


            return View("Permission", permission);
        }

        /// <summary>
        /// Role permission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RolePermission(string id)
        {
            PermissionViewModel permission = new PermissionViewModel
            {
                Roles = new RoleBussiness().GetRoles().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList(),
                MenuChilds = new MenuBussiness().GetChildMenus(),
                Operations = new OperationBussiness().GetOperations()
            };
            if (permission.Roles != null)
            {
                int roleId = Convert.ToInt32(id);
                permission.RoleId = id;
                permission.RoleAccess = new RoleAccessBussiness().GetOperationInRole(roleId);
            }
            permission.MenuView = new List<MenuViewModel>();
            if (permission.MenuChilds != null)
            {
                foreach (var menuChild in permission.MenuChilds)
                {
                    MenuViewModel menuViewModel = new MenuViewModel
                    {
                        Name = menuChild.Name,
                        CssClass = menuChild.CssClass,
                        Id = menuChild.Id,
                        Orders = menuChild.Orders ?? 0,
                        ParentId = menuChild.ParentId,
                        Operation = new List<OperationViewModel>()
                    };
                    if (permission.Operations != null)
                    {
                        foreach (var operation in permission.Operations)
                        {
                            if (operation.MenuId == menuChild.Id)
                            {
                                OperationViewModel operationModel = new OperationViewModel
                                {
                                    Id = operation.Id,
                                    MenuId = operation.MenuId ?? 0,
                                    Name = operation.Name,
                                    Controller = operation.Controller,
                                    Action = operation.Action,
                                };
                                if (permission.RoleAccess != null)
                                {
                                    foreach (var roleAccess in permission.RoleAccess)
                                    {
                                        if (roleAccess.OperationId == operation.Id)
                                        {
                                            operationModel.IsSelected = true;
                                        }
                                    }
                                }
                                menuViewModel.Operation.Add(operationModel);
                            }
                        }
                    }
                    permission.MenuView.Add(menuViewModel);
                }
            }
            return View("Permission", permission);
        }

        /// <summary>
        /// Remove permission
        /// </summary>
        /// <param name="operationid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("RemovePermission")]
        public ActionResult RemovePermission(string operationid, string roleid)
        {
            try
            {
                Role_Access roleAccess = new Role_Access
                {
                    RoleId = Convert.ToInt32(roleid),
                    OperationId = Convert.ToInt32(operationid),
                };
                new RoleAccessBussiness().Delete(roleAccess);

                new CacheBussiness().UpdateCacheRoleAccess();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return Json(new
            {
                status = "success",
                result = "Done"
            });
        }

        /// <summary>
        /// Add permission
        /// </summary>
        /// <param name="operationid"></param>
        /// <param name="roleid"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AddPermission")]
        public ActionResult AddPermission(string operationid, string roleid)
        {
            try
            {
                Role_Access groupUserAccess = new Role_Access
                {
                    RoleId = Convert.ToInt32(roleid),
                    OperationId = Convert.ToInt32(operationid),
                };
                new RoleAccessBussiness().Insert(groupUserAccess);

                new CacheBussiness().UpdateCacheRoleAccess();
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return Json(new
            {
                status = "success",
                result = "Done"
            });
        }

        /// <summary>
        /// Delete menu
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            try
            {
                new MenuBussiness().Delete(id);

                //Update cache when delete menu
                new CacheBussiness().UpdateCacheAllMenu();
                TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Index", "Menu");
        }

        /// <summary>
        /// Delete opeartion
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveOperation(int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new OperationBussiness().Delete(id);

                    //Update cache when delete opeation
                    new CacheBussiness().UpdateCacheOperation();
                    TempData["Success"] = Messages_Contants.SUCCESS_DELETE;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = Messages_Contants.ERROR_COMMON;
            }
            return RedirectToAction("Index", "Menu");
        }


        #region Private funtion
        private void BindMenuParent(int? parentId)
        {
            var listMenu = new MenuBussiness().GetLstMenuParent().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            listMenu.Insert(0, new SelectListItem { Value = "0", Text = "Chọn chức năng" });
            ViewBag.MenuList = listMenu;
            ViewBag.ParentId = parentId;
        }

        #endregion
    }
}