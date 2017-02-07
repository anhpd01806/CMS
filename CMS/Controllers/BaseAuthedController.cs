using CMS.Bussiness;
using CMS.Data;
using CMS.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebBackendPlus.Controllers
{
    public class BaseAuthedController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Check login 
            var isLogin = (Session["SS-USERID"] != null);
            if (!isLogin)
            {
                filterContext.Result = new RedirectResult("~/account/login");
                return;
            }

            string urlCurrent = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            if (urlCurrent == "/")
            {
                urlCurrent = "/Home/Index";
            }
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            string controller = "";
            string action = "";
            GetInfoAuth(urlCurrent, ref controller, ref action);
            string urlFirst = GetMenuList(urlCurrent);
            if (!new RoleAccessBussiness().IsPermission(controller, action, userId) && urlCurrent != "/Home/Index")
            {
                filterContext.Result = new RedirectResult("~/page/index");
                return;
            }
            else if (!new RoleAccessBussiness().IsPermission(controller, action, userId) && urlCurrent == "/Home/Index")
            {
                filterContext.Result = new RedirectResult("~" + urlFirst);
                return;
            }


            string titleForm = "";
            ViewBag.BreadScrumb = GetBreadCrumb(controller, action, ref titleForm);
            ViewBag.TitleForm = titleForm;
            var userInfo = (User)Session["SS-USER"];
            ViewBag.FullName = Session["SS-FULLNAME"] != null? Session["SS-FULLNAME"] : userInfo.FullName != null ? userInfo.FullName : userInfo.UserName;
        }

        /// <summary>
        /// Load menu when login
        /// </summary>
        /// <param name="urlCurrent"></param>
        /// <returns></returns>
        public string GetMenuList(string urlCurrent)
        {
            int userId = Convert.ToInt32(Session["SS-USERID"]);
            var allMenus = new CacheBussiness().CacheAllMenus();
            List<MenuViewModel> listMenu = new List<MenuViewModel>();
            List<Operation> listMenuAction = new OperationBussiness().GetMenuByUserId(userId).ToList();
            foreach (var menuItem in allMenus)
            {
                Operation operation = listMenuAction.OrderBy(m => m.Orders).FirstOrDefault(m => m.MenuId == menuItem.Id);
                // Check permission
                if (operation != null)
                {
                    string classSubMenu = "";
                    if (urlCurrent.ToUpper() == ("/" + operation.Controller + "/" + operation.Action).ToUpper())
                    {
                        classSubMenu = "active";
                    }
                    //Add menu child
                    listMenu.Add(new MenuViewModel
                    {
                        Id = menuItem.Id,
                        Name = menuItem.Name,
                        Orders = menuItem.Orders ?? 0,
                        CssClass = menuItem.CssClass,
                        ParentId = menuItem.ParentId,
                        Url = "/" + operation.Controller + "/" + operation.Action,
                        ClassSubMenu = classSubMenu
                    });
                    //Find parent and add to list
                    var listMenuParent = GetMenuParent(allMenus, menuItem.ParentId);
                    listMenu.AddRange(listMenuParent.Where(m => !listMenu.Select(lm => lm.Id.ToString()).Contains(m.Id.ToString())));
                    if (classSubMenu != "")
                    {
                        SetActiveMenu(ref listMenu, menuItem.ParentId ?? 0, "active open");
                    }

                    // Remove item of allmenu exsist in listMenu
                    // Saving ram 
                    allMenus = allMenus.Where(m => !listMenu.Select(lm => lm.Id.ToString()).Contains(m.Id.ToString())).ToList();
                }
            }
            ViewBag.PatialMenus = listMenu.OrderBy(m => m.Orders).ToList();
            ViewBag.ListMenuAction = listMenuAction;
            string urlFirst = listMenu.OrderBy(m => m.Orders).FirstOrDefault().Url;
            return urlFirst;
        }

        /// <summary>
        /// set active
        /// </summary>
        /// <param name="listMenu"></param>
        /// <param name="parentId"></param>
        /// <param name="classSubMenuParent"></param>
        public void SetActiveMenu(ref List<MenuViewModel> listMenu, int parentId, string classSubMenuParent)
        {
            if (parentId != 0)
            {
                var firstOrDefault = listMenu.FirstOrDefault(m => m.Id == parentId);
                if (firstOrDefault != null)
                {
                    firstOrDefault.ClassSubMenu = classSubMenuParent;
                    if (firstOrDefault.ParentId != 0)
                    {
                        SetActiveMenu(ref listMenu, firstOrDefault.ParentId ?? 0, classSubMenuParent);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get menu parent
        /// </summary>
        /// <param name="allMenus"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<MenuViewModel> GetMenuParent(List<Menus> allMenus, int? parentId)
        {
            var parentList = new List<MenuViewModel>();
            foreach (var menuItem in allMenus)
            {
                if (menuItem.Id == parentId)
                {
                    parentList.Add(new MenuViewModel
                    {
                        Id = menuItem.Id,
                        Name = menuItem.Name,
                        Orders = menuItem.Orders ?? 0,
                        CssClass = menuItem.CssClass,
                        ParentId = menuItem.ParentId,
                    });
                    if (menuItem.ParentId != 0)
                    {
                        parentList.AddRange(GetMenuParent(allMenus, menuItem.ParentId));
                    }
                }

            }
            return parentList;
        }

        /// <summary>
        /// return titleForm + BreadCrumb
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="titleForm"></param>
        /// <returns></returns>
        protected string GetBreadCrumb(string controller, string action, ref string titleForm)
        {
            try
            {
                string breadCrumb = "";
                var allMenus = new CacheBussiness().CacheAllMenus();
                var operationList = new CacheBussiness().CacheOperation();
                var operation = operationList.FirstOrDefault(op => op.Controller.ToLower() == (controller.ToLower()) && op.Action.ToLower() == (action.ToLower()));
                var defaultOperation = new CacheBussiness().CacheOperation().Where(op => op.Controller.ToLower().Contains(controller.ToLower())).OrderBy(op => op.Orders).FirstOrDefault();
                if (operation != null)
                {
                    var menu = allMenus.FirstOrDefault(m => m.Id == operation.MenuId);

                    var ListOperation = operationList.Where(op => op.MenuId == menu.Id).ToList();
                    // Neu operation la duy nhat thi load luon ten chuc nang do,hoặc trung với default
                    if (ListOperation != null && ListOperation.Count == 1 || (ListOperation.Count > 1 && operation.Id == defaultOperation.Id))
                    {
                        titleForm = menu.Name;
                        if (menu.ParentId != 0)
                        {
                            breadCrumb = "<li class='active'>" + menu.Name + "</li>";
                            while (menu.ParentId != 0)
                            {
                                menu = allMenus.FirstOrDefault(m => m.Id == menu.ParentId);
                                if (menu.ParentId != 0)
                                {
                                    breadCrumb = "<li ><a href = '#' >" + menu.Name + "</a></li>" + breadCrumb;
                                }
                                else
                                {
                                    breadCrumb = "<li ><i class='ace-icon " + menu.CssClass + "'></i> <a href = '#' >" + menu.Name + "</a></li>" + breadCrumb;
                                }
                            }
                        }
                        else
                        {
                            breadCrumb = "<li class='active'><i class='ace-icon " + menu.CssClass + "'></i>" + menu.Name + "</li>";
                        }

                    }
                    else
                    {
                        breadCrumb = "<li class='active'>" + operation.Name + "</li>";
                        titleForm = operation.Name;
                        if (menu.ParentId != 0)
                        {
                            breadCrumb = "<li ><a href = '/" + defaultOperation.Controller + "/" + defaultOperation.Action + "' >" + menu.Name + "</a></li>" + breadCrumb;
                            while (menu.ParentId != 0)
                            {
                                menu = allMenus.FirstOrDefault(m => m.Id == menu.ParentId);
                                if (menu.ParentId != 0)
                                {
                                    breadCrumb = "<li><a href = '#' >" + menu.Name + "</a></li>" + breadCrumb;
                                }
                                else
                                {
                                    breadCrumb = "<li><i class='ace-icon " + menu.CssClass + "'></i> <a href = '#' >" + menu.Name + "</a></li>" + breadCrumb;
                                }
                            }
                        }
                        else
                        {
                            breadCrumb = "<li ><i class='ace-icon " + menu.CssClass + "'></i> <a href = '/" + defaultOperation.Controller + "/" + defaultOperation.Action + "' >" + menu.Name + "</a></li>" + breadCrumb;
                        }
                    }

                }
                return breadCrumb;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
        
        /// <summary>
        /// return controller + action
        /// </summary>
        /// <param name="urlCurrent"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        protected void GetInfoAuth(string urlCurrent, ref string controller, ref string action)
        {
            string[] array = urlCurrent.Split('/');

            if (array.Length > 2)
            {
                controller = array[1];
                action = array[2];
            }
            if (array.Length == 2)
            {
                if (array[0] == "" && array[1] == "")
                {
                    controller = "Home";
                    action = "Index";
                }
                else if (array[0] == "" && array[1] != "")
                {
                    controller = array[1];
                    action = "Index";
                }
            }
        }
    }
}